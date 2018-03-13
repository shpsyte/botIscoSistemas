using BotBlog.Models;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bot4App.QnA
{
    [Serializable]
    public class QnaAboutMe : QnAMakerDialog
    {
        private readonly static string _QnaKnowledgedId = KeyPassAndPhrase._QnaKnowledgedJoseId;
        private readonly static string _QnaSubscriptionKey = KeyPassAndPhrase._QnaSubscriptionKey;
        private readonly static string _DefatulMsg = KeyPassAndPhrase._MsgNotUndertand;
        private readonly static double _Score = KeyPassAndPhrase._Score;
        private readonly static int _QtyAnswerReturn = KeyPassAndPhrase._QtyAnswerReturn;


        public QnaAboutMe() : base(new QnAMakerService(new QnAMakerAttribute(_QnaSubscriptionKey, _QnaKnowledgedId, _DefatulMsg, _Score, _QtyAnswerReturn)))
        {

        }




        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {

            //await base.RespondFromQnAMakerResultAsync(context, message, result);
            if (result.Answers.Count > 0)
            {
                var response = result.Answers.First().Answer;
                await context.PostAsync(response);
            }
            else
            {
                var response = "EU sei que você queria saber algo sobre mim, " +
                    "mas não sei lhe responder isso ainda...";
                await context.PostAsync(response);

            }

        }

 

        ////// Override to log matched Q&A before ending the dialog
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
           // await base.DefaultWaitNextMessageAsync(context, message, results);
           //  await context.PostAsync("It taht");


            var answer = results.Answers.First().Answer;
            //string[] qnaAnswerData = answer.Split(';');
            //string qnaURL = qnaAnswerData[2];

            // pass user's question
            var userQuestion = (context.Activity as Activity).Text;

            context.Call(new BotBlog.Dialogs.Qna.FeedbackDialog("url", userQuestion), ResumeAfterFeedback);

        }


        private async Task ResumeAfterFeedback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (await result != null)
            {
                await MessageReceivedAsync(context, result);
            }
            else
            {
                context.Done<IMessageActivity>(null);
            }
        }

    }


}