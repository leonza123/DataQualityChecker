using System;
using System.Collections.Generic;
using System.Text;
using DataQualityChecker.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataQualityChecker.Constants;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DataQualityChecker.Data
{
    public partial class QualityProjectDbContext : DbContext
    {
        public int CheckForNull(string sessionID, int columnNum) 
        {
            List<TableCell> emptyTableCells = GetDocumentsStorageQueryForTableCells(sessionID)
                                                    .Where(z => z.ColumnNum == columnNum && string.IsNullOrEmpty(z.CellValue))
                                                    .ToList();

            if (emptyTableCells != null && emptyTableCells.Any()) 
            {
                foreach(var item in emptyTableCells) 
                {
                    SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.Null);
                }
                return emptyTableCells.Count();
            }
            else 
            {
                return 0;
            }
        }

        public int CheckForDuplicates(string sessionID, int columnNum)
        {
            int duplicateCount = 0;

            List<string> duplicateValues = GetDocumentsStorageQueryForTableCells(sessionID)
                                                .Where(z => z.ColumnNum == columnNum)
                                                .GroupBy(i => i.CellValue)
                                                .Where(x => x.Count() > 1)
                                                .Select(z => z.Key)
                                                .ToList();

            foreach (string dupValue in duplicateValues) 
            {
                List<TableCell> duplicateCells = GetDocumentsStorageQueryForTableCells(sessionID)
                                                        .Where(z => z.ColumnNum == columnNum && z.CellValue == dupValue)
                                                        .ToList();

                if (duplicateCells != null && duplicateCells.Any())
                {
                    foreach (var item in duplicateCells)
                    {
                        SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.Duplicate);
                    }
                    duplicateCount += duplicateCells.Count();
                }
            }

            return duplicateCount;
        }

        public int CheckForType(string sessionID, int columnNum, string type) 
        {
            List<TableCell> cells = GetDocumentsStorageQueryForTableCells(sessionID)
                                        .Where(z => z.ColumnNum == columnNum)
                                        .ToList();

            int errorCount = 0;
            if (cells != null && cells.Any())
            {
                foreach (var item in cells)
                {
                    switch (type) 
                    {
                        case TypeConstants.Boolean:
                            try 
                            {
                                bool.Parse(item.CellValue);
                            }
                            catch (Exception ex) 
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, Enums.CellError.WrongTypeBoolean);
                                errorCount += 1;
                            }
                            break;
                        case TypeConstants.Integer:
                            try
                            {
                                int.Parse(item.CellValue);
                            }
                            catch (Exception ex)
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, Enums.CellError.WrongTypeInteger);
                                errorCount += 1;
                            }
                            break;
                        case TypeConstants.Float:
                            try
                            {
                                float.Parse(item.CellValue);
                            }
                            catch (Exception ex)
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, Enums.CellError.WrongTypeFloat);
                                errorCount += 1;
                            }
                            break;
                        case TypeConstants.DateTime:
                            try 
                            { 
                                DateTime.Parse(item.CellValue);
                            }
                            catch (Exception ex)
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, Enums.CellError.WrongTypeDateTime);
                                errorCount += 1;
                            }
                            break;
                        case TypeConstants.Time:
                            try
                            {
                                TimeSpan.Parse(item.CellValue);
                            }
                            catch (Exception ex)
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, Enums.CellError.WrongTypeTime);
                                errorCount += 1;
                            }
                            break;
                        case TypeConstants.Char:
                            try
                            {
                                TimeSpan.Parse(item.CellValue);
                            }
                            catch (Exception ex)
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, Enums.CellError.WrongTypeChar);
                                errorCount += 1;
                            }
                            break;
                    }
                }

                return errorCount;
            }
            else
            {
                return errorCount;
            }
        }

        public int CheckForFormat(string sessionID, int columnNum, string type, string format)
        {
            int counter = 0;

            List<TableCell> unformatedTableCells = GetDocumentsStorageQueryForTableCells(sessionID)
                                                        .Where(z => z.ColumnNum == columnNum)
                                                        .ToList();

            if (unformatedTableCells != null && unformatedTableCells.Any())
            {
                switch (type)
                {
                    case (TypeConstants.DateTime):
                        foreach (var item in unformatedTableCells)
                        {
                            if (string.IsNullOrEmpty(item.CellValue))
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.FormatDateTime);
                                counter++;
                            }
                            else
                            {
                                if (!DateTime.TryParseExact(item.CellValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                                {
                                    SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.FormatDateTime);
                                    counter++;
                                }
                            }
                        }
                        break;

                    case (TypeConstants.Time):
                        foreach (var item in unformatedTableCells)
                        {
                            if (string.IsNullOrEmpty(item.CellValue))
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.FormatDateTime);
                                counter++;
                            }
                            else
                            {
                                if (!DateTime.TryParseExact(item.CellValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                                {
                                    SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.FormatDateTime);
                                    counter++;
                                }
                            }
                        }
                        break;

                    case (TypeConstants.Text):
                        foreach (var item in unformatedTableCells)
                        {
                            if (string.IsNullOrEmpty(item.CellValue))
                            {
                                SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.FormatDateTime);
                                counter++;
                            }
                            else
                            {
                                if (!Regex.IsMatch(item.CellValue, format))
                                {
                                    SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.FormatText);
                                    counter++;
                                }
                            }
                        }
                        break;
                }
            }

            return counter;
        }

        public int CheckForOptions(string sessionID, int columnNum) 
        {
            int counter = 0;

            var outOfOptions = GetDocumentsStorageQueryForTableCells(sessionID)
                                    .Where(x => x.ColumnNum == columnNum)
                                    .Where(c => !GetDocumentsStorageQueryForValueOptions(sessionID)
                                                    .Where(x => x.ColumnNum == columnNum)
                                                    .Select(x => x.OptionVal)
                                                    .Contains(c.CellValue)
                                    ).ToList();

            if (outOfOptions != null && outOfOptions.Any()) 
            {
                counter = outOfOptions.Count();

                foreach (var item in outOfOptions) 
                {
                    SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.WrongOptions);
                }
            }

            return counter;
        }


        public int CheckForRanges(string sessionID, int columnNum, string type, List<string> ranges) 
        {
            int counter = 0;

            var forCheckRanges = GetDocumentsStorageQueryForTableCells(sessionID)
                                    .Where(x => x.ColumnNum == columnNum)
                                    .ToList();

            switch (type) 
            {
                case (TypeConstants.DateTime):
                    foreach (string range in ranges) 
                    {
                        try
                        {
                            DateTime minVal = DateTime.Parse(range.Split(" - ")[0]);
                            DateTime maxVal = DateTime.Parse(range.Split(" - ")[1]);
                            forCheckRanges = forCheckRanges.Where(x => DateTime.Parse(x.CellValue) < minVal || DateTime.Parse(x.CellValue) > maxVal).ToList();
                        }
                        catch(Exception ex) { }
                    }
                    break;

                case (TypeConstants.Time):
                    foreach (string range in ranges)
                    {
                        try
                        {
                            DateTime minVal = DateTime.Parse(range.Split(" - ")[0]);
                            DateTime maxVal = DateTime.Parse(range.Split(" - ")[1]);
                            forCheckRanges = forCheckRanges.Where(x => DateTime.Parse(x.CellValue) < minVal || DateTime.Parse(x.CellValue) > maxVal).ToList();
                        }
                        catch (Exception ex) { }
                    }
                    break;

                case (TypeConstants.Integer):
                    foreach (string range in ranges)
                    {
                        try
                        {
                            float minVal = float.Parse(range.Split(" - ")[0]);
                            float maxVal = float.Parse(range.Split(" - ")[1]);
                            forCheckRanges = forCheckRanges.Where(x => int.Parse(x.CellValue) < minVal || int.Parse(x.CellValue) > maxVal).ToList();
                        }
                        catch (Exception ex) { }
                    }
                    break;

                case (TypeConstants.Float):
                    foreach (string range in ranges)
                    {
                        try
                        {
                            float minVal = float.Parse(range.Split(" - ")[0]);
                            float maxVal = float.Parse(range.Split(" - ")[1]);
                            forCheckRanges = forCheckRanges.Where(x => float.Parse(x.CellValue) < minVal || float.Parse(x.CellValue) > maxVal).ToList();
                        }
                        catch (Exception ex) { }
                    }
                    break;
            }

            if (forCheckRanges != null && forCheckRanges.Any())
            {
                foreach (var item in forCheckRanges)
                {
                    SaveCellError(sessionID, item.ColumnNum, item.RowNum, (int)Enums.CellError.WrongRanges);
                    counter++;
                }
            }

            return counter;
        }
    }
}
