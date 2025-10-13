using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        //private CNTTVNData db = new CNTTVNData();
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "QLBLTT"; //Quản lý bình luận tin tức
        private static string functionCode1 = "QLBLSP"; //Quản lý bình luận sản phẩm

        // GET: api/Comment
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<CommentDT> data = (from c in db.Customer
                                                  join cm in db.Comment on c.CustomerId equals cm.CustomerId
                                                  where cm.Status != (int)Const.Status.DELETED
                                                  select new CommentDT
                                                  {
                                                      CommentId = cm.CommentId,
                                                      CustomerId = c.CustomerId,
                                                      CustomerName = c.FullName,
                                                      TargetId = cm.TargetId,
                                                      TargetType = cm.TargetType,
                                                      Contents = cm.Contents,
                                                      CommentParentId = cm.CommentParentId,
                                                      CreatedAt = cm.CreatedAt,
                                                      UpdateAt = cm.UpdateAt,
                                                      Status = cm.Status
                                                  }).AsQueryable();
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
                            data = data.OrderBy("CommentId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("CommentId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select).ToDynamicList();
                    }
                    else
                    {
                        List<CommentDT> res = new List<CommentDT>();
                        var listData = await data.ToListAsync();
                        foreach (var item in listData)
                        {
                            item.SumComment = db.Comment.Where(c => c.TargetId == item.TargetId
                            && c.TargetType == item.TargetType && c.Status != (int)Const.Status.DELETED).Count();
                            switch (item.TargetType)
                            {
                                case (int)Const.TypeComment.COMMENT_NEWS:
                                    News news = db.News.Where(n => n.NewsId == item.TargetId && n.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                                    if (news != null)
                                    {
                                        item.TargetName = news.Title;

                                        Category category = (from cm in db.CategoryMapping
                                                             join c in db.Category on cm.CategoryId equals c.CategoryId
                                                             where cm.Status == (int)Const.Status.NORMAL
                                                             && c.Status == (int)Const.Status.NORMAL
                                                             && cm.TargetId == news.NewsId
                                                             && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                                             select c).FirstOrDefault();
                                        if (category != null)
                                        {
                                            switch (news.TypeNewsId)
                                            {
                                                case (int)Const.TypeNews.NEWS_TEXT:
                                                    item.Link = "/" + Const.DETAIL_NEWS + "/" + news.Url + "-" + category.CategoryId + "-" + news.NewsId + ".html";
                                                    break;
                                                case (int)Const.TypeNews.NEWS_IMAGE:
                                                    item.Link = "/" + Const.DETAIL_IMAGE + "/" + news.Url + "-" + (int)Const.WEBSITEID + "-" + news.NewsId + ".html";
                                                    break;
                                                case (int)Const.TypeNews.NEWS_VIDEO:
                                                    item.Link = "/" + Const.DETAIL_VIDEO + "/" + news.Url + "-" + (int)Const.WEBSITEID + "-" + news.NewsId + ".html";
                                                    break;
                                                case (int)Const.TypeNews.NEWS_NOTIFICATION:
                                                    item.Link = "/" + Const.DETAIL_NOTIFICATION + "/" + news.Url + "-" + (int)Const.WEBSITEID + "-" + news.NewsId + ".html";
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                case (int)Const.TypeComment.COMMENT_PRODUCT:
                                    Product product = db.Product.Where(n => n.ProductId == item.TargetId && n.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                                    if (product != null)
                                    {
                                        item.TargetName = product.Name;

                                        item.Link = "/" + Const.DETAIL_PRODUCT + "/" + product.Url + "-" + (int)Const.WEBSITEID + "-" + product.ProductId + ".html";
                                    }
                                    break;
                                default:
                                    break;
                            }

                            res.Add(item);
                        }

                        def.data = res.ToList();
                    }

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    Comment data = await db.Comment.FindAsync(id);

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // PUT: api/Comment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, [FromBody] Comment data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Comment comment = db.Comment.AsNoTracking().Where(c => c.CommentId == data.CommentParentId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (comment != null)
                    {
                        if (comment.CommentParentId != 0)
                        {
                            data.CommentParentId = comment.CommentParentId;
                        }

                        data.TargetId = comment.TargetId;
                        data.TargetType = comment.TargetType;
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.CommentParentId = data.CommentParentId != null ? data.CommentParentId : 0;
                        data.UpdateAt = DateTime.Now;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CommentId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa bình luận “" + data.CommentId + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.CommentId.ToString();
                                action.TargetName = data.CustomerId.ToString();
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
                            }

                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!CommentExists(data.CommentId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Comment
        [HttpPost]
        public async Task<IActionResult> PostComment(Comment data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Comment comment = db.Comment.AsNoTracking().Where(c => c.CommentId == data.CommentParentId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (comment != null)
                    {
                        if (comment.CommentParentId != 0)
                        {
                            data.CommentParentId = comment.CommentParentId;
                        }

                        data.TargetId = comment.TargetId;
                        data.TargetType = comment.TargetType;
                    }


                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.CommentParentId = data.CommentParentId != null ? data.CommentParentId : 0;
                        data.CreatedAt = DateTime.Now;
                        data.UpdateAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        db.Comment.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CommentId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm bình luận “" + data.CommentId + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.CommentId.ToString();
                                action.TargetName = data.CustomerId.ToString();
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CommentExists(data.CommentId))
                            {
                                def.meta = new Meta(211, "Exist");
                                return Ok(def);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpPut("ShowHide/{id}/{stt}")]
        public async Task<ActionResult> ShowHide(int id, int stt)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            using (var db = new IOITDataContext())
            {
                Comment data = await db.Comment.FindAsync(id);
                if (data == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    //db.Comment.Remove(data);
                    data.Status = (byte)stt;
                    db.Entry(data).State = EntityState.Modified;
                    try
                    {
                        await db.SaveChangesAsync();

                        if (data.CommentId > 0)
                        {

                            transaction.Commit();

                            Models.EF.Action action = new Models.EF.Action();
                            if (stt == 3)
                            {
                                action.ActionName = "Đổi trạng thái bình luận thành không duyệt: “" + data.CommentId + "”";
                            }
                            else
                            {
                                action.ActionName = "Đổi trạng thái bình luận thành duyệt: “" + data.CommentId + "”";
                            }

                            action.ActionType = (int)Const.ActionType.UPDATE;
                            action.TargetId = data.CommentId.ToString();
                            action.TargetName = data.CustomerId.ToString();
                            action.CompanyId = companyId;
                            action.Logs = JsonConvert.SerializeObject(data);
                            action.Time = 0;
                            action.Ipaddress = IpAddress();
                            action.Type = (int)Const.TypeAction.ACTION;
                            action.CreatedAt = DateTime.Now;
                            action.UserPushId = userId;
                            action.UserId = userId;
                            //action.Status = (int)Const.Status.NORMAL;

                            await db.Action.AddAsync(action);
                            await db.SaveChangesAsync();
                        }

                        else
                            transaction.Rollback();

                        def.meta = new Meta(200, "Success");
                        def.data = data;
                        return Ok(def);
                    }
                    catch (DbUpdateException e)
                    {
                        transaction.Rollback();
                        log.Error("DbUpdateException:" + e);
                        if (!CommentExists(data.CommentId))
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }
                        else
                        {
                            def.meta = new Meta(500, "Internal Server Error");
                            return Ok(def);
                        }
                    }
                }
            }
        }

        // DELETE: api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    Comment data = await db.Comment.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    //Kiểm tra còn bình luận con của nó thì k đc xóa
                    List<Comment> comments = db.Comment.Where(c => c.CommentParentId == data.CommentId && c.Status != (int)Const.Status.DELETED).ToList();
                    if (comments.Count() > 0)
                    {
                        def.meta = new Meta(212, "Bình luận này còn bình luận trả lời. Không đc xóa!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Comment.Remove(data);
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CommentId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá bình luận “" + data.CommentId + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.CommentId.ToString();
                                action.TargetName = data.CustomerId.ToString();
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
                            }

                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!CommentExists(data.CommentId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        //API xóa danh sách bình luận
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteComments([FromBody] int[] data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data == null)
                {
                    def.meta = new Meta(211, "Array Empty!");
                    return Ok(def);
                }

                if (data.Count() == 0)
                {
                    def.meta = new Meta(211, "Array Empty!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        Comment comment = await db.Comment.FindAsync(data[i]);

                        if (comment == null)
                        {
                            continue;
                        }

                        //Kiểm tra còn bình luận con của nó thì k đc xóa
                        List<Comment> comments = db.Comment.Where(c => c.CommentParentId == comment.CommentId && c.Status != (int)Const.Status.DELETED).ToList();
                        if (comments.Count() > 0)
                        {
                            continue;
                        }

                        comment.Status = (int)Const.Status.DELETED;
                        db.Entry(comment).State = EntityState.Modified;
                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá bình luận “" + comment.CommentId + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = comment.CommentId.ToString();
                        action.TargetName = comment.CustomerId.ToString();
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(comment);
                        action.Time = 0;
                        action.Ipaddress = IpAddress();
                        action.Type = (int)Const.TypeAction.ACTION;
                        action.CreatedAt = DateTime.Now;
                        action.UserPushId = userId;
                        action.UserId = userId;
                        action.Status = (int)Const.Status.NORMAL;
                        await db.Action.AddAsync(action);
                        await db.SaveChangesAsync();
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            await db.SaveChangesAsync();
                            transaction.Commit();
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            def.meta = new Meta(500, "Internal Server Error");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool CommentExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Comment.Count(e => e.CommentId == id) > 0;
            }
        }
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }


        // haohv Get Comment 
        [HttpGet("GetCommentByPage")]
        public async Task<IActionResult> GetCommentByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();

            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<CommentDT> data = (from c in db.Customer
                                                  join cm in db.Comment on c.CustomerId equals cm.CustomerId
                                                  where cm.TargetId == 2 // Dùng cho comment của người tham gia
                                                  && (cm.Status == (int)Const.Status.NORMAL || cm.Status == (int)Const.Status.OK)
                                                  select new CommentDT
                                                  {
                                                      CommentId = cm.CommentId,
                                                      CustomerName = c.FullName,
                                                      CustomerId = c.CustomerId,
                                                      TargetId = cm.TargetId,
                                                      TargetType = cm.TargetType,
                                                      Contents = cm.Contents,
                                                      CommentParentId = cm.CommentParentId,
                                                      CreatedAt = cm.CreatedAt,
                                                      UpdateAt = cm.UpdateAt,
                                                      Status = cm.Status,
                                                      EmailComment = cm.Email, // email người comment
                                                      Name = cm.Name, // họ tên người comment
                                                  }).AsQueryable();

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
                            data = data.OrderBy("CommentId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("CommentId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select).ToDynamicList();
                    }
                    else
                    {
                        List<CommentDT> res = new List<CommentDT>();
                        var listData = await data.ToListAsync();
                        foreach (var item in listData)
                        {
                            item.SumComment = db.Comment.Where(c => c.TargetId == item.TargetId
                            && c.TargetType == item.TargetType && c.Status != (int)Const.Status.DELETED).Count();

                            res.Add(item);
                        }

                        def.data = res.ToList();
                    }

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // haohv Get List Comment theo ID người tham gia
        [HttpGet("GetCommentByJoinnerID/{id}")]
        public async Task<IActionResult> GetCommentByJoinnerId(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;

            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    var data = db.Comment.Where(w => w.CustomerId == id
                         && w.TargetId == 2
                         && w.Status == (int)Const.Status.OK) // đã được duyệt
                        .ToArray();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }

        }

        [HttpPost("AddNewComment")]
        public async Task<IActionResult> AddNewComment(Comment data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE) && !CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }


                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        // validate = mỗi 1 email chỉ được comment 1 lần
                        var existComment = db.Comment.Where(c => c.Email == data.Email).FirstOrDefault();
                        if (existComment != null)
                        {
                            def.meta = new Meta(201, "Địa chỉ email đã bình luận. Không được tiếp tục bình luận.");
                            return Ok(def);
                        }

                        data.CommentParentId = data.CommentParentId != null ? data.CommentParentId : 0;
                        data.CreatedAt = DateTime.Now;
                        data.UpdateAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        data.TargetId = 2; // Sử dụng cho comment của người tham gia

                        try
                        {
                            db.Comment.Add(data);
                            await db.SaveChangesAsync();
                            transaction.Commit();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CommentExists(data.CommentId))
                            {
                                def.meta = new Meta(211, "Exist");
                                return Ok(def);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
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


