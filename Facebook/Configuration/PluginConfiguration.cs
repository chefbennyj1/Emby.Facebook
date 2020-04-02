using System;
using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Facebook.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string accessToken                        { get; set; }
        public bool enableRecommendationUpdate           { get; set; }
        public bool enableUserWatchUpdate                { get; set; }
        public List<Guid> UserPostsOptIn                 { get; set; }
    }
}
