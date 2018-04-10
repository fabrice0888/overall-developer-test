using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Models;
using System.Data.Entity;
using System.Text;
using System.Web.Http.Cors;

namespace WebApi.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/location")]
    public class LocationApiController : ApiController
    {

        private ApiContext db = new ApiContext();
        private string CLIENT_ID = "WMLYW1FKLOHN3G1XALGYPNKA2EFSEUE0LFCJFF3OHZUFEVAR";
        private string CLIENT_SECRET = "HJDXZU4HSWQKDVF24WEYYU4I4SWF0SJVWAONKXR4YC2GY1AK";

        private string API_KEY = "d6f5e0c57427ad9d86e7366f5771a804";
        private string API_SECRET = "ffa83a0dfd82811e";
 

        //get geo coord from fourquare
        [Route("{search}")]
        public IHttpActionResult GetLocation(string search)
        {

            string URL = "https://api.foursquare.com/v2/venues/search";
            string urlParameters = "?near='" + search + "'" + "&client_id=" + CLIENT_ID + "&client_secret=" + CLIENT_SECRET + "&v=" + DateTime.Now.ToString("yyyyMMdd");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
       
            // List data response.
            HttpResponseMessage response =  client.GetAsync(urlParameters).Result;  // Blocking call!

            Location loc = new Location();

            if (response.IsSuccessStatusCode)
            {
               

                try
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(res);


                    loc.Name = jsonData.response.geocode.feature.matchedName;
                    loc.Lat = jsonData.response.geocode.feature.geometry.center.lat;
                    loc.Long = jsonData.response.geocode.feature.geometry.center.lng;
                    //var dataObjects = response.Content.ReadAsAsync<IEnumerable<string>>().Result;
                    //foreach (var d in dataObjects)
                    //{
                    //    Console.WriteLine("{0}", d);
                    //}

                }
               catch(Exception ex)
                {

                }
                
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return Ok(loc);
        }

               

        //get list of locations
        public IHttpActionResult Get()
        {
            AuthenticationHeaderValue authenticationHeaderValue = Request.Headers.Authorization;
            var userNamePasswordString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationHeaderValue.Parameter));
            string usrename = userNamePasswordString.Split(':')[0];
            string password = userNamePasswordString.Split(':')[1];

            int userId = GetUserId(usrename);


            var locations = db.Location.Where(x=> x.UserId == userId).ToList();

            var response = new
            {
                recordsTotal = locations.Count(),
                recordsFiltered = locations.Count(),
                data = locations.ToArray()
            };
            return Ok(response);
        }

        //save location per user
        [HttpPost]
        [ResponseType(typeof(Location))]
        [Route("~/api/postLocation")]
        public IHttpActionResult PostLocation(LocationVM location)
        {
            AuthenticationHeaderValue authenticationHeaderValue = Request.Headers.Authorization;
            var userNamePasswordString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationHeaderValue.Parameter));
            string usrename = userNamePasswordString.Split(':')[0];
            string password = userNamePasswordString.Split(':')[1];

            int userId = GetUserId(usrename);

            var loc = db.Location.Where(x => x.Name == location.Name && x.UserId == userId).FirstOrDefault();

            if (loc != null)
            {
                return Ok();
            }

            loc = new Location();
            loc.Name = location.Name;
            loc.Long = location.Long;
            loc.Lat = location.Lat;
            loc.CreatedDate = DateTime.Now;
            loc.UserId = userId;

            db.Location.Add(loc);

            db.SaveChanges();
        

            return CreatedAtRoute("DefaultApi", new { controller = "locations", id = loc.Id }, loc);
        }


        //update locations per user
        [HttpPut]
        [ResponseType(typeof(Location))]
        [Route("~/api/putLocation")]
        public IHttpActionResult PutLocation(LocationEditVM location)
        {
            AuthenticationHeaderValue authenticationHeaderValue = Request.Headers.Authorization;
            var userNamePasswordString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationHeaderValue.Parameter));
            string usrename = userNamePasswordString.Split(':')[0];
            string password = userNamePasswordString.Split(':')[1];

            int userId = GetUserId(usrename);

            var loc = db.Location.Where(x => x.Id ==location.Id && x.UserId == userId).FirstOrDefault();

            if (loc == null)
            {
                return Ok();
            }

     
            loc.Name = location.Name;
            loc.Long = location.Long;
            loc.Lat = location.Lat;
            loc.UpdatedDate = DateTime.Now;


            db.Entry(loc).State = EntityState.Modified;

            db.SaveChanges();


            return CreatedAtRoute("DefaultApi", new { controller = "locations", id = loc.Id }, loc);
        }

       //get location by Id
        [Route("{Id:int}")]
        public IHttpActionResult GetLocationById(int Id)
        {

            var loc = db.Location.Where(x => x.Id == Id).FirstOrDefault();

            if (loc == null)
            {
                return NotFound();
            }

            return Ok(loc);
        }

        //delete a location
        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("~/api/location/delete/{Id:int}")]
        public IHttpActionResult DeleteLocation(int Id)
        {

            Location location = db.Location.Find(Id);
            db.Location.Remove(location);
            db.SaveChanges();    
            return Ok();
        }


        //get images from flickr bases on geo coord
        [HttpGet]
        [Route("~/api/location/photo/lng/{lng:float}/lat/{lat:float}")]      
        public IHttpActionResult SearchPhoto(float lng, float lat)
       // public IEnumerable<Fpicture> SearchPhoto(float lng, float lat)
        {
                   
    
            string URL = "https://api.flickr.com/services/rest";
            string urlParameters = "?method=flickr.photos.search&api_key="+ API_KEY + "&lat="+ lat + "&lon="+ lng + "&extras=path_alias%2Curl_n+&format=json&nojsoncallback=1";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!

            List<Fpicture> pics = new List<Fpicture>();

            if (response.IsSuccessStatusCode)
            {


                try
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(res);
                    var imgs = jsonData.photos.photo;

                    foreach (var d in imgs)
                    {
                        Fpicture p = new Fpicture();
                        p.Id = d.id;
                        p.Title = d.title;
                        p.Url = d.url_n;
                        p.Width = d.width_n;
                        p.Height = d.height_n;

                        pics.Add(p);
                    }

                

                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }


            var responsedt = new
            {
                recordsTotal = pics.Count(),
                recordsFiltered = pics.Count(),
                data = pics.ToArray()
            };
            return Ok(responsedt);
            // return pics;
        }


        //save favorite image of user
        [HttpPost]
        [ResponseType(typeof(Image))]
        [Route("~/api/location/favorites")]
        public IHttpActionResult PostFavorites(Spicture location)
        {      

            var loc = db.Location.Where(x => x.Id == location.LocId).FirstOrDefault();

            if (loc == null)
            {
                return Ok();
            }


            var simg = db.Image.Where(x => x.ImgId == location.Id).FirstOrDefault();

            Image img = new Image();

            if (simg == null)
            {


                img.ImgId = location.Id;
                img.LocationId = location.LocId;
                img.Title = location.Title;
                img.Url = location.Url;
                img.Date = DateTime.Now;

                db.Image.Add(img);

                db.SaveChanges();
            }
            else
                img = simg;


            return CreatedAtRoute("DefaultApi", new { controller = "locations", id = img.Id }, img);
        }


        //get list of favorite images
        [Route("~/api/location/favoriteImages/{locationId:int}")]
        public IHttpActionResult GetFavoriteImges(int locationId)
        {

            var imges = db.Image.Where(x => x.LocationId == locationId).ToList();

            ImpPreview imgPrev = new ImpPreview();

            List<ImgConfig> previewconfig = new List<ImgConfig>();
            List<string> preview = new List<string>();

            foreach (var img in imges)
            {

                previewconfig.Add(new ImgConfig
                {
                    caption =img.Title,
                    key = img.Id,
                    size = 100,
                    width = "120px",
                    url = "/Locations/Delete",
                    showDelete = false,
                    type = "image"                   

                });

                preview.Add(img.Url);

             
            }

            imgPrev.preview = preview;
            imgPrev.previewconfig = previewconfig;

            return  Ok(imgPrev);

        }

        public int GetUserId(string username)
        {
            var user = db.User.Where(x => x.Email == username).FirstOrDefault();

            if (user != null)
                return user.Id;
            else
                return 0;
        }
        //public DataTableResponse Get()
        //{
        //    ApiContext db = new ApiContext();
        //    var locations = db.Location.ToList();

        //    return new DataTableResponse
        //    {
        //        recordsTotal = locations.Count(),
        //        recordsFiltered = locations.Count(),
        //        data = locations.ToArray()
        //    };
        //}


        //public IHttpActionResult Get() //legacy
        //{
        //    ApiContext db = new ApiContext();
        //    var locations = db.Location.ToList();

        //    var response = new
        //    {
        //        sEcho = 1,
        //        iTotalRecords = locations.Count(),
        //        iTotalDisplayRecords = locations.Count(),
        //        aaData = locations
        //    };
        //    return Ok(response);
        //}

    }
}