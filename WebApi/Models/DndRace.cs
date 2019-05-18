/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Model class to hold information about a DnD race from the DnD5e api.
 */

using System;
using System.Linq;
using Newtonsoft.Json;

namespace DndBuilder.WebApi.Models
{
    [Serializable]
    public class DndRace
    {
        [JsonProperty("Name")]
        private string _Name;

        [JsonProperty("RacialBonuses")]
        private int[] _RacialBonuses;

        public string Name
        {
            get => _Name;
            set
            {
                _Name = HtmlUtil.Encode(HtmlUtil.Decode(value));
            }
        }

        public int[] RacialBonuses
        {
            get => _RacialBonuses;
            set { _RacialBonuses = value; }
        }

        public bool Equals(DndRace b)
        {
            if (this.Name == b.Name &&
                JsonConvert.SerializeObject(this.RacialBonuses) == JsonConvert.SerializeObject(b.RacialBonuses))
            {
                return true;
            }

            return false;
        }

        public bool IsValid =>
            !string.IsNullOrEmpty(this.Name) &&
            this.RacialBonuses != null &&
            this.RacialBonuses.Length == 6 &&
            this.RacialBonuses.All(bonus => bonus >= 0);
    }
}
