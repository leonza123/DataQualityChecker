using DataQualityChecker.Constants;
using DataQualityChecker.Data;
using DataQualityChecker.Helpers;
using DataQualityChecker.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DataQualityChecker.BusinessLogic
{
    public partial class UploadMethods
    {
        public UploadFileReturnModel CheckProvidedFile(IFormFile file, bool noHeader, int headerStarts, int rowsCount) 
        {
            UploadFileReturnModel returnData = new UploadFileReturnModel();

            if (file != null)
            {
                try 
                {
                    returnData.sessionID = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 12);
                    int newDocID = QualityContext.SaveNewDocument(returnData.sessionID, file.FileName, !noHeader);

                    try
                    {
                        if (newDocID != 0)
                        {
                            using (var stream = file.OpenReadStream())
                            {
                                returnData.headers = DataProcessor.StoreFileData(stream, newDocID, noHeader, headerStarts, rowsCount);
                            }
                        }
                    }
                    catch(Exception ex) 
                    {
                        returnData.status = false;
                        returnData.errorMessage = "Unable to process provided file";
                    }

                    returnData.status = returnData.headers.Any();    
                }
                catch(Exception ex) 
                {
                    returnData.status = false;
                    returnData.errorMessage = "Error during Process. Please, contact with Administrator";
                }
            }
            else
            {
                returnData.status = false;
                returnData.errorMessage = "Please, provide a URL or file.";
            }

            return returnData;
        }

        public UploadFileReturnModel CheckProvidedURL(FileURLModel fileURLModel)
        {
            UploadFileReturnModel returnData = new UploadFileReturnModel();

            if (fileURLModel != null && !string.IsNullOrEmpty(fileURLModel.URL))
            {
                try
                {
                    returnData.sessionID = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 12);

                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.OpenRead(fileURLModel.URL);

                            string header_contentDisposition = client.ResponseHeaders["content-disposition"];
                            string filename = new ContentDisposition(header_contentDisposition).FileName;

                            if (filename.ToLower().EndsWith(FileConstants.CSV) || filename.ToLower().EndsWith(FileConstants.XLS) || filename.ToLower().EndsWith(FileConstants.XLSX))
                            {
                                int newDocID = QualityContext.SaveNewDocument(returnData.sessionID, filename, !fileURLModel.noHeader);

                                var content = client.DownloadData(fileURLModel.URL);
                                using (var stream = new MemoryStream(content))
                                {
                                    returnData.headers = DataProcessor.StoreFileData(stream, newDocID, fileURLModel.noHeader, fileURLModel.headerStarts, fileURLModel.rowsCount);

                                    returnData.status = returnData.headers.Any();
                                }
                            }
                        }
                    }
                    catch(Exception ex) 
                    {
                        returnData.status = false;
                        returnData.errorMessage = "Error during file upload. Please, provide another file";
                    }
                }
                catch (Exception ex)
                {
                    returnData.status = false;
                    returnData.errorMessage = "Error during Process. Please, contact with Administrator";
                }
            }
            else
            {
                returnData.status = false;
                returnData.errorMessage = "Please, provide a URL or file.";
            }

            return returnData;
        }

        public ValidatedDataOutput ValidateDataUsingSettings(SettingsModel jsonData)
        {
            ValidatedDataOutput returnData = new ValidatedDataOutput();

            if (jsonData != null) 
            {
                returnData = DataProcessor.FileValidation(jsonData);
            }
            else 
            {
                returnData.status = false;
                returnData.errorMessage = "Empty model provided.";
            }

            return returnData;
        }

        public ValidatedDataOutputForExistingDocument GetValidation(string sessionID)
        {
            ValidatedDataOutputForExistingDocument returnData = new ValidatedDataOutputForExistingDocument();

            if (!string.IsNullOrEmpty(sessionID))
            {
                try 
                {
                    if (QualityContext.GetDocumentStorageIDBySessionID(sessionID) != 0) 
                    {
                        returnData.counters = QualityContext.GetErrorCounters(sessionID);
                        returnData.settings = QualityContext.GetHeaderDataForExistingDocument(sessionID);
                        returnData.status = true;
                    }
                    else 
                    {
                        returnData.status = false;
                        returnData.errorMessage = "Wrong sessionID provided.";
                    }
                }
                catch(Exception ex) 
                {
                    returnData.status = false;
                    returnData.errorMessage = "Unable to process request. Please, contact with Administrator.";
                }
            }
            else
            {
                returnData.status = false;
                returnData.errorMessage = "Empty model provided.";
            }

            return returnData;
        }

        public DefaultReturnModel RemoveRow(string sessionID, int rowID)
        {
            DefaultReturnModel returnData = new DefaultReturnModel();

            if (!string.IsNullOrEmpty(sessionID) && rowID != 0)
            {
                try
                {
                    QualityContext.RemoveRowData(rowID, sessionID);
                    returnData.status = true;
                }
                catch (Exception ex)
                {
                    returnData.status = false;
                    returnData.errorMessage = "Unable to process request. Please, contact with Administrator.";
                }
            }
            else
            {
                returnData.status = false;
                returnData.errorMessage = "Empty model provided.";
            }

            return returnData;
        }

        public ValidatedDataOutput GetCounters(string sessionID)
        {
            ValidatedDataOutput returnData = new ValidatedDataOutput();

            if (!string.IsNullOrEmpty(sessionID))
            {
                try
                {
                    returnData.counters = QualityContext.GetErrorCounters(sessionID);
                    returnData.status = true;
                }
                catch (Exception ex)
                {
                    returnData.status = false;
                    returnData.errorMessage = "Unable to process request. Please, contact with Administrator.";
                }
            }
            else
            {
                returnData.status = false;
                returnData.errorMessage = "Empty model provided.";
            }

            return returnData;
        }
    }
}
