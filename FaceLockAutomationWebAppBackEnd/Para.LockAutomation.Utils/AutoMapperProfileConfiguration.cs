using AutoMapper;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Utils
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<FaceLogEntity, FaceLogDTO>();
            
        }
    }
}
