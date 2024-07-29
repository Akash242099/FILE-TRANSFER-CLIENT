using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    [Serializable]
    public class Jobs
    {
        

        public int Id { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public string SourcePath { get; set; }
        public string DestPath { get; set; }
        public int CreatedBy { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }
}