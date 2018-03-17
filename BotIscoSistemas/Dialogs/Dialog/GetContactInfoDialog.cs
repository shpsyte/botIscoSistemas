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
    public class GetContactInfoDialog : IDialog<IMessageActivity>
    {
        SendMsg _email = new SendMsg();
        private string user;
        private string email;
        private string question;

        public GetContactInfoDialog()
        {

        }

        public GetContactInfoDialog(string _user, string _email)  
        {
            this.user = _user;
            this.email = _email;

        }

        public GetContactInfoDialog(string _question)  
        {
            this.question = _question;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(GetContact);
            return Task.CompletedTask;

        }

        private async Task GetContact(IDialogContext context, IAwaitable<object> result)
        {

            var feedback = ((Activity)context.Activity).CreateReply(KeyPassAndPhrase._GetContactInfo);
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

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;

            if (userFeedback.Text.Contains("yes-positive-feedback"))
            {

                    var activity = (context.Activity as Activity);
                    var capLeadForm = new CaptureLead();

                    var form = new FormDialog<CaptureLead>(capLeadForm, CaptureLead.BuildForm, FormOptions.PromptInStart, null);
                    context.Call<CaptureLead>(form, FormCompleteCallback);
            }
            else
            {
                // no feedback
                await context.PostAsync(KeyPassAndPhrase._IamHereForHelp);
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
                return;
            }

            if (order != null)
            {
                await _email.SendEmailAsync(KeyPassAndPhrase._NewLeadComming, order.ToString(), order.Email, KeyPassAndPhrase._emailVendas, null, new string[] { KeyPassAndPhrase._emailCopiaVendas });
                await context.PostAsync(KeyPassAndPhrase._ItsDoneContact);
                
                (context.ConversationData).SetValue("CustomerName", order.Name);
                (context.ConversationData).SetValue("CustomerEmail", order.Email);
            }
            else
            {
                await context.PostAsync(KeyPassAndPhrase._SometinhgWrong);
            }

            context.Done<string>(null);

        }

    }
}