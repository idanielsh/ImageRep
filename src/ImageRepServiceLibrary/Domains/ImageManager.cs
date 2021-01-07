using ImageRepServiceLibrary.DataAccess;
using ImageRepServiceLibrary.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImageRepServiceLibrary.Domains
{
    public interface IImageManager
    {
        public Task<bool> SaveImageAsync(Image image);
        public Task<Image> GetImageAsync(Guid guid, bool loadTags);
        public Task<IEnumerable<Image>> SearchImagesAsync(bool loadTags, string name = null, string description = null, string tagName = null);
        public Task<bool> DeleteImageAsync(Guid guid);
        public Task<bool> UpdateImageAsync(Image image, bool updateTags);
        public IEnumerable<string> GetLegalFileExtensions();
    }


    public class ImageManager : IImageManager
    {
        private readonly IImageDataAccess _imageDataAccess;
        private readonly IEnumerable<string> _extenstionsAllowed = new List<string>()
        {
            ".jpeg",
            ".jpg"
        };

        public ImageManager(IConfiguration configuration)
        {
            _imageDataAccess = new ImageDataAccess(new TagDataAccess(configuration), configuration);
        }

        /// <summary>
        /// Returns true iff (An image exists with this guid AND the image (with associated tags) was removed)
        /// </summary>
        public async Task<bool> DeleteImageAsync(Guid guid)
        {
            int rowsChanged = await _imageDataAccess.DeleteImageAsync(guid);
            return rowsChanged > 0;
        }

        public async Task<Image> GetImageAsync(Guid guid, bool loadTags)
        {
            return await _imageDataAccess.GetImageAsync(guid, loadTags);
        }

        /// <summary>
        /// Returns true if the Image was saved succesfully. False otherwise
        /// </summary>
        public async Task<bool> SaveImageAsync(Image image)
        {
            int rowsChanged;
            try
            {
                rowsChanged = await _imageDataAccess.SaveImageAsync(image);
            }
            catch (Exception e)
            {
                return false;
            }
            return rowsChanged > 0;
        }

        public IEnumerable<string> GetLegalFileExtensions()
        {
            return _extenstionsAllowed;
        }

        public async Task<IEnumerable<Image>> SearchImagesAsync(bool loadTags, string name = null, string description = null, string tagName = null)
        {
            return await _imageDataAccess.SearchImagesAsync(loadTags, name, description, tagName);
        }

        public async Task<bool> UpdateImageAsync(Image image, bool updateTags)
        {
            int rowsChanged;
            try
            {
                rowsChanged = await _imageDataAccess.UpdateImageAsync(image, updateTags);
            }
            catch (Exception)
            {
                return false;
            }
            return rowsChanged > 0;
        }
    }
}
