using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Models
{
    public class SettingsModel 
    {
        public string docID { get; set; }
        public List<TableSettingsModel> settings { get; set; }
    }

    public class NewElementValueModel 
    {
        public string newSessionID { get; set; }
        public int newRowNum { get; set; }
        public int newColumnNum { get; set; }
        public string newValue { get; set; }
    }

    public class FileURLModel 
    {
        public string URL { get; set; }
        public bool noHeader { get; set; }
        public int headerStarts { get; set; }
        public int rowsCount { get; set; }
    }
}
