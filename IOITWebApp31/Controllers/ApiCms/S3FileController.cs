using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using IOITWebApp31.Models;
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiCms
{
    [Authorize]
    [Route("api/[controller]")]
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
                            string fileName = attactment.AttactmentId + "_" + now.ToString("yyyyMMddHHmmssfff") + extension;

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
                                def.meta = new Meta(200, "Tải dữ liệu không thành công!");
                                return Ok(def);
                            }
                            catch (Exception e)
                            {
                                def.meta = new Meta(200, "Tải dữ liệu không thành công!");
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
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }

        }

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
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    using (var db = new IOITDataContext())
                    {
                        try
                        {
                            var listFiles = await db.Attactment.Where(e => e.TargetId == id
                            && e.TargetType == (int)Const.TypeAttachment.FILE_DATASET
                            && e.Status != (int)Const.Status.DELETED).ToListAsync();

                            foreach (var file in listFiles)
                            {
                                try
                                {
                                    var downloadRequest = new GetObjectRequest
                                    {
                                        BucketName = bucketName,
                                        Key = file.Url
                                    };

                                    using (var response = await client.GetObjectAsync(downloadRequest))
                                    {
                                        using (var responseStream = response.ResponseStream)
                                        {
                                            var fileInArchive = archive.CreateEntry(Path.GetFileName(file.Name), CompressionLevel.Optimal);
                                            using (var entryStream = fileInArchive.Open())
                                            {
                                                await responseStream.CopyToAsync(entryStream);
                                            }
                                        }

                                    }
                                }
                                catch { };

                            }
                        }
                        catch { }
                        //Lưu lại thông tin mõi khi down
                        //var dataSet = await db.DataSet.Where(e => e.DataSetId == id).FirstOrDefaultAsync();
                        //if (dataSet != null)
                        //{
                        //    dataSet.DownNumber += 1;
                        //    db.DataSet.Update(dataSet);
                        //    //list pvud
                        //    var listAr = await db.DataSetMapping.Where(e => e.DataSetId == id
                        //    && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE).ToListAsync();
                        //    //list lvnc
                        //    var listRa = await db.DataSetMapping.Where(e => e.DataSetId == id
                        //    && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA).ToListAsync();

                        //    //list unit
                        //    var listUnit = await db.DataSetMapping.Where(e => e.DataSetId == id
                        //    && e.TargetType == (int)Const.DataSetMapping.DATA_UNIT).ToListAsync();
                        //    List<DataSetDown> listDataSetDowns = new List<DataSetDown>();
                        //    if (listAr.Count > 0)
                        //    {
                        //        foreach (var itemA in listAr)
                        //        {
                        //            if (listRa.Count > 0)
                        //            {
                        //                foreach (var itemB in listRa)
                        //                {
                        //                    if (listUnit.Count > 0)
                        //                    {
                        //                        foreach (var itemC in listUnit)
                        //                        {
                        //                            DataSetDown dataSetDown = new DataSetDown();
                        //                            dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                            dataSetDown.DataSetId = id;
                        //                            dataSetDown.ApplicationRangeId = itemA.TargetId;
                        //                            dataSetDown.ResearchAreaId = itemB.TargetId;
                        //                            dataSetDown.UnitId = itemC.TargetId;
                        //                            dataSetDown.UpdatedId = idc;
                        //                            dataSetDown.CreatedId = idc;
                        //                            dataSetDown.CreatedAt = DateTime.Now;
                        //                            dataSetDown.UpdatedAt = DateTime.Now;
                        //                            dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                            listDataSetDowns.Add(dataSetDown);
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        DataSetDown dataSetDown = new DataSetDown();
                        //                        dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                        dataSetDown.DataSetId = id;
                        //                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                        //                        dataSetDown.ResearchAreaId = itemB.TargetId;
                        //                        dataSetDown.UnitId = -1;
                        //                        dataSetDown.UpdatedId = idc;
                        //                        dataSetDown.CreatedId = idc;
                        //                        dataSetDown.CreatedAt = DateTime.Now;
                        //                        dataSetDown.UpdatedAt = DateTime.Now;
                        //                        dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                        listDataSetDowns.Add(dataSetDown);
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (listUnit.Count > 0)
                        //                {
                        //                    foreach (var itemC in listUnit)
                        //                    {
                        //                        DataSetDown dataSetDown = new DataSetDown();
                        //                        dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                        dataSetDown.DataSetId = id;
                        //                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                        //                        dataSetDown.ResearchAreaId = -1;
                        //                        dataSetDown.UnitId = itemC.TargetId;
                        //                        dataSetDown.UpdatedId = idc;
                        //                        dataSetDown.CreatedId = idc;
                        //                        dataSetDown.CreatedAt = DateTime.Now;
                        //                        dataSetDown.UpdatedAt = DateTime.Now;
                        //                        dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                        listDataSetDowns.Add(dataSetDown);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    DataSetDown dataSetDown = new DataSetDown();
                        //                    dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                    dataSetDown.DataSetId = id;
                        //                    dataSetDown.ApplicationRangeId = itemA.TargetId;
                        //                    dataSetDown.ResearchAreaId = -1;
                        //                    dataSetDown.UnitId = -1;
                        //                    dataSetDown.UpdatedId = idc;
                        //                    dataSetDown.CreatedId = idc;
                        //                    dataSetDown.CreatedAt = DateTime.Now;
                        //                    dataSetDown.UpdatedAt = DateTime.Now;
                        //                    dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                    listDataSetDowns.Add(dataSetDown);
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (listRa.Count > 0)
                        //        {
                        //            foreach (var itemB in listRa)
                        //            {
                        //                if (listUnit.Count > 0)
                        //                {
                        //                    foreach (var itemC in listUnit)
                        //                    {
                        //                        DataSetDown dataSetDown = new DataSetDown();
                        //                        dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                        dataSetDown.DataSetId = id;
                        //                        dataSetDown.ApplicationRangeId = -1;
                        //                        dataSetDown.ResearchAreaId = itemB.TargetId;
                        //                        dataSetDown.UnitId = itemC.TargetId;
                        //                        dataSetDown.UpdatedId = idc;
                        //                        dataSetDown.CreatedId = idc;
                        //                        dataSetDown.CreatedAt = DateTime.Now;
                        //                        dataSetDown.UpdatedAt = DateTime.Now;
                        //                        dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                        listDataSetDowns.Add(dataSetDown);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    DataSetDown dataSetDown = new DataSetDown();
                        //                    dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                    dataSetDown.DataSetId = id;
                        //                    dataSetDown.ApplicationRangeId = -1;
                        //                    dataSetDown.ResearchAreaId = itemB.TargetId;
                        //                    dataSetDown.UnitId = -1;
                        //                    dataSetDown.UpdatedId = idc;
                        //                    dataSetDown.CreatedId = idc;
                        //                    dataSetDown.CreatedAt = DateTime.Now;
                        //                    dataSetDown.UpdatedAt = DateTime.Now;
                        //                    dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                    listDataSetDowns.Add(dataSetDown);
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (listUnit.Count > 0)
                        //            {
                        //                foreach (var itemC in listUnit)
                        //                {
                        //                    DataSetDown dataSetDown = new DataSetDown();
                        //                    dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                    dataSetDown.DataSetId = id;
                        //                    dataSetDown.ApplicationRangeId = -1;
                        //                    dataSetDown.ResearchAreaId = -1;
                        //                    dataSetDown.UnitId = itemC.TargetId;
                        //                    dataSetDown.UpdatedId = idc;
                        //                    dataSetDown.CreatedId = idc;
                        //                    dataSetDown.CreatedAt = DateTime.Now;
                        //                    dataSetDown.UpdatedAt = DateTime.Now;
                        //                    dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                    listDataSetDowns.Add(dataSetDown);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                DataSetDown dataSetDown = new DataSetDown();
                        //                dataSetDown.DataSetDownId = Guid.NewGuid();
                        //                dataSetDown.DataSetId = id;
                        //                dataSetDown.ApplicationRangeId = -1;
                        //                dataSetDown.ResearchAreaId = -1;
                        //                dataSetDown.UnitId = -1;
                        //                dataSetDown.UpdatedId = idc;
                        //                dataSetDown.CreatedId = idc;
                        //                dataSetDown.CreatedAt = DateTime.Now;
                        //                dataSetDown.UpdatedAt = DateTime.Now;
                        //                dataSetDown.Status = (int)Const.Status.NORMAL;
                        //                listDataSetDowns.Add(dataSetDown);
                        //            }
                        //        }
                        //    }

                        //    await db.DataSetDown.AddRangeAsync(listDataSetDowns);
                        //    await db.SaveChangesAsync();
                        //}

                        outStream.Position = 0;
                        return File(outStream.ToArray(), "application/zip", "dataset.zip");
                    }
                }
            }
        }

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
                    //var dataSet = await db.DataSet.Where(e => e.DataSetId == id).FirstOrDefaultAsync();
                    //if (dataSet != null)
                    //{
                    //    dataSet.DownNumber += 1;
                    //    db.DataSet.Update(dataSet);
                    //    //list pvud
                    //    var listAr = await db.DataSetMapping.Where(e => e.DataSetId == id
                    //    && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE).ToListAsync();
                    //    //list lvnc
                    //    var listRa = await db.DataSetMapping.Where(e => e.DataSetId == id
                    //    && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA).ToListAsync();

                    //    //list unit
                    //    var listUnit = await db.DataSetMapping.Where(e => e.DataSetId == id
                    //    && e.TargetType == (int)Const.DataSetMapping.DATA_UNIT).ToListAsync();
                    //    List<DataSetDown> listDataSetDowns = new List<DataSetDown>();
                    //    if (listAr.Count > 0)
                    //    {
                    //        foreach (var itemA in listAr)
                    //        {
                    //            if (listRa.Count > 0)
                    //            {
                    //                foreach (var itemB in listRa)
                    //                {
                    //                    if (listUnit.Count > 0)
                    //                    {
                    //                        foreach (var itemC in listUnit)
                    //                        {
                    //                            DataSetDown dataSetDown = new DataSetDown();
                    //                            dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                            dataSetDown.DataSetId = id;
                    //                            dataSetDown.ApplicationRangeId = itemA.TargetId;
                    //                            dataSetDown.ResearchAreaId = itemB.TargetId;
                    //                            dataSetDown.UnitId = itemC.TargetId;
                    //                            dataSetDown.UpdatedId = idc;
                    //                            dataSetDown.CreatedId = idc;
                    //                            dataSetDown.CreatedAt = DateTime.Now;
                    //                            dataSetDown.UpdatedAt = DateTime.Now;
                    //                            dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                            listDataSetDowns.Add(dataSetDown);
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        DataSetDown dataSetDown = new DataSetDown();
                    //                        dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                        dataSetDown.DataSetId = id;
                    //                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                    //                        dataSetDown.ResearchAreaId = itemB.TargetId;
                    //                        dataSetDown.UnitId = -1;
                    //                        dataSetDown.UpdatedId = idc;
                    //                        dataSetDown.CreatedId = idc;
                    //                        dataSetDown.CreatedAt = DateTime.Now;
                    //                        dataSetDown.UpdatedAt = DateTime.Now;
                    //                        dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                        listDataSetDowns.Add(dataSetDown);
                    //                    }
                    //                }
                    //            }
                    //            else
                    //            {
                    //                if (listUnit.Count > 0)
                    //                {
                    //                    foreach (var itemC in listUnit)
                    //                    {
                    //                        DataSetDown dataSetDown = new DataSetDown();
                    //                        dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                        dataSetDown.DataSetId = id;
                    //                        dataSetDown.ApplicationRangeId = itemA.TargetId;
                    //                        dataSetDown.ResearchAreaId = -1;
                    //                        dataSetDown.UnitId = itemC.TargetId;
                    //                        dataSetDown.UpdatedId = idc;
                    //                        dataSetDown.CreatedId = idc;
                    //                        dataSetDown.CreatedAt = DateTime.Now;
                    //                        dataSetDown.UpdatedAt = DateTime.Now;
                    //                        dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                        listDataSetDowns.Add(dataSetDown);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    DataSetDown dataSetDown = new DataSetDown();
                    //                    dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                    dataSetDown.DataSetId = id;
                    //                    dataSetDown.ApplicationRangeId = itemA.TargetId;
                    //                    dataSetDown.ResearchAreaId = -1;
                    //                    dataSetDown.UnitId = -1;
                    //                    dataSetDown.UpdatedId = idc;
                    //                    dataSetDown.CreatedId = idc;
                    //                    dataSetDown.CreatedAt = DateTime.Now;
                    //                    dataSetDown.UpdatedAt = DateTime.Now;
                    //                    dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                    listDataSetDowns.Add(dataSetDown);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (listRa.Count > 0)
                    //        {
                    //            foreach (var itemB in listRa)
                    //            {
                    //                if (listUnit.Count > 0)
                    //                {
                    //                    foreach (var itemC in listUnit)
                    //                    {
                    //                        DataSetDown dataSetDown = new DataSetDown();
                    //                        dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                        dataSetDown.DataSetId = id;
                    //                        dataSetDown.ApplicationRangeId = -1;
                    //                        dataSetDown.ResearchAreaId = itemB.TargetId;
                    //                        dataSetDown.UnitId = itemC.TargetId;
                    //                        dataSetDown.UpdatedId = idc;
                    //                        dataSetDown.CreatedId = idc;
                    //                        dataSetDown.CreatedAt = DateTime.Now;
                    //                        dataSetDown.UpdatedAt = DateTime.Now;
                    //                        dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                        listDataSetDowns.Add(dataSetDown);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    DataSetDown dataSetDown = new DataSetDown();
                    //                    dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                    dataSetDown.DataSetId = id;
                    //                    dataSetDown.ApplicationRangeId = -1;
                    //                    dataSetDown.ResearchAreaId = itemB.TargetId;
                    //                    dataSetDown.UnitId = -1;
                    //                    dataSetDown.UpdatedId = idc;
                    //                    dataSetDown.CreatedId = idc;
                    //                    dataSetDown.CreatedAt = DateTime.Now;
                    //                    dataSetDown.UpdatedAt = DateTime.Now;
                    //                    dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                    listDataSetDowns.Add(dataSetDown);
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (listUnit.Count > 0)
                    //            {
                    //                foreach (var itemC in listUnit)
                    //                {
                    //                    DataSetDown dataSetDown = new DataSetDown();
                    //                    dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                    dataSetDown.DataSetId = id;
                    //                    dataSetDown.ApplicationRangeId = -1;
                    //                    dataSetDown.ResearchAreaId = -1;
                    //                    dataSetDown.UnitId = itemC.TargetId;
                    //                    dataSetDown.UpdatedId = idc;
                    //                    dataSetDown.CreatedId = idc;
                    //                    dataSetDown.CreatedAt = DateTime.Now;
                    //                    dataSetDown.UpdatedAt = DateTime.Now;
                    //                    dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                    listDataSetDowns.Add(dataSetDown);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                DataSetDown dataSetDown = new DataSetDown();
                    //                dataSetDown.DataSetDownId = Guid.NewGuid();
                    //                dataSetDown.DataSetId = id;
                    //                dataSetDown.ApplicationRangeId = -1;
                    //                dataSetDown.ResearchAreaId = -1;
                    //                dataSetDown.UnitId = -1;
                    //                dataSetDown.UpdatedId = idc;
                    //                dataSetDown.CreatedId = idc;
                    //                dataSetDown.CreatedAt = DateTime.Now;
                    //                dataSetDown.UpdatedAt = DateTime.Now;
                    //                dataSetDown.Status = (int)Const.Status.NORMAL;
                    //                listDataSetDowns.Add(dataSetDown);
                    //            }
                    //        }
                    //    }

                    //    await db.DataSetDown.AddRangeAsync(listDataSetDowns);
                    //    await db.SaveChangesAsync();
                    //}

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
    }
}
