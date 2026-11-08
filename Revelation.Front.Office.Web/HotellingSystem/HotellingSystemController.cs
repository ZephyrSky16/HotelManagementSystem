using System;
using System.Web.Mvc;
using Revelation.Front.Office.Repositories.HotellingSystem;

namespace Revelation.Front.Office.Web.HotellingSystem
{
    /// <summary>
    /// Base for controllers backed by the HotellingSystem database: one
    /// DbContext per request, disposed with the controller.
    /// </summary>
    public abstract class HotellingSystemController : Controller
    {
        private HotellingSystemDbContext _db;

        protected HotellingSystemDbContext Db => _db ?? (_db = new HotellingSystemDbContext());

        /// <summary>Value written to the InsertedBy/UpdatedBy columns.</summary>
        protected string CurrentUser =>
            User != null && User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(User.Identity.Name)
                ? User.Identity.Name
                : Environment.UserName;

        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                _db.Dispose();
                _db = null;
            }
            base.Dispose(disposing);
        }
    }
}
