using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MDevCampBot.Models;
using MDevCampBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MDevCampBot.Dialogs
{
    [Serializable]
    public class WhoIsDialog : IDialog<double>
    {
        private PeopleCollection _people = null;

        public Task StartAsync(IDialogContext context)
        {
            context.ConversationData.SetValue(Constants.SCORE_KEY, 0); // initialize/reset score
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            if (_people == null)
            {
                _people = await PeopleService.GetPeopleLisAtAsync();
            }

            var mess = await result as IMessageActivity;

            if (context.ConversationData.ContainsKey(Constants.LAST_PERSON_KEY))
            {
                var lastPerson = context.ConversationData.GetValue<Person>(Constants.LAST_PERSON_KEY);

                var guessResult = _people.FindByName(mess.Text);
                if (guessResult.Count > 3 && guessResult.Contains(lastPerson, new PersonComparer()))
                {
                    await context.PostAsync("That is too generic. Try again.");
                    context.Wait(MessageReceivedAsync);
                }
                else if (guessResult.Contains(lastPerson, new PersonComparer()))
                {
                    // správná odpověď
                    TelemetryService.SendTelemetry(new TelemetryModel("correct", lastPerson.Name));

                    await context.PostAsync("✔️ Correct!");

                    // zvednout skóre
                    if (context.ConversationData.ContainsKey(Constants.SECOND_CHANCE))
                    {
                        await context.PostAsync("That's **0.5** points for you.");
                        await Utils.ChangeScoreAsync(context, 0.5);
                        context.ConversationData.RemoveValue(Constants.SECOND_CHANCE);
                    }
                    else
                    {
                        await context.PostAsync("That's **2** points for you.");
                        await Utils.ChangeScoreAsync(context, 2);
                    }

                    // DEBUG
                    //context.Done(1.0);
                    //return;

                    // zobrazit další
                    await GoNext(context);
                }
                else
                {
                    // špatná odpověď - dáme nápovědu, pokud už nejsme v nápovědě :)
                    TelemetryService.SendTelemetry(new TelemetryModel("incorrect", lastPerson.Name));

                    if (context.ConversationData.ContainsKey(Constants.SECOND_CHANCE))
                    {
                        await context.PostAsync($"❌ That is not correct. Let's try another speaker.");
                        context.ConversationData.RemoveValue(Constants.SECOND_CHANCE);

                        await GoNext(context);
                    }
                    else
                    {
                        await context.PostAsync("Not correct. I'll give you a hint. But it will be for less points. This person's name is one of those:");
                        var msg = context.MakeMessage();
                        msg.Attachments.Add(PrepareButtonsCard(_people.GetRandomPeople(lastPerson, 5).Select(p => p.Name).ToArray()).ToAttachment());
                        await context.PostAsync(msg);
                        context.ConversationData.SetValue(Constants.SECOND_CHANCE, "true");

                        context.Wait(MessageReceivedAsync);
                    }

                }
            }
            else
            {
                await GoNext(context);
            }
        }

        private async Task AfterSecondChance(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var mess = await result;

            var lastPerson = context.ConversationData.GetValue<Person>(Constants.LAST_PERSON_KEY);

            if (Utils.NormalizeText(mess.Text) == Utils.NormalizeText(lastPerson.Company))
            {
                await Utils.ChangeScoreAsync(context, 0.5);
                await context.PostAsync("Good, that's correct! You earned **0.5** points.");
            }
            else
            {
                await context.PostAsync("Nope, that's not correct. Let's try next speaker.");
            }

            await GoNext(context);
        }

        private async Task GoNext(IDialogContext context)
        {
            var typing = context.MakeMessage();
            typing.Type = ActivityTypes.Typing;
            typing.Text = null;
            await context.PostAsync(typing);
            Thread.Sleep(1500);

            var faceShown = await TryShowRandomFaceAsync(context);
            if (!faceShown)
            {
                await context.PostAsync("That was it! You got to the end.");
                var score = context.ConversationData.GetValue<double>(Constants.SCORE_KEY);
                context.Done(score);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            } 
        }

        private async Task<bool> TryShowRandomFaceAsync(IDialogContext context)
        {
            var recentPeople = context.ConversationData.GetValueOrDefault(Constants.RECENT_PEOPLE_KEY, new List<string> { });
            if (recentPeople.Count == _people.Count)
            {
                return false;
            }

            Person randomPerson;
            do
            {
                randomPerson = _people.GetRandomPerson();
            } while (recentPeople.Contains(randomPerson.Name));

            context.ConversationData.SetValue(Constants.LAST_PERSON_KEY, randomPerson);
            recentPeople.Add(randomPerson.Name);
            context.ConversationData.SetValue(Constants.RECENT_PEOPLE_KEY, recentPeople);

            var subtitle = (ConfigurationManager.AppSettings["DebugMode"] == "true")
                                ? $"(Debug) {randomPerson.Name} {recentPeople.Count}/{_people.Count}"
                                : $"{recentPeople.Count}/{_people.Count}"; 

            var card = new HeroCard("Who is this?", subtitle: subtitle, images: new List<CardImage>() { new CardImage(randomPerson.PhotoUrl) });

            var message = context.MakeMessage();
            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);

            return true;
        }

        private HeroCard PrepareButtonsCard(string[] options)
        {
            var res = new HeroCard();
            var buttons = new List<CardAction>();
            foreach (var o in options)
            {
                buttons.Add(new CardAction(ActionTypes.ImBack, title: o, value: o));
            }

            res.Buttons = buttons;
            return res;
        }
    }
}