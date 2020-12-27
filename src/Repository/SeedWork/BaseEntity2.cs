using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.SeedWork
{
    public class BaseEntity2<T> : BaseEntity
    {
        public T Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
    }
}
