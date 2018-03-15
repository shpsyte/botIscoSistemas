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


    public class Email
    {



        public static Task SendEmail(string subject, string body, string from, string to, string[] cc = null)
        {
          
            if (string.IsNullOrEmpty(to))
                to = "jose.luiz@iscosistemas.com";

            MailMessage mail = new MailMessage("jose.luiz@iscosistemas.com", to, subject, body);
            mail.ReplyToList.Add(new MailAddress(from));
            mail.Priority = MailPriority.High;
           // mail.Sender = new MailAddress(from);



            if (cc.Length > 0)
            {
                foreach (var item in cc)
                {
                    mail.CC.Add(item);
                }
            }

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("jose.luiz@iscosistemas.com", "Jymkatana_6985");
            client.UseDefaultCredentials = true;
            client.Timeout = 1000;
            client.Host = "mail.iscosistemas.com";


            client.SendMailAsync(mail);
            //client.Send(mail);

            return null;

        }

        internal static Task SendEmail(string v1, string v2, string email, object emailSuporte, string[] v3)
        {
            throw new NotImplementedException();
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