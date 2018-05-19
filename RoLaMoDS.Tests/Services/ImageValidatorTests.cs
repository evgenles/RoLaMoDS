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
        public void ImageWeightAndHeightOutOfRange()
        {
            Bitmap bmp = new Bitmap(1000000,100);
            Assert.False(_IImageValidator.IsImageValid(bmp));
        }

        [Fact]
        public void ImageWeightAndHeightInRange()
        {
            Bitmap bmp = new Bitmap(200,100);
            Assert.True(_IImageValidator.IsImageValid(bmp));
        }
    }
}
