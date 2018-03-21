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
    public class LoadMoreInfoDialog : IDialog<IMessageActivity>
    {
        SendMsg _email = new SendMsg();
        private string describe;
        private string templateEmailid;
        private string nome;
        private string email;

        public LoadMoreInfoDialog()
        {

        }

        public LoadMoreInfoDialog(string _templateEmailid, string _describe)
        {
            this.templateEmailid = _templateEmailid;
            this.describe = _describe;


        }

        public async Task StartAsync(IDialogContext context)
        {
            var feedback = ((Activity)context.Activity).CreateReply($"Legal, nosso sistema é muito prático: \n " +
               $" * Custa apenas R$ **69,90/Mês**  \n" +
               $" * É **Online**, você só precisa de Internet  \n" +
               $" * **Não tem limite** para emissão de notas  \n" +
               $" * É **homologado** para Nota do Consumidor **(Nfc-e)** e **(Nf-e)** \n" +
               $" * Suporte **Gratuito e Ilimitado**  \n" +
               $" * Envio das notas **por Email (Cliente e contador)**  \n" +
               $" * Está pronto para uso!");




            feedback.Attachments = new List<Attachment>
                {
                    new HeroCard()
                    {
                        Title = "Qual seu email ?",
                       // Subtitle = "Ou pode me fazer outra pergunta...",
                        Text = "Deixa eu te enviar um folder com estas informações no seu email, mas se preferir \n" +
                        " pode me fazer perguntas por aqui mesmo... ",
                        Buttons = new List<CardAction>
                        {
                            new CardAction(){ Title = "👍 Sim me envie por email..", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" }
                        }
                    }.ToAttachment()
                };


            await context.PostAsync(feedback);

            context.Wait(this.MessageReceivedAsync);
        }

       


        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;


            if (userFeedback.Text.Contains("yes-positive-feedback") )
            {
                
                    var activity = (context.Activity as Activity);
                    var capLeadForm = new CaptureLead();


                    (context.ConversationData).TryGetValue("CustomerName", out nome);
                    (context.ConversationData).TryGetValue("CustomerEmail", out email);

                    capLeadForm.Name = nome;
                    capLeadForm.Email = email;
                    capLeadForm.Describe = this.describe;


                    var form = new FormDialog<CaptureLead>(capLeadForm, CaptureLead.BuildForm, FormOptions.PromptInStart, null);
                    context.Call<CaptureLead>(form, FormCompleteCallback);
               
            }
            else
            {
                // no feedback return
                context.Done<IMessageActivity>(userFeedback);

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


                //send email to customer
                _email.SendEmailAsync("Oi, sou eu a **Ian**. Segue as informações sobre o sistema que solicitou",
                                                "",
                                                KeyPassAndPhrase._emailVendas,
                                                order.Email,
                                                this.templateEmailid,
                                                null,
                                                new string[] { KeyPassAndPhrase._emailVendas, KeyPassAndPhrase._emailCopiaVendas });





                await context.PostAsync("Ok, já enviei, se precisar pode **me perguntar também**...");


            }
            else
            {
                await context.PostAsync(KeyPassAndPhrase._SometinhgWrong);
            }

            context.Done<string>(null);

        }

    }
}