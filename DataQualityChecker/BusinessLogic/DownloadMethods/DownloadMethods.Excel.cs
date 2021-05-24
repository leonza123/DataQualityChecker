using ClosedXML.Excel;
using DataQualityChecker.Data;
using DataQualityChecker.Helpers;
using DataQualityChecker.Models;
using System;
using System.IO;

namespace DataQualityChecker.BusinessLogic
{
    public partial class DownloadMethods
    {
        public ValidatedFileModel CreateExcelFileForDownload(string sessionID) 
        {
            ValidatedFileModel returnData = new ValidatedFileModel();
            returnData.fileData = new FileReturnModel();

            if (!string.IsNullOrEmpty(sessionID)) 
            {
                try 
                {
                    bool hasHeader = QualityContext.DocumentHasHeader(sessionID);
                    FileDataForDownload fileData = QualityContext.RetrieveDocumentData(sessionID);

                    var workbook = new XLWorkbook();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("ProcessedData");

                    if (hasHeader)
                    {
                        //Adding Headers to file
                        foreach (var header in fileData.headers)
                        {
                            worksheet.Cell(1, header.ColumnNum).Value = header.CellValue;
                        }
                    }

                    //Adding Cells to file
                    foreach(var rowData in fileData.cells) 
                    {
                        foreach(var cell in rowData) 
                        {
                            worksheet.Cell(cell.RowNum, cell.ColumnNum).Value = cell.CellValue;
                        }
                    }

                    //Writing file content to memory
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        returnData.fileData.fileContent = stream.ToArray();
                    }

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
