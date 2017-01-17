using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebCatalogue.Models;

namespace WebCatalogue.Controllers
{
    public class CsvListController : ApiController
    {

        private EFContext db = new EFContext();

        [ResponseType(typeof(Product[]))]
        public IHttpActionResult PostProducts(string productCsv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(productCsv))
            {
                return BadRequest("CSV text contnet is null or empty");
            }

            var elementsCsv = productCsv.Split('|');
            var resultingProducts = new List<Product>();

            //loop through the elements and attempt to create a product for each new element
            foreach (string csvElement in elementsCsv)
            {
                var properties = csvElement.Split(',', ';');
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

                //add the product to the list of products to be returned
                resultingProducts.Add(product);
            }

            return Ok(resultingProducts);

        }

        private bool ProductExists(string id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}
