using DataQualityChecker.Constants;
using DataQualityChecker.Data;
using DataQualityChecker.DBModels;
using DataQualityChecker.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DataQualityChecker.Constants.Enums;

namespace DataQualityChecker.BusinessLogic
{
    public class DataProcessor
    {
        private QualityProjectDbContext QualityContext { get; set; }
        public DataProcessor(QualityProjectDbContext _context) 
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            QualityContext = _context;
        }
        public List<HeaderReturnModel> StoreFileData(Stream fileStram, int documentID, bool noHeader, int headerStarts, int rowsCount) 
        {
            List<HeaderReturnModel> headers = new List<HeaderReturnModel>();

            using (var reader = ExcelReaderFactory.CreateCsvReader(fileStram))
            {
                //Headers
                DataTable tableData = reader.AsDataSet().Tables[0];
                reader.Dispose();

                int rowIndex = 0;
                if (!noHeader && headerStarts != 1 && tableData.Rows.Count > headerStarts) 
                {
                    rowIndex = headerStarts - 1;
                }

                List<HeaderCell> headersDB = new List<HeaderCell>();
                for (int i = 0; i != tableData.Columns.Count; i++)
                {
                    var cellVal = tableData.Rows[rowIndex][i];

                    HeaderReturnModel data = new HeaderReturnModel
                    {
                        ColumnIndex = i + 1,
                        ColumnName = string.IsNullOrEmpty(cellVal.ToString()) || noHeader ? null : (cellVal.ToString().Length < 15000 ? cellVal.ToString() : cellVal.ToString().Substring(0, 15000)),
                    };
                    headers.Add(data);

                    //Store Headers into Additional Table
                    HeaderCell headerData = new HeaderCell
                    {
                        DocumentID = documentID,
                        ColumnNum = i + 1,
                        CellValue = data.ColumnName,
                    };
                    headersDB.Add(headerData);
                }
                QualityContext.SaveHeadersData(headersDB);

                if (!noHeader) 
                {
                    rowIndex++;
                }

                int tableRowCount = 0;
                if (rowsCount > 0)
                {
                    if (tableData.Rows.Count - rowIndex > rowsCount) tableRowCount = rowIndex + rowsCount;
                    else tableRowCount = tableData.Rows.Count;
                }
                else tableRowCount = tableData.Rows.Count;

                //Cells
                List<TableCell> cells = new List<TableCell>();
                for (int j = rowIndex; j != tableRowCount; j++)
                {
                    for (int i = 0; i != tableData.Columns.Count; i++)
                    {
                        var cellVal = tableData.Rows[j][i];

                        TableCell cellData = new TableCell
                        {
                            DocumentID = documentID,
                            RowNum = j + 1,
                            ColumnNum = i + 1,
                            CellValue = string.IsNullOrEmpty(cellVal.ToString()) ? null : (cellVal.ToString().Length < 15000 ? cellVal.ToString() : cellVal.ToString().Substring(0, 15000))
                        };

                        cells.Add(cellData);
                    }
                    QualityContext.SaveCellsData(cells);
                    cells = new List<TableCell>();
                }           
            }

            return headers;
        }

        public ValidatedDataOutput FileValidation(SettingsModel jsonData) 
        {
            ValidatedDataOutput returnData = new ValidatedDataOutput();

            try 
            {
                returnData.counters = new ValidatedDataCounters();
                returnData.counters.AllCells = QualityContext.CountAllCells(jsonData.docID);

                foreach(var item in jsonData.settings) 
                {
                    string format = string.IsNullOrEmpty(item.formatVal) ? null : (item.formatVal.Length <= 50 ? item.formatVal : item.formatVal.Substring(0, 50));
                    string ranges = null;
                    if (item.rangesVal != null && item.rangesVal.Any()) 
                    {
                        ranges = ";";
                        foreach(string rangeVal in item.rangesVal) 
                        {
                            ranges += rangeVal + ";";
                        }
                    }

                    QualityContext.UpdateHeaderData(jsonData.docID, item.colIDVal, item.columnTypeVal, item.allowNullVal, item.uniqueVal, format, ranges);

                    //check for Null values
                    if (!item.allowNullVal) 
                    {
                        returnData.counters.NullCells += QualityContext.CheckForNull(jsonData.docID, item.colIDVal);
                    }

                    //check for Unique values
                    if (item.uniqueVal) 
                    {
                        returnData.counters.DuplicateCells += QualityContext.CheckForDuplicates(jsonData.docID, item.colIDVal);
                    }

                    //check for types
                    returnData.counters.WrongTypeCells += QualityContext.CheckForType(jsonData.docID, item.colIDVal, item.columnTypeVal);

                    if (!string.IsNullOrEmpty(item.formatVal) && !string.IsNullOrEmpty(item.columnTypeVal)) 
                    {
                        if (item.columnTypeVal == TypeConstants.DateTime || item.columnTypeVal == TypeConstants.Time || item.columnTypeVal == TypeConstants.Text) 
                        {
                            returnData.counters.WrongFormatCells = QualityContext.CheckForFormat(jsonData.docID, item.colIDVal, item.columnTypeVal, format);
                        }
                    }

                    if (item.optionsVal != null && item.optionsVal.Any()) 
                    {
                        QualityContext.SaveOptions(item.optionsVal, jsonData.docID, item.colIDVal);
                        returnData.counters.WrongOptionsCells = QualityContext.CheckForOptions(jsonData.docID, item.colIDVal);
                    }

                    if (item.rangesVal != null && item.rangesVal.Any() && (item.columnTypeVal == TypeConstants.DateTime || item.columnTypeVal == TypeConstants.Time || item.columnTypeVal == TypeConstants.Integer || item.columnTypeVal == TypeConstants.Float))
                    {
                        returnData.counters.WrongRangesCells = QualityContext.CheckForRanges(jsonData.docID, item.colIDVal, item.columnTypeVal, item.rangesVal);
                    }
                }

                returnData.status = true;
            }
            catch(Exception ex) 
            {
                returnData.status = false;
                returnData.errorMessage = "Error during data validation process.";
            }

            return returnData;
        }
    }
}
