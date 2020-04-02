using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
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
        private ILogger Logger { get; }
        private IHttpClient HttpClient { get; }
        private ILogManager LogManager { get; }

        // ReSharper disable once TooManyDependencies
        public FacebookScheduledTask(ILibraryManager libMan, ITaskManager task, ILogManager log, IHttpClient client)
        {
            LibraryManager = libMan;
            TaskManager    = task;
            LogManager     = log;
            Logger         = LogManager.GetLogger(Plugin.Instance.Name);
            HttpClient     = client;
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var ids = LibraryManager.GetInternalItemIds(new InternalItemsQuery()
            {
                IncludeItemTypes = new[] {"Movies", "Episodes"},
                Recursive = true,
                MinDateCreated = TaskManager.ScheduledTasks.FirstOrDefault(task => task.Name == Name)
                    ?.LastExecutionResult.EndTimeUtc
            });

            if (ids.Any())
            {
                var message = $"Here's what is new!\n";
                foreach (var id in ids)
                {
                    var item = LibraryManager.GetItemById(id);

                    message += $"{item.Name}\n";

                    var data = new Payload()
                    {
                        message = message
                    };

                    FacebookClient.PostToPage(data, Logger, HttpClient, Plugin.Instance.Configuration);


                }
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
