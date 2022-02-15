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

namespace JacWebShopGUI.Controllers
{
    public class GridProductsManagementController : Controller
    {
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

        public ActionResult GetAllProducts([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
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
        public ActionResult CreateNewProduct([DataSourceRequest] DataSourceRequest request, uspGetAllProducts_Result productEdit)
        {
            try
            {
              
                productEdit.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 

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