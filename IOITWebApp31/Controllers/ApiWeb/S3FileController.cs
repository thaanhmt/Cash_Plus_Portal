using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class S3FileController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }

        public S3FileController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }
        [Authorize]
        [HttpPost("uploadFiles")]
        public async Task<IActionResult> UploadFiles()
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                string bucketName = _configuration["S3Settings:bucketName"];
                string accessKey = _configuration["S3Settings:accessKey"];
                string secretKey = _configuration["S3Settings:secretKey"];
                string endpoint = _configuration["S3Settings:endpoint"];
                var config = new AmazonS3Config
                {
                    ServiceURL = endpoint,
                    ForcePathStyle = true
                };

                var client = new AmazonS3Client(accessKey, secretKey, config);
                int i = 0;
                var httpRequest = Request.Form.Files;
                List<Attactment> files = new List<Attactment>();
                foreach (var file in httpRequest)
                {
                    Attactment attactment = new Attactment();
                    var postedFile = httpRequest[i];
                    if (postedFile != null && postedFile.Length > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1024; //Size = 1GB  

                        //IList<string> AllowedFileExtensions = new List<string> { ".pdf", ".csv", ".kml", ".shp", ".api" };
                        string extensionFileName = _configuration["AppSettings:extensionFileName"];
                        string extensionFileId = _configuration["AppSettings:extensionFileId"];
                        IList<string> AllowedFileExtensionsId = extensionFileId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim()).ToArray();
                        IList<string> AllowedFileExtensions = extensionFileName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim()).ToArray();
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format($"Bạn vui lòng tải tài liệu có định dạng: {extensionFileName}");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length > MaxContentLength)
                        {
                            var message = string.Format("Bạn vui lòng tải tài liệu có dung lượng nhỏ hơn 1GB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length < 0)
                        {
                            var message = string.Format("Bạn vui lòng tải tài liệu có dung lượng lớn hơn 1KB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            attactment.AttactmentId = Guid.NewGuid();
                            //var nameReplate = name.Replace(" ", "_");
                            string fileName = now.ToString("yyyyMMddHHmmssfff") + "_" + attactment.AttactmentId + extension;

                            try
                            {
                                var uploadRequest = new TransferUtilityUploadRequest
                                {
                                    InputStream = postedFile.OpenReadStream(),
                                    Key = fileName,
                                    BucketName = bucketName,
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                var transferUtility = new TransferUtility(client);
                                await transferUtility.UploadAsync(uploadRequest);
                            }
                            catch (AmazonS3Exception e)
                            {
                                def.meta = new Meta(200, "Tải dữ liệu không thành công!" + e.Message);
                                return Ok(def);
                            }
                            catch (Exception e)
                            {
                                def.meta = new Meta(200, "Tải dữ liệu không thành công!" + e.Message);
                                return Ok(def);
                            }

                            attactment.Name = postedFile.FileName;
                            attactment.Url = fileName;
                            attactment.Storage = postedFile.Length;
                            attactment.ExtensionName = extension;
                            for (int j = 0; j < AllowedFileExtensions.Count; j++)
                            {
                                if (AllowedFileExtensions[j].Trim() == extension)
                                {
                                    attactment.Extension = byte.Parse(AllowedFileExtensionsId[j].Trim());
                                    break;
                                }
                            }
                            attactment.Note = attactment.ExtensionName != null ? attactment.ExtensionName.Substring(1, attactment.ExtensionName.Length - 1).ToUpper() : "";
                            attactment.Status = 1;
                            files.Add(attactment);
                        }
                    }
                    i++;
                }
                def.meta = new Meta(200, "Success");
                def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Bad request:" + ex.Message);
                return Ok(def);
            }

        }
        //[Authorize]
        //[HttpGet("downloadFiles/{id}/{idc}")]
        //public async Task<IActionResult> DownloadFiles(long id, int idc)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    string bucketName = _configuration["S3Settings:bucketName"];
        //    string accessKey = _configuration["S3Settings:accessKey"];
        //    string secretKey = _configuration["S3Settings:secretKey"];
        //    string endpoint = _configuration["S3Settings:endpoint"];
        //    var config = new AmazonS3Config
        //    {
        //        ServiceURL = endpoint,
        //        ForcePathStyle = true
        //    };
        //    var client = new AmazonS3Client(accessKey, secretKey, config);
        //    using (var stream = new MemoryStream())
        //    {
        //        try
        //        {
        //            // Tạo một MemoryStream để lưu trữ nội dung của file RAR
        //            using (var archive = ArchiveFactory.Create(ArchiveType.Rar))
        //            {
        //                using (var db = new IOITDataContext())
        //                {
        //                    try
        //                    {
        //                        var listFiles = await db.Attactment.Where(e => e.TargetId == id
        //                        && e.TargetType == (int)Const.TypeAttachment.FILE_DATASET
        //                        && e.Status != (int)Const.Status.DELETED).ToListAsync();
        //                        foreach (var item in listFiles)
        //                        {
        //                            // Tải nội dung của từng file từ S3
        //                            var getObjectRequest = new GetObjectRequest
        //                            {
        //                                BucketName = bucketName,
        //                                Key = item.Url
        //                            };
        //                            var response = await client.GetObjectAsync(getObjectRequest);
        //                            using (var responseStream = response.ResponseStream)
        //                            {
        //                                //using (var memoryStream = new MemoryStream())
        //                                //{
        //                                response.ResponseStream.CopyTo(stream);
        //                                // Thêm từng file vào file RAR
        //                                var entry = archive.AddEntry(item.Name, stream, true, responseStream.Length);
        //                                //entry.Size = responseStream.Length;
        //                                //}
        //                            }
        //                        }
        //                    }
        //                    catch { };
        //                }
        //                // Ghi nội dung của file RAR vào MemoryStream
        //                archive.SaveTo(stream, new WriterOptions(SharpCompress.Common.CompressionType.Rar));
        //            }
        //            // Lưu nội dung của file RAR vào một file
        //            //using (var fileStream = new FileStream("archive.rar", FileMode.Create))
        //            //{
        //            //    stream.Seek(0, SeekOrigin.Begin);
        //            //    stream.CopyTo(fileStream);
        //            //    fileStream.Position = 0;
        //            return File(stream.ToArray(), "application/zip", "dataset.zip");
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            var a = ex.Message;
        //            return null;
        //        }
        //    }

        //}
        [Authorize]
        [HttpGet("downloadFiles/{id}/{idc}")]
        public async Task<IActionResult> DownloadFiles(long id, int idc)
        {
            DefaultResponse def = new DefaultResponse();
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };
            var client = new AmazonS3Client(accessKey, secretKey, config);
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    // Tạo một MemoryStream để lưu trữ nội dung của file RAR
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        using (var db = new IOITDataContext())
                        {
                            try
                            {
                                var listFiles = await db.Attactment.Where(e => e.TargetId == id
                                && e.TargetType == (int)Const.TypeAttachment.FILE_DATASET
                                && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listFiles)
                                {
                                    // Tải nội dung của từng file từ S3
                                    var getObjectRequest = new GetObjectRequest
                                    {
                                        BucketName = bucketName,
                                        Key = item.Url
                                    };
                                    var response = await client.GetObjectAsync(getObjectRequest);
                                    var stream = response.ResponseStream;
                                    var entry = archive.CreateEntry(item.Name);
                                    using (var entryStream = entry.Open())
                                    {
                                        await stream.CopyToAsync(entryStream);
                                    }
                                }

                                //Lưu lại thông tin mõi khi down
                                var dataSet = await db.DataSet.Where(e => e.DataSetId == id).FirstOrDefaultAsync();
                                if (dataSet != null)
                                {
                                    dataSet.DownNumber += 1;
                                    //Tính lại điểm sao
                                    var cs = await db.ConfigStar.Where(e =>
                                    e.FromView <= dataSet.ViewNumber && e.ToView >= dataSet.ViewNumber
                                    && e.FromDownload <= dataSet.DownNumber && e.ToDownload >= dataSet.DownNumber
                                    && e.Operator == (int)Const.OperatorType.AND).OrderByDescending(e => e.Star).FirstOrDefaultAsync();
                                    if (cs != null)
                                        dataSet.RateStar = cs.Star;
                                    else
                                    {
                                        cs = await db.ConfigStar.Where(e =>
                                        ((e.FromView <= dataSet.ViewNumber && e.ToView >= dataSet.ViewNumber)
                                        || (e.FromDownload <= dataSet.DownNumber && e.ToDownload >= dataSet.DownNumber))
                                        && e.Operator == (int)Const.OperatorType.OR).OrderByDescending(e => e.Star).FirstOrDefaultAsync();
                                        if (cs != null)
                                            dataSet.RateStar = cs.Star;
                                    }
                                    db.DataSet.Update(dataSet);
                                    //list pvud
                                    var listAr = await db.DataSetMapping.Where(e => e.DataSetId == id
                                    && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE).ToListAsync();
                                    //list lvnc
                                    var listRa = await db.DataSetMapping.Where(e => e.DataSetId == id
                                    && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA).ToListAsync();

                                    //list unit
                                    var listUnit = await db.DataSetMapping.Where(e => e.DataSetId == id
                                    && e.TargetType == (int)Const.DataSetMapping.DATA_UNIT).ToListAsync();
                                    List<DataSetDown> listDataSetDowns = new List<DataSetDown>();
                                    if (listAr.Count > 0)
                                    {
                                        foreach (var itemA in listAr)
                                        {
                                            if (listRa.Count > 0)
                                            {
                                                foreach (var itemB in listRa)
                                                {
                                                    if (listUnit.Count > 0)
                                                    {
                                                        foreach (var itemC in listUnit)
                                                        {
                                                            DataSetDown dataSetDown = new DataSetDown();
                                                            dataSetDown.DataSetDownId = Guid.NewGuid();
                                                            dataSetDown.DataSetId = id;
                                                            dataSetDown.ApplicationRangeId = itemA.TargetId;
                                                            dataSetDown.ResearchAreaId = itemB.TargetId;
                                                            dataSetDown.UnitId = itemC.TargetId;
                                                            dataSetDown.UpdatedId = idc;
                                                            dataSetDown.CreatedId = idc;
                                                            dataSetDown.CreatedAt = DateTime.Now;
                                                            dataSetDown.UpdatedAt = DateTime.Now;
                                                            dataSetDown.Status = (int)Const.Status.NORMAL;
                                                            listDataSetDowns.Add(dataSetDown);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DataSetDown dataSetDown = new DataSetDown();
                                                        dataSetDown.DataSetDownId = Guid.NewGuid();
                                                        dataSetDown.DataSetId = id;
                                                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                                                        dataSetDown.ResearchAreaId = itemB.TargetId;
                                                        dataSetDown.UnitId = -1;
                                                        dataSetDown.UpdatedId = idc;
                                                        dataSetDown.CreatedId = idc;
                                                        dataSetDown.CreatedAt = DateTime.Now;
                                                        dataSetDown.UpdatedAt = DateTime.Now;
                                                        dataSetDown.Status = (int)Const.Status.NORMAL;
                                                        listDataSetDowns.Add(dataSetDown);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (listUnit.Count > 0)
                                                {
                                                    foreach (var itemC in listUnit)
                                                    {
                                                        DataSetDown dataSetDown = new DataSetDown();
                                                        dataSetDown.DataSetDownId = Guid.NewGuid();
                                                        dataSetDown.DataSetId = id;
                                                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                                                        dataSetDown.ResearchAreaId = -1;
                                                        dataSetDown.UnitId = itemC.TargetId;
                                                        dataSetDown.UpdatedId = idc;
                                                        dataSetDown.CreatedId = idc;
                                                        dataSetDown.CreatedAt = DateTime.Now;
                                                        dataSetDown.UpdatedAt = DateTime.Now;
                                                        dataSetDown.Status = (int)Const.Status.NORMAL;
                                                        listDataSetDowns.Add(dataSetDown);
                                                    }
                                                }
                                                else
                                                {
                                                    DataSetDown dataSetDown = new DataSetDown();
                                                    dataSetDown.DataSetDownId = Guid.NewGuid();
                                                    dataSetDown.DataSetId = id;
                                                    dataSetDown.ApplicationRangeId = itemA.TargetId;
                                                    dataSetDown.ResearchAreaId = -1;
                                                    dataSetDown.UnitId = -1;
                                                    dataSetDown.UpdatedId = idc;
                                                    dataSetDown.CreatedId = idc;
                                                    dataSetDown.CreatedAt = DateTime.Now;
                                                    dataSetDown.UpdatedAt = DateTime.Now;
                                                    dataSetDown.Status = (int)Const.Status.NORMAL;
                                                    listDataSetDowns.Add(dataSetDown);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (listRa.Count > 0)
                                        {
                                            foreach (var itemB in listRa)
                                            {
                                                if (listUnit.Count > 0)
                                                {
                                                    foreach (var itemC in listUnit)
                                                    {
                                                        DataSetDown dataSetDown = new DataSetDown();
                                                        dataSetDown.DataSetDownId = Guid.NewGuid();
                                                        dataSetDown.DataSetId = id;
                                                        dataSetDown.ApplicationRangeId = -1;
                                                        dataSetDown.ResearchAreaId = itemB.TargetId;
                                                        dataSetDown.UnitId = itemC.TargetId;
                                                        dataSetDown.UpdatedId = idc;
                                                        dataSetDown.CreatedId = idc;
                                                        dataSetDown.CreatedAt = DateTime.Now;
                                                        dataSetDown.UpdatedAt = DateTime.Now;
                                                        dataSetDown.Status = (int)Const.Status.NORMAL;
                                                        listDataSetDowns.Add(dataSetDown);
                                                    }
                                                }
                                                else
                                                {
                                                    DataSetDown dataSetDown = new DataSetDown();
                                                    dataSetDown.DataSetDownId = Guid.NewGuid();
                                                    dataSetDown.DataSetId = id;
                                                    dataSetDown.ApplicationRangeId = -1;
                                                    dataSetDown.ResearchAreaId = itemB.TargetId;
                                                    dataSetDown.UnitId = -1;
                                                    dataSetDown.UpdatedId = idc;
                                                    dataSetDown.CreatedId = idc;
                                                    dataSetDown.CreatedAt = DateTime.Now;
                                                    dataSetDown.UpdatedAt = DateTime.Now;
                                                    dataSetDown.Status = (int)Const.Status.NORMAL;
                                                    listDataSetDowns.Add(dataSetDown);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (listUnit.Count > 0)
                                            {
                                                foreach (var itemC in listUnit)
                                                {
                                                    DataSetDown dataSetDown = new DataSetDown();
                                                    dataSetDown.DataSetDownId = Guid.NewGuid();
                                                    dataSetDown.DataSetId = id;
                                                    dataSetDown.ApplicationRangeId = -1;
                                                    dataSetDown.ResearchAreaId = -1;
                                                    dataSetDown.UnitId = itemC.TargetId;
                                                    dataSetDown.UpdatedId = idc;
                                                    dataSetDown.CreatedId = idc;
                                                    dataSetDown.CreatedAt = DateTime.Now;
                                                    dataSetDown.UpdatedAt = DateTime.Now;
                                                    dataSetDown.Status = (int)Const.Status.NORMAL;
                                                    listDataSetDowns.Add(dataSetDown);
                                                }
                                            }
                                            else
                                            {
                                                DataSetDown dataSetDown = new DataSetDown();
                                                dataSetDown.DataSetDownId = Guid.NewGuid();
                                                dataSetDown.DataSetId = id;
                                                dataSetDown.ApplicationRangeId = -1;
                                                dataSetDown.ResearchAreaId = -1;
                                                dataSetDown.UnitId = -1;
                                                dataSetDown.UpdatedId = idc;
                                                dataSetDown.CreatedId = idc;
                                                dataSetDown.CreatedAt = DateTime.Now;
                                                dataSetDown.UpdatedAt = DateTime.Now;
                                                dataSetDown.Status = (int)Const.Status.NORMAL;
                                                listDataSetDowns.Add(dataSetDown);
                                            }
                                        }
                                    }

                                    await db.DataSetDown.AddRangeAsync(listDataSetDowns);
                                    await db.SaveChangesAsync();
                                }
                            }
                            catch { };
                        }
                    }
                    memoryStream.Position = 0;
                    return File(memoryStream.ToArray(), "application/zip", "dataset.zip");
                    //}
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                    return null;
                }
            }

        }

        //[HttpGet("downloadFiles/{id}/{idc}")]
        //public async Task<IActionResult> DownloadFiles(long id, int idc)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    string bucketName = _configuration["S3Settings:bucketName"];
        //    string accessKey = _configuration["S3Settings:accessKey"];
        //    string secretKey = _configuration["S3Settings:secretKey"];
        //    string endpoint = _configuration["S3Settings:endpoint"];
        //    var config = new AmazonS3Config
        //    {
        //        ServiceURL = endpoint,
        //        ForcePathStyle = true
        //    };

        //    var client = new AmazonS3Client(accessKey, secretKey, config);
        //    using (var outStream = new MemoryStream())
        //    {
        //        using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
        //        {
        //            using (var db = new IOITDataContext())
        //            {
        //                try
        //                {
        //                    var listFiles = await db.Attactment.Where(e => e.TargetId == id
        //                    && e.TargetType == (int)Const.TypeAttachment.FILE_DATASET
        //                    && e.Status != (int)Const.Status.DELETED).ToListAsync();

        //                    foreach (var file in listFiles)
        //                    {
        //                        try
        //                        {
        //                            var downloadRequest = new GetObjectRequest
        //                            {
        //                                BucketName = bucketName,
        //                                Key = file.Url
        //                            };

        //                            using (var response = client.GetObjectAsync(downloadRequest))
        //                            {
        //                                using (var responseStream = response.Result.ResponseStream)
        //                                {
        //                                //    using (var memoryStream = new MemoryStream())
        //                                //{
        //                                    //    memoryStream.CopyTo(responseStream);
        //                                    //response.Result.ResponseStream.CopyTo(memoryStream);
        //                                    var fileInArchive = archive.CreateEntry(Path.GetFileName(file.Name), CompressionLevel.Optimal);
        //                                    using (var entryStream = fileInArchive.Open())
        //                                    {
        //                                        responseStream.CopyTo(entryStream);
        //                                    }
        //                                    //}
        //                                }
        //                            }
        //                        }
        //                        catch (Exception ex){
        //                            var err = ex.Message;
        //                        };

        //                    }
        //                }
        //                catch { }
        //                //Lưu lại thông tin mõi khi down
        //                var dataSet = await db.DataSet.Where(e => e.DataSetId == id).FirstOrDefaultAsync();
        //                if (dataSet != null)
        //                {
        //                    dataSet.DownNumber += 1;
        //                    db.DataSet.Update(dataSet);
        //                    //list pvud
        //                    var listAr = await db.DataSetMapping.Where(e => e.DataSetId == id
        //                    && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE).ToListAsync();
        //                    //list lvnc
        //                    var listRa = await db.DataSetMapping.Where(e => e.DataSetId == id
        //                    && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA).ToListAsync();

        //                    //list unit
        //                    var listUnit = await db.DataSetMapping.Where(e => e.DataSetId == id
        //                    && e.TargetType == (int)Const.DataSetMapping.DATA_UNIT).ToListAsync();
        //                    List<DataSetDown> listDataSetDowns = new List<DataSetDown>();
        //                    if (listAr.Count > 0)
        //                    {
        //                        foreach (var itemA in listAr)
        //                        {
        //                            if (listRa.Count > 0)
        //                            {
        //                                foreach (var itemB in listRa)
        //                                {
        //                                    if (listUnit.Count > 0)
        //                                    {
        //                                        foreach (var itemC in listUnit)
        //                                        {
        //                                            DataSetDown dataSetDown = new DataSetDown();
        //                                            dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                            dataSetDown.DataSetId = id;
        //                                            dataSetDown.ApplicationRangeId = itemA.TargetId;
        //                                            dataSetDown.ResearchAreaId = itemB.TargetId;
        //                                            dataSetDown.UnitId = itemC.TargetId;
        //                                            dataSetDown.UpdatedId = idc;
        //                                            dataSetDown.CreatedId = idc;
        //                                            dataSetDown.CreatedAt = DateTime.Now;
        //                                            dataSetDown.UpdatedAt = DateTime.Now;
        //                                            dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                            listDataSetDowns.Add(dataSetDown);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        DataSetDown dataSetDown = new DataSetDown();
        //                                        dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                        dataSetDown.DataSetId = id;
        //                                        dataSetDown.ApplicationRangeId = itemA.TargetId;
        //                                        dataSetDown.ResearchAreaId = itemB.TargetId;
        //                                        dataSetDown.UnitId = -1;
        //                                        dataSetDown.UpdatedId = idc;
        //                                        dataSetDown.CreatedId = idc;
        //                                        dataSetDown.CreatedAt = DateTime.Now;
        //                                        dataSetDown.UpdatedAt = DateTime.Now;
        //                                        dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                        listDataSetDowns.Add(dataSetDown);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (listUnit.Count > 0)
        //                                {
        //                                    foreach (var itemC in listUnit)
        //                                    {
        //                                        DataSetDown dataSetDown = new DataSetDown();
        //                                        dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                        dataSetDown.DataSetId = id;
        //                                        dataSetDown.ApplicationRangeId = itemA.TargetId;
        //                                        dataSetDown.ResearchAreaId = -1;
        //                                        dataSetDown.UnitId = itemC.TargetId;
        //                                        dataSetDown.UpdatedId = idc;
        //                                        dataSetDown.CreatedId = idc;
        //                                        dataSetDown.CreatedAt = DateTime.Now;
        //                                        dataSetDown.UpdatedAt = DateTime.Now;
        //                                        dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                        listDataSetDowns.Add(dataSetDown);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    DataSetDown dataSetDown = new DataSetDown();
        //                                    dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                    dataSetDown.DataSetId = id;
        //                                    dataSetDown.ApplicationRangeId = itemA.TargetId;
        //                                    dataSetDown.ResearchAreaId = -1;
        //                                    dataSetDown.UnitId = -1;
        //                                    dataSetDown.UpdatedId = idc;
        //                                    dataSetDown.CreatedId = idc;
        //                                    dataSetDown.CreatedAt = DateTime.Now;
        //                                    dataSetDown.UpdatedAt = DateTime.Now;
        //                                    dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                    listDataSetDowns.Add(dataSetDown);
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (listRa.Count > 0)
        //                        {
        //                            foreach (var itemB in listRa)
        //                            {
        //                                if (listUnit.Count > 0)
        //                                {
        //                                    foreach (var itemC in listUnit)
        //                                    {
        //                                        DataSetDown dataSetDown = new DataSetDown();
        //                                        dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                        dataSetDown.DataSetId = id;
        //                                        dataSetDown.ApplicationRangeId = -1;
        //                                        dataSetDown.ResearchAreaId = itemB.TargetId;
        //                                        dataSetDown.UnitId = itemC.TargetId;
        //                                        dataSetDown.UpdatedId = idc;
        //                                        dataSetDown.CreatedId = idc;
        //                                        dataSetDown.CreatedAt = DateTime.Now;
        //                                        dataSetDown.UpdatedAt = DateTime.Now;
        //                                        dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                        listDataSetDowns.Add(dataSetDown);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    DataSetDown dataSetDown = new DataSetDown();
        //                                    dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                    dataSetDown.DataSetId = id;
        //                                    dataSetDown.ApplicationRangeId = -1;
        //                                    dataSetDown.ResearchAreaId = itemB.TargetId;
        //                                    dataSetDown.UnitId = -1;
        //                                    dataSetDown.UpdatedId = idc;
        //                                    dataSetDown.CreatedId = idc;
        //                                    dataSetDown.CreatedAt = DateTime.Now;
        //                                    dataSetDown.UpdatedAt = DateTime.Now;
        //                                    dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                    listDataSetDowns.Add(dataSetDown);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (listUnit.Count > 0)
        //                            {
        //                                foreach (var itemC in listUnit)
        //                                {
        //                                    DataSetDown dataSetDown = new DataSetDown();
        //                                    dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                    dataSetDown.DataSetId = id;
        //                                    dataSetDown.ApplicationRangeId = -1;
        //                                    dataSetDown.ResearchAreaId = -1;
        //                                    dataSetDown.UnitId = itemC.TargetId;
        //                                    dataSetDown.UpdatedId = idc;
        //                                    dataSetDown.CreatedId = idc;
        //                                    dataSetDown.CreatedAt = DateTime.Now;
        //                                    dataSetDown.UpdatedAt = DateTime.Now;
        //                                    dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                    listDataSetDowns.Add(dataSetDown);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                DataSetDown dataSetDown = new DataSetDown();
        //                                dataSetDown.DataSetDownId = Guid.NewGuid();
        //                                dataSetDown.DataSetId = id;
        //                                dataSetDown.ApplicationRangeId = -1;
        //                                dataSetDown.ResearchAreaId = -1;
        //                                dataSetDown.UnitId = -1;
        //                                dataSetDown.UpdatedId = idc;
        //                                dataSetDown.CreatedId = idc;
        //                                dataSetDown.CreatedAt = DateTime.Now;
        //                                dataSetDown.UpdatedAt = DateTime.Now;
        //                                dataSetDown.Status = (int)Const.Status.NORMAL;
        //                                listDataSetDowns.Add(dataSetDown);
        //                            }
        //                        }
        //                    }

        //                    await db.DataSetDown.AddRangeAsync(listDataSetDowns);
        //                    await db.SaveChangesAsync();
        //                }

        //                outStream.Position = 0;
        //                return File(outStream.ToArray(), "application/zip", "dataset.zip");
        //            }
        //        }
        //    }
        //}

        //[Authorize]
        [Authorize]
        [HttpGet("downloadOneFile/{id}/{idc}/{ida}")]
        public async Task<HttpResponseMessage> DownloadOneFile(long id, int idc, Guid ida)
        {
            //DefaultResponse def = new DefaultResponse();
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(accessKey, secretKey, config);
            using (var outStream = new MemoryStream())
            {
                using (var db = new IOITDataContext())
                {
                    Attactment attactment = await db.Attactment.Where(a => a.AttactmentId == ida).FirstOrDefaultAsync();
                    try
                    {

                        if (attactment != null)
                        {
                            try
                            {
                                var downloadRequest = new GetObjectRequest
                                {
                                    BucketName = bucketName,
                                    Key = attactment.Url
                                };

                                using (var responseS = await client.GetObjectAsync(downloadRequest))
                                {
                                    using (var responseStream = responseS.ResponseStream)
                                    {
                                        await responseStream.CopyToAsync(outStream);

                                    }

                                }
                            }
                            catch { };

                        }
                    }
                    catch { }
                    //Lưu lại thông tin mõi khi down
                    var dataSet = await db.DataSet.Where(e => e.DataSetId == id).FirstOrDefaultAsync();
                    if (dataSet != null)
                    {
                        dataSet.DownNumber += 1;
                        //Tính lại điểm sao
                        var cs = await db.ConfigStar.Where(e =>
                        e.FromView <= dataSet.ViewNumber && e.ToView >= dataSet.ViewNumber
                        && e.FromDownload <= dataSet.DownNumber && e.ToDownload >= dataSet.DownNumber
                        && e.Operator == (int)Const.OperatorType.AND).OrderByDescending(e => e.Star).FirstOrDefaultAsync();
                        if (cs != null)
                            dataSet.RateStar = cs.Star;
                        else
                        {
                            cs = await db.ConfigStar.Where(e =>
                            ((e.FromView <= dataSet.ViewNumber && e.ToView >= dataSet.ViewNumber)
                            || (e.FromDownload <= dataSet.DownNumber && e.ToDownload >= dataSet.DownNumber))
                            && e.Operator == (int)Const.OperatorType.OR).OrderByDescending(e => e.Star).FirstOrDefaultAsync();
                            if (cs != null)
                                dataSet.RateStar = cs.Star;
                        }
                        db.DataSet.Update(dataSet);
                        //list pvud
                        var listAr = await db.DataSetMapping.Where(e => e.DataSetId == id
                        && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE).ToListAsync();
                        //list lvnc
                        var listRa = await db.DataSetMapping.Where(e => e.DataSetId == id
                        && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA).ToListAsync();

                        //list unit
                        var listUnit = await db.DataSetMapping.Where(e => e.DataSetId == id
                        && e.TargetType == (int)Const.DataSetMapping.DATA_UNIT).ToListAsync();
                        List<DataSetDown> listDataSetDowns = new List<DataSetDown>();
                        if (listAr.Count > 0)
                        {
                            foreach (var itemA in listAr)
                            {
                                if (listRa.Count > 0)
                                {
                                    foreach (var itemB in listRa)
                                    {
                                        if (listUnit.Count > 0)
                                        {
                                            foreach (var itemC in listUnit)
                                            {
                                                DataSetDown dataSetDown = new DataSetDown();
                                                dataSetDown.DataSetDownId = Guid.NewGuid();
                                                dataSetDown.DataSetId = id;
                                                dataSetDown.ApplicationRangeId = itemA.TargetId;
                                                dataSetDown.ResearchAreaId = itemB.TargetId;
                                                dataSetDown.UnitId = itemC.TargetId;
                                                dataSetDown.UpdatedId = idc;
                                                dataSetDown.CreatedId = idc;
                                                dataSetDown.CreatedAt = DateTime.Now;
                                                dataSetDown.UpdatedAt = DateTime.Now;
                                                dataSetDown.Status = (int)Const.Status.NORMAL;
                                                listDataSetDowns.Add(dataSetDown);
                                            }
                                        }
                                        else
                                        {
                                            DataSetDown dataSetDown = new DataSetDown();
                                            dataSetDown.DataSetDownId = Guid.NewGuid();
                                            dataSetDown.DataSetId = id;
                                            dataSetDown.ApplicationRangeId = itemA.TargetId;
                                            dataSetDown.ResearchAreaId = itemB.TargetId;
                                            dataSetDown.UnitId = -1;
                                            dataSetDown.UpdatedId = idc;
                                            dataSetDown.CreatedId = idc;
                                            dataSetDown.CreatedAt = DateTime.Now;
                                            dataSetDown.UpdatedAt = DateTime.Now;
                                            dataSetDown.Status = (int)Const.Status.NORMAL;
                                            listDataSetDowns.Add(dataSetDown);
                                        }
                                    }
                                }
                                else
                                {
                                    if (listUnit.Count > 0)
                                    {
                                        foreach (var itemC in listUnit)
                                        {
                                            DataSetDown dataSetDown = new DataSetDown();
                                            dataSetDown.DataSetDownId = Guid.NewGuid();
                                            dataSetDown.DataSetId = id;
                                            dataSetDown.ApplicationRangeId = itemA.TargetId;
                                            dataSetDown.ResearchAreaId = -1;
                                            dataSetDown.UnitId = itemC.TargetId;
                                            dataSetDown.UpdatedId = idc;
                                            dataSetDown.CreatedId = idc;
                                            dataSetDown.CreatedAt = DateTime.Now;
                                            dataSetDown.UpdatedAt = DateTime.Now;
                                            dataSetDown.Status = (int)Const.Status.NORMAL;
                                            listDataSetDowns.Add(dataSetDown);
                                        }
                                    }
                                    else
                                    {
                                        DataSetDown dataSetDown = new DataSetDown();
                                        dataSetDown.DataSetDownId = Guid.NewGuid();
                                        dataSetDown.DataSetId = id;
                                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                                        dataSetDown.ResearchAreaId = -1;
                                        dataSetDown.UnitId = -1;
                                        dataSetDown.UpdatedId = idc;
                                        dataSetDown.CreatedId = idc;
                                        dataSetDown.CreatedAt = DateTime.Now;
                                        dataSetDown.UpdatedAt = DateTime.Now;
                                        dataSetDown.Status = (int)Const.Status.NORMAL;
                                        listDataSetDowns.Add(dataSetDown);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (listRa.Count > 0)
                            {
                                foreach (var itemB in listRa)
                                {
                                    if (listUnit.Count > 0)
                                    {
                                        foreach (var itemC in listUnit)
                                        {
                                            DataSetDown dataSetDown = new DataSetDown();
                                            dataSetDown.DataSetDownId = Guid.NewGuid();
                                            dataSetDown.DataSetId = id;
                                            dataSetDown.ApplicationRangeId = -1;
                                            dataSetDown.ResearchAreaId = itemB.TargetId;
                                            dataSetDown.UnitId = itemC.TargetId;
                                            dataSetDown.UpdatedId = idc;
                                            dataSetDown.CreatedId = idc;
                                            dataSetDown.CreatedAt = DateTime.Now;
                                            dataSetDown.UpdatedAt = DateTime.Now;
                                            dataSetDown.Status = (int)Const.Status.NORMAL;
                                            listDataSetDowns.Add(dataSetDown);
                                        }
                                    }
                                    else
                                    {
                                        DataSetDown dataSetDown = new DataSetDown();
                                        dataSetDown.DataSetDownId = Guid.NewGuid();
                                        dataSetDown.DataSetId = id;
                                        dataSetDown.ApplicationRangeId = -1;
                                        dataSetDown.ResearchAreaId = itemB.TargetId;
                                        dataSetDown.UnitId = -1;
                                        dataSetDown.UpdatedId = idc;
                                        dataSetDown.CreatedId = idc;
                                        dataSetDown.CreatedAt = DateTime.Now;
                                        dataSetDown.UpdatedAt = DateTime.Now;
                                        dataSetDown.Status = (int)Const.Status.NORMAL;
                                        listDataSetDowns.Add(dataSetDown);
                                    }
                                }
                            }
                            else
                            {
                                if (listUnit.Count > 0)
                                {
                                    foreach (var itemC in listUnit)
                                    {
                                        DataSetDown dataSetDown = new DataSetDown();
                                        dataSetDown.DataSetDownId = Guid.NewGuid();
                                        dataSetDown.DataSetId = id;
                                        dataSetDown.ApplicationRangeId = -1;
                                        dataSetDown.ResearchAreaId = -1;
                                        dataSetDown.UnitId = itemC.TargetId;
                                        dataSetDown.UpdatedId = idc;
                                        dataSetDown.CreatedId = idc;
                                        dataSetDown.CreatedAt = DateTime.Now;
                                        dataSetDown.UpdatedAt = DateTime.Now;
                                        dataSetDown.Status = (int)Const.Status.NORMAL;
                                        listDataSetDowns.Add(dataSetDown);
                                    }
                                }
                                else
                                {
                                    DataSetDown dataSetDown = new DataSetDown();
                                    dataSetDown.DataSetDownId = Guid.NewGuid();
                                    dataSetDown.DataSetId = id;
                                    dataSetDown.ApplicationRangeId = -1;
                                    dataSetDown.ResearchAreaId = -1;
                                    dataSetDown.UnitId = -1;
                                    dataSetDown.UpdatedId = idc;
                                    dataSetDown.CreatedId = idc;
                                    dataSetDown.CreatedAt = DateTime.Now;
                                    dataSetDown.UpdatedAt = DateTime.Now;
                                    dataSetDown.Status = (int)Const.Status.NORMAL;
                                    listDataSetDowns.Add(dataSetDown);
                                }
                            }
                        }

                        await db.DataSetDown.AddRangeAsync(listDataSetDowns);
                        await db.SaveChangesAsync();
                    }

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(outStream.ToArray())
                    };
                    response.Content.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type,x-filename");
                    response.Content.Headers.Add("Access-Control-Expose-Headers", "Authorization,Content-Type,x-filename");
                    response.Content.Headers.Add("x-filename", attactment.Name);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue
                           ("application/octet-stream");
                    response.Content.Headers.ContentDisposition =
                           new ContentDispositionHeaderValue("attachment")
                           {
                               FileName = attactment.Name
                           };

                    return response;
                    //response.Content = new StreamContent(outStream);
                    //// Thiết lập kiểu content
                    //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    //// Thiết lập tên file và loại file trong header
                    //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    //{
                    //    FileName = attactment.Name,
                    //    Size = outStream.Length
                    //};

                    //return response;
                    //outStream.Position = 0;
                    //return File(outStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, attactment.Name);
                }
            }
        }

        [HttpGet("viewFile/{ida}")]
        public async Task<IActionResult> ViewFile(Guid ida)
        {
            DefaultResponse def = new DefaultResponse();
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(accessKey, secretKey, config);
            using (var db = new IOITDataContext())
            {
                var base64String = "";
                Attactment attactment = await db.Attactment.Where(a => a.AttactmentId == ida).FirstOrDefaultAsync();
                try
                {

                    if (attactment != null)
                    {
                        try
                        {
                            var downloadRequest = new GetObjectRequest
                            {
                                BucketName = bucketName,
                                Key = attactment.Url
                            };

                            using (var response = await client.GetObjectAsync(downloadRequest))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    response.ResponseStream.CopyTo(memoryStream);

                                    // Chuyển đổi MemoryStream thành chuỗi Base64
                                    base64String = Convert.ToBase64String(memoryStream.ToArray());

                                    // In ra chuỗi Base64
                                    //Console.WriteLine(base64String);
                                }
                                //using (var responseStream = response.ResponseStream)
                                //{
                                //    //var fileInArchive = archive.CreateEntry(Path.GetFileName(attactment.Name), CompressionLevel.Optimal);
                                //    //using (var entryStream = fileInArchive.Open())
                                //    //{
                                //    //await responseStream.CopyToAsync(outStream);
                                //    //}
                                //    var buffer = new byte[responseStream.Length];
                                //    responseStream.Read(buffer, 0, buffer.Length);
                                //    base64String = Convert.ToBase64String(buffer);
                                //}

                            }
                        }
                        catch { };

                    }
                    else
                    {
                        def.data = base64String;
                        def.meta = new Meta(200, "Thông tin tệp đính kèm không tồn tại!");
                        return Ok(def);
                    }
                }
                catch { }

                attactment.Note = base64String;
                def.data = attactment;
                def.meta = new Meta(200, "Lấy dữ liệu thành công!");
                return Ok(def);
                //outStream.Position = 0;
                //return File(outStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, attactment.Name);
            }
        }

        [HttpGet("getAllFile")]
        public async Task<IActionResult> GetAllFile()
        {
            DefaultResponse def = new DefaultResponse();
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(accessKey, secretKey, config);
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            var response = await client.ListObjectsV2Async(request);

            foreach (var obj in response.S3Objects)
            {
                Console.WriteLine($"Key: {obj.Key} | Size: {obj.Size}");
            }
            return Ok(response);
        }

        [HttpGet("getAllFolders")]
        public async Task<IActionResult> GetAllFolders([FromQuery] FilteredPagination paging)
        {


            DefaultResponse def = new DefaultResponse();
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");

                    string extensionFileName = _configuration["AppSettings:extensionFileName"];
                    string extensionFileId = _configuration["AppSettings:extensionFileId"];
                    IList<string> AllowedFileExtensionsId = extensionFileId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim()).ToArray();
                    IList<string> AllowedFileExtensions = extensionFileName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim()).ToArray();

                    List<FolderFileCeph> listFolderCephs = new List<FolderFileCeph>();
                    var dataBB = await db.FolderCeph.Where(e => e.Status != (int)Const.Status.DELETED).ToListAsync();
                    await ListAllPrefixesAsync(paging, listFolderCephs, dataBB, "");

                    var config = new AmazonS3Config
                    {
                        ServiceURL = endpoint,
                        ForcePathStyle = true
                    };

                    var client = new AmazonS3Client(accessKey, secretKey, config);

                    ////// Tạo folder trong Ceph S3
                    ////string folderName = "test-folder/";
                    ////PutObjectRequest putRequest = new PutObjectRequest
                    ////{
                    ////    BucketName = bucketName,
                    ////    Key = folderName,
                    ////    ContentBody = string.Empty
                    ////};
                    ////await client.PutObjectAsync(putRequest);

                    //// Liệt kê các folder trong Ceph S3
                    //ListObjectsV2Request listRequest = new ListObjectsV2Request
                    //{
                    //    BucketName = bucketName,
                    //    Delimiter = "/",
                    //    MaxKeys = 100000,
                    //};
                    //ListObjectsV2Response listResponse = await client.ListObjectsV2Async(listRequest);
                    //if (paging.query != null)
                    //{
                    //    paging.query = HttpUtility.UrlDecode(paging.query);
                    //}

                    //var dataB = await db.FolderCeph.Where(e => e.Status != (int)Const.Status.DELETED).ToListAsync();
                    //var data = listResponse.CommonPrefixes.AsQueryable();
                    //data = data.Where(e => e != "/");
                    //if (paging.query != "1=1")
                    //    data = data.Where(e => e.Contains(paging.query));
                    ////loại bớt các folder đã dùng rùi
                    //var dataNotInB = data
                    //.GroupJoin(
                    //    dataB,
                    //    a => a,
                    //    b => b.Link,
                    //    (a, b) => new { TableAData = a, TableBData = b })
                    //.Where(ab => !ab.TableBData.Any())
                    //.Select(ab => ab.TableAData).AsQueryable();

                    def.metadata = listFolderCephs.Count();
                    var data = listFolderCephs.AsQueryable().Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                    List<FolderFileCeph> folderCephs = new List<FolderFileCeph>();
                    int k = (paging.page - 1) * paging.page_size + 1;
                    foreach (var commonPrefix in data.ToList())
                    {
                        if (commonPrefix.Link != "/")
                        {
                            FolderFileCeph folderCeph = new FolderFileCeph();
                            folderCeph.STT = k + "";
                            folderCeph.Name = commonPrefix.Name;
                            folderCeph.Link = commonPrefix.Link;
                            folderCeph.Storage = 0;
                            folderCeph.IsView = false;
                            //
                            // Yêu cầu lấy dữ liệu trong folder
                            ListObjectsV2Request listRequestFile = new ListObjectsV2Request
                            {
                                BucketName = bucketName,
                                Prefix = commonPrefix.Link
                            };
                            ListObjectsV2Response listResponseFile = await client.ListObjectsV2Async(listRequestFile);
                            List<AttactmentDTO> fileCephs = new List<AttactmentDTO>();
                            int kk = 1;
                            foreach (var obj in listResponseFile.S3Objects)
                            {
                                if (obj.Size > 0)
                                {

                                    folderCeph.Storage += obj.Size;
                                    AttactmentDTO fileCeph = new AttactmentDTO();
                                    fileCeph.AttactmentId = Guid.NewGuid();
                                    fileCeph.STT = k + "." + kk;
                                    fileCeph.Name = obj.Key.Split("/").LastOrDefault();
                                    fileCeph.Url = obj.Key;
                                    fileCeph.Storage = obj.Size;
                                    string extension = "." + fileCeph.Name.Split(".").LastOrDefault();
                                    fileCeph.ExtensionName = extension;
                                    for (int j = 0; j < AllowedFileExtensions.Count; j++)
                                    {
                                        if (AllowedFileExtensions[j].Trim() == extension)
                                        {
                                            fileCeph.Extension = byte.Parse(AllowedFileExtensionsId[j].Trim());
                                            break;
                                        }
                                    }
                                    fileCeph.Note = fileCeph.ExtensionName != null ? fileCeph.ExtensionName.Substring(1, fileCeph.ExtensionName.Length - 1).ToUpper() : "";
                                    fileCeph.Status = 1;
                                    fileCephs.Add(fileCeph);
                                    kk++;
                                }
                            }
                            folderCeph.listFiles = fileCephs;
                            folderCephs.Add(folderCeph);
                            k++;
                        }
                    }
                    def.data = folderCephs;

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }

        }

        [HttpPost("selectFileCeph")]
        public async Task<IActionResult> SelectFileCeph([FromBody] AttactmentDTO input)
        {
            DefaultResponse def = new DefaultResponse();
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(accessKey, secretKey, config);
            using (var db = new IOITDataContext())
            {
                DataCeph data = new DataCeph();
                DataCeph2 data2 = new DataCeph2();
                DataCeph3 data3 = new DataCeph3();
                int type = 1;

                try
                {
                    var downloadRequest = new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = input.Url
                    };

                    using (var response = await client.GetObjectAsync(downloadRequest))
                    {
                        using (var stream = response.ResponseStream)
                        {
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                string jsonString = await reader.ReadToEndAsync();
                                // Đọc nội dung JSON và xử lý dữ liệu tùy ý
                                try
                                {
                                    data = JsonSerializer.Deserialize<DataCeph>(jsonString);
                                    type = 1;
                                }
                                catch
                                {
                                    try
                                    {
                                        data2 = JsonSerializer.Deserialize<DataCeph2>(jsonString);
                                        type = 2;
                                    }
                                    catch
                                    {
                                        data3 = JsonSerializer.Deserialize<DataCeph3>(jsonString);
                                        type = 3;
                                    }
                                }
                            }
                        }


                    }
                }
                catch { };
                if (type == 1)
                    def.data = data;
                else if (type == 2)
                    def.data = data2;
                else if (type == 3)
                    def.data = data3;
                def.meta = new Meta(200, "Lấy dữ liệu thành công!");
                return Ok(def);
            }
        }

        private async Task ListAllPrefixesAsync([FromQuery] FilteredPagination paging,
            List<FolderFileCeph> listFolderCephs, List<FolderCeph> dataB, string marker)
        {
            string bucketName = _configuration["S3Settings:bucketName"];
            string accessKey = _configuration["S3Settings:accessKey"];
            string secretKey = _configuration["S3Settings:secretKey"];
            string endpoint = _configuration["S3Settings:endpoint"];

            string extensionFileName = _configuration["AppSettings:extensionFileName"];
            string extensionFileId = _configuration["AppSettings:extensionFileId"];
            IList<string> AllowedFileExtensionsId = extensionFileId.Split(',', StringSplitOptions.RemoveEmptyEntries)
           .Select(s => s.Trim()).ToArray();
            IList<string> AllowedFileExtensions = extensionFileName.Split(',', StringSplitOptions.RemoveEmptyEntries)
           .Select(s => s.Trim()).ToArray();

            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(accessKey, secretKey, config);

            //// Tạo folder trong Ceph S3
            //string folderName = "test-folder/";
            //PutObjectRequest putRequest = new PutObjectRequest
            //{
            //    BucketName = bucketName,
            //    Key = folderName,
            //    ContentBody = string.Empty
            //};
            //await client.PutObjectAsync(putRequest);

            // Liệt kê các folder trong Ceph S3
            ListObjectsV2Request listRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Delimiter = "/",
                ContinuationToken = marker,
            };
            ListObjectsV2Response listResponse = await client.ListObjectsV2Async(listRequest);

            if (listResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                if (paging.query != null)
                {
                    paging.query = HttpUtility.UrlDecode(paging.query);
                }

                //var dataB = await db.FolderCeph.Where(e => e.Status != (int)Const.Status.DELETED).ToListAsync();
                var data = listResponse.CommonPrefixes.AsQueryable();
                data = data.Where(e => e != "/");
                if (paging.query != "1=1")
                    data = data.Where(e => e.Contains(paging.query));
                //loại bớt các folder đã dùng rùi
                var dataNotInB = data
                .GroupJoin(
                    dataB,
                    a => a,
                    b => b.Link,
                    (a, b) => new { TableAData = a, TableBData = b })
                .Where(ab => !ab.TableBData.Any())
                .Select(ab => ab.TableAData).AsQueryable();

                //data = dataNotInB.Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                //List<FolderFileCeph> folderCephs = new List<FolderFileCeph>();
                int k = (paging.page - 1) * paging.page_size + 1;
                foreach (var commonPrefix in data)
                {
                    if (commonPrefix != "/")
                    {
                        FolderFileCeph folderCeph = new FolderFileCeph();
                        folderCeph.STT = k + "";
                        folderCeph.Name = commonPrefix.Replace("/", "");
                        folderCeph.Link = commonPrefix;
                        folderCeph.Storage = 0;
                        folderCeph.IsView = false;
                        //
                        //// Yêu cầu lấy dữ liệu trong folder
                        //ListObjectsV2Request listRequestFile = new ListObjectsV2Request
                        //{
                        //    BucketName = bucketName,
                        //    Prefix = commonPrefix
                        //};
                        //ListObjectsV2Response listResponseFile = await client.ListObjectsV2Async(listRequestFile);
                        //List<AttactmentDTO> fileCephs = new List<AttactmentDTO>();
                        //int kk = 1;
                        //foreach (var obj in listResponseFile.S3Objects)
                        //{
                        //    if (obj.Size > 0)
                        //    {

                        //        folderCeph.Storage += obj.Size;
                        //        AttactmentDTO fileCeph = new AttactmentDTO();
                        //        fileCeph.AttactmentId = Guid.NewGuid();
                        //        fileCeph.STT = k + "." + kk;
                        //        fileCeph.Name = obj.Key.Split("/").LastOrDefault();
                        //        fileCeph.Url = obj.Key;
                        //        fileCeph.Storage = obj.Size;
                        //        string extension = "." + fileCeph.Name.Split(".").LastOrDefault();
                        //        fileCeph.ExtensionName = extension;
                        //        for (int j = 0; j < AllowedFileExtensions.Count; j++)
                        //        {
                        //            if (AllowedFileExtensions[j].Trim() == extension)
                        //            {
                        //                fileCeph.Extension = byte.Parse(AllowedFileExtensionsId[j].Trim());
                        //                break;
                        //            }
                        //        }
                        //        fileCeph.Note = fileCeph.ExtensionName != null ? fileCeph.ExtensionName.Substring(1, fileCeph.ExtensionName.Length - 1).ToUpper() : "";
                        //        fileCeph.Status = 1;
                        //        fileCephs.Add(fileCeph);
                        //        kk++;
                        //    }
                        //}
                        //folderCeph.listFiles = fileCephs;
                        listFolderCephs.Add(folderCeph);
                        k++;
                    }
                }
                if (listResponse.NextContinuationToken != null)
                {
                    await ListAllPrefixesAsync(paging, listFolderCephs, dataB, listResponse.NextContinuationToken);
                }
            }
        }


        //[HttpPost("deleteFiles/{id}")]
        //public async Task<IActionResult> DeleteFiles(long id)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    string bucketName = "ceph-bkt-4ba23b92-40fa-4fa7-83b5-3a8d0f620edd";
        //    string fileKey = "CNTT_Dataset_URD_v2.1_20230225210558274.kml";
        //    //string filePath = @"D:\Project\2023\IOITDataSetT\IOITWebApp31\wwwroot\datasets\CNTT_Dataset_URD_v2.1_20230225210558274.kml";
        //    string accessKey = "85032LK8JTAGGCIWM7LG";
        //    string secretKey = "wJUnCIOA2lJLRjGANurO3Nw4yUivx3mQOh0gs2Cs";
        //    string endpoint = "http://119.15.161.250:31665/";

        //    var config = new AmazonS3Config
        //    {
        //        ServiceURL = endpoint,
        //        ForcePathStyle = true
        //    };

        //    var client = new AmazonS3Client(accessKey, secretKey, config);
        //    try
        //    {
        //        var deleteRequest = new DeleteObjectRequest
        //        {
        //            BucketName = bucketName,
        //            Key = fileKey
        //        };

        //        await client.DeleteObjectAsync(deleteRequest);
        //        def.meta = new Meta(200, "Xóa dữ liệu thành công!");
        //        return Ok(def);
        //    }
        //    catch (AmazonS3Exception e)
        //    {
        //        def.meta = new Meta(200, "Xóa dữ liệu không thành công!");
        //        return Ok(def);
        //    }
        //    catch (Exception e)
        //    {
        //        def.meta = new Meta(200, "Xóa dữ liệu không thành công!");
        //        return Ok(def);
        //    }
        //}

    }
}
