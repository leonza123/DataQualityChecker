using ClosedXML.Excel;
using DataQualityChecker.Data;
using DataQualityChecker.Helpers;
using DataQualityChecker.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DataQualityChecker.BusinessLogic
{
    public partial class DownloadMethods
    {
        public ValidatedCSVFileModel CreateCSVFileForDownload(string sessionID) 
        {
            ValidatedCSVFileModel returnData = new ValidatedCSVFileModel();
            returnData.fileData = new CSVFileReturnModel();

            if (!string.IsNullOrEmpty(sessionID)) 
            {
                try 
                {
                    bool hasHeader = QualityContext.DocumentHasHeader(sessionID);
                    FileDataForDownload fileData = QualityContext.RetrieveDocumentData(sessionID);

                    StringBuilder stringBuilder = new StringBuilder();
                    string csvRow = "";

                    if (hasHeader)
                    {
                        //Sort headers
                        fileData.headers = fileData.headers.OrderBy(x => x.ColumnNum).ToList();

                        //Adding Headers to file
                        foreach (var header in fileData.headers)
                        {
                            csvRow += header.CellValue + ",";
                        }
                        csvRow = csvRow.Remove(csvRow.Length - 1);
                        stringBuilder.AppendLine(csvRow);
                    }

                    //Adding Cells to file
                    foreach (var rowData in fileData.cells) 
                    {
                        csvRow = "";
                        foreach (var cell in rowData) 
                        {
                            csvRow += cell.CellValue + ",";
                        }
                        csvRow = csvRow.Remove(csvRow.Length - 1);
                        stringBuilder.AppendLine(csvRow);
                    }

                    returnData.fileData.fileContent = stringBuilder.ToString();
                    returnData.fileData.fileName = fileData.documentData.Name;
                    returnData.fileData.contentType = FileHelper.GetContentType(fileData.documentData.Name);
                    returnData.status = true;
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
                returnData.errorMessage = "Null sessionID provided.";
            }

            return returnData;
        }
    }
}
