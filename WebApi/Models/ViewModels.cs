using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class DataTableResponse
    {
        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object[] data { get; set; }
        public string error { get; set; }
    }

    public class LocationVM
    {
        public string Name { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
    }

    public class LocationEditVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
    }

    public class Fpicture
    {
        public double Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class Spicture
    {
        public double Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int LocId { get; set; }
        
    }
    public class ImgConfig
    {
        public string caption { get; set; }
        public string width { get; set; }
        public int size { get; set; }
        public string url { get; set; }
        public int key { get; set; }
        public bool showDelete { get; set; }
        public string type { get; set; }
    }
    public class ImpPreview
    {
        public List<ImgConfig> previewconfig { get; set; }
        public List<string> preview { get; set; }
    }

    public class Register
    {
       
        public string Email { get; set; }

        [DataType(DataType.Password)]    
        public string Password { get; set; }

        [DataType(DataType.Password)]    
        public string ConfirmPassword { get; set; }
    }

    public class Login
    {
    
        public string Email { get; set; }    
        public string Password { get; set; } 
       
    }
}
