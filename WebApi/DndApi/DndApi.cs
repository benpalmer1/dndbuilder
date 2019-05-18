/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 16MAY19
 * 
 * Purpose:
 * Interaction with the DnD5e API - retrieval of race and class information.
 * Data is sanitised through encoding and validity checks.
 *
 * Note to marker:
 * Unfortunately the dnd5eapi.co does not support HTTPS, but I would have liked to connect via HTTPS and verify the certificate.
 * Almost implemented this until I realised this wasn't the case. For example:
 * https://stackoverflow.com/questions/44953894/how-can-you-verify-validity-of-an-https-ssl-certificate-in-net
 */

using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

using DndBuilder.WebApi.Models;
using Newtonsoft.Json;

namespace DndBuilder.WebApi.Dnd5eApiAccess
{
    public static class DndApi
    {
        // Custom exception class to represent errors that arise from the dnd5e api in a consistent format
        // Used to easily differentiate between an issue accesing Dnd5e API and the server database.
        public class DndApiException : Exception
        {
            public DndApiException(string message) : base(message)
            {
                Logger.Log("Error: " + message);
            }
        }

        private static readonly string RACE_ENDPOINT = "http://www.dnd5eapi.co/api/races/";
        private static readonly string CLASS_ENDPOINT = "http://www.dnd5eapi.co/api/classes/";

        public static Dictionary<string, int> GetRaceOrClassesNameIdList(bool isRaceRequest)
        {
            string endpoint = isRaceRequest ? RACE_ENDPOINT : CLASS_ENDPOINT;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                request.ContentType = "application/json";
                request.Method = "GET";

                Dictionary<string, int> nameUrlList = new Dictionary<string, int>();

                using (WebResponse response = request.GetResponse())
                {
                    Stream data = response.GetResponseStream();
                    StreamReader reader = new StreamReader(data);

                    foreach (JObject item in JObject.Parse(reader.ReadToEnd())["results"])
                    {
                        string tempName = HtmlUtil.Encode(HtmlUtil.Decode(item["name"].ToString()));
                        string tempUrl = item["url"].ToString();

                        // Protect against server spoofing of URL. Replaces expected endpoint from returned URL, interacts with "ids" instead.
                        // StringComparison.Ordinal to ensure correct response when system is using a non en-us locale.
                        if (tempUrl.StartsWith(endpoint, StringComparison.Ordinal))
                        {
                            tempUrl = tempUrl.Replace(endpoint, "");
                            nameUrlList.Add(tempName, int.Parse(tempUrl));
                        }
                        else
                        {
                            throw new DndApiException("DnD Character URL returned from API using the wrong format. Server may have been spoofed.");
                        }
                    }

                    return nameUrlList;
                }
            }
            catch (WebException e)
            {
                throw new DndApiException($"Web error when getting data from Dnd5e API. Status: {e.Status}. Message: + {e.Message}");
            }
            catch (Exception e) when (e is IOException || e is InvalidCastException || e is DndApiException ||
                                      e is FormatException || e is ArgumentException || e is JsonReaderException)
            {
                throw new DndApiException($"Unable to retrieve character list from Dnd5e API: {e.Message}");
            }
        }

        // Usable based on return value of the Race from the URL query (api/races/{id}).
        public static DndRace GetRaceById(int raceId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RACE_ENDPOINT + "/" + raceId);
                request.ContentType = "application/json";
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                {
                    Stream data = response.GetResponseStream();
                    StreamReader reader = new StreamReader(data);

                    JObject parsedJson = JObject.Parse(reader.ReadToEnd());

                    return new DndRace()
                    {
                        Name = HtmlUtil.Encode(HtmlUtil.Decode(parsedJson["name"].ToString())),
                        RacialBonuses = parsedJson["ability_bonuses"].Select(x => (int)x).ToArray()
                    };
                }
            }
            catch (WebException e)
            {
                throw new DndApiException($"Web error when getting data from Dnd5e API. Status: {e.Status}. Message: + {e.Message}");
            }
            catch (Exception e) when ( e is InvalidCastException || e is FormatException || e is ArgumentException || e is JsonReaderException)
            {
                throw new DndApiException($"Unable to specified character race from Dnd5e API: {e.Message}");
            }
        }

        // Usable based on return value of the Class from the URL query (api/classes/{id}).
        public static DndClass GetClassById(int classId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(CLASS_ENDPOINT + "/" + classId);
                request.ContentType = "application/json";
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                {
                    Stream data = response.GetResponseStream();
                    StreamReader reader = new StreamReader(data);

                    JObject parsedJson = JObject.Parse(reader.ReadToEnd());

                    return new DndClass()
                    {
                        Name = HtmlUtil.Encode(HtmlUtil.Decode(parsedJson["name"].ToString())),
                        HitDie = int.Parse(parsedJson["hit_die"].ToString()),
                        IsSpellCaster = parsedJson["spellcasting"] != null
                    };
                }
            }
            catch (WebException e)
            {
                throw new DndApiException($"Web error when getting data from Dnd5e API. Status: {e.Status}. Message: + {e.Message}");
            }
            catch (Exception e) when ( e is InvalidCastException || e is FormatException || e is ArgumentException || e is JsonReaderException)
            {
                throw new DndApiException($"Unable to specified character class from Dnd5e API: {e.Message}");
            }
        }
    }
}