using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JacWebShopGUI.Models;
using Kendo.Mvc.Extensions;
using System.Threading;
using System.Globalization;
using System.Web.Caching;
using System.Runtime.Caching;

namespace JacWebShopGUI.Controllers
{
    public class GridProductsManagementController : Controller
    {

        private MemoryCache cache = MemoryCache.Default;


        // GET: GridProductsManagement
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategoriesList()
        {
            try
            {
                using (var DB = new JacShopDBEntities())
                {
                    var result = DB.uspGetCategoriesList().ToList();
                    List<Object> viewModel = new List<object>();
                    foreach (var resultItem in result)
                    {
                        viewModel.Add(new
                        {
                            Id = resultItem.Id,
                            Name = resultItem.Name
                        });
                    }
                    return Json(viewModel, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
            }
            return Json(100);
        }

        public ActionResult GetAllProducts([DataSourceRequest] DataSourceRequest request, int dataStorage)
        {
            try
            {
                if (dataStorage == 2)
                {

                    List<uspGetAllProducts_Result> dataProductCache = new List<uspGetAllProducts_Result>();
                    if (HttpRuntime.Cache.Get("dataCache") != null)
                    {
                        dataProductCache = (List<uspGetAllProducts_Result>)HttpRuntime.Cache.Get("dataCache");             
                    }

                    IQueryable<uspGetAllProducts_Result> getAllProductsList = dataProductCache.AsQueryable();
                    DataSourceResult result = getAllProductsList.ToList().ToDataSourceResult(request);
                    return Json(result);

                }

                using (var DB = new JacShopDBEntities())
                {
                    IQueryable<uspGetAllProducts_Result> getAllProductsList = DB.uspGetAllProducts().AsQueryable();
                    DataSourceResult result = getAllProductsList.ToList().ToDataSourceResult(request);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(550, "Some error" + ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNewProduct([DataSourceRequest] DataSourceRequest request, uspGetAllProducts_Result productEdit, int dataStorage)
        {
            try
            {
              
                productEdit.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (dataStorage == 2)
                {

                    if (productEdit != null && ModelState.IsValid)
                    {
                        List<uspGetAllProducts_Result> dataProductCache = new List<uspGetAllProducts_Result>();
                        
                        if (HttpRuntime.Cache.Get("dataCache") != null)
                        {
                            dataProductCache = (List<uspGetAllProducts_Result>)HttpRuntime.Cache.Get("dataCache");
                            HttpRuntime.Cache.Remove("dataCache");
                            dataProductCache.Add(productEdit);
                            HttpRuntime.Cache.Insert("dataCache"
                                                   , dataProductCache
                                                   , null
                                                   , DateTime.UtcNow.AddMinutes(10)
                                                   , Cache.NoSlidingExpiration);
                            return Json(new[] { productEdit }.ToDataSourceResult(request, ModelState));

                        }

                        dataProductCache.Add(productEdit);
                        HttpRuntime.Cache.Insert("dataCache"
                                                   , dataProductCache
                                                   , null
                                                   , DateTime.UtcNow.AddMinutes(10)
                                                   , Cache.NoSlidingExpiration);

                        return Json(new[] { productEdit }.ToDataSourceResult(request, ModelState));
                    }               
                }

                using (var DB = new JacShopDBEntities())
                {
                    if (productEdit != null && ModelState.IsValid)
                    {
                        DB.uspInsertNewProduct(
                            productEdit.Category,
                            productEdit.ProductName,
                            productEdit.ProductCode,
                            productEdit.Price,
                            productEdit.Description);
                        DB.SaveChanges();
                    }
                    return Json(new[] { productEdit }.ToDataSourceResult(request, ModelState));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}