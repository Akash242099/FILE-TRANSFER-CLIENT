using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Users
    {
        
        public int UserID { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string EmailId { get; set; }
        public int ContantNo { get; set; }
        public bool isAdmin { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string PassWord { get; set; }

    }
}