using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace BotBlog.Dialogs.Qna
{
    [Serializable]
    public class FeedbackDialog : IDialog<IMessageActivity>
    {
        private string qnaAnsewer;
        private string userQuestion;

        public FeedbackDialog(string ansewer, string question)
        {
            // keep track of data associated with feedback
            qnaAnsewer = ansewer;
            userQuestion = question;
        }

        public async Task StartAsync(IDialogContext context)
        {
                var feedback = ((Activity)context.Activity).CreateReply("Você encontrou o que precisava ?");

                feedback.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                    {
                        new CardAction(){ Title = "👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
                        new CardAction(){ Title = "👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
                    }
                };

                await context.PostAsync(feedback);

                context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;

            if (userFeedback.Text.Contains("yes-positive-feedback") || userFeedback.Text.Contains("no-negative-feedback"))
            {
                // create telemetry client to post to Application Insights 
                TelemetryClient telemetry = new TelemetryClient();

                if (userFeedback.Text.Contains("yes-positive-feedback"))
                {
                    // post feedback to App Insights
                    var properties = new Dictionary<string, string>
                    {
                        {"Question", userQuestion },
                        {"Ansewer", qnaAnsewer },
                        {"Vote", "Yes" }
                        // add properties relevant to your bot 
                    };

                    telemetry.TrackEvent("Yes-Vote", properties);
                }
                else if (userFeedback.Text.Contains("no-negative-feedback"))
                {
                    // post feedback to App Insights
                    var properties = new Dictionary<string, string>
                    {
                        {"Question", userQuestion },
                        {"Ansewer", qnaAnsewer },
                        {"Vote", "No" }
                        // add properties relevant to your bot 
                    };

                    telemetry.TrackEvent("No-Vote", properties);
                }

                await context.PostAsync("Thanks for your feedback!");

                context.Done<string>(null);
            }
            else
            {
                // no feedback, return to QnA dialog
                context.Done<IMessageActivity>(userFeedback);
            }
        }
    }
}