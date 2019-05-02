/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 02MAY19
 * 
 * Purpose:
 * Database schema class responsible for holding DB schema and associated queries.
 */

using DndBuilder.WebApi.Models;

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public static class Schema
    {
        public static class CharacterTable
        {
            public const string NAME = "characters";

            public static class Columns
            {
                public const string CHARNAME = "name";                  // non empty string, case insensitive, UNIQUE, primary key
                public const string AGE = "age";                        // 0 - 500
                public const string GENDER = "gender";                  // any string
                public const string BIOGRAPHY = "biography";            // string, up to 500 chars long
                public const string LEVEL = "level";                    // 1 - 20
                public const string RACE = "race";                      // any string
                public const string CHARCLASS = "character";            // any string
                public const string ISSPELLCASTER = "is_spellcaster";   // boolean - base off Class (if can cast spells)
                public const string HITPOINTS = "hitpoints";            // int, HP = (level*class hit dice) + constitution
            }

            public static class Queries
            {
                public static string GetTableSchemaQuerySQLite()
                {
                    // No point specifing string length as per unimposed string length restrictions in SQLite.
                    // Done in validation instead.

                    return "CREATE TABLE " + NAME + "(" +
                        Columns.CHARNAME + " TEXT PRIMARY KEY, " +
                        Columns.AGE + " NUMERIC, " +
                        Columns.GENDER + " TEXT, " +
                        Columns.BIOGRAPHY + " TEXT, " +
                        Columns.LEVEL + " NUMERIC, " +
                        Columns.RACE + " TEXT, " +
                        Columns.CHARCLASS + " TEXT, " +
                        Columns.ISSPELLCASTER + " BOOLEAN, " + // really, numeric
                        Columns.HITPOINTS + " NUMERIC, " + ")";
                }

                public static string GetQuerySQLite(string charName)
                {
                    return "SELECT * FROM " + NAME + " WHERE " + Columns.CHARNAME + " = " + charName;
                }

                public static string GetInsertQuerySQLite(CharacterModel charModel)
                {
                    return "INSERT INTO " + NAME + "VALUES(" +
                        charModel.Name + ", " +
                        charModel.Age + ", " +
                        charModel.Gender + ", " +
                        charModel.Biography + ", " +
                        charModel.Level + ", " +
                        charModel.Race + ", " +
                        charModel.CharacterClass + ", " +
                        charModel.SpellCaster + ", " +
                        charModel.HitPoints + ", " + ")";
                }

                public static string GetUpdateQuerySQLite(CharacterModel charModel)
                {

                }

                public static string GetDeleteQuerySQLite(CharacterModel charModel)
                {

                }
            }
        }
    }
}