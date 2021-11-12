using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.DTO
{
    public class PersonDTO
    {
        public int? Id { get; set; }
        public int PersonGroupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string InternalName {
            get
            {
                return FirstName + LastName;
            }
        }
        public string ExternalName {
            get
            {
                return FirstName + ' ' + LastName;
            }
        }
    }
}
