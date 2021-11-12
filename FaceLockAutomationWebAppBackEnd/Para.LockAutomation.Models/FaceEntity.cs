using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Models
{
    public class FaceEntity : GenericEntity
    {
        public PersonEntity Person { get; set; }
        public int PersonId { get; set; }
        public AzureFile File { get; set; }
    }
}
