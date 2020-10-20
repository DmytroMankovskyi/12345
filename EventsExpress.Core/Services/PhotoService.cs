﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using EventsExpress.Core.Extensions;
using EventsExpress.Core.Infrastructure;
using EventsExpress.Core.IServices;
using EventsExpress.Db.Entities;
using EventsExpress.Db.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EventsExpress.Core.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _db;
        private readonly IOptions<ImageOptionsModel> _widthOptions;

        public PhotoService(
            IUnitOfWork uow,
            IOptions<ImageOptionsModel> opt)
        {
            _db = uow;
            _widthOptions = opt;
        }

        public async Task<Photo> AddPhoto(IFormFile uploadedFile)
        {
            if (!IsValidImage(uploadedFile))
            {
                throw new ArgumentException();
            }

            byte[] imgData;
            using (var reader = new BinaryReader(uploadedFile.OpenReadStream()))
            {
                imgData = reader.ReadBytes((int)uploadedFile.Length);
            }

            var photo = new Photo
            {
                Thumb = GetResizedBytesFromFile(uploadedFile, _widthOptions.Value.Thumbnail),
                Img = GetResizedBytesFromFile(uploadedFile, _widthOptions.Value.Image),
            };

            _db.PhotoRepository.Insert(photo);
            await _db.SaveAsync();

            return photo;
        }

        public async Task Delete(Guid id)
        {
            var photo = _db.PhotoRepository.Get(id);
            if (photo != null)
            {
                _db.PhotoRepository.Delete(photo);
                await _db.SaveAsync();
            }
        }

        private static bool IsValidImage(IFormFile file) => file != null && file.IsImage();

        public byte[] GetResizedBytesFromFile(IFormFile file, int newWidth)
        {
            using (var memoryStream = file.OpenReadStream())
            {
                var oldBitMap = new Bitmap(memoryStream);
                var newSize = new Size
                {
                    Width = newWidth,
                    Height = (int)(oldBitMap.Size.Height * newWidth / oldBitMap.Size.Width),
                };

                var newBitmap = new Bitmap(oldBitMap, newSize);

                return ImageToByteArray(newBitmap);
            }
        }

        private byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
