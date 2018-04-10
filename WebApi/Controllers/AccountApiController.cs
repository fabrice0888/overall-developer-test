using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountApiController : ApiController
    {
        private ApiContext db = new ApiContext();

        [AllowAnonymous]
        [HttpPost]     
        [Route("~/api/account/register")]
        public IHttpActionResult Register(Register model)
        {

            var usr = db.User.Where(x => x.Email ==model.Email).FirstOrDefault();

            if (usr != null) //if user exist, modify only password
            {
                usr.Password = Utils.MD5Hash(model.Password);
                usr.UpdatedDate = DateTime.Now;
                db.Entry(usr).State = EntityState.Modified;
            }
            else
            {
                usr = new Models.User();
                usr.Email = model.Email;
                usr.Password = Utils.MD5Hash(model.Password);
                usr.CreatedDate = DateTime.Now;
                db.User.Add(usr);
            }

            db.SaveChanges();


           return Ok(true);
        }


        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(User))]
        [Route("~/api/account/login")]
        public IHttpActionResult Login(Login model)
        {

            string pass = Utils.MD5Hash(model.Password);
            var usr = db.User.Where(x => x.Email == model.Email && x.Password ==pass).FirstOrDefault();

            if (usr == null) 
            {
                return Ok(false);
            }
            else
            {
                return Ok(true);
            }

 
        }


    }
}
