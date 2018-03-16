using BotBlog.Dialogs.Qna;
using BotBlog.Models;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot4App.QnA
{
    [Serializable]
    public class QnaIscoSistemas : QnAMakerDialog
    {
        private readonly static string _QnaKnowledgedId = KeyPassAndPhrase._QnaKnowledgedIscoId;
        private readonly static string _QnaSubscriptionKey = KeyPassAndPhrase._QnaSubscriptionKey;
        private readonly static string _DefatulMsg = KeyPassAndPhrase._MsgNotUndertand;
        private readonly static double _Score = KeyPassAndPhrase._Score;
        private readonly static int _QtyAnswerReturn = KeyPassAndPhrase._QtyAnswerReturn;

        public bool askLead { get; set; }
        


        public QnaIscoSistemas(bool _askLead) : base(new QnAMakerService(new QnAMakerAttribute(_QnaSubscriptionKey, _QnaKnowledgedId, _DefatulMsg, _Score, _QtyAnswerReturn)))
        {
            this.askLead = _askLead;
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
            var answer = results.Answers.First().Answer;
            var userQuestion = (context.Activity as Activity).Text;
          

            if (askLead)
            {
                await Task.Delay(5000).ContinueWith(t =>
                 {
                    context.Call(new FeedbackAskLeadDialog(answer, userQuestion), ResumeAfterFeedback);
                 });
               
            }
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

       
        //// Qunado o ML está ativa este metodo pergunta: "Você quis dizer isso ?" para que o qna possa aprender.
        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            // responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.75)
            {
                await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            }
        }


    }


}


