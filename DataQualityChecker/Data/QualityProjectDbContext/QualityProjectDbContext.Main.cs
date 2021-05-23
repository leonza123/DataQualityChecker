using System;
using System.Collections.Generic;
using System.Text;
using DataQualityChecker.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataQualityChecker.Constants;

namespace DataQualityChecker.Data
{
    public partial class QualityProjectDbContext : DbContext
    {
        public QualityProjectDbContext(DbContextOptions<QualityProjectDbContext> options)
            : base(options)
        {
        }

        public QualityProjectDbContext()
        {
        }

        private DbSet<DocumentStorage> DocumentsStorage { get; set; }
        private DbSet<TableCell> TableCells { get; set; }
        private DbSet<HeaderCell> HeaderCells { get; set; }
        private DbSet<ValidatedCell> ValidatedCells { get; set; }
        private DbSet<ValueOption> ValueOptions { get; set; }
    }
}
