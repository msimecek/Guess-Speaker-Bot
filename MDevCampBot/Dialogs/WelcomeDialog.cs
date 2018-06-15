using System;
using System.Threading.Tasks;
using MDevCampBot.Models;
using MDevCampBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MDevCampBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            var playerName = Utils.SanitizeText(activity.Text);
            if (await LeaderboardService.IsNewPlayer(playerName))
            {
                context.ConversationData.SetValue(Constants.PLAYER_NAME_KEY, playerName);
                await context.PostAsync($"Let's play and earn some coins!");

                await context.Forward(new WhoIsDialog(), AfterWhoIsDialog, activity);
            }
            else
            {
                await context.PostAsync("This address has already been used.");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task AfterWhoIsDialog(IDialogContext context, IAwaitable<double> result)
        {
            var score = await result;
            var playerName = context.ConversationData.GetValue<string>(Constants.PLAYER_NAME_KEY);

            await LeaderboardService.SaveScoreAsync(playerName, score);
            await context.PostAsync($"💯 You are done! Your final score is: **{score}**.");
            await Utils.SendEventAsync(context, Constants.END_GAME_EVENT, score);

            // cleanup
            context.ConversationData.RemoveValue(Constants.LAST_PERSON_KEY);
            context.ConversationData.RemoveValue(Constants.RECENT_PEOPLE_KEY);
            context.ConversationData.RemoveValue(Constants.PLAYER_NAME_KEY);
            context.ConversationData.RemoveValue(Constants.SCORE_KEY);

            context.Done(true);
        }
    }
}