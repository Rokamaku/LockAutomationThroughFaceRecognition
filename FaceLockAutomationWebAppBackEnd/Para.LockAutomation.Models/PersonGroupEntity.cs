using System;
using System.Collections.Generic;

namespace Para.LockAutomation.Models
{
    public class PersonGroupEntity : GenericEntity
    {
        public List<PersonEntity> Persons { get; set; }
        public string Name { get; set; }
        public TrainingStatus TrainingStatus { get; set; }
        public bool IsDefault { get; set; }
        public double ConfindenceThreshold { get; set; }
    }

    public enum TrainingStatus
    {
        Succeeded,
        Failed,
        Running,
        NotTrain
    }
}
