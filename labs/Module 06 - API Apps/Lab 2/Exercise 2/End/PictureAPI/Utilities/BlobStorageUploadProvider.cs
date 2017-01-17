using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using PictureAPI.Models;

namespace PictureAPI.Utilities
{
    public class BlobStorageUploadProvider : MultipartFileStreamProvider
    {
        public BlobUploadedImageModel UploadedImage { get; set; }

        private string _blobFileName;


        public BlobStorageUploadProvider(String blobFileName) : base(Path.GetTempPath())
        {
            //intialize the UploadedImage property to a new empty object
            UploadedImage = new BlobUploadedImageModel();

            //save the name of the blob file that is passed into the constructor
            _blobFileName = blobFileName;
        }


        public override Task ExecutePostProcessingAsync()
        {
            //FileData is a property of MultipartFileStreamProvider and is a list of multipart 
            //files that have been uploaded and saved to disk in the Path.GetTempPath() location
            //we are only interested in the first uploaded file, should there be more than one 
            var fileData = FileData.First();
           
            // Sometimes the filename has a leading and trailing double-quote character 
            // when uploaded, so we trim it; otherwise, we get an illegal character exception 
            string fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));


            //Retrieve reference to a blob 
            var blobContainer = BlobHelper.GetBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(_blobFileName);


            //set the content type of the new blob
            blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;


            //copy from local storage into the Windows Azure blob service
            using (var fs = File.OpenRead(fileData.LocalFileName))
            {
                blob.UploadFromStream(fs);
            }


            //delete the local file
            File.Delete(fileData.LocalFileName);


            //create blob upload model with properties from blob info 
            var blobUpload = new BlobUploadedImageModel
            {
                FileName = blob.Name,
                FileUrl = blob.Uri.AbsoluteUri,
                FileSizeInBytes = blob.Properties.Length
            };

            //set the reference to the uploaded blob metadata to the upload property
            UploadedImage = blobUpload;
           
            return base.ExecutePostProcessingAsync();
        }
    }
}