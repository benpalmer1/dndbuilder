/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 * 
 * Purpose:
 * Interaction with the DnD5e Api.
 */

using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DndBuilder.WebApi.Dnd5eApiAccess
{
    public static class DndApi
    {
        // Custom exception class to represent errors that arise from the dnd5e api in a consistent format
        public class DndApiException : Exception
        {
            public DndApiException(string message)
            {
                Logger.Log("Error: " + message);
            }
        }

        private static string RaceEndpoint = "http://www.dnd5eapi.co/api/races";
        private static string ClassEndpoint = "http://www.dnd5eapi.co/api/classes";

        public static Dictionary<string,string> GetRaceUrlList()
        {
            // Requires exception handling & implementation - return error state back to caller.
            Dictionary<string,string> races = new Dictionary<string,string>();

            HttpWebRequest racesRequest = (HttpWebRequest)WebRequest.Create(RaceEndpoint);
            racesRequest.ContentType = "application/json";    
            racesRequest.Method = "GET";

            using (WebResponse response = racesRequest.GetResponse())
            {

            }


            return races;
        }

        // Usable based on return value of the Race from the URL query.
        public static string GetRaceById(int raceId)
        {
            return "";
        }

        public static Dictionary<string,string> GetClassUrlList()
        {
            Dictionary<string,string> classes = new Dictionary<string,string>();
            try
            {
                HttpWebRequest classesRequest = (HttpWebRequest)WebRequest.Create(ClassEndpoint);
                classesRequest.ContentType = "application/json";    
                classesRequest.Method = "GET";

                using (WebResponse response = classesRequest.GetResponse())
                {
                    Stream data = response.GetResponseStream();
                    StreamReader reader = new StreamReader(data);

                    string textResponse = reader.ReadToEnd();
                    foreach (var item in JObject.Parse(textResponse)["results"])
                    {
                        classes.Add(item["name"].ToString(), item["url"].ToString());
                    }
                }

                return classes;
            }
            catch (Exception e) when (e is WebException || e is InvalidCastException)
            {
                throw new DndApiException("Unable to retrieve character list from Dnd5e API: " + e.Message);
            }
        }

        // Usable based on return value of the Class from the URL query.
        public static string GetClassById(int classId)
        {
            return "";
        }
    }
}