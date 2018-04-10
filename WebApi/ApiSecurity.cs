using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi
{
    public class ApiSecurity
    {
      
        public static bool VaidateUser(string username, string password)
        {
           
            ApiContext db = new ApiContext();

            string pass = Utils.MD5Hash(password);
            var usr = db.User.Where(x => x.Email == username && x.Password == pass).FirstOrDefault();

            if (usr == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
