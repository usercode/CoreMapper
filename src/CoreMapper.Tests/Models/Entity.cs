using CoreMapper.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Tests.Models
{
    public class Entity : IEntity<long>
    {
        public long Id { get; set; }

        public bool IsNew => false;
        
    }
}
