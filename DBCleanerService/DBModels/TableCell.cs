using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBCleanerService.DBModels
{
    public class TableCell
    {
        [Key]
        public int ID { get; set; }
        public int DocumentID { get; set; }
        public int ColumnNum { get; set; }
        public int RowNum { get; set; }
        public string CellValue { get; set; }
    }
}
