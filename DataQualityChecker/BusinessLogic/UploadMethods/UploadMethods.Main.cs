using DataQualityChecker.Data;
using DataQualityChecker.Models;

namespace DataQualityChecker.BusinessLogic
{
    public partial class UploadMethods
    {
        private ConfigurationModel configurations { get; set; }
        private DataProcessor DataProcessor { get; set; }
        private QualityProjectDbContext QualityContext { get; set; }
        public UploadMethods(ConfigurationModel _configs, QualityProjectDbContext _context) 
        {
            configurations = _configs;
            QualityContext = _context;
            DataProcessor = new DataProcessor(QualityContext);
        }
    }
}
