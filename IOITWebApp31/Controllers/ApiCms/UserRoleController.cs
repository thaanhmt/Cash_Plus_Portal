using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
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
    public class UserRoleController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("userRole", "userRole");
        private static string functionCode = "QLND";
        private IHostingEnvironment _hostingEnvironment;

        public UserRoleController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/UserRole
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault());
            int roleLevel = int.Parse(identity.Claims.Where(c => c.Type == "RoleLevel").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<User> data = db.User.Where(c => c.Status != (int)Const.Status.DELETED);

                    //if (roleMax != 1 && roleMax != 8)
                    //{
                    //    paging.query = "RoleLevel > " + roleLevel;
                    //}
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
                            data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("CreatedAt desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                    {
                        def.data = data.Select(c => new
                        {
                            c.UserId,
                            c.Code,
                            c.FullName,
                            c.UserName,
                            c.Avata,
                            c.Address,
                            c.Email,
                            c.Phone,
                            c.CreatedAt,
                            c.Status,
                            c.UnitId,
                            c.DepartmentId,
                            c.PositionId,
                            c.RoleMax,
                            c.RoleLevel,
                            c.IsRoleGroup,
                            c.CompanyId,
                            //unit = db.Units.Where(e => e.UnitId == c.UnitId && e.Status != (int)Const.Status.DELETED).Select(e => new
                            //{
                            //    e.UnitId,
                            //    e.Name
                            //}).FirstOrDefault(),
                            department = db.Department.Where(e => e.DepartmentId == c.DepartmentId && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.DepartmentId,
                                e.Name
                            }).FirstOrDefault(),
                            position = db.Position.Where(e => e.PositionId == c.PositionId && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.PositionId,
                                e.Name
                            }).FirstOrDefault(),
                            listRole = db.UserRole.Where(e => e.UserId == c.UserId && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.RoleId,
                                RoleName = db.Role.Where(r => r.RoleId == e.RoleId).FirstOrDefault().Name,
                            }).ToList(),
                            listFunction = db.FunctionRole.Where(e => e.TargetId == c.UserId && e.Type == (int)Const.TypeFunction.FUNCTION_USER && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.FunctionId,
                                e.ActiveKey
                            }).ToList(),
                            //listUnit = db.UserProjects.Where(e => e.UserId == c.UserId && e.Type == (int)Const.TypeUserProject.USER_UNIT && e.Status != (int)Const.Status.DELETED).Select(e => new
                            //{
                            //    e.TargetId,
                            //}).ToList(),
                            //listProject = db.UserProjects.Where(e => e.UserId == c.UserId && e.Type == (int)Const.TypeUserProject.USER_PROJECT && e.Status != (int)Const.Status.DELETED).Select(e => new
                            //{
                            //    e.TargetId,
                            //}).ToList()
                        }).ToList();
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

        [HttpGet("GetByPageNotRole")]
        public IActionResult GetByPageNotRole([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<User> data = from c in db.User.Where(e => e.Status == (int)Const.Status.NORMAL)
                                            where !db.UserRole.Any(m => m.UserId == c.UserId && m.Status != (int)Const.Status.DELETED)
                                            select c;
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
                            data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("CreatedAt desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                    {
                        def.data = data;
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

        // GET: api/UserRole/5
        [HttpGet("{id}")]
        public IActionResult GetUserRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                using (var db = new IOITDataContext())
                {
                    IQueryable<User> data = db.User.Where(c => c.UserId == id && c.Status != (int)Const.Status.DELETED);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    var dataS = data.Select(c => new
                    {
                        c.UserId,
                        EmployeeCode = c.Code,
                        c.FullName,
                        listRole = db.UserRole.Where(e => e.UserId == c.UserId && e.Status != (int)Const.Status.DELETED).Select(e => new
                        {
                            e.RoleId,
                            RoleName = db.Role.Where(r => r.RoleId == e.RoleId).FirstOrDefault().Name,
                        }).ToList(),

                    });

                    def.data = dataS.Where(e => e.listRole.Count() > 0).ToList();
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

        //[HttpGet]
        //[Route("api/userRole/ListUserMonitor")]
        //public IHttpActionResult ListUserMonitor()
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    try
        //    {
        //        //IQueryable<User> data = db.Users.Where(c => c.UserId == id && c. && c.Status != (int)Const.Status.DELETED);
        //        IQueryable<User> data = from user in db.Users
        //                                join ur in db.UserRoles on user.UserId equals ur.UserId
        //                                where user.Status == (int)Const.Status.NORMAL && ur.Status != (int)Const.Status.DELETED
        //                                && ur.Role.Code == "MONITORING"
        //                                select user;
        //        if (data == null)
        //        {
        //            def.meta = new Meta(404, "Not Found");
        //            return Ok(def);
        //        }

        //        def.meta = new Meta(200, "Success");
        //        def.data = data.Select(c => new
        //        {
        //            c.UserId,
        //            c.FullName,
        //        });

        //        return Ok(def);
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Internal Server Error");
        //        return Ok(def);
        //    }
        //}

        // PUT: api/UserRole/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRole(int id, [FromBody] UserRoleDT data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
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

                if (id != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    User current = await db.User.FindAsync(id);
                    if (current == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    User checkUserNameExist = db.User.Where(f => f.UserId != data.UserId && f.UserName == data.UserName && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkUserNameExist != null)
                    {
                        def.meta = new Meta(211, "Tài khoản đã tồn tại!");
                        return Ok(def);
                    }

                    User checkEmailExist = db.User.Where(f => f.UserId != data.UserId && f.Email == data.Email && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmailExist != null)
                    {
                        def.meta = new Meta(2111, "Email đã được sử dụng cho tài khoản khác!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //update user
                        current.FullName = data.FullName;
                        current.Code = data.Code;
                        current.Phone = data.Phone;
                        current.Email = data.Email;
                        current.Address = data.Address;
                        current.Avata = data.Avata;
                        current.UnitId = data.UnitId;
                        current.DepartmentId = data.DepartmentId;
                        current.PositionId = data.PositionId;
                        //current.CompanyId = data.CompanyId;
                        current.IsRoleGroup = data.IsRoleGroup != null ? data.IsRoleGroup : true;
                        current.UpdatedAt = DateTime.Now;
                        db.Entry(current).State = EntityState.Modified;

                        try
                        {
                            //role old
                            byte levelOld = (byte)current.RoleLevel;
                            // role
                            var checkRole = false;
                            byte level = 99;
                            int max = 9999;
                            //update list role
                            //add new
                            foreach (var item in data.listRole)
                            {
                                var role = db.Role.Find(item.RoleId);
                                if (role != null)
                                {
                                    var userRoleNew = db.UserRole.Where(e => e.UserId == data.UserId && e.RoleId == item.RoleId && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (userRoleNew.Count <= 0)
                                    {
                                        UserRole userRole = new UserRole();
                                        userRole.RoleId = item.RoleId;
                                        userRole.UserId = data.UserId;
                                        userRole.Status = (int)Const.Status.NORMAL;
                                        db.UserRole.Add(userRole);
                                    }
                                    //check role
                                    if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                                        checkRole = true;
                                    //
                                    if (role.LevelRole < level)
                                    {
                                        level = (byte)role.LevelRole;
                                        max = role.RoleId;
                                    }
                                }
                            }
                            //delete old
                            var listUserRole = db.UserRole.Where(e => e.UserId == data.UserId && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in listUserRole)
                            {
                                var listNew = data.listRole.Where(e => e.RoleId == item.RoleId).ToList();
                                if (listNew.Count <= 0)
                                {
                                    UserRole userRoleExit = await db.UserRole.FindAsync(item.UserRoleId);
                                    userRoleExit.Status = (int)Const.Status.DELETED;
                                    db.Entry(userRoleExit).State = EntityState.Modified;
                                }
                                else
                                {
                                    //Check xem có phải quyền giám sát ko
                                    var role = db.Role.Find(item.RoleId);
                                    if (role != null)
                                    {
                                        //check role
                                        if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                                            checkRole = true;
                                    }
                                }
                            }

                            //update quyền cao nhất và cấp cao nhất của user
                            current.RoleLevel = level;
                            current.RoleMax = max;
                            db.Entry(current).State = EntityState.Modified;

                            //update list function
                            foreach (var item in data.listFunction)
                            {
                                var functionNew = db.FunctionRole.Where(e => e.TargetId == data.UserId
                                && e.FunctionId == item.FunctionId
                                && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                //add new
                                if (functionNew.Count <= 0)
                                {
                                    FunctionRole functionRole = new FunctionRole();
                                    functionRole.TargetId = data.UserId;
                                    functionRole.FunctionId = item.FunctionId;
                                    functionRole.ActiveKey = item.ActiveKey;
                                    functionRole.Type = (int)Const.TypeFunction.FUNCTION_USER;
                                    functionRole.CreatedAt = DateTime.Now;
                                    functionRole.UpdatedAt = DateTime.Now;
                                    functionRole.UserId = data.UserCreateId;
                                    functionRole.Status = (int)Const.Status.NORMAL;
                                    db.FunctionRole.Add(functionRole);
                                }
                                else
                                {
                                    //update
                                    var functionRoleExit = functionNew.FirstOrDefault();
                                    functionRoleExit.ActiveKey = item.ActiveKey;
                                    functionRoleExit.UpdatedAt = DateTime.Now;
                                    functionRoleExit.UserId = data.UserCreateId;
                                    db.Entry(functionRoleExit).State = EntityState.Modified;
                                }
                            }

                            if (current.UserMapId != null)
                            {
                                var userMap = await db.Customer.Where(x => x.CustomerId == current.UserMapId).FirstOrDefaultAsync();
                                if (userMap != null)
                                {
                                    userMap.FullName = current.FullName;
                                    userMap.Email = current.Email;
                                    userMap.Phone = current.Phone;
                                    userMap.Address = current.Address;
                                    db.Customer.Update(userMap);
                                }
                            }
                            await db.SaveChangesAsync();

                            transaction.Commit();
                            Models.EF.Action action = new Models.EF.Action();
                            action.ActionName = "Sửa tài khoản “" + data.FullName + "”";
                            action.ActionType = (int)Const.ActionType.UPDATE;
                            action.TargetId = data.UserId.ToString();
                            action.TargetName = data.FullName;
                            action.CompanyId = 1;
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
                            //create action
                            //Models.EF.Action action = new Models.EF.Action();
                            //action.ActionName = "Sửa tài khoản";
                            //action.ActionType = "UPDATE";
                            //action.TargetId = data.UserId;
                            //action.TargetType = (int)Const.T;
                            //action.Logs = JsonConvert.SerializeObject(data);
                            //action.Time = 0;
                            //action.Type = (int)Const.TypeAction.ACTION;
                            //action.CreatedAt = DateTime.Now;
                            //action.UserId = data.UserCreateId;
                            //action.Status = (int)Const.Status.NORMAL;
                            //db.Actions.Add(action);
                            //await db.SaveChangesAsync();

                            ////push action firebase
                            //Models.Data.Firebase.pushAction(action);

                            //push user firebase
                            //var tasks = new[]
                            //{
                            //    Task.Run(() => IOITWebApp31.Models.Data.Firebase.updateUser(current))
                            //};

                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserRoleExists(id))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception: " + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Function
        [HttpPost]
        public async Task<IActionResult> PostUserRole([FromBody] UserRoleDT data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
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
                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }


                using (var db = new IOITDataContext())
                {
                    User checkUserNameExist = db.User.Where(f => f.UserName == data.UserName && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkUserNameExist != null)
                    {
                        def.meta = new Meta(211, "Tài khoản đã tồn tại!");
                        return Ok(def);
                    }

                    User checkEmailExist = db.User.Where(f => f.Email == data.Email && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmailExist != null)
                    {
                        def.meta = new Meta(2111, "Email đã được sử dụng cho tài khoản khác!");
                        return Ok(def);
                    }
                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận tài khoản!");
                        return Ok(def);
                    }

                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                    {
                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận tài khoản không chính xác!");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        User user = new User();
                        user.Address = data.Address;
                        user.FullName = data.FullName;
                        user.UserName = data.UserName;
                        user.Code = data.Code;
                        user.Email = data.Email;
                        user.Avata = data.Avata;
                        user.Password = Utils.GetMD5Hash(data.Password);
                        user.Phone = data.Phone;
                        user.UnitId = data.UnitId;
                        user.DepartmentId = data.DepartmentId;
                        user.PositionId = data.PositionId;
                        user.KeyLock = Utils.RandomString(20);
                        user.RegEmail = Utils.RandomString(8);
                        user.RoleMax = 9999;
                        user.RoleLevel = 99;
                        user.IsRoleGroup = data.IsRoleGroup != null ? data.IsRoleGroup : true;
                        user.CreatedAt = DateTime.Now;
                        user.UpdatedAt = DateTime.Now;
                        user.Status = (int)Const.Status.NORMAL;
                        user.UserCreateId = userId;
                        user.UserEditId = userId;
                        user.CompanyId = 1;
                        db.User.Add(user);
                        await db.SaveChangesAsync();
                        data.UserId = user.UserId;

                        //update pass
                        string pass = user.KeyLock.Trim() + user.RegEmail.Trim() + user.UserId + user.Password.Trim();
                        user.Password = Utils.GetMD5Hash(pass);

                        // role
                        var checkRole = false;
                        byte level = 99;
                        int max = 9999;
                        //add role 
                        foreach (var item in data.listRole)
                        {
                            var role = db.Role.Find(item.RoleId);
                            if (role != null)
                            {
                                UserRole userRole = new UserRole();
                                userRole.RoleId = item.RoleId;
                                userRole.UserId = user.UserId;
                                userRole.Status = (int)Const.Status.NORMAL;
                                db.UserRole.Add(userRole);
                                //check role
                                if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL" || role.Code.Trim() == "TKTC")
                                    checkRole = true;
                                //
                                if (role.LevelRole < level)
                                {
                                    level = (byte)role.LevelRole;
                                    max = role.RoleId;
                                }
                            }
                        }
                        //update cấp độ user và quyền cao nhất của user đó
                        user.RoleLevel = level;
                        user.RoleMax = max;

                        //add function
                        foreach (var item in data.listFunction)
                        {
                            FunctionRole functionRole = new FunctionRole();
                            functionRole.TargetId = data.UserId;
                            functionRole.FunctionId = item.FunctionId;
                            functionRole.ActiveKey = item.ActiveKey;
                            functionRole.Type = (int)Const.TypeFunction.FUNCTION_USER;
                            functionRole.CreatedAt = DateTime.Now;
                            functionRole.UpdatedAt = DateTime.Now;
                            functionRole.UserId = data.UserCreateId;
                            functionRole.Status = (int)Const.Status.NORMAL;
                            db.FunctionRole.Add(functionRole);
                        }



                        try
                        {
                            await db.SaveChangesAsync();

                            if (user.UserId > 0)
                            {
                                int customerId = 0;
                                //Thêm tài khoản sang customer
                                Customer checkEmail = db.Customer.Where(c => c.Username == data.Email.ToLower().Trim() && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (checkEmail == null)
                                {
                                    Customer customer = new Customer();
                                    customer.Username = data.Email;
                                    customer.FullName = data.FullName;
                                    customer.Email = data.Email.ToLower().Trim();
                                    customer.Password = "123456";
                                    customer.KeyRandom = Utils.RandomString(8);
                                    customer.Password = Utils.GetMD5Hash(customer.Password + customer.KeyRandom);
                                    customer.Phone = data.Phone;
                                    customer.Avata = data.Avata;
                                    //customer.Sex = data.S;
                                    //customer.Birthday = data.Birthday;
                                    customer.Address = data.Address;
                                    //customer.Note = data.Note;
                                    customer.IsEmailConfirm = true;
                                    customer.IsSentEmailConfirm = true;
                                    customer.IsPhoneConfirm = true;
                                    customer.Type = (int)Const.TypeCustomer.CUSTOMER_UNIT;
                                    customer.UnitId = data.UnitId;
                                    //customer.CountryId = data.CountryId;
                                    //customer.TypeId = data.TypeId;
                                    //customer.IdNumber = data.IdNumber;
                                    //customer.DateNumber = data.DateNumber;
                                    //customer.AddressNumber = data.AddressNumber;
                                    customer.PositionId = data.PositionId;
                                    //customer.AcademicRankId = data.AcademicRankId;
                                    //customer.DegreeId = data.DegreeId;
                                    //customer.RoleId = data.RoleId;
                                    customer.WebsiteId = 1;
                                    customer.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                                    customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_ADMIN;
                                    customer.LastLoginAt = DateTime.Now;
                                    customer.IsNotificationMail = true;
                                    customer.IsNotificationWeb = true;
                                    customer.UserId = userId;
                                    customer.CreatedAt = DateTime.Now;
                                    customer.UpdatedAt = DateTime.Now;
                                    customer.Status = (int)Const.Status.TEMP;
                                    await db.Customer.AddAsync(customer);
                                    await db.SaveChangesAsync();
                                    if (customer.CustomerId > 0)
                                    {
                                        //Gửi Email xác nhận
                                        if (customer.Email != null && customer.Email != "")
                                        {
                                            String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-confirm-resgister-customer.html");

                                            string key = Utils.GetMD5Hash(customer.KeyRandom + customer.Password);
                                            string linkConfirm = "'" + config.Website + "thiet-lap-mat-khau-" + key + "-" + customer.CustomerId + "'";
                                            //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                                            string keyRandom = "/thiet-lap-mat-khau-" + key + "-" + customer.CustomerId;
                                            sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                                config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                                config.Website, customer.FullName, linkConfirm, keyRandom);
                                            string subject = config.EmailSender + " - Xác thực tài khoản";
                                            //EmailService emailService = new EmailService();
                                            customer.IsSentEmailConfirm = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject, sBody);
                                        }
                                        db.Entry(customer).State = EntityState.Modified;
                                        //db.Entry(customer).State = EntityState.Detached;

                                        await db.SaveChangesAsync();

                                        customerId = customer.CustomerId;
                                    }
                                }
                                else
                                {
                                    customerId = checkEmail.CustomerId;
                                }
                                user.UserMapId = customerId;
                                db.User.Update(user);
                                await db.SaveChangesAsync();

                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Tạo tài khoản “" + data.FullName + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.UserId.ToString();
                                action.TargetName = data.FullName;
                                action.CompanyId = 1;
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
                                ////create action
                                //Models.EF.Action action = new Models.EF.Action();
                                //action.ActionName = "Tạo tài khoản";
                                //action.ActionType = "CREATE";
                                //action.TargetId = data.UserId;
                                //action.TargetType = "USER";
                                //action.Logs = JsonConvert.SerializeObject(data);
                                //action.Time = 0;
                                //action.Type = (int)Const.TypeAction.ACTION;
                                //action.CreatedAt = DateTime.Now;
                                //action.UserId = data.UserCreateId;
                                //action.Status = (int)Const.Status.NORMAL;
                                //db.Actions.Add(action);
                                //await db.SaveChangesAsync();

                                ////push action firebase
                                //Models.Data.Firebase.pushAction(action);
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
                            if (UserRoleExists(data.UserId))
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
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Function/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
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
                    User data = await db.User.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    if (id == 1)
                    {
                        def.meta = new Meta(210, "Not delete Admin Super");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //delete user
                        data.UserEditId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;

                        //delete user role
                        var userRoles = db.UserRole.Where(e => e.UserId == id && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in userRoles)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //delete function role
                        var functionRoles = db.FunctionRole.Where(e => e.TargetId == id
                        && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in functionRoles)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            item.UpdatedAt = DateTime.Now;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //delete customer
                        var customer = await db.Customer.Where(e => e.CustomerId == data.UserMapId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                        if (customer != null)
                        {
                            customer.Status = (int)Const.Status.DELETED;
                            db.Entry(customer).State = EntityState.Modified;
                        }
                        //delete user unit
                        //var userUnit = db.UserProjects.Where(e => e.UserId == id
                        //&& e.Type == (int)Const.TypeUserProject.USER_UNIT
                        //&& e.Status != (int)Const.Status.DELETED).ToList();
                        //foreach (var item in userUnit)
                        //{
                        //    item.Status = (int)Const.Status.DELETED;
                        //    item.UpdatedAt = DateTime.Now;
                        //    db.Entry(item).State = EntityState.Modified;
                        //}

                        //delete user project
                        //var userProject = db.UserProjects.Where(e => e.UserId == id
                        //&& e.Type == (int)Const.TypeUserProject.USER_PROJECT
                        //&& e.Status != (int)Const.Status.DELETED).ToList();
                        //foreach (var item in userProject)
                        //{
                        //    item.Status = (int)Const.Status.DELETED;
                        //    item.UpdatedAt = DateTime.Now;
                        //    db.Entry(item).State = EntityState.Modified;
                        //}

                        try
                        {
                            await db.SaveChangesAsync();
                            if (data.UserId > 0)
                            {
                                transaction.Commit();
                                //create action
                                IOITWebApp31.Models.EF.Action action = new IOITWebApp31.Models.EF.Action();
                                action.ActionName = "Xóa tài khoản";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.UserId + "";
                                action.TargetName = data.FullName + " - " + data.UserName;
                                action.Logs = action.ActionName + " " + action.TargetName;
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = data.UserId;
                                action.UserId = data.UserId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                //push action
                                IOITWebApp31.Models.Data.Firebase.pushAction(action);
                                //push user firebase
                                var tasks = new[]
                                {
                                    Task.Run(() => IOITWebApp31.Models.Data.Firebase.updateUser(data))
                                };
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = id;
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserRoleExists(id))
                            {
                                def.meta = new Meta(500, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        //API xóa danh sách người dùng
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteUserRoles([FromBody] int[] data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
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
                        User user = await db.User.FindAsync(data[i]);

                        if (user == null)
                        {
                            continue;
                        }


                        if (user.UserId == 1)
                        {
                            def.meta = new Meta(210, "Not delete Admin Super");
                            return Ok(def);
                        }

                        user.UpdatedAt = DateTime.Now;
                        user.Status = (int)Const.Status.DELETED;
                        db.Entry(user).State = EntityState.Modified;

                        //delete user role
                        var userRoles = db.UserRole.Where(e => e.UserId == user.UserId && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in userRoles)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //delete function role
                        var functionRoles = db.FunctionRole.Where(e => e.TargetId == user.UserId
                        && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in functionRoles)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            item.UpdatedAt = DateTime.Now;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        //delete customer
                        var customer = await db.Customer.Where(e => e.CustomerId == user.UserMapId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                        if (customer != null)
                        {
                            customer.Status = (int)Const.Status.DELETED;
                            db.Entry(customer).State = EntityState.Modified;
                        }

                        //lưu vào bảng action nhưng không bắn thông báo
                        IOITWebApp31.Models.EF.Action action = new IOITWebApp31.Models.EF.Action();
                        action.ActionName = "Xóa tài khoản";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = user.UserId + "";
                        action.TargetName = user.FullName + " - " + user.UserName;
                        action.Logs = action.ActionName + " " + action.TargetName;
                        action.Time = 0;
                        action.Type = (int)Const.TypeAction.ACTION;
                        action.CreatedAt = DateTime.Now;
                        action.UserPushId = user.UserId;
                        action.UserId = user.UserId;
                        action.Status = (int)Const.Status.NORMAL;
                        db.Action.Add(action);
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

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private bool UserRoleExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.User.Count(e => e.UserId == id) > 0;
            }
        }
    }
}
