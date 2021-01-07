using ImageRepServiceLibrary.DataAccess;
using ImageRepServiceLibrary.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace ImageRepServiceLibrary.Domains
{
    public interface ITagManager
    {
        public Task<bool> DeleteTagAsync(Guid tagGuid);
        public Task<IEnumerable<string>> GetDistinctTagsAsync();
        public Task<bool> InsertTagAsync(Tag tag);
    }
    public class TagManager : ITagManager
    {
        private readonly ITagDataAccess _tagDataAccess;
        public TagManager(IConfiguration configuration)
        {
            _tagDataAccess = new TagDataAccess(configuration);
        }

        /// <summary>
        /// Returns true iff (the Tag exists AND the tag was deleted)
        /// </summary>
        /// <param name="tagGuid">The tag to be deleted</param>
        public async Task<bool> DeleteTagAsync(Guid tagGuid)
        {
            int rowsChanged = await _tagDataAccess.DeleteTagAsync(tagGuid);
            return rowsChanged > 0;
        }

        public async Task<IEnumerable<string>> GetDistinctTagsAsync()
        {
            return await _tagDataAccess.GetDistinctTagsAsync();
        }

        /// <summary>
        /// Returns true iff all parameters of tag were valid
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<bool> InsertTagAsync(Tag tag)
        {
            int rowsChanged;
            try
            {
                rowsChanged = await _tagDataAccess.InsertSingleTagAsync(tag);
            }catch(Exception)
            {
                return false;
            }
            return rowsChanged > 0;
        }

    }
}
