using DataQualityChecker.DBModels;
using DataQualityChecker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Data
{
    public partial class QualityProjectDbContext : DbContext
    {
        public IQueryable<TableCell> GetDocumentsStorageQueryForTableCells(string sessionID) 
        {
            return DocumentsStorage
                        .Where(x => x.SessionID == sessionID)
                        .Join(
                            TableCells,
                            docStorage => docStorage.ID,
                            cells => cells.DocumentID,
                            (docStorage, cells) => new TableCell
                            {
                                ID = cells.ID,
                                RowNum = cells.RowNum,
                                ColumnNum = cells.ColumnNum,
                                DocumentID = cells.DocumentID,
                                CellValue = cells.CellValue
                            }
                        );
        }

        public IQueryable<HeaderCell> GetDocumentsStorageQueryForHeaderCells(string sessionID)
        {
            return DocumentsStorage
                        .Where(x => x.SessionID == sessionID)
                        .Join(
                            HeaderCells,
                            docStorage => docStorage.ID,
                            cells => cells.DocumentID,
                            (docStorage, cells) => new HeaderCell
                            {
                                ID = cells.ID,
                                ColumnNum = cells.ColumnNum,
                                DocumentID = cells.DocumentID,
                                CellValue = cells.CellValue,
                                Format = cells.Format,
                                Nullable = cells.Nullable,
                                Type = cells.Type,
                                Unique = cells.Unique,
                                HasOptions = cells.HasOptions,
                                Ranges = cells.Ranges,
                            }
                        );
        }

        public IQueryable<ValidatedCellsReturnModel> GetDocumentsStorageQueryForValidatedCells(string sessionID)
        {
            return DocumentsStorage
                        .Where(x => x.SessionID == sessionID)
                        .Join(
                            TableCells,
                            docStorage => docStorage.ID,
                            cells => cells.DocumentID,
                            (docStorage, cells) => new TableCell
                            {
                                ID = cells.ID,
                                RowNum = cells.RowNum,
                                ColumnNum = cells.ColumnNum,
                                DocumentID = cells.DocumentID,
                                CellValue = cells.CellValue
                            }
                        )
                        .Join(
                            ValidatedCells,
                            cells => cells.ID,
                            validatedData => validatedData.TableCellsID,
                            (cells, validatedData) => new ValidatedCellsReturnModel
                            {
                                ID = cells.ID,
                                RowNum = cells.RowNum,
                                ColumnNum = cells.ColumnNum,
                                TableCellsID = cells.ID,
                                ErrorCode = validatedData.ErrorCode,
                            }
                        );
        }

        public IQueryable<ValueOptionReturnModel> GetDocumentsStorageQueryForValueOptions(string sessionID)
        {
            return DocumentsStorage
                        .Where(x => x.SessionID == sessionID)
                        .Join(
                            HeaderCells,
                            docStorage => docStorage.ID,
                            cells => cells.DocumentID,
                            (docStorage, cells) => new HeaderCell
                            {
                                ID = cells.ID,
                                ColumnNum = cells.ColumnNum,
                            }
                        )
                        .Join(
                            ValueOptions,
                            headers => headers.ID,
                            options => options.HeaderID,
                            (headers, options) => new ValueOptionReturnModel
                            {
                                ID = options.ID,
                                HeaderID = options.HeaderID,
                                ColumnNum = headers.ColumnNum,
                                OptionVal = options.OptionVal,
                            }
                        );
        }
    }
}
