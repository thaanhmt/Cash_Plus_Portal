using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("upload", "upload");

        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }

        public UploadController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
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

                        IList<string> AllowedFileExtensions = new List<string> { ".pdf", ".csv", ".kml", ".shp", ".api" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Bạn vui lòng tải tài liệu có định dạng: .pdf, .csv, .kml, .shp, .api");
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
                            string folderName = _configuration["AppSettings:rootUploadsDataFiles"];
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }

                            DateTime now = DateTime.Now;

                            var nameReplate = name.Replace(" ", "_");
                            string img = nameReplate + "_" + now.ToString("yyyyMMddHHmmssfff") + extension;

                            string fullPath = Path.Combine(newPath, img);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                postedFile.CopyTo(stream);
                            }
                            attactment.Name = img;
                            attactment.Storage = postedFile.Length;
                            attactment.ExtensionName = extension;
                            if (extension == ".pdf")
                                attactment.Extension = 1;
                            else if (extension == ".csv")
                                attactment.Extension = 2;
                            else if (extension == ".kml")
                                attactment.Extension = 3;
                            else if (extension == ".shp")
                                attactment.Extension = 4;
                            else if (extension == ".api")
                                attactment.Extension = 5;
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
        [HttpPost]
        [Route("uploadImage/{id}")]
        public IActionResult Upload(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;
                var httpRequest = Request.Form.Files;
                List<String> files = new List<string>();
                foreach (var file in httpRequest)
                {
                    var postedFile = httpRequest[i];
                    if (postedFile != null && postedFile.Length > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 100; //Size = 100 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có định dạng: .jpg, .jpeg, .gif, .png, .bmp.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length > MaxContentLength)
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng nhỏ hơn 100MB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length < 0)
                        {
                            var message = string.Format("Bạn vui lòng upload hình ảnh có dung lượng lớn hơn 1KB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            string folderName = _configuration["AppSettings:rootUploads"];
                            if (id == 6)
                                folderName = _configuration["AppSettings:rootUploadsAvatas"];
                            else
                            {
                                //Công theo năm tháng
                                string folderName2 = "/images/" + DateTime.Now.Year + "/" + DateTime.Now.Month;
                                folderName = folderName + "/" + folderName2;
                            }
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }

                            DateTime now = DateTime.Now;
                            string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                            //Lấy danh sách tạo ảnh thumb
                            using (var db = new IOITDataContext())
                            {
                                string fullPath = Path.Combine(newPath, img);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    postedFile.CopyTo(stream);

                                    var Thumb = db.ConfigThumb.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type == id).FirstOrDefault();
                                    int width = Thumb != null ? Thumb.Width : 300;

                                    var image = Bitmap.FromStream(stream);
                                    string rel = @"_thumb/" + img;
                                    string thumbPath = Path.Combine(newPath, rel);
                                    createThumb(width, image, thumbPath, extension);
                                }
                            }
                            files.Add(img);
                        }
                    }
                }
                def.meta = new Meta(200, "Success");
                def.data = files;
                return Ok(def);
            }
            catch (Exception ex)
            {
                def.meta = new Meta(400, "Có lỗi xẩy ra, vui lòng thực hiện lại!");
                return Ok(def);
            }
        }

        [DisableRequestSizeLimit]
        [HttpPost("uploadMedia/{type}")]
        public async Task<IActionResult> UploadMedia(int type)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            //{
            //    def.meta = new Meta(222, "No permission");
            //    return Ok(def);
            //}
            try
            {
                int i = 0;
                var httpRequest = Request.Form.Files;
                List<String> files = new List<string>();
                foreach (var file in httpRequest)
                {
                    var postedFile = httpRequest[i];
                    if (postedFile != null && postedFile.Length > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 512; //Size = 600 MB  

                        IList<string> AllowedFileExtensionsImg = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp", ".svg" };
                        IList<string> AllowedFileExtensionsFile = new List<string> { ".mp4", ".mp3", ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".rtf", ".ppt", ".pptx", ".txt", ".csv" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        string typeFile = "";
                        int targetId = 1;
                        if (AllowedFileExtensionsImg.Contains(extension))
                        {
                            typeFile = "images";
                            targetId = 1;
                        }
                        if (AllowedFileExtensionsFile.Contains(extension))
                        {
                            typeFile = "files";
                            targetId = 2;
                        }
                        if (typeFile == "")
                        {
                            var message = string.Format("Bạn vui lòng upload file có định dạng: .jpg, .jpeg, .gif, .png, .bmp, .svg");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length > MaxContentLength)
                        {
                            var message = string.Format("Bạn vui lòng upload file có dung lượng nhỏ hơn 512MB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else if (postedFile.Length < 0)
                        {
                            var message = string.Format("Bạn vui lòng upload file có dung lượng lớn hơn 1KB.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            string folderName = _configuration["AppSettings:rootUploads"];
                            //Tính type
                            //Công theo năm tháng
                            string folderName2 = typeFile + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month;
                            folderName = folderName + "/" + folderName2;
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }
                            //Bỏ ký tự đặc biệt và cắt bớt độ dài tên
                            string img = Utils.NonUnicode(name).Trim();
                            if (img.Length > 50)
                                img = img.Substring(0, 50);
                            int k = 1;
                            string nameFile = img + extension;
                            List<Files> listFiles = new List<Files>();
                            GetListFile(newPath, listFiles);
                            var check = listFiles.Where(e => e.name == nameFile).FirstOrDefault();
                            while (check != null)
                            {
                                nameFile = img + "(" + k + ")" + extension;
                                check = listFiles.Where(e => e.name == nameFile).FirstOrDefault();
                                k++;
                            }
                            //DateTime now = DateTime.Now;
                            //string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension;
                            //Lấy danh sách tạo ảnh thumb
                            using (var db = new IOITDataContext())
                            {
                                string fullPath = Path.Combine(newPath, nameFile);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    postedFile.CopyTo(stream);

                                    if (typeFile == "images")
                                    {
                                        var Thumb = db.ConfigThumb.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type == type).FirstOrDefault();
                                        int width = Thumb != null ? Thumb.Width : 300;

                                        var image = Bitmap.FromStream(stream);
                                        string rootUploads = _configuration["AppSettings:rootUploads"];
                                        string rel = rootUploads + "/_thumbs/" + folderName2 + "/" + nameFile;
                                        string thumbPath = Path.Combine(webRootPath, rel);
                                        createThumb(width, image, thumbPath, extension);
                                    }
                                }

                                img = folderName2 + "/" + nameFile;
                                files.Add(img);

                                //Check xem có trong attactment chưa, chưa có thì thêm mới
                                var checkAt = db.Attactment.Where(e => e.Name == nameFile
                                && e.TargetType == 99 && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (checkAt == null)
                                {
                                    //Thêm vào bàng Attacment
                                    Attactment attactment = new Attactment();
                                    attactment.AttactmentId = Guid.NewGuid();
                                    attactment.Name = nameFile;
                                    attactment.Url = postedFile.FileName;
                                    attactment.Thumb = postedFile.FileName;
                                    if (name.Length > 200)
                                        attactment.Note = name.Substring(0, 200);
                                    else
                                        attactment.Note = name;
                                    attactment.TargetId = targetId;
                                    attactment.TargetType = 99;
                                    attactment.CreatedAt = DateTime.Now;
                                    attactment.CreatedId = userId;
                                    attactment.Status = (int)Const.Status.NORMAL;
                                    await db.Attactment.AddAsync(attactment);
                                    await db.SaveChangesAsync();
                                }
                            }
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
                log.Error(ex);
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        public static void createThumb(int thumbWidth, Image image, string thumbPath, string file_type)
        {
            //extract path
            int lastIndex = thumbPath.LastIndexOf("/");
            string path = thumbPath.Substring(0, lastIndex);
            Directory.CreateDirectory(path);
            //using (var image = new Bitmap(Image.FromFile(filePath)))
            //{
            //MemoryStream ms = new MemoryStream(input);
            //Image returnImage = Image.FromStream(ms);
            //Image image = Image.FromStream(ms);
            double srcWidth = image.Width;
            double srcHeight = image.Height;
            double thumbHeight = (srcHeight / srcWidth) * thumbWidth;
            Bitmap bmp = new Bitmap(thumbWidth, (int)thumbHeight);

            Graphics gr = Graphics.FromImage(bmp);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.InterpolationMode = InterpolationMode.High;

            Rectangle rectDestination = new Rectangle(0, 0, thumbWidth, (int)thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, (int)srcWidth, (int)srcHeight, GraphicsUnit.Pixel);
            if (file_type.ToLower() == ".jpg" || file_type.ToLower() == ".jpeg")
                bmp.Save(thumbPath, ImageFormat.Jpeg);
            else if (file_type.ToLower() == ".png")
                bmp.Save(thumbPath, ImageFormat.Png);
            else if (file_type.ToLower() == ".gif")
                bmp.Save(thumbPath, ImageFormat.Gif);
            else
                bmp.Save(thumbPath, ImageFormat.Jpeg);

            bmp.Dispose();
            image.Dispose();
            //}
        }

        public void GetListFile(string path, List<Files> files)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(path);
                FileInfo[] Files = d.GetFiles(); //Getting Text files

                foreach (FileInfo item in Files)
                {
                    //check xem trùng tên file ko
                    var check = files.Where(e => e.name.Trim().ToLower().Equals(item.Name.Trim().ToLower())).FirstOrDefault();
                    if (check == null)
                    {
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
                        file.url = path;
                        file.size = (int)item.Length / 1024;
                        files.Add(file);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
            }

        }

        //[HttpPost]
        //[Route("api/upload/uploadImage/{id}")]
        //public IActionResult UploadImage(int id)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        List<String> files = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {
        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();
        //                    byte[] fileData = null;
        //                    using (var binaryReader = new BinaryReader(postedFile.InputStream))
        //                    {
        //                        DateTime now = DateTime.Now;
        //                        string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; ;
        //                        //Lấy danh sách tạo ảnh thumb
        //                        using (var db = new CNTTVNData())
        //                        {
        //                            fileData = binaryReader.ReadBytes(postedFile.ContentLength);

        //                            //Tạo ảnh gốc
        //                            var filePath = HttpContext.Current.Server.MapPath("~/uploads/" + img);
        //                            Utils.createFile(filePath, fileData, extension);

        //                            //Tạo ảnh thumb
        //                            var listThumb = db.ConfigThumbs.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID && e.Type==id).ToList();
        //                            foreach (var item in listThumb)
        //                            {
        //                                filePath = HttpContext.Current.Server.MapPath("~/uploads/thumb/_thumb" + item.Width + "/" + img);
        //                                Utils.createThumb(item.Width, filePath, fileData, extension);
        //                            }
        //                            files.Add(img);
        //                        }
        //                    }
        //                //}
        //            }
        //        }
        //        def.meta = new Meta(200, "Success");
        //        def.data = files;
        //        return Ok(def);
        //    }
        //    catch (Exception ex)
        //    {
        //        def.meta = new Meta(400, "Bad request");
        //        return Ok(def);
        //    }
        //}

        //[HttpPost]
        //[Route("api/upload/uploadImage")]
        //public IHttpActionResult UploadImage()
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        List<String> files = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {
        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();
        //                byte[] fileData = null;
        //                using (var binaryReader = new BinaryReader(postedFile.InputStream))
        //                {
        //                    DateTime now = DateTime.Now;
        //                    string img = name + "_" + now.ToString("yyyyMMddHHmmssfff") + extension; ;
        //                    //Lấy danh sách tạo ảnh thumb
        //                    using (var db = new CNTTVNData())
        //                    {
        //                        fileData = binaryReader.ReadBytes(postedFile.ContentLength);

        //                        //Tạo ảnh gốc
        //                        var filePath = HttpContext.Current.Server.MapPath("~/uploads/" + img);
        //                        Utils.createFile(filePath, fileData, extension);

        //                        var listThumb = db.ConfigThumbs.Where(e => e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID).ToList();

        //                        foreach (var item in listThumb)
        //                        {
        //                            filePath = HttpContext.Current.Server.MapPath("~/uploads/thumb/_thumb" + item.Width + "/" + img);
        //                            Utils.createThumb(item.Width, filePath, fileData, extension);
        //                        }
        //                        files.Add(img);
        //                    }
        //                }
        //                //}
        //            }
        //        }
        //        def.meta = new Meta(200, "Success");
        //        def.data = files;
        //        return Ok(def);
        //    }
        //    catch (Exception ex)
        //    {
        //        def.meta = new Meta(400, "Bad request");
        //        return Ok(def);
        //    }
        //}

        //id: folder
        //[HttpPost]
        //[Route("api/uploadFiles/{id}")]
        //public async Task<IHttpActionResult> uploadFiles(int id = 0)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        string domainFile = ConfigurationManager.AppSettings["domainFile"].ToString();

        //        var httpRequest = HttpContext.Current.Request;
        //        List<String> files = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {

        //                int MaxContentLength = 1024 * 1024 * 1000;  //Size = 1G 

        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();

        //                if (postedFile.ContentLength > MaxContentLength)
        //                {
        //                    var message = string.Format("Bạn vui lòng upload file có dung lượng nhỏ hơn 1GB.");
        //                    def.meta = new Meta(600, message);
        //                    return Ok(def);
        //                }
        //                else
        //                {
        //                    byte[] fileData = null;
        //                    using (var binaryReader = new BinaryReader(postedFile.InputStream))
        //                    {
        //                        DateTime now = DateTime.Now;
        //                        fileData = binaryReader.ReadBytes(postedFile.ContentLength);
        //                        string url = "";
        //                        string urlSave = "";
        //                        if (id == (int)Const.TypeFolder.PROJECT)
        //                        {
        //                            url = "/project";
        //                            urlSave += "\\project";
        //                        }
        //                        else if (id == (int)Const.TypeFolder.BID_PACKAGE)
        //                        {
        //                            url = "/bidpackage";
        //                            urlSave += "\\bidpackage";
        //                        }
        //                        else if (id == (int)Const.TypeFolder.CONTRACT)
        //                        {
        //                            url = "/contract";
        //                            urlSave += "\\contract";
        //                        }
        //                        else if (id == (int)Const.TypeFolder.CASH)
        //                        {
        //                            url = "/cash";
        //                            urlSave += "\\cash";
        //                        }
        //                        else if (id == (int)Const.TypeFolder.CASH_ITEM)
        //                        {
        //                            url = "/cashitem";
        //                            urlSave += "\\cashitem";
        //                        }
        //                        else if (id == (int)Const.TypeFolder.ATTACTMENT)
        //                        {
        //                            url = "/attactment";
        //                            urlSave += "\\attactment";
        //                        }
        //                        name += now.ToString("yyyyMMddHHmmssfff");
        //                        url += "/" + name + extension;
        //                        urlSave += "\\" + name + extension;

        //                        //var filePath = HttpContext.Current.Server.MapPath("~/Files" + url);
        //                        //Utils.createFile(filePath, fileData, extension);
        //                        var filePath = domainFile + "Files" + urlSave;
        //                        Utils.createFile(filePath, fileData, extension);
        //                        string rel = "/files" + url;
        //                        files.Add(rel);

        //                        await db.SaveChangesAsync();
        //                    }
        //                }
        //            }
        //            response.Dispose();
        //        }
        //        def.meta = new Meta(200, "Success");
        //        def.data = files;
        //        return Ok(def);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception:" + ex);
        //        def.meta = new Meta(400, "Bad request");
        //        return Ok(def);
        //    }
        //}

        //public static void createThumb(int thumbWidth, string fileThumbnailName, byte[] input, string file_type)
        //{
        //    //extract path
        //    int lastIndex = fileThumbnailName.LastIndexOf("\\");
        //    string path = fileThumbnailName.Substring(0, lastIndex);
        //    Directory.CreateDirectory(path);
        //    MemoryStream ms = new MemoryStream(input);
        //    //Image returnImage = Image.FromStream(ms);
        //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
        //    double srcWidth = image.Width;
        //    double srcHeight = image.Height;
        //    double thumbHeight = (srcHeight / srcWidth) * thumbWidth;
        //    Bitmap bmp = new Bitmap(thumbWidth, (int)thumbHeight);

        //    System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
        //    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

        //    System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, (int)thumbHeight);
        //    gr.DrawImage(image, rectDestination, 0, 0, (int)srcWidth, (int)srcHeight, GraphicsUnit.Pixel);
        //    //rotate
        //    //foreach (var prop in image.PropertyItems)
        //    //{
        //    //    if (prop.Id == 0x0112) //value of EXIF
        //    //    {
        //    //        int orientationValue = image.GetPropertyItem(prop.Id).Value[0];
        //    //        RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
        //    //        bmp.RotateFlip(rotateFlipType);
        //    //        break;
        //    //    }
        //    //}
        //    if (file_type.ToLower() == ".jpg" || file_type.ToLower() == ".jpeg")
        //        bmp.Save(fileThumbnailName, ImageFormat.Jpeg);
        //    else if (file_type.ToLower() == ".png")
        //        bmp.Save(fileThumbnailName, ImageFormat.Png);
        //    else if (file_type.ToLower() == ".gif")
        //        bmp.Save(fileThumbnailName, ImageFormat.Gif);
        //    else
        //        bmp.Save(fileThumbnailName, ImageFormat.Jpeg);

        //    bmp.Dispose();
        //    image.Dispose();
        //}



        //[HttpGet]
        //[Route("api/upload/GetFolders")]
        //public IHttpActionResult GetFolders(int id)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        string filePath = HttpContext.Current.Server.MapPath("~/uploads/");
        //        //string path = "C:/folder1/folder2/file.txt";
        //        string lastFolderName = Path.GetFileName(Path.GetDirectoryName(filePath));

        //        //def.meta = new Meta(200, "Success");
        //        //def.data = files;
        //        return Ok(def);
        //    }
        //    catch (Exception ex)
        //    {
        //        def.meta = new Meta(400, "Bad request");
        //        return Ok(def);
        //    }
        //}

        //Đọc dữ liệu từ file excel
        [HttpPost]
        [Route("importDataSetExcel")]
        public async Task<IActionResult> importDataSetExcel()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int i = 0;
                var httpRequest = Request.Form.Files;
                List<DataSetExcel> data = new List<DataSetExcel>();
                foreach (var file in httpRequest)
                {
                    var postedFile = httpRequest[i];
                    if (postedFile != null && postedFile.Length > 0)
                    {

                        int MaxContentLength = 4096 * 4096 * 100; //Size = 16 MB  

                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var name = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();

                        if (postedFile.Length > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1600 mb.");
                            def.meta = new Meta(600, message);
                            return Ok(def);
                        }
                        else
                        {
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(postedFile.OpenReadStream()))
                            {
                                DateTime now = DateTime.Now;
                                fileData = binaryReader.ReadBytes((int)postedFile.Length);

                                using (MemoryStream ms = new MemoryStream(fileData))
                                {
                                    using (var db = new IOITDataContext())
                                    {
                                        //đọc từ file excel
                                        data = await importDataSet(ms, 0, 1, db);
                                        //Thêm vào db
                                        await saveDataSet(data, db);
                                    }
                                }
                            }
                        }

                    }
                    i++;
                }

                if (data.Count > 0)
                {
                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    def.metadata = data.Count;
                }
                else
                {
                    def.meta = new Meta(400, "Bad request");
                }
                return Ok(def);
            }
            catch (Exception ex)
            {
                log.Error("Exception:" + ex);
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }
        }

        public static async Task<List<DataSetExcel>> importDataSet(MemoryStream ms, int sheetnumber, int rowStart, IOITDataContext db)
        {
            XSSFWorkbook workbook = new XSSFWorkbook(ms);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            List<DataSetExcel> dataItems = new List<DataSetExcel>();

            //Đọc dữ liệu từ excel
            int k = 1;
            for (int row = rowStart; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null)
                {
                    int i = 0;
                    //Đọc dữ liệu từ từng cell
                    DataSetExcel dataItem = new DataSetExcel();
                    dataItem.Error = "";
                    dataItem.Location = k;
                    foreach (var cell in sheet.GetRow(row))
                    {
                        try
                        {
                            //Lấy giá trị trong cell
                            string str = Utils.getCellValue(cell).Trim();
                            if (i == 0)
                            {
                                try
                                {
                                    dataItem.STT = str;
                                }
                                catch
                                {
                                }
                            }
                            else if (i == 1)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 20;
                                        dataItem.Error += " Cột tiêu đề chưa có dữ liệu;";
                                    }
                                    dataItem.Title = str;
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột tiêu đề;";
                                }
                            }
                            else if (i == 2)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 20;
                                        dataItem.Error += " Cột phạm vi ứng dụng chưa có dữ liệu;";
                                    }
                                    dataItem.ListPvud = str;
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi phạm vi ứng dụng;";
                                }
                            }
                            else if (i == 3)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 20;
                                        dataItem.Error += " Cột lĩnh vực nghiên cứu chưa có dữ liệu;";
                                    }
                                    dataItem.ListLvnc = str;
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột lĩnh vực nghiên cứu;";
                                }
                            }
                            else if (i == 4)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 20;
                                        dataItem.Error += " Cột loại dữ liệu chưa có dữ liệu;";
                                    }
                                    else
                                    {
                                        dataItem.Type = int.Parse(str);
                                    }
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột loại dữ liệu;";
                                }
                            }
                            else if (i == 5)
                            {
                                try
                                {
                                    dataItem.Description = str;
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột mô tả nội dung;";
                                }
                            }
                            else if (i == 6)
                            {
                                try
                                {
                                    dataItem.AuthorName = str;
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột tên tác giả;";
                                }
                            }
                            else if (i == 7)
                            {
                                try
                                {
                                    dataItem.AuthorEmail = str;
                                }
                                catch
                                {
                                    //categoryContractExcelChild.Status = 10;
                                    //categoryContractExcelChild.Error += " Lỗi cột Bắt đầu Tiến độ theo hợp đồng;";
                                }
                            }
                            else if (i == 8)
                            {
                                try
                                {
                                    dataItem.AuthorPhone = str;
                                }
                                catch
                                {
                                    //categoryContractExcelChild.Status = 10;
                                    //categoryContractExcelChild.Error += " Lỗi cột Kết thúc Tiến độ theo hợp đồng;";
                                }
                            }
                            else if (i == 9)
                            {
                                try
                                {
                                    dataItem.Version = str;
                                }
                                catch
                                {
                                    //categoryContractExcelChild.Status = 10;
                                    //categoryContractExcelChild.Error += " Lỗi cột khối lượng;";
                                }
                            }
                            else if (i == 10)
                            {
                                try
                                {
                                    dataItem.Note = str;
                                }
                                catch
                                {
                                    //dataItem.Status = 10;
                                    //dataItem.Error += " Lỗi cột Thành tiền;";
                                }
                            }
                            else if (i == 11)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 20;
                                        dataItem.Error += " Cột trạng thái chưa có dữ liệu;";
                                    }
                                    else
                                    {
                                        if (str == "Tạo mới")
                                            dataItem.Status = 10;
                                        else if (str == "Chờ phê duyệt")
                                            dataItem.Status = 3;
                                        else if (str == "Đã duyệt")
                                            dataItem.Status = 2;
                                        else if (str == "Đã công khai")
                                            dataItem.Status = 1;
                                        else if (str == "Không duyệt")
                                            dataItem.Status = 4;
                                    }
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột trạng thái;";
                                }
                            }
                            else if (i == 12)
                            {
                                try
                                {
                                    dataItem.ListAttactment = str;
                                }
                                catch
                                {
                                    //dataItem.Status = 10;
                                    //dataItem.Error += " Lỗi cột Thành tiền;";
                                }
                            }
                            else if (i == 13)
                            {
                                try
                                {
                                    dataItem.ListUnit = str;
                                }
                                catch
                                {
                                    //dataItem.Status = 10;
                                    //dataItem.Error += " Lỗi cột Thành tiền;";
                                }
                            }
                            else if (i == 14)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 20;
                                        dataItem.Error += " Cột người tạo chưa có dữ liệu;";
                                    }
                                    else
                                    {
                                        dataItem.UserCreated = str.Trim().ToLower();
                                        //check tài khoản
                                        var checkUser = await db.Customer.Where(e => e.Email.Trim().ToLower() == dataItem.UserCreated).FirstOrDefaultAsync();
                                        if (checkUser == null)
                                        {
                                            dataItem.Status = 20;
                                            dataItem.Error += " Tài khoản người tạo không tồn tại trên hệ thống;";
                                        }
                                        else
                                        {
                                            dataItem.UserCreatedId = checkUser.CustomerId;
                                            dataItem.UnitId = checkUser.UnitId;
                                            dataItem.Type = checkUser.Type == (int)Const.TypeCustomer.CUSTOMER_UNIT ? (int)Const.DataSetType.DATA_UNIT : (int)Const.DataSetType.DATA_PERSONAL;
                                        }
                                    }
                                }
                                catch
                                {
                                    dataItem.Status = 20;
                                    dataItem.Error += " Lỗi cột người tạo;";
                                }
                            }
                            else if (i == 15)
                            {
                                try
                                {
                                    if (str == "")
                                    {
                                        dataItem.Status = 10;
                                        dataItem.Error += " Cột ngày tạo chưa có dữ liệu;";
                                    }
                                    else
                                    {
                                        dataItem.DateStartActive = Utils.ConvertStringToDate(str);
                                    }
                                }
                                catch
                                {
                                    dataItem.Status = 10;
                                    dataItem.Error += " Lỗi cột ngày tạo;";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dataItem.Status = 10;
                            dataItem.Error += " Lỗi dữ liệu;";
                            log.Error("Exception:" + ex);
                        }
                        i++;
                    }

                    dataItems.Add(dataItem);
                    k++;
                }
            }

            return dataItems;
        }

        private async Task saveDataSet(List<DataSetExcel> listDatas, IOITDataContext db)
        {
            DefaultResponse def = new DefaultResponse();
            int languageId = 1;

            //using (var db = new IOITDataContext())
            //{
            using (var transaction = db.Database.BeginTransaction())
            {
                var listAr = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE
                && e.Status != (int)Const.Status.DELETED).ToListAsync();
                var listRa = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA
                && e.Status != (int)Const.Status.DELETED).ToListAsync();

                foreach (var data in listDatas)
                {
                    try
                    {
                        if (data.Status != 20)
                        {
                            //check xem trùng link ko, nếu trùng tự random ra link mới
                            string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                            url = url.Trim().ToLower();
                            var pLink = await db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            while (pLink != null)
                            {
                                url += Utils.RandomString(5).Trim().ToLower();
                                pLink = await db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            }

                            //add data
                            DataSet dataSet = new DataSet();
                            dataSet.Title = data.Title;
                            dataSet.Description = data.Description == null ? "" : data.Description;
                            dataSet.Image = data.Image;
                            dataSet.Url = url;
                            dataSet.LinkVideo = data.LinkVideo;
                            dataSet.AuthorName = data.AuthorName;
                            dataSet.AuthorEmail = data.AuthorEmail;
                            dataSet.AuthorPhone = data.AuthorPhone;
                            dataSet.Version = data.Version;
                            dataSet.Note = data.Note;
                            dataSet.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                            dataSet.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                            dataSet.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                            dataSet.DownNumber = data.DownNumber != null ? data.DownNumber : 0;
                            dataSet.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                            dataSet.Location = data.Location != null ? data.Location : 1;
                            dataSet.IsHot = data.IsHot != null ? data.IsHot : false;
                            dataSet.Type = data.Type != null ? data.Type : (int)Const.DataSetType.DATA_PERSONAL;
                            dataSet.ApplicationRangeId = data.ApplicationRangeId;
                            dataSet.ResearchAreaId = data.ResearchAreaId;
                            dataSet.UnitId = data.UnitId;
                            dataSet.IsPublish = data.IsPublish;
                            dataSet.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                            dataSet.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                            dataSet.MetaDescription = data.Description != null ? (data.Description.Length >= 500 ? data.Description.Substring(0, 499) : data.Description) : data.Title;
                            dataSet.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                            dataSet.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                            dataSet.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                            dataSet.CreatedAt = DateTime.Now;
                            dataSet.UpdatedAt = DateTime.Now;
                            dataSet.UserCreatedId = data.UserCreatedId;
                            dataSet.UserId = 1;
                            dataSet.Status = data.Status != null ? data.Status : (int)Const.DataSetStatus.TEMP;
                            if (dataSet.Status == (int)Const.DataSetStatus.TEMP)
                            {
                                dataSet.UserEditedId = 1;
                                dataSet.EditedAt = data.DateStartActive;
                            }
                            else if (dataSet.Status == (int)Const.DataSetStatus.PENDING)
                            {
                                dataSet.UserEditedId = 1;
                                dataSet.EditedAt = data.DateStartActive;
                                dataSet.ApprovingAt = data.DateStartActive;
                            }
                            else if (dataSet.Status == (int)Const.DataSetStatus.NORMAL
                                || dataSet.Status == (int)Const.DataSetStatus.APPROVED
                                || dataSet.Status == (int)Const.DataSetStatus.NOT_APPROVED
                                || dataSet.Status == (int)Const.DataSetStatus.NOT_APPROVED_PUBLISH)
                            {
                                dataSet.UserEditedId = 1;
                                dataSet.UserApprovedId = 1;
                                dataSet.UserPublishedId = 1;
                                dataSet.EditedAt = data.DateStartActive;
                                dataSet.ApprovingAt = data.DateStartActive;
                                dataSet.PublishingAt = data.DateStartActive;
                                dataSet.PublishedAt = data.DateStartActive;
                            }
                            await db.DataSet.AddAsync(dataSet);
                            await db.SaveChangesAsync();

                            data.DataSetId = dataSet.DataSetId;
                            if (data.DataSetRootId == null) data.DataSetRootId = dataSet.DataSetId;

                            //add list phạm vi ứng dụng
                            data.applicationRange = new List<CategoryDTL>();
                            string[] listPvud = data.ListPvud.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim()).ToArray();
                            foreach (var itemP in listPvud)
                            {
                                var ar = listAr.Where(e => e.Name.Trim().Equals(itemP.Trim())).Select(e => new CategoryDTL
                                {
                                    CategoryId = e.CategoryId,
                                    Name = e.Name,
                                }).FirstOrDefault();
                                if (ar != null)
                                {
                                    data.applicationRange.Add(ar);
                                }
                            }
                            if (data.applicationRange != null)
                            {
                                int kk = 1;
                                foreach (var item in data.applicationRange)
                                {
                                    DataSetMapping dataSetMapping = new DataSetMapping();
                                    dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                    dataSetMapping.DataSetId = dataSet.DataSetId;
                                    dataSetMapping.TargetId = item.CategoryId;
                                    dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_APPLICATION_RANGE;
                                    dataSetMapping.Location = kk;
                                    dataSetMapping.CreatedAt = DateTime.Now;
                                    dataSetMapping.UserId = 1;
                                    dataSetMapping.Status = (int)Const.Status.NORMAL;
                                    await db.DataSetMapping.AddAsync(dataSetMapping);
                                    kk++;
                                }
                            }

                            //add list lĩnh vực nghiên cứu
                            data.researchArea = new List<CategoryDTL>();
                            string[] listLvnc = data.ListLvnc.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim()).ToArray();
                            foreach (var itemL in listLvnc)
                            {
                                var ra = listRa.Where(e => e.Name.Trim().Equals(itemL.Trim())).Select(e => new CategoryDTL
                                {
                                    CategoryId = e.CategoryId,
                                    Name = e.Name,
                                }).FirstOrDefault();
                                if (ra != null)
                                {
                                    data.researchArea.Add(ra);
                                }
                            }
                            if (data.researchArea != null)
                            {
                                int kk = 1;
                                foreach (var item in data.researchArea)
                                {
                                    DataSetMapping dataSetMapping = new DataSetMapping();
                                    dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                    dataSetMapping.DataSetId = dataSet.DataSetId;
                                    dataSetMapping.TargetId = item.CategoryId;
                                    dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_RESEARCH_AREA;
                                    dataSetMapping.Location = kk;
                                    dataSetMapping.CreatedAt = DateTime.Now;
                                    dataSetMapping.UserId = 1;
                                    dataSetMapping.Status = (int)Const.Status.NORMAL;
                                    await db.DataSetMapping.AddAsync(dataSetMapping);
                                    kk++;
                                }
                            }

                            //add list files
                            data.listFiles = new List<AttactmentDTO>();
                            string[] listAttactment = data.ListAttactment.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim()).ToArray();
                            foreach (var itemA in listAttactment)
                            {
                                string extensionFileName = _configuration["AppSettings:extensionFileName"];
                                string extensionFileId = _configuration["AppSettings:extensionFileId"];
                                IList<string> AllowedFileExtensionsId = extensionFileId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim()).ToArray();
                                IList<string> AllowedFileExtensions = extensionFileName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim()).ToArray();
                                var ext = itemA.Substring(itemA.LastIndexOf('.'));
                                var name = itemA.Substring(0, itemA.LastIndexOf('.'));
                                var extension = ext.ToLower();
                                if (!AllowedFileExtensions.Contains(extension))
                                {
                                    data.Status = 20;
                                    data.Error += string.Format($"Bạn vui lòng tải tài liệu có định dạng: {extensionFileName}");
                                }
                                AttactmentDTO attactment = new AttactmentDTO();
                                attactment.AttactmentId = Guid.NewGuid();
                                attactment.Name = itemA;
                                attactment.Url = itemA;
                                attactment.Storage = data.Storage;
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
                                data.listFiles.Add(attactment);

                            }
                            if (data.listFiles != null)
                            {
                                foreach (var item in data.listFiles)
                                {
                                    if (item.Status != 99)
                                    {
                                        Attactment attactment = new Attactment();
                                        attactment.AttactmentId = item.AttactmentId != null ? item.AttactmentId : Guid.NewGuid();
                                        attactment.Name = item.Name;
                                        attactment.TargetId = dataSet.DataSetId;
                                        attactment.IsImageMain = item.IsImageMain;
                                        attactment.TargetType = (int)Const.TypeAttachment.FILE_DATASET;
                                        attactment.Url = item.Url;
                                        attactment.Note = item.Note;
                                        attactment.Thumb = item.Thumb;
                                        attactment.Extension = item.Extension;
                                        attactment.ExtensionName = item.ExtensionName;
                                        attactment.Storage = item.Storage;
                                        attactment.CreatedAt = DateTime.Now;
                                        attactment.CreatedId = 1;
                                        attactment.UpdatedId = 1;
                                        attactment.Status = (int)Const.Status.NORMAL;
                                        await db.Attactment.AddAsync(attactment);
                                    }
                                }
                            }

                            //tính toán add unit
                            if (data.UnitId != null)
                            {
                                //Lấy unit cha
                                List<int> outPut = new List<int>();
                                //outPut.Add((int)checkUser.UnitId);
                                //var unit = await db.Unit.Where(x => x.UnitId == checkUser.UnitId).FirstOrDefaultAsync();
                                //if (unit != null)
                                //{
                                //    if(unit.UnitParentId )
                                await GetListUnit(outPut, (int)data.UnitId, db);
                                //}
                                int kk = 1;
                                foreach (var item in outPut)
                                {
                                    DataSetMapping dataSetMapping = new DataSetMapping();
                                    dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                    dataSetMapping.DataSetId = dataSet.DataSetId;
                                    dataSetMapping.TargetId = item;
                                    dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_UNIT;
                                    dataSetMapping.Location = kk;
                                    dataSetMapping.CreatedAt = DateTime.Now;
                                    dataSetMapping.UserId = 1;
                                    dataSetMapping.Status = (int)Const.Status.NORMAL;
                                    await db.DataSetMapping.AddAsync(dataSetMapping);
                                    kk++;
                                }
                            }

                            try
                            {
                                await db.SaveChangesAsync();
                                data.listLanguage = new List<LanguageMappingDTO>();
                                if (data.DataSetId > 0)
                                {
                                    //Thêm permalink
                                    PermaLink permaLink = new PermaLink();
                                    permaLink.PermaLinkId = Guid.NewGuid();
                                    permaLink.Slug = dataSet.Url;
                                    permaLink.TargetId = dataSet.DataSetId;
                                    permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET;
                                    permaLink.CreatedAt = DateTime.Now;
                                    permaLink.UpdatedAt = DateTime.Now;
                                    permaLink.Status = (int)Const.Status.NORMAL;
                                    await db.PermaLink.AddAsync(permaLink);
                                    await db.SaveChangesAsync();
                                    //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                    //và có id bài viết gốc
                                    if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                        && data.DataSetRootId != null && data.DataSetRootId > 0)
                                    {
                                        var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                          && e.TargetId1 == data.DataSetRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_DATASET
                                          && e.Status != (int)Const.Status.DELETED).ToList();
                                        if (listLang.Count > 0)
                                        {
                                            LanguageMapping languageMapping = new LanguageMapping();
                                            languageMapping.LanguageId1 = languageId;
                                            languageMapping.LanguageId2 = data.LanguageId;
                                            languageMapping.TargetId1 = data.DataSetRootId;
                                            languageMapping.TargetId2 = data.DataSetId;
                                            languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_DATASET;
                                            languageMapping.CreatedAt = DateTime.Now;
                                            languageMapping.Status = (int)Const.Status.NORMAL;
                                            await db.LanguageMapping.AddAsync(languageMapping);

                                            foreach (var item in listLang)
                                            {
                                                LanguageMapping languageMapping2 = new LanguageMapping();
                                                languageMapping2.LanguageId1 = item.LanguageId2;
                                                languageMapping2.LanguageId2 = data.LanguageId;
                                                languageMapping2.TargetId1 = item.TargetId2;
                                                languageMapping2.TargetId2 = data.DataSetId;
                                                languageMapping2.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_DATASET;
                                                languageMapping2.CreatedAt = DateTime.Now;
                                                languageMapping2.Status = (int)Const.Status.NORMAL;
                                                await db.LanguageMapping.AddAsync(languageMapping2);
                                            }
                                        }
                                        else
                                        {
                                            LanguageMapping languageMapping = new LanguageMapping();
                                            languageMapping.LanguageId1 = languageId;
                                            languageMapping.LanguageId2 = data.LanguageId;
                                            languageMapping.TargetId1 = data.DataSetRootId;
                                            languageMapping.TargetId2 = data.DataSetId;
                                            languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_DATASET;
                                            languageMapping.CreatedAt = DateTime.Now;
                                            languageMapping.Status = (int)Const.Status.NORMAL;
                                            await db.LanguageMapping.AddAsync(languageMapping);
                                        }
                                        await db.SaveChangesAsync();
                                        //numLang = listLang.Count + 2;
                                    }
                                    transaction.Commit();

                                    data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.DataSetRootId
                                       && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_DATASET && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageMappingDTO
                                       {
                                           LanguageMappingId = a.LanguageMappingId,
                                           LanguageId1 = a.LanguageId1,
                                           LanguageId2 = a.LanguageId2,
                                           TargetId1 = a.TargetId1,
                                           TargetId2 = a.TargetId2,
                                           TargetType = a.TargetType,
                                           CreatedAt = a.CreatedAt,
                                           Status = a.Status
                                       }).ToList();
                                }
                                else
                                    transaction.Rollback();

                            }
                            catch (DbUpdateException e)
                            {
                                log.Error("DbUpdateException:" + e);
                                transaction.Rollback();

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Error:" + e);
                    }
                }
            }
            //}
        }

        private async Task GetListUnit(List<int> outPut, int unitId, IOITDataContext db)
        {
            var unit = await db.Unit.Where(x => x.UnitId == unitId && x.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
            if (unit != null)
            {
                outPut.Add(unit.UnitId);
                if (unit.UnitParentId > 0)
                {
                    await GetListUnit(outPut, unit.UnitParentId, db);
                }
            }
        }


    }
}
