using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDevCampBot.Models
{
    public static class Constants
    {
        public const string LAST_PERSON_KEY = "LastPerson";
        public const string RECENT_PEOPLE_KEY = "RecentPeople";
        public const string SCORE_KEY = "Score";
        public const string PLAYER_NAME_KEY = "PlayerName";
        public const string SECOND_CHANCE = "SecondChance";

        public const string SCORE_UPDATE_EVENT = "scoreUpdate";
        public const string END_GAME_EVENT = "endGame";
    }
}