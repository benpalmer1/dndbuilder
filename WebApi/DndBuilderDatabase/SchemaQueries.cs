/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 22MAY19
 * 
 * Purpose:
 * Database schema class responsible for holding DB schema and associated queries.
 * In order to counter a potential SQL injection attack, all values are encoded with SQlite parameters (data only).
 * Therefore implicity sanitising all database input.
 * 
 * When sent to the website, all values from the DB will be subject to HtmlEncode due to the auto-encode properties within each model.
 */

using DndBuilder.WebApi.Models;
using Mono.Data.Sqlite;

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public static class SchemaQueries
    {
        public static class CharacterTable
        {
            public const string NAME = "characters";

            public static class Columns
            {
                public const string ID = "id";                          // primary key, used by the system for efficient queries, little overhead
                public const string CHARNAME = "name";                  // non empty string, case insensitive, UNIQUE
                public const string AGE = "age";                        // 0 - 500
                public const string GENDER = "gender";                  // any string
                public const string BIOGRAPHY = "biography";            // string, up to 500 chars long
                public const string LEVEL = "level";                    // 1 - 20
                public const string RACE = "race";                      // blob representing serialisation of race data.
                public const string CHARCLASS = "character_class";      // blob representing serialisation of character class data.
                public const string HITPOINTS = "hitpoints";            // int, HP = (level*class hit dice) + constitution
                public const string ABILITYSCORES = "ability_scores";   // blob representing serialisation of ability name / score pair dictionary.
            }

            public static class Queries
            {
                public static string TableSchemaQuery()
                {
                    // No point specifing string length as per unimposed string length restrictions in SQLite.
                    // Done in validation instead.
                    // Create table if it doesn't exist, else do nothing.

                    return "CREATE TABLE IF NOT EXISTS " + NAME + "(" +
                        Columns.ID              + " INTEGER PRIMARY KEY, " +
                        Columns.CHARNAME        + " TEXT UNIQUE COLLATE NOCASE NOT NULL, " +
                        Columns.AGE             + " NUMERIC NOT NULL, " +
                        Columns.GENDER          + " TEXT NOT NULL, " +
                        Columns.BIOGRAPHY       + " TEXT NOT NULL, " +
                        Columns.LEVEL           + " NUMERIC NOT NULL, " +
                        Columns.RACE            + " BLOB NOT NULL, " +
                        Columns.CHARCLASS       + " BLOB NOT NULL, " +
                        Columns.HITPOINTS       + " NUMERIC NOT NULL, " +
                        Columns.ABILITYSCORES   + " BLOB NOT NULL"+ ")";
                }

                public static SqliteCommand CharacterQuery(string charName)
                {
                    SqliteCommand getCharQuery = new SqliteCommand(
                        "SELECT * FROM " + CharacterTable.NAME +
                        " WHERE " + Columns.CHARNAME + " = @name");

                    getCharQuery.Parameters.Add(new SqliteParameter("@name", charName));

                    return getCharQuery;
                }

                public static SqliteCommand CharacterQuery(int id)
                {
                    SqliteCommand getCharQuery = new SqliteCommand(
                        "SELECT * FROM " + CharacterTable.NAME +
                        " WHERE " + Columns.ID + " = @id");

                    getCharQuery.Parameters.Add(new SqliteParameter("@id", id));

                    return getCharQuery;
                }

                public static string AllCharactersQuery()
                {
                    return "SELECT * FROM " + CharacterTable.NAME;
                }

                public static SqliteCommand InsertQuery(DndCharacter charModel)
                {
                    SqliteCommand insertQuery = new SqliteCommand(
                        "INSERT INTO " + CharacterTable.NAME + "(" +
                        Columns.CHARNAME + "," + Columns.AGE + "," + Columns.GENDER + "," + Columns.BIOGRAPHY + "," +
                        Columns.LEVEL + "," + Columns.RACE + "," + Columns.CHARCLASS + "," + Columns.HITPOINTS + "," +
                        Columns.ABILITYSCORES + ")" +
                        " VALUES(@name,@age,@gender,@biography,@level,@race,@class,@hp,@abilityscores)");

                    insertQuery.Parameters.Add(new SqliteParameter("@name", charModel.Name));
                    insertQuery.Parameters.Add(new SqliteParameter("@age", charModel.Age));
                    insertQuery.Parameters.Add(new SqliteParameter("@gender", charModel.Gender));
                    insertQuery.Parameters.Add(new SqliteParameter("@biography", charModel.Biography));
                    insertQuery.Parameters.Add(new SqliteParameter("@level", charModel.Level));
                    insertQuery.Parameters.Add(new SqliteParameter("@race", Database.Serialize(charModel.Race)));
                    insertQuery.Parameters.Add(new SqliteParameter("@class", Database.Serialize(charModel.CharacterClass)));
                    insertQuery.Parameters.Add(new SqliteParameter("@hp", charModel.HitPoints));
                    insertQuery.Parameters.Add(new SqliteParameter("@abilityscores", Database.Serialize(charModel.AbilityScores)));

                    return insertQuery;
                }

                public static SqliteCommand CheckExistsQuery(string charName)
                {
                    SqliteCommand checkExistsQuery = new SqliteCommand(
                        "SELECT COUNT(*) FROM " + CharacterTable.NAME +
                        " WHERE " + Columns.CHARNAME + " = @name");

                    checkExistsQuery.Parameters.Add(new SqliteParameter("@name", charName));

                    return checkExistsQuery;
                }

                public static SqliteCommand CheckExistsQuery(int id)
                {
                    SqliteCommand checkExistsQuery = new SqliteCommand(
                        "SELECT COUNT(*) FROM " + CharacterTable.NAME +
                        " WHERE " + Columns.ID + " = @id");

                    checkExistsQuery.Parameters.Add(new SqliteParameter("@id", id));

                    return checkExistsQuery;
                }


                public static string CheckAnyExistsQuery()
                {
                    return "SELECT COUNT(*) FROM " + CharacterTable.NAME;
                }

                public static SqliteCommand EditQuery(DndCharacter charModel, int id)
                {
                    SqliteCommand editQuery = new SqliteCommand(
                        "UPDATE " + CharacterTable.NAME + " SET " +
                        Columns.CHARNAME       + " = @name, " +
                        Columns.AGE            + " = @age, " +
                        Columns.GENDER         + " = @gender, " +
                        Columns.BIOGRAPHY      + " = @biography, " +
                        Columns.LEVEL          + " = @level, " +
                        Columns.RACE           + " = @race, " +
                        Columns.CHARCLASS      + " = @class, " +
                        Columns.HITPOINTS      + " = @hp, " +
                        Columns.ABILITYSCORES  + " = @abilityscores" +
                        " WHERE " + Columns.ID + " = @id");

                    editQuery.Parameters.Add(new SqliteParameter("@id", id));
                    editQuery.Parameters.Add(new SqliteParameter("@name", charModel.Name));
                    editQuery.Parameters.Add(new SqliteParameter("@age", charModel.Age));
                    editQuery.Parameters.Add(new SqliteParameter("@gender", charModel.Gender));
                    editQuery.Parameters.Add(new SqliteParameter("@biography", charModel.Biography));
                    editQuery.Parameters.Add(new SqliteParameter("@level", charModel.Level));
                    editQuery.Parameters.Add(new SqliteParameter("@race", Database.Serialize(charModel.Race)));
                    editQuery.Parameters.Add(new SqliteParameter("@class", Database.Serialize(charModel.CharacterClass)));
                    editQuery.Parameters.Add(new SqliteParameter("@hp", charModel.HitPoints));
                    editQuery.Parameters.Add(new SqliteParameter("@abilityscores", Database.Serialize(charModel.AbilityScores)));

                    return editQuery;
                }

                public static SqliteCommand DeleteQuery(string charName)
                {
                    SqliteCommand deleteQuery = new SqliteCommand(
                        "DELETE FROM " + CharacterTable.NAME +
                        " WHERE " + Columns.CHARNAME + " = @name");

                    deleteQuery.Parameters.Add(new SqliteParameter("@name", charName));

                    return deleteQuery;
                }

                public static SqliteCommand DeleteQuery(int id)
                {
                    SqliteCommand deleteQuery = new SqliteCommand(
                        "DELETE FROM " + CharacterTable.NAME +
                        " WHERE " + Columns.ID + " = @id");

                    deleteQuery.Parameters.Add(new SqliteParameter("@id", id));

                    return deleteQuery;
                }
            }
        }
    }
}
