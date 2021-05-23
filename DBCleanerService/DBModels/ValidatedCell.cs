using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBCleanerService.DBModels
{
    public class ValidatedCell
    {
        public int ID { get; set; }
        public int TableCellsID { get; set; }
        public double ErrorCode { get; set; }
    }
}
