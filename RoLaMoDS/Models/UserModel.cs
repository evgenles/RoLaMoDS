using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System;

namespace RoLaMoDS.Models
{
<<<<<<< HEAD
    public class UserModel:IdentityUser<Guid>
    {
                public List<ImageDBModel> DownloadedImages {get;set;}
=======
    public class UserModel : IdentityUser<Guid>
    {
        public List<ImageDBModel> DownloadedImages { get; set; } = new List<ImageDBModel>();
>>>>>>> 9d198b4b1633309de920499864efac7e3f9b23a2
    }
}