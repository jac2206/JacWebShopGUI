using JacWebShopGUI.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using JacWebShopGUI.Util;
using Newtonsoft.Json;

namespace JacWebShopGUI.Controllers
{
    public class APIGridProductsManagementController : Controller
    {
        // GET: APIGridProductsManagement
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
                    string responseBody = APICallUtil.GetMethodConsumer("http://localhost/JacWebShopAPI/api/ProductsManagement/GetAllProducts");
                    IQueryable<uspGetAllProducts_Result> ObjOrderList = JsonConvert.DeserializeObject<List<uspGetAllProducts_Result>>(responseBody).AsQueryable();
                    DataSourceResult result = ObjOrderList.ToList().ToDataSourceResult(request);
                    return Json(result);   
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
                string jsonProductString = JsonConvert.SerializeObject(productEdit);
                APICallUtil.PostMethodConsumer("http://localhost/JacWebShopAPI/api/ProductsManagement/CreateNewProduct", jsonProductString);
                return Json(new[] { productEdit }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}