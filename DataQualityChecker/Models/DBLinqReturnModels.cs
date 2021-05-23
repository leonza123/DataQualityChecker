using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Models
{
    public class ErrorForTableReturnModel
    {
        public int ColumnNum { get; set; }
        public int RowNum { get; set; }
    }

    public class HeaderReturnData 
    {
        public int ColumnNum { get; set; }
        public string CellValue { get; set; }
    }

    public class CellReturnData 
    {
        public int ColumnNum { get; set; }
        public int RowNum { get; set; }
        public string CellValue { get; set; }
    }

    public class DocumentReturnData 
    {
        public string Name { get; set; }
    }

    public class FileDataForDownload
    {
        public List<HeaderReturnData> headers { get; set; }
        public List<List<CellReturnData>> cells { get; set; }
        public DocumentReturnData documentData { get; set; }
    }

    public class ErrorCellModelForTable 
    {
        public int ColumnNum { get; set; }
        public double ErrorCode { get; set; }
    }

    public class NewValueCheckReturnModel 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public bool searchForDuplicate { get; set; }
    }

    public class DataForAnalysisModel 
    {
        public int ColumnNum { get; set; }
        public int RowNum { get; set; }
        public double? ErrorCode { get; set; }
        public string CellValue { get; set; }
    }

    public class AnalysisModel
    {
        public string title { get; set; }
        public string type { get; set; }
        public int validCount { get; set; }
        public int inValidCount { get; set; }
        public int nullCount { get; set; }
        public int notNullCount { get; set; }
        public int uniqueCount { get; set; }
        public int distinctCount { get; set; }
        public int specialCharsCount { get; set; }
    }


    public class ValidatedCellsReturnModel 
    {
        public int ID { get; set; }
        public int TableCellsID { get; set; }
        public int ColumnNum { get; set; }
        public int RowNum { get; set; }
        public double ErrorCode { get; set; }
    }

    public class ValueOptionReturnModel 
    {
        public int ID { get; set; }
        public int HeaderID { get; set; }
        public int ColumnNum { get; set; }
        public string OptionVal { get; set; }
    }
}
