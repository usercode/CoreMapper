using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Tests.Models
{
    class UpdateItem
    {
        public UpdateItem()
        {
            if (ItemsAdded == null)
            {
                ItemsAdded = new List<ItemToItem>();
            }
        }

        public String Value { get; set; }

        public double? Number { get; set; }

        public long? CategoryId { get; set; }

        public IList<ItemToItem> ItemsAdded { get; set; }
    }
}
