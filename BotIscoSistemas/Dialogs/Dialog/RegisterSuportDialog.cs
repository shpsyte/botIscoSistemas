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

namespace Bot4App.Dialogs.Dialog
{
    [Serializable]
    public class RegisterSuportDialog : IDialog<object>
    {
        //private readonly static string _LuisModelId = KeyPassAndPhrase._LuisModelId;
        //private readonly static string _LuiSubscriptionKey = KeyPassAndPhrase._LuiSubscriptionKey;
        //private readonly static string _MsgNotUndertand = KeyPassAndPhrase._MsgNotUndertand;
        //private readonly static string _DefaultMsgHelp = KeyPassAndPhrase._MsgHelp;


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(SendConversation);
            return Task.CompletedTask;
        }




        private Task SendConversation(IDialogContext context, IAwaitable<object> result)
        {


            var activity = (context.Activity as Activity);
            string[] words = activity.Text.Split(' ');
            string[] nf = new string[] { "nf", "nota", "fiscal", "nfe", "nfce" };
            string[] acesso = new string[] { "acesso", "senha", "usuario" };
            string[] financeiro = new string[] { "titulo", "boleto", "financeiro", "título" };
            string[] cadastrar = new string[] { "cadastrar", "cliente", "produto" };




            var capLeadForm = new RegisterSuport();



            foreach (var b in words)
            {
                foreach (var a in nf)

                    if (a.ToLower().Equals(b.ToLower()))
                        capLeadForm.TipoProblema = TipoProblema.NF;

                foreach (var a in acesso)

                    if (a.ToLower().Equals(b.ToLower()))
                        capLeadForm.TipoProblema = TipoProblema.Acesso;


                foreach (var a in financeiro)

                    if (a.ToLower().Equals(b.ToLower()))
                        capLeadForm.TipoProblema = TipoProblema.Financeiro;

                foreach (var a in cadastrar)
                    if (a.ToLower().Equals(b.ToLower()))
                        capLeadForm.TipoProblema = TipoProblema.Cadastro;


            }

            capLeadForm.Assunto = activity.Text;


            var form = new FormDialog<RegisterSuport>(capLeadForm, RegisterSuport.BuildForm, FormOptions.PromptInStart, null);
            context.Call<RegisterSuport>(form, FormCompleteCallback);
            return Task.CompletedTask;

        }

        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<RegisterSuport> result)
        {
            var activity = (context.Activity as Activity);

            RegisterSuport order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Você cancelou, espero que conseguiu sua ajuda, mas se precisar estou aqui, ;) ");
                return;
            }

            if (order != null)
            {
                await context.PostAsync("Ok, está feito, o **suporte** já recebeu este chamado \n," +
                    " agora é só aguardar o atendimento.");

                await Services.Email.SendEmail("Suporte Técnico ", order.ToString(), order.Email, KeyPassAndPhrase._emailSuporte, new string[] { "jose.luiz@iscosistemas.com" });

            }
            else
            {
                await context.PostAsync("Hum.. algo deu errado, por favor tente **novamente**");
            }

            // context.Done<string>(null);

        }



    }
}