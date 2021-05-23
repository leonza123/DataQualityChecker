using System;
using System.Collections.Generic;
using System.Text;
using DataQualityChecker.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataQualityChecker.Constants;
using DataQualityChecker.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DataQualityChecker.Data
{
    public partial class QualityProjectDbContext : DbContext
    {
        public int CountAllCells(string sessionID)
        {
            return GetDocumentsStorageQueryForTableCells(sessionID).Count();
        }

        public void SaveCellsData(List<TableCell> cells)
        {
            TableCells.AddRange(cells);
            SaveChanges();
        }

        public bool CheckIfDuplicateExists(string sessionID, int columnNum, string searchableValue) 
        {
            return GetDocumentsStorageQueryForTableCells(sessionID)
                        .Where(z => z.ColumnNum == columnNum && z.CellValue == searchableValue)
                        .Any();
        }

        public void UpdateCellValueToSolveError(string sessionID, int rowNum, int columnNum, bool searchForDuplicate, string newValue)
        {
            int tableCellsID = TableCells.Where(x => DocumentsStorage.Where(x => x.SessionID == sessionID).Any() && DocumentsStorage.Where(x => x.SessionID == sessionID).Select(z => z.ID).FirstOrDefault() == x.DocumentID)
                                .Where(z => z.ColumnNum == columnNum && z.RowNum == rowNum)
                                .Select(z => z.ID).FirstOrDefault();
            TableCell cell = TableCells.Where(x => x.ID == tableCellsID)
                                .FirstOrDefault();

            if (cell != null)
            {
                if (searchForDuplicate) 
                {
                    List<int> cellsIDs = GetDocumentsStorageQueryForTableCells(sessionID)
                                                .Where(z => z.ColumnNum == columnNum && z.CellValue == cell.CellValue)
                                                .Select(x => x.ID)
                                                .ToList();

                    if (cellsIDs != null && cellsIDs.Any()) 
                    {
                        if (cellsIDs.Count == 2) 
                        {
                            List<ValidatedCell> validatedCells = ValidatedCells.Where(x => x.TableCellsID == cellsIDs[0] || x.TableCellsID == cellsIDs[1]).ToList();
                            ValidatedCells.RemoveRange(validatedCells);
                        }
                        else
                        {
                            ValidatedCell validatedCell = ValidatedCells.Where(x => x.TableCellsID == cell.ID).FirstOrDefault();
                            ValidatedCells.Remove(validatedCell);
                        }
                    }
                }

                cell.CellValue = newValue;

                SaveChanges();

                RemoveCellError(cell.ID);
            }
        }
    }
}
