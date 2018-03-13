using BotBlog.Models;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot4App.QnA
{
    [Serializable]
    public class QnaBlogSite : QnAMakerDialog
    {
        private readonly static string _QnaKnowledgedId = KeyPassAndPhrase._QnaKnowledgedBlogId;
        private readonly static string _QnaSubscriptionKey = KeyPassAndPhrase._QnaSubscriptionKey;
        private readonly static string _DefatulMsg = KeyPassAndPhrase._MsgNotUndertand;
        private readonly static double _Score = KeyPassAndPhrase._Score;
        private readonly static int _QtyAnswerReturn = KeyPassAndPhrase._QtyAnswerReturn;

        public QnaBlogSite() : base(new QnAMakerService(new QnAMakerAttribute(_QnaSubscriptionKey, _QnaKnowledgedId, _DefatulMsg, _Score, _QtyAnswerReturn)))
        {

        }


        protected async Task SendAwnser(IDialogContext context, QnAMakerResults result)
        {
            var primeiraresposta = result.Answers.First().Answer;

            Activity resposta = ((Activity)context.Activity).CreateReply();
            var dadosResposta = primeiraresposta.Split('|');

            if (dadosResposta.Length == 1)
            {
                await context.PostAsync(primeiraresposta);
                return;
            }

            var descricao = Truncate(dadosResposta[0], 160);
            var titulo = dadosResposta[1];
            var url = dadosResposta[2];
            var img = "https://joseluiz.azurewebsites.net/images/" + (string.IsNullOrEmpty(dadosResposta[3]) ? "blog.png" : dadosResposta[3]);
            var data = dadosResposta[4];


            var card = CreateCard(titulo, descricao, url, img, data);
            resposta.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resposta.Attachments.Add(card);
            await context.PostAsync(resposta);
        }


        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            //await base.RespondFromQnAMakerResultAsync(context, message, result);
            await SendAwnser(context, result);
        }

        private Attachment CreateCard(string titulo, string descricao, string url, string img, string data)
        {


            HeroCard card = new HeroCard
            {
                Title = titulo,
                Subtitle = descricao,
            };

             
                card.Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, "Visite", value:url)
                };
           


            card.Images = new List<CardImage>
            {
                new CardImage(url = img)
            };

            return card.ToAttachment();

        }


        ////// Override to log matched Q&A before ending the dialog
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            // get the URL
            // await context.PostAsync("asdfasd");


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

        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            // responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.80)
            {
                await SendAwnser(context, qnaMakerResults);
            }
            else
            {
                //await base.QnAFeedbackStepAsync(context, qnaMakerResults);
                Activity resposta = ((Activity)context.Activity).CreateReply();
                foreach (var item in qnaMakerResults.Answers.ToList())
                {

                    var primeiraresposta = item.Answer;
                    var dadosResposta = primeiraresposta.Split('|');

                    if (dadosResposta.Length == 1)
                    {
                        await context.PostAsync(primeiraresposta);
                        return;
                    }

                    var descricao = Truncate(dadosResposta[0], 160);
                    var titulo = dadosResposta[1];
                    var url = dadosResposta[2];
                    var img = "https://joseluiz.azurewebsites.net/images/" + (string.IsNullOrEmpty(dadosResposta[3]) ? "blog.png" : dadosResposta[3]);
                    var data = dadosResposta[4];


                    var card = CreateCard(titulo, descricao, url, img, data);
                    resposta.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resposta.Attachments.Add(card);
                }
                await context.PostAsync(resposta);

                var userQuestion = (context.Activity as Activity).Text;
                context.Call(new BotBlog.Dialogs.Qna.FeedbackDialog("url", userQuestion), ResumeAfterFeedback);

            }
        }

        public string Truncate(string s, int length)
        {
            if (s.Length > length)
                return string.Concat(s.Substring(0, length));
            return s;
        }


    }


}


