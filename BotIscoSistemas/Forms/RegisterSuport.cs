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
    class RegisterSuport
    {

        [Prompt("Qual é sua Empresa ?")]
        public string Empresa;
        [Prompt("Qual o seu Nome ?")]
        public string Name;
        [Prompt("Qual o seu E-Mail ?")]
        [Pattern(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        [Template(TemplateUsage.NotUnderstood, "Este email não me parece certo, pode confirmar ?  \"{0}\".")]
        public string Email;
        [Prompt("Qual o seu Telefone ?")]
        //[Pattern(@"(\(\d{2}\))?\s*\d{3}(-|\s*)\d{4}")]
        public string Fone;
        [Describe("Área")]
        [Template(TemplateUsage.EnumSelectOne, "Qual {&} do sistema está com problema ? {||}", ChoiceStyle = ChoiceStyleOptions.Buttons)]
        public TipoProblema TipoProblema;
        [Describe("Assunto")]
        public string Assunto;
        [Describe("Número da Nota com Problema")]
        public string NumeroDaNota;
        [Describe("Quais os dados do Cliente, como CNPJ ou Código")]
        public string Cliente;
        [Describe("Me de detalhes do título, como Número, Valor e Cliente")]
        public string Titulo;
        [Describe("Pode informar o Teamview junto com a Senha ?")]
        public string Teamview;
        [Prompt("Ok, quer me falar algo mais, como detalhes ?")]
        [Optional]
        public string Describe;
        [Optional]
        [Template(TemplateUsage.StatusFormat, "{&}: {:t}", FieldCase = CaseNormalization.None)]
        public DateTime? ContactTime;
        [Describe("Qual problema com acesso ? ")]
        public string AccessOption;
        [Describe("Qual o usuário está usando ? ")]
        public string UserName;

        public static IForm<RegisterSuport> BuildForm()
        {

            OnCompletionAsyncDelegate<RegisterSuport> processOrder = async (context, state) =>
            {

            };

            //var form = new FormBuilder<RegisterSuport>();
            //form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Buttons;

            //form.Configuration.Yes = new string[] { "sim", "yes", "s", "y", "yeap", "ok" };
            //form.Configuration.No = new string[] { "nao", "não", "no", "not", "n" };
            //form.Message("Vou lhe pedir alguns dados ok ?");


            var builder = new FormBuilder<RegisterSuport>();

            ActiveDelegate<RegisterSuport> isNF = (suporte) => suporte.TipoProblema == TipoProblema.NF;
            ActiveDelegate<RegisterSuport> isOutro = (suporte) => suporte.TipoProblema == TipoProblema.Outro;
            ActiveDelegate<RegisterSuport> isCadastro = (suporte) => suporte.TipoProblema == TipoProblema.Cadastro;
            ActiveDelegate<RegisterSuport> isFinanceiro = (suporte) => suporte.TipoProblema == TipoProblema.Financeiro;
            ActiveDelegate<RegisterSuport> isAcesso = (suporte) => suporte.TipoProblema == TipoProblema.Acesso;
            ActiveDelegate<RegisterSuport> isTV = (suporte) => ( suporte.TipoProblema == TipoProblema.Acesso || suporte.TipoProblema == TipoProblema.NF);
            ActiveDelegate<RegisterSuport> isAccuserBo = (suporte) => (suporte.TipoProblema == TipoProblema.Acesso && suporte.AccessOption == "senha");

            return builder
                 // .Field(nameof(PizzaOrder.Choice))
                .Field(nameof(RegisterSuport.TipoProblema))
                .Field(new FieldReflector<RegisterSuport>(nameof(AccessOption))
                        .SetType(null)
                        .SetActive((state) => state.TipoProblema == TipoProblema.Acesso)
                        .SetDefine(async (state, field) =>
                        {
                            field
                                .AddDescription("Atualizar", "Atualizar meu Sistema")
                                .AddTerms("Atualizar", "renovar", "liberar", "Atualizar meu Sistema")
                                .AddDescription("Senha", "Problemas com minha senha")
                                .AddTerms("Senha", "Problemas com minha senha", "reset senha");
                            return true;
                        }))
                .Field(nameof(RegisterSuport.UserName), isAccuserBo)
                .Field(nameof(RegisterSuport.Empresa))
                .Field(nameof(RegisterSuport.Name))
                .Field(nameof(RegisterSuport.Email))
                .Field(nameof(RegisterSuport.Fone))
                .Field(nameof(RegisterSuport.NumeroDaNota), isNF)
                .Confirm("Você digitou o número {NumeroDaNota} está correta ? ", isNF)
                .Field(nameof(RegisterSuport.Cliente), isCadastro)
                .Field(nameof(RegisterSuport.Titulo), isFinanceiro)
                .Field(nameof(RegisterSuport.Teamview), isTV)
               // .Confirm("O Teaview v12 {Teamview}, está correto ? ", isTV)
                .Field(nameof(RegisterSuport.ContactTime), "Qual o horário que podemos conectar pelo Teamview ? {||}", isTV)
                .AddRemainingFields()
                //.Confirm("Ok, {Name}, já estou em contato {?at {DeliveryTime:t}}?")
                .Build();


        }


        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Suporte da Empresa: {0}, \n ", Empresa);
            builder.AppendFormat("Nome: {0}, \n ", Name);
            builder.AppendFormat("Email:  {0}, \n ", Email);
            builder.AppendFormat("Fone:  {0}, \n ", Fone);
            builder.AppendFormat("TipoProblema:  {0}, \n ", TipoProblema);

            if (!string.IsNullOrEmpty(AccessOption))
                builder.AppendFormat(" Tipo do Erro acesso:  {0}, \n ", AccessOption);

            if (!string.IsNullOrEmpty(UserName))
                builder.AppendFormat(" Usuário:  {0}, \n ", UserName);


            builder.AppendFormat("Assunto: {0}, \n ", Assunto);
            if (!string.IsNullOrEmpty(NumeroDaNota))
               builder.AppendFormat("Nota: {0}, \n ", NumeroDaNota);

            if (!string.IsNullOrEmpty(Cliente))
                builder.AppendFormat(" Cliente:  {0}, \n ", Cliente);

            if (!string.IsNullOrEmpty(Titulo))
                builder.AppendFormat(" Titulo  {0}, \n ", Titulo);

            if (!string.IsNullOrEmpty(Teamview))
                builder.AppendFormat(" Teamview:  {0}, \n ", Teamview);


            if (!string.IsNullOrEmpty(Describe))
                builder.AppendFormat(" Describe: {0}, \n ", Describe);

            if (ContactTime.HasValue)
                builder.AppendFormat(" Retornar as:  {0}, \n ", ContactTime);




            return builder.ToString();
        }

    }





}