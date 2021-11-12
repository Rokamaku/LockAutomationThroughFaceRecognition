using Microsoft.AspNetCore.Http;
using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;

namespace Para.LockAutomation.DTO
{
    public class FaceDTO
    {
        public int FaceId { get; set; }
        public int PersonId { get; set; }
        public int PerosnGroupId { get; set; }
        public string PersonName { get; set; }
        public AzureFile Files { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
    public class UploadFaceDTO
    {
        public FaceEntity NewFace { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
