/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Exposed API for communication with a front end user interface which implements character building functionality.
 * Primarily used as an intermediary between the website and database calls.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DndBuilder.WebApi.DndBuilderDatabase;
using DndBuilder.WebApi.Models;

namespace DndBuilder.WebApi
{
    public class DndApiController : ApiController
    {
        // Add a new character to the database.
        // Returns true on success.
        [HttpPost]
        [Route("api/characters/add")]
        public bool Post_AddCharacter([FromBody] DndCharacter newCharacter)
        {
            try {
                Database database = new Database();
                return database.AddCharacter(newCharacter);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Edit a character in the database - by id.
        // Returns true on success.
        [HttpPost]
        [Route("api/characters/edit")]
        public bool Post_EditCharacter([FromBody] DndCharacter updatedCharacter)
        {
            try {
                Database database = new Database();
                return database.EditCharacter(updatedCharacter);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Edit a character in the database - by name.
        // Returns true on success.
        [HttpPost]
        [Route("api/characters/edit")]
        public bool Post_EditCharacter([FromBody] DndCharacter updatedCharacter, [FromBody] string existingName)
        {
            try {
                Database database = new Database();
                return database.EditCharacter(updatedCharacter, existingName);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Returns a list of all characters in the database in the full format.
        [HttpGet]
        [Route("api/characters")]
        public List<DndCharacter> Get_List()
        {
            try {
                Database database = new Database();
                return database.GetAllCharacters();
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Returns a list of all characters in the database in a simple format.
        // Used by the web interface in order to save unnecessary data transfer.
        [HttpGet]
        [Route("api/characters/simple")]
        public List<SimpleDndCharacter> Get_SimpleList()
        {
            try {
                Database database = new Database();
                return database.GetAllCharactersSimple();
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Get a character from the database - by id.
        [HttpGet]
        [Route("api/characters/{id}")]
        public DndCharacter Get_GetCharacter(int id)
        {
            try {
                Database database = new Database();
                return database.GetCharacter(id);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Get a character from the database - by name.
        [HttpPost]
        [Route("api/characters")]
        public DndCharacter Post_GetCharacter([FromBody]string characterName)
        {
            try {
                Database database = new Database();
                return database.GetCharacter(characterName);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Determine if a character by that name exists in the database - by name.
        [HttpPost]
        [Route("api/characters/exists")]
        public bool Post_CharacterExists([FromBody]string characterName)
        {
            try {
                Database database = new Database();
                return database.IsCharacterNameInUse(characterName);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Delete a character from the database - by id.
        [HttpDelete]
        [Route("api/characters/delete")]
        public bool Delete_DeleteCharacteryById([FromBody]int characterId)
        {
            try {
                Database database = new Database();
                return database.DeleteCharacter(characterId);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }

        // Delete a character from the database - by name.
        [HttpDelete]
        [Route("api/characters/delete")]
        public bool Delete_DeleteCharacterByName([FromBody]string characterName)
        {
            try {
                Database database = new Database();
                return database.DeleteCharacter(characterName);
            } catch (Exception e) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, e.Message));
            }
        }
    }
}