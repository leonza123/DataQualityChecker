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
        public void UpdateDocumentsModifyDate(string sessionID) 
        {
            DocumentStorage document = DocumentsStorage.Where(x => x.SessionID == sessionID).FirstOrDefault();
            if (document != null) 
            {
                document.Modified = DateTime.Now;
                SaveChanges();
            }
        }

        public string GetDocumentData(string sessionID) 
        {
            return DocumentsStorage.Where(x => x.SessionID == sessionID).Select(z => z.Name).FirstOrDefault();
        }

        public int GetDocumentStorageIDBySessionID(string sessionID) 
        {
            return DocumentsStorage.Where(x => x.SessionID == sessionID).Select(z => z.ID).FirstOrDefault();
        }
        public int SaveNewDocument(string sessionID, string name, bool hasHeader)
        {
            DocumentStorage storeDocument = new DocumentStorage();
            storeDocument.Modified = DateTime.Now;
            storeDocument.Name = name;
            storeDocument.SessionID = sessionID;
            storeDocument.HasHeader = hasHeader;
            DocumentsStorage.Add(storeDocument);
            SaveChanges();

            return storeDocument.ID;
        }

        public FileDataForDownload RetrieveDocumentData(string sessionID) 
        {
            FileDataForDownload returnData = new FileDataForDownload();

            returnData.documentData = DocumentsStorage.Where(x => x.SessionID == sessionID)
                                                        .Select(z => new DocumentReturnData 
                                                        { 
                                                            Name = z.Name
                                                        })
                                                        .FirstOrDefault();

            returnData.headers = GetDocumentsStorageQueryForHeaderCells(sessionID)
                                                    .Select(z => new HeaderReturnData
                                                    {
                                                        CellValue = z.CellValue,
                                                        ColumnNum = z.ColumnNum
                                                    })
                                                    .OrderBy(x => x.ColumnNum)
                                                    .ToList();

            List<CellReturnData> unsortedCells = GetDocumentsStorageQueryForTableCells(sessionID)
                                                        .Select(x => new CellReturnData
                                                        {
                                                            CellValue = x.CellValue,
                                                            ColumnNum = x.ColumnNum,
                                                            RowNum = x.RowNum
                                                        })
                                                        .OrderBy(x => x.ColumnNum)
                                                        .ToList();

            returnData.cells = unsortedCells.OrderBy(x => x.RowNum).GroupBy(item => item.RowNum)
                                            .Select(group => group.OrderBy(z => z.ColumnNum).ToList())
                                            .ToList();

            return returnData;
        }
    }
}
