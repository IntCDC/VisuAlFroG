using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using System.Web.Http;
using Visualizations.WebAPI.Models;



/*
 * Web API Controller
 * 
 */
namespace Visualizations
{
    namespace WebAPI
    {
        namespace Controllers
        {
            [RoutePrefix("api/request")]
            public class RequestController : ApiController
            {

                /* ------------------------------------------------------------------*/
                #region public functions

                /// <summary>
                /// Ctor.
                /// </summary>
                public RequestController()
                {
                    _requests = new Request[]
                                    {
                    new Request { Id = 1, Name = "r1" },
                    new Request { Id = 2, Name = "r2" },
                    new Request { Id = 3, Name = "r3" }
                                    };
                }

                /// <summary>
                /// Answer all available requests.
                /// </summary>
                /// <returns>Enumerated requests.</returns>
                [Route("")]
                public IEnumerable<Request> GetAllRequests()
                {
                    return _requests;
                }

                /// <summary>
                /// Answer request by ID.
                /// </summary>
                /// <param name="id">The ID of the request.</param>
                /// <returns>The requested request.</returns>
                [Route("{id:int}")]
                [ResponseType(typeof(Request))]
                public IHttpActionResult GetRequest(int id)
                {
                    var request = _requests.FirstOrDefault((p) => p.Id == id);
                    if (request == null)
                    {
                        return NotFound();
                    }
                    return Ok(request);
                }

                /// <summary>
                /// Post request.
                /// </summary>
                /// <param name="value"></param>
                // POST api/request 
                public void Post([FromBody] string value)
                {
                }

                /// <summary>
                /// Put request.
                /// </summary>
                /// <param name="id"></param>
                /// <param name="value"></param>
                // PUT api/values/5 
                public void Put(int id, [FromBody] string value)
                {
                }

                /// <summary>
                /// Delete request.
                /// </summary>
                /// <param name="id"></param>
                // DELETE api/values/5 
                public void Delete(int id)
                {
                }

                #endregion

                /* ------------------------------------------------------------------*/
                #region private variables

                /// DEBUG 
                private Request[] _requests = null;

                #endregion
            }
        }
    }
}
