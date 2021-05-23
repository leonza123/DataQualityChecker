using DataQualityChecker.Constants;
using DataQualityChecker.Data;
using DataQualityChecker.Helpers;
using DataQualityChecker.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.BusinessLogic
{
    public partial class UploadMethods
    {
        public string GetDataForCellErrorTable(SearchRequest request, string sessionID, int errorType)
        {
            TableOutput returnData = new TableOutput();
            returnData.recordsTotal = QualityContext.GetTotalCountOfRowsWithErrors(sessionID);
            returnData.recordsFiltered = returnData.recordsTotal;
            returnData.draw = request.Draw;
            string returnString = "";

            try
            {
                List<List<string>> cellsData = QualityContext.GetCellsDataForErrorTable(sessionID, request.Start, request.Length, errorType);
                List<string> headers = QualityContext.GetHeadersForTable(sessionID);
                //Add header for RowNum column
                headers.Add("RowNum");
                headers.Add("ErrorColumnNums");

                string dataValue = JSONHelper.BuildJSON(cellsData, headers);

                returnData.data = "{replacePl}";
                returnString = JsonConvert.SerializeObject(returnData);
                returnString = returnString.Replace("\"{replacePl}\"", dataValue);

                returnData.status = true;
            }
            catch(Exception ex) 
            {
                returnData.data = "{replacePl}";
                returnString = JsonConvert.SerializeObject(returnData);
                returnString = returnString.Replace("\"{replacePl}\"", "[]");
            }

            return returnString;
        }

        public TableHeadersOutput GetTableHeaders(string sessionID) 
        {
            TableHeadersOutput returnData = new TableHeadersOutput();

            try 
            {
                returnData.headers = QualityContext.GetHeadersForTable(sessionID);
                returnData.status = true;
            }
            catch(Exception ex) 
            {
                returnData.status = false;
            }

            return returnData;
        }

        public CellErrorFixModel FixError(NewElementValueModel newValData)
        {
            CellErrorFixModel returnData = new CellErrorFixModel();
            returnData.status = true;

            if (newValData != null && !string.IsNullOrEmpty(newValData.newSessionID) && newValData.newRowNum != 0 && newValData.newColumnNum != 0) 
            {
                try
                {
                    NewValueCheckReturnModel newValCheck = QualityContext.CheckValueForColumnSettings(newValData.newSessionID, newValData.newColumnNum, newValData.newValue);
                    returnData.status = newValCheck.status;
                    returnData.errorMessage = newValCheck.errorMessage;

                    if (returnData.status) 
                    {
                        QualityContext.UpdateCellValueToSolveError(newValData.newSessionID, newValData.newRowNum, newValData.newColumnNum, newValCheck.searchForDuplicate, newValData.newValue);
                        returnData.counters = QualityContext.GetErrorCounters(newValData.newSessionID);
                    }
                }
                catch(Exception ex) 
                {
                    returnData.status = false;
                    returnData.errorMessage = ex.Message;
                }
            }
            else 
            {
                returnData.status = false;
                returnData.errorMessage = "No connection between server and website. Please, contact the Administrator.";
            }

            return returnData;
        }

        public AnalysisResultModel GetAnalysis(string sessionID) 
        {
            AnalysisResultModel returnData = new AnalysisResultModel();

            try
            {
                if (!string.IsNullOrEmpty(sessionID))
                {
                    returnData.analysis = QualityContext.GetDataForAnalysisTable(sessionID);
                    if (returnData.analysis != null && returnData.analysis.Any()) 
                    {
                        returnData.status = true;
                    }
                    else 
                    {
                        returnData.status = false;
                        returnData.errorMessage = "Unable to retrieve the data from provided file.";
                    }
                }
                else
                {
                    returnData.status = false;
                    returnData.errorMessage = "Before analysis please, upload your file.";
                }
            }
            catch(Exception ex) 
            {
                returnData.status = false;
                returnData.errorMessage = "Error during request process. Please, contact the Administrator.";
            }

            return returnData;
        }
    }
}
