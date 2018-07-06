using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace multibot.Utilities
{
    public class Settings
    {
        public static string GetSubscriptionKey()
        {
            return ConfigurationManager.AppSettings["SubscriptionKey"];
        }
        public static string GetCognitiveServicesTokenUri()
        {
            return ConfigurationManager.AppSettings["CognitiveServicesTokenUri"];
        }
        public static string GetTranslatorUri()
        {
            return ConfigurationManager.AppSettings["TranslatorUri"];
        }
    }
}