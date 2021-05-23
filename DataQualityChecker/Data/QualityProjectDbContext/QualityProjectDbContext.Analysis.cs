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
        public List<AnalysisModel> GetDataForAnalysisTable(string sessionID)
        {
            List<HeaderCell> headers = GetDocumentsStorageQueryForHeaderCells(sessionID)
                                            .ToList();

            var cellsWithErrors = GetDocumentsStorageQueryForTableCells(sessionID)
                                    .GroupJoin(
                                        ValidatedCells,
                                        cells => cells.ID,
                                        errors => errors.TableCellsID,
                                        (cells, errors) => new { cells = cells, errors = errors })
                                    .SelectMany(
                                        x => x.errors.DefaultIfEmpty(),
                                        (x, y) => new DataForAnalysisModel
                                        {
                                            ColumnNum = x.cells.ColumnNum,
                                            RowNum = x.cells.RowNum,
                                            CellValue = x.cells.CellValue,
                                            ErrorCode = y.ErrorCode,
                                        });

            List<AnalysisModel> analysis = new List<AnalysisModel>();
            foreach (var item in headers) 
            {
                AnalysisModel itemAnalys = new AnalysisModel
                {
                    title = item.CellValue,
                    type = item.Type,
                    validCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum && x.ErrorCode == null).Count(),
                    inValidCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum && x.ErrorCode != null).Select(z => z.RowNum).Distinct().Count(),
                    nullCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum && string.IsNullOrEmpty(x.CellValue)).Select(z => z.RowNum).Distinct().Count(),
                    notNullCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum && !string.IsNullOrEmpty(x.CellValue)).Select(z => z.RowNum).Distinct().Count(),
                    uniqueCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum).Select(z => z.CellValue).GroupBy(z => z).Where(x => x.Count() == 1).Count(),
                    distinctCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum).Select(z => z.CellValue).Distinct().Count(),
                    specialCharsCount = cellsWithErrors.Where(x => x.ColumnNum == item.ColumnNum && !string.IsNullOrEmpty(x.CellValue)).ToList().Where(x => x.CellValue.Where(z => CharsContants.SpecialChars.Contains(z)).Any()).Select(z => z.RowNum).Distinct().Count(),
                };
                analysis.Add(itemAnalys);
            }

            return analysis;
        }
    }
}
