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
        public Task SendEmailAsync(string subject, string body, string replayto, string to, string templateId = null, string[] cc = null, string[] bcc = null)
        {
            return ExecuteEmailAsync(subject, body, replayto, to, templateId, cc, bcc);
          
        }

        public Task ExecuteEmailAsync(string subject, string body, string replayto, string to, string templateId, string[] cc = null, string[] bcc = null)
        {
            var client = new SendGridClient(KeyPassAndPhrase._sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(KeyPassAndPhrase._from),
                Subject = subject,
               // PlainTextContent = body,
                HtmlContent = string.Concat(HtmlEmailHeader, body, HtmlEmailFooter),
                ReplyTo = new EmailAddress(replayto),
                
            };
            msg.AddTo(new EmailAddress(to));
            if (!string.IsNullOrEmpty(templateId))
                msg.TemplateId = templateId;

            return client.SendEmailAsync(msg);

            

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
            //SmtpClient client = new SmtpClient(KeyPassAndPhrase._host, KeyPassAndPhrase._portSmtp);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.Credentials = new System.Net.NetworkCredential(KeyPassAndPhrase._userSmtp, KeyPassAndPhrase._passSmtp);
            //client.UseDefaultCredentials = false;
            //client.Timeout = KeyPassAndPhrase._timeOut;
            //return client.SendMailAsync(message);
        }




        // send
        public Task SendSmsAsync(string body, string to)
        {
            return ExecuteSmsAsync(body, to);


        }


        public Task ExecuteSmsAsync(string body, string to)
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
                _email.SendEmailAsync("Erro no twilo", e.InnerException.Message, "jose.luiz@iscosistemas.com", "jose.luiz@iscosistemas.com");


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



}