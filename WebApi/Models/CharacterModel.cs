using System;
using System.Collections.Generic;

namespace DndBuilder.WebApi.Models
{
    public class CharacterModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Biography { get; set; }
        public int Level { get; set; }

        public string Race { get; set; }
        public string CharacterClass { get; set;  }
        public bool SpellCaster { get; set; }
        public int HitPoints { get; set; }
        public Dictionary<string, int> AbilityScores { get; set; }

        // looked up every time the character is "viewed".
        // dictionary, can be empty. To access: <race, ability> = bonus
        public Dictionary<Tuple<string,string>, int> RacialBonus { get; set; }
    }
}
