using BotBlog.Forms;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Bot4App.Forms
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Hum, esta opção não é valida \"{0}\".")]
    public class CaptureLead
    {
        
        [Prompt("Qual o seu Nome ?")]
        [Describe("Nome")]
        public string Name { get; set; }

        [Prompt("Qual o seu E-Mail ?")]
        [Describe("Email")]
        [Pattern(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string Email { get; set; }
 

        [Prompt("Me fala um pouco sobre o que você precisa...")]
        [Optional]
        public string Describe { get; set; }

       
        public static IForm<CaptureLead> BuildForm()
        {

            OnCompletionAsyncDelegate<CaptureLead> processOrder = async (context, state) =>
            {

            };
            var builder = new FormBuilder<CaptureLead>();
            return builder
               .Message("Legal, vou precisar de alguns dados")
               .AddRemainingFields()
               .Build();
        }


        public override string ToString()
        {
            string body = "";
            body = $"<h3> Lead  </h3> <p> <b>Nome.:</b>  { Name } </p> " +
                $"  <p> <b>Email.:</b>  { Email } </p>  " +
                $"  <p> <b>Obs.:</b>  { Describe } </p>  ";




            return body;
        }

    }





}