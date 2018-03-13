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

namespace Bot4App.Dialogs.Luis.ai
{
    [Serializable]
    public class RequestQuoteDialog : LuisDialog<object>
    {
        private readonly static string _LuisModelId = KeyPassAndPhrase._LuisModelId;
        private readonly static string _LuiSubscriptionKey = KeyPassAndPhrase._LuiSubscriptionKey;
        private readonly static string _MsgNotUndertand = KeyPassAndPhrase._MsgNotUndertand;
        private readonly static string _DefaultMsgHelp = KeyPassAndPhrase._MsgHelp;


        public RequestQuoteDialog() : base(new LuisService(new LuisModelAttribute(_LuisModelId, _LuiSubscriptionKey, LuisApiVersion.V2)))
        {

        }



        [LuisIntent("request-quote")]
        public async Task RequestQuoteForm(IDialogContext context, LuisResult result)
        {


            var activity = (context.Activity as Activity);
            var capLeadForm = new CaptureLead();
            var entities = new List<EntityRecommendation>(result.Entities);

            var form = new FormDialog<CaptureLead>(capLeadForm, CaptureLead.BuildForm, FormOptions.PromptInStart, entities);
            context.Call<CaptureLead>(form, CaptureLeadComplete);

            //await Conversation.SendAsync(activity, () => Chain.From(() => FormDialog.FromForm(() => CaptureLead.BuildForm(), FormOptions.PromptFieldsWithValues)));


        }


        private async Task CaptureLeadComplete(IDialogContext context, IAwaitable<CaptureLead> result)
        {
            var activity = (context.Activity as Activity);

            CaptureLead order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            if (order != null)
            {


                await context.PostAsync("Ok, enviado, logo algum representante lhe fará contato., lembre-se pode digitar **ajuda** \n" +
                    "Posso ajudar em algo mais ?");

                await Services.Email.SendEmail("Sugstão de Artigo", order.ToString());

                
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }


    }
}