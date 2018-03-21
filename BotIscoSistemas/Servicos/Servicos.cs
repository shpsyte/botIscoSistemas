using BotBlog.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
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
    public class AnaliseSentimentoEmTexto
    {
        private readonly string _textAnalisekey = ConfigurationManager.AppSettings["TextAnalysesKey"];
        private readonly string _textAnaliseUri = ConfigurationManager.AppSettings["TextAnaliseUri"];

        public async Task<string> TextAnalytics(string texto)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _textAnalisekey);

            var uri = "https://westcentralus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + texto;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response = await client.PostAsync(uri, content);
            }




            var result = await response.Content.ReadAsStringAsync();
            var contents = XElement.Parse(result).Value;

            return $"Texto original: **{ texto }**\nTradução: **{ contents }**";
        }
    }


    public class Translate
    {
        private readonly string _translateApiKey = "0bdfbf416967418dbdfd66c3d2a12466"; //ConfigurationManager.AppSettings["TranslateApiKey"];
        private readonly string _translateUri = "https://api.microsofttranslator.com/V2/Http.svc/Translate";  //ConfigurationManager.AppSettings["TranslateUri"];

        public async Task<string> TranslateText(string texto)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _translateApiKey);

            var uri = _translateUri + "?to=pt-br" +
                      "&text=" + System.Net.WebUtility.UrlEncode(texto);

            var response = await client.GetAsync(uri);
            var result = await response.Content.ReadAsStringAsync();
            var content = XElement.Parse(result).Value;

            return $"Tradução: **{ content }**";
        }
    }



    public class BestDestination
    {


        public static string GetBestEmailTo(string to)
        {
            string best = "jose.luiz@iscosistemas.com.br";
            to = to.ToLower();
            string[] fer = new string[] { "fernanda", "fer", "venda", "vendas", "fianceiro" };

            foreach (var item in fer)
                if (item.ToLower().Contains(to))
                    best = "fernanda.galvao@iscosistemas.com.br";

            return best;
        }

        public static string GetBestFoneTo(string to)
        {
            string best = "41999325815";
            to = to.ToLower();
            string[] fer = new string[] { "fernanda", "fer", "venda", "vendas", "fianceiro" };

            foreach (var item in fer)
                if (item.ToLower().Contains(to))
                    best = "41996030814";

            return best;
        }


    }

    [Serializable]
    public class SendMsg
    {


        private static SmtpClient SetupEmail(string account = null, string pass = null, string host = null)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //            client.Credentials = new System.Net.NetworkCredential("");
            client.UseDefaultCredentials = false;
            client.Timeout = 10000;
            client.Host = string.IsNullOrEmpty(host) ? "mail.iscosistemas.com" : host;
            return client;
        }


        // constants
        private const string HtmlEmailHeader = "<html><head><title></title></head><body style='font-family:arial; font-size:14px;'>";
        private const string HtmlEmailFooter = "</body></html>";





        // send
        public Task SendEmailAsync(string nameCustomer, string subject, string body, string replayto, string to, string templateId = null, string[] cc = null, string[] bcc = null)
        {
          //  return null;
            return ExecuteEmailAsync(nameCustomer, subject, body, replayto, to, templateId, cc, bcc);

        }


        // send
        public Task SendSmsAsync(string body, string to)
        {

          
           return ExecuteSmsAsync(body, to);


        }




        private Task ExecuteEmailAsync(string nameCustomer,string subject, string body, string replayto, string to, string templateId, string[] cc = null, string[] bcc = null)
        {
            var client = new SendGridClient(KeyPassAndPhrase._sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(KeyPassAndPhrase._from, "Ian da Isco Sistemas"),
                Subject = subject,
                // PlainTextContent = body,
                HtmlContent = string.Concat(HtmlEmailHeader, body, HtmlEmailFooter),
                ReplyTo = new EmailAddress(replayto),

            };
            msg.AddTo(new EmailAddress(to));
            if (!string.IsNullOrEmpty(templateId))
                msg.SetTemplateId(templateId);

            msg.SetSubject(subject);

            if (!string.IsNullOrEmpty(nameCustomer))
            msg.AddSubstitution("-name-", nameCustomer);

            return client.SendEmailAsync(msg);

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

            //MailMessage message = new MailMessage(KeyPassAndPhrase._from, to);
            //message.Subject = subject;
            //message.Body = string.Concat(HtmlEmailHeader, body, HtmlEmailFooter);
            //message.BodyEncoding = Encoding.UTF8;
            //message.SubjectEncoding = Encoding.UTF8;
            //message.IsBodyHtml = true;
            //message.Priority = MailPriority.High;
            //message.ReplyToList.Add(new MailAddress(replayto));
            //if (cc != null && cc.Length > 0) foreach (var x in cc) message.CC.Add(x);
            //if (bcc != null && bcc.Length > 0) foreach (var x in bcc) message.Bcc.Add(x);
            //SmtpClient client = new SmtpClient(KeyPassAndPhrase._host, Convert.ToInt32(KeyPassAndPhrase._portSmtp));
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.Credentials = new System.Net.NetworkCredential(KeyPassAndPhrase._userSmtp, KeyPassAndPhrase._passSmtp);
            //client.UseDefaultCredentials = false;
            //client.Timeout = KeyPassAndPhrase._timeOut;
            //return client.SendMailAsync(message);
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
                SendMsg _email = new SendMsg();
                _email.SendEmailAsync("twilo", "Erro no twilo", e.InnerException.Message, "jose.luiz@iscosistemas.com", "jose.luiz@iscosistemas.com");


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
            }else
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