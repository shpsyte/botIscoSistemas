using Bot4App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotBlog.Models
{
    public static class KeyPassAndPhrase
    {

        //msg
        internal static string _MsgNotUndertand = $"Hum... Estou aprendendo isso ainda, vou falar com o **pessoal da Isco** para me explicar isso, ok ? ";
        internal static string _MsgHelp = $"Oi, tudo bem ? eu sou a ***Ian*** 😎, sou uma *Agente Virtual.* \n" +
            $" Ainda estou em treinamento, mas adoraria ajudá-lo 😍😍. Por favor, **descreva brevemente** do que você precisa..." ;
        internal static string _GetContactInfo = $"Quer me deixar seu **Nome** e **Email**, para alguém (mais esperto que eu, 🙂) lhe enviar *mais informações* ?";

        

      
        internal static string _IamHereForHelp = "Ok, sem problemas, mas estou aqui para te ajudar.. 😃 ";
        internal static string _OkImSorryButIamHere = "Puxa vida, que pena, 😐 se precisar estou por aqui... 😃 ";
        internal static string _NewLeadComming = "Possível Lead entrando! 😍😍";
        internal static string _SometinhgWrong  = "Hum..algo deu errado, 😐 por favor tente** novamente**";
        internal static string _ItsDoneContact = "Ok, enviado, já já aguém entra em contato..";



        //luis
        internal static string _LuisModelId = "c0eabe53-c8cc-4a05-bdde-052b434b95b3";
        internal static string _LuiSubscriptionKey = "a8046f7776b7494db8f1ea873eac5c3e";



        //qna 
        public readonly static string _QnaKnowledgedIscoId  = "c2a14c09-4859-4a46-84de-2ce0cd01f6b1";
        

        public readonly static string _QnaSubscriptionKey = "ac1a50f16e0b400f93e81d87eea081fc"; 
        public readonly static double _Score = 0.3;
        public readonly static int _QtyAnswerReturn = 3;



        ///api isco
        public readonly static string endpointCustomer = $"http://www.api.iscosistemas.com.br/v1/Cliente/001/001/";

        ///Email Setup Config
        public readonly static string _emailSuporte = "jose.luiz@iscosistemas.com";//"support@iscosistemas.zohosupport.com";
        public readonly static string _emailVendas = "jose.iscosistemas@gmail.com";
        internal static string _emailCopiaVendas = "";//"jose.iscosistemas@zoho.com";

        public readonly static string _host = "mail.iscosistemas.com.br";
        public readonly static string _from = "jose.luiz@iscosistemas.com";
        public readonly static string _userSmtp = "jose.luiz@iscosistemas.com";
        public readonly static string _passSmtp = "Jymkatana_6985";
        public readonly static int _portSmtp = 587;
        public readonly static int _timeOut = 10000;
        


        //jose.luiz@iscosistemas.com


        //documentodb
        public readonly static string _cosmoDburi = "https://botiscosistemas.documents.azure.com:443/";
        public readonly static string _cosmokey = "CMzzDGk4BaamsigNeUmhgXPX5wdp7P0ESUVwVYw7tiw6jVjir4xMurdaIFhQbDIYDYQ9XC0mX4maoiJFiXAnww==";


        //twilo
        public readonly static string _twiloId = "ACac0aa58b65b7d8dc6976c4cb4c9b3534";
        public readonly static string _twiloauthToken = "f1f39694c8b708c43250c4b836999d5f";
    }
}