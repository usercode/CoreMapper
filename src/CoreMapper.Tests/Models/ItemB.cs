using CoreMapper.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Tests
{
    public class ItemB : Entity
    {
        public ItemB()
        {
            if (Items == null)
            {
                Items = new List<SubItem>();
            }
        }

        public virtual String Value { get; set; }

        public virtual int Counter { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual double Number { get; set; }

        public virtual CategoryB Category { get; set; }

        public virtual IList<SubItem> Items { get; set; }
    }
}
