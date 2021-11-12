using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
    public abstract class GenericEntity : BaseEntity
    {
        public Guid ObjectId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
