/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 02MAY19
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

        // Returns null on failure.
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
                Logger.Log("Error: Unable to get character.");
            }

            Logger.Log("Info: Client disconnected.");
            return charModelFromDb;
        }


        public bool AddCharacter(CharacterModel character)
        {
            bool isSuccess = false;

            try
            {
                using (var dbConnection = DatabaseSetup())
                {
                    var addCharCmd = new SqliteCommand(CharacterTable.Queries.GetInsertQuerySQLite(character), dbConnection);

                    if (addCharCmd.ExecuteNonQuery() == 1)
                    {
                        isSuccess = true;
                    }
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to add character.");
            }

            Logger.Log("Info: Client disconnected.");
            return isSuccess;
        }


        public void UpdateCharacter(CharacterModel character)
        {
            try
            {
                using (var dbConnection = DatabaseSetup())
                {

                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to update character.");
            }

            Logger.Log("Info: Client disconnected.");
        }


        public void DeleteCharacter(string characterName)
        {
            try
            {
                using (var dbConnection = DatabaseSetup())
                {

                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                Logger.Log("Error: Unable to delete character.");
            }

            Logger.Log("Info: Client disconnected.");
        }

        private SqliteConnection DatabaseSetup()
        {
            try
            {
                var isNewDb = false;
                if (!File.Exists(DATABASE_FILENAME))
                {
                    SqliteConnection.CreateFile(DATABASE_FILENAME);
                    isNewDb = true;
                }

                SqliteConnection newConnection = new SqliteConnection($"DataSource={DATABASE_FILENAME};Version=3;");
                newConnection.Open();

                // If new database, also setup the table schema.
                if (isNewDb)
                {
                    SqliteCommand newTableCmd = new SqliteCommand(CharacterTable.Queries.GetTableSchemaQuerySQLite(), newConnection);
                    newTableCmd.ExecuteNonQuery();
                }

                Logger.Log("Info: Client connected.");
                return newConnection;
            }
            catch(SqliteException e)
            {
                Logger.Log("Error: Unable to connect to SQLite database: " + e.Message);

                // rethrow to using and cleanup resources
                throw;
            }
        }
    }
}