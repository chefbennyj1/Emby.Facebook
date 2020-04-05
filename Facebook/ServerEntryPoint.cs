using System;
using System.Threading;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
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
        // ReSharper disable ComplexConditionExpression
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable TooManyDependencies
        // ReSharper disable MethodNameNotMeaningful

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
        
        public ServerEntryPoint(IJsonSerializer json, ISessionManager ses, ILogManager log, IHttpClient client, 
                                IServerApplicationHost host, IUserDataManager userMan, ILibraryManager lib)
        {
            JsonSerializer = json;
            SessionManager = ses;
            LogManager     = log;
            Logger         = LogManager.GetLogger(Plugin.Instance.Name);
            HttpClient     = client;
            Host           = host;
            UserManager    = userMan;
            LibraryManager = lib;
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        
        public void Run()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
            
            WanAddress = Host.GetPublicSystemInfo(CancellationToken.None).Result.WanAddress;
            
            //Server Events 
            UserManager.UserDataSaved    += UserManager_UserDataSaved;
            SessionManager.PlaybackStart += SessionManager_PlaybackStart;
            LibraryManager.ItemAdded     += LibraryManager_ItemAdded;

        }

        private static string LastItemAdded = string.Empty;
       
        private readonly Func<BaseItem, bool> ShouldPost = item =>
        {
            switch (item.GetType().Name)
            {
                case "Episode":
                    var nameItemAdded = LastItemAdded;
                    LastItemAdded = item.Parent.Parent.Name;
                    return item.Parent.Parent.Name != nameItemAdded;
                case "Movie":
                    return true;
                default:
                    return false;
            }
        };
        
        private void LibraryManager_ItemAdded(object sender, ItemChangeEventArgs e)
        {
            var config = Plugin.Instance.Configuration;
            var type   = e.Item.GetType();
            if (!ShouldPost(e.Item)) return;
           
            var message = $"New {type.Name} available: {e.Item.Name} "; 
                          
            var data = new Payload
            {
                message  = message,
                endpoint = "me/feed",
                link     = e.Item.RemoteTrailers.Length == 0  ? null : e.Item.RemoteTrailers[0].Url
            };
            
            //FacebookClient.PostToPage(data, Logger, HttpClient, config);
        }

        private void UserManager_UserDataSaved(object sender, UserDataSaveEventArgs e)
        {
            
            if (e.SaveReason != UserDataSaveReason.UpdateUserRating) return;
            if (!e.Item.IsFavoriteOrLiked(e.User)) return;

            var config  = Plugin.Instance.Configuration;
            if (!config.UserPostsOptIn.Contains(e.User.Id)) return;

            var type    = e.Item.GetType();
            var item    = type.Name == "Episode" ? LibraryManager.GetItemById(e.Item.Parent.Parent.InternalId) : e.Item;
            var message = $"{e.User.Name} likes the {type.Name}: {e.Item.Name} ";

            message += type.Name == "Episode" ? $" from the series {item.Name}" : "";

            var data = new Payload
            {
                message  = message,
                url      = $"{WanAddress}/emby/Items/{item.InternalId}/Images/Primary?maxHeight=1108&amp;maxWidth=800&amp;quality=90",
                endpoint = "me/photos"
            };
            
            FacebookClient.PostToPage(data, Logger, HttpClient, config);
        }

        private void SessionManager_PlaybackStart(object sender, PlaybackProgressEventArgs e)
        {
            // ReSharper disable once ComplexConditionExpression
            if (e.MediaInfo.RunTimeTicks != null && (e.Item.MediaType == MediaType.Video && e.MediaInfo.RunTimeTicks.Value < IntroOrVideoBackDrop)) return;

            var config  = Plugin.Instance.Configuration;

            if (!config.UserPostsOptIn.Exists(id => id.ToString().Replace("-",string.Empty) == e.Session.UserId)) return; //Somethings wrong here!!

            var type    = e.Item.GetType();
            var item    = type.Name == "Episode" ? LibraryManager.GetItemById(e.Item.Parent.ParentId) : e.Item;
            var message = $"{e.Session.UserName} is watching the {e.MediaInfo.Type}:  {e.Item.Name} ";

            message += e.MediaInfo.Type == "Episode" ? $" from the series {item.Name}" : "";

            var data = new Payload
            {
                message  = message,
                url      = $"{WanAddress}/emby/Items/{item.InternalId}/Images/Primary?maxHeight=1108&amp;maxWidth=800&amp;quality=90",
                endpoint = "me/photos"
            };
            
            FacebookClient.PostToPage(data, Logger, HttpClient, config);
        }
    }
}
