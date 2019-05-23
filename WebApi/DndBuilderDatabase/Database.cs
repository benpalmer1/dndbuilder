/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 22MAY19
 * 
 * Purpose:
 * Database interactivity class responsible for all communication with the SQLite database in a standard and consistent way.
 * All queries and schema information from SchemaQueries.cs.
 */

using System;
using System.IO;
using Mono.Data.Sqlite;

using DndBuilder.WebApi.Models;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using static DndBuilder.WebApi.DndBuilderDatabase.SchemaQueries;

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public class Database
    {
        // Custom exception class to represent errors that arise from the database in a consistent format
        public class DatabaseException : Exception
        {
            public DatabaseException(string message) : base(message)
            {
                Logger.Log("Error: " + message);
            }
        }

        public static readonly string DATABASE_FILENAME = "dnd_database.sqlite";

        public bool IsCharacterNameInUse(string charName)
        {
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(charName);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        return true;
                    }

                    if (count == 0)
                    {
                        return false;
                    }

                    throw new SqliteException("Database integrity error: more than one character exists with this name.");
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                throw new DatabaseException("Unable to get character. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        public DndCharacter GetCharacter(string charName)
        {
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(charName);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand getCharCmd = CharacterTable.Queries.CharacterQuery(charName);
                        getCharCmd.Connection = dbConnection;
                        SqliteDataReader reader = getCharCmd.ExecuteReader();

                        reader.Read();

                        DndCharacter newCharacter = new DndCharacter()
                        {
                            Id = int.Parse(reader[CharacterTable.Columns.ID].ToString()),
                            Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                            Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                            Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                            Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                            Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                            Race = (DndRace)Deserialize((byte[])reader[CharacterTable.Columns.RACE]),
                            CharacterClass = (DndClass)Deserialize((byte[])reader[CharacterTable.Columns.CHARCLASS]),
                            AbilityScores = (int[])Deserialize((byte[])reader[CharacterTable.Columns.ABILITYSCORES])
                        };
                        newCharacter.HitPoints = (newCharacter.Level * newCharacter.CharacterClass.HitDie) + newCharacter.AbilityScores[0] + newCharacter.Race.RacialBonuses[0];

                        return newCharacter;
                    }

                    if (count == 0)
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
                throw new DatabaseException("Unable to get character. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        // Method overloading, same functionality as above except with id
        public DndCharacter GetCharacter(int id)
        {
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(id);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand getCharCmd = CharacterTable.Queries.CharacterQuery(id);
                        getCharCmd.Connection = dbConnection;
                        SqliteDataReader reader = getCharCmd.ExecuteReader();

                        reader.Read();

                        DndCharacter newCharacter = new DndCharacter()
                        {
                            Id = int.Parse(reader[CharacterTable.Columns.ID].ToString()),
                            Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                            Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                            Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                            Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                            Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                            Race = (DndRace)Deserialize((byte[])reader[CharacterTable.Columns.RACE]),
                            CharacterClass = (DndClass)Deserialize((byte[])reader[CharacterTable.Columns.CHARCLASS]),
                            AbilityScores = (int[])Deserialize((byte[])reader[CharacterTable.Columns.ABILITYSCORES]),
                        };
                        newCharacter.HitPoints = (newCharacter.Level * newCharacter.CharacterClass.HitDie) + newCharacter.AbilityScores[0] + newCharacter.Race.RacialBonuses[0];

                        return newCharacter;
                    }

                    if (count == 0)
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
                throw new DatabaseException("Unable to get character. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        public List<DndCharacter> GetAllCharacters()
        {
            List<DndCharacter> characterModels = new List<DndCharacter>();
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.CheckAnyExistsQuery(), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count >= 1)
                    {
                        SqliteCommand getCharCmd = new SqliteCommand(CharacterTable.Queries.AllCharactersQuery(), dbConnection);
                        SqliteDataReader reader = getCharCmd.ExecuteReader();

                        while(reader.Read())
                        {
                            DndCharacter tempChar = new DndCharacter()
                            {
                                Id = int.Parse(reader[CharacterTable.Columns.ID].ToString()),
                                Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                                Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                                Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                                Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                                Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                                Race =  (DndRace)Deserialize((byte[])reader[CharacterTable.Columns.RACE]),
                                CharacterClass = (DndClass)Deserialize((byte[])reader[CharacterTable.Columns.CHARCLASS]),
                                AbilityScores = (int[])Deserialize((byte[])reader[CharacterTable.Columns.ABILITYSCORES])
                            };
                            tempChar.HitPoints = (tempChar.Level * tempChar.CharacterClass.HitDie) + tempChar.AbilityScores[0] + tempChar.Race.RacialBonuses[0];

                            characterModels.Add(tempChar);
                        }

                        return characterModels;
                    }

                    throw new SqliteException("No characters exist in the database.");
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                throw new DatabaseException("Unable to get list of characters. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        public List<SimpleDndCharacter> GetAllCharactersSimple()
        {
            List<SimpleDndCharacter> characterModels = new List<SimpleDndCharacter>();
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = new SqliteCommand(CharacterTable.Queries.CheckAnyExistsQuery(), dbConnection);
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count >= 1)
                    {
                        SqliteCommand getCharCmd = new SqliteCommand(CharacterTable.Queries.AllCharactersQuery(), dbConnection);
                        SqliteDataReader reader = getCharCmd.ExecuteReader();

                        while(reader.Read())
                        {
                            SimpleDndCharacter tempChar = new SimpleDndCharacter()
                            {
                                Id = int.Parse(reader[CharacterTable.Columns.ID].ToString()),
                                Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                                Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                                Race =  ((DndRace)Deserialize((byte[])reader[CharacterTable.Columns.RACE])).Name,
                                CharacterClass = ((DndClass)Deserialize((byte[])reader[CharacterTable.Columns.CHARCLASS])).Name
                            };
                            characterModels.Add(tempChar);
                        }

                        return characterModels;
                    }

                    throw new SqliteException("No characters exist in the database.");
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                throw new DatabaseException("Unable to get list of characters. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }
        }

        // Returns true on success
        public bool AddCharacter(DndCharacter character)
        {
            try
            {
                if (!character.IsValid)
                {
                    throw new DatabaseException("Unable to add character. Character fails server validation.");
                }

                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(character.Name);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 0)
                    {
                        SqliteCommand addCharCmd = CharacterTable.Queries.InsertQuery(character);
                        addCharCmd.Connection = dbConnection;

                        if (addCharCmd.ExecuteNonQuery() == 1)
                        {
                            Logger.Log("Info: Character added: " + character.Name);
                            return true;
                        }
                    }
                    else
                    {
                        throw new SqliteException("Character by that name already exists.");
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                throw new DatabaseException("Unable to add character. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }

            return false;
        }

        // Returns true on success
        public bool EditCharacter(DndCharacter character)
        {
            try
            {
                if (!character.IsValid)
                {
                    throw new DatabaseException("Unable to edit character. Character invalid.");
                }

                using (SqliteConnection dbConnection = DatabaseSetup())
                {

                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(character.Id);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand updateCharCmd = CharacterTable.Queries.EditQuery(character, character.Id);
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
                throw new DatabaseException("Unable to update character. " + e.Message);
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
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(characterName);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand deleteCharCmd = CharacterTable.Queries.DeleteQuery(characterName);
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
                throw new DatabaseException("Unable to delete character. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }

            return false;
        }

        // Returns true on success
        public bool DeleteCharacter(int id)
        {
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(id);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand deleteCharCmd = CharacterTable.Queries.DeleteQuery(id);
                        deleteCharCmd.Connection = dbConnection;

                        if (deleteCharCmd.ExecuteNonQuery() == 1)
                        {
                            Logger.Log("Info: Character deleted by id: " + id);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                throw new DatabaseException("Unable to delete character. " + e.Message);
            }
            finally
            {
                Logger.Log("Info: Client disconnected.");
            }

            return false;
        }

        public SqliteConnection DatabaseSetup()
        {
            try
            {
                if (!File.Exists(DATABASE_FILENAME))
                {
                    SqliteConnection.CreateFile(DATABASE_FILENAME);
                }

                SqliteConnection newConnection = new SqliteConnection($"Data Source={DATABASE_FILENAME};Version=3;");
                newConnection.Open();

                // Contains IF NOT EXISTS (removes redundant query to check for existing table)
                SqliteCommand newTableCmd = new SqliteCommand(CharacterTable.Queries.TableSchemaQuery(), newConnection);
                newTableCmd.ExecuteNonQuery();

                Logger.Log("Info: Client connected.");
                return newConnection;
            }
            catch(SqliteException e)
            {
                throw new DatabaseException("Unable to connect to database. " + e.Message);
            }
        }

        public static byte[] Serialize(object objectToSerialize)
        {
            if (objectToSerialize != null)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    BinaryFormatter binFormat = new BinaryFormatter();
                    binFormat.Serialize(memStream, objectToSerialize);

                    return memStream.ToArray();
                }
            }

            return null;
        }

        public static object Deserialize(byte[] byteArrayDeserialize)
        {
            if (byteArrayDeserialize != null)
            {
                using (MemoryStream byteStream = new MemoryStream(byteArrayDeserialize))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(byteStream);
                }
            }

            return null;
        }
    }
}
