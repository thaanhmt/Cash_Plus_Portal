using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("customer", "customer");
        private static string functionCode = "QLND";
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public CustomerController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginV2([FromBody] LoginModel loginModel)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (loginModel != null)
                {
                    string username = loginModel.email;

                    using (var db = new IOITDataContext())
                    {
                        bool check = await LoginSSO(loginModel);
                        var customer = db.Customer.Where(e => e.Username == username && e.Status != (int)Const.Status.DELETED).ToList();
                        if (customer.Count > 0)
                        {
                            string password = loginModel.password.Trim() + customer.FirstOrDefault().KeyRandom;
                            password = Utils.GetMD5Hash(password);
                            CustomerLogin userLogin = db.Customer.Where(e => e.Username == username && e.Password == password && e.Status != (int)Const.Status.DELETED).Select(e => new CustomerLogin()
                            {
                                CustomerId = e.CustomerId,
                                Username = e.Username,
                                Email = e.Email,
                                FullName = e.FullName,
                                Avata = e.Avata,
                                Address = e.Address,
                                Password = e.Password,
                                PhomeNumber = e.Phone,
                                Status = e.Status,
                                Type = e.Type,
                                RoleId = e.RoleId,
                                Sex = e.Sex,
                                IsEmailConfirm = e.IsEmailConfirm,
                                TypeThirdId = e.TypeThirdId,
                                CreatedAt = e.CreatedAt,
                                KeyRandom = e.KeyRandom,
                                KeyToken = e.KeyToken
                            }).FirstOrDefault();

                            if (userLogin != null)
                            {
                                //check if user lock
                                if (userLogin.Status == (int)Const.Status.LOCK)
                                {
                                    def.meta = new Meta(223, "Tài khoản đang bị khóa!");
                                    return Ok(def);
                                }

                                //check if user not confirm yet
                                if (userLogin.IsEmailConfirm == false || userLogin.Status == (int)Const.Status.TEMP)
                                {
                                    def.meta = new Meta(223, "Tài khoản chưa được kích hoạt!");
                                    return Ok(def);
                                }

                                var userId = userLogin.CustomerId;

                                //check role tạo accetkey ở đây
                                List<MenuDTO> listFunctionRole = new List<MenuDTO>();
                                var listFRR = db.FunctionRole.Where(e => e.TargetId == userLogin.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                            && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Function.Location).ToList();
                                foreach (var itemFR in listFRR)
                                {
                                    //check exits
                                    var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
                                    if (fr.Count > 0)
                                    {
                                        string key1 = fr.FirstOrDefault().ActiveKey;
                                        if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
                                        {
                                            key1 = plusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
                                        }
                                        fr.FirstOrDefault().ActiveKey = key1;
                                    }
                                    else
                                    {
                                        Function function = db.Function.Where(e => e.FunctionId == itemFR.FunctionId).FirstOrDefault();
                                        if (function != null)
                                        {
                                            MenuDTO menu = new MenuDTO();
                                            menu.MenuId = itemFR.FunctionId;
                                            menu.Code = function.Code;
                                            menu.Name = function.Name;
                                            menu.Url = function.Url;
                                            menu.Icon = function.Icon;
                                            menu.MenuParent = (int)function.FunctionParentId;
                                            menu.ActiveKey = itemFR.ActiveKey;
                                            listFunctionRole.Add(menu);
                                        }
                                    }
                                }

                                string access_key = "";
                                int count = listFunctionRole.Count;
                                if (count > 0)
                                {
                                    for (int i = 0; i < count - 1; i++)
                                    {
                                        if (listFunctionRole[i].ActiveKey != "000000000")
                                        {
                                            access_key += listFunctionRole[i].Code + ":" + listFunctionRole[i].ActiveKey + "-";
                                        }
                                    }

                                    access_key = access_key + listFunctionRole[count - 1].Code + ":" + listFunctionRole[count - 1].ActiveKey;
                                }

                                userLogin.access_key = access_key;
                                userLogin.listMenus = CreateMenu(listFunctionRole, 0);
                                //Nếu là tk tổ chức check xem ng dùng trên ql những đơn vị nào
                                var listU = await db.CustomerMapping.Where(e => e.CustomerId == userLogin.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                List<int> listUnits = new List<int>();
                                //
                                var listInputUnits = await db.Unit.Where(x => x.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var unit in listU)
                                {
                                    listUnits.Add((int)unit.TargetId);
                                    await GetListUnit(listUnits, listInputUnits, (int)unit.TargetId, db);
                                }
                                userLogin.listUnits = listUnits;
                                string listUs = "";
                                foreach (var item in listUnits)
                                {
                                    listUs += item + "-";
                                }
                                if (listUs != "")
                                    listUs = listUs.Substring(0, listUs.Length - 1);
                                var claims = new List<Claim>
                                {
                                    new Claim(JwtRegisteredClaimNames.Email, userLogin.Email),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.NameIdentifier, userLogin.CustomerId.ToString()),
                                    new Claim(ClaimTypes.Name, userLogin.FullName),
                                        new Claim("CustomerId", userLogin.CustomerId != null ? userLogin.CustomerId.ToString() : ""),
                                        new Claim("ListUnits", listUs),
                                        //new Claim("RoleMax", userLogin.roleMax != null ? userLogin.roleMax.ToString() : ""),
                                        new Claim("TypeThirdId", userLogin.TypeThirdId != null ? userLogin.TypeThirdId.ToString() : ""),
                                        new Claim("AccessKey", access_key != null ? access_key : ""),
                                        //new Claim("LanguageId", access_key != null ? languageId.ToString() : ""),
                                        new Claim("Type", userLogin.Type != null ? userLogin.Type.ToString() : ""),
                                };
                                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
                                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                                var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));
                                var token = new JwtSecurityToken(
                                    _configuration["AppSettings:JwtIssuer"],
                                    _configuration["AppSettings:JwtIssuer"],
                                    claims,
                                    expires: expires,
                                    signingCredentials: creds
                                );
                                userLogin.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                                //HttpContext.Session.SetInt32("CustomerId", userLogin.CustomerId);
                                //HttpContext.Session.SetString("access_token", userLogin.access_token);
                                //var option = new CookieOptions();
                                //option.Expires = DateTime.Now.AddMinutes(600);
                                //Response.Cookies.Append("CustomerId", userLogin.CustomerId.ToString(), option);
                                var userU = await db.Customer.Where(e => e.CustomerId == userLogin.CustomerId).FirstOrDefaultAsync();
                                if (userLogin.TypeThirdId != (int)Const.TypeThird.CUSTOMER_KEYLOCK && userLogin.KeyToken != null
                                    && userLogin.KeyToken != "")
                                {
                                    try
                                    {
                                        CustomerDTO data = new CustomerDTO();
                                        data.FullName = userLogin.FullName;
                                        data.Username = userLogin.Username;
                                        data.Password = Security.Decrypt(userLogin.KeyRandom, userLogin.KeyToken);
                                        data.Email = userLogin.Email;
                                        string keycloakUrl = _configuration["KeycloakSettings:endpoint"];
                                        string keycloakClientId = _configuration["KeycloakSettings:client_id"];
                                        string keycloakClientSecret = _configuration["KeycloakSettings:client_secret"];
                                        string keycloakClientKey = _configuration["KeycloakSettings:client_key"];
                                        string keycloakClientValue = _configuration["KeycloakSettings:client_value"];
                                        string keycloakRealm = _configuration["KeycloakSettings:realm"];
                                        bool checkCreate = await CreateUserAsync(keycloakUrl, keycloakClientId, keycloakClientSecret,
                                            keycloakClientKey, keycloakClientValue, keycloakRealm, data);
                                        if (checkCreate)
                                        {
                                            userU.TypeThirdId = (int)Const.TypeThird.CUSTOMER_KEYLOCK;
                                        }
                                    }
                                    catch { }
                                }

                                userU.LastLoginAt = DateTime.Now;
                                db.Customer.Update(userU);
                                await db.SaveChangesAsync();
                                //Lấy số thông báo
                                var notification = await db.Notification.Where(e => e.UserReadId == userLogin.CustomerId
                                && e.Status == (int)Const.Status.TEMP).ToListAsync();
                                userLogin.NunberNotification = notification.Count;
                                InitSession(userLogin);

                                def.data = userLogin;
                                def.meta = new Meta(200, "Đăng nhập thành công!");
                                return Ok(def);
                            }
                            else
                            {
                                //check nếu login sso ok
                                if (check)
                                {
                                    CustomerLogin userLoginSso = db.Customer.Where(e => e.Username == username && e.Status != (int)Const.Status.DELETED).Select(e => new CustomerLogin()
                                    {
                                        CustomerId = e.CustomerId,
                                        Username = e.Username,
                                        Email = e.Email,
                                        FullName = e.FullName,
                                        Avata = e.Avata,
                                        Address = e.Address,
                                        Password = e.Password,
                                        PhomeNumber = e.Phone,
                                        Status = e.Status,
                                        Type = e.Type,
                                        RoleId = e.RoleId,
                                        Sex = e.Sex,
                                        IsEmailConfirm = e.IsEmailConfirm,
                                        TypeThirdId = e.TypeThirdId,
                                        CreatedAt = e.CreatedAt
                                    }).FirstOrDefault();

                                    if (userLoginSso != null)
                                    {
                                        //check if user lock
                                        if (userLoginSso.Status == (int)Const.Status.LOCK)
                                        {
                                            def.meta = new Meta(223, "Tài khoản đang bị khóa!");
                                            return Ok(def);
                                        }

                                        //check if user not confirm yet
                                        if (userLoginSso.IsEmailConfirm == false || userLoginSso.Status == (int)Const.Status.TEMP)
                                        {
                                            def.meta = new Meta(223, "Tài khoản chưa được kích hoạt!");
                                            return Ok(def);
                                        }

                                        var userId = userLoginSso.CustomerId;

                                        //check role tạo accetkey ở đây
                                        List<MenuDTO> listFunctionRole = new List<MenuDTO>();
                                        var listFRR = db.FunctionRole.Where(e => e.TargetId == userLoginSso.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                                    && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Function.Location).ToList();
                                        foreach (var itemFR in listFRR)
                                        {
                                            //check exits
                                            var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
                                            if (fr.Count > 0)
                                            {
                                                string key1 = fr.FirstOrDefault().ActiveKey;
                                                if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
                                                {
                                                    key1 = plusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
                                                }
                                                fr.FirstOrDefault().ActiveKey = key1;
                                            }
                                            else
                                            {
                                                Function function = db.Function.Where(e => e.FunctionId == itemFR.FunctionId).FirstOrDefault();
                                                if (function != null)
                                                {
                                                    MenuDTO menu = new MenuDTO();
                                                    menu.MenuId = itemFR.FunctionId;
                                                    menu.Code = function.Code;
                                                    menu.Name = function.Name;
                                                    menu.Url = function.Url;
                                                    menu.Icon = function.Icon;
                                                    menu.MenuParent = (int)function.FunctionParentId;
                                                    menu.ActiveKey = itemFR.ActiveKey;
                                                    listFunctionRole.Add(menu);
                                                }
                                            }
                                        }

                                        string access_key = "";
                                        int count = listFunctionRole.Count;
                                        if (count > 0)
                                        {
                                            for (int i = 0; i < count - 1; i++)
                                            {
                                                if (listFunctionRole[i].ActiveKey != "000000000")
                                                {
                                                    access_key += listFunctionRole[i].Code + ":" + listFunctionRole[i].ActiveKey + "-";
                                                }
                                            }

                                            access_key = access_key + listFunctionRole[count - 1].Code + ":" + listFunctionRole[count - 1].ActiveKey;
                                        }

                                        userLoginSso.access_key = access_key;
                                        userLoginSso.listMenus = CreateMenu(listFunctionRole, 0);
                                        //Nếu là tk tổ chức check xem ng dùng trên ql những đơn vị nào
                                        var listU = await db.CustomerMapping.Where(e => e.CustomerId == userLoginSso.CustomerId
                                        && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                        && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                        List<int> listUnits = new List<int>();
                                        //
                                        var listInputUnits = await db.Unit.Where(x => x.Status != (int)Const.Status.DELETED).ToListAsync();
                                        foreach (var unit in listU)
                                        {
                                            listUnits.Add((int)unit.TargetId);
                                            await GetListUnit(listUnits, listInputUnits, (int)unit.TargetId, db);
                                        }
                                        userLoginSso.listUnits = listUnits;
                                        string listUs = "";
                                        foreach (var item in listUnits)
                                        {
                                            listUs += item + "-";
                                        }
                                        if (listUs != "")
                                            listUs = listUs.Substring(0, listUs.Length - 1);
                                        var claims = new List<Claim>
                                        {
                                            new Claim(JwtRegisteredClaimNames.Email, userLoginSso.Email),
                                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                            new Claim(ClaimTypes.NameIdentifier, userLoginSso.CustomerId.ToString()),
                                            new Claim(ClaimTypes.Name, userLoginSso.FullName),
                                                new Claim("CustomerId", userLoginSso.CustomerId != null ? userLoginSso.CustomerId.ToString() : ""),
                                                new Claim("ListUnits", listUs),
                                                //new Claim("RoleMax", userLogin.roleMax != null ? userLogin.roleMax.ToString() : ""),
                                                new Claim("TypeThirdId", userLoginSso.TypeThirdId != null ? userLoginSso.TypeThirdId.ToString() : ""),
                                                new Claim("AccessKey", access_key != null ? access_key : ""),
                                                //new Claim("LanguageId", access_key != null ? languageId.ToString() : ""),
                                                new Claim("Type", userLoginSso.Type != null ? userLoginSso.Type.ToString() : ""),
                                        };
                                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
                                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                                        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));
                                        var token = new JwtSecurityToken(
                                            _configuration["AppSettings:JwtIssuer"],
                                            _configuration["AppSettings:JwtIssuer"],
                                            claims,
                                            expires: expires,
                                            signingCredentials: creds
                                        );
                                        userLoginSso.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                                        //HttpContext.Session.SetInt32("CustomerId", userLogin.CustomerId);
                                        //HttpContext.Session.SetString("access_token", userLogin.access_token);
                                        //var option = new CookieOptions();
                                        //option.Expires = DateTime.Now.AddMinutes(600);
                                        //Response.Cookies.Append("CustomerId", userLogin.CustomerId.ToString(), option);
                                        //update thông tin đăng nhập
                                        var userU = await db.Customer.Where(e => e.CustomerId == userLoginSso.CustomerId).FirstOrDefaultAsync();
                                        if (userLoginSso.TypeThirdId != (int)Const.TypeThird.CUSTOMER_KEYLOCK && userLoginSso.KeyToken != null
                                            && userLoginSso.KeyToken != "")
                                        {
                                            try
                                            {
                                                CustomerDTO data = new CustomerDTO();
                                                data.FullName = userLoginSso.FullName;
                                                data.Username = userLoginSso.Username;
                                                data.Password = Security.Decrypt(userLoginSso.KeyRandom, userLoginSso.KeyToken);
                                                data.Email = userLoginSso.Email;
                                                string keycloakUrl = _configuration["KeycloakSettings:endpoint"];
                                                string keycloakClientId = _configuration["KeycloakSettings:client_id"];
                                                string keycloakClientSecret = _configuration["KeycloakSettings:client_secret"];
                                                string keycloakClientKey = _configuration["KeycloakSettings:client_key"];
                                                string keycloakClientValue = _configuration["KeycloakSettings:client_value"];
                                                string keycloakRealm = _configuration["KeycloakSettings:realm"];
                                                bool checkCreate = await CreateUserAsync(keycloakUrl, keycloakClientId, keycloakClientSecret,
                                                    keycloakClientKey, keycloakClientValue, keycloakRealm, data);
                                                if (checkCreate)
                                                {
                                                    userU.TypeThirdId = (int)Const.TypeThird.CUSTOMER_KEYLOCK;
                                                }
                                            }
                                            catch { }
                                        }

                                        userU.LastLoginAt = DateTime.Now;
                                        db.Customer.Update(userU);
                                        await db.SaveChangesAsync();
                                        //Lấy số thông báo
                                        InitSession(userLoginSso);

                                        def.data = userLoginSso;
                                        def.meta = new Meta(200, "Đăng nhập thành công!");
                                        return Ok(def);
                                    }
                                    else
                                    {
                                        //check if email exist
                                        var existed = db.Customer.Where(e => e.Email == username && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                        if (existed != null)
                                        {
                                            def.meta = new Meta(213, "Tài khoản hoặc mật khẩu không chính xác!");
                                            return Ok(def);
                                        }
                                        else
                                        {
                                            def.meta = new Meta(404, "Tài khoản hoặc mật khẩu không chính xác!");
                                            return Ok(def);
                                        }
                                    }
                                }
                                else
                                {
                                    //check if email exist
                                    var existed = db.Customer.Where(e => e.Username == username && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (existed != null)
                                    {
                                        def.meta = new Meta(213, "Tài khoản hoặc mật khẩu không chính xác!");
                                        return Ok(def);
                                    }
                                    else
                                    {
                                        def.meta = new Meta(404, "Tài khoản hoặc mật khẩu không chính xác!");
                                        return Ok(def);
                                    }
                                }
                            }
                        }
                        else
                        {
                            def.meta = new Meta(404, "Tài khoản hoặc mật khẩu không chính xác!");
                            return Ok(def);
                        }
                    }
                }
                else
                {
                    def.meta = new Meta(400, "Tài khoản hoặc mật khẩu không chính xác!");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Info("Exception:" + e);
                def.meta = new Meta(500, "Server xảy ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        public async Task<bool> LoginSSO([FromBody] LoginModel loginModel)
        {
            try
            {
                if (loginModel != null)
                {

                    using (var db = new IOITDataContext())
                    {
                        string keycloakUrl = _configuration["KeycloakSettings:endpoint"];
                        string keycloakClientId = _configuration["KeycloakSettings:client_id"];
                        string keycloakClientSecret = _configuration["KeycloakSettings:client_secret"];
                        string keycloakRealm = _configuration["KeycloakSettings:realm"];

                        // Yêu cầu token
                        var tokenResponse = await GetTokenAsync(keycloakUrl, keycloakClientId, keycloakClientSecret, loginModel.email, loginModel.password, keycloakRealm);

                        // Xác thực token
                        var authenticated = await ValidateTokenAsync(keycloakUrl, keycloakClientId, keycloakClientSecret, tokenResponse.access_token, keycloakRealm);

                        if (authenticated.active)
                        {
                            //kiểm tra xem email đó có trong tk hệ thống bên mình ko
                            var checkEmail = db.Customer.Where(x => x.Username == authenticated.preferred_username && x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (checkEmail == null)
                            {
                                //Tạo tài khoản
                                Customer customer = new Customer();
                                customer.Username = authenticated.preferred_username;
                                customer.Password = Utils.GetMD5Hash(Utils.RandomString(8));
                                customer.FullName = authenticated.name != null ? authenticated.name : authenticated.preferred_username;
                                customer.Email = authenticated.email != null ? authenticated.email.ToLower().Trim() : authenticated.email;
                                customer.KeyRandom = Utils.RandomString(8);
                                string passWordHash = loginModel.password + customer.KeyRandom;
                                customer.Password = Utils.GetMD5Hash(passWordHash);
                                customer.IsNotificationMail = true;
                                customer.IsNotificationWeb = true;
                                customer.IsEmailConfirm = true;
                                customer.IsSentEmailConfirm = true;
                                customer.IsPhoneConfirm = true;
                                //customer.UnitId = data.UnitId;
                                customer.Type = (int)Const.TypeCustomer.CUSTOMER_PERSONAL;
                                customer.WebsiteId = Const.WEBSITEID;
                                customer.CompanyId = Const.COMPANYID;
                                customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_KEYLOCK;
                                customer.LastLoginAt = DateTime.Now;
                                customer.CreatedAt = DateTime.Now;
                                customer.UpdatedAt = DateTime.Now;
                                customer.Status = (int)Const.Status.NORMAL;

                                //Thêm quyền
                                string roleCode = "USER_FULL";
                                //if (customer.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                //    roleCode = "USER_FULL";
                                var role = await db.Role.Where(e => e.Code.Trim() == roleCode).FirstOrDefaultAsync();
                                if (role != null)
                                {
                                    CustomerMapping mapping = new CustomerMapping();
                                    mapping.CustomerId = customer.CustomerId;
                                    mapping.TargetId = role.RoleId;
                                    mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                                    mapping.CreatedId = 1;
                                    mapping.UpdatedId = 1;
                                    mapping.CreatedAt = DateTime.Now;
                                    mapping.UpdatedAt = DateTime.Now;
                                    mapping.Status = (int)Const.Status.NORMAL;
                                    await db.CustomerMapping.AddAsync(mapping);
                                    customer.RoleId = role.RoleId;
                                }

                                await db.Customer.AddAsync(customer);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                //cập nhật lại thông tin tài khoản ở đây
                                if (checkEmail.TypeThirdId == (int)Const.TypeThird.CUSTOMER_KEYLOCK)
                                {
                                    checkEmail.FullName = authenticated.name != null ? authenticated.name : checkEmail.FullName;
                                    checkEmail.Email = authenticated.email != null ? authenticated.email.ToLower().Trim() : checkEmail.Email;
                                    string passWordHash = loginModel.password + checkEmail.KeyRandom;
                                    checkEmail.Password = Utils.GetMD5Hash(passWordHash);
                                    db.Customer.Update(checkEmail);
                                    await db.SaveChangesAsync();
                                }
                            }

                            return true;
                        }
                        else
                        {
                            return false;
                        }


                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        static async Task<TokenResponse> GetTokenAsync(string keycloakUrl, string keycloakClientId, string keycloakClientSecret,
            string username, string password, string realm)
        {
            using (var client = new HttpClient())
            {
                string keycloakUrlToken = $"{keycloakUrl}/auth/realms/{realm}";
                // Tạo yêu cầu để lấy token
                var request = new HttpRequestMessage(HttpMethod.Post, keycloakUrlToken + "/protocol/openid-connect/token");
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("client_id", keycloakClientId),
                    new KeyValuePair<string, string>("client_secret", keycloakClientSecret)
                });

                // Gửi yêu cầu và lấy kết quả
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Phân tích kết quả
                var tokenResponse = JObject.Parse(responseContent).ToObject<TokenResponse>();

                return tokenResponse;
            }
        }

        static async Task<IntrospectionResponse> ValidateTokenAsync(string keycloakUrl, string keycloakClientId, string keycloakClientSecret,
            string accessToken, string realm)
        {
            using (var client = new HttpClient())
            {
                // Tạo yêu cầu để xác thực token
                var request2 = new HttpRequestMessage(HttpMethod.Post, $"{keycloakUrl}/auth/realms/{realm}/protocol/openid-connect/token/introspect");
                request2.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", accessToken),
                    new KeyValuePair<string, string>("client_id", keycloakClientId),
                    new KeyValuePair<string, string>("client_secret", keycloakClientSecret)
                });

                // Gửi yêu cầu và lấy kết quả
                var response2 = await client.SendAsync(request2);
                var responseContent2 = await response2.Content.ReadAsStringAsync();

                // Phân tích kết quả
                var introspectionResponse = JObject.Parse(responseContent2).ToObject<IntrospectionResponse>();

                return introspectionResponse;
            }
        }

        static async Task<bool> CreateUserAsync(string keycloakUrl, string keycloakClientId, string keycloakClientSecret,
            string keycloakClientKey, string keycloakClientValue, string realm, CustomerDTO data)
        {
            //string adminUsername = "admin"; // Tên người dùng admin
            //string adminPassword = _configuration["KeycloakSettings:endpoint"]; // Mật khẩu người dùng admin
            //string realm = "master";
            //string keycloakUrlToken =  $"{keycloakUrl}/auth/realms/{realm}";
            var token = await GetTokenAsync(keycloakUrl, keycloakClientId, keycloakClientSecret, keycloakClientKey, keycloakClientValue, realm);
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{keycloakUrl}/auth/admin/realms/{realm}/");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

            // Thêm người dùng mới với mật khẩu
            var newUser = new
            {
                username = data.Username,
                email = data.Email,
                enabled = true,
                firstName = "",
                lastName = data.FullName,
                credentials = new[]
                {
                new
                {
                    type = "password",
                    value = data.Password,
                    temporary = false
                }
            }
            };

            var addUserResponse = await httpClient.PostAsync("users",
                new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json"));

            if (addUserResponse.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                //string Encrypt = Security.Encrypt("123456!@#", data.Password);
                //string Decrypt = Security.Decrypt("123456!@#", Encrypt);
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (data.FullName == null || data.FullName == "")
                {
                    def.meta = new Meta(211, "Tên không được để trống!");
                    return Ok(def);
                }
                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }
                if (data.Password == null || data.Password == "")
                {
                    def.meta = new Meta(211, "Mật khẩu không được để trống!");
                    return Ok(def);
                }
                if (data.Password != data.ConfirmPassword)
                {
                    def.meta = new Meta(211, "Mật khẩu xác nhận không đúng!");
                    return Ok(def);
                }
                if (!Security.IsPasswordValid(data.Password))
                {
                    def.meta = new Meta(211, "Mật khẩu phải đảm bảo 8 ký tự bao gồm chữ hoa, chữ thường và ít nhất 1 ký tự đặc biệt!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExistEmail = db.Customer.Where(e => e.Email == data.Email.ToLower().Trim() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }

                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                        return Ok(def);
                    }

                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                    {
                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        string keyRandom = Utils.RandomString(8);
                        string passWordHash = data.Password + keyRandom;
                        Customer customer = new Customer();
                        customer.Username = data.Email.ToLower().Trim();
                        customer.Password = Utils.GetMD5Hash(passWordHash);
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.KeyRandom = keyRandom;
                        customer.IsEmailConfirm = false;
                        customer.IsSentEmailConfirm = false;
                        customer.IsPhoneConfirm = true;
                        customer.IsViewInfo = false;
                        customer.IsNotificationMail = false;
                        customer.IsNotificationWeb = false;
                        customer.UnitId = data.UnitId;
                        customer.Type = data.IsUnit ? (int)Const.TypeCustomer.CUSTOMER_UNIT : (int)Const.TypeCustomer.CUSTOMER_PERSONAL;
                        customer.WebsiteId = Const.WEBSITEID;
                        customer.CompanyId = Const.COMPANYID;
                        customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_USER;
                        customer.KeyToken = Security.Encrypt(keyRandom, data.Password);
                        customer.LastLoginAt = DateTime.Now;
                        customer.CreatedAt = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = (int)Const.Status.TEMP;
                        await db.Customer.AddAsync(customer);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;

                            //Gửi Email xác nhận
                            if (customer.Email != null && customer.Email != "")
                            {
                                String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-confirm-resgister-customer.html");

                                string key = Utils.GetMD5Hash(customer.KeyRandom + customer.Password);
                                string linkConfirm1 = "'" + config.Website + "xac-thuc-tai-khoan-" + key + "-" + customer.CustomerId + "'";
                                //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                                data.KeyRandom = "/xac-thuc-tai-khoan-" + key + "-" + customer.CustomerId;
                                sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                    config.Website, customer.FullName, linkConfirm1, customer.KeyRandom);
                                string subject1 = config.EmailSender + " - Xác thực tài khoản";
                                //EmailService emailService = new EmailService();
                                customer.IsSentEmailConfirm = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject1, sBody);
                                //if (customer.IsSentEmailConfirm == false)
                                //{
                                //    def.meta = new Meta(404, "Không gửi được thông tin xác nhận đến Email " + customer.Email +", bạn vui lòng kiểm tra lại thông tin Email!");
                                //    return Ok(def);
                                //}
                            }
                            db.Entry(customer).State = EntityState.Modified;

                            //Tạo tk trên keyclock

                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Đăng ký tài khoản thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        [HttpPost("registerV2")]
        public async Task<IActionResult> RegisterV2(CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (data.FullName == null || data.FullName == "")
                {
                    def.meta = new Meta(211, "Tên không được để trống!");
                    return Ok(def);
                }
                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }
                if (data.Password == null || data.Password == "")
                {
                    def.meta = new Meta(211, "Mật khẩu không được để trống!");
                    return Ok(def);
                }
                if (data.Password != data.ConfirmPassword)
                {
                    def.meta = new Meta(211, "Mật khẩu xác nhận không đúng!");
                    return Ok(def);
                }
                if (!Security.IsPasswordValid(data.Password))
                {
                    def.meta = new Meta(211, "Mật khẩu phải đảm bảo 8 ký tự bao gồm chữ hoa, chữ thường và ít nhất 1 ký tự đặc biệt!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExistEmail = db.Customer.Where(e => e.Email == data.Email.ToLower().Trim() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }

                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                        return Ok(def);
                    }

                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                    {
                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        string keyRandom = Utils.RandomString(8);
                        string passWordHash = data.Password + keyRandom;
                        Customer customer = new Customer();
                        customer.Username = data.Email.ToLower().Trim();
                        customer.Password = Utils.GetMD5Hash(passWordHash);
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.KeyRandom = keyRandom;
                        customer.IsEmailConfirm = false;
                        customer.IsSentEmailConfirm = false;
                        customer.IsPhoneConfirm = true;
                        customer.IsViewInfo = false;
                        customer.IsNotificationMail = false;
                        customer.IsNotificationWeb = false;
                        customer.UnitId = data.UnitId;
                        customer.Type = data.IsUnit ? (int)Const.TypeCustomer.CUSTOMER_UNIT : (int)Const.TypeCustomer.CUSTOMER_PERSONAL;
                        customer.WebsiteId = Const.WEBSITEID;
                        customer.CompanyId = Const.COMPANYID;
                        customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_USER;
                        customer.LastLoginAt = DateTime.Now;
                        customer.CreatedAt = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = (int)Const.Status.TEMP;
                        await db.Customer.AddAsync(customer);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;
                            //update lại password
                            //string passWordHash = data.Password + checkExist.KeyRandom;
                            //checkExist.Password = Utils.GetMD5Hash(passWordHash);
                            //checkExist.IsEmailConfirm = true;
                            //checkExist.IsNotificationMail = true;
                            //checkExist.IsNotificationWeb = true;
                            //checkExist.UpdatedAt = DateTime.Now;
                            //db.Customer.Update(checkExist);

                            //Gửi Email xác nhận
                            if (customer.Email != null && customer.Email != "")
                            {
                                String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-confirm-resgister-customer.html");

                                string key = Utils.GetMD5Hash(customer.KeyRandom + customer.Password);
                                string linkConfirm1 = "'" + config.Website + "xac-thuc-tai-khoan-" + key + "-" + customer.CustomerId + "'";
                                //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                                data.KeyRandom = "/xac-thuc-tai-khoan-" + key + "-" + customer.CustomerId;
                                sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                    config.Website, customer.FullName, linkConfirm1, customer.KeyRandom);
                                string subject1 = config.EmailSender + " - Xác thực tài khoản";
                                //EmailService emailService = new EmailService();
                                customer.IsSentEmailConfirm = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject1, sBody);
                                //if (customer.IsSentEmailConfirm == false)
                                //{
                                //    def.meta = new Meta(404, "Không gửi được thông tin xác nhận đến Email " + customer.Email +", bạn vui lòng kiểm tra lại thông tin Email!");
                                //    return Ok(def);
                                //}
                            }
                            db.Entry(customer).State = EntityState.Modified;

                            ////Gửi Email và thông báo
                            //string subject = config.EmailSender + " -  Duyệt tài khoản đăng ký mới";
                            ////Tạo thông báo
                            //string linkConfirm = "nguoi-dung-to-chuc";
                            ////Nếu tài khoản cá nhân link vào cms, nếu tài khoản tổ chức link vào tổ chức
                            //if (customer.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                            //{
                            //    linkConfirm = "cms/system/user-data";
                            //}
                            ////Lấy ds người nhận, nếu là cá nhân thì các tk có quyền ql ng dùng,
                            ////nếu tổ chức là các tk quản lý tổ chức đó
                            //linkConfirm = config.Website + linkConfirm;
                            //string linkConfirmUrl = "'" + linkConfirm + "'";
                            //if (customer.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                            //{
                            //    var listUser = await (from u in db.User
                            //                          join c in db.Customer on u.UserMapId equals c.CustomerId
                            //                          join ur in db.UserRole on u.UserId equals ur.UserId
                            //                          join fr in db.FunctionRole on ur.RoleId equals fr.TargetId
                            //                          join f in db.Function on fr.FunctionId equals f.FunctionId
                            //                          where u.Status == (int)Const.Status.NORMAL
                            //                          && c.Status == (int)Const.Status.NORMAL
                            //                          && ur.Status != (int)Const.Status.DELETED
                            //                          && fr.Status != (int)Const.Status.DELETED
                            //                          && f.Status != (int)Const.Status.DELETED
                            //                          && fr.ActiveKey.Substring(0, 1) == "1" && fr.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                            //                          && f.Code == "QLND"
                            //                          select c).GroupBy(e => new
                            //                          {
                            //                              e.CustomerId,
                            //                              e.Email,
                            //                              e.FullName,
                            //                              e.IsNotificationMail,
                            //                              e.IsNotificationWeb
                            //                          }).Select(e => new
                            //                          {
                            //                              CustomerId = e.Key.CustomerId,
                            //                              Email = e.Key.Email,
                            //                              FullName = e.Key.FullName,
                            //                              IsNotificationMail = e.Key.IsNotificationMail,
                            //                              IsNotificationWeb = e.Key.IsNotificationWeb,
                            //                          }).ToListAsync();
                            //    foreach (var item in listUser)
                            //    {
                            //        String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-register.html");
                            //        sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                            //           config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                            //           config.Website, item.FullName, customer.Email, linkConfirmUrl, linkConfirm);
                            //        //Tạo thông báo và gửi mail
                            //        Notification notification = new Notification();
                            //        if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                            //            notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                            //        if (item.IsNotificationWeb == true)
                            //        {
                            //            notification.NotificationId = Guid.NewGuid();
                            //            notification.Title = subject;
                            //            notification.Contents = sBody;
                            //            notification.UserPushId = customer.CustomerId;
                            //            notification.UserReadId = item.CustomerId;
                            //            notification.UrlLink = linkConfirm;
                            //            notification.TargetId = customer.CustomerId + "";
                            //            notification.TargetType = (int)Const.NotificationTargetType.CUSTOMER;
                            //            notification.CreatedAt = DateTime.Now;
                            //            notification.UpdatedAt = DateTime.Now;
                            //            notification.Status = (int)Const.Status.TEMP;
                            //            await db.Notification.AddAsync(notification);
                            //        }

                            //    }
                            //}
                            //else
                            //{
                            //    var listUser = await (from cm in db.CustomerMapping
                            //                          join c in db.Customer on cm.CustomerId equals c.CustomerId
                            //                          where cm.TargetId == customer.UnitId
                            //    && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                            //    && cm.Status != (int)Const.Status.DELETED
                            //    && c.Status == (int)Const.Status.NORMAL
                            //                          select c).ToListAsync();
                            //    foreach (var item in listUser)
                            //    {
                            //        String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-register.html");
                            //        sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                            //            config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                            //            config.Website, item.FullName, customer.Email, linkConfirmUrl, linkConfirm);
                            //        //Tạo thông báo và gửi mail
                            //        Notification notification = new Notification();
                            //        if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                            //            notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                            //        if (item.IsNotificationWeb == true)
                            //        {
                            //            notification.NotificationId = Guid.NewGuid();
                            //            notification.Title = subject;
                            //            notification.Contents = sBody;
                            //            notification.UserPushId = customer.CustomerId;
                            //            notification.UserReadId = item.CustomerId;
                            //            notification.UrlLink = linkConfirm;
                            //            notification.TargetId = customer.CustomerId + "";
                            //            notification.TargetType = (int)Const.NotificationTargetType.CUSTOMER;
                            //            notification.CreatedAt = DateTime.Now;
                            //            notification.UpdatedAt = DateTime.Now;
                            //            notification.Status = (int)Const.Status.TEMP;
                            //            await db.Notification.AddAsync(notification);
                            //        }
                            //    }
                            //}

                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Đăng ký tài khoản thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        [HttpPost("registerV1")]
        public async Task<IActionResult> RegisterV1(CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (data.FullName == null || data.FullName == "")
                {
                    def.meta = new Meta(211, "Tên không được để trống!");
                    return Ok(def);
                }
                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExistEmail = db.Customer.Where(e => e.Email == data.Email.ToLower().Trim() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }

                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                        return Ok(def);
                    }

                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                    {
                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Customer customer = new Customer();
                        customer.Username = data.Email.ToLower().Trim();
                        customer.Password = Utils.GetMD5Hash(Utils.RandomString(8));
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.KeyRandom = Utils.RandomString(8);
                        customer.IsEmailConfirm = false;
                        customer.IsSentEmailConfirm = false;
                        customer.IsPhoneConfirm = true;
                        customer.IsViewInfo = false;
                        customer.UnitId = data.UnitId;
                        customer.Type = data.IsUnit ? (int)Const.TypeCustomer.CUSTOMER_UNIT : (int)Const.TypeCustomer.CUSTOMER_PERSONAL;
                        customer.WebsiteId = Const.WEBSITEID;
                        customer.CompanyId = Const.COMPANYID;
                        customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_USER;
                        customer.LastLoginAt = DateTime.Now;
                        customer.CreatedAt = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = (int)Const.Status.TEMP;
                        await db.Customer.AddAsync(customer);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;


                            //Gửi Email xác nhận
                            if (customer.Email != null && customer.Email != "")
                            {
                                String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-confirm-resgister-customer.html");

                                string key = Utils.GetMD5Hash(customer.KeyRandom + customer.Password);
                                string linkConfirm = "'" + config.Website + "thiet-lap-mat-khau-" + key + "-" + customer.CustomerId + "'";
                                //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                                data.KeyRandom = "/thiet-lap-mat-khau-" + key + "-" + customer.CustomerId;
                                sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                    config.Website, customer.FullName, linkConfirm, customer.KeyRandom);
                                string subject = config.EmailSender + " - Xác thực tài khoản";
                                //EmailService emailService = new EmailService();
                                customer.IsSentEmailConfirm = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject, sBody);
                                //if (customer.IsSentEmailConfirm == false)
                                //{
                                //    def.meta = new Meta(404, "Không gửi được thông tin xác nhận đến Email " + customer.Email +", bạn vui lòng kiểm tra lại thông tin Email!");
                                //    return Ok(def);
                                //}
                            }
                            db.Entry(customer).State = EntityState.Modified;

                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Đăng ký tài khoản thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        //[HttpPost("login")]
        //public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        if (loginModel != null)
        //        {
        //            string username = loginModel.email;

        //            using (var db = new IOITDataContext())
        //            {
        //                var customer = db.Customer.Where(e => e.Email == username && e.Status != (int)Const.Status.DELETED).ToList();
        //                if (customer.Count > 0)
        //                {
        //                    string password = loginModel.password.Trim() + customer.FirstOrDefault().KeyRandom;
        //                    password = Utils.GetMD5Hash(password);
        //                    CustomerLogin userLogin = db.Customer.Where(e => e.Email == username && e.Password == password && e.Status != (int)Const.Status.DELETED).Select(e => new CustomerLogin()
        //                    {
        //                        CustomerId = e.CustomerId,
        //                        Username = e.Username,
        //                        Email = e.Email,
        //                        FullName = e.FullName,
        //                        Avata = e.Avata,
        //                        Address = e.Address,
        //                        Password = e.Password,
        //                        PhomeNumber = e.Phone,
        //                        Status = e.Status,
        //                        Type = e.Type,
        //                        RoleId = e.RoleId,
        //                        Sex = e.Sex,
        //                        IsEmailConfirm = e.IsEmailConfirm,
        //                        CreatedAt = e.CreatedAt
        //                    }).FirstOrDefault();

        //                    if (userLogin != null)
        //                    {
        //                        //check if user lock
        //                        if (userLogin.Status == (int)Const.Status.LOCK)
        //                        {
        //                            def.meta = new Meta(223, "Tài khoản đang bị khóa!");
        //                            return Ok(def);
        //                        }

        //                        //check if user not confirm yet
        //                        if (userLogin.IsEmailConfirm == false || userLogin.Status == (int)Const.Status.TEMP)
        //                        {
        //                            def.meta = new Meta(223, "Tài khoản chưa được kích hoạt!");
        //                            return Ok(def);
        //                        }

        //                        var userId = userLogin.CustomerId;

        //                        //check role tạo accetkey ở đây
        //                        List<MenuDTO> listFunctionRole = new List<MenuDTO>();
        //                        var listFRR = db.FunctionRole.Where(e => e.TargetId == userLogin.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
        //                                    && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Function.Location).ToList();
        //                        foreach (var itemFR in listFRR)
        //                        {
        //                            //check exits
        //                            var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
        //                            if (fr.Count > 0)
        //                            {
        //                                string key1 = fr.FirstOrDefault().ActiveKey;
        //                                if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
        //                                {
        //                                    key1 = plusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
        //                                }
        //                                fr.FirstOrDefault().ActiveKey = key1;
        //                            }
        //                            else
        //                            {
        //                                Function function = db.Function.Where(e => e.FunctionId == itemFR.FunctionId).FirstOrDefault();
        //                                if (function != null)
        //                                {
        //                                    MenuDTO menu = new MenuDTO();
        //                                    menu.MenuId = itemFR.FunctionId;
        //                                    menu.Code = function.Code;
        //                                    menu.Name = function.Name;
        //                                    menu.Url = function.Url;
        //                                    menu.Icon = function.Icon;
        //                                    menu.MenuParent = (int)function.FunctionParentId;
        //                                    menu.ActiveKey = itemFR.ActiveKey;
        //                                    listFunctionRole.Add(menu);
        //                                }
        //                            }
        //                        }

        //                        string access_key = "";
        //                        int count = listFunctionRole.Count;
        //                        if (count > 0)
        //                        {
        //                            for (int i = 0; i < count - 1; i++)
        //                            {
        //                                if (listFunctionRole[i].ActiveKey != "000000000")
        //                                {
        //                                    access_key += listFunctionRole[i].Code + ":" + listFunctionRole[i].ActiveKey + "-";
        //                                }
        //                            }

        //                            access_key = access_key + listFunctionRole[count - 1].Code + ":" + listFunctionRole[count - 1].ActiveKey;
        //                        }

        //                        userLogin.access_key = access_key;
        //                        userLogin.listMenus = CreateMenu(listFunctionRole, 0);
        //                        //Nếu là tk tổ chức check xem ng dùng trên ql những đơn vị nào
        //                        var listU = await db.CustomerMapping.Where(e => e.CustomerId == userLogin.CustomerId
        //                        && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
        //                        && e.Status != (int)Const.Status.DELETED).ToListAsync();
        //                        List<int> listUnits = new List<int>();
        //                        foreach (var unit in listU)
        //                        {
        //                            listUnits.Add((int)unit.TargetId);
        //                            await GetListUnit(listUnits, (int)unit.TargetId, db);
        //                        }
        //                        userLogin.listUnits = listUnits;
        //                        string listUs = "";
        //                        foreach(var item in listUnits)
        //                        {
        //                            listUs += item + "-";
        //                        }
        //                        if (listUs != "")
        //                            listUs = listUs.Substring(0, listUs.Length - 1);
        //                        var claims = new List<Claim>
        //                        {
        //                            new Claim(JwtRegisteredClaimNames.Email, userLogin.Email),
        //                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                            new Claim(ClaimTypes.NameIdentifier, userLogin.CustomerId.ToString()),
        //                            new Claim(ClaimTypes.Name, userLogin.FullName),
        //                                new Claim("CustomerId", userLogin.CustomerId != null ? userLogin.CustomerId.ToString() : ""),
        //                                new Claim("ListUnits", listUs),
        //                                //new Claim("RoleMax", userLogin.roleMax != null ? userLogin.roleMax.ToString() : ""),
        //                                //new Claim("RoleLevel", userLogin.roleLevel != null ? userLogin.roleLevel.ToString() : ""),
        //                                new Claim("AccessKey", access_key != null ? access_key : ""),
        //                                //new Claim("LanguageId", access_key != null ? languageId.ToString() : ""),
        //                                new Claim("Type", userLogin.Type != null ? userLogin.Type.ToString() : ""),
        //                        };
        //                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
        //                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //                        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));
        //                        var token = new JwtSecurityToken(
        //                            _configuration["AppSettings:JwtIssuer"],
        //                            _configuration["AppSettings:JwtIssuer"],
        //                            claims,
        //                            expires: expires,
        //                            signingCredentials: creds
        //                        );
        //                        userLogin.access_token = new JwtSecurityTokenHandler().WriteToken(token);
        //                        //HttpContext.Session.SetInt32("CustomerId", userLogin.CustomerId);
        //                        //HttpContext.Session.SetString("access_token", userLogin.access_token);
        //                        //var option = new CookieOptions();
        //                        //option.Expires = DateTime.Now.AddMinutes(600);
        //                        //Response.Cookies.Append("CustomerId", userLogin.CustomerId.ToString(), option);
        //                        InitSession(userLogin);

        //                        def.data = userLogin;
        //                        def.meta = new Meta(200, "Đăng nhập thành công!");
        //                        return Ok(def);
        //                    }
        //                    else
        //                    {
        //                        //check if email exist
        //                        var existed = db.Customer.Where(e => e.Email == username && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
        //                        if (existed != null)
        //                        {
        //                            def.meta = new Meta(213, "Tài khoản hoặc mật khẩu không chính xác!");
        //                            return Ok(def);
        //                        }
        //                        else
        //                        {
        //                            def.meta = new Meta(404, "Tài khoản hoặc mật khẩu không chính xác!");
        //                            return Ok(def);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    //Login qua keycloak
        //                    def.meta = new Meta(404, "Tài khoản hoặc mật khẩu không chính xác!");
        //                    return Ok(def);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            def.meta = new Meta(400, "Tài khoản hoặc mật khẩu không chính xác!");
        //            return Ok(def);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Info("Exception:" + e);
        //        def.meta = new Meta(500, "Server xảy ra lỗi. Xin vui lòng thử lại sau!");
        //        return Ok(def);
        //    }
        //}

        //plus role
        private string plusActiveKey(string key1, string key2)
        {
            string str = "";
            char[] str1 = key1.ToCharArray();
            char[] str2 = key2.ToCharArray();
            for (int i = 0; i < str1.Length; i++)
            {
                int k = int.Parse(str1[i].ToString()) + int.Parse(str2[i].ToString());
                if (k > 1) k = 1;
                str += k;
            }
            return str;
        }

        //create menu
        private List<MenuDTO> CreateMenu(List<MenuDTO> list, int k)
        {
            var listMenu = list.Where(e => e.MenuParent == k).ToList();
            if (listMenu.Count > 0)
            {
                List<MenuDTO> menus = new List<MenuDTO>();
                foreach (var item in listMenu)
                {
                    char[] str = item.ActiveKey.ToCharArray();
                    if (int.Parse(str[8].ToString()) == 1)
                    {
                        MenuDTO menu = new MenuDTO();
                        menu.MenuId = item.MenuId;
                        menu.Code = item.Code;
                        menu.Name = item.Name;
                        menu.Url = item.Url;
                        menu.Icon = item.Icon;
                        menu.MenuParent = item.MenuParent;
                        menu.ActiveKey = item.ActiveKey;
                        menu.listMenus = CreateMenu(list, item.MenuId);
                        menus.Add(menu);
                    }
                }
                return menus;
            }
            return new List<MenuDTO>();
        }

        private async Task GetListUnit(List<int> output, List<Unit> input, int unitId, IOITDataContext db)
        {
            var listU = input.Where(x => x.UnitParentId == unitId).ToList();
            if (listU.Count > 0)
            {
                foreach (var item in listU)
                {
                    output.Add(item.UnitId);
                    if (item.UnitId != item.UnitParentId)
                    {
                        await GetListUnit(output, input, item.UnitId, db);
                    }
                }
            }
        }

        private void InitSession(CustomerLogin userLogin)
        {
            HttpContext.Session.SetInt32("CustomerId", userLogin.CustomerId);
            if (userLogin.Email != null)
            {
                HttpContext.Session.SetString("CustomerEmail", userLogin.Email);
            }
            if (userLogin.FullName != null)
            {
                HttpContext.Session.SetString("CustomerFullName", userLogin.FullName);
            }
            if (userLogin.Avata != null)
            {
                HttpContext.Session.SetString("CustomerAvata", userLogin.Avata);
            }
            if (userLogin.Address != null)
            {
                HttpContext.Session.SetString("CustomerAddress", userLogin.Address);
            }
            if (userLogin.Password != null)
            {
                HttpContext.Session.SetString("CustomerPassword", userLogin.Password);
            }
            if (userLogin.PhomeNumber != null)
            {
                HttpContext.Session.SetString("CustomerPhoneNumber", userLogin.PhomeNumber);
            }
            if (userLogin.access_token != null)
            {
                HttpContext.Session.SetString("access_token", userLogin.access_token);
            }
            if (userLogin.Sex != null)
            {
                HttpContext.Session.SetString("CustomerSex", userLogin.Sex + "");
            }
            if (userLogin.Status != null)
            {
                HttpContext.Session.SetString("Status", userLogin.Status + "");
            }
            if (userLogin.CreatedAt != null)
            {
                HttpContext.Session.SetString("CreatedAt", userLogin.CreatedAt + "");
            }
            if (userLogin.access_key != null)
            {
                HttpContext.Session.SetString("access_key", userLogin.access_key + "");
            }
            if (userLogin.listMenus != null)
            {
                HttpContext.Session.SetObject("listMenus", userLogin.listMenus);
            }
            if (userLogin.listUnits != null)
            {
                HttpContext.Session.SetObject("listUnits", userLogin.listUnits);
            }
            if (userLogin.TypeThirdId != null)
            {
                HttpContext.Session.SetInt32("TypeThirdId", (int)userLogin.TypeThirdId);
            }
            if (userLogin.NunberNotification != null)
            {
                HttpContext.Session.SetInt32("NunberNotification", (int)userLogin.NunberNotification);
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            DefaultResponse def = new DefaultResponse();
            def.meta = new Meta(200, "Success");
            HttpContext.Session.Clear();
            return Ok(def);
        }

        [HttpPost("settingPass/{id}")]
        public async Task<IActionResult> SettingPass(CustomerDTO data, long id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                //if (data.Email == null || data.Email == "")
                //{
                //    def.meta = new Meta(211, "Email đăng ký không được để trống!");
                //    return Ok(def);
                //}
                if (data.KeyRandom == null || data.KeyRandom == "")
                {
                    def.meta = new Meta(211, "Mã xác thực không được để trống!");
                    return Ok(def);
                }
                //if (data.Password == null || data.Password == "")
                //{
                //    def.meta = new Meta(211, "Mật khẩu không được để trống!");
                //    return Ok(def);
                //}

                using (var db = new IOITDataContext())
                {
                    var checkExist = await db.Customer.Where(e => e.CustomerId == id && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                    if (checkExist == null)
                    {
                        def.meta = new Meta(212, "Tài khoản không tồn tại!");
                        return Ok(def);
                    }
                    if (checkExist.Status == (int)Const.Status.NORMAL)
                    {
                        def.meta = new Meta(212, "Tài khoản đã được xác thực!");
                        return Ok(def);
                    }
                    if (checkExist.KeyRandom != data.KeyRandom)
                    {
                        def.meta = new Meta(212, "Mã xác thực không đúng!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        string keycloakUrl = _configuration["KeycloakSettings:endpoint"];
                        string keycloakClientId = _configuration["KeycloakSettings:client_id"];
                        string keycloakClientSecret = _configuration["KeycloakSettings:client_secret"];
                        string keycloakClientKey = _configuration["KeycloakSettings:client_key"];
                        string keycloakClientValue = _configuration["KeycloakSettings:client_value"];
                        string keycloakRealm = _configuration["KeycloakSettings:realm"];

                        data.FullName = checkExist.FullName;
                        data.Email = checkExist.Email;
                        data.Username = checkExist.Username;
                        data.Password = Security.Decrypt(checkExist.KeyRandom, checkExist.KeyToken);

                        bool checkCreate = await CreateUserAsync(keycloakUrl, keycloakClientId, keycloakClientSecret,
                            keycloakClientKey, keycloakClientValue, keycloakRealm, data);

                        string roleCode = "NDTC";
                        if (checkExist.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                            roleCode = "USER_FULL";
                        var role = await db.Role.Where(e => e.Code.Trim() == roleCode).FirstOrDefaultAsync();
                        if (role != null)
                        {
                            CustomerMapping mapping = new CustomerMapping();
                            mapping.CustomerId = checkExist.CustomerId;
                            mapping.TargetId = role.RoleId;
                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                            mapping.CreatedId = 1;
                            mapping.UpdatedId = 1;
                            mapping.CreatedAt = DateTime.Now;
                            mapping.UpdatedAt = DateTime.Now;
                            mapping.Status = (int)Const.Status.NORMAL;
                            await db.CustomerMapping.AddAsync(mapping);
                            checkExist.RoleId = role.RoleId;
                        }
                        //string passWordHash = data.Password + checkExist.KeyRandom;
                        //checkExist.Password = Utils.GetMD5Hash(passWordHash);
                        checkExist.IsEmailConfirm = true;
                        checkExist.IsNotificationMail = true;
                        checkExist.IsNotificationWeb = true;
                        checkExist.TypeThirdId = checkCreate ? (int)Const.TypeThird.CUSTOMER_KEYLOCK : (int)Const.TypeThird.CUSTOMER_USER;
                        checkExist.Status = (int)Const.Status.NORMAL;
                        checkExist.UpdatedAt = DateTime.Now;
                        db.Customer.Update(checkExist);

                        try
                        {
                            await db.SaveChangesAsync();

                            try
                            {


                                if (checkExist.Status == (int)Const.Status.TEMP)
                                {
                                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                                    if (config == null)
                                    {
                                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                                        return Ok(def);
                                    }

                                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                                    {
                                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                                        return Ok(def);
                                    }
                                    //Gửi Email vả thông báo
                                    string subject = config.EmailSender + " -  Duyệt tài khoản đăng ký mới";
                                    //Tạo thông báo
                                    string linkConfirm = "nguoi-dung-to-chuc";
                                    //Nếu tài khoản cá nhân link vào cms, nếu tài khoản tổ chức link vào tổ chức
                                    if (checkExist.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                    {
                                        linkConfirm = "cms/system/user-data";
                                    }
                                    //Lấy ds người nhận, nếu là cá nhân thì các tk có quyền ql ng dùng,
                                    //nếu tổ chức là các tk quản lý tổ chức đó
                                    linkConfirm = config.Website + linkConfirm;
                                    string linkConfirmUrl = "'" + linkConfirm + "'";
                                    if (checkExist.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                    {
                                        var listUser = await (from u in db.User
                                                              join c in db.Customer on u.UserMapId equals c.CustomerId
                                                              join ur in db.UserRole on u.UserId equals ur.UserId
                                                              join fr in db.FunctionRole on ur.RoleId equals fr.TargetId
                                                              join f in db.Function on fr.FunctionId equals f.FunctionId
                                                              where u.Status == (int)Const.Status.NORMAL
                                                              && c.Status == (int)Const.Status.NORMAL
                                                              && ur.Status != (int)Const.Status.DELETED
                                                              && fr.Status != (int)Const.Status.DELETED
                                                              && f.Status != (int)Const.Status.DELETED
                                                              && fr.ActiveKey.Substring(0, 1) == "1" && fr.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                                              && f.Code == "QLND"
                                                              select c).GroupBy(e => new
                                                              {
                                                                  e.CustomerId,
                                                                  e.Email,
                                                                  e.FullName,
                                                                  e.IsNotificationMail,
                                                                  e.IsNotificationWeb
                                                              }).Select(e => new
                                                              {
                                                                  CustomerId = e.Key.CustomerId,
                                                                  Email = e.Key.Email,
                                                                  FullName = e.Key.FullName,
                                                                  IsNotificationMail = e.Key.IsNotificationMail,
                                                                  IsNotificationWeb = e.Key.IsNotificationWeb,
                                                              }).ToListAsync();
                                        foreach (var item in listUser)
                                        {
                                            String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-register.html");
                                            sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                               config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                               config.Website, item.FullName, checkExist.Email, linkConfirmUrl, linkConfirm);
                                            //Tạo thông báo và gửi mail
                                            Notification notification = new Notification();
                                            if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                                                notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                                            if (item.IsNotificationWeb == true)
                                            {
                                                notification.NotificationId = Guid.NewGuid();
                                                notification.Title = subject;
                                                notification.Contents = sBody;
                                                notification.UserPushId = checkExist.CustomerId;
                                                notification.UserReadId = item.CustomerId;
                                                notification.UrlLink = linkConfirm;
                                                notification.TargetId = checkExist.CustomerId + "";
                                                notification.TargetType = (int)Const.NotificationTargetType.CUSTOMER;
                                                notification.CreatedAt = DateTime.Now;
                                                notification.UpdatedAt = DateTime.Now;
                                                notification.Status = (int)Const.Status.TEMP;
                                                await db.Notification.AddAsync(notification);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        var listUser = await (from cm in db.CustomerMapping
                                                              join c in db.Customer on cm.CustomerId equals c.CustomerId
                                                              where cm.TargetId == checkExist.UnitId
                                        && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                        && cm.Status != (int)Const.Status.DELETED
                                        && c.Status == (int)Const.Status.NORMAL
                                                              select c).ToListAsync();
                                        foreach (var item in listUser)
                                        {
                                            String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-register.html");
                                            sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                                config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                                config.Website, item.FullName, checkExist.Email, linkConfirmUrl, linkConfirm);
                                            //Tạo thông báo và gửi mail
                                            Notification notification = new Notification();
                                            if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                                                notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                                            if (item.IsNotificationWeb == true)
                                            {
                                                notification.NotificationId = Guid.NewGuid();
                                                notification.Title = subject;
                                                notification.Contents = sBody;
                                                notification.UserPushId = checkExist.CustomerId;
                                                notification.UserReadId = item.CustomerId;
                                                notification.UrlLink = linkConfirm;
                                                notification.TargetId = checkExist.CustomerId + "";
                                                notification.TargetType = (int)Const.NotificationTargetType.CUSTOMER;
                                                notification.CreatedAt = DateTime.Now;
                                                notification.UpdatedAt = DateTime.Now;
                                                notification.Status = (int)Const.Status.TEMP;
                                                await db.Notification.AddAsync(notification);
                                            }
                                        }
                                    }

                                    await db.SaveChangesAsync();
                                }
                            }
                            catch { }

                            if (checkExist.CustomerId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thiết lập mật khẩu thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        [HttpPost("resetPass/{id}")]
        public async Task<IActionResult> ResetPass(CustomerDTO data, long id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                //if (data.Email == null || data.Email == "")
                //{
                //    def.meta = new Meta(211, "Email đăng ký không được để trống!");
                //    return Ok(def);
                //}
                //if (data.KeyRandom == null || data.KeyRandom == "")
                //{
                //    def.meta = new Meta(211, "Mã xác thực không được để trống!");
                //    return Ok(def);
                //}
                if (data.Password == null || data.Password == "")
                {
                    def.meta = new Meta(211, "Mật khẩu không được để trống!");
                    return Ok(def);
                }
                if (data.ConfirmPassword == null || data.ConfirmPassword == "")
                {
                    def.meta = new Meta(211, "Mật khẩu xác nhận không được để trống!");
                    return Ok(def);
                }
                if (data.ConfirmPassword != data.Password)
                {
                    def.meta = new Meta(211, "Mật khẩu xác nhận không đúng!");
                    return Ok(def);
                }
                if (!Security.IsPasswordValid(data.Password))
                {
                    def.meta = new Meta(211, "Mật khẩu phải đảm bảo 8 ký tự bao gồm chữ hoa, chữ thường và ít nhất 1 ký tự đặc biệt!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExist = await db.Customer.Where(e => e.CustomerId == id && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                    if (checkExist == null)
                    {
                        def.meta = new Meta(212, "Tài khoản không tồn tại!");
                        return Ok(def);
                    }
                    if (checkExist.KeyRandom != data.KeyRandom)
                    {
                        def.meta = new Meta(212, "Mã xác thực không đúng!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        string passWordHash = data.Password + checkExist.KeyRandom;
                        checkExist.Password = Utils.GetMD5Hash(passWordHash);
                        //checkExist.IsEmailConfirm = true;
                        //checkExist.IsNotificationMail = true;
                        //checkExist.IsNotificationWeb = true;
                        checkExist.UpdatedAt = DateTime.Now;
                        db.Customer.Update(checkExist);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (checkExist.CustomerId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thiết lập mật khẩu thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        [Authorize]
        [HttpPost("UpdateInfoCustomer/{CustomerId}")]
        public async Task<IActionResult> UpdateInfoCustomer(int CustomerId, CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            string CusId = identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault();
            if (CustomerId != int.Parse(CusId))
            {
                def.meta = new Meta(222, "Lỗi tài khoản đăng nhập!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }

                if (data.FullName == null || data.FullName == "")
                {
                    def.meta = new Meta(211, "Tên không được để trống!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Customer customer = db.Customer.Where(e => e.CustomerId == CustomerId && e.Email == data.Email && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy thông tin đăng nhập của bạn!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        customer.FullName = data.FullName;
                        customer.Email = data.Email;
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.IsViewInfo = data.IsViewInfo != null ? data.IsViewInfo : false;
                        customer.UpdatedAt = DateTime.Now;
                        db.Update(customer);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                transaction.Commit();

                                if (customer.Email != null)
                                {
                                    HttpContext.Session.SetString("CustomerEmail", customer.Email);
                                }

                                if (customer.FullName != null)
                                {
                                    HttpContext.Session.SetString("CustomerFullName", customer.FullName);
                                }

                                if (customer.Avata != null)
                                {
                                    HttpContext.Session.SetString("CustomerAvata", customer.Avata);
                                }

                                if (customer.Address != null)
                                {
                                    HttpContext.Session.SetString("CustomerAddress", customer.Address);
                                }

                                if (customer.Password != null)
                                {
                                    HttpContext.Session.SetString("CustomerPassword", customer.Password);
                                }

                                if (customer.Phone != null)
                                {
                                    HttpContext.Session.SetString("CustomerPhoneNumber", customer.Phone);
                                }

                                if (customer.Sex != null)
                                {
                                    HttpContext.Session.SetString("CustomerSex", customer.Sex + "");
                                }
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Cập nhật tài khoản thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPost("FollowProduct/{CustomerId}/{ProductId}")]
        public async Task<IActionResult> FollowProduct(int CustomerId, int ProductId)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            string CusId = identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault();
            if (CustomerId != int.Parse(CusId))
            {
                def.meta = new Meta(222, "Lỗi tài khoản đăng nhập!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    ProductCustomer productCustomer = db.ProductCustomer.Where(pc => pc.TargetId == ProductId && pc.CustomerId == CustomerId && pc.TargetType == (int)Const.TypeProductCustomer.FOLLOW && pc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (productCustomer != null)
                    {
                        def.meta = new Meta(200, "Theo dõi thành công!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        ProductCustomer newpc = new ProductCustomer();
                        newpc.TargetId = ProductId;
                        newpc.TargetType = (int)Const.TypeProductCustomer.FOLLOW;
                        newpc.CustomerId = CustomerId;
                        newpc.Location = 1;
                        newpc.CreatedAt = DateTime.Now;
                        newpc.Status = (int)Const.Status.NORMAL;
                        db.ProductCustomer.Add(newpc);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (newpc.ProductCustomerId > 0)
                            {
                                transaction.Commit();
                                def.meta = new Meta(200, "Theo dõi thành công!");
                                def.data = true;
                            }
                            else
                            {
                                transaction.Rollback();
                                def.meta = new Meta(212, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                                def.data = true;
                            }


                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Hệ thống xả ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPost("LoveProduct/{CustomerId}/{ProductId}")]
        public async Task<IActionResult> LoveProduct(int CustomerId, int ProductId)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            string CusId = identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault();
            if (CustomerId != int.Parse(CusId))
            {
                def.meta = new Meta(222, "Lỗi tài khoản đăng nhập!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    ProductCustomer productCustomer = db.ProductCustomer.Where(pc => pc.TargetId == ProductId && pc.CustomerId == CustomerId && pc.TargetType == (int)Const.TypeProductCustomer.LOVE && pc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (productCustomer != null)
                    {
                        def.meta = new Meta(200, "Thêm sản phẩm vào danh sách yêu thích thành công!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        ProductCustomer newpc = new ProductCustomer();
                        newpc.TargetId = ProductId;
                        newpc.TargetType = (int)Const.TypeProductCustomer.LOVE;
                        newpc.CustomerId = CustomerId;
                        newpc.Location = 1;
                        newpc.CreatedAt = DateTime.Now;
                        newpc.Status = (int)Const.Status.NORMAL;
                        db.ProductCustomer.Add(newpc);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (newpc.ProductCustomerId > 0)
                            {
                                transaction.Commit();
                                def.meta = new Meta(200, "Thêm sản phẩm vào danh sách yêu thích thành công!");
                                def.data = true;
                            }
                            else
                            {
                                transaction.Rollback();
                                def.meta = new Meta(212, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                                def.data = true;
                            }


                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [HttpPost("RecoverPasssword")]
        public async Task<IActionResult> RecoverPasssword(LoginModel data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }


                if (data.email == null || data.email == "")
                {
                    def.meta = new Meta(211, "Bạn chưa nhập Email!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExistEmail = db.Customer.Where(e => e.Email == data.email && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistEmail == null)
                    {
                        def.meta = new Meta(212, "Không tìm thấy Email trong hệ thống!");
                        return Ok(def);
                    }

                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                        return Ok(def);
                    }

                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                    {
                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //var Password = Utils.RandomString(8);
                        //var Md5Password = Utils.GetMD5Hash(Password);
                        checkExistEmail.KeyRandom = Utils.RandomString(8);
                        //checkExistEmail.Password = Utils.GetMD5Hash(Md5Password + checkExistEmail.KeyRandom);
                        checkExistEmail.UpdatedAt = DateTime.Now;
                        db.Entry(checkExistEmail).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/recover-password.html");

                            string key = Utils.GetMD5Hash(checkExistEmail.KeyRandom + checkExistEmail.Password);
                            string linkConfirm = "'" + config.Website + "thiet-lap-mat-khau-" + key + "-" + checkExistEmail.CustomerId + "'";
                            //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                            data.password = "/thiet-lap-mat-khau-" + key + "-" + checkExistEmail.CustomerId;
                            sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                config.Website, checkExistEmail.FullName, linkConfirm, checkExistEmail.KeyRandom);
                            string subject = config.EmailSender + " - Quên mật khẩu";
                            //EmailService emailService = new EmailService();
                            bool success = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, checkExistEmail.Email, subject, sBody);
                            if (!success)
                            {
                                transaction.Rollback();
                                def.meta = new Meta(500, "Gửi mail không thành công. Xin vui lòng kiểm tra lại thông tin Email của bạn!");
                                return Ok(def);
                            }
                            else
                            {
                                transaction.Commit();
                                def.meta = new Meta(200, "Yêu cầu đổi mật khẩu thành công!");
                                def.data = data;
                                return Ok(def);
                            }
                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(checkExistEmail.CustomerId))
                            {
                                def.meta = new Meta(212, "Đã xảy ra lỗi. Xin vui lòng thử lại!");
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
                def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPost("ChangePasssword")]
        public async Task<IActionResult> ChangePasssword(UserChangePass data)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            string CusId = identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault();
            if (data.UserId != int.Parse(CusId))
            {
                def.meta = new Meta(222, "Lỗi tài khoản đăng nhập!");
                return Ok(def);
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }

                if (data.PasswordOld == null || data.PasswordOld == "")
                {
                    def.meta = new Meta(211, "Bạn chưa nhập Mật khẩu!");
                    return Ok(def);
                }
                if (data.PasswordNew == null || data.PasswordNew == "")
                {
                    def.meta = new Meta(211, "Bạn chưa nhập Mật khẩu mới!");
                    return Ok(def);
                }
                if (!Security.IsPasswordValid(data.PasswordNew))
                {
                    def.meta = new Meta(211, "Mật khẩu phải đảm bảo 8 ký tự bao gồm chữ hoa, chữ thường và ít nhất 1 ký tự đặc biệt!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExistEmail = db.Customer.Where(e => e.CustomerId == data.UserId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistEmail == null)
                    {
                        def.meta = new Meta(212, "Không tìm thấy Tài khoản trong hệ thống!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var CheckPass = Utils.GetMD5Hash(data.PasswordOld + checkExistEmail.KeyRandom);
                        if (CheckPass != checkExistEmail.Password)
                        {
                            def.meta = new Meta(212, "Mật khẩu cũ bạn nhập không đúng. Xin vui lòng thử lại!");
                            return Ok(def);
                        }

                        checkExistEmail.Password = Utils.GetMD5Hash(data.PasswordNew + checkExistEmail.KeyRandom);
                        checkExistEmail.UpdatedAt = DateTime.Now;
                        db.Entry(checkExistEmail).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (checkExistEmail.CustomerId > 0)
                            {
                                transaction.Commit();
                                def.meta = new Meta(200, "Đổi mật khẩu thành công!");
                                def.data = data;
                                return Ok(def);
                            }
                            else
                            {
                                transaction.Rollback();
                                def.meta = new Meta(500, "Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }



                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(checkExistEmail.CustomerId))
                            {
                                def.meta = new Meta(212, "Đã xảy ra lỗi. Xin vui lòng thử lại!");
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
                def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPut("UpdateAvata/{type}")]
        public async Task<IActionResult> UpdateAvata(UpdateAvata data, int type)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            string CusId = identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault();
            if (data.CustomerId != int.Parse(CusId))
            {
                def.meta = new Meta(222, "Lỗi tài khoản đăng nhập!");
                return Ok(def);
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }

                if (type == 1 && (data.Avata == null || data.Avata == ""))
                {
                    def.meta = new Meta(211, "Bạn chưa tải lên ảnh đại diện!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExistEmail = db.Customer.Where(e => e.CustomerId == data.CustomerId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistEmail == null)
                    {
                        def.meta = new Meta(212, "Không tìm thấy Tài khoản trong hệ thống!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        checkExistEmail.Avata = data.Avata;
                        checkExistEmail.UpdatedAt = DateTime.Now;
                        db.Entry(checkExistEmail).State = EntityState.Modified;
                        data.Avata = data.Avata == null ? "" : data.Avata;
                        HttpContext.Session.SetString("CustomerAvata", data.Avata);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (checkExistEmail.CustomerId > 0)
                            {
                                transaction.Commit();
                                def.meta = new Meta(200, "Cập nhật ảnh đại diện thành công!");
                                def.data = data;
                                return Ok(def);
                            }
                            else
                            {
                                transaction.Rollback();
                                def.meta = new Meta(500, "Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }



                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (CustomerExists(checkExistEmail.CustomerId))
                            {
                                def.meta = new Meta(212, "Đã xảy ra lỗi. Xin vui lòng thử lại!");
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
                def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        //Phần quản trị
        [Authorize]
        [HttpPost("GetByPagePost")]
        public async Task<IActionResult> GetByPagePost([FromBody] FilterReport paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            //int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            string listUnits = identity.Claims.Where(c => c.Type == "ListUnits").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "Bạn không có quyền xem danh sách người dùng!");
                return Ok(def);
            }
            if (paging != null)
            {
                try
                {
                    using (var db = new IOITDataContext())
                    {

                        def.meta = new Meta(200, "Success");
                        var dateStart = new DateTime(2000, 1, 1);
                        var dateEnd = DateTime.Now;
                        if (paging.DateStart != null)
                            dateStart = new DateTime(paging.DateStart.Value.Year, paging.DateStart.Value.Month, paging.DateStart.Value.Day, 0, 0, 0);
                        if (paging.DateEnd != null)
                            dateEnd = new DateTime(paging.DateEnd.Value.Year, paging.DateEnd.Value.Month, paging.DateEnd.Value.Day, 23, 59, 59);
                        string[] listU = listUnits.Split('-');

                        IQueryable<CustomerDTO> data = db.Customer.Where(c =>
                        c.Status != (int)Const.Status.DELETED
                        && c.CreatedAt >= dateStart && c.CreatedAt <= dateEnd
                        && listU.Contains(c.UnitId.ToString())).Select(e => new CustomerDTO
                        {
                            CustomerId = e.CustomerId,
                            Username = e.Username,
                            Password = e.Password,
                            FullName = e.FullName,
                            Email = e.Email,
                            Phone = e.Phone,
                            Avata = e.Avata,
                            Sex = e.Sex,
                            Birthday = e.Birthday,
                            Address = e.Address,
                            Note = e.Note,
                            KeyRandom = e.KeyRandom,
                            IsEmailConfirm = e.IsEmailConfirm,
                            IsSentEmailConfirm = e.IsSentEmailConfirm,
                            IsPhoneConfirm = e.IsPhoneConfirm,
                            Type = e.Type,
                            UnitId = e.UnitId,
                            CountryId = e.CountryId,
                            TypeId = e.TypeId,
                            IdNumber = e.IdNumber,
                            DateNumber = e.DateNumber,
                            AddressNumber = e.AddressNumber,
                            PositionId = e.PositionId,
                            AcademicRankId = e.AcademicRankId,
                            DegreeId = e.DegreeId,
                            RoleId = e.RoleId,
                            WebsiteId = e.WebsiteId,
                            CompanyId = e.CompanyId,
                            TypeThirdId = e.TypeThirdId,
                            LastLoginAt = e.LastLoginAt,
                            IsNotificationMail = e.IsNotificationMail,
                            IsNotificationWeb = e.IsNotificationWeb,
                            IsViewInfo = e.IsViewInfo,
                            UserId = e.UserId,
                            CreatedAt = e.CreatedAt,
                            UpdatedAt = e.UpdatedAt,
                            Status = e.Status
                        });

                        if (paging.query != null)
                        {
                            paging.query = HttpUtility.UrlDecode(paging.query);
                        }

                        data = data.Where(paging.query);

                        //MetaDataDT metaDataDT = new MetaDataDT();
                        //metaDataDT.Sum = data.Count();
                        //metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                        //metaDataDT.Temp = data.Where(e => e.Status == 10).Count();
                        //metaDataDT.Lock = data.Where(e => e.Status == 98).Count();

                        def.metadata = data.Count();

                        if (paging.page_size > 0)
                        {
                            if (paging.order_by != null && paging.order_by != "")
                            {
                                data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                            }
                            else
                            {
                                data = data.OrderBy("CustomerId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                            }
                        }
                        else
                        {
                            if (paging.order_by != null && paging.order_by != "")
                            {
                                data = data.OrderBy(paging.order_by);
                            }
                            else
                            {
                                data = data.OrderBy("CustomerId desc");
                            }
                        }

                        if (paging.select != null && paging.select != "")
                        {
                            paging.select = "new(" + paging.select + ")";
                            paging.select = HttpUtility.UrlDecode(paging.select);
                            def.data = await data.Select(paging.select).ToDynamicListAsync();
                        }
                        else
                        {
                            var listDatas = data.ToList();
                            foreach (var item in listDatas)
                            {
                                var unit = db.Unit.Where(e => e.UnitId == item.UnitId).FirstOrDefault();
                                item.UnitName = unit != null ? unit.Name : "";
                                var listMap = await db.CustomerMapping.Where(e => e.CustomerId == item.CustomerId
                                && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                item.ListRoles = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE)
                                    .Select(e => (int)e.TargetId).ToList();
                                item.ListResearchArea = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION)
                                    .Select(e => (int)e.TargetId).ToList();
                                item.ListUnitManager = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT)
                                    .Select(e => (int)e.TargetId).ToList();
                            }
                            def.data = listDatas;
                        }

                        return Ok(def);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception:" + e);
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Customer/5
        [Authorize]
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "Bạn không có quyền xem chi tiết người dùng!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    CustomerDTO data = await db.Customer.Where(e => e.CustomerId == id).Select(e => new CustomerDTO
                    {
                        CustomerId = e.CustomerId,
                        Username = e.Username,
                        Password = e.Password,
                        FullName = e.FullName,
                        Email = e.Email,
                        Phone = e.Phone,
                        Avata = e.Avata,
                        Sex = e.Sex,
                        Birthday = e.Birthday,
                        Address = e.Address,
                        Note = e.Note,
                        KeyRandom = e.KeyRandom,
                        IsEmailConfirm = e.IsEmailConfirm,
                        IsSentEmailConfirm = e.IsSentEmailConfirm,
                        IsPhoneConfirm = e.IsPhoneConfirm,
                        Type = e.Type,
                        UnitId = e.UnitId,
                        CountryId = e.CountryId,
                        TypeId = e.TypeId,
                        IdNumber = e.IdNumber,
                        DateNumber = e.DateNumber,
                        AddressNumber = e.AddressNumber,
                        PositionId = e.PositionId,
                        AcademicRankId = e.AcademicRankId,
                        DegreeId = e.DegreeId,
                        RoleId = e.RoleId,
                        WebsiteId = e.WebsiteId,
                        CompanyId = e.CompanyId,
                        TypeThirdId = e.TypeThirdId,
                        LastLoginAt = e.LastLoginAt,
                        IsNotificationMail = e.IsNotificationMail,
                        IsNotificationWeb = e.IsNotificationWeb,
                        IsViewInfo = e.IsViewInfo,
                        UserId = e.UserId,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt,
                        Status = e.Status
                    }).FirstOrDefaultAsync();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    data.listRA = await (from cm in db.CustomerMapping
                                         join cat in db.Category on cm.TargetId equals cat.CategoryId
                                         where cm.CustomerId == id
                     && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                     && cm.Status != (int)Const.Status.DELETED
                                         select new CategoryDTL
                                         {
                                             CategoryId = cat.CategoryId,
                                             Name = cat.Name,
                                             Code = cat.Code,
                                             Url = cat.Url,
                                         }).ToListAsync();
                    //foreach(var item in listRA)
                    //{
                    //    data.ListResearchArea.Add(item.Value);
                    //}
                    //data.listMU = await db.CustomerMapping.Where(e => e.CustomerId == id
                    //&& e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                    //&& e.Status != (int)Const.Status.DELETED).Select(e => e.TargetId).ToListAsync();
                    data.listMU = await (from cm in db.CustomerMapping
                                         join u in db.Unit on cm.TargetId equals u.UnitId
                                         where cm.CustomerId == id
                     && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                     && cm.Status != (int)Const.Status.DELETED
                                         select new UnitDT
                                         {
                                             UnitId = u.UnitId,
                                             Name = u.Name,
                                             Code = u.Code,
                                         }).ToListAsync();
                    //foreach (var item in listMU)
                    //{
                    //    data.ListUnitManager.Add(item.Value);
                    //}

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

        [Authorize]
        [HttpGet("getByIdMe/{id}")]
        public async Task<IActionResult> getByIdMe(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            //{
            //    def.meta = new Meta(222, "Bạn không có quyền xem chi tiết người dùng!");
            //    return Ok(def);
            //}
            try
            {
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    CustomerDTO data = await db.Customer.Where(e => e.CustomerId == id).Select(e => new CustomerDTO
                    {
                        CustomerId = e.CustomerId,
                        Username = e.Username,
                        Password = e.Password,
                        FullName = e.FullName,
                        Email = e.Email,
                        Phone = e.Phone,
                        Avata = e.Avata,
                        Sex = e.Sex,
                        Birthday = e.Birthday,
                        Address = e.Address,
                        Note = e.Note,
                        KeyRandom = e.KeyRandom,
                        IsEmailConfirm = e.IsEmailConfirm,
                        IsSentEmailConfirm = e.IsSentEmailConfirm,
                        IsPhoneConfirm = e.IsPhoneConfirm,
                        Type = e.Type,
                        UnitId = e.UnitId,
                        CountryId = e.CountryId,
                        TypeId = e.TypeId,
                        IdNumber = e.IdNumber,
                        DateNumber = e.DateNumber,
                        AddressNumber = e.AddressNumber,
                        PositionId = e.PositionId,
                        AcademicRankId = e.AcademicRankId,
                        DegreeId = e.DegreeId,
                        RoleId = e.RoleId,
                        WebsiteId = e.WebsiteId,
                        CompanyId = e.CompanyId,
                        TypeThirdId = e.TypeThirdId,
                        LastLoginAt = e.LastLoginAt,
                        IsNotificationMail = e.IsNotificationMail,
                        IsNotificationWeb = e.IsNotificationWeb,
                        IsViewInfo = e.IsViewInfo,
                        UserId = e.UserId,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt,
                        Status = e.Status
                    }).FirstOrDefaultAsync();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    data.listRA = await (from cm in db.CustomerMapping
                                         join cat in db.Category on cm.TargetId equals cat.CategoryId
                                         where cm.CustomerId == id
                     && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                     && cm.Status != (int)Const.Status.DELETED
                                         select new CategoryDTL
                                         {
                                             CategoryId = cat.CategoryId,
                                             Name = cat.Name,
                                             Code = cat.Code,
                                             Url = cat.Url,
                                         }).ToListAsync();
                    //foreach(var item in listRA)
                    //{
                    //    data.ListResearchArea.Add(item.Value);
                    //}
                    //data.listMU = await db.CustomerMapping.Where(e => e.CustomerId == id
                    //&& e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                    //&& e.Status != (int)Const.Status.DELETED).Select(e => e.TargetId).ToListAsync();
                    data.listMU = await (from cm in db.CustomerMapping
                                         join u in db.Unit on cm.TargetId equals u.UnitId
                                         where cm.CustomerId == id
                     && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                     && cm.Status != (int)Const.Status.DELETED
                                         select new UnitDT
                                         {
                                             UnitId = u.UnitId,
                                             Name = u.Name,
                                             Code = u.Code,
                                         }).ToListAsync();
                    //foreach (var item in listMU)
                    //{
                    //    data.ListUnitManager.Add(item.Value);
                    //}

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

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, [FromBody] CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền sửa người dùng!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }

                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }
                    Customer checkEmail = db.Customer.Where(c => c.Email == data.Email.ToLower().Trim() && c.CustomerId != id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //customer.Username = data.Email.ToLower().Trim();
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        //customer.IsEmailConfirm = true;
                        //customer.IsSentEmailConfirm = true;
                        //customer.IsPhoneConfirm = true;
                        customer.Type = data.Type;
                        customer.UnitId = data.UnitId;
                        customer.CountryId = data.CountryId;
                        customer.TypeId = data.TypeId;
                        customer.IdNumber = data.IdNumber;
                        customer.DateNumber = data.DateNumber;
                        customer.AddressNumber = data.AddressNumber;
                        customer.PositionId = data.PositionId;
                        customer.AcademicRankId = data.AcademicRankId;
                        customer.DegreeId = data.DegreeId;
                        customer.RoleId = data.RoleId;
                        customer.IsViewInfo = data.IsViewInfo != null ? data.IsViewInfo : false;
                        //customer.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                        //customer.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                        //customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_ADMIN;
                        //customer.LastLoginAt = DateTime.Now;
                        customer.UserId = userId;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = data.Status != null ? data.Status : (int)Const.Status.NORMAL;
                        db.Entry(customer).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CustomerId > 0)
                            {
                                //Nhóm quyền
                                List<CustomerMapping> customerRoles = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerRoles != null && data.ListRoles != null)
                                {
                                    foreach (var item in data.ListRoles)
                                    {
                                        CustomerMapping customerRole = customerRoles.Where(cf => cf.TargetId == item).FirstOrDefault();
                                        if (customerRole != null)
                                        {
                                            customerRole.TargetId = item;
                                            customerRole.UpdatedId = userId;
                                            customerRole.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerRole);

                                            customerRoles.Remove(customerRole);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerRoles.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                    await db.SaveChangesAsync();
                                }
                                //Lĩnh vực nghiên cứu
                                List<CustomerMapping> customerApplications = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerApplications != null && data.listRA != null)
                                {
                                    foreach (var item in data.listRA)
                                    {
                                        CustomerMapping customerApplication = customerApplications.Where(cf => cf.TargetId == item.CategoryId).FirstOrDefault();
                                        if (customerApplication != null)
                                        {
                                            customerApplication.TargetId = item.CategoryId;
                                            customerApplication.UpdatedId = userId;
                                            customerApplication.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerApplication);

                                            customerApplications.Remove(customerApplication);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item.CategoryId;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerApplications.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                }
                                //Cơ quan tổ chức quản trị
                                List<CustomerMapping> customerUnits = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerUnits != null && data.listMU != null)
                                {
                                    foreach (var item in data.listMU)
                                    {
                                        CustomerMapping customerUnit = customerUnits.Where(cf => cf.TargetId == item.UnitId).FirstOrDefault();
                                        if (customerUnit != null)
                                        {
                                            customerUnit.TargetId = item.UnitId;
                                            customerUnit.UpdatedId = userId;
                                            customerUnit.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerUnit);

                                            customerUnits.Remove(customerUnit);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item.UnitId;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_UNIT;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerUnits.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                }
                                await db.SaveChangesAsync();
                                transaction.Commit();
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
                            if (!CustomerExists(data.CustomerId))
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

        [Authorize]
        [HttpPut("updateInfo/{id}")]
        public async Task<IActionResult> UpdateInfo(int id, [FromBody] CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            //{
            //    def.meta = new Meta(222, "Bạn không có quyền sửa người dùng!");
            //    return Ok(def);
            //}
            try
            {
                if (!ModelState.IsValid || userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }

                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }
                    Customer checkEmail = db.Customer.Where(c => c.Email == data.Email.ToLower().Trim() && c.CustomerId != id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //customer.Username = data.Email.ToLower().Trim();
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        //customer.IsEmailConfirm = true;
                        //customer.IsSentEmailConfirm = true;
                        //customer.IsPhoneConfirm = true;
                        customer.Type = data.Type;
                        customer.UnitId = data.UnitId;
                        customer.CountryId = data.CountryId;
                        customer.TypeId = data.TypeId;
                        customer.IdNumber = data.IdNumber;
                        customer.DateNumber = data.DateNumber;
                        customer.AddressNumber = data.AddressNumber;
                        customer.PositionId = data.PositionId;
                        customer.AcademicRankId = data.AcademicRankId;
                        customer.DegreeId = data.DegreeId;
                        customer.RoleId = data.RoleId;
                        customer.IsViewInfo = data.IsViewInfo != null ? data.IsViewInfo : false;
                        //customer.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                        //customer.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                        //customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_ADMIN;
                        //customer.LastLoginAt = DateTime.Now;
                        customer.UserId = userId;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = data.Status != null ? data.Status : (int)Const.Status.NORMAL;
                        db.Entry(customer).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CustomerId > 0)
                            {
                                //Nhóm quyền
                                List<CustomerMapping> customerRoles = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerRoles != null && data.ListRoles != null)
                                {
                                    foreach (var item in data.ListRoles)
                                    {
                                        CustomerMapping customerRole = customerRoles.Where(cf => cf.TargetId == item).FirstOrDefault();
                                        if (customerRole != null)
                                        {
                                            customerRole.TargetId = item;
                                            customerRole.UpdatedId = userId;
                                            customerRole.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerRole);

                                            customerRoles.Remove(customerRole);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerRoles.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                    await db.SaveChangesAsync();
                                }
                                //Lĩnh vực nghiên cứu
                                List<CustomerMapping> customerApplications = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerApplications != null && data.listRA != null)
                                {
                                    foreach (var item in data.listRA)
                                    {
                                        CustomerMapping customerApplication = customerApplications.Where(cf => cf.TargetId == item.CategoryId).FirstOrDefault();
                                        if (customerApplication != null)
                                        {
                                            customerApplication.TargetId = item.CategoryId;
                                            customerApplication.UpdatedId = userId;
                                            customerApplication.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerApplication);

                                            customerApplications.Remove(customerApplication);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item.CategoryId;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerApplications.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                }
                                //Cơ quan tổ chức quản trị
                                List<CustomerMapping> customerUnits = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerUnits != null && data.listMU != null)
                                {
                                    foreach (var item in data.listMU)
                                    {
                                        CustomerMapping customerUnit = customerUnits.Where(cf => cf.TargetId == item.UnitId).FirstOrDefault();
                                        if (customerUnit != null)
                                        {
                                            customerUnit.TargetId = item.UnitId;
                                            customerUnit.UpdatedId = userId;
                                            customerUnit.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerUnit);

                                            customerUnits.Remove(customerUnit);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item.UnitId;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_UNIT;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerUnits.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                }

                                //Kiểm tra có map vs user ko thì sửa cả bên user
                                var userMap = await db.User.Where(x => x.UserMapId == customer.CustomerId).FirstOrDefaultAsync();
                                if (userMap != null)
                                {
                                    userMap.FullName = customer.FullName;
                                    userMap.Email = customer.Email;
                                    userMap.Phone = customer.Phone;
                                    userMap.Address = customer.Address;
                                    db.User.Update(userMap);
                                }
                                await db.SaveChangesAsync();

                                transaction.Commit();

                                if (data.FullName != null)
                                    HttpContext.Session.SetString("CustomerFullName", data.FullName);
                                else
                                    HttpContext.Session.SetString("CustomerFullName", "");
                                if (data.Avata != null)
                                    HttpContext.Session.SetString("CustomerAvata", data.Avata);
                                else
                                    HttpContext.Session.SetString("CustomerAvata", "");
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
                            if (!CustomerExists(data.CustomerId))
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostCustomer(CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thêm người dùng!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Customer checkUser = db.Customer.Where(c => c.Username == data.Email.ToLower().Trim() && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkUser != null)
                    {
                        def.meta = new Meta(212, "Tài khoản đã tồn tại!");
                        return Ok(def);
                    }
                    Customer checkEmail = db.Customer.Where(c => c.Email == data.Email.ToLower().Trim() && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
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
                        Customer customer = new Customer();
                        customer.Username = data.Email.ToLower().Trim();
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Password = "123456";
                        customer.KeyRandom = Utils.RandomString(8);
                        customer.Password = Utils.GetMD5Hash(customer.Password + customer.KeyRandom);
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.IsEmailConfirm = true;
                        customer.IsSentEmailConfirm = true;
                        customer.IsPhoneConfirm = true;
                        customer.Type = data.Type;
                        customer.UnitId = data.UnitId;
                        customer.CountryId = data.CountryId;
                        customer.TypeId = data.TypeId;
                        customer.IdNumber = data.IdNumber;
                        customer.DateNumber = data.DateNumber;
                        customer.AddressNumber = data.AddressNumber;
                        customer.PositionId = data.PositionId;
                        customer.AcademicRankId = data.AcademicRankId;
                        customer.DegreeId = data.DegreeId;
                        customer.RoleId = data.RoleId;
                        customer.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                        customer.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                        customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_ADMIN;
                        customer.LastLoginAt = DateTime.Now;
                        customer.IsNotificationMail = true;
                        customer.IsNotificationWeb = true;
                        customer.IsViewInfo = data.IsViewInfo != null ? data.IsViewInfo : false;
                        customer.UserId = userId;
                        customer.CreatedAt = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = data.Status != null ? data.Status : (int)Const.Status.NORMAL;
                        await db.Customer.AddAsync(customer);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;
                            if (data.CustomerId > 0)
                            {
                                //Nhóm quyền
                                if (data.ListRoles != null)
                                {
                                    foreach (var item in data.ListRoles)
                                    {
                                        CustomerMapping mapping = new CustomerMapping();
                                        mapping.CustomerId = data.CustomerId;
                                        mapping.TargetId = item;
                                        mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                                        mapping.CreatedId = userId;
                                        mapping.UpdatedId = userId;
                                        mapping.CreatedAt = DateTime.Now;
                                        mapping.UpdatedAt = DateTime.Now;
                                        mapping.Status = (int)Const.Status.NORMAL;
                                        await db.CustomerMapping.AddAsync(mapping);
                                    }
                                }
                                //Lĩnh vực nghiên cứu
                                if (data.listRA != null)
                                {
                                    foreach (var item in data.listRA)
                                    {
                                        CustomerMapping mapping = new CustomerMapping();
                                        mapping.CustomerId = data.CustomerId;
                                        mapping.TargetId = item.CategoryId;
                                        mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION;
                                        mapping.CreatedId = userId;
                                        mapping.UpdatedId = userId;
                                        mapping.CreatedAt = DateTime.Now;
                                        mapping.UpdatedAt = DateTime.Now;
                                        mapping.Status = (int)Const.Status.NORMAL;
                                        await db.CustomerMapping.AddAsync(mapping);
                                    }
                                }
                                //Cơ quan tổ chức quản trị
                                if (data.listMU != null)
                                {
                                    foreach (var item in data.listMU)
                                    {
                                        CustomerMapping mapping = new CustomerMapping();
                                        mapping.CustomerId = data.CustomerId;
                                        mapping.TargetId = item.UnitId;
                                        mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_UNIT;
                                        mapping.CreatedId = userId;
                                        mapping.UpdatedId = userId;
                                        mapping.CreatedAt = DateTime.Now;
                                        mapping.UpdatedAt = DateTime.Now;
                                        mapping.Status = (int)Const.Status.NORMAL;
                                        await db.CustomerMapping.AddAsync(mapping);
                                    }
                                }

                                //Gửi Email xác nhận
                                if (customer.Email != null && customer.Email != "")
                                {
                                    String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-confirm-resgister-customer.html");

                                    string key = Utils.GetMD5Hash(customer.KeyRandom + customer.Password);
                                    string linkConfirm = "'" + config.Website + "thiet-lap-mat-khau-" + key + "-" + customer.CustomerId + "'";
                                    //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                                    data.KeyRandom = "/thiet-lap-mat-khau-" + key + "-" + customer.CustomerId;
                                    sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                        config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                        config.Website, customer.FullName, linkConfirm, customer.KeyRandom);
                                    string subject = config.EmailSender + " - Xác thực tài khoản";
                                    //EmailService emailService = new EmailService();
                                    customer.IsSentEmailConfirm = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject, sBody);
                                }
                                db.Entry(customer).State = EntityState.Modified;

                                await db.SaveChangesAsync();

                                transaction.Commit();
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
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(211, "Khách hàng đã tồn tại!");
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
                def.meta = new Meta(500, "Lỗi server!");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa người dùng!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    Customer data = await db.Customer.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Customer.Update(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CustomerId > 0)
                                transaction.Commit();
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
                            if (!CustomerExists(data.CustomerId))
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

        [Authorize]
        [HttpPost("ResetPassword/{CustomerId}")]
        public async Task<IActionResult> ResetPasswordCustomer([FromBody] ResetPasswordCustomerDTO data, int CustomerId)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
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
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }

                if (data.PasswordInit == null || data.PasswordInit == "")
                {
                    def.meta = new Meta(211, "Password khởi tạo không được để trống!");
                    return Ok(def);
                }

                if (data.Password == null || data.Password == "")
                {
                    def.meta = new Meta(211, "Password không được để trống!");
                    return Ok(def);
                }

                if (data.ConfirmPassword == null || data.ConfirmPassword == "")
                {
                    def.meta = new Meta(211, "Password xác nhận không được để trống!");
                    return Ok(def);
                }

                if (data.ConfirmPassword != data.Password)
                {
                    def.meta = new Meta(211, "Password xác nhận không giống với mật khẩu!");
                    return Ok(def);
                }
                if (!Security.IsPasswordValid(data.Password))
                {
                    def.meta = new Meta(211, "Mật khẩu phải đảm bảo 8 ký tự bao gồm chữ hoa, chữ thường và ít nhất 1 ký tự đặc biệt!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                        return Ok(def);
                    }

                    Customer customer = db.Customer.Where(c => c.CustomerId == CustomerId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy khách hàng!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        customer.Password = Utils.GetMD5Hash(data.Password + customer.KeyRandom);
                        customer.UserId = userId;
                        customer.UpdatedAt = DateTime.Now;
                        db.Customer.Update(customer);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-reset-password.html");
                                sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                    config.Website, customer.FullName, data.PasswordInit);
                                string subject = config.EmailSender + " - Cấp lại mật khẩu";
                                bool SendEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject, sBody);
                                if (SendEmail == true)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    def.meta = new Meta(218, "Lỗi gửi mail!");
                                    return Ok(def);
                                }
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Reset mật khẩu thành công. Đã gửi mail mật khẩu tới email khách hàng!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (!CustomerExists(customer.CustomerId))
                            {
                                def.meta = new Meta(211, "Không tìm thấy Khách hàng!");
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
                def.meta = new Meta(500, "Lỗi server!");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPut("lockUser/{id}/{k}")]
        public async Task<ActionResult> LockUser(int id, byte k)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
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

                using (var db = new IOITDataContext())
                {
                    Customer user = await db.Customer.FindAsync(id);
                    if (user == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }
                    if (user.CustomerId != id)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        user.UpdatedAt = DateTime.Now;
                        user.UserId = userId;
                        user.Status = k;
                        db.Entry(user).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            if (user.CustomerId > 0)
                            {
                                transaction.Commit();

                                //create action
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thay đổi trạng thái tài khoản người dùng";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = user.CustomerId + "";
                                action.TargetName = "Thay đổi trạng thái tài khoản " + k;
                                action.Logs = "Thay đổi trạng thái tài khoản “" + user.FullName + "” - " + k;
                                action.Time = 0;
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserId = id;
                                action.UserPushId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                //    //push action firebase
                                //    Models.Data.Firebase.pushAction(action);

                                //    //push user firebase
                                //    var tasks = new[]
                                //    {
                                //    Task.Run(() => Models.Data.Firebase.updateUser(user))
                                //};
                            }
                            else
                                transaction.Rollback();
                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!CustomerExists(id))
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool CustomerExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Customer.Count(e => e.CustomerId == id) > 0;
            }
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }


        // haohv - 25.08.2023
        [HttpPost("new-register")]
        public async Task<IActionResult> NewRegister(CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                //string Encrypt = Security.Encrypt("123456!@#", data.Password);
                //string Decrypt = Security.Decrypt("123456!@#", Encrypt);
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (data.FullName == null || data.FullName == "")
                {
                    def.meta = new Meta(211, "Tên không được để trống!");
                    return Ok(def);
                }
                if (data.StudentCode == null || data.StudentCode == "")
                {
                    def.meta = new Meta(211, "Mã sinh viên không được để trống!");
                    return Ok(def);
                }
                if (data.SchoolCode == null)
                {
                    def.meta = new Meta(211, "Tên trường không được để trống!");
                    return Ok(def);
                }
                if (data.StudentYear == null || data.StudentYear == "")
                {
                    def.meta = new Meta(211, "Niên khoá không được để trống!");
                    return Ok(def);
                }

                if (data.StudentClass == null || data.StudentClass == "")
                {
                    def.meta = new Meta(211, "Lớp học không được để trống!");
                    return Ok(def);
                }

                if (data.Birthday == null)
                {
                    def.meta = new Meta(211, "Ngày tháng năm sinh không được để trống!");
                    return Ok(def);
                }

                if (data.Email == null)
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }
                if (data.Phone == null)
                {
                    def.meta = new Meta(211, "Số điện thoại không được để trống!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    // Check trùng mã sinh viên
                    var checkExistStudentCode = db.Customer.Where(w => w.StudentCode == data.StudentCode.Trim() && w.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExistStudentCode != null)
                    {
                        def.meta = new Meta(212, "Mã sinh viên đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        string keyRandom = Utils.RandomString(8);
                        string passWordHash = data.Password + keyRandom;
                        Customer customer = new Customer();
                        customer.Username = data.StudentCode.ToLower().Trim();
                        customer.Password = Utils.GetMD5Hash(passWordHash);
                        customer.FullName = data.FullName;
                        customer.Email = data.Email;
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.KeyRandom = keyRandom;
                        customer.IsEmailConfirm = false;
                        customer.IsSentEmailConfirm = false;
                        customer.IsPhoneConfirm = true;
                        customer.IsViewInfo = false;
                        customer.IsNotificationMail = false;
                        customer.IsNotificationWeb = false;
                        customer.UnitId = 0;
                        customer.Type = data.IsUnit ? (int)Const.TypeCustomer.CUSTOMER_UNIT : (int)Const.TypeCustomer.CUSTOMER_PERSONAL;
                        customer.WebsiteId = Const.WEBSITEID;
                        customer.CompanyId = Const.COMPANYID;
                        customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_USER;
                        //customer.KeyToken = Security.Encrypt(keyRandom, data.Password);
                        customer.LastLoginAt = DateTime.Now;
                        customer.CreatedAt = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = (int)Const.Status.NORMAL;

                        // Thêm các trường mới gần đây
                        customer.AcademicRankId = 0;
                        customer.StudentCode = data.StudentCode;
                        customer.StudentClass = data.StudentClass;
                        customer.StudentYear = data.StudentYear;
                        customer.SchoolCode = data.SchoolCode;
                        customer.AchievementNote = data.AchievementNote;
                        customer.HobbyNote = data.HobbyNote;
                        customer.PersonSummary = data.PersonSummary;
                        customer.SocialNetworks = data.SocialNetworks;

                        var mapping = db.Model.FindEntityType(typeof(Customer));
                        string schema = mapping.GetSchema();
                        string table = mapping.GetTableName();

                        try
                        {
                            await db.Customer.AddAsync(customer);
                            await db.SaveChangesAsync();
                            transaction.Commit();

                            def.meta = new Meta(200, "Đăng ký tài khoản thành công!");
                            return Ok(def);
                        }
                        catch (Exception ex)
                        {
                            log.Error("Error:" + ex);
                            def.meta = new Meta(500, "Đăng ký tài khoản thất bại");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Đăng ký tài khoản thất bại");
                return Ok(def);
            }
        }


        [HttpPut("updateRegister/{id}")]
        public async Task<IActionResult> UpdateRegisterId(int id)
        {
            DefaultResponse def = new DefaultResponse();

            using (var db = new IOITDataContext())
            {
                Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (customer == null)
                {
                    def.meta = new Meta(212, "Người đăng ký không tồn tại!");
                    return Ok(def);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    customer.AcademicRankId = customer.AcademicRankId == null ? 1 : customer.AcademicRankId + 1;
                    try
                    {
                        db.Update(customer);
                        await db.SaveChangesAsync();
                        transaction.Commit();

                        def.meta = new Meta(200, "Success");
                        def.data = "thanh cong";
                        return Ok(def);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error:" + ex);
                        def.meta = new Meta(500, "Internal Server Error");
                        return Ok(def);
                    }

                }
            }
        }

        [HttpGet("list-register")]
        public IActionResult ListRegister()
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                using var db = new IOITDataContext();
                // Lấy danh sách người đăng ký có trạng thái đã được cho phép hiển thị
                IQueryable<CustomerDTO> data = db.Customer.Where(c => c.Status != (int)Const.Status.DELETED
                && c.StudentCode != null && c.StudentCode != "").Select(e => new CustomerDTO
                {
                    CustomerId = e.CustomerId,
                    Username = e.Username,
                    Password = e.Password,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Avata = e.Avata,
                    Sex = e.Sex,
                    Birthday = e.Birthday,
                    Address = e.Address,
                    Note = e.Note,
                    KeyRandom = e.KeyRandom,
                    IsEmailConfirm = e.IsEmailConfirm,
                    IsSentEmailConfirm = e.IsSentEmailConfirm,
                    IsPhoneConfirm = e.IsPhoneConfirm,
                    Type = e.Type,
                    UnitId = e.UnitId,
                    CountryId = e.CountryId,
                    TypeId = e.TypeId,
                    IdNumber = e.IdNumber,
                    DateNumber = e.DateNumber,
                    AddressNumber = e.AddressNumber,
                    PositionId = e.PositionId,
                    AcademicRankId = e.AcademicRankId,
                    DegreeId = e.DegreeId,
                    RoleId = e.RoleId,
                    WebsiteId = e.WebsiteId,
                    CompanyId = e.CompanyId,
                    TypeThirdId = e.TypeThirdId,
                    LastLoginAt = e.LastLoginAt,
                    UserId = e.UserId,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Status = e.Status,
                    StudentCode = e.StudentCode,
                    StudentClass = e.StudentClass,
                    StudentYear = e.StudentYear,
                    SchoolCode = e.SchoolCode,
                    AchievementNote = e.AchievementNote,
                    HobbyNote = e.HobbyNote,
                    PersonSummary = e.PersonSummary,
                    SocialNetworks = e.SocialNetworks,
                    IsViewInfo = e.IsViewInfo,
                    StepTwo = e.StepTwo,
                    StepFour = e.StepFour,
                    StepFive = e.StepFive,
                    TopThree = e.TopThree,
                });
                var listData = data.ToList();
                def.data = listData;
                return Ok(def);

            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lấy danh sách thất bại");
                return Ok(def);
            }
        }

        [HttpGet("getByStudentCode/{code}")]
        public async Task<IActionResult> GetByStudentCode(string code)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    CustomerDTO data = await db.Customer.Where(e => e.StudentCode == code).OrderByDescending(w => w.UpdatedAt)
                        .Select(e => new CustomerDTO
                        {
                            CustomerId = e.CustomerId,
                            FullName = e.FullName,
                            Avata = e.Avata,
                            Sex = e.Sex,
                            Birthday = e.Birthday,
                            Address = e.Address,
                            StudentCode = e.StudentCode,
                            StudentClass = e.StudentClass,
                            StudentYear = e.StudentYear,
                            SchoolCode = e.SchoolCode,
                            AchievementNote = e.AchievementNote,
                            HobbyNote = e.HobbyNote,
                            PersonSummary = e.PersonSummary,
                            SocialNetworks = e.SocialNetworks,
                            Note = e.Note,
                            Email = e.Email,
                            Phone = e.Phone,
                            UnitId = e.UnitId,
                            TypeId = e.TypeId,
                            AcademicRankId = e.AcademicRankId,
                            KeyRandom = e.KeyRandom,
                            IsEmailConfirm = e.IsEmailConfirm,
                            IsSentEmailConfirm = e.IsSentEmailConfirm,
                            IsPhoneConfirm = e.IsPhoneConfirm,
                            Type = e.Type,
                            CountryId = e.CountryId,
                            IdNumber = e.IdNumber,
                            DateNumber = e.DateNumber,
                            AddressNumber = e.AddressNumber,
                            PositionId = e.PositionId,
                            DegreeId = e.DegreeId,
                            RoleId = e.RoleId,
                            WebsiteId = e.WebsiteId,
                            CompanyId = e.CompanyId,
                            TypeThirdId = e.TypeThirdId,
                            LastLoginAt = e.LastLoginAt,
                            UserId = e.UserId,
                            CreatedAt = e.CreatedAt,
                            UpdatedAt = e.UpdatedAt,
                            Status = e.Status,
                            IsViewInfo = e.IsViewInfo,
                            StepTwo = e.StepTwo,
                            StepFour = e.StepFour,
                            StepFive = e.StepFive,
                            TopThree = e.TopThree,
                        }).FirstOrDefaultAsync();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy thông tin sinh viên");
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

        [HttpGet("GetGroupByPage")]
        public IActionResult GetGroupByPage()
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                using var db = new IOITDataContext();
                // Lấy danh sách người đăng ký có trạng thái đã được cho phép hiển thị
                IQueryable<TypeAttribute> data = db.TypeAttribute.Where(c => c.Status != (int)Const.Status.DELETED);

                def.data = data.Select(e => new
                {
                    e.TypeAttributeId,
                    e.Name,
                    e.IsUpdate,
                    e.IsDelete,
                    e.TypeAttribuiteParentId,
                    e.UserId,
                    e.CreatedAt,
                    e.UpdatedAt,
                    e.Location,
                    e.Size,
                    e.Status,
                    e.Image,
                    e.Description,
                    e.IsGroup,
                    listCustomer = db.Customer.Where(c => c.TypeAttributeId == e.TypeAttributeId && c.Status != (int)Const.Status.DELETED).Select(c => new
                    {
                        //thêm gì thì tự viết vào
                        c.TypeAttributeId,
                        c.FullName,
                        c.Avata,
                        c.CustomerId,
                        c.StudentCode,
                        //c.FullName,
                        c.CreatedAt,
                        c.UpdatedAt,
                        c.Status
                    }).ToList()
                }).ToList();
                def.meta = new Meta(200, "Success");

                //def.data = data.ToList();
                return Ok(def);

            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lấy danh sách thất bại");
                return Ok(def);
            }
        }

        // GET: web/customer/5
        [HttpGet("GetGroup/{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                using (var db = new IOITDataContext())
                {
                    TypeAttribute data = await db.TypeAttribute.FindAsync(id);

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    else
                    {
                        def.data = new
                        {
                            data.TypeAttributeId,
                            data.Name,
                            data.IsUpdate,
                            data.IsDelete,
                            data.TypeAttribuiteParentId,
                            data.UserId,
                            data.CreatedAt,
                            data.UpdatedAt,
                            data.Location,
                            data.Size,
                            data.Status,
                            data.Image,
                            data.Description,
                            data.IsGroup,
                            listCustomer = db.Customer.Where(c => c.TypeAttributeId == data.TypeAttributeId && c.Status != (int)Const.Status.DELETED)
                                .Select(c => new
                                {
                                    c.TypeAttributeId,
                                    c.FullName,
                                    c.Avata,
                                    c.CustomerId,
                                    c.StudentCode,
                                    c.CreatedAt,
                                    c.UpdatedAt,
                                    c.Status
                                }).ToList()
                        };

                        def.meta = new Meta(200, "Success");
                        return Ok(def);
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