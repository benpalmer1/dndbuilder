using System;
using System.Collections.Generic;

using DndBuilder.WebApi.Models;

namespace DndBuilder.WebApi.Dnd5eApiAccess
{
    public class ApiAccess
    {
        public ApiAccess()
        {
        }

        public List<DndRace> GetRaceList()
        {
            return new List<DndRace>();
        }

        public List<DndClass> GetClassList()
        {
            return new List<DndClass>();
        }
    }
}