using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JacWebShopGUI.Models;
using Kendo.Mvc.Extensions;

namespace JacWebShopGUI.Controllers
{
    public class GridProductsManagementController : Controller
    {
        // GET: GridProductsManagement
        public ActionResult Index()
        {
            return View();
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

    }
}