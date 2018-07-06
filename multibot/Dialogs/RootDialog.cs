using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using multibot.Extention;

namespace multibot.Dialogs
{
    [Serializable]
    [LuisModel("2b736283-d98f-4858-b4bc-599c1fa74e3d", "347095405a91411a95c703c451e99e17")]
    public class RootDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Default;

            await context.PostAsync(response.ToUserLocale(context));

            context.Wait(MessageReceived);
        }

        [LuisIntent("greetings")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Greeting2;

            await context.PostAsync(response.ToUserLocale(context));

            context.Wait(MessageReceived);
        }

        [LuisIntent("cacplatform")]
        public async Task Cacplaform(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            string a = "searching for a service provider to meet your demand..";
            string b = "Found CAC Nigeria";
            await context.PostAsync(a.ToUserLocale(context));
            await context.PostAsync(b.ToUserLocale(context));
        }

        [LuisIntent("firsplatform")]
        public async Task firsplatform(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            string a = "searching for a service provider to meet your demand..";
            string b = "Found Federal Inland Revenue Service";
            await context.PostAsync(a.ToUserLocale(context));
            await context.PostAsync(b.ToUserLocale(context));
        }
    }
}