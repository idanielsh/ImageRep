using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageRepServiceLibrary.Model;
using ImageRepServiceLibrary.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageRep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageManager _imageManager;
        public ImageController(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        [HttpGet("{imageId}")] // api/Image/{imageId}?loadTags={load}
        public async Task<IActionResult> GetImageAsync([FromRoute]Guid imageId, [FromQuery] bool loadTags = false)
        {
            var image = await _imageManager.GetImageAsync(imageId, loadTags);
            if (image != default)
                return Ok(image);
            throw new ArgumentException($"Image with ID {imageId} could not be found");

        }

        [HttpGet("extenstions")] // api/Image/extensions
        public IActionResult GetExtensions()
        {
            return Ok(_imageManager.GetLegalFileExtensions());

        }

        [HttpGet]
        public async Task<IActionResult> GetImageListAsync([FromQuery]bool loadTags = false, [FromQuery]string name = null, [FromQuery]string description = null, [FromQuery]string tagName = null)
        {
            return Ok(await _imageManager.SearchImagesAsync(loadTags, name, description,tagName));
        }

        [HttpPost] 
        public async Task<IActionResult> AddImageAsync([FromBody] Image image )
        {
            var response = await _imageManager.SaveImageAsync(image);
            if (response)
                return Ok();
            return BadRequest("Image Could not be added."); 
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateImageAsync([FromBody] Image image, [FromQuery] bool updateTags = false)
        {
            bool success = await _imageManager.UpdateImageAsync(image, updateTags);

            if (success)
                return Ok();
            return NotFound($"Image not updated succesfully");
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute] Guid imageId)
        {
            bool success = await _imageManager.DeleteImageAsync(imageId);

            if (success)
                return Ok();
            return NotFound($"Image with ID {imageId} could not be deleated");
        }
    }
}
