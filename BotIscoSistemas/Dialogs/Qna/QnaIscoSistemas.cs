using Bot4App.Services;
using BotBlog.Dialogs.Dialog;
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
        private int _qtdePerguntas;

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
                // await context.PostAsync(result.Answers.First().Answer);
                 
                await context.PostAsync(FormatMsg(context, result));
                 

            }
        }

        ////// Override to log matched Q&A before ending the dialog
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            (context.ConversationData).TryGetValue("qtperguntas", out _qtdePerguntas);
            _qtdePerguntas++;
            (context.ConversationData).SetValue("qtperguntas", _qtdePerguntas);

            if (_qtdePerguntas >= 3)
            {
                (context.ConversationData).SetValue("qtperguntas", 0);
                _qtdePerguntas = 0;
                context.Call(new LoadMoreInfoDialog("send.email.sistema", "Quer que eu lhe envie *mais informação* por e-mail? 🙂"), ResumeAfterFeedback);
            }


        }


        ////// Qunado o ML está ativa este metodo pergunta: "Você quis dizer isso ?" para que o qna possa aprender.
        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            //await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            //responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.80)
            {
                //await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
                await context.PostAsync(FormatMsg(context, qnaMakerResults));
            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
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




        protected IMessageActivity FormatMsg(IDialogContext context, QnAMakerResults result)
        {

            var primeiraresposta = result.Answers.First().Answer;
            Activity resposta = ((Activity)context.Activity).CreateReply();
            var dadosResposta = primeiraresposta.Split('|');
            resposta.AttachmentLayout = AttachmentLayoutTypes.Carousel;


            if (dadosResposta.Length == 1)
            {
                var cardProduct = CreateCard("", "", primeiraresposta, "", "");
                resposta.Attachments.Add(cardProduct);


            }
            else
            {
                string urlMedia = dadosResposta[3] ?? "";
                string urlOpen = dadosResposta[4] ?? "";
                var cardProduct = CreateCard(dadosResposta[0], dadosResposta[1], dadosResposta[2], urlMedia, urlOpen);
                resposta.Attachments.Add(cardProduct);

            }


            return resposta;
        }


        private Attachment CreateCard(string titulo, string subtitulo, string descricao, string mediaUrl = null, string urlopen = null)
        {
            var heroCard = new HeroCard
            {
                Title = titulo,
                Subtitle = subtitulo,
                Text = descricao
            };

            if (!string.IsNullOrEmpty(mediaUrl))
                heroCard.Images = new List<CardImage> { new CardImage(mediaUrl) };

            if (!string.IsNullOrEmpty(urlopen))
                heroCard.Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "+ Info aqui", value: "urlopen") };

            return heroCard.ToAttachment();
        }






        //protected async Task MessageReceivedAsync(IDialogContext context, QnAMakerResults result)
        //{
        //    await SendAwnser(context, result);
        //}





        //public async Task GetMoreInfo(IDialogContext context, IAwaitable<IMessageActivity> result)
        //{
        //    var userFeedback = await result;
        //    Activity resposta = ((Activity)context.Activity).CreateReply();

        //    if (userFeedback.Text.Contains("yes-positive-feedback"))
        //    {
        //        if (userFeedback.Text.Contains("yes-positive-feedback"))
        //        {
        //            string last;
        //            Random rnd = new Random();
        //            (context.ConversationData).TryGetValue("last", out last);
        //            this.listGenericFeature.Remove(last);
        //            int r = rnd.Next(listGenericFeature.Count);



        //            if (listGenericFeature.Count() == 0)
        //            {
        //                context.Done<IMessageActivity>(null);
        //            }
        //            else
        //            {

        //                var feature = listGenericFeature[r];
        //                (context.ConversationData).SetValue("last", feature);

        //                var dadosResposta = feature.Split('|');

        //                if (dadosResposta.Length == 1)
        //                    await context.PostAsync(feature);
        //                else
        //                {
        //                    resposta.Attachments.Add(CreateCard(dadosResposta[0], dadosResposta[1], dadosResposta[2]));
        //                    await context.PostAsync(resposta);
        //                }


        //                await Task.Delay(1500).ContinueWith(t =>
        //                {

        //                    Activity feedback = ShowMoreOptions(context);
        //                    context.PostAsync(feedback);
        //                    context.Wait(GetMoreInfo);
        //                });
        //            }


        //        }
        //        //else if (userFeedback.Text.Contains("no-negative-feedback"))
        //        //{
        //        //    context.Done<IMessageActivity>(null);
        //        //}
        //    }
        //    else
        //    {
        //        //await MessageReceivedAsync(context, result);
        //        context.Done<IMessageActivity>(null);
        //    }
        //}

        //private static Activity ShowMoreOptions(IDialogContext context)
        //{
        //    var moreOptionsReply = ((Activity)context.Activity).CreateReply();
        //    moreOptionsReply.Attachments = new List<Attachment>
        //    {
        //        new HeroCard()
        //        {
        //            Title = "Deseja mais informação?",
        //            Subtitle = "Ou me faça outra pergunta...",
        //            Buttons = new List<CardAction>
        //            {
        //                new CardAction(){ Title = "👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
        //                 new CardAction(){ Title = "👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
        //            }
        //        }.ToAttachment()
        //    };

        //    return moreOptionsReply;
        //}


    }


}


