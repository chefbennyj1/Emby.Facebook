using System;
using System.Threading;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Data;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace Facebook
{
    public class ServerEntryPoint : IServerEntryPoint
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private static IJsonSerializer JsonSerializer { get; set; }
        private ISessionManager SessionManager        { get; }
        private ILogger Logger                        { get; }
        private IHttpClient HttpClient                { get; }
        private ILogManager LogManager                { get; }
        private IServerApplicationHost Host           { get; }
        private string WanAddress                     { get; set; }
        private IUserDataManager UserManager          { get; }
        private ILibraryManager LibraryManager        { get; }

        private const long IntroOrVideoBackDrop = 3000000000L;
        // ReSharper disable once TooManyDependencies
        public ServerEntryPoint(IJsonSerializer json, ISessionManager ses, ILogManager log, IHttpClient client, IServerApplicationHost host, IUserDataManager userMan, ILibraryManager lib)
        {
            JsonSerializer = json;
            SessionManager = ses;
            LogManager = log;
            Logger = LogManager.GetLogger(Plugin.Instance.Name);
            HttpClient = client;
            Host = host;
            UserManager = userMan;
            LibraryManager = lib;
            
        }

        

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        // ReSharper disable once MethodNameNotMeaningful
        public void Run()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
            
            WanAddress = Host.GetPublicSystemInfo(CancellationToken.None).Result.WanAddress;
            
            //Server Events 
            UserManager.UserDataSaved += UserManager_UserDataSaved;
            SessionManager.PlaybackStart += SessionManager_PlaybackStart;
            LibraryManager.ItemAdded += LibraryManager_ItemAdded;

        }

        private void LibraryManager_ItemAdded(object sender, ItemChangeEventArgs e)
        {
            var config = Plugin.Instance.Configuration;
            
            // ReSharper disable once TooManyChainedReferences
            var item = e.Item.GetType().Name == "Episode" ? LibraryManager.GetItemById(e.Item.Parent.Parent.InternalId) : e.Item;

            var data = new Payload
            {
                message  = $"New {e.Item.GetType().Name} available: {item.Name}! Watch the trailer now!",
                endpoint = "me/feed",
                link     = e.Item.RemoteTrailers[0].Url
            };

            FacebookClient.PostToPage(data, Logger, HttpClient, config);
        }

        private void UserManager_UserDataSaved(object sender, UserDataSaveEventArgs e)
        {
            if (e.SaveReason != UserDataSaveReason.UpdateUserRating) return;
            if (!e.Item.IsFavoriteOrLiked(e.User)) return;
            var config = Plugin.Instance.Configuration;

            // ReSharper disable once TooManyChainedReferences
            var item = e.Item.GetType().Name == "Episode" ? LibraryManager.GetItemById(e.Item.Parent.Parent.InternalId) : e.Item;
            // ReSharper disable once ComplexConditionExpression
            var message = $"{e.User.Name} is likes the { e.Item.GetType().Name}:  {e.Item.Name} " + e.Item.GetType().Name == "Episode" ? $" from the series {item.Name}" : "";

            var data = new Payload
            {
                message = message,
                url = $"{WanAddress}/emby/Items/{item.InternalId}/Images/Primary?maxHeight=1108&amp;maxWidth=800&amp;quality=90",
                endpoint = "me/photos"
            };
            
            FacebookClient.PostToPage(data, Logger, HttpClient, config);
        }

        private void SessionManager_PlaybackStart(object sender, PlaybackProgressEventArgs e)
        {
            // ReSharper disable once ComplexConditionExpression
            if (e.MediaInfo.RunTimeTicks != null && (e.Item.MediaType == MediaType.Video && e.MediaInfo.RunTimeTicks.Value < IntroOrVideoBackDrop)) return;

            var config = Plugin.Instance.Configuration;
            
            // ReSharper disable TooManyChainedReferences
            var item    = e.Item.GetType().Name == "Episode" ? LibraryManager.GetItemById(e.Item.Parent.ParentId) : e.Item;
            // ReSharper disable once ComplexConditionExpression
            var message = $"{e.Session.UserName} is watching the {e.MediaInfo.Type}:  {e.Item.Name} " + e.MediaInfo.Type == "Episode" ? $" from the series {item.Name}" : "";

            var data = new Payload
            {
                message = message,
                url = $"{WanAddress}/emby/Items/{item.InternalId}/Images/Primary?maxHeight=1108&amp;maxWidth=800&amp;quality=90",
                endpoint = "me/photos"
            };
            
            FacebookClient.PostToPage(data, Logger, HttpClient, config);
        }
    }
}
