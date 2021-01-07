using System;
using System.Collections.Generic;
using System.Text;

namespace ImageRepServiceLibrary.Model
{
    public class Tag
    {
        public Guid TagId { get; set; }
        public string Name { get; set; }
        public Guid ImageKey { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Tag tag &&
                   TagId.Equals(tag.TagId);
        }
    }
}
