using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Models
{
    public class HeaderReturnModel
    {
        public int ColumnIndex { get;set; }
        public string ColumnName { get; set; }
    }

    public class TableSettingsModel 
    {
        public int colIDVal { get; set; }
		public string columnTypeVal { get; set; }
		public bool uniqueVal { get; set; }
		public bool allowNullVal { get; set; }
        public string formatVal { get; set; }
        public List<string> optionsVal { get; set; }
        public List<string> rangesVal { get; set; }
    }
}
