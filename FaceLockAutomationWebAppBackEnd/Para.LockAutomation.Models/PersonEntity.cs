using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Models
{
    public class PersonEntity : GenericEntity
    {
        public PersonGroupEntity PersonGroup { get; set; }
        public int PersonGroupId { get; set; }
        public List<FaceEntity> Faces { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string InternalName
        {
            get
            {
                return FirstName + LastName;
            }
        }
    }
}
