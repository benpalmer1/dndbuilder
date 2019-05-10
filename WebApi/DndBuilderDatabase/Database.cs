/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 10MAY19
 * 
 * Purpose:
 * Database interactivity class responsible for all communication with the SQLite database.
 * 
 * I was contemplating whether to combine all methods into one, with a switch (get,add,update,delete) for their functionality:
 * Decided not to for a few reasons: readability, testability and method responsibility.
 */

using System;
using System.IO;
using System.Configuration;
using Mono.Data.Sqlite;

using static DndBuilder.WebApi.DndBuilderDatabase.Schema;
using DndBuilder.WebApi.Models;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public class Database
    {
        private readonly string DATABASE_FILENAME = ConfigurationManager.AppSettings["DatabaseName"];

        // Returns null on failure
        public CharacterModel GetCharacter(string charName)
        {
            try
            {
                using (var dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.GetCheckExistsQuerySQLite(charName), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand getCharCmd = new SqliteCommand(CharacterTable.Queries.GetCharacterQuerySQLite(charName), dbConnection);
                        SqliteDataReader reader = getCharCmd.ExecuteReader();

                        reader.Read();

                        return new CharacterModel()
                        {
                            Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                            Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                            Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                            Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                            Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                            Race = reader[CharacterTable.Columns.RACE].ToString(),
                            CharacterClass = reader[CharacterTable.Columns.CHARCLASS].ToString(),
                            SpellCaster = bool.Parse(reader[CharacterTable.Columns.ISSPELLCASTER].ToString()),
                            HitPoints = int.Parse(reader[CharacterTable.Columns.HITPOINTS].ToString()),
                            AbilityScores = Deserialize<Dictionary<string, int>>((byte[])reader[CharacterTable.Columns.ABILITYSCORES])
                        };
                    }
                    else if (count == 0)
                    {
                        throw new SqliteException("Character does not exist in the database.");
                    }
                    else
                    {
                        throw new SqliteException("Database integrity error: more than one character exists with this name.");
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to get character. Message: " + e.Message);
                return null;
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        // Returns empty list when no characters were found.
        public List<CharacterModel> GetAllCharacters()
        {
            List<CharacterModel> characterModels = new List<CharacterModel>();
            try
            {
                using (var dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.GetCheckAnyExistsQuerySQLite(), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count >= 1)
                    {
                        SqliteCommand getCharCmd = new SqliteCommand(CharacterTable.Queries.GetAllCharactersQuerySQLite(), dbConnection);
                        SqliteDataReader reader = getCharCmd.ExecuteReader();

                        while(reader.Read())
                        {
                            var tempChar = new CharacterModel()
                            {
                                Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                                Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                                Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                                Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                                Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                                Race = reader[CharacterTable.Columns.RACE].ToString(),
                                CharacterClass = reader[CharacterTable.Columns.CHARCLASS].ToString(),
                                SpellCaster = bool.Parse(reader[CharacterTable.Columns.ISSPELLCASTER].ToString()),
                                HitPoints = int.Parse(reader[CharacterTable.Columns.HITPOINTS].ToString()),
                                AbilityScores = Deserialize<Dictionary<string, int>>((byte[])reader[CharacterTable.Columns.ABILITYSCORES])
                            };
                            characterModels.Add(tempChar);
                        }

                        return characterModels;
                    }
                    else
                    {
                        throw new SqliteException("No characters exist in the database.");
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to get character. Message: " + e.Message);
                return new List<CharacterModel>();
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        // Returns true on success
        public bool AddCharacter(CharacterModel character)
        {
            try
            {
                using (var dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.GetCheckExistsQuerySQLite(character.Name), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 0)
                    {
                        var addCharCmd = CharacterTable.Queries.GetInsertQuerySQLite(character);
                        addCharCmd.Connection = dbConnection;

                        if (addCharCmd.ExecuteNonQuery() == 1)
                        {
                            Logger.Log("Info: Character added: " + character.Name);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to add character. Message: " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }

            return false;
        }

        // Returns true on success
        public bool UpdateCharacter(CharacterModel character, string existingName)
        {
            try
            {
                using (var dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.GetCheckExistsQuerySQLite(character.Name), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        var updateCharCmd = CharacterTable.Queries.GetUpdateQuerySQLite(character, existingName);
                        updateCharCmd.Connection = dbConnection;

                        if (updateCharCmd.ExecuteNonQuery() == 1)
                        {
                            Logger.Log("Info: Character updated: " + character.Name);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to update character. Message: " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }

            return false;
        }

        // Returns true on success
        public bool DeleteCharacter(string characterName)
        {
            try
            {
                using (var dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.GetCheckExistsQuerySQLite(characterName), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        var deleteCharCmd = CharacterTable.Queries.GetDeleteQuerySQLite(characterName);
                        deleteCharCmd.Connection = dbConnection;

                        if (deleteCharCmd.ExecuteNonQuery() == 1)
                        {
                            Logger.Log("Info: Character deleted: " + characterName);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to delete character. Message: " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }

            return false;
        }

        private SqliteConnection DatabaseSetup()
        {
            try
            {
                if (!File.Exists(DATABASE_FILENAME))
                {
                    SqliteConnection.CreateFile(DATABASE_FILENAME);
                }

                SqliteConnection newConnection = new SqliteConnection($"DataSource={DATABASE_FILENAME};Version=3;");
                newConnection.Open();

                // Contains IF NOT EXISTS (removes redundant query to check for existing table)
                SqliteCommand newTableCmd = new SqliteCommand(CharacterTable.Queries.GetTableSchemaQuerySQLite(), newConnection);
                newTableCmd.ExecuteNonQuery();

                Logger.Log("Info: Client connected.");
                return newConnection;
            }
            catch(SqliteException e)
            {
                Logger.Log("Error: Unable to connect to SQLite database. Message: " + e.Message);

                // rethrow to calling statement and cleanup resources
                throw;
            }
        }

        public static byte[] Serialize(object objectToSerialize)
        {
            if (objectToSerialize != null)
            {
                using (var byteStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(byteStream, objectToSerialize);

                    return byteStream.ToArray();
                }
            }

            return null;
        }

        public static T Deserialize<T>(byte[] byteArrayDeserialize)
        {
            try
            {
                if (byteArrayDeserialize != null)
                {
                    using (var byteStream = new MemoryStream(byteArrayDeserialize))
                    {
                        var formatter = new BinaryFormatter();
                        T newObject = (T)formatter.Deserialize(byteStream);

                        return newObject;
                    }
                }
            }
            catch (InvalidCastException)
            {
               return default(T);
            }
            return default(T);
        }
    }
}
