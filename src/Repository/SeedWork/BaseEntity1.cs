using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.SeedWork
{
    public class BaseEntity1<T> : BaseEntity
    {
        public T Id { get; set; }
    }
}
