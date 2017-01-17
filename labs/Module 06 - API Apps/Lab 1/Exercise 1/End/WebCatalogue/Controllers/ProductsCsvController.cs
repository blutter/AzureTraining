using WebCatalogue.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebCatalogue.Controllers
{
    public class CsvController : ApiController
    {
        private EFContext db = new EFContext();

        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(string productCsv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(productCsv))
            {
                return BadRequest("CSV text contnet is null or empty");
            }

            var properties = productCsv.Split(',', ';');
            if (properties.Length <= 7)
            {
                return BadRequest("CSV entry for  the procduct doesn't seems right (< 7 properties)");
            }

            // Name, Description, Band, ProductColor, Price, ImagePath, Category, SubCategory

            // Mapping
            var product = new Product
            {
                Name = properties[0],
                Description = properties[1],
                Brand = properties[2],
                ProductColor = properties[3],
                Price = decimal.Parse(properties[4]), //price,
                ImagePath = properties[5],
                Category = properties[6],
                SubCategory = properties[7]
            };

            product.Id = Guid.NewGuid().ToString();
            db.Products.Add(product);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        private bool ProductExists(string id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}