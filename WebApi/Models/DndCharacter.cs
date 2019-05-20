/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Model class to hold information about a specific DnD character created by a user.
 */

using System;
using System.Linq;
using Newtonsoft.Json;

namespace DndBuilder.WebApi.Models
{
    [Serializable]
    public class DndCharacter
    {
        // JsonProperty attribute so that Serialization converts to sensible names, instead of "<ClassName>k__BackingField"
        [JsonProperty("Id")]
        private int _Id;

        [JsonProperty("Name")]
        private string _Name;

        [JsonProperty("Age")]
        private int _Age;

        [JsonProperty("Gender")]
        private string _Gender;

        [JsonProperty("Biography")]
        private string _Biography;

        [JsonProperty("Level")]
        private int _Level;

        [JsonProperty("HitPoints")]
        private int _HitPoints;

        [JsonProperty("Race")]
        private DndRace _Race = new DndRace();

        [JsonProperty("CharacterClass")]
        private DndClass _CharacterClass = new DndClass();

        [JsonProperty("AbilityScores")]
        private int[] _AbilityScores;


        public int Id
        {
            get => _Id;
            set { _Id = value; }
        }

        public int Age
        {
            get => _Age;
            set { _Age = value; }
        }
        public int Level
        {
            get => _Level;
            set { _Level = value; }
        }

        public int HitPoints
        {
            get => _HitPoints;
            set { _HitPoints = value; }
        }

        public DndRace Race
        {
            get => _Race;
            set { _Race = value; }
        }

        public DndClass CharacterClass
        {
            get => _CharacterClass;
            set { _CharacterClass = value; }
        }

        public int[] AbilityScores
        {
            get => _AbilityScores;
            set { _AbilityScores = value; }
        }


        // Must be encoded:
        public string Name
        {
            get => _Name;
            set { _Name = HtmlUtil.Encode(HtmlUtil.Decode(value)); }
        }

        public string Gender
        {
            get => _Gender;
            set { _Gender = HtmlUtil.Encode(HtmlUtil.Decode(value)); }
        }
        public string Biography
        {
            get => _Biography;
            set { _Biography = HtmlUtil.Encode(HtmlUtil.Decode(value)); }
        }


        // Does not check Id when comparing for equality.
        public bool Equals(DndCharacter b)
        {
            if (this.Name == b.Name &&
                this.Gender == b.Gender &&
                this.Biography == b.Biography &&
                this.Age == b.Age &&
                this.Level == b.Level &&
                this.HitPoints == b.HitPoints &&
                this.Race.Equals(b.Race) &&
                this.CharacterClass.Equals(b.CharacterClass) &&
                JsonConvert.SerializeObject(this.AbilityScores).Equals(JsonConvert.SerializeObject(b.AbilityScores)))
            {
                return true;
            }

            return false;
        }

        // Due to spec being vague about gender and biography requirements, only enforce must be non-null string.
        public bool IsValid =>
            !string.IsNullOrEmpty(this.Name) &&
            this.Age >= 0 &&
            this.Age <= 500 &&
            this.Gender != null &&
            this.Biography != null &&
            this.Biography.Length <= 500 &&
            this.Level >= 1 &&
            this.Level <= 20 &&
            this.Race != null &&
            this.Race.IsValid &&
            this.CharacterClass != null &&
            this.CharacterClass.IsValid &&
            this.AbilityScores != null &&
            this.AbilityScores.Length == 6 &&
            this.AbilityScores.All(x => x >= 0) &&
            this.AbilityScores.Sum() == 20;
    }     
}         