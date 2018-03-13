using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot4App.Models
{
    public class QnAMakerResult
    {
        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}

//string responseString = string.Empty;

//var query = “hi”; //User Query
//var knowledgebaseId = “YOUR_KNOWLEDGE_BASE_ID”; // Use knowledge base id created.
//var qnamakerSubscriptionKey = “YOUR_SUBSCRIPTION_KEY”; //Use subscription key assigned to you.

////Build the URI
//Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
//var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

////Add the question as part of the body
//var postBody = $"{{\"question\": \"{query}\"}}";

////Send the POST request
//using (WebClient client = new WebClient())
//{
//    //Set the encoding to UTF8
//    client.Encoding = System.Text.Encoding.UTF8;

//    //Add the subscription key header
//    client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
//    client.Headers.Add("Content-Type", "application/json");
//    responseString = client.UploadString(builder.Uri, postBody);
//}