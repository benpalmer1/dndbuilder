using System;
using System.Collections.Generic;

namespace DndBuilder.WebApi.Models
{
    public class CharacterModel
    {
        public string Name { get; set; }                // non empty string, case insensitive, UNIQUE
        public int Age { get; set; }                    // 0 - 500
        public string Gender { get; set; }              // any string
        public string Biography { get; set; }           // string, up to 500 chars long
        public int Level { get; set; }                  // 1 - 20

        public string Race { get; set; }                // any string
        public string IsCharacterClass { get; set;  }              // any string
        public bool SpellCaster { get; set; }           // boolean - base off Class (if can cast spells)
        public int HitPoints { get; set; }              // int, HP = (level*class hit dice) + constitution

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
