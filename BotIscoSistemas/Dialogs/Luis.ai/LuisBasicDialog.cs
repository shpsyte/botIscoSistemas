
using Bot4App.Dialogs.Dialog;
using Bot4App.Forms;
using Bot4App.Models;
using Bot4App.QnA;
using Bot4App.Services;
using BotBlog.Dialogs.Dialog;
using BotBlog.Models;
using Microsoft.Bot.Builder.Dialogs;
 
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;


namespace Bot4App.Dialogs.Luis.ai
{
    [Serializable]
    public class LuisBasicDialog : LuisDialog<object>
    {

        public static string _agent;
        public static string _person;
        public static string _fone;
        private readonly static string _LuisModelId = KeyPassAndPhrase._LuisModelId;
        private readonly static string _LuiSubscriptionKey = KeyPassAndPhrase._LuiSubscriptionKey;
        private readonly static string _LuisEndPoint = KeyPassAndPhrase._LuiEndPoint;

        private readonly static string _MsgNotUndertand = KeyPassAndPhrase._MsgNotUndertand;
        private readonly static string _DefaultMsgHelp = KeyPassAndPhrase._MsgHelp;
        
        SendMsg _sendMsg = new SendMsg();
        
       


        public LuisBasicDialog() : base(new LuisService(new LuisModelAttribute(_LuisModelId, _LuiSubscriptionKey, LuisApiVersion.V2, _LuisEndPoint)))
        {

        }

        

        [LuisIntent("")]
        [LuisIntent("None")]
        [LuisIntent("Qna-Bot")]
        public async Task QnaDialog(IDialogContext context, LuisResult result)
        {
            await QnaMakerTask(context, true);

            //await _sendMsg.SendEmailAsync(nameCustomer: "Customer", 
            //                              subject: "Ops.. Não sei que isso quer dizer...", 
            //                              body: $" <b> { (context.Activity as Activity).Text } </b> Você pode me treinar por favor ? Lá no QNA e no Luis.Ai..", 
            //                              to: KeyPassAndPhrase._emailAdmIan);

            //await context.PostAsync($"{_MsgNotUndertand}");

            //await context.Forward(new GetNotUndorstondDialog(), ResumeAfterInfo, context.Activity, CancellationToken.None);
        }



        [LuisIntent("sense-bot")]
        public async Task Sense(IDialogContext context, LuisResult result)
        {
            await QnaMakerTask(context, false);
        }


        private async Task QnaMakerTask(IDialogContext context, bool ask)
        {
            var qnadialog = new QnaIscoSistemas(ask);
            var messageToForward = context.Activity;
            var cts = new CancellationTokenSource();

            await context.Forward(qnadialog, AfterQnADialog, messageToForward, cts.Token);
        }

        private async Task AfterQnADialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;

            if (await result != null)
            {
                await MessageReceived(context, result);
                
            }
            else
            {
                context.Done(true);
            }
        }



        [LuisIntent("contact-bot")]
        public async Task ContactUrgent(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            _fone    = entities.FirstOrDefault(c => c.Type == "Contact.Fone")?.Entity;
            _agent  = entities.FirstOrDefault(c => c.Type == "Contact.Agent")?.Entity;
            _person  = entities.FirstOrDefault(c => c.Type == "Contact.Person")?.Entity;


            if (string.IsNullOrEmpty(_fone))
            {
                await context.PostAsync("Qual seu fone ?");
                context.Wait(GetFone);
            }else
            { 

                await _sendMsg.SendEmailAsync(nameCustomer: _person,
                                              subject: "Favor entrar em contato",
                                              body: $"Olá { _agent }, Pode por favor entrar em contato com { _person } no Fone { _fone } ", 
                                              to: BestDestination.GetBestEmailTo(_agent));

                await context.PostAsync($"Ok, já pedi para { _agent } entrarem em contato no fone { _fone }, obrigado ");

            }

        }


        private async Task GetFone(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var fone = await value;
            await _sendMsg.SendEmailAsync(nameCustomer: _person,
                              subject: "Favor entrar em contato",
                              body: $"Olá { _agent }, Pode por favor entrar em contato com { _person } no Fone { fone.Text } ",
                              to: BestDestination.GetBestEmailTo(_agent));

            await context.PostAsync($"Ok, já pedi para { _agent } entrarem em contato no fone { fone.Text }, obrigado ");

            context.Wait(MessageReceived);
        }

         


        [LuisIntent("infosystem-bot")]
        public async Task GetInfoAndSendEmail(IDialogContext context, LuisResult result)
        {
            


            await context.Forward(new GetEmailToSendInfoDialog(
                        KeyPassAndPhrase._templateEmailSalesId, 
                        "Mais informação sistema"), ResumeAfterQnA, context.Activity, CancellationToken.None);

        }
         

        //private async Task ResumeAfterInfo(IDialogContext context, IAwaitable<object> result)
        //{
        //    await context.Forward(new QnaIscoSistemas(true), ResumeAfterQnA, context.Activity, CancellationToken.None);

        //}


        [LuisIntent("buy-bot")]
        public async Task Buy(IDialogContext context, LuisResult result)
        {
             
            await context.Forward(new RegisterBuyDialog(0), ResumeAfterQnA, context.Activity, new CancellationTokenSource().Token);
        }




        [LuisIntent("help-bot")]
        public async Task Help(IDialogContext context, LuisResult result) => await context.Forward(new RegisterSuportDialog(), ResumeAfterQnA, context.Activity, CancellationToken.None);





        [LuisIntent("greeting-bot")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")).TimeOfDay;
            string saudacao;


            if (now < TimeSpan.FromHours(12)) saudacao = "Bom dia";
            else if (now < TimeSpan.FromHours(18)) saudacao = "Boa tarde";
            else saudacao = "Boa noite";


            await context.PostAsync($"{saudacao}!  Em que posso ajudar ?");
            context.Done<string>(null);
        }






        [LuisIntent("thank-bot")]
        public async Task Laugh(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($" Love to help! 😍😍 ");
            context.Done(true);

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

            context.Done(true);
        }
 
         





    }
}