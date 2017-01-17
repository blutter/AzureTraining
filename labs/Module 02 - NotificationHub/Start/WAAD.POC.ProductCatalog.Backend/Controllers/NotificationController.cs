using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace WAAD.POC.ProductCatalog.Backend.Controllers
{
    public class NotificationController : ApiController
    {
        // POST api/notification
        public async Task<HttpResponseMessage> Get([FromUri]string message)
        {
           
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent("Success");
            return response;
        }

    }
}
