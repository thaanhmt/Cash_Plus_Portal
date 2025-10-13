using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using log4net;
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
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("download", "download");

        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }

        public DownloadController(IHostingEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpGet("downloadFiles/{id}/{idc}")]
        public async Task<IActionResult> DownloadFiles(long id, int idc)
        {
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    string folderName = _configuration["AppSettings:rootUploadsDataFiles"];
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string dataPath = Path.Combine(webRootPath, folderName);

                    var files = new List<string>();
                    using (var db = new IOITDataContext())
                    {
                        try
                        {
                            var listFiles = await db.Attactment.Where(e => e.TargetId == id
                            && e.TargetType == (int)Const.TypeAttachment.FILE_DATASET
                            && e.Status != (int)Const.Status.DELETED).ToListAsync();
                            foreach (var item in listFiles)
                            {
                                string file = dataPath + "/" + item.Name;
                                files.Add(file);
                            }

                            foreach (var file in files)
                            {
                                try
                                {
                                    var fileInArchive = archive.CreateEntry(Path.GetFileName(file), CompressionLevel.Optimal);
                                    using (var entryStream = fileInArchive.Open())
                                    {
                                        using (var fileCompressionStream = new MemoryStream(System.IO.File.ReadAllBytes(file)))
                                        {
                                            await fileCompressionStream.CopyToAsync(entryStream);
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
                }

                outStream.Position = 0;
                return File(outStream.ToArray(), "application/zip", "files.zip");
            }
        }

        [HttpPost("downloadOneFile/{id}")]
        public async Task<HttpResponseMessage> DownloadOneFile(Guid id)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    string fileName = "";

                    Attactment attactment = await db.Attactment.Where(a => a.AttactmentId == id).FirstOrDefaultAsync();
                    if (attactment != null)
                    {
                        string folderName = _configuration["AppSettings:rootUploadsDataFiles"];
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string dataPath = Path.Combine(webRootPath, folderName);
                        fileName = dataPath + "/" + attactment.Name;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                            {

                                byte[] bytes = new byte[file.Length];
                                file.Read(bytes, 0, (int)file.Length);
                                ms.Write(bytes, 0, (int)file.Length);

                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                                    {
                                        Content = new ByteArrayContent(ms.ToArray())
                                    };
                                    response.Content.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type,x-filename");
                                    response.Content.Headers.Add("Access-Control-Expose-Headers", "Authorization,Content-Type,x-filename");
                                    response.Content.Headers.Add("x-filename", fileName);
                                    response.Content.Headers.ContentType = new MediaTypeHeaderValue
                                           ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                                    response.Content.Headers.ContentDisposition =
                                           new ContentDispositionHeaderValue("attachment")
                                           {
                                               FileName = fileName
                                           };

                                    return response;
                                }
                                var response1 = new HttpResponseMessage(HttpStatusCode.NotFound);
                                return response1;
                            }
                        }
                    }
                    else
                    {
                        var response3 = new HttpResponseMessage(HttpStatusCode.NotFound);
                        return response3;
                    }
                };
            }
            catch (Exception ex)
            {
                var response2 = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response2;
            }
        }

    }
}
