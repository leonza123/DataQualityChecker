using DataQualityChecker.Constants;
using DataQualityChecker.Data;
using DataQualityChecker.Models;
using System.Linq;
using static DataQualityChecker.Helpers.FileHelper;

namespace DataQualityChecker.BusinessLogic
{
    public partial class DownloadMethods
    {
        private ConfigurationModel configurations { get; set; }
        private DataProcessor DataProcessor { get; set; }
        private QualityProjectDbContext QualityContext { get; set; }
        public DownloadMethods(ConfigurationModel _configs, QualityProjectDbContext _context) 
        {
            configurations = _configs;
            QualityContext = _context;
            DataProcessor = new DataProcessor(QualityContext);
        }
    }
}
