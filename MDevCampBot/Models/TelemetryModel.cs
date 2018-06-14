using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDevCampBot.Models
{
    public class TelemetryModel
    {
        /// <summary>
        /// "correct" | "incorrect"
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Speaker name.
        /// </summary>
        public string Name { get; set; }

        public TelemetryModel() { }

        public TelemetryModel(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}