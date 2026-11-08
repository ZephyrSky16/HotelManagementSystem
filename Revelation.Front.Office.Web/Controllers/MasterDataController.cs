using System;
using System.Collections.Generic;
using System.Web.Mvc;
using log4net;
using Revelation.Front.Office.Web.HotellingSystem;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    /// <summary>
    /// One controller for every HotellingSystem lookup table; the table is
    /// chosen by the {key} in /Setup/{key} (see MasterDataRegistry).
    /// </summary>
    public class MasterDataController : HotellingSystemController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MasterDataController));

        public ActionResult Index(string key)
        {
            var definition = MasterDataRegistry.Find(key);
            if (definition == null) { return HttpNotFound(); }

            return View(new MasterDataPageViewModel
            {
                Definition = definition,
                Rows = definition.CreateHandler(Db).GetRows()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Save(string key, int? id, bool? isActive)
        {
            return Execute(key, handler =>
            {
                var error = handler.Save(id, name => Request.Form[name], isActive, CurrentUser);
                return error == null ? Json(new { success = true }) : Json(new { success = false, message = error });
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetActive(string key, int id, bool active)
        {
            return Execute(key, handler =>
            {
                handler.SetActive(id, active, CurrentUser);
                return Json(new { success = true });
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string key, int id)
        {
            return Execute(key, handler =>
            {
                handler.Delete(id);
                return Json(new { success = true });
            });
        }

        private JsonResult Execute(string key, Func<IMasterDataHandler, JsonResult> action)
        {
            var definition = MasterDataRegistry.Find(key);
            if (definition == null) { return Json(new { success = false, message = "Unknown table." }); }

            try
            {
                return action(definition.CreateHandler(Db));
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error("HotellingSystem " + key + " update failed", ex);
                return Json(new { success = false, message = "The database update failed. See the application log for details." });
            }
        }
    }
}
