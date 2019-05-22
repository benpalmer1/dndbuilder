/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 22MAY19
 * 
 * Purpose:
 * Exposed API for communication with a front end user interface which implements character building functionality.
 * Primarily used as an intermediary between the website and database calls.
 * 
 * I find it appropriate to catch base Exception for these methods, as well as my custom exception classes, as it
 * prevents a stack trace being shown to the user, and any other undesired effects. For these I report internal server errors as it was an undesired action - likely not client error.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DndBuilder.WebApi.DndBuilderDatabase;
using DndBuilder.WebApi.Models;
using DndBuilder.WebApi.Dnd5eApi;
using static DndBuilder.WebApi.Dnd5eApi.DndApi;
using static DndBuilder.WebApi.DndBuilderDatabase.Database;

namespace DndBuilder.WebApi
{
    public class DndApiController : ApiController
    {
        // Get list of classes and ids from dnd5e api - call before using Get_ClassById.
        // This method works quickly - you should prefer to call this over Get_AllClasses if possible.
        [HttpGet]
        [Route("api/classes/simple")]
        public Dictionary<string,int> Get_AllClassesSimple()
        {
            try
            {
                DndApi dndApi = new DndApi();
                return dndApi.GetRaceOrClassesNameIdList(false);
            }
            catch (DndApiException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get individual class by Id
        // Must be called using a value from Get_ClassIds due to possible changes to id, on an update of dnd5eapi
        [HttpGet]
        [Route("api/classes/{id}")]
        public DndClass Get_ClassById(int id)
        {
            try
            {
                DndApi dndApi = new DndApi();
                return dndApi.GetClassById(id);
            }
            catch (DndApiException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get list of classes from dnd5e api
        // This call requires quite a lot of network interaction (i.e time) with the Dnd5e Api. Avoid if possible.
        [HttpGet]
        [Route("api/classes")]
        public List<DndClass> Get_AllClasses()
        {
            try
            {
                List<DndClass> classes = new List<DndClass>();
                DndApi dndApi = new DndApi();

                Dictionary<string,int> classIdList = dndApi.GetRaceOrClassesNameIdList(false);
                foreach (int classId in classIdList.Values)
                {
                    classes.Add(dndApi.GetClassById(classId));
                }

                return classes;

            }
            catch (DndApiException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get list of races and ids from dnd5e api - call before using Get_RaceById.
        // This method works quickly - you should prefer to call this over Get_AllRaces if possible.
        [HttpGet]
        [Route("api/races/simple")]
        public Dictionary<string,int> Get_AllRacesSimple()
        {
            try
            {
                DndApi dndApi = new DndApi();
                return dndApi.GetRaceOrClassesNameIdList(true);
            }
            catch (DndApiException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get individual race by Id
        // Must be called using a value from Get_RaceIds due to possible changes to id, on an update of dnd5eapi
        [HttpGet]
        [Route("api/races/{id}")]
        public DndRace Get_RaceById(int id)
        {
            try
            {
                DndApi dndApi = new DndApi();
                return dndApi.GetRaceById(id);
            }
            catch (DndApiException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get list of races from dnd5e api
        // This call requires quite a lot of network interaction with the Dnd5e Api. Avoid if possible.
        [HttpGet]
        [Route("api/races")]
        public List<DndRace> Get_AllRaces()
        {
            try
            {
                List<DndRace> races = new List<DndRace>();
                DndApi dndApi = new DndApi();

                Dictionary<string,int> raceIdList = dndApi.GetRaceOrClassesNameIdList(true);
                foreach (int raceId in raceIdList.Values)
                {
                    races.Add(dndApi.GetRaceById(raceId));
                }

                return races;

            }
            catch (DndApiException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Add a new character to the database.
        // Returns true on success.
        [HttpPost]
        [Route("api/characters/add")]
        public bool Post_AddCharacter([FromBody] DndCharacter newCharacter)
        {
            try
            {
                Database database = new Database();
                return database.AddCharacter(newCharacter);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Edit a character in the database - by id.
        // Returns true on success.
        [HttpPost]
        [Route("api/characters/edit")]
        public bool Post_EditCharacter([FromBody] DndCharacter updatedCharacter)
        {
            try
            {
                Database database = new Database();
                return database.EditCharacter(updatedCharacter);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Returns a list of all characters in the database in the full format.
        [HttpGet]
        [Route("api/characters")]
        public List<DndCharacter> Get_List()
        {
            try
            {
                Database database = new Database();
                return database.GetAllCharacters();
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Returns a list of all characters in the database in a simple format.
        // Used by the web interface in order to save unnecessary data transfer.
        [HttpGet]
        [Route("api/characters/simple")]
        public List<SimpleDndCharacter> Get_SimpleList()
        {
            try
            {
                Database database = new Database();
                return database.GetAllCharactersSimple();
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get a character from the database - by id.
        [HttpGet]
        [Route("api/characters/{id}")]
        public DndCharacter Get_GetCharacter(int id)
        {
            try
            {
                Database database = new Database();
                return database.GetCharacter(id);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Get a character from the database - by name.
        [HttpPost]
        [Route("api/characters")]
        public DndCharacter Post_GetCharacter([FromBody]string characterName)
        {
            try
            {
                Database database = new Database();
                return database.GetCharacter(characterName);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Determine if a character by that name exists in the database - by name.
        [HttpPost]
        [Route("api/characters/exists")]
        public bool Post_CharacterExists([FromBody]string characterName)
        {
            try
            {
                Database database = new Database();
                return database.IsCharacterNameInUse(characterName);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Delete a character from the database - by id.
        [HttpGet]
        [Route("api/characters/delete/{id}")]
        public bool Get_DeleteCharacteryById(int id)
        {
            try
            {
                Database database = new Database();
                return database.DeleteCharacter(id);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        // Delete a character from the database - by name.
        [HttpPost]
        [Route("api/characters/delete")]
        public bool Post_DeleteCharacterByName([FromBody]string characterName)
        {
            try
            {
                Database database = new Database();
                return database.DeleteCharacter(characterName);
            }
            catch (DatabaseException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, e.Message));
            }
        }
    }
}