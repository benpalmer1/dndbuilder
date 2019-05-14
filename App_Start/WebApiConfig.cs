using System.Web.Http;

namespace TMWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /* Settings defined as per practical 3 submission.
             * Setup here to handle manual routing, use JSON globally and setup timezone format. */

            config.MapHttpAttributeRoutes();

            // Specify JSON formatting
            // i.e Clear default formatting and set to JSON.
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());

            // Ignore circular references when serialising
            Newtonsoft.Json.JsonConvert.DefaultSettings = () =>
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };

            // Time zone handling 
            config.Formatters.JsonFormatter.SerializerSettings.
            DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        }
    }
}