using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Mail;
using Bot4App.Forms;
using BotBlog.Models;
using BotBlog.Forms;
using System.Linq;
using Bot4App.Services;
using System.Globalization;
using System.Threading;
using BotBlog.Dialogs.Qna;

namespace Bot4App.Dialogs.Dialog
{
    [Serializable]
    public class RegisterLeadDialog : IDialog<object>
    {
        //private readonly static string _LuisModelId = KeyPassAndPhrase._LuisModelId;
        //private readonly static string _LuiSubscriptionKey = KeyPassAndPhrase._LuiSubscriptionKey;
        //private readonly static string _MsgNotUndertand = KeyPassAndPhrase._MsgNotUndertand;
        //private readonly static string _DefaultMsgHelp = KeyPassAndPhrase._MsgHelp;


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(SendConversation);
            return Task.CompletedTask;

        }




        private Task SendConversation(IDialogContext context, IAwaitable<object> result)
        {
            var activity = (context.Activity as Activity);
            CultureInfo lang = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = lang;
            Thread.CurrentThread.CurrentUICulture = lang;
            context.Activity.AsMessageActivity().Locale = lang.ToString();
            var capLeadForm = new CaptureLead();

            var form = new FormDialog<CaptureLead>(capLeadForm, CaptureLead.BuildForm, FormOptions.PromptInStart, null);
            context.Call<CaptureLead>(form, FormCompleteCallback);
            return Task.CompletedTask;

        }

        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<CaptureLead> result)
        {
            var activity = (context.Activity as Activity);

            CaptureLead order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Puxa vida, que pena, se precisar estou por aqui... ;) ");
                return;
            }

            if (order != null)
            {

                Email.Send("Novo Lead CREATELEAD", order.ToString(), order.Email, KeyPassAndPhrase._emailVendas, new string[] { "suporte@iscosistemas.com.br", "jose.luiz@iscosistemas.com", "jose.iscosistemas@zoho.com" });


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
            else
            {
                await context.PostAsync("Hum.. algo deu errado, por favor tente **novamente**");
            }

            //context.Call(new FeedbackAskBuy("name", "email"), ResumeAfterQnA);

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
                    await context.PostAsync("Ok, já enviei seus contato e já irei lhe enviar mais detalhes no seu **email** , ta bom ?...");
                    await context.PostAsync($" Love to help! 😍😍 ");
                   context.Done<string>(null);

                }

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