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
using BotBlog.Forms;
using System.Linq;
using Bot4App.Services;
using System.Globalization;
using System.Threading;

namespace Bot4App.Dialogs.Dialog
{
    [Serializable]
    public class RegisterBuyDialog : IDialog<object>
    {
        SendMsg _email = new SendMsg();
        public decimal discont { get; set; }

        public RegisterBuyDialog(decimal _discont)
        {
            this.discont = _discont;
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(SendConversation);
            return Task.CompletedTask;

        }




        private Task SendConversation(IDialogContext context, IAwaitable<object> result)
        {
            var activity = (context.Activity as Activity);
            (context.ConversationData).TryGetValue("User.Setting.Name", out string nome);
            (context.ConversationData).TryGetValue("User.Setting.Email", out string email);

            var capLeadForm = new RegisterBuy(nome, email);

            var form = new FormDialog<RegisterBuy>(capLeadForm, RegisterBuy.BuildForm, FormOptions.PromptInStart, null);
            context.Call<RegisterBuy>(form, FormCompleteCallback);
            return Task.CompletedTask;

        }

        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<RegisterBuy> result)
        {
            var activity = (context.Activity as Activity);

            RegisterBuy order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Puxa vida, que pena, se precisar estou por aqui... ;) ");
                return;
            }

            if (order != null)
            {
                (context.ConversationData).SetValue("User.Setting.Name", order.Nome);
                (context.ConversationData).SetValue("User.Setting.Email", order.Email);

                await _email.SendEmailAsync(nameCustomer: order.Nome,
                                            subject: "Nova Venda CREATELEAD",
                                            body: order.ToString(),
                                            to: KeyPassAndPhrase._emailVendas,
                                            replayto: order.Email,
                                            cc: new string[] { KeyPassAndPhrase._emailSuporte });

                await context.PostAsync("Ok, já estou criando sua base, como leva um tempinho, já aviso no seu **email** ou **telefone**, ta bom ?...");
                await context.PostAsync($"Obrigado pela confiança...{ order.Nome }");
                await context.PostAsync($" Love to help! 😍😍 ");

            }
            else
            {
                await context.PostAsync("Hum.. algo deu errado, por favor tente **novamente**");
            }

              context.Done<string>(null);

        }



    }
}