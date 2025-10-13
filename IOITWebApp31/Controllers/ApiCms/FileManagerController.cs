using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiCms
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("file-manager", "file-manager");
        private static string functionCode = "QLF";
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }
        public FileManagerController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpGet("GetFolders")]
        public async Task<ActionResult> GetFolders([FromQuery] Parameters paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }


            if (Utils.ConvertUrlpath(paging.currentFolder).Contains("../") || Utils.ConvertUrlpath(paging.currentFolder).Contains("..\\") || Utils.ConvertUrlpath(paging.currentFolder).IndexOfAny(Path.GetInvalidPathChars()) > -1)
            {
                def.meta = new Meta(222, "Thư mục không hợp lệ!");
                return Ok(def);
            }

            try
            {
                string rootUploads = _configuration["AppSettings:rootUploads"];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string folderName = rootUploads + "/" + paging.type + paging.currentFolder;
                string path = Path.Combine(webRootPath, folderName);
                GetFolders getFolders = new GetFolders();
                getFolders.resourceType = paging.type;
                CurrentFolder currentFolder = new CurrentFolder();
                currentFolder.path = paging.currentFolder;
                currentFolder.url = "/" + rootUploads + "/" + paging.type + paging.currentFolder;
                currentFolder.acl = 1023;
                getFolders.currentFolder = currentFolder;

                //Lấy danh sách folder
                var directories = Directory.GetDirectories(path);

                List<Folders> folders = new List<Folders>();
                foreach (var item in directories)
                {
                    Folders folder = new Folders();
                    folder.name = item.Split('/').LastOrDefault();

                    var directoriesChild = Directory.GetDirectories(item);

                    folder.hasChildren = directoriesChild.Count() > 0 ? true : false;
                    folder.acl = 1023;
                    folders.Add(folder);
                }
                getFolders.folders = folders;

                def.meta = new Meta(200, "Success");
                def.data = getFolders;
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpGet("GetFiles")]
        public async Task<ActionResult> GetFiles([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();

            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }


            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    string rootUploads = _configuration["AppSettings:rootUploads"];
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = Path.Combine(webRootPath, rootUploads);

                    //check extention
                    string extention = "";
                    string extention1 = ".jpg,.jpeg,.gif,.png,.bmp,.svg";
                    string extention2 = ".mp3,.aiff,.wav,.dsd,.flac,.pcm";
                    string extention3 = ".mp4,.avi, .flv,.mkv,.webm,.m4v,.mpc,.mpeg";
                    string extention4 = ".doc,.docx,.xls,.xlsx,.pdf,.rtf,.ppt,.pptx,.txt,.csv";
                    string extention5 = ".rar,.zip,.7z,.gz,.gzip,.rm";
                    if (paging.select == "1")
                    {
                        extention = extention1;
                    }
                    else if (paging.select == "2")
                    {
                        extention = extention2;
                    }
                    else if (paging.select == "3")
                    {
                        extention = extention3;
                    }
                    else if (paging.select == "4")
                    {
                        extention = extention4;
                    }
                    else if (paging.select == "5")
                    {
                        extention = extention5;
                    }

                    List<Files> files = new List<Files>();
                    GetListFolder(path, files, "", extention);
                    var data = files.AsQueryable();
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("date desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("date desc");
                        }
                    }

                    foreach (var file in data)
                    {
                        try
                        {
                            string[] str1 = extention1.Split(',');
                            string[] str2 = extention2.Split(',');
                            string[] str3 = extention3.Split(',');
                            string[] str4 = extention4.Split(',');
                            string[] str5 = extention5.Split(',');
                            if (str1.Contains(file.extension))
                            {
                                file.type = 1;
                                try
                                {
                                    Image img = Image.FromFile(file.path + "/" + file.name);
                                    file.width = img.Width;
                                    file.height = img.Height;
                                    //img.
                                    img.Dispose();
                                }
                                catch { };
                            }
                            else if (str2.Contains(file.extension))
                            {
                                file.type = 2;
                            }
                            else if (str3.Contains(file.extension))
                            {
                                file.type = 3;
                            }
                            else if (str4.Contains(file.extension))
                            {
                                file.type = 4;
                            }
                            else if (str5.Contains(file.extension))
                            {
                                file.type = 5;
                            }
                            file.path = file.url + "/" + file.name;
                            file.alt = file.name;
                            file.note = file.name;
                            //check xem trong attactment có ko để lấy thêm thông tin
                            var checkAt = db.Attactment.Where(e => e.Name == file.name
                                    && e.TargetType == 99 && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (checkAt != null)
                            {
                                file.alt = checkAt.Url;
                                file.note = checkAt.Thumb;
                                //check ng upload
                                var user = await db.User.Where(e => e.UserId == checkAt.CreatedId).FirstOrDefaultAsync();
                                file.userName = user != null ? user.FullName : "admin";
                            }

                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                    def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }

            //try
            //{
            //    string rootUploads = _configuration["AppSettings:rootUploadImages"];
            //    string webRootPath = _hostingEnvironment.WebRootPath;
            //    string path = Path.Combine(webRootPath, rootUploads);

            //    List<Files> files = new List<Files>();
            //    GetListFolder(path, files, "", paging.select);
            //    def.meta = new Meta(200, "Success");
            //    def.data = files;
            //    return Ok(def);
            //}
            //catch (Exception e)
            //{
            //    log.Error("Error:" + e);
            //    def.meta = new Meta(500, "Internal Server Error");
            //    return Ok(def);
            //}
        }

        public void GetListFolder(string path, List<Files> listFiles, string folder, string extension)
        {
            try
            {

                if (Utils.ConvertUrlpath(path).Contains("../") || Utils.ConvertUrlpath(path).Contains("..\\") || Utils.ConvertUrlpath(path).IndexOfAny(Path.GetInvalidPathChars()) > -1)
                {

                }
                else
                {
                    GetListFile(path, listFiles, folder, extension);
                    //Lấy danh sách folder
                    var directories = Directory.GetDirectories(path);
                    foreach (var item in directories)
                    {
                        if (!item.Split('\\').LastOrDefault().StartsWith("_thumb"))
                        {
                            //Folders folder = new Folders();
                            //folder.name = item.Split('/').LastOrDefault();
                            string link = folder + "/" + item.Split('\\').LastOrDefault();
                            //folder += "/" + item.Split('\\').LastOrDefault();
                            //GetListFile(item, listFiles, link, extension);
                            GetListFolder(item, listFiles, link, extension);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
            }

        }

        public void GetListFile(string path, List<Files> files, string folder, string extension)
        {
            if (Utils.ConvertUrlpath(path).Contains("../") || path.Contains("..\\") || Utils.ConvertUrlpath(path).IndexOfAny(Path.GetInvalidPathChars()) > -1)
            {
                // do nothing
            }
            else
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(path);
                    ArrayList Files = new ArrayList();
                    //
                    if (extension == "")
                    {
                        FileInfo[] listFiles = d.GetFiles(); //Getting Text files
                        Files.AddRange(listFiles);
                    }
                    else
                    {
                        string[] listE = extension.Split(',');
                        foreach (var item in listE)
                        {
                            FileInfo[] listFiles = d.GetFiles("*" + item.Trim());
                            Files.AddRange(listFiles);
                        }
                    }
                    foreach (FileInfo item in Files)
                    {
                        //check xem trùng tên file trong 1 folder ko
                        //var check = files.Where(e => e.name.Trim().ToLower().Equals(item.Name.Trim().ToLower())).FirstOrDefault();
                        //if (check == null)
                        //{
                        Files file = new Files();
                        file.name = item.Name;
                        string date = "";
                        date += item.CreationTime.Year;
                        date += item.CreationTime.Month < 10 ? ("0" + item.CreationTime.Month) : item.CreationTime.Month + "";
                        date += item.CreationTime.Day < 10 ? ("0" + item.CreationTime.Day) : item.CreationTime.Day + "";
                        date += item.CreationTime.Hour < 10 ? ("0" + item.CreationTime.Hour) : item.CreationTime.Hour + "";
                        date += item.CreationTime.Minute < 10 ? ("0" + item.CreationTime.Minute) : item.CreationTime.Minute + "";
                        date += item.CreationTime.Second < 10 ? ("0" + item.CreationTime.Second) : item.CreationTime.Second + "";
                        file.date = date;
                        file.url = folder;
                        file.size = (int)item.Length / 1024;
                        file.type = 5;
                        file.extension = item.Extension;
                        file.path = path;
                        file.dateCreate = item.CreationTime;

                        files.Add(file);
                        //}
                    }
                }
                catch (Exception e)
                {
                    log.Error("Error:" + e);
                }
            }

        }

        [HttpPost("UpdateInfoFile")]
        public async Task<ActionResult> UpdateInfoFile([FromBody] Attactment data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    //Check xem có trong attactment chưa, chưa có thì thêm mới, có rùi thì update
                    var checkAt = db.Attactment.Where(e => e.Name == data.Name
                    && e.TargetType == 99 && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkAt == null)
                    {
                        //Thêm vào bàng Attacment
                        Attactment attactment = new Attactment();
                        attactment.Name = data.Name;
                        attactment.Url = data.Url;
                        attactment.Thumb = data.Thumb;
                        attactment.Note = data.Note;
                        attactment.TargetId = data.TargetId;
                        attactment.TargetType = 99;
                        attactment.CreatedAt = DateTime.Now;
                        attactment.CreatedId = userId;
                        attactment.Status = (int)Const.Status.NORMAL;
                        await db.Attactment.AddAsync(attactment);
                    }
                    else
                    {
                        checkAt.Url = data.Url;
                        checkAt.Thumb = data.Thumb;
                        checkAt.UpdatedId = userId;
                        checkAt.Status = (int)Const.Status.NORMAL;
                        db.Attactment.Update(checkAt);
                    }
                    await db.SaveChangesAsync();
                }
                def.meta = new Meta(200, "Success");
                def.data = data;
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpPost("DeleteFile")]
        public async Task<ActionResult> DeleteFile([FromBody] Parameters paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                //delete file
                string rootUploads = _configuration["AppSettings:rootUploads"];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string fileName = rootUploads + "/" + paging.currentFolder + "/" + paging.fileName;


                if (Utils.ConvertUrlpath(paging.fileName).Contains("../") || Utils.ConvertUrlpath(paging.fileName).Contains("..\\") || Utils.ConvertUrlpath(paging.fileName).IndexOfAny(Path.GetInvalidPathChars()) > -1)
                {
                    def.meta = new Meta(222, "File name không hợp lẹ!");
                    return Ok(def);
                }

                if (Utils.ConvertUrlpath(paging.currentFolder).Contains("../") || Utils.ConvertUrlpath(paging.currentFolder).Contains("..\\") || Utils.ConvertUrlpath(paging.currentFolder).IndexOfAny(Path.GetInvalidPathChars()) > -1)
                {
                    def.meta = new Meta(222, "Thư mục không hợp lệ!");
                    return Ok(def);
                }


                var ext = Path.GetExtension(fileName).TrimStart('.');
                var allow = "7z,aiff,asf,avi,bmp,csv,doc,docx,fla,flv,gif,gz,gzip,jpeg,jpg,mid,mov,mp3,mp4,mpc,mpeg,mpg,ods,odt,pdf,png,ppt,pptx,pxd,qt,ram,rar,rm,rmi,rmvb,rtf,sdc,sitd,swf,sxc,sxw,tar,tgz,tif,tiff,txt,vsd,wav,wma,wmv,xls,xlsx,zip,bmp,gif,jpeg,jpg,png";
                if (!allow.Split(',', StringSplitOptions.RemoveEmptyEntries).Contains(ext))
                {

                    def.meta = new Meta(222, "File mở rộng không hợp lệ!");
                    return Ok(def);

                }

                string path = Path.Combine(webRootPath, fileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                DeleteFolder deleteFolder = new DeleteFolder();
                deleteFolder.resourceType = paging.type;
                CurrentFolder currentFolder = new CurrentFolder();
                currentFolder.path = path;
                currentFolder.url = paging.currentFolder;
                currentFolder.acl = 1023;
                deleteFolder.currentFolder = currentFolder;
                deleteFolder.deleted = 1;
                using (var db = new IOITDataContext())
                {
                    //Check xem có trong attactment chưa, có rùi thì xóa
                    var checkAt = db.Attactment.Where(e => e.Name == paging.fileName
                    && e.TargetType == 99 && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkAt != null)
                    {
                        checkAt.UpdatedId = userId;
                        checkAt.Status = (int)Const.Status.DELETED;
                        db.Attactment.Update(checkAt);
                    }
                }
                log.Error("Delete File:" + paging.fileName + " - " + userId);
                def.meta = new Meta(200, "Success");
                def.data = deleteFolder;
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

    }
}
