using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using PictureAPI.Models;
using PictureAPI.Utilities;

namespace PictureAPI.Controllers
{
    public class PicturesApiController : ApiController
    {
        private EFContext db = new EFContext();

        [Route("api/uploadpicture/{id}")]
        [ResponseType(typeof(BlobUploadedImageModel))]
        public async Task<IHttpActionResult> PostPicture(string id)
        {
          
                //check if the request we are receiving is of type multipart-content
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                //attempt to retrieve a product and get the image path from it to be passed to
                //the blob storage provider
                Product product = db.Products.Find(id);

                if (product == null || String.IsNullOrWhiteSpace(product.ImagePath))
                {
                    return NotFound();
                }
                else
                {
                    try
                    {

                        //create the blob upload storage provider to help with the upload to blob storage
                        var blobUploadProvider = new BlobStorageUploadProvider(product.ImagePath);

                        var uploadResult = await Request.Content.ReadAsMultipartAsync(blobUploadProvider)
                         .ContinueWith(task =>
                         {
                             if (task.IsFaulted || task.IsCanceled)
                             {
                                 throw task.Exception;
                             }


                             var provider = task.Result;
                             return provider.UploadedImage;
                         });


                        if (uploadResult != null)
                        {
                            return Ok(uploadResult);
                        }


                        // Otherwise 
                        return BadRequest();
                    }

                    catch (Exception ex)
                    {
                        return InternalServerError(ex);
                    }

            }
            
        }


        [Route("api/{id}/picture")]
        public async Task<HttpResponseMessage> GetPicture(string id)
        {

            //to do - get a hold of the product and check if it has an associated image
            Product product = db.Products.Find(id);

            //should there be no such products or the image path be empty - return null
            if (product == null || String.IsNullOrWhiteSpace(product.ImagePath))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
               
                try
                {

                    var blobName = product.ImagePath;

                    //should the name begin wih a slah, remove the first slash
                    if (blobName.StartsWith("/"))
                    {
                        blobName = blobName.Substring(1);
                    }

                    var container = BlobHelper.GetBlobContainer();
                    var blob = container.GetBlockBlobReference(blobName);


                    //download the blob into a memory stream. Notice that we're not putting the memory 
                    //stream in a using statement. This is because we need the stream to be open for the 
                    //API controller in order for the file to actually be downloadable. The closing and 
                    //disposing of the stream is handled by the Web API framework. 
                    var ms = new MemoryStream();
                    await blob.DownloadToStreamAsync(ms);


                    //strip off any folder structure so the file name is just the file name 
                    var lastPos = blob.Name.LastIndexOf('/');
                    var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);


                    // Build and return the download model with the blob stream and its relevant info 
                    var download = new BlobDownloadImageModel
                    {
                        BlobStream = ms,
                        BlobFileName = fileName,
                        BlobLength = blob.Properties.Length,
                        BlobContentType = blob.Properties.ContentType
                    };

                    //Reset the stream position; otherwise, download will not work
                    download.BlobStream.Position = 0;


                    //create response message with blob stream as its content 
                    var message = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(download.BlobStream)
                    };

                    // Set content headers 
                    message.Content.Headers.ContentLength = download.BlobLength;
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue(download.BlobContentType);
                    message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = HttpUtility.UrlDecode(download.BlobFileName),
                        Size = download.BlobLength
                    };

                    //return the model image object
                    return message;


                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Content = new StringContent(ex.Message)
                    };
                }
            }
        }
    }
}
