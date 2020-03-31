using System;
using System.Collections.Generic;
using System.IO;
using Facebook.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Facebook
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasThumbImage, IHasWebPages
    {
        public static Plugin Instance { get; private set; }
        public ImageFormat ThumbImageFormat => ImageFormat.Png;
        
        public override Guid Id => new Guid("55A5756C-F9EA-4E96-ABF4-6360607FBDCD");

        public override string Name => "Facebook";

        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
        }

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths,
            xmlSerializer)
        {
            Instance = this;
        }

        public IEnumerable<PluginPageInfo> GetPages() => new[]
        {
            new PluginPageInfo
            {
                Name                 = "FacebookPluginConfigurationPage",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.FacebookPluginConfigurationPage.html"
            },
            new PluginPageInfo
            {
                Name = "FacebookPluginConfigurationPageJS",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.FacebookPluginConfigurationPage.js"
            }
        };
    }
}
