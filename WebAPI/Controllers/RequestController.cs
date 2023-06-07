using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Web.Http;
using WebAPI.Models;


namespace WebAPI.Controllers
{

    [RoutePrefix("api/request")]
    public class RequestController : ApiController
    {

        // Public ------------------------------------------------------------

        public RequestController() { }

        [Route("")]
        public IEnumerable<Request> GetAllRequests()
        {
            return this.requests;
        }

        [Route("{id:int}")]
        [ResponseType(typeof(Request))]
        public IHttpActionResult GetRequest(int id)
        {
            var request = this.requests.FirstOrDefault((p) => p.Id == id);
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

        // Private ------------------------------------------------------------

        private Request[] requests = new Request[]
        {
            new Request { Id = 1, Name = "r1" },
            new Request { Id = 2, Name = "r2" },
            new Request { Id = 3, Name = "r3" }
        };
    }

}
