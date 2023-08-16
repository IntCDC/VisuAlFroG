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
                // public functions

                public RequestController() { }


                [Route("")]
                public IEnumerable<Request> GetAllRequests()
                {
                    return _requests;
                }


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


                // POST api/request 
                public void Post([FromBody] string value)
                {
                }


                // PUT api/values/5 
                public void Put(int id, [FromBody] string value)
                {
                }


                // DELETE api/values/5 
                public void Delete(int id)
                {
                }


                /* ------------------------------------------------------------------*/
                // private varaiables

                ///  DEBUG request data
                private Request[] _requests = new Request[]
                {
                    new Request { Id = 1, Name = "r1" },
                    new Request { Id = 2, Name = "r2" },
                    new Request { Id = 3, Name = "r3" }
                };
            }

        }
    }
}
