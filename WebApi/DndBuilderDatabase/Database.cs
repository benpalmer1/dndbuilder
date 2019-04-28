using System;
using Mono.Data.Sqlite;

namespace DndBuilder.WebApi.DndBuilderDatabase
{
    public class Database
    {
        string cs = "Data Source=:memory:";

        SqliteConnection connection = null;
        SqliteCommand cmd = null;

        public Database()
        {
            // Taken directly from http://zetcode.com/db/sqlitecsharp/intro/
            // change to usable format if this is the right approach.
            try 
            {
                connection = new SqliteConnection(cs);
                connection.Open();

                string stm = "SELECT SQLITE_VERSION()";   
                cmd = new SqliteCommand(stm, connection);

                string version = Convert.ToString(cmd.ExecuteScalar());

                Console.WriteLine("SQLite version : {0}", version);
                
            } catch (SqliteException ex) 
            {
                Console.WriteLine("Error: {0}",  ex.ToString());

            } finally 
            {   
                if (cmd != null)
                {
                    cmd.Dispose();
                }
             
                if (connection != null) 
                {
                    try 
                    {
                        connection.Close();

                    } catch (SqliteException ex)
                    { 
                        Console.WriteLine("Closing connection failed.");
                        Console.WriteLine("Error: {0}",  ex.ToString());
                        
                    } finally 
                    {
                        connection.Dispose();
                    }
                }                        
            }
    }
        }
    }
}
