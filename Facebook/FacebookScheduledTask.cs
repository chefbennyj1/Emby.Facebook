using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;

namespace Facebook
{
    public class FacebookScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private ILibraryManager LibraryManager { get; }
        private ITaskManager TaskManager       { get; }
        private ILogger Logger                 { get; }
        private IHttpClient HttpClient         { get; }
        private ILogManager LogManager         { get; }
        private IServerApplicationHost Host    { get; }
        private string WanAddress              { get; set; }

        // ReSharper disable once TooManyDependencies
        public FacebookScheduledTask(ILibraryManager libMan, ITaskManager task, ILogManager log, IHttpClient client, IServerApplicationHost host)
        {
            LibraryManager = libMan;
            TaskManager    = task;
            LogManager     = log;
            Logger         = LogManager.GetLogger(Plugin.Instance.Name);
            HttpClient     = client;
            Host           = host;
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            //var systemInfo = await Host.GetPublicSystemInfo(CancellationToken.None);
            //WanAddress = systemInfo.WanAddress;

            var ids = LibraryManager.GetInternalItemIds(new InternalItemsQuery()
            {
                IncludeItemTypes = new[] {"Movie", "Episode"},
                MinDateCreated = DateTime.Now.AddDays(-7),
                Limit = 8
            });

            if (ids.Any())
            {
                var data = new Payload();
                
                foreach (var id in ids)
                {
                    var item = LibraryManager.GetItemById(id);

                    if (item.GetType().Name == "Episode")
                    {
                        data.message += $"• {item.Name} from the series {item.Parent.Parent.Name}\n";
                        continue;
                    }
                    data.message += $"• {item.Name}\n";
                    //data.url = "https://images.unsplash.com/photo-1564923867983-41d586b10e54?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1350&q=80";
                }

                data.url = "https://images.unsplash.com/photo-1564923867983-41d586b10e54?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1350&q=80";
                data.endpoint = "me/photos";

                FacebookClient.PostToPage(data, Logger, HttpClient, Plugin.Instance.Configuration);
            }
            progress.Report(100.0);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type          = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromDays(7).Ticks
                },
                new TaskTriggerInfo()
                {
                    SystemEvent = SystemEvent.WakeFromSleep,
                    Type        = TaskTriggerInfo.TriggerSystemEvent
                }
            };
        }

        public string Name        => "Facebook recommendation update";
        public string Key         => "Facebook";
        public string Description => "Update Facebook page with recommended media items.";
        public string Category    => "Facebook";
        public bool IsHidden      => false;
        public bool IsEnabled     => true;
        public bool IsLogged      => true;
    }
}
