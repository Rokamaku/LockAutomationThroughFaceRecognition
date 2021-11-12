using System;

namespace Para.LockAutomation.Utils
{
    public class AzureStorageConfig
    {
        public string AccountName { get; set; }
        public string AccessKey { get; set; }
        public string FaceDatasetContainer { get; set; }
        public string FaceLogContainer { get; set; }
    }
}
