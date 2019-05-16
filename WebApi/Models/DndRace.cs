/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 16MAY19
 * 
 * Purpose:
 * Model class to hold information about a DnD race from the DnD5e api.
 */

using DndBuilder.WebApi.DndBuilderDatabase;

namespace DndBuilder.WebApi.Models
{
    public class DndRace
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set
            {
                _Name = Database.Encode(Database.Decode(value));
            }
        }

        public int[] RacialBonuses { get; set; } = new int[6];
    }
}
