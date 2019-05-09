/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 09MAY19
 * 
 * Purpose:
 * Database interactivity class responsible for all communication with the SQLite database.
 */

using System;
using System.IO;
using System.Configuration;
using Mono.Data.Sqlite;

using static DndBuilder.WebApi.DndBuilderDatabase.Schema;
using DndBuilder.WebApi.Models;

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public class Database
    {
        private readonly string DATABASE_FILENAME = ConfigurationManager.AppSettings["DatabaseName"];

        // Returns null on failure and empty when char doesn't exist
        public CharacterModel GetCharacter(string charName)
        {
            CharacterModel charModelFromDb = null;
            try
            {
                using (var dbConnection = DatabaseSetup())
                {

                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to get character. Message: " + e.Message);
            }

            Logger.Log("Info: Client disconnected.");
            return charModelFromDb;
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

            Logger.Log("Info: Client disconnected.");
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

            Logger.Log("Info: Client disconnected.");
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

            Logger.Log("Info: Client disconnected.");
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

                // rethrow to using and cleanup resources
                throw;
            }
        }
    }
}