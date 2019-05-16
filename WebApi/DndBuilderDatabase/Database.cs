/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 16MAY19
 * 
 * Purpose:
 * Database interactivity class responsible for all communication with the SQLite database in a standard and consistent way.
 * All queries and schema information from SchemaQueries.cs.
 */

using System;
using System.IO;
using System.Web;
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

        private static readonly string DATABASE_FILENAME = "dnd_database.sqlite";

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
                        return false;
                    }
                    else if (count == 0)
                    {
                        return true;
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

                        return new DndCharacter()
                        {
                            Id = int.Parse(reader[CharacterTable.Columns.ID].ToString()),
                            Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                            Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                            Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                            Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                            Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                            Race = Deserialize<DndRace>((byte[])reader[CharacterTable.Columns.RACE]),
                            CharacterClass = Deserialize<DndClass>((byte[])reader[CharacterTable.Columns.CHARCLASS]),
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

                        return new DndCharacter()
                        {
                            Id = int.Parse(reader[CharacterTable.Columns.ID].ToString()),
                            Name = reader[CharacterTable.Columns.CHARNAME].ToString(),
                            Age = int.Parse(reader[CharacterTable.Columns.AGE].ToString()),
                            Gender = reader[CharacterTable.Columns.GENDER].ToString(),
                            Biography = reader[CharacterTable.Columns.BIOGRAPHY].ToString(),
                            Level = int.Parse(reader[CharacterTable.Columns.LEVEL].ToString()),
                            Race =  Deserialize<DndRace>((byte[])reader[CharacterTable.Columns.RACE]),
                            CharacterClass = Deserialize<DndClass>((byte[])reader[CharacterTable.Columns.CHARCLASS]),
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
                                Race =  Deserialize<DndRace>((byte[])reader[CharacterTable.Columns.RACE]),
                                CharacterClass = Deserialize<DndClass>((byte[])reader[CharacterTable.Columns.CHARCLASS]),
                                SpellCaster = bool.Parse(reader[CharacterTable.Columns.ISSPELLCASTER].ToString()),
                                HitPoints = int.Parse(reader[CharacterTable.Columns.HITPOINTS].ToString()),
                                AbilityScores = Deserialize<Dictionary<string, int>>((byte[])reader[CharacterTable.Columns.ABILITYSCORES])
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
                                Race =  Deserialize<DndRace>((byte[])reader[CharacterTable.Columns.RACE]).Name,
                                CharacterClass = Deserialize<DndClass>((byte[])reader[CharacterTable.Columns.CHARCLASS]).Name,
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
        public bool UpdateCharacter(DndCharacter character, string existingName)
        {
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(character.Name);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand updateCharCmd = CharacterTable.Queries.UpdateQuery(character, existingName);
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
        // Method overload
        public bool UpdateCharacter(DndCharacter character, int id)
        {
            try
            {
                using (SqliteConnection dbConnection = DatabaseSetup())
                {
                    // Still check by name
                    SqliteCommand checkDBCmd = CharacterTable.Queries.CheckExistsQuery(character.Name);
                    checkDBCmd.Connection = dbConnection;
                    int count = Convert.ToInt32(checkDBCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        SqliteCommand updateCharCmd = CharacterTable.Queries.UpdateQuery(character, id);
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

        public static string Encode(string htmlInput)
        {
            return HttpUtility.HtmlEncode(htmlInput);
        }

        // Decode(string) - Decodes a html input string of any encoding depth < 1000
        // Designed to handle possibility of any issues from multiple encoding / XSS attempts
        public static string Decode(string htmlInput)
        {
            if (htmlInput ==  HttpUtility.HtmlDecode(htmlInput))
            {
                return htmlInput;
            }
            else
            {
                int ii = 0;
		        string temp = htmlInput;
		        string decoded = HttpUtility.HtmlDecode(htmlInput);
                
                // Limited to prevent long running script on some error
		        while (temp != decoded && ii++ < 1000)
		        {
			        temp = decoded;
			        decoded = HttpUtility.HtmlDecode(decoded);
		        }

                return decoded;
            }
        }

        public static byte[] Serialize(object objectToSerialize)
        {
            if (objectToSerialize != null)
            {
                using (MemoryStream byteStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
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
                    using (MemoryStream byteStream = new MemoryStream(byteArrayDeserialize))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
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
