/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 10MAY19
 * 
 * Purpose:
 * Database schema class responsible for holding DB schema and associated queries.
 */

using DndBuilder.WebApi.Models;
using Mono.Data.Sqlite;

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
                public const string ABILITYSCORES = "ability_scores";   // BLOB representing serialisation of ability name / score pair dictionary.
            }

            public static class Queries
            {
                public static string GetTableSchemaQuerySQLite()
                {
                    // No point specifing string length as per unimposed string length restrictions in SQLite.
                    // Done in validation instead.
                    // Create table if it doesn't exist, else do nothing.

                    return "CREATE TABLE IF NOT EXISTS " + NAME + "(" +
                        Columns.CHARNAME        + " TEXT PRIMARY KEY, " +
                        Columns.AGE             + " NUMERIC, " +
                        Columns.GENDER          + " TEXT, " +
                        Columns.BIOGRAPHY       + " TEXT, " +
                        Columns.LEVEL           + " NUMERIC, " +
                        Columns.RACE            + " TEXT, " +
                        Columns.CHARCLASS       + " TEXT, " +
                        Columns.ISSPELLCASTER   + " BOOLEAN, " + // really, numeric
                        Columns.HITPOINTS       + " NUMERIC, " +
                        Columns.ABILITYSCORES   + " BLOB"+ ")";
                }

                public static string GetCharacterQuerySQLite(string charName)
                {
                    return "SELECT * FROM " + CharacterTable.NAME + " WHERE " + Columns.CHARNAME + " = '" + charName + "'";
                }

                public static string GetAllCharactersQuerySQLite()
                {
                    return "SELECT * FROM " + CharacterTable.NAME;
                }

                public static SqliteCommand GetInsertQuerySQLite(CharacterModel charModel)
                {
                    SqliteCommand insertQuery = new SqliteCommand(
                        "INSERT INTO " + CharacterTable.NAME +
                        " VALUES(@name,@age,@gender,@biography,@level,@race,@class,@spellcaster,@hp,@abilityscores)");

                    insertQuery.Parameters.Add(new SqliteParameter("name", charModel.Name));
                    insertQuery.Parameters.Add(new SqliteParameter("age", charModel.Age));
                    insertQuery.Parameters.Add(new SqliteParameter("gender", charModel.Gender));
                    insertQuery.Parameters.Add(new SqliteParameter("biography", charModel.Biography));
                    insertQuery.Parameters.Add(new SqliteParameter("level", charModel.Level));
                    insertQuery.Parameters.Add(new SqliteParameter("race", charModel.Race));
                    insertQuery.Parameters.Add(new SqliteParameter("class", charModel.CharacterClass));
                    insertQuery.Parameters.Add(new SqliteParameter("spellcaster", charModel.SpellCaster));
                    insertQuery.Parameters.Add(new SqliteParameter("hp", charModel.HitPoints));
                    insertQuery.Parameters.Add(new SqliteParameter("abilityscores", Database.Serialize(charModel.AbilityScores)));

                    return insertQuery;
                }

                public static string GetCheckExistsQuerySQLite(string charName)
                {
                    return "SELECT COUNT(*) FROM " + CharacterTable.NAME + " WHERE " + Columns.CHARNAME + " = '" + charName + "'";
                }

                public static string GetCheckAnyExistsQuerySQLite()
                {
                    return "SELECT COUNT(*) FROM " + CharacterTable.NAME;
                }

                public static SqliteCommand GetUpdateQuerySQLite(CharacterModel charModel, string existingName)
                {
                    SqliteCommand updateQuery = new SqliteCommand(
                        "UPDATE " + CharacterTable.NAME + " SET " +
                        Columns.CHARNAME       + " = @name, " +
                        Columns.AGE            + " = @age, " +
                        Columns.GENDER         + " = @gender, " +
                        Columns.BIOGRAPHY      + " = @biography, " +
                        Columns.LEVEL          + " = @level, " +
                        Columns.RACE           + " = @race, " +
                        Columns.CHARCLASS      + " = @class, " +
                        Columns.ISSPELLCASTER  + " = @spellcaster, " +
                        Columns.HITPOINTS      + " = @hp, " +
                        Columns.ABILITYSCORES  + " = @abilityscores" +
                        " WHERE " + Columns.CHARNAME + $" = {existingName}");

                    updateQuery.Parameters.Add(new SqliteParameter("name", charModel.Name));
                    updateQuery.Parameters.Add(new SqliteParameter("age", charModel.Age));
                    updateQuery.Parameters.Add(new SqliteParameter("gender", charModel.Gender));
                    updateQuery.Parameters.Add(new SqliteParameter("biography", charModel.Biography));
                    updateQuery.Parameters.Add(new SqliteParameter("level", charModel.Level));
                    updateQuery.Parameters.Add(new SqliteParameter("race", charModel.Race));
                    updateQuery.Parameters.Add(new SqliteParameter("class", charModel.CharacterClass));
                    updateQuery.Parameters.Add(new SqliteParameter("spellcaster", charModel.SpellCaster));
                    updateQuery.Parameters.Add(new SqliteParameter("hp", charModel.HitPoints));
                    updateQuery.Parameters.Add(new SqliteParameter("abilityscores", Database.Serialize(charModel.AbilityScores)));

                    return updateQuery;
                }

                public static SqliteCommand GetDeleteQuerySQLite(string charName)
                {
                    return new SqliteCommand(
                        "DELETE FROM " + CharacterTable.NAME + " WHERE " + Columns.CHARNAME + " = '" + charName + "'");
                }
            }
        }
    }
}
