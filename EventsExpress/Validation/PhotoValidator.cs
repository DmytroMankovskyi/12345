﻿namespace EventsExpress.Validation
{
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using EventsExpress.Core.Extensions;
    using EventsExpress.ViewModels;
    using FluentValidation;
    using FreeImageAPI;
    using Microsoft.AspNetCore.Http;

    public class PhotoValidator : AbstractValidator<PhotoViewModel>
    {
        private readonly string[] allowedExtensions = { "jpeg", "jpg", "png", "bmp" };
        private readonly int tenMegaBytesInBytes = 1024 * 1024 * 10;

        public PhotoValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(f => f.Photo).NotEmpty().Must(ValidImage).OverridePropertyName("image")
                .WithMessage("The upload file should be a valid image!");
            RuleFor(f => f.Photo).Must(f => !allowedExtensions.Contains(Path.GetExtension(f.FileName))).OverridePropertyName("image")
                .WithMessage("Accepted file formats are .jpeg, .jpg, .png, or .bmp");
            RuleFor(f => f.Photo).Must(f => f.Length < tenMegaBytesInBytes).OverridePropertyName("image")
                .WithMessage("File size can not exceed 10 MB");
            RuleFor(f => f.Photo).Must(ValidImageSize).OverridePropertyName("image")
                .WithMessage("Image size should be at least 400x400px");
        }

        private bool ValidImage(IFormFile file) => file.IsImage();

        private bool ValidImageSize(IFormFile file)
        {
            using var memoryStream = file.ToMemoryStream();
            var image = new FreeImageBitmap(memoryStream);
            return image.Width >= 400 && image.Height >= 400;
        }
    }
}
