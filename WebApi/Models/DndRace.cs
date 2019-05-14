/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 * 
 * Purpose:
 * Model class to hold information about a DnD race from the DnD5e api.
 */

using System.Collections.Generic;

namespace DndBuilder.WebApi.Models
{
    public class DndRace
    {
        public string Name { get; set; }

        public int HitDie { get; set; }

        public Dictionary<string, int> RacialBonuses { get; set; }
    }
}
