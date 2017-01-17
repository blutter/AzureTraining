using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PictureAPI.Utilities
{
    public static class BlobHelper
    {
        public static CloudBlobContainer GetBlobContainer()
        {
            // Pull these from config 
            var blobStorageConnectionString = WebConfigurationManager.AppSettings["BlobStorageConnectionString"];
            var blobStorageContainerName = WebConfigurationManager.AppSettings["BlobStorageContainerName"];


            // Create blob client and return reference to the container 
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(blobStorageContainerName);

            //create the container if it does not exist - and set its access to blob,
            //which means users can download a blob given they know the URL, but cannot
            //list the contents of the container without auth
            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob, null, null);

            return container;
        }
    }
}