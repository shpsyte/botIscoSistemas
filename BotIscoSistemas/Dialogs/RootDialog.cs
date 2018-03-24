using System;
using System.Threading;
using System.Threading.Tasks;
using Bot4App.Dialogs.Dialog;
using Bot4App.QnA;
using Bot4App.Services;
using BotBlog.Dialogs.Dialog;
using BotBlog.Models;
 
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot4App.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        SendMsg _sendMsg = new SendMsg();
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }



        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            // await context.Forward(new GetEmailToSendInfoDialog("send.email.sistema", "Mais informação sistema"), ResumeAfterQnA, context.Activity, CancellationToken.None);


            


            await QnaMakerTask(context, true);




        }

        private async Task QnaMakerTask(IDialogContext context, bool ask)
        {
            var qnadialog = new QnaIscoSistemas(ask);
            var messageToForward = context.Activity;
            var cts = new CancellationTokenSource();

            await context.Forward(qnadialog, AfterQnADialog, messageToForward, cts.Token);
        }


        private async Task AfterQnADialog(IDialogContext context, IAwaitable<object> result)
        {
            var userFeedback = await result;

            if (await result != null)
            {
                await MessageReceivedAsync(context, result);
            }
            else
            {
                context.Done(true);
            }




        }

        private async Task AfterQnADialogNull(IDialogContext context, IAwaitable<object> result)
        {
            



        }



    }
}