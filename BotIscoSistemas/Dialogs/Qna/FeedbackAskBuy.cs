using Bot4App.Dialogs.Dialog;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace BotBlog.Dialogs.Qna
{
    [Serializable]
    public class FeedbackAskBuy : IDialog<IMessageActivity>
    {
        private string qnaURL;
        private string userQuestion;

        public FeedbackAskBuy(string url, string question)
        {
            // keep track of data associated with feedback
            qnaURL = url;
            userQuestion = question;
        }

        public async Task StartAsync(IDialogContext context)
        {
            var feedback = ((Activity)context.Activity).CreateReply("Quer aproveitar e fechar comigo com **5%** de desconto ?, ainda dá tempo .... ");

            feedback.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
                    new CardAction(){ Title = "👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
                }
            };

            await context.PostAsync(feedback);

            context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;

            if (userFeedback.Text.Contains("yes-positive-feedback") || userFeedback.Text.Contains("no-negative-feedback"))
            {

                if (userFeedback.Text.Contains("yes-positive-feedback"))
                {
                    await context.Forward(new RegisterBuyDialog(5), ResumeAfterQnA, context.Activity, CancellationToken.None);
                }
                else if (userFeedback.Text.Contains("no-negative-feedback"))
                {
                    await context.PostAsync("Ok, sem problemas, mas estou aqui para te ajudar.. :) ");
                }

                //context.Done<IMessageActivity>(null);
            }
            else
            {
                // no feedback, return to QnA dialog
                context.Done<IMessageActivity>(userFeedback);
            }
        }



        private async Task ResumeAfterQnA(IDialogContext context, IAwaitable<object> result)
        {
           
        }


    }
}