using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Models
{
    public class UploadFileReturnModel 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public string sessionID { get; set; }
        public List<HeaderReturnModel> headers { get; set; }
    }

    public class ValidatedDataCounters 
    {
        public int AllCells { get; set; }
        public int ErrorCells { get; set; }
        public int NullCells { get; set; }
        public int DuplicateCells { get; set; }
        public int WrongTypeCells { get; set; }
        public int WrongFormatCells { get; set; }
        public int WrongOptionsCells { get; set; }
        public int WrongRangesCells { get; set; }
    }

    public class ValidatedDataOutput 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public ValidatedDataCounters counters { get; set; }
    }

    public class ValidatedDataOutputForExistingDocument
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public ValidatedDataCounters counters { get; set; }
        public List<TableSettingsModel> settings { get; set; }
    }

    public class TableOutput
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public string data { get; set; }
    }

    public class TableHeadersOutput
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public List<string> headers { get; set; }
    }

    public class CellErrorFixModel 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public ValidatedDataCounters counters { get; set; }
    }

    public class DefaultReturnModel 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
    }

    public class AnalysisResultModel 
    {
        public bool status { get; set; }
        public string errorMessage { get; set; }
        public List<AnalysisModel> analysis { get; set; }
    }
}
