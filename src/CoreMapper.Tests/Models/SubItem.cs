using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Tests.Models
{
    public class SubItem : Entity
    {
        public ItemB ItemB { get; set; }

        public int SortKey { get; set; }
    }
}
