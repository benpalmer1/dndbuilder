/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 16MAY19
 * 
 * Purpose:
 * Model class to hold information about a specific DnD character created by a user.
 */

using System.Collections.Generic;

using DndBuilder.WebApi.DndBuilderDatabase;

namespace DndBuilder.WebApi.Models
{
    public class DndCharacter
    {
        // private Backing fields
        private string _Name;
        private string _Gender;
        private string _Biography;

        public int Id { get; set; } // database id
        public int Age { get; set; }
        public int Level { get; set; }
        public bool SpellCaster { get; set; }
        public int HitPoints { get; set; }

        public string Name
        {
            get => _Name;
            set
            {
                _Name = Database.Encode(Database.Decode(value));
            }
        }

        public string Gender
        {
            get => _Gender;
            set
            {
                _Gender = Database.Encode(Database.Decode(value));
            }
        }
        public string Biography
        {
            get => _Biography;
            set
            {
                _Biography = Database.Encode(Database.Decode(value));
            }
        }

        public DndRace Race { get; set; } = new DndRace();
        public DndClass CharacterClass { get; set; } = new DndClass();
        public Dictionary<string, int> AbilityScores { get; set; } = new Dictionary<string, int>();
    }
}
