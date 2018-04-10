using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class LocationsController : BaseController
    {
     
        // GET: Locations
        public ActionResult Index()
        {
          
            return View();
        }
   
        // GET: Locations/Create
        public ActionResult Create()
        {      
            return View();
        }
 
        // GET: Locations/Edit/5
        public ActionResult Edit(int? locId)
        {           
            return View();
        }
        
        public ActionResult AddPhoto(int? locId)
        {
            return View();
        }


        // GET: Locations/ViewPhoto/5
        public ActionResult ViewPhoto(int? locId)
        {             
            return View();
        }

       
    }
}
