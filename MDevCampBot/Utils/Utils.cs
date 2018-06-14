using MDevCampBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MDevCampBot
{
    public class Utils
    {
        public static string NormalizeText(string input)
        {
            var res = input.ToLower();
            res = RemoveDiacritics(res);

            return res;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static async Task ChangeScoreAsync(IDialogContext context, double delta)
        {
            var score = context.ConversationData.GetValue<double>(Constants.SCORE_KEY);
            score += delta;
            context.ConversationData.SetValue(Constants.SCORE_KEY, score);
            await Utils.SendEventAsync(context, Constants.SCORE_UPDATE_EVENT, score);
        }

        public static string SanitizeText(string input)
        {
            return HttpUtility.UrlEncode(input); // will be used in URL
        }

        public static async Task SendEventAsync(IDialogContext context, string eventName, object value)
        {
            var eventMsg = context.MakeMessage() as IEventActivity;
            eventMsg.Type = "event";
            eventMsg.Name = eventName;
            eventMsg.Value = value;
            await context.PostAsync((IMessageActivity)eventMsg);
        }
    }
}