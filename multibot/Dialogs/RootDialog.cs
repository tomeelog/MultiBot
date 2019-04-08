using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using multibot.Extention;
using multibot.forms;

namespace multibot.Dialogs
{
    [Serializable]
    [LuisModel("2b736283-d98f-4858-b4bc-599c1fa74e3d", "347095405a91411a95c703c451e99e17")]
    public class RootDialog : LuisDialog<object>
    {

        // Request section enum
        private enum select
        {
            upload, request, Inquiry
        }

        private enum Selection
        {
            eTax, eResig, StampDuty
        }
        [LuisIntent("None")]
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

        // Request section// the help desk

        [LuisIntent("helpdesk")]
        public async Task request(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var options = new select[] { select.upload, select.request, select.Inquiry};
            var description = new string[] { "Upload document", "Make a request", "Inquiry" };
            PromptDialog.Choice<select>(context, ResumeAfterRequest,
        options, "Which selection do you want?", descriptions: description);
        }

        // Requst resumption // resume after help desk
        private async Task ResumeAfterRequest(IDialogContext context, IAwaitable<select> result)
        {
            var selection = await result;

            switch (selection) {
                case select.upload:

                    //case select.upload:
                    await context.PostAsync($"this will go to the {selection} dialog.");
                    break;

                case select.request:
                    var myform = new FormDialog<requestForm>(new requestForm(), requestForm.BuildRequestForm, FormOptions.PromptInStart, null);
                    context.Call<requestForm>(myform, RequestFormCompleteCallback);
                    break;

                case select.Inquiry:
                    await context.PostAsync($"this will go to the {selection} dialog.");
                    break;

            }
           
        }

        private Task RequestFormCompleteCallback(IDialogContext context, IAwaitable<requestForm> result)
        {
            throw new NotImplementedException();
        }

        [LuisIntent("cacplatform")]
        public async Task Cacplaform(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            string a = "searching for a service provider to meet your demand..";
            string b = "Welcome to CORPORATE AFFAIRS COMMISION";
            string c = "Sorry this service is currently unavailable at the moment...";
            
            await context.PostAsync(a.ToUserLocale(context));
            await context.PostAsync(b.ToUserLocale(context));
            await context.PostAsync(c.ToUserLocale(context));
        }

        [LuisIntent("firsplatform")]
        public async Task firsplatform(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {   string b = "Welcome to Federal Inland Revenue Service";
            string c = "Select from the list or ask what you want on FIRS";

            var message = await activity;

            string a = "searching for a service provider to meet your demand..";
            string d = "E-tax Payment";
            string e = "E-registration";
            string f = "E-stamp duty";

            await context.PostAsync(a.ToUserLocale(context));
            await context.PostAsync(b.ToUserLocale(context));

            var options = new Selection[] { Selection.eTax, Selection.eResig, Selection.StampDuty };
            var descriptions = new string[] { d.ToUserLocale(context), e.ToUserLocale(context), f.ToUserLocale(context) };
            PromptDialog.Choice<Selection>(context, ResumeAfterSelectionClarificationAsync,
            options, c.ToUserLocale(context), descriptions: descriptions);
        }

        [LuisIntent("verifyReceipt")]
        public async Task verifyReceipt(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult rsult) {
            var message = await activity;
            var response = ChatResponse.note1;
            await context.PostAsync(response.ToUserLocale(context));
            context.Call(new VerifyReceipt(),ResumeAfterRegister);
        }

        [LuisIntent("checkStatus")]
        public async Task checkStatus(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult rsult)
        {
            PromptDialog.Confirm(context, ResumeAfterTcc, "Do you have the code to check?");

        }

        private async Task ResumeAfterTcc(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            if (confirmation)
            {
                await context.PostAsync("Checking");
            }
            else {
                await context.PostAsync("not Checking");
            }

        }

        [LuisIntent("tcc")]
        public async Task tcc(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
         var message = await activity;
                await context.Forward(new RegistertccDialog(), ResumeAfterRegister, message, CancellationToken.None);
        }

        [LuisIntent("verifyTcc")]
        public async Task VerifyTcc(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Verify your Tax Clearance Certificate by providing the details");
            context.Call(new VerifyTCCDialog(), ResumeAfterRegister);
        }
            private async Task ResumeAfterRegister(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterSelectionClarificationAsync(IDialogContext context, IAwaitable<Selection> result)
        {
            var selection = await result;
            await context.PostAsync($"I see you want to order a {selection}.");
        }

        //private async Task CheckStatus(IDialogContext context, IAwaitable<object> result)
        //{
        //    var response = await result as IMessageActivity;
           
        //    Console.WriteLine(response);
        //    await context.PostAsync("checking ur register");
            
        //}
    }
}