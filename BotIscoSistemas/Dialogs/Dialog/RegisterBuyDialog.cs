﻿using Microsoft.Bot.Builder.Dialogs;
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
            CultureInfo lang = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = lang;
            Thread.CurrentThread.CurrentUICulture = lang;
            context.Activity.AsMessageActivity().Locale = lang.ToString();
            var capLeadForm = new RegisterBuy();
           // capLeadForm.Discount = useDiscont;

            string[] words = activity.Text.Split(' ');
            string[] nfe = new string[] { "nfe", "nota fiscal" };
            string[] nfce = new string[] { "nfce", "consumidor", "usuario" };

           

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
                await context.PostAsync("Ok, já estou criando sua base, como leva um tempinho, já aviso no seu **email** ou **telefone**, ta bom ?...");
                await context.PostAsync($"Obrigado pela confiança...{ order.Nome }");
                await context.PostAsync($" Love to help! 😍😍 ");

                _email.SendEmailAsync(order.Nome, 
                    "Nova Venda CREATELEAD",
                    order.ToString(), 
                    order.Email,
                    KeyPassAndPhrase._emailVendas, 
                    null, new string[] { "suporte@iscosistemas.com", "jose.luiz@iscosistemas.com", "support@iscosistemas.zohosupport.com"});
            }
            else
            {
                await context.PostAsync("Hum.. algo deu errado, por favor tente **novamente**");
            }

              context.Done<string>(null);

        }



    }
}