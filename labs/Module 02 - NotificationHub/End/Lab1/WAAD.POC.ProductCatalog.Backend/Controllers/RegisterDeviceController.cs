using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.IO;
using Microsoft.Azure.NotificationHubs;
using System.Web;
using System.Text;

namespace WAAD.POC.ProductCatalog.Backend.Controllers
{
    public class RegisterDeviceController : ApiController
    {

        // POST api/RegisterDevice
        public void Post([FromBody] string value)
        {
            var connectionString = ConfigurationManager.AppSettings["NotificationHubConnectionString"];
            var notificationHubName = ConfigurationManager.AppSettings["NotificationHubName"];
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, notificationHubName);
            // Decode from Base64
            string Base64DecodedChannelUri = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            hub.DeleteRegistrationsByChannelAsync(Base64DecodedChannelUri).Wait();
            hub.CreateWindowsNativeRegistrationAsync(Base64DecodedChannelUri);
        }

    }
}
