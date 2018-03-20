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
        private List<string> listGenericFeature;


        public bool askLead { get; set; }



        public QnaIscoSistemas(bool _askLead) : base(new QnAMakerService(new QnAMakerAttribute(_QnaSubscriptionKey, _QnaKnowledgedId, _DefatulMsg, _Score, _QtyAnswerReturn)))
        {
            this.askLead = _askLead;
            this.listGenericFeature = Feature.ListGenericFeature();
        }


        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var response = result.Answers.First().Answer;
            await context.PostAsync(response);

        }

        ////// Override to log matched Q&A before ending the dialog
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            if (askLead)
            {
                await Task.Delay(1500).ContinueWith(t =>
                {
                    Activity feedback = GetFeedBackQuestion(context, false);
                    context.PostAsync(feedback);
                    context.Wait(GetMoreInfo);
                });
            }
        }



        public async Task GetMoreInfo(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;
            Activity resposta = ((Activity)context.Activity).CreateReply();

            if (userFeedback.Text.Contains("yes-positive-feedback") || userFeedback.Text.Contains("no-negative-feedback"))
            {
                if (userFeedback.Text.Contains("yes-positive-feedback"))
                {
                    string last;
                    Random rnd = new Random();
                    (context.ConversationData).TryGetValue("last", out last);
                    this.listGenericFeature.Remove(last);
                    int r = rnd.Next(listGenericFeature.Count);

                   

                    if (listGenericFeature.Count() == 0)
                    {
                        context.Done<IMessageActivity>(null);
                    }
                    else
                    {

                        var feature = listGenericFeature[r];
                        (context.ConversationData).SetValue("last", feature);

                        var dadosResposta = feature.Split('|');

                        if (dadosResposta.Length == 1)
                            await context.PostAsync(feature);
                        else
                        {
                            resposta.Attachments.Add(CreateCard(dadosResposta[0], dadosResposta[1], dadosResposta[2]));
                            await context.PostAsync(resposta);
                        }


                        await Task.Delay(1500).ContinueWith(t =>
                        {

                            Activity feedback = GetFeedBackQuestion(context, false);
                            context.PostAsync(feedback);
                            context.Wait(GetMoreInfo);
                        });
                    }


                }
                else if (userFeedback.Text.Contains("no-negative-feedback"))
                {
                    context.Done<IMessageActivity>(null);
                }
            }
            else
            {
                await MessageReceivedAsync(context, result);
            }
        }

        private static Activity GetFeedBackQuestion(IDialogContext context, bool card = false)
        {
            string lastLoadMore;
            (context.ConversationData).TryGetValue("lastloadmore", out lastLoadMore);

            lastLoadMore = Feature.GetRandomListGenericLoadMore("lastLoadMore");
            (context.ConversationData).SetValue("lastloadmore", lastLoadMore);


            var feedback = ((Activity)context.Activity).CreateReply(lastLoadMore);
            var actions = new List<CardAction>()
            {
                new CardAction(){ Title = "👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
                new CardAction(){ Title = "👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
            };

            if (card)
            {
                HeroCard herocard = new HeroCard
                {
                    // Title = lastLoadMore,
                    Subtitle = "ou, pode me fazer outra pergunta...",
                };
                herocard.Buttons = actions;
                feedback.Attachments.Add(herocard.ToAttachment());
            }
            else
            {
                feedback.SuggestedActions = new SuggestedActions()
                {
                    Actions = actions
                };
            }

            return feedback;
        }


        //// Qunado o ML está ativa este metodo pergunta: "Você quis dizer isso ?" para que o qna possa aprender.
        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            //await base.QnAFeedbackStepAsync(context, qnaMakerResults);

            //responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.80)
            {
                await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            }


        }


        private Attachment CreateCard(string titulo, string descricao, string url)
        {
            var animationCard = new AnimationCard
            {
                Title = titulo,
                Subtitle = descricao,
               
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = url
                    }
                }
            };

            return animationCard.ToAttachment();
        }


    }


}


