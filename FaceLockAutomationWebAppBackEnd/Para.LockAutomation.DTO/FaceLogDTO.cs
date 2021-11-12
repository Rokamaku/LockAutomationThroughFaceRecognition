using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.DTO
{
    public class FaceLogDTO
    {
        public int Id { get; set; }
        public int PersonGroupId { get; set; }
        public List<FaceRectangle> FaceRectangles { get; set; }
        public AzureFile File { get; set; }
        public List<Guid> Persons { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
