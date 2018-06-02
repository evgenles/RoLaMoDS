using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System;

namespace RoLaMoDS.Models
{
    public class UserModel : IdentityUser<Guid>
    {
        public List<ImageDBModel> DownloadedImages { get; set; } = new List<ImageDBModel>();
    }
}