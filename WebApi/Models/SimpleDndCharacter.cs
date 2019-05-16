/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 16MAY19
 * 
 * Purpose:
 * Simpler model class to hold information about a character, when it is being retrieved to display the list of characters. Used to reduce the amount of data sent from server to client when many of the character fields aren't necessary in order to display the list.
 */

using DndBuilder.WebApi.DndBuilderDatabase;

namespace DndBuilder.WebApi.Models
{
    public class SimpleDndCharacter
    {
        // private Backing fields
        private string _Name;
        private string _Race;
        private string _CharacterClass;

        public int Id { get; set; } // Database id
        public int Level { get; set; }

        public string Name
        {
            get => _Name;
            set
            {
                _Name = Database.Encode(Database.Decode(value));
            }
        }

        public string Race
        {
            get => _Race;
            set
            {
                _Race = Database.Encode(Database.Decode(value));
            }
        }

        public string CharacterClass
        {
            get => _CharacterClass;
            set
            {
                _CharacterClass = Database.Encode(Database.Decode(value));
            }
        }
    }
}