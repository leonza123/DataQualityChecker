using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.DBModels
{
    public class ValueOption
    {
        [Key]
        public int ID { get; set; }
        public int HeaderID { get; set; }
        public string OptionVal { get; set; }
    }
}
