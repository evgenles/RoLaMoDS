using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RoLaMoDS.Models;
using Microsoft.AspNetCore.Identity;
using System;
namespace RoLaMoDS.Data
{
    public class ApplicationDBContext : IdentityDbContext<UserModel, IdentityRole<Guid>, Guid>
    {
        public DbSet<ImageDBModel> Images {get;set;}
        public DbSet<CellDB> Cells {get;set;}
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);
<<<<<<< HEAD
=======
            builder.Entity<UserModel>()
                .HasMany(user=>user.DownloadedImages)
                .WithOne(image=>image.User)
                .HasForeignKey(image=>image.UserId);
>>>>>>> 9d198b4b1633309de920499864efac7e3f9b23a2
        }
    }
}