// Schema defintion file.
// Unsure if this is the best approach, or whether I should really be using the EF.
// .... Email Amy, once I have an example of both.

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public class DndBuilderSchema
    {
        public static class CharacterTable
        {
            public const string NAME = "characters";

            public static class Columns
            {
                public const string NAME = "name";
                public const string AGE = "age";
                public const string GENDER = "gender";
                public const string BIOGRAPHY = "biography";
                public const string LEVEL = "level";
                public const string RACE = "race";
                public const string CHARACTERCLASS = "character";
                public const string ISSPELLCASTER = "is_spellcaster";
                public const string HITPOINTS = "hitpoints";
            }
        }
    }
}