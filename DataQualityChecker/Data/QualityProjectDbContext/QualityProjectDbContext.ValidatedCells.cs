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
        public ValidatedDataCounters GetErrorCounters(string sessionID) 
        {
            var allCellQuery = GetDocumentsStorageQueryForTableCells(sessionID);
            var errorCellCount = GetDocumentsStorageQueryForValidatedCells(sessionID);
            var nullErrorCount = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.ErrorCode == Enums.CellError.Null);
            var duplicateErrorCount = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.ErrorCode == Enums.CellError.Duplicate);
            var wrongTypeErrorCount = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.ErrorCode.ToString().StartsWith(Enums.CellError.WrongType.ToString() + "."));
            var formatErrorCount = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.ErrorCode.ToString().StartsWith(Enums.CellError.WrongFormat.ToString() + "."));
            var optionsErrorCount = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.ErrorCode == Enums.CellError.WrongOptions);
            var rangesErrorCount = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.ErrorCode == Enums.CellError.WrongRanges);

            return TableCells
                    .Take(1)
                    .Select(x => new ValidatedDataCounters
                    {
                        AllCells = allCellQuery.Count(),
                        ErrorCells = errorCellCount.Count(),
                        NullCells = nullErrorCount.Count(),
                        DuplicateCells = duplicateErrorCount.Count(),
                        WrongTypeCells = wrongTypeErrorCount.Count(),
                        WrongFormatCells = formatErrorCount.Count(),
                        WrongOptionsCells = optionsErrorCount.Count(),
                        WrongRangesCells = rangesErrorCount.Count(),
                    })
                    .FirstOrDefault();
        }

        public void SaveCellError(string sessionID, int colIndex, int rowIndex, double errorCode)
        {
            int errorID = GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.RowNum == rowIndex && x.ColumnNum == colIndex && x.ErrorCode == errorCode).Select(z => z.ID).FirstOrDefault();
            
            if (!GetDocumentsStorageQueryForValidatedCells(sessionID).Where(x => x.RowNum == rowIndex && x.ColumnNum == colIndex && x.ErrorCode == errorCode).Select(z => z.ID).Any()) 
            {
                int cellID = GetDocumentsStorageQueryForTableCells(sessionID).Where(x => x.RowNum == rowIndex && x.ColumnNum == colIndex).Select(z => z.ID).FirstOrDefault();

                if (cellID != 0) 
                {
                    ValidatedCell data = new ValidatedCell
                    {
                        TableCellsID = cellID,
                        ErrorCode = errorCode,
                    };

                    ValidatedCells.Add(data);
                    SaveChanges();
                }

            }
        }

        public void RemoveCellError(int recordID)    
        {
            List<ValidatedCell> cellErrors = ValidatedCells.Where(x => x.TableCellsID == recordID).ToList();
            if (cellErrors != null)
            {
                ValidatedCells.RemoveRange(cellErrors);
                SaveChanges();
            }
        }

        public void RemoveRowData(int rowID, string sessionID) 
        {
            List<ValidatedCell> cellErrors = ValidatedCells.Where(x => TableCells.Where(z => x.TableCellsID == z.ID && z.RowNum == rowID && DocumentsStorage.Where(c => c.ID == z.DocumentID && c.SessionID == sessionID).Any()).Any()).ToList();
            ValidatedCells.RemoveRange(cellErrors);
            SaveChanges();

            List<TableCell> cells = TableCells.Where(z => z.RowNum == rowID && DocumentsStorage.Where(c => c.ID == z.DocumentID && c.SessionID == sessionID).Any()).ToList();
            TableCells.RemoveRange(cells);
            SaveChanges();
        }
    }
}
