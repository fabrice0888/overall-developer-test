using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ApiContext : DbContext
    {


        public ApiContext() : base("name=ApiContext")
        {
            Database.SetInitializer<ApiContext>(new MigrateDatabaseToLatestVersion<ApiContext, Migrations.Configuration>());
            //will migrate and seed db on first connection

            Configuration.LazyLoadingEnabled = false;
        }

        public System.Data.Entity.DbSet<WebApi.Models.User> User { get; set; }
        public System.Data.Entity.DbSet<WebApi.Models.Location> Location { get; set; }
        public System.Data.Entity.DbSet<WebApi.Models.Image> Image { get; set; }
    }
}
