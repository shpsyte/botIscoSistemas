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
        //[Prompt("Qual o seu Telefone ?")]
        ////[Pattern(@"(\(\d{2}\))?\s*\d{3}(-|\s*)\d{4}")]
        //public string Fone;
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
                .Field(nameof(RegisterSuport.NumeroDaNota), isNF)
                .Confirm("Você digitou o número {NumeroDaNota} está correta ? ", isNF)
                .Field(nameof(RegisterSuport.Cliente), isCadastro)
                .Field(nameof(RegisterSuport.Titulo), isFinanceiro)
                .Field(nameof(RegisterSuport.Teamview), isTV)
                .Field(nameof(RegisterSuport.Empresa))
                .Field(nameof(RegisterSuport.Name))
                .Field(nameof(RegisterSuport.Email))
               // .Field(nameof(RegisterSuport.Fone))
               // .Confirm("O Teaview v12 {Teamview}, está correto ? ", isTV)
                //.Field(nameof(RegisterSuport.ContactTime), "Qual o horário que podemos conectar pelo Teamview ? {&}", isTV)
                .AddRemainingFields()
                //.Confirm("Ok, {Name}, já estou em contato {?at {DeliveryTime:t}}?")
                .Build();


        }


        public override string ToString()
        {
            string body = $"<h3> Suporte Técnico </h3> <p> <b>Empresa.:</b>  { Empresa } </p> " +
                $"  <p> <b>Empresa.:</b>  { Empresa } </p>  " +
                $"  <p> <b>Nome.:</b>  { Name } </p>  " +
                $"  <p> <b>Email.:</b>  { Email } </p>  " +
                $"  <p> <b>Assunto Inicial.:</b>  { Assunto } </p>  " +
                $"  <p> <b>TipoProblema.:</b>  { TipoProblema } </p>  ";


            if (!string.IsNullOrEmpty(AccessOption)) body += $"  <p> <b>Tipo Acesso.:</b>  { AccessOption } </p>  ";
            if (!string.IsNullOrEmpty(UserName)) body += $"  <p> <b>User Name.:</b>  { UserName } </p>  ";
            if (!string.IsNullOrEmpty(NumeroDaNota)) body += $"  <p> <b>Nota.:</b>  { NumeroDaNota } </p>  ";
            if (!string.IsNullOrEmpty(Cliente)) body += $"  <p> <b>Cliente.:</b>  { Cliente } </p>  ";
            if (!string.IsNullOrEmpty(Titulo)) body += $"  <p> <b>Titulo.:</b>  { Titulo } </p>  ";
            if (!string.IsNullOrEmpty(Teamview)) body += $"  <p> <b>Teamview.:</b>  { Teamview } </p>  ";
            if (!string.IsNullOrEmpty(Describe)) body += $"  <p> <b>Describe.:</b>  { Describe } </p>  ";


            return body;
        }

    }





}