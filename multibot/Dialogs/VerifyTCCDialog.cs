using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using multibot.Extention;

namespace multibot.Dialogs
{
    [Serializable]
    public class VerifyTCCDialog : IDialog<object>
    {
        public string tccNum { get; set; }
        public int num1 { get; set; }
        public int num2 { get; set; }
        public int ans { get; set; }
        public string ss { get; set; }
        public async Task StartAsync(IDialogContext context)
        {
            string a = "Enter TCC number";
            

            await context.PostAsync(a.ToUserLocale(context));
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            
            var activity = await result as IMessageActivity;

            this.tccNum = activity.Text;
            string b = "What is";

            Random ran = new Random();
            int getRanNum1 = ran.Next(10);
            int getRanNum2 = ran.Next(10);
            while (getRanNum2 == getRanNum1)
                getRanNum2 = ran.Next(10);
            this.num1 = getRanNum1;
            this.num2 = getRanNum2;
            this.ans = this.num1 + this.num2;
            this.ss = this.ans.ToString();
            PromptDialog.Text(context, ResumeAfterquestion, $"{b.ToUserLocale(context)} {getRanNum1} + {getRanNum2}?");

        }

        private async Task ResumeAfterquestion(IDialogContext context, IAwaitable<string> result)
        {
            string activity = await result;
            string number = activity;
            string mm = this.ss;
            string c = "your receipt is fine";
            string d = "Please re-enter your TCC number";
            if (number.ToLower().Contains(mm))
            {
                
                await context.PostAsync($"{c.ToUserLocale(context)}");
                context.Done<object>(null);
            }
            else {
                await context.PostAsync("wrong");
                await context.PostAsync(d.ToUserLocale(context));
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}