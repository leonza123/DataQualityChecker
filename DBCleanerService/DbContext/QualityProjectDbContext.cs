using System;
using System.Collections.Generic;
using System.Text;
using DBCleanerService.DBModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DBCleanerService.Data
{
    public class QualityProjectDbContext : DbContext
    {
        public QualityProjectDbContext(DbContextOptions<QualityProjectDbContext> options)
            : base(options)
        {
        }

        public QualityProjectDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;user=root;Password=Lorio_123;Database=quality_project_db", new MySqlServerVersion(new Version(8, 0, 11)));
        }

        private DbSet<DocumentStorage> DocumentsStorage { get; set; }
        private DbSet<TableCell> TableCells { get; set; }
        private DbSet<HeaderCell> HeaderCells { get; set; }
        private DbSet<ValidatedCell> ValidatedCells { get; set; }
        private DbSet<ValueOption> ValueOptions { get; set; }

        public void RemoveOldData(int hoursPeriod) 
        {
            List<ValidatedCell> oldValidatedData = ValidatedCells.Where(x => TableCells.Where(z => z.ID == x.TableCellsID && DocumentsStorage.Where(y => y.ID == z.DocumentID && y.Modified.AddHours(hoursPeriod) < DateTime.Now).Any()).Any()).ToList();
            ValidatedCells.RemoveRange(oldValidatedData);
            List<ValueOption> oldValueOptions = ValueOptions.Where(x => HeaderCells.Where(z => z.ID == x.HeaderID && DocumentsStorage.Where(y => y.ID == z.DocumentID && y.Modified.AddHours(hoursPeriod) < DateTime.Now).Any()).Any()).ToList();
            ValueOptions.RemoveRange(oldValueOptions);
            SaveChanges();

            List<TableCell> oldTableCells = TableCells.Where(z => DocumentsStorage.Where(y => y.ID == z.DocumentID && y.Modified.AddHours(hoursPeriod) < DateTime.Now).Any()).ToList();
            TableCells.RemoveRange(oldTableCells);
            List<HeaderCell> oldHeaderCells = HeaderCells.Where(z => DocumentsStorage.Where(y => y.ID == z.DocumentID && y.Modified.AddHours(hoursPeriod) < DateTime.Now).Any()).ToList();
            HeaderCells.RemoveRange(oldHeaderCells);
            SaveChanges();

            List<DocumentStorage> oldDocumentsData = DocumentsStorage.Where(x => x.Modified.AddHours(hoursPeriod) < DateTime.Now).ToList();
            DocumentsStorage.RemoveRange(oldDocumentsData);
            SaveChanges();
        }
    }
}
