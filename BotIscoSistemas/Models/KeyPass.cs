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
        internal static string _MsgHelp = $"Eu sou o **Agente Virtual da Isco.** Eu ainda estou em treinamento e eu adoraria ajudá-lo. \n" +
            " Você também pode pedir para conversar com uma pessoa a qualquer momento. Por favor, **descreva brevemente o problema** abaixo." ;

        

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
        public readonly static string _emailSuporte = "jose.luiz@iscosistemas.com";

    }
}