using BABERECEN.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BABERECEN.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly MyLuisRecognizer _luisRecognizer;
        private Sleep sleep = null;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(MyLuisRecognizer luisRecognizer)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;

            // This array defines how the Waterfall will execute.

            var waterfallSteps = new WaterfallStep[]
            {
                IntroStepAsync,
                RecordStartStepAsync,
                RecordEndStepAsync,
                RecordSpecialAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions{
                   Prompt = MessageFactory.Text("What can I help you with today?")
               }, cancellationToken);
        }

        private async Task<DialogTurnResult> RecordStartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(MainDialog), cancellationToken);
            }

            var utterance = stepContext.Result.ToString();
            var endpointUri = String.Format("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/e02fffa5-4d00-4919-bdc2-9f77a4ce5c9f" +
                "?verbose=true&timezoneOffset=0&subscription-key=86b028669ad341cabf470efa587648a8&q={0}", utterance);
            var client = new HttpClient();
            var response = await client.GetAsync(endpointUri);
            var strResponseContent = await response.Content.ReadAsStringAsync();
     
            string message = "";
            JObject luisResult = JObject.Parse(strResponseContent);
            var entities = luisResult.SelectToken("entities");
            var topScoringIntent = luisResult.SelectToken("topScoringIntent").SelectToken("intent").ToString();
            switch (topScoringIntent)
            {
                case "RecordSleepStart":
                    message = RecordSleepStart(entities);
                    break;
                default:
                    await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Sorry, It's an incorrect input.") }, cancellationToken);
                    break;

            }

            var messageActivity = MessageFactory.Text(message, message, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(messageActivity, cancellationToken);

           return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = messageActivity }, cancellationToken);
        }

        private async Task<DialogTurnResult> RecordEndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(MainDialog), cancellationToken);
            }

            var utterance = stepContext.Result.ToString();
            var endpointUri = String.Format("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/e02fffa5-4d00-4919-bdc2-9f77a4ce5c9f" +
                "?verbose=true&timezoneOffset=0&subscription-key=86b028669ad341cabf470efa587648a8&q={0}", utterance);
            var client = new HttpClient();
            var response = await client.GetAsync(endpointUri);
            var strResponseContent = await response.Content.ReadAsStringAsync();

            string message = "";
            JObject luisResult = JObject.Parse(strResponseContent);
            var entities = luisResult.SelectToken("entities");
            var topScoringIntent = luisResult.SelectToken("topScoringIntent").SelectToken("intent").ToString();
            switch (topScoringIntent)
            {
                case "RecordSleepEnd":
                    message = RecordSleepEnd(entities);
                    break;
                default:
                    await stepContext.PromptAsync(nameof(TextPrompt), 
                        new PromptOptions { Prompt = MessageFactory.Text("Sorry, It's an incorrect input.") }, cancellationToken);
                    break;
            }

            var messageActivity = MessageFactory.Text(message, message, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(messageActivity, cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = messageActivity }, cancellationToken);
        }

        private async Task<DialogTurnResult> RecordSpecialAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(MainDialog), cancellationToken);
            }

            var utterance = stepContext.Result.ToString();
            var endpointUri = String.Format("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/e02fffa5-4d00-4919-bdc2-9f77a4ce5c9f" +
                "?verbose=true&timezoneOffset=0&subscription-key=86b028669ad341cabf470efa587648a8&q={0}", utterance);
            var client = new HttpClient();
            var response = await client.GetAsync(endpointUri);
            var strResponseContent = await response.Content.ReadAsStringAsync();

            string message = "";
            JObject luisResult = JObject.Parse(strResponseContent);
            var entities = luisResult.SelectToken("entities");
            var topScoringIntent = luisResult.SelectToken("topScoringIntent").SelectToken("intent").ToString();
            switch (topScoringIntent)
            {
                case "NegativeResponse":
                    message = "Okay, Then I'll just save this";
                    break;
                default:
                    message = "Okay, You said \"" + utterance + "\" to me. I'll keep a record of it";
                    sleep.special = utterance;
                    break;
            }

            var messageActivity = MessageFactory.Text(message, message, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(messageActivity, cancellationToken);

            // Save Data
            SqlConnection con = new SqlConnection("Data Source=yeopdbcore.database.windows.net;" +
                "Initial Catalog=YeopDBCore;User ID=yeop;Password=ekdh6@naver.com");

            con.Open();

            //baby_id   - auto increment 꼭 안넣어줘도됌
            //baby_start_time   - 잠들기 시작한 시간
            //baby_end_time     - 잠에서 깬 시간
            //baby_sleep_time   - 잠잔 시간
            //baby_record_time  - 말로 기록한 시간 (get time으로 알아서 들어감 꼭 안넣어줘도됌)
            //baby_speacial     - 잠 자면서 특이사항 (특이사항 있을때만 쓰면 됌)
           
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "INSERT INTO Sleep (baby_start_time, baby_end_time, baby_sleep_time, baby_special) VALUES (@start_time ,@end_time, @sleep_time, @special)";
            cmd.Parameters.Add("@start_time", SqlDbType.DateTime).Value = sleep.start_time;
            cmd.Parameters.Add("@end_time", SqlDbType.DateTime).Value = sleep.end_time;
            cmd.Parameters.Add("@sleep_time", SqlDbType.Int).Value = sleep.sleep_time;
            cmd.Parameters.Add("@special", SqlDbType.VarChar).Value = sleep.special;
            cmd.ExecuteNonQuery();
            con.Close();

            sleep = null;   // 객체 초기화

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private string RecordSleepStart(JToken entities)
        {
            sleep = new Sleep();    // 객체 생성

            DateTime startTime = DateTime.UtcNow;
            string response = "Okay. I'll start timer";
            foreach (var item in entities)
            {
                switch (item.ToString())
                {
                    // 처리해줘야하는 데 고민좀
                    case "builtin.number":
                        break;
                    case "time":
                        break;
                }
            }

            sleep.start_time = startTime;

            return response;
        }
        private string RecordSleepEnd(JToken entities)
        {
            if(sleep == null)
            {
                return "Sorry, I couldn't get the previous sleep information of your baby ";
            }
           
            DateTime endTime = DateTime.UtcNow;
            foreach (var item in entities)
            {
                switch (item.ToString())
                {
                    // 처리해줘야하는 데 고민좀
                    case "builtin.number":
                        break;
                    case "time":
                        break;
                }
            }

            sleep.end_time = endTime;
            TimeSpan timeSpan = sleep.end_time - sleep.start_time;
            sleep.sleep_time = timeSpan.Minutes;

            string response = "Today your baby slept for " + sleep.sleep_time
                + " minutes. Was there anything unusal when your baby was sleeping?";
            return response;
        }

    }
}

