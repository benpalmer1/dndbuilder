/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 * 
 * Purpose:
 * Model class to hold information about a specific DnD character created by a user.
 */

using System;
using System.Collections.Generic;

namespace DndBuilder.WebApi.Models
{
    public class DndCharacter
    {
        public int Id { get; set; } // database id
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Biography { get; set; }
        public int Level { get; set; }

        public DndRace Race { get; set; }
        public DndClass CharacterClass { get; set;  }
        public bool SpellCaster { get; set; }
        public int HitPoints { get; set; }
        public Dictionary<string, int> AbilityScores { get; set; }
    }
}
