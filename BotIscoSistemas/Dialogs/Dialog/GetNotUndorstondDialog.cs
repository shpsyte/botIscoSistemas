//using Bot4App.Dialogs.Dialog;
//using Bot4App.Forms;
//using Bot4App.Services;
//using BotBlog.Models;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.FormFlow;
//using Microsoft.Bot.Connector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web;

//namespace BotBlog.Dialogs.Dialog
//{
//    [Serializable]
//    public class GetNotUndorstondDialog : IDialog<IMessageActivity>
//    {
//        SendMsg _email = new SendMsg();
//        private string describe;


//        //public GetNotUndorstondDialog()
//        //{

//        //}

//        //public GetNotUndorstondDialog(string _describe)
//        //{
//        //    this.describe = _describe;
//        //}

//        public Task StartAsync(IDialogContext context)
//        {
//            context.Wait(GetContact);
//            return Task.CompletedTask;
//        }

//        private async Task GetContact(IDialogContext context, IAwaitable<object> result)
//        { 


//            var feedback = ((Activity)context.Activity).CreateReply();

             

//            feedback.Attachments = new List<Attachment>
//                {
//                    new HeroCard()
//                    {
//                        Title = "Qual seu email ?",
//                       // Subtitle = "Ou pode me fazer outra pergunta...",
//                        Text = "Deixa eu perguntar isso para alguém mais esperto que eu.. 🙂 " +
//                        " e eu mesmo vou te enviar um e-mail com a resposta... ",
//                        Buttons = new List<CardAction>
//                        {
//                            new CardAction(){ Title = "👍 Ok, me envie por email..", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
//                            new CardAction(){ Title = "👎 Prefiro tentar por aqui..", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
//                        }
//                    }.ToAttachment()
//                };


//            await context.PostAsync(feedback);

//            context.Wait(this.MessageReceivedAsync);

//        }


//        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
//        {
//            var userFeedback = await result;


//            if (userFeedback.Text.Contains("yes-positive-feedback") || userFeedback.Text.Contains("no-negative-feedback"))
//            {
//                if (userFeedback.Text.Contains("yes-positive-feedback"))
//                {
//                    var activity = (context.Activity as Activity);
//                    (context.ConversationData).TryGetValue("User.Setting.Name", out string nome);
//                    (context.ConversationData).TryGetValue("User.Setting.Email", out string  email);
 

//                    var form = new FormDialog<CaptureLead>(new CaptureLead(nome, email, describe), CaptureLead.BuildForm, FormOptions.PromptInStart, null);
//                    context.Call<CaptureLead>(form, FormCompleteCallback);
//                }
//                else
//                {
//                   await context.PostAsync(KeyPassAndPhrase._IamHereForHelp);
//                    context.Done<string>(null);
//                }
//            }
//            else
//            {
//                // no feedback return
//                context.Done<IMessageActivity>(userFeedback);

//            }

//        }




//        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<CaptureLead> result)
//        {
//            var activity = (context.Activity as Activity);

//            CaptureLead order = null;
//            try
//            {
//                order = await result;
//            }
//            catch (OperationCanceledException)
//            {
//                await context.PostAsync(KeyPassAndPhrase._OkImSorryButIamHere);
//                context.Done<string>(null);
//                return;
//            }

//            if (order != null)
//            {
//                (context.ConversationData).SetValue("User.Setting.Name", order.Name);
//                (context.ConversationData).SetValue("User.Setting.Email", order.Email);


//                await _email.SendEmailAsync(nameCustomer: order.Name,
//                                            subject: $"Oi, sou eu a Ian. Segue as informações que { order.Name } solicitou",
//                                            body: order.ToString(),
//                                            replayto: order.Email,
//                                            to: KeyPassAndPhrase._emailVendas);
                

//                await context.PostAsync("Ok, vou buscar esta informação \n " +
//                    " e já te respondo. Se precisar de outra informação pode **me perguntar por aqui** 🙂 ");


//            }
//            else
//            {
//                await context.PostAsync(KeyPassAndPhrase._SometinhgWrong);
//            }



//            context.Done<string>(null);

//        }

//    }
//}