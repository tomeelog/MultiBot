using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using multibot.Extention;

namespace multibot.Dialogs
{
    [Serializable]
    public class VerifyReceipt : IDialog<object>
    {
        public string ReceiptNum { get; set; }
        public string TinNum { get; set; }
        public int ans { get; set; }
        public int num1 { get; set; }
        public int num2 { get; set; }
        string a = "Provide Your Receipt Number";
        
        string c = "Your receipt is active";
        string b = "Provide your TIN";
        string d = "Wrong answer, You will need to restart";
        string e = "Enter your Pin number";
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(a.ToUserLocale(context));
            context.Wait(MessageReceivedNumAsync);
        }

        private async Task MessageReceivedNumAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            this.ReceiptNum = activity.Text;
            
            
            await context.PostAsync(b.ToUserLocale(context));
            context.Wait(MessageReceivedTINAsync);
        }

        private async Task MessageReceivedTINAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            this.TinNum = activity.Text;
            string a = "What is";
            Random ran = new Random();
            int getRanNum1 = ran.Next(10);
            int getRanNum2 = ran.Next(10);
            while (getRanNum2 == getRanNum1)
                getRanNum2 = ran.Next(10);
            this.num1 = getRanNum1;
            this.num2 = getRanNum2;
            this.ans = this.num1 + this.num2;
            
            await context.PostAsync($"{a.ToUserLocale(context)} {this.num1} + {this.num2}?");
            context.Wait(MessageReceivedAnsAsync);
        }

        private async Task MessageReceivedAnsAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            string response = activity.Text;
            string ss = this.ans.ToString();
            if (response.Equals(ss)) {
                await context.PostAsync(c.ToUserLocale(context));
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync(d.ToUserLocale(context));
                await context.PostAsync(e.ToUserLocale(context));
                context.Wait(MessageReceivedNumAsync);
            }

            
        }
    }
}