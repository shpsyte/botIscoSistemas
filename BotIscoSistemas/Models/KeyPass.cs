using Bot4App.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace BotBlog.Models
{
    public static class KeyPassAndPhrase
    {

        //msg
        internal static string _MsgNotUndertand = $"Hum... Estou aprendendo isso ainda, vou falar com o **pessoal da Isco** para me explicar isso, ok ? ";
        internal static string _MsgHelp = $"Oi, tudo bem ? eu sou a ***Ian*** 😎, sou uma *Agente Virtual.* \n" +
            $" Ainda estou em treinamento, mas adoraria ajudá-lo 😍😍. Por favor, **descreva brevemente** do que você precisa..." ;
        internal static string _GetContactInfo = $"Quer me deixar seu **Nome** e **Email**, para que eu🙂 possa lhe enviar *mais informações* ?";

        

      
        internal static string _IamHereForHelp = "Ok, sem problemas, mas estou aqui para te ajudar.. 😃 ";
        internal static string _OkImSorryButIamHere = "Puxa vida, que pena, 😐 se precisar estou por aqui... 😃 ";
        internal static string _NewLeadComming = "Possível Lead entrando! 😍😍";
        internal static string _SometinhgWrong  = "Hum..algo deu errado, 😐 por favor tente** novamente**";
        internal static string _ItsDoneContact = "Ok, enviado, já já aguém entra em contato..";



        public readonly static string _emailSuporte = "jose.luiz@iscosistemas.com";
        public readonly static string _emailSuportTicket = "support@iscosistemas.zohosupport.com";
        public readonly static string _emailAdmIan  = "jose.luiz@iscosistemas.com";
        public readonly static string _emailVendas = "fernanda.galvao@iscosistemas.com";
        public readonly static string _emailCopiaVendas = "";
        
         

        ///api isco
        public readonly static string endpointCustomer = $"http://www.api.iscosistemas.com.br/v1/Cliente/001/001/";




        //luis
        internal static string _LuisModelId = ConfigurationManager.AppSettings["LuisModelId"];
        internal static string _LuiSubscriptionKey = ConfigurationManager.AppSettings["LuiSubscriptionKey"];
        internal static string _LuiEndPoint = ConfigurationManager.AppSettings["EndPointLuis"];



        //qna 
        public readonly static string _QnaKnowledgedIscoId  = ConfigurationManager.AppSettings["QnaKnowledgedIscoId"];  
        public readonly static string _QnaSubscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionKey"]; 
        public readonly static double _Score = 0.3;
        public readonly static int _QtyAnswerReturn = 3;
        public readonly static int _TitleMoreInfo = 1;
        public readonly static int _TitleInfoSystem = 2;




        ///SendGrid & Email
        internal static string _sendGridKey = ConfigurationManager.AppSettings["SendGridKey"];
        internal static string _templateEmailSalesId  = "ae2e85f2-82e2-4a12-b393-bce14be35fcb";



        public readonly static string _host = ConfigurationManager.AppSettings["HostEmail"];
        public readonly static string _from = ConfigurationManager.AppSettings["FromEmail"];
        public readonly static string _userSmtp = ConfigurationManager.AppSettings["UserSmtp"];
        public readonly static string _passSmtp = ConfigurationManager.AppSettings["PassSmtp"];
        public readonly static string _portSmtp = ConfigurationManager.AppSettings["PortSmtp"];
        public readonly static int _timeOut = 10000;
        



        //twilo
        public readonly static string _twiloId = ConfigurationManager.AppSettings["TwiloId"];
        public readonly static string _twiloauthToken = ConfigurationManager.AppSettings["TwiloToken"];


    }
}