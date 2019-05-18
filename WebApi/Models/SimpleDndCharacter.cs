/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Simpler model class to hold information about a character, when it is being retrieved to display the list of characters.
 * Used to reduce the amount of data sent from server to client when many of the character fields aren't necessary in order to display the list.
 * No validation required as data is never updated through this class.
 */

using System;
using Newtonsoft.Json;

namespace DndBuilder.WebApi.Models
{
    [Serializable]
    public class SimpleDndCharacter
    {
        [JsonProperty("Id")]
        private int _Id;

        [JsonProperty("Name")]
        private string _Name;

        [JsonProperty("Level")]
        private int _Level;

        [JsonProperty("Race")]
        private string _Race;

        [JsonProperty("CharacterClass")]
        private string _CharacterClass;

        public int Id
        {
            get => _Id;
            set
            {
                _Id = value;
            }
        }
        public int Level
        {
            get => _Level;
            set
            {
                _Level = value;
            }
        }

        public string Name
        {
            get => _Name;
            set
            {
                _Name = HtmlUtil.Encode(HtmlUtil.Decode(value));
            }
        }

        public string Race
        {
            get => _Race;
            set
            {
                _Race = HtmlUtil.Encode(HtmlUtil.Decode(value));
            }
        }

        public string CharacterClass
        {
            get => _CharacterClass;
            set
            {
                _CharacterClass = HtmlUtil.Encode(HtmlUtil.Decode(value));
            }
        }

        public bool Equals(SimpleDndCharacter b)
        {
            if (this.Id == b.Id &&
                this.Name == b.Name &&
                this.Level == b.Level &&
                this.Race == b.Race &&
                this.CharacterClass == b.CharacterClass)
            {
                return true;
            }

            return false;
        }
    }
}