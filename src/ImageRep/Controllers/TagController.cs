using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageRepServiceLibrary.Domains;
using ImageRepServiceLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageRep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagManager _tagManager;

        public TagController(ITagManager tagManager)
        {
            _tagManager = tagManager;
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute] Guid tagId)
        {
            bool success = await _tagManager.DeleteTagAsync(tagId);

            if (success)
                return Ok();
            throw new ArgumentException($"Tag with ID {tagId} could not be deleated");
        }

        [HttpGet]
        public async Task<IActionResult> GetDistinctTagsAsync()
        {
            return Ok(await _tagManager.GetDistinctTagsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddTagAsync([FromBody]Tag tag)
        {
            bool success = await _tagManager.InsertTagAsync(tag);

            if (success)
                return Ok();
            throw new ArgumentException($"Tag not added succesfully");
        }
    }
}
