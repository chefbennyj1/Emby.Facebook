using Facebook.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;

namespace Facebook
{
    public class Payload
    {
        public string message  { get; set; }
        public string url      { get; set; }
        public string link     { get; set; }
        public string endpoint { get; set; }
    }

    public class FacebookClient
    {
        // ReSharper disable once TooManyArguments
        public static async void PostToPage(Payload data, ILogger logger, IHttpClient httpClient, PluginConfiguration config)
        {
            logger.Info("Begin Facebook Post");

            var link = data.link != null ? $"&link={data.link}" : "";
            var url  = data.url  != null ? $"&url={data.url}"   : "";
            
            await httpClient.Post(new HttpRequestOptions()
            {
                Url = $"https://graph.facebook.com/v2.3/{data.endpoint}?access_token={config.accessToken}{url}&message={data.message}" + link
            });
            
            logger.Info("Facebook Post complete");
        }

    }
}
