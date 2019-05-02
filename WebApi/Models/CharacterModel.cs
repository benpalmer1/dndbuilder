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

        /*
            // should these be here?
            Probably needs to be looked up every time the character is "viewed".
        
            // dictionary, values must total 2, excluding race bonus).
            public Dictionary<string, int> AbilityScores { get; set; }

            // dictionary, can be empty. To access: <race, ability> = bonus
            public Dictionary<Tuple<string,string>, int> RacialBonus { get; set; }
        */

        // Empty constructor
        public CharacterModel(){}

        // Build a new CharacterModel as per constructor input.
        public CharacterModel(
            string name,
            int age,
            string gender,
            string biography,
            int level,
            string race,
            string isCharacterClass,
            bool spellCaster,
            int hitPoints)
           /*Dictionary<string, int> abilityScores,
            Dictionary<Tuple<string,string>, int> racialBonus*/
        {
            // create the character... Error handling?
        }
    }
}
