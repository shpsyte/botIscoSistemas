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
    class RegisterBuy
    {
        [Describe(Description = "Opção de Pagamento.", Image = "http://www.iscosistemas.com/img/logo.png", Title = "Contratação Sistema", SubTitle = "Melhor custo benefício do mercado")]
        public OpcaoVendas OpcaoVendas;


        [Prompt("Qual seu CNPJ ?")]
        public string Cnpj;

        [Prompt("Qual sua inscrição estadual ?")]
        public string Ie;

        
        [Describe(Description = " informar seu endereço fiscal ", Title = "Endereço Fiscal", SubTitle = "Podemos usar da receita, basta dizer não...")]
        [Template(TemplateUsage.Confirmation)]
        public bool GetReceita;


        [Prompt("Qual CEP ? ")]
        public string Cep;
        [Prompt("Qual seu endereço ? ")]
        public string Address;
        [Prompt("Qual Número ? ")]
        public string Numero;
        [Prompt("Complemento")]
        public string Complemento;



        [Describe("Qual melhor dia para faturamento da mensalidade ?")]
        public int Dia;

        [Describe("Informe seu Nome")]
        public string Nome;
        [Describe("Informe seu Email")]
        [Pattern(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string Email;


        public static IForm<RegisterBuy> BuildForm()
        {


            var builder = new FormBuilder<RegisterBuy>();

            ActiveDelegate<RegisterBuy> notGetAddressFromReceita = (suporte) => suporte.GetReceita == true;
            
            return builder
                .Message("Legal, vou precisar de alguns dados")
                .Field(nameof(RegisterBuy.OpcaoVendas))
                //.Confirm(async (state) =>
                //{
                //    return new PromptAttribute($"A primeira parcela do contrato é a vista, ok ? {{||}}");
                //})
             
                .Field(nameof(RegisterBuy.Cnpj))
                .Field(nameof(RegisterBuy.Ie))
                .Field(nameof(RegisterBuy.GetReceita))
                .Field(nameof(RegisterBuy.Cep), notGetAddressFromReceita)
                .Field(nameof(RegisterBuy.Address), notGetAddressFromReceita)
                .Field(nameof(RegisterBuy.Numero), notGetAddressFromReceita)
                .Field(nameof(RegisterBuy.Complemento), notGetAddressFromReceita)
                .AddRemainingFields()
                .Confirm(async (state) =>
                 {
                     var cost = 0.0;
                     var data = DateTime.Now.AddDays(7).ToShortDateString();
                     

                     switch (state.OpcaoVendas)
                     {
                         case OpcaoVendas.Mensal: cost = 69.90;  break;
                         case OpcaoVendas.Anual: cost = 754.92; break;
                         case OpcaoVendas.Trimentral: cost = 199.23; break;
                     }
                      
                    
                     return new PromptAttribute($" Seu contrato ficou **{state.OpcaoVendas}**, com valor " +
                         $" de **{cost:C2}**, para validar o contrato a primeira parcela é gerada para 7 dias: **{data}**, " +
                         $" as demais parcelas ficaram para dia *{state.Dia}*, conforme sua escolha, tudo bem ? *ajuda* para exibir opções  {{||}}");
                 })
                .Confirm("Ok, já vou colocar na fila, mas é preciso **efetuar o pagamento da primeira parcela** para ter acesso ao sistema, ok ?")
                .Build();

        }


        public override string ToString()
        {
            string body = "";
            body = $"<h3> Registro de Compra </h3> <p> <b>Opção.:</b>  { OpcaoVendas } </p> " +
                $"  <p> <b>Cnpj.:</b>  { Cnpj } </p>  " +
                $"  <p> <b>Ie.:</b>  { Ie } </p>  " +
                $"  <p> <b>GetReceita.:</b>  { GetReceita } </p>  ";

            if (!string.IsNullOrEmpty(Cep))
                body += $"  <p> <b>Cep.:</b>  { Cep } </p>  ";
            if (!string.IsNullOrEmpty(Address))
                body += $"  <p> <b>Address.:</b>  { Address } </p>  ";
            if (!string.IsNullOrEmpty(Numero))
                body += $"  <p> <b>Numero.:</b>  { Numero } </p>  ";
            if (!string.IsNullOrEmpty(Complemento))
                body += $"  <p> <b>Complemento.:</b>  { Complemento } </p>  ";



            body += $"  <p> <b>Dia.:</b>  { Dia } </p>  " +
                $"  <p> <b>Nome.:</b>  { Nome } </p>  " +
                $"  <p> <b>Email.:</b>  { Email } </p>  ";
               



            return body;
        }

    }





}