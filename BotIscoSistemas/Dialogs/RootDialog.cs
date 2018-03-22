using System;
using System.Threading;
using System.Threading.Tasks;
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
            SendMsg _email = new SendMsg();

            await context.Forward(new QnaIscoSistemas(true), ResumeAfterQnA, context.Activity, CancellationToken.None);



            // context.Wait(MessageReceivedAsync);
        }


        private async Task ResumeAfterQnA(IDialogContext context, IAwaitable<object> result)
        {
            // await context.PostAsync("Vamos lá..");
            //var activity = (context as Activity);
            //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            //Activity reply = activity.CreateReply();
            //reply.Type = ActivityTypes.Typing;
            //reply.Text = null;
            //await connector.Conversations.ReplyToActivityAsync(reply);

            //context.Done<object>(null);
        }
    }
}