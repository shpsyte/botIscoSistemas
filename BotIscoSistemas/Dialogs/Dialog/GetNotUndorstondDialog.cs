using Bot4App.Dialogs.Dialog;
using Bot4App.Forms;
using Bot4App.Services;
using BotBlog.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BotBlog.Dialogs.Dialog
{
    [Serializable]
    public class GetNotUndorstondDialog : IDialog<IMessageActivity>
    {
        SendMsg _email = new SendMsg();
        
        private string nome;
        private string email;

        public GetNotUndorstondDialog()
        {

        }
         

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(GetContact);
            return Task.CompletedTask;
        }

        private async Task GetContact(IDialogContext context, IAwaitable<object> result)
        { 


            var feedback = ((Activity)context.Activity).CreateReply();


            //feedback.SuggestedActions = new SuggestedActions()
            //{
            //    Actions = new List<CardAction>()
            //    {
            //        new CardAction(){ Title = "👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
            //        new CardAction(){ Title = "👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
            //    }
            //};


            feedback.Attachments = new List<Attachment>
                {
                    new HeroCard()
                    {
                        Title = "Qual seu email ?",
                       // Subtitle = "Ou pode me fazer outra pergunta...",
                        Text = "Deixa eu perguntar isso para alguém mais esperto que eu.. 🙂 " +
                        " e eu mesmo vou te enviar um e-mail com a resposta... ",
                        Buttons = new List<CardAction>
                        {
                            new CardAction(){ Title = "👍 Ok, me envie por email..", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
                            new CardAction(){ Title = "👎 Prefiro tentar por aqui..", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
                        }
                    }.ToAttachment()
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
                    var activity = (context.Activity as Activity);
                    var capLeadForm = new CaptureLead();


                    (context.ConversationData).TryGetValue("CustomerName", out nome);
                    (context.ConversationData).TryGetValue("CustomerEmail", out email);

                    capLeadForm.Name = nome;
                    capLeadForm.Email = email;
                     

                    var form = new FormDialog<CaptureLead>(capLeadForm, CaptureLead.BuildForm, FormOptions.PromptInStart, null);
                    context.Call<CaptureLead>(form, FormCompleteCallback);
                }
                else
                {
                   await context.PostAsync(KeyPassAndPhrase._IamHereForHelp);
                    context.Done<string>(null);
                }
            }
            else
            {
                // no feedback return
                context.Done<string>(null);

            }

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
                await context.PostAsync(KeyPassAndPhrase._OkImSorryButIamHere);
                context.Done<string>(null);
                return;
            }

            if (order != null)
            {
                (context.ConversationData).SetValue("CustomerName", order.Name);
                (context.ConversationData).SetValue("CustomerEmail", order.Email);


                
                _email.SendEmailAsync(order.Name, $"Oi, sou eu a Ian. Segue as informações que { order.Name } solicitou",
                                                order.ToString(),
                                                order.Email,
                                                KeyPassAndPhrase._emailVendas,
                                                null,
                                                null);
                

                await context.PostAsync("Ok, já já vou **aprender isso** e te respondo, mas se precisar de outra informação pode **me perguntar por aqui** 🙂 ");


            }
            else
            {
                await context.PostAsync(KeyPassAndPhrase._SometinhgWrong);
            }

            context.Done<string>(null);

        }

    }
}