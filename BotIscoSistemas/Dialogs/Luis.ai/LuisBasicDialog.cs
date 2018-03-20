
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
        private readonly static string _MsgNotUndertand = KeyPassAndPhrase._MsgNotUndertand;
        private readonly static string _DefaultMsgHelp = KeyPassAndPhrase._MsgHelp;
        private static string body = $"Olá { _agent }, Favor entrar em contato <b>Urgente</b> com { _person } no Fone { _fone } ";
        SendMsg _sendMsg = new SendMsg();
       


        public LuisBasicDialog() : base(new LuisService(new LuisModelAttribute(_LuisModelId, _LuiSubscriptionKey, LuisApiVersion.V2)))
        {

        }



       


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NotUnderstod(IDialogContext context, LuisResult result)
        {
           // _sendMsg.SendEmailAsync("Oi, alguém fez esta pergunta e não soube responder...", $" <b> { (context.Activity as Activity).Text } </b>","jose.iscosistemas@gmail.com", "jose.iscosistemas@gmail.com", null);
            await context.PostAsync($"{_MsgNotUndertand}");
            await context.Forward(new GetContactInfoDialog(), ResumeAfterQnA, context.Activity, CancellationToken.None);
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

                string to = BestDestination.GetBestEmailTo(_agent);
                string foneto = BestDestination.GetBestFoneTo(_agent);
                await context.PostAsync($"Ok, já pedi para { _agent } entrarem em contato no fone { _fone }, obrigado ");

               // _sendMsg.SendEmailAsync("Solicitação de Contato",  body, to, to);
               // _sendMsg.SendSmsAsync($"Oi {_agent}, Favor Entrar em contato: {_person} no fone { _fone }", foneto);

            }

        }


        private async Task GetFone(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var fone = await value;
            string to = BestDestination.GetBestEmailTo(_agent);
            string foneto = BestDestination.GetBestFoneTo(_agent);
            _fone = fone.Text;

           // _sendMsg.SendEmailAsync("Solicitação de Contato", body, to, to);
           // _sendMsg.SendSmsAsync($"Oi {_agent}, Favor Entrar em contato: {_person} no fone { _fone }", foneto);
            await context.PostAsync($"Ok, já pedi para { _agent } entrarem em contato no fone { _fone }, obrigado ");

            context.Wait(MessageReceived);
        }


        [LuisIntent("feature-bot")]
        public async Task Feature(IDialogContext context, LuisResult result)
        {
            (context.ConversationData).SetValue("interesse", 1);
            await context.Forward(new QnaIscoSistemas(true), ResumeAfterQnA, context.Activity, CancellationToken.None);
            
        }
        



        [LuisIntent("sense-bot")]
        public async Task Sense(IDialogContext context, LuisResult result)
        {
            var userQuestion = (context.Activity as Activity).Text;
            await context.Forward(new QnaIscoSistemas(false), ResumeAfterQnA, context.Activity, CancellationToken.None);
            //public async Task None(IDialogContext context, LuisResult result) => await context.PostAsync($"{_MsgNotUndertand}\n{_DefaultMsgHelp}");
        }


        [LuisIntent("help-bot")]
        public async Task Help(IDialogContext context, LuisResult result) => await context.Forward(new RegisterSuportDialog(), ResumeAfterQnA, context.Activity, CancellationToken.None);




        [LuisIntent("buy-bot")]
        public async Task Buy(IDialogContext context, LuisResult result)
        {
            await context.Forward(new RegisterBuyDialog(0), ResumeAfterQnA, context.Activity, CancellationToken.None);
        }



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
            //await context.PostAsync($"{ FakeList.GetRandomLaugh()}  { FakeList.GetListRandomEmojiHappy(3) }");
            await context.PostAsync($" Love to help! 😍😍 ");
            context.Done<string>(null);

        }

        [LuisIntent("hate-bot")]
        public async Task Hat(IDialogContext context, LuisResult result) => await context.PostAsync($"{ FakeList.GetRandomHatPhrase()} { FakeList.GetListRandomEmojiAngry(6) }  ");


        [LuisIntent("joke-bot")]
        public async Task Joke(IDialogContext context, LuisResult result) => await context.PostAsync($"{ FakeList.GetRandomJoke()} { FakeList.GetListRandomEmojiHappy(6) } ");



         

        [LuisIntent("request-quote")]
        public async Task ForwardRequestQuote(IDialogContext context, LuisResult result) => await context.Forward(new RequestQuoteDialog(), ResumeAfterQnA, context.Activity, CancellationToken.None);


        [LuisIntent("suggestion-article")]
        public async Task SugestionArticle(IDialogContext context, LuisResult result) => await context.Forward(new SugestionArticleDialog(), ResumeAfterQnA, context.Activity, CancellationToken.None);


        [LuisIntent("pay-article")]
        public async Task PayArticle(IDialogContext context, LuisResult result)
        {
            var active = (context.Activity as Activity);
            await context.PostAsync("Ok, para criar uma propaganda, vou precisar alguns dados...");
            await MessagesController.SendBotIsTyping(active);
            await context.PostAsync("Vamos lá..");


            await context.Forward(new PayArticleDialog(), ResumeAfterQnA, context.Activity, CancellationToken.None);
                
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
 

        private Attachment GetAudioCard()
        {
            return new AudioCard
            {
                Title = "I am your father",
                Subtitle = "Star Wars: Episode V - The Empire Strikes Back",
                Text = "The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back) is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and Lawrence Kasdan wrote the screenplay, with George Lucas writing the film's story and serving as executive producer. The second installment in the original Star Wars trilogy, it was produced by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams, Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.",
                Autostart = true,
                Image = new ThumbnailUrl
                {
                    Url = "https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg"
                },
                Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = "http://www.wavlist.com/movies/004/father.wav"
                        }
                    },
                Buttons = new List<CardAction>
                    {
                        new CardAction()
                        {
                            Title = "Read More",
                            Type = ActionTypes.OpenUrl,
                            Value = "https://en.wikipedia.org/wiki/The_Empire_Strikes_Back"
                        }
                    }
            }.ToAttachment();
        }









    }
}