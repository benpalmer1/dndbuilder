/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 02MAY19
 * 
 * Purpose:
 * Simple server-side logging functionality to record errors and client connections/disconnections.
 */

using System;
using System.IO;
using System.Configuration;

namespace DndBuilder.WebApi
{
    public static class Logger
    {
        public static void Log(string log)
        {
            var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var tempLog = currentTime + ": " + log + "\n";

            try
            {
                File.AppendAllText(ConfigurationManager.AppSettings["LogfileName"], tempLog);
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: Unable to write to log: " + e.Message + " Log information: " + tempLog);
            }
        }
    }
}
