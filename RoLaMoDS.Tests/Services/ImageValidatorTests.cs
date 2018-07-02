using System;
using Xunit;
using RoLaMoDS.Services;
using RoLaMoDS.Services.Interfaces;
using System.Drawing;

namespace RoLaMoDS.Tests.Services
{
    public class ImageValidatorTests
    {
        private readonly IImageValidator _IImageValidator;
        public ImageValidatorTests(){
            _IImageValidator = new ImageValidator();
        }
        [Fact]
        public void IsImageValidSizeTest1()
        {
            Bitmap bmp = new Bitmap(110,500);
            Assert.True(_IImageValidator.IsImageValidSize(bmp));
        }

        [Fact]
        public void IsImageValidSizeTest2()
        {
            Bitmap bmp = new Bitmap(50,500);
            Assert.False(_IImageValidator.IsImageValidSize(bmp));
        }
        
        [Fact]
        public void IsImageValidSizeTest3()
        {
            Bitmap bmp = new Bitmap(100,5000);
            Assert.True(_IImageValidator.IsImageValidSize(bmp));
        }
    }
}
