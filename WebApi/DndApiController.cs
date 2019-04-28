/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: ......
 * 
 * Purpose:
 * This API is used as an intermediary between the website and database calls.
 */

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DndBuilder.WebApi
{
    public class DndApiController : ApiController
    {
        [HttpPost]
        [Route("postExampleStub")]
        public void Post([FromBody]int val1, [FromBody]int val2, [FromBody]int val3)
        {
            HttpContent badPostParamValue = Request.Content;

            // return something.
        }

        [HttpGet]
        [Route("getExampleStub")]
        public string Get(int inValue)
        {
            try {
                if (inValue == 1) {
                    return "one";
                }
                if (inValue == 2) {
                    return "two";
                }
                throw new InvalidOperationException("impossible request");
            } catch (Exception ex) {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.BadRequest, "400 error: " + ex.Message));
            }
        }
    }
}