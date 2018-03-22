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
    public class RegisterSuportDialog : IDialog<object>
    {

        SendMsg _email = new SendMsg();


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
            // force Culture

            CultureInfo lang = new CultureInfo("pt-BR");
            
            Thread.CurrentThread.CurrentCulture = lang;
            Thread.CurrentThread.CurrentUICulture = lang;
            context.Activity.AsMessageActivity().Locale = lang.ToString();
            

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
                await context.PostAsync("Você cancelou, espero que conseguiu ajuda, mas se **precisar estou aqui** 😍 ");
                context.Done<string>(null);
                return;
            }

            if (order != null)
            {

                await context.PostAsync("Hum, não consegui identificar o problema, me da alguns **minutos** já resolvo para você e lhe respondo no **e-mail**, ta bom ?...");
                _email.SendEmailAsync(order.Name, 
                    "Suporte Técnico", 
                    order.ToString(), 
                    order.Email, 
                    KeyPassAndPhrase._emailSuporte, null, new string[] { "support@iscosistemas.zohosupport.com" }, null);
               
            }
            else
            {
                await context.PostAsync("Hum.. algo deu errado, por favor tente **novamente**");
            }

              context.Done<string>(null);

        }



    }
}