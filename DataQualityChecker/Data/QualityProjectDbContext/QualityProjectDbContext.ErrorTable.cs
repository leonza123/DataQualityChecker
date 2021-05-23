using System;
using System.Collections.Generic;
using System.Text;
using DataQualityChecker.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataQualityChecker.Constants;
using DataQualityChecker.Models;

namespace DataQualityChecker.Data
{
    public partial class QualityProjectDbContext : DbContext
    {
        public List<List<string>> GetCellsDataForErrorTable(string sessionID, int start, int length, int errorType) 
        {
            List<ValidatedCellsReturnModel> cellsWithErrors = GetDocumentsStorageQueryForValidatedCells(sessionID)
                                                                        .OrderBy(x => x.ColumnNum)
                                                                        .ToList();

            List<List<string>> cellsForTable = new List<List<string>>();

            if (cellsWithErrors != null) 
            {
                List<int> rowsWithErrors = cellsWithErrors.Select(x => x.RowNum)
                                                            .OrderBy(z => z)
                                                            .Distinct()
                                                            .Skip(start)
                                                            .Take(length)
                                                            .ToList();


                foreach (var item in rowsWithErrors) 
                {
                    List<string> cellsData = GetDocumentsStorageQueryForTableCells(sessionID)
                                                .Where(z => z.RowNum == item)
                                                .OrderBy(z => z.ColumnNum)
                                                .Select(z => z.CellValue)
                                                .ToList();

                    cellsData.Add(item.ToString());

                    //Add column errors
                    string errorColumnsInString = ";";
                    List<ErrorCellModelForTable> errorColumns = cellsWithErrors.Where(x => x.RowNum == item)
                                                            .Select(z => new ErrorCellModelForTable 
                                                            {
                                                                ColumnNum = z.ColumnNum,
                                                                ErrorCode = z.ErrorCode
                                                            })
                                                            .ToList();


                    foreach (var error in errorColumns) 
                    {
                        errorColumnsInString += error.ColumnNum.ToString() + "|" + error.ErrorCode.ToString() + ";";
                    } 

                    cellsData.Add(errorColumnsInString);

                    cellsForTable.Add(cellsData);
                }
            }

            return cellsForTable;
        }

        public List<string> GetHeadersForTable(string sessionID) 
        {
            return GetDocumentsStorageQueryForHeaderCells(sessionID)
                                .OrderBy(z => z.ColumnNum)
                                .Select(z => z.CellValue)
                                .ToList();
        }

        public int GetTotalCountOfRowsWithErrors(string sessionID) 
        {
            return GetDocumentsStorageQueryForValidatedCells(sessionID)
                        .OrderBy(x => x.ColumnNum)
                        .Select(x => x.RowNum).OrderBy(z => z)
                        .Distinct()
                        .Count();
        }
    }
}
