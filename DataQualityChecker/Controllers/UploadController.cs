using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataQualityChecker.BusinessLogic;
using DataQualityChecker.Constants;
using DataQualityChecker.Data;
using DataQualityChecker.Helpers;
using DataQualityChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataQualityChecker.Controllers
{
    public class UploadController : Controller
    {
        private ConfigurationModel config { get; set; }
        private QualityProjectDbContext context { get; set; }
        private UploadMethods Methods { get; set; }
        private DownloadMethods DownloadMethods { get; set; }
        public UploadController(ConfigurationModel _config, QualityProjectDbContext _context)
        {
            config = _config;
            context = _context;
            Methods = new UploadMethods(_config, _context);
            DownloadMethods = new DownloadMethods(_config, _context);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [Route("/api/uploadfile")]
        public string UploadFile(IFormFile file, bool noHeader, int headerStarts, int rowsCount)
        { 
            return JsonConvert.SerializeObject(Methods.CheckProvidedFile(file, noHeader, headerStarts, rowsCount));
        }

        [HttpPost]
        [Route("/api/uploadurl")]
        public string UploadUrl([FromBody] FileURLModel urlData)
        {
            return JsonConvert.SerializeObject(Methods.CheckProvidedURL(urlData));
        }

        //APIs
        [HttpPost]
        [Route("api/validatefile")]
        public string ValidateFile([FromBody] SettingsModel jsonData)
        {
            context.UpdateDocumentsModifyDate(jsonData.docID);
            return JsonConvert.SerializeObject(Methods.ValidateDataUsingSettings(jsonData));
        }

        [HttpPost]
        [Route("api/errorstable")]
        public string GetDataForErrorsTable(SearchRequest request, string sessionID, int errorType) 
        {
            context.UpdateDocumentsModifyDate(sessionID);
            return Methods.GetDataForCellErrorTable(request, sessionID, errorType);
        }

        [HttpGet]
        [Route("api/gettableheaders")]
        public string GetTableHeaders(string sessionID)
        {
            context.UpdateDocumentsModifyDate(sessionID);
            return JsonConvert.SerializeObject(Methods.GetTableHeaders(sessionID));
        }

        [HttpPost]
        [Route("api/fixerror")]
        public string FixError([FromBody] NewElementValueModel newValData)
        {
            context.UpdateDocumentsModifyDate(newValData.newSessionID);
            return JsonConvert.SerializeObject(Methods.FixError(newValData));
        }

        [HttpGet]
        [Route("api/getanalysis")]
        public string GetAnalysis(string sessionID) 
        {
            context.UpdateDocumentsModifyDate(sessionID);
            return JsonConvert.SerializeObject(Methods.GetAnalysis(sessionID));
        }

        [HttpGet]
        [Route("api/getvalidation")]
        public string GetValidation(string sessionID)
        {
            context.UpdateDocumentsModifyDate(sessionID);
            return JsonConvert.SerializeObject(Methods.GetValidation(sessionID));
        }

        [HttpGet]
        [Route("api/getcounters")]
        public string GetCounters(string sessionID)
        {
            context.UpdateDocumentsModifyDate(sessionID);
            return JsonConvert.SerializeObject(Methods.GetCounters(sessionID));
        }


        [HttpGet]
        [Route("api/removerow")]
        public string RemoveRow(string sessionID, int rowID)
        {
            context.UpdateDocumentsModifyDate(sessionID);
            return JsonConvert.SerializeObject(Methods.RemoveRow(sessionID, rowID));
        }


        // api/downloadfile?sessionID=9d418ac457f0
        [HttpGet]
        [Route("api/downloadfile")]
        public IActionResult DownloadFile(string sessionID)
        {
            context.UpdateDocumentsModifyDate(sessionID);
            try
            {
                string documentName = context.GetDocumentData(sessionID);
                ValidatedFileModel returnData = new ValidatedFileModel();

                if (!string.IsNullOrEmpty(documentName))
                {
                    string extension = documentName.Split('.').Last().ToLower();
                    switch (extension)
                    {
                        case (FileConstants.CSV):
                            ValidatedCSVFileModel csvModel = DownloadMethods.CreateCSVFileForDownload(sessionID);
                            if (csvModel.status)
                            {
                                return new FileHelper.Utf8ForExcelCsvResult()
                                {
                                    Content = csvModel.fileData.fileContent,
                                    ContentType = csvModel.fileData.contentType,
                                    FileName = csvModel.fileData.fileName,
                                };
                            }
                            else return BadRequest();

                        case (FileConstants.XLS):
                            ValidatedFileModel xlsModel = DownloadMethods.CreateExcelFileForDownload(sessionID);
                            if (xlsModel.status) return File(xlsModel.fileData.fileContent, FileHelper.GetContentType(xlsModel.fileData.contentType), xlsModel.fileData.fileName);
                            else return BadRequest();

                        case (FileConstants.XLSX):
                            ValidatedFileModel xlsxModel = DownloadMethods.CreateExcelFileForDownload(sessionID);
                            if (xlsxModel.status) return File(xlsxModel.fileData.fileContent, FileHelper.GetContentType(xlsxModel.fileData.contentType), xlsxModel.fileData.fileName);
                            else return BadRequest();

                        default:
                            return BadRequest();
                    }
                }
                else 
                {
                    return BadRequest();
                }
            }
            catch(Exception ex) 
            {
                return BadRequest();
            }
        }
    }
}
