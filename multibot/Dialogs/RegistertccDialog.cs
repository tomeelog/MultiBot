using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using multibot.Extention;
using multibot.forms;

namespace multibot.Dialogs
{
    [Serializable]
    public class RegistertccDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            string uptext = "Please choose the account type you want to open";
            string reg = "Register";
            string fore = "forex";
            string personal = "individual";
            string noteA = "Register your individual tcc account";
            string noteB = "register corporate tcc account";
            string noteC = "register forex tcc account";
            string cop = "corporate";
            string aa = "Please wait a minuite";
            await context.PostAsync(aa.ToUserLocale(context));
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();

            List<CardImage> cardImages1 = new List<CardImage>();
            List<CardAction> cardButtons = new List<CardAction>();
            cardImages1.Add(new CardImage(url: "https://image.ibb.co/nDUO1o/Individual_Membership.png"));
            CardAction individual = new CardAction()
            {
                Type = "imBack",
                Title = reg.ToUserLocale(context),
                Value = personal.ToUserLocale(context)
            };
            cardButtons.Add(individual);
            HeroCard plCard1 = new HeroCard()
            {
                Title = personal.ToUserLocale(context),
                Subtitle = noteA.ToUserLocale(context),
                Images = cardImages1,
                Buttons = cardButtons
            };
            Attachment plAttachment1 = plCard1.ToAttachment();
            reply.Attachments.Add(plAttachment1);

            List<CardImage> cardImages2 = new List<CardImage>();
            List<CardAction> cardButtons2 = new List<CardAction>();
            cardImages2.Add(new CardImage(url: "https://image.ibb.co/eco0go/Corporate.jpg"));
            CardAction corporate = new CardAction()
            {
                Type = "imBack",
                Title = reg.ToUserLocale(context),
                Value = cop.ToUserLocale(context)
            };
            cardButtons2.Add(corporate);
            HeroCard plCard2 = new HeroCard()
            {
                Title = cop.ToUserLocale(context),
                Subtitle = noteB.ToUserLocale(context),
                Images = cardImages2,
                Buttons = cardButtons2
            };
            Attachment plAttachment2 = plCard2.ToAttachment();
            reply.Attachments.Add(plAttachment2);

            List<CardImage> cardImages3 = new List<CardImage>();
            List<CardAction> cardButtons3 = new List<CardAction>();
            cardImages3.Add(new CardImage(url: "https://image.ibb.co/ii9y1o/forex_transa.jpg"));
            CardAction forex = new CardAction()
            {
                Type = "imBack",
                Title = reg.ToUserLocale(context),
                Value = fore.ToUserLocale(context)
            };
            cardButtons3.Add(forex);
            HeroCard plCard3 = new HeroCard()
            {
                Title = fore.ToUserLocale(context),
                Subtitle = noteC.ToUserLocale(context),
                Images = cardImages3,
                Buttons = cardButtons3
            };
            Attachment plAttachment3 = plCard3.ToAttachment();
            reply.Attachments.Add(plAttachment3);
            reply.Text = uptext.ToUserLocale(context);
            await context.PostAsync(reply);

            context.Wait(SelectionAsync);
        }
        private async Task SelectionAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
                        
            string person = "individual";

            if (activity.Text.ToUserLocale(context) == person.ToUserLocale(context))
            {

                await context.PostAsync("Please enter yout TIN");
                context.Wait(IndividualRegAsync);
            }
            else if (activity.Text.ToLower().ToUserLocale(context) == "corporate")
            {
                await context.PostAsync("Corporate");
            }
            else if (activity.Text.ToLower().ToUserLocale(context) == "forex")
            {
                await context.PostAsync("Forex");
            }
            else
            {
                await context.PostAsync("Please selct from the options given");
            }
        }
        private async Task IndividualRegAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            await context.PostAsync("Verifying your TIN");

            var myform = new FormDialog<Individual>(new Individual(), Individual.BuildIndividualForm, FormOptions.PromptInStart, null);
            context.Call<Individual>(myform, FormCompleteCallback);
        }

        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<Individual> result)
        {
            await context.PostAsync("Your registration is still pending approval. Your registration number will be sent to your mail when approved");
             
            context.Done<object>(null);
        }
    }
}