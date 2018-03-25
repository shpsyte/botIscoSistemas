using BotBlog.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace Bot4App.Services
{

    public class BestDestination
    {
        public static string GetBestEmailTo(string agent)
        {


            string best = KeyPassAndPhrase._emailSuporte;

            if (!string.IsNullOrEmpty(agent))
            {
                
                string[] fernanda = new string[] { "fernanda", "fer", "venda", "vendas", "financeiro" };
                string[] jose     = new string[] { "ze", "jose", "luis", "luiz", "suporte", "tecnico" };

                foreach (var item in fernanda)
                    if (agent.ToLower().Contains(item))
                        best = KeyPassAndPhrase._emailVendas;

                foreach (var item in jose)
                    if (agent.ToLower().Contains(item))
                        best = KeyPassAndPhrase._emailSuporte;

            }

            return best;
        }

        public static string GetBestFoneTo(string to)
        {
            string best = "41999325815";

            if (!string.IsNullOrEmpty(to))
            {
                to = to.ToLower();
                string[] fer = new string[] { "fernanda", "fer", "venda", "vendas", "fianceiro" };

                foreach (var item in fer)
                    if (item.ToLower().Contains(to))
                        best = "41996030814";
            }
            return best;
        }


    }

    [Serializable]
    public class SendMsg
    {


        //private static SmtpClient SetupEmail(string account = null, string pass = null, string host = null)
        //{
        //    SmtpClient client = new SmtpClient();
        //    client.Port = 587;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    //            client.Credentials = new System.Net.NetworkCredential("");
        //    client.UseDefaultCredentials = false;
        //    client.Timeout = 10000;
        //    client.Host = string.IsNullOrEmpty(host) ? "mail.iscosistemas.com" : host;
        //    return client;
        //}


        // constants
        private const string HtmlEmailHeader = "<html><head><title></title></head><body style='font-family:arial; font-size:14px;'>";
        private const string HtmlEmailFooter = "</body></html>";




        // send
        public Task SendEmailAsync(string nameCustomer = null, string subject = null, string body = null, string replayto = null, string to = null, string templateId = null, string[] cc = null, string[] bcc = null)
        {
            return  ExecuteEmailAsync(nameCustomer, subject, body, replayto, to, templateId, cc, bcc);
            
        }


        // send
        public Task SendSmsAsync(string body, string to)
        {


            return ExecuteSmsAsync(body, to);


        }




        private Task ExecuteEmailAsync(string nameCustomer, string subject, string body, string replayto, string to, string templateId, string[] cc = null, string[] bcc = null)
        {
            var client = new SendGridClient(KeyPassAndPhrase._sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new SendGrid.Helpers.Mail.EmailAddress(KeyPassAndPhrase._from, "Ian da Isco Sistemas"),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = string.Concat(HtmlEmailHeader, body, HtmlEmailFooter),
                ReplyTo = new EmailAddress(string.IsNullOrEmpty(replayto) ? to : replayto),
                
                
            };
            msg.AddTo(new EmailAddress(to));


            if (!string.IsNullOrEmpty(templateId))
                msg.SetTemplateId(templateId);

            msg.SetSubject(subject);

            if (cc != null)
                foreach (var item in cc)
                    msg.AddCc(new SendGrid.Helpers.Mail.EmailAddress(item.ToLower().Trim()));


            //if (bcc != null)
            //    foreach (var item in bcc)
            //        msg.AddBcc(new SendGrid.Helpers.Mail.EmailAddress(item));



            if (!string.IsNullOrEmpty(nameCustomer))
                msg.AddSubstitution("-name-", nameCustomer);

            string proximo = (new Random()).Next(1000,5000).ToString();
            var cupom = string.Concat(
                    (nameCustomer.Length > 3 ? nameCustomer.Substring(1, 3) : nameCustomer.Substring(1, nameCustomer.Length)),
                    proximo);
                     

            msg.AddSubstitution("-cupom-", cupom);
             

            return  client.SendEmailAsync(msg);
            //return email;
           


            //var myMessage = new SendGridMessage()
            //{
            //    HtmlContent = string.Concat(HtmlEmailHeader, body, HtmlEmailFooter),
            //};
            //myMessage.AddTo(to);
            //myMessage.
            //myMessage.Subject = subject;
            //myMessage.PlainTextContent = body;
            //return client.SendEmailAsync(myMessage);


            //MailMessage mail = new MailMessage("jose.luiz@iscosistemas.com", "jose.luiz@iscosistemas.com");
            //SmtpClient client = new SmtpClient();
            //client.Port = 587;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Timeout = 10000;
            //client.Credentials = new System.Net.NetworkCredential("jose.luiz@iscosistemas.com", "Jymkatana_6985");
            //client.Host = "mail.iscosistemas.com";
            //mail.Subject = subject;
            //mail.From = new MailAddress(KeyPassAndPhrase._from, "Ian da Isco Sistemas");
            //mail.Body = string.Concat(HtmlEmailHeader, body, HtmlEmailFooter);
            //mail.BodyEncoding = Encoding.UTF8;
            //mail.SubjectEncoding = Encoding.UTF8;
            //mail.IsBodyHtml = true;
            //mail.Priority = MailPriority.High;
            //mail.ReplyToList.Add(new MailAddress(replayto));

            //mail.Body = body;
            //return client.SendMailAsync(mail);



            //static async Task Execute()
            //{
            //    var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            //    var client = new SendGridClient(apiKey);
            //    var msg = new SendGridMessage();
            //    msg.SetFrom(new EmailAddress("test@example.com", "Example User"));
            //    msg.SetSubject("I'm replacing the subject tag");
            //    msg.AddTo(new EmailAddress("test@example.com", "Example User"));
            //    msg.AddContent(MimeType.Text, "I'm replacing the <strong>body tag</strong>");
            //    msg.SetTemplateId("13b8f94f-bcae-4ec6-b752-70d6cb59f932");
            //    msg.AddSubstitution("-name-", "Example User");
            //    msg.AddSubstitution("-city-", "Denver");
            //    var response = await client.SendEmailAsync(msg);
            //    Console.WriteLine(response.StatusCode);
            //    Console.WriteLine(response.Headers.ToString());
            //    Console.WriteLine("\n\nPress any key to exit.");
            //    Console.ReadLine();
            //}




        }





        private Task ExecuteSmsAsync(string body, string to)
        {

            try
            {
                TwilioClient.Init(KeyPassAndPhrase._twiloId, KeyPassAndPhrase._twiloauthToken);
                var too = new PhoneNumber(string.Concat("+55", to));
                var message = MessageResource.Create(
                    too,
                    from: new PhoneNumber("12678332952"),
                    body: $" { body }");
            }
            catch (Exception e)
            {

                this.SendEmailAsync("twilo", "Erro no twilo", e.InnerException.Message, "jose.luiz@iscosistemas.com", "jose.luiz@iscosistemas.com");


            }

            return Task.CompletedTask;
        }

    }




    public class ValidaDocumento
    {
        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            tempCnpj = cnpj.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }

        public static string getNumber(string s)
        {
            return Regex.Match(s, @"\d+").Value;
        }


    }



    public class Feature
    {


        public static string GetRandomFeature(string notThis)
        {
            Random rnd = new Random();
            List<string> list = ListGenericFeature();
            list.Remove(notThis);

            if (list.Count() > 0)
            {
                int r = rnd.Next(list.Count);

                return list[r];
            }
            else
            {
                return "not-anymore";
            }
        }


        public static List<string> ListGenericFeature()
        {

            List<string> list = new List<string>();

            list.Add("Em nuvem!|Nosso **sistema** roda em nuvem, você não precisa instalar nada...|https://media.giphy.com/media/aQBFZ5v7dleKs/giphy.gif|");
            list.Add("Nota Ilimitadas!|Você tem notas ilimitadas, não se preocupe com limites..|https://gph.is/2r87BhL");
            list.Add("Facilidade que ajuda|A emissão de notas é muito, muito fácil... |https://gph.is/1qzIOOt");
            list.Add("Envio automático|Agente transmite oarquivo **XML** e PDF para o cliente...|https://gph.is/1qzIOOt");
            list.Add("Seu contador agradece|O contador também recebe o lote de notas emitidas no mês |https://gph.is/H3imLo");
            list.Add("Nota Grande ou para consumidor|Você pode emitir Nf-e e Nfc-e, dentro do mesmo sistema|https://gph.is/1cLzUXa");
            list.Add("Preço justo|Além disso, custa apartir de R$ 69,90 / mês , muito barato|https://gph.is/1dYQ7si");
            list.Add("Não fique sem suporte|Nosso suporte é gratuito e ilimitado... Isso é segurança!|https://gph.is/1maiWUY");
            list.Add("Funções rápidas que facilitam|Possuímos a função cópia de Nota, que facilita a devolução de mercadorias|https://media.giphy.com/media/8Pdy3Dn7Wxd0jsRi21/giphy.gif");
            list.Add("100% Homologado NF 4.0|Nosso sistema é homologado pela receita, é muita segurança para seu negócio|https://media.giphy.com/media/1lzgw4kIm6Ig6bNQPR/giphy.gif");
            list.Add("Está pronto|Basta você virar a chave e sair trabalhando.|");

            return list;

        }





        public static string GetRandomListGenericLoadMore(string notThis)
        {
            Random rnd = new Random();
            List<string> list = ListGenericLoadMore();
            list.Remove(notThis);


            int r = rnd.Next(list.Count);

            return list[r];

        }


        private static List<string> ListGenericLoadMore()
        {

            List<string> list = new List<string>();

            list.Add("Quer mais informação ***sobre o sistema*** ? ");
            list.Add("Posso lhe falar mais ***detalhe*** ?");
            list.Add("Quer que eu busque mais ***informação***?");
            list.Add("Posso ajudar com ***mais informação***, deseja ver ?");

            return list;

        }


    }


}