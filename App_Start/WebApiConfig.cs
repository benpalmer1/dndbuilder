using System.Web.Http;
using Newtonsoft.Json;

namespace TMWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /* Settings defined as per practical 3 submission.
             * Setup here to handle manual routing, use JSON globally and setup timezone format. */

            config.MapHttpAttributeRoutes();

            // JSON SERIALIZER SETTINGS:

            // Specify JSON formatting - clear default formatting and set to JSON.
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());

            // Ignore circular references when serialising
            JsonConvert.DefaultSettings = () =>
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

            // Time zone handling 
            config.Formatters.JsonFormatter.SerializerSettings.
            DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}