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
    public class SendEmailToCustomerDialog : IDialog<IMessageActivity>
    {
        SendMsg _email = new SendMsg();
        private string Describe;
        private string TemplateEmailId;
        private string Title;
       

        public SendEmailToCustomerDialog()
        {

        }

        public SendEmailToCustomerDialog(string _templateEmailid, string _describe, string _title)
        {
            this.TemplateEmailId = _templateEmailid;
            this.Describe = _describe;
            this.Title = _title;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(this.Title);

            var feedback = ((Activity)context.Activity).CreateReply();


            feedback.Attachments = new List<Attachment>
                {
                    new HeroCard()
                    {
                       //Title = "Que tal eu enviar no seu email ?",
                       // Subtitle = "Ou pode me fazer outra pergunta...",
                        //Text = "Deixa eu te enviar um folder com estas informações no seu email ?",
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
                    (context.ConversationData).TryGetValue("User.Setting.Name", out string nome);
                    (context.ConversationData).TryGetValue("User.Setting.Email", out string email);

                    var form = new FormDialog<CaptureLead>(new CaptureLead(nome, email, Describe), CaptureLead.BuildForm, FormOptions.PromptInStart, null);
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
                context.Done<IMessageActivity>(null);
                return;
            }

            if (order != null)
            {
                (context.ConversationData).SetValue("User.Setting.Name", order.Name);
                (context.ConversationData).SetValue("User.Setting.Email", order.Email);

                 await _email.SendEmailAsync(nameCustomer: order.Name,
                                           subject: "Oi, sou eu a ***Ian*** da Isco. Segue as informações sobre o sistema que você me solicitou",
                                           to: order.Email,
                                           replayto: KeyPassAndPhrase._emailVendas,
                                           cc: new string[] { KeyPassAndPhrase._emailVendas  },
                                           templateId: this.TemplateEmailId);


                await context.PostAsync("Ok, já enviei no seu email, se precisar pode **me perguntar também** \n " +
                " Ah, tentei lhe enviar um CUPOM de desconto lá, se você fechar comigo...");

                
                

            }
            else
            {
                await context.PostAsync(KeyPassAndPhrase._SometinhgWrong);
            }

            context.Done<IMessageActivity>(null);

            

        }

    }
}