using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBCleanerService.DBModels
{
    public class HeaderCell
    {
        [Key]
        public int ID { get; set; }
        public int DocumentID { get; set; }
        public int ColumnNum { get; set; }
        public string CellValue { get; set; }
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public bool Unique { get; set; }
        public string Format { get; set; }
        public bool HasOptions { get; set; }
        public string Ranges { get; set; }
    }
}
