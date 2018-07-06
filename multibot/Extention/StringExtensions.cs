using Microsoft.Bot.Builder.Dialogs;
using multibot.Translator;
using multibot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace multibot.Extention
{
    public static class StringExtensions
    {

        public static string ToUserLocale(this string text, IDialogContext context)
        {
            context.UserData.TryGetValue(StringConstants.UserLanguageKey, out string userLanguageCode);

            text = TranslationHandler.TranslateText(text, StringConstants.DefaultLanguage, userLanguageCode);

            return text;
        }
    }
}