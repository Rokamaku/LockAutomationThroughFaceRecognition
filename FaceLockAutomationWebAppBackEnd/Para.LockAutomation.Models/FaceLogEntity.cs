using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Models
{
    public class FaceLogEntity : GenericEntity
    { 
        public List<Guid> Persons { get; set; }
        public PersonGroupEntity PersonGroup { get; set; }
        public int PersonGroupId { get; set; }
        public AzureFile File { get; set; }
        public List<FaceRectangle> FaceRectangles { get; set; } 
        public List<double> Confidences { get; set; }
    }

    public class FaceRectangle
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
    }
}
