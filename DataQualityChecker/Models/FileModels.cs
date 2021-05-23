using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Models
{
    public class ValidatedFileModel 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public FileReturnModel fileData { get; set; }
    }

    public class ValidatedCSVFileModel
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public CSVFileReturnModel fileData { get; set; }
    }

    public class CSVFileReturnModel
    {
        public string contentType { get; set; }
        public string fileName { get; set; }
        public string fileContent { get; set; }
    }

    public class FileReturnModel
    {
        public string contentType { get; set; }
        public string fileName { get; set; }
        public byte[] fileContent { get; set; }
    }
}
