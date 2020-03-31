using Facebook.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;

namespace Facebook
{
    public class Payload
    {
        public string message { get; set; }
        public string url { get; set; }
    }

    public class FacebookClient
    {
        // ReSharper disable once TooManyArguments
        public static void PostToPage(Payload data, ILogger logger, IHttpClient httpClient, PluginConfiguration config)
        {
            logger.Info("Begin Facebook Post");
            httpClient.Post(new HttpRequestOptions()
            {
                Url = "https://graph.facebook.com/v2.3/" + "me/photos" + "?url=" + data.url + "&message=" + data.message + "&access_token=" + config.accessToken
            });
            logger.Info("Facebook Post complete");
        }

    }
}
