using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepServiceLibrary.Model
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        public override bool Equals(object obj)
        {
            return obj is Image image &&
                   Id.Equals(image.Id);
        }
    }
}
