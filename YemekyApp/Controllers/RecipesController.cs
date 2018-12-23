using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YemekyApp.Models;

namespace YemekyApp.Controllers
{
    public class RecipesController : Controller
    {
        private YemekyAppEntities db = new YemekyAppEntities();

     

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
