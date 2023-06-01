using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelfrontEnd.Model;
using Newtonsoft.Json;
using System.Collections;
using System.Drawing.Printing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ShoppingCard2FrondEnd.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(string product="",string orderBy="",int currentPage = 1)
        {
            List<Product> productlist = new List<Product>();

            using (var httpclient = new HttpClient())
            {
                string token = HttpContext.Session.GetString("JwtToken");


                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpclient.GetAsync("https://localhost:7036/api/Product"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    productlist = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                    //if (!string.IsNullOrEmpty(product))
                    //{
                    //     productlist = productlist.Where(x => x.ProductCategory == product).ToList();
                     
                    //}

                    product =string.IsNullOrEmpty(product) ? "" : product.ToLower();

                    var proModel = new ProductViewModel();


                    proModel.NameSortOrder = string.IsNullOrEmpty(orderBy) ? "name_desc" : "";

                    var productAll = (from pro in productlist
                                      where product == "" || pro.ProductName.ToLower().StartsWith(product)
                                      select new Product
                                      {
                                          ProductId = pro.ProductId,
                                          ProductName = pro.ProductName,
                                          ProductCategory = pro.ProductCategory,
                                          ProductPrice = pro.ProductPrice,
                                          ProductImage = pro.ProductImage,
                                          ProductDescription = pro.ProductDescription,
                                          ProductQuality = pro.ProductQuality
                                      }
                                      ).ToList();
                    switch (orderBy)
                    {
                        case "name_desc":
                            productAll = productAll.OrderByDescending(a => a.ProductName).ToList();
                            break;
                        default:
                            productAll = productAll.OrderBy(a => a.ProductName).ToList();
                            break;
                    }

                    int totolRecords = productAll.Count();
                    int pageSize = 5;
                    int totalPages = (int)Math.Ceiling(totolRecords / (double)pageSize);
                    productAll = (List<Product>)productAll.Skip((currentPage - 1) * pageSize).Take(pageSize);

             //       proModel.Products = productAll;

                }
            }
           // return View(products);
            return View(productlist);
        }

        [HttpGet]
        public IActionResult Addproduct()
        {
            return View();
        }
        public async Task<IActionResult> Addproduct(UpDateProduct model)
        {
            List<UpDateProduct> productlist = new List<UpDateProduct>();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var serializedModel = JsonConvert.SerializeObject(model);
                    var stringContent = new StringContent(serializedModel, Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync("https://localhost:7036/api/Product", stringContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {

                            return RedirectToAction("Index");
                        }
                        else
                        {

                            var errorResponse = await response.Content.ReadAsStringAsync();
                            ModelState.AddModelError("", errorResponse);
                        }
                    }
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(Guid? ProductId)
        {
            if (ProductId == null)
            {
                return BadRequest();
            }

            var productModel = new Product();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7036/api/Product/{ProductId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        productModel = JsonConvert.DeserializeObject<Product>(apiResponse);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(productModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(Guid ProductId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"https://localhost:7036/api/Product/{ProductId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid ProductId)
        {
            var productModel = new Product();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7036/api/Product/{ProductId}"))
                {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            productModel = JsonConvert.DeserializeObject<Product>(apiResponse);
                        }
                        else
                        {
                            return NotFound();
                        }
                    
                }
            }
            HttpContext.Session.SetString("imag", productModel.ProductImage);
            return View(productModel);
        }
        [HttpPost]
       
        public async Task<IActionResult> EditProduct(Guid ProductId, Product model)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"https://localhost:7036/api/Product/{ProductId}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var productModel = JsonConvert.DeserializeObject<Product>(apiResponse);
                            productModel.ProductName = model.ProductName;
                            productModel.ProductPrice = model.ProductPrice;
                            productModel.ProductQuality = model.ProductQuality;
                            productModel.ProductDescription = model.ProductDescription;
                            productModel.ProductCategory = model.ProductCategory;
                            productModel.ProductImage = model.ProductImage;

                            var serializedModel = JsonConvert.SerializeObject(productModel);
                            var stringContent = new StringContent(serializedModel, Encoding.UTF8, "application/json");

                            using (var putResponse = await httpClient.PutAsync($"https://localhost:7036/api/Product/{ProductId}", stringContent))
                            {
                                if (putResponse.IsSuccessStatusCode)
                                {
                                    return RedirectToAction("Index", "Product");
                                }
                                else
                                {
                                    var errorResponse = await putResponse.Content.ReadAsStringAsync();
                                    ModelState.AddModelError("", errorResponse);
                                }
                            }
                        }
                        else
                        {
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            ModelState.AddModelError("", errorResponse);
                        }
                    }
                }
            }
            model.ProductImage = HttpContext.Session.GetString("imag");
            return View(model);
        }

    }
}
