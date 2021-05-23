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
        public void SaveOptions(List<string> options, string sessionID, int columnNum) 
        {
            var header = HeaderCells.Where(x => x.ColumnNum == columnNum && DocumentsStorage.Where(z => z.SessionID == sessionID).Any() && DocumentsStorage.Where(z => z.SessionID == sessionID).Select(z => z.ID).FirstOrDefault() == x.DocumentID).FirstOrDefault();
            if (header != null) 
            {
                header.HasOptions = true;

                List<ValueOption> NewValueOptions = new List<ValueOption>();
                foreach (string option in options) 
                {
                    ValueOption valOption = new ValueOption
                    {
                        HeaderID = header.ID,
                        OptionVal = option,
                    };
                    NewValueOptions.Add(valOption);
                }
                ValueOptions.AddRange(NewValueOptions);

                SaveChanges();
            }
        }
    }
}
