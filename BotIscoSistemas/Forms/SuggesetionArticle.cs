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
    public class SuggesetionArticle
    {
        [Describe("Tipo do Artigo")]
        [Template(TemplateUsage.EnumSelectOne, "Qual tipo de {&} que gostaria que eu cria-se ? {||}", ChoiceStyle = ChoiceStyleOptions.Buttons)]
        public TipoArtigo TipoArtigo { get; set; }

        [Prompt("Qual o seu Nome ?")]
        [Describe("Nome")]
        public string Name { get; set; }

        [Prompt("Qual o seu E-Mail ?")]
        [Describe("Email")]
        [Pattern(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string Email { get; set; }


        [Prompt("Me fala um pouco sobre o artigo que você queria ver...")]
        [Optional]
        public string Describe { get; set; }

       
        public static IForm<SuggesetionArticle> BuildForm()
        {

            OnCompletionAsyncDelegate<CaptureLead> processOrder = async (context, state) =>
            {

            };

            var form = new FormBuilder<SuggesetionArticle>();
            form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Buttons;

            form.Configuration.Yes = new string[] { "sim", "yes", "s", "y", "yeap", "ok" };
            form.Configuration.No = new string[] { "nao", "não", "no", "not", "n" };
            form.Message("Vou lhe pedir alguns dados ok ?");
            //form.Confirm($"Está tudo correto ?\n" +
            //                "* Tipo do Bot: {TipoBot}\n" +
            //                "* Nome: {Name}\n" +
            //                "* Email: {Email}\n" +
            //                "* Possui Integração: {api}\n");

            //var form = new FormBuilder<CaptureLead>()
            //.Message("Vou lhe pedir alguns dados ok ?")
            //.Field(nameof(TipoBot))
            //.Field(nameof(Name))
            //.Field(nameof(Email))
            //.Field(nameof(Fone))
            //.Field(nameof(Describe))
            //    validate: async (state, value) =>
            //    {
            //        var result = new ValidateResult { IsValid = true, Value = "" };
            //        return result;
            //    })
            //.Message("For sandwich toppings you have selected {TipoBot}.")

            //.Confirm(async (state) =>
            //{
            //    var cost = 0.0;

            //    return new PromptAttribute($"Seu e-mail está Total for your sandwich is {cost:C2} is that ok?");
            //})
            //.AddRemainingFields()
            //.Confirm($"Está tudo correto ?\n" +
            //        "* Tipo do Bot: {TipoBot}\n" +
            //        "* Nome: {Name}\n" +
            //        "* Email: {Email}\n" +
            //        "* Possui Integração: {api}\n")
            ////.Message("Thanks for ordering a sandwich!")
            //.OnCompletion(processOrder);

            //form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Buttons;
            //form.Configuration.Yes = new string[] { "sim", "yes", "s", "y", "yeap", "ok" };
            //form.Configuration.No = new string[] { "nao", "não", "no", "not", "n" };

            return form.Build();


        }


        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Tipo de Artigo.: {0}, ", TipoArtigo);
            builder.AppendFormat("Nome: {0}: ", Name);
            builder.AppendFormat("Email: {0} ", Email);
            builder.AppendFormat("Descrição: {0} ", Describe);
            return builder.ToString();
        }

    }




}