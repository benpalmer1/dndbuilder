/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 * 
 * Purpose:
 * Simple server-side logging functionality to record errors and client connections/disconnections.
 */

using System;
using System.IO;

namespace DndBuilder.WebApi
{
    public static class Logger
    {
        private static readonly string LOGFILE_NAME = "dnd_log.txt";

        public static void Log(string log)
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string tempLog = currentTime + ": " + log + "\n";

            try
            {
                string path = LOGFILE_NAME;

                File.AppendAllText(path, tempLog);
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: Unable to write to log: " + e.Message + " Log information: " + tempLog);
            }
        }
    }
}
