using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBCleanerService.DBModels
{
    public class DocumentStorage
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string SessionID { get; set; }
        public DateTime Modified { get; set; }
        public bool HasHeader { get; set; }
    }
}
