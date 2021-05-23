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
        public List<TableSettingsModel> GetHeaderDataForExistingDocument(string sessionID) 
        {
            var headerValue = GetDocumentsStorageQueryForHeaderCells(sessionID)
                                .Select(x => new
                                {
                                    colIDVal = x.ColumnNum,
                                    columnTypeVal = x.Type,
                                    allowNullVal = x.Nullable,
                                    formatVal = x.Format,
                                    uniqueVal = x.Unique,
                                    rangesVal = x.Ranges,
                                    optionsVal = x.HasOptions ? GetDocumentsStorageQueryForValueOptions(sessionID).Where(z => z.ColumnNum == x.ColumnNum).Select(z => z.OptionVal).ToList() : new List<string>(),
                                }).ToList();

            return headerValue.Select(x => new TableSettingsModel
                    {
                        colIDVal = x.colIDVal,
                        columnTypeVal = x.columnTypeVal,
                        allowNullVal = x.allowNullVal,
                        formatVal = x.formatVal,
                        uniqueVal = x.uniqueVal,
                        rangesVal = !string.IsNullOrEmpty(x.rangesVal) ? x.rangesVal.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
                        optionsVal = x.optionsVal,
                    }).ToList();
        }

        public void SaveHeadersData(List<HeaderCell> cells)
        {
            HeaderCells.AddRange(cells);
            SaveChanges();
        }

        public void UpdateHeaderData(string sessionID, int columnNum, string type, bool nullable, bool unique, string format, string ranges)
        {
            HeaderCell header = HeaderCells
                                    .Where(x => DocumentsStorage.Where(z => z.SessionID == sessionID).Any() && DocumentsStorage.Where(z => z.SessionID == sessionID).FirstOrDefault().ID == x.DocumentID && x.ColumnNum == columnNum)
                                    .FirstOrDefault();

            header.Type = type;
            header.Nullable = nullable;
            header.Unique = unique;
            header.Format = format;
            header.Ranges = ranges;

            SaveChanges();
        }

        public NewValueCheckReturnModel CheckValueForColumnSettings(string sessionID, int columnNum, string searchableValue) 
        {
            NewValueCheckReturnModel returnData = new NewValueCheckReturnModel();
            returnData.status = true;
            returnData.searchForDuplicate = false;

            HeaderCell header = GetDocumentsStorageQueryForHeaderCells(sessionID)
                                    .Where(x => x.ColumnNum == columnNum).FirstOrDefault();
            if (header != null)
            {
                //Check for type
                switch (header.Type)
                {
                    case (TypeConstants.Text):
                        break;
                    case (TypeConstants.Boolean):
                        try
                        {
                            bool.Parse(searchableValue);
                        }
                        catch (Exception ex)
                        {
                            returnData.status = false;
                            returnData.errorMessage = "Unable to convert to Boolean.";
                        }
                        break;
                    case (TypeConstants.Integer):
                        try
                        {
                            int.Parse(searchableValue);
                        }
                        catch (Exception ex)
                        {
                            returnData.status = false;
                            returnData.errorMessage = "Unable to convert to Integer.";
                        }
                        break;
                    case (TypeConstants.Float):
                        try
                        {
                            float.Parse(searchableValue);
                        }
                        catch (Exception ex)
                        {
                            returnData.status = false;
                            returnData.errorMessage = "Unable to convert to Float.";
                        }
                        break;
                    case (TypeConstants.DateTime):
                        try
                        {
                            DateTime.Parse(searchableValue);
                        }
                        catch (Exception ex)
                        {
                            returnData.status = false;
                            returnData.errorMessage = "Unable to convert to DateTime.";
                        }
                        break;
                    case (TypeConstants.Time):
                        try
                        {
                            TimeSpan.Parse(searchableValue);
                        }
                        catch (Exception ex)
                        {
                            returnData.status = false;
                            returnData.errorMessage = "Unable to convert to Time.";
                        }
                        break;
                    case (TypeConstants.Char):
                        try
                        {
                            Char.Parse(searchableValue);
                        }
                        catch (Exception ex)
                        {
                            returnData.status = false;
                            returnData.errorMessage = "Unable to convert to Char.";
                        }
                        break;
                    default:
                        returnData.status = false;
                        returnData.errorMessage = "Unrecognized provided data type.";
                        break;
                }

                //Check for specific value
                if (!header.Nullable)
                {
                    if (string.IsNullOrEmpty(searchableValue))
                    {
                        returnData.status = false;
                        if (string.IsNullOrEmpty(returnData.errorMessage)) returnData.errorMessage = "New element data cannot be null.";
                        else returnData.errorMessage += "\nNew element data cannot be null.";
                    }
                }

                if (header.Unique)
                {
                    returnData.searchForDuplicate = true;
                
                    if (GetDocumentsStorageQueryForTableCells(sessionID).Where(x => x.CellValue == searchableValue).Any())
                    {
                        returnData.status = false;
                        if (string.IsNullOrEmpty(returnData.errorMessage)) returnData.errorMessage = "Provided data has dublicates.";
                        else returnData.errorMessage += "\nProvided data has dublicates.";
                    }
                }

                if (!string.IsNullOrEmpty(header.Format))
                {
                    switch (header.Type)
                    {
                        case (TypeConstants.DateTime):
                            if (!DateTime.TryParseExact(searchableValue, header.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                            {
                                returnData.status = false;
                                if (string.IsNullOrEmpty(returnData.errorMessage)) returnData.errorMessage = "Unable to convert the provided data using provided format.";
                                else returnData.errorMessage += "\nUnable to convert the provided data using provided format.";
                            }
                            break;

                        case (TypeConstants.Time):
                            if (!DateTime.TryParseExact(searchableValue, header.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newTime))
                            {
                                returnData.status = false;
                                if (string.IsNullOrEmpty(returnData.errorMessage)) returnData.errorMessage = "Unable to convert the provided data using provided format.";
                                else returnData.errorMessage += "\nUnable to convert the provided data using provided format.";
                            }
                            break;

                        case (TypeConstants.Text):
                            if (!Regex.IsMatch(searchableValue, "^" + header.Format + "$"))
                            {
                                returnData.status = false;
                                if (string.IsNullOrEmpty(returnData.errorMessage)) returnData.errorMessage = "Unable to convert the provided data using provided format.";
                                else returnData.errorMessage += "\nUnable to convert the provided data using provided format.";
                            }
                            break;
                    }
                }

                if (header.HasOptions) 
                {
                    if (!GetDocumentsStorageQueryForValueOptions(sessionID).Where(x => x.OptionVal == searchableValue).Any()) 
                    {
                        returnData.status = false;
                        if (string.IsNullOrEmpty(returnData.errorMessage)) returnData.errorMessage = "Unable to find the provided value in possible options list.";
                        else returnData.errorMessage += "\nUnable to find the provided value in possible options list.";
                    }
                }
            }
            else 
            {
                returnData.status = false;
                returnData.errorMessage = "Error during data retrieve from DB. Please, contact the Administrator.";
            }

            return returnData;
        }
    }
}
