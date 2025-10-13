using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        [HttpGet("listComment/{id}")]
        public async Task<IActionResult> ListComment(int id)
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new IOITDataContext())
            {
                def.meta = new Meta(200, "Success");
                var data = await db.Comment.Where(e => e.TargetId == id
                && e.TargetType == (int)Const.TypeComment.COMMENT_NEWS).Select(e => new CommentDT
                {
                    CommentId = e.CommentId,
                    CustomerId = e.CustomerId,
                    TargetId = e.TargetId,
                    TargetType = e.TargetType,
                    Contents = e.Contents,
                    CommentParentId = e.CommentParentId,
                    SumLike = e.NumberLike,
                    CreatedAt = e.CreatedAt,
                    UpdateAt = e.UpdateAt,
                    Status = e.Status,
                }).ToListAsync();
                List<CommentDT> listComments = new List<CommentDT>();
                listComments = getListComment(data, 0);
            }
            return Ok(def);
        }

        public static List<CommentDT> getListComment(List<CommentDT> input, int parent)
        {
            List<CommentDT> listComments = new List<CommentDT>();
            var data = input.Where(e => e.CommentParentId == parent).ToList();
            foreach (var item in data)
            {
                item.commentChild = getListComment(input, (int)item.CommentParentId);
                listComments.Add(item);
            }
            return listComments;
        }

        //create comment
        [HttpPost]
        public async Task<IActionResult> PostComment(CommentDT data)
        {
            DefaultResponse def = new DefaultResponse();
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
                        //Tự động tạo tk khách hàng theo Email nếu chưa có tk
                        string email = data.Email.Trim().ToLower();
                        var checkCus = await db.Customer.Where(e => e.Email.Equals(email)).FirstOrDefaultAsync();
                        if (checkCus == null)
                        {
                            Customer customer = new Customer();
                            customer.FullName = data.CustomerName;
                            customer.Email = data.Email.Trim().ToLower();
                            customer.Username = customer.Email;
                            customer.Type = 1;
                            customer.WebsiteId = 1;
                            customer.CompanyId = 1;
                            customer.CountryId = 1;
                            customer.TypeThirdId = 1;
                            customer.IsEmailConfirm = true;
                            customer.IsSentEmailConfirm = true;
                            customer.IsPhoneConfirm = true;
                            customer.KeyRandom = Utils.RandomString(8);
                            customer.Password = Utils.GetMD5Hash("123456" + customer.KeyRandom);
                            customer.LastLoginAt = DateTime.Now;
                            customer.CreatedAt = DateTime.Now;
                            customer.UpdatedAt = DateTime.Now;
                            customer.Status = (int)Const.Status.NORMAL;
                            await db.Customer.AddAsync(customer);
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;
                        }
                        else
                        {
                            data.CustomerId = checkCus.CustomerId;
                            checkCus.Status = (int)Const.Status.NORMAL;
                            db.Customer.Update(checkCus);
                        }
                        //
                        Comment comment = new Comment();
                        comment.CustomerId = data.CustomerId;
                        comment.TargetId = data.TargetId;
                        comment.TargetType = data.TargetType;
                        comment.Contents = data.Contents;
                        comment.CommentParentId = data.CommentParentId != null ? data.CommentParentId : 0;
                        comment.NumberLike = 0;
                        comment.CreatedAt = DateTime.Now;
                        comment.UpdateAt = DateTime.Now;
                        comment.Status = (int)Const.Status.NORMAL;
                        await db.Comment.AddAsync(comment);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CommentId = comment.CommentId;
                            if (data.CommentId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            def.meta = new Meta(500, "Internal Server Error");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // haohv Get List Comment theo ID người tham gia
        [HttpGet("GetCommentByJoinnerID/{id}")]
        public async Task<IActionResult> GetCommentByJoinnerId(int id)
        {
            DefaultResponse def = new DefaultResponse();
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
                    def.data = data.OrderByDescending(x => x.CommentId);
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }

        }

        [HttpPost("AddNewComment")]
        public async Task<IActionResult> AddNewComment(Comment data)
        {
            DefaultResponse def = new DefaultResponse();
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
                        // validate = mỗi 1 email chỉ được comment 1 lần trong 1 người

                        var existComment = db.Comment.Where(c => c.Email == data.Email && c.CustomerId == data.CustomerId).FirstOrDefault();
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
                            def.meta = new Meta(211, "Exist");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }
    }
}
