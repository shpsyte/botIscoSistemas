using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotBlog.Models
{
    public static class KeyPassAndPhrase
    {

        //msg
        internal static string _MsgNotUndertand = $"Hum... Minha conciência não entende isso ainda, mas com certeza aprenderei mais sobre isso... ";
        internal static string _MsgHelp = $"Posso te ajuda a :\n" +
            "* **Encontrar artigos no blog**, \n" +
            "* **Registrar Sugestão de artigos**, \n" +
            "* **Contratar consultoria**, \n" +
            "* **Anunciar conosco**, \n";

        //luis
        internal static string _LuisModelId = "646ff876-0858-4d57-9f6c-78e995779540";
        internal static string _LuiSubscriptionKey = "a8046f7776b7494db8f1ea873eac5c3e";

        //qna 
        public readonly static string _QnaKnowledgedBlogId  = "d4254e4f-9925-45c7-9fdb-8bd018deaa8e";
        public readonly static string _QnaKnowledgedSenseId = "b80c3d4a-e80c-436f-800c-16e9cc73f402";
        public readonly static string _QnaKnowledgedJoseId = "1d8246fb-49ff-4178-86dd-f5c17070357a";
        

        public readonly static string _QnaSubscriptionKey = "ac1a50f16e0b400f93e81d87eea081fc"; 
        public readonly static double _Score = 0.3;
        public readonly static int _QtyAnswerReturn = 3;



    }
}