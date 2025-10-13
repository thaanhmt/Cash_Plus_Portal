using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IOITWebApp31
{
    public class Startup
    {
        //if (!optionsBuilder.IsConfigured)
        //    {
        //        var builder = new ConfigurationBuilder()
        //           .SetBasePath(Directory.GetCurrentDirectory())
        //           .AddJsonFile("appsettings.json");

        //var configuration = builder.Build();
        //string con = configuration.GetConnectionString("DefaultConnection");
        //optionsBuilder.UseSqlServer(con);
        //    }
        //Scaffold-DbContext "Server=210.86.231.82,5000;Database=IOITDataSetDev;user id=cnttweb;password=cntt@2018;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\EF -Force -Context IOITDataContext
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IStringLocalizer _lang;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDetection();
            //services.AddDbContext<IOITDataContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // Register the Swagger generator, defining 1 or more Swagger documents
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
            //});
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = long.MaxValue;
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue; // <-- ! long.MaxValue
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartHeadersCountLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("0.0.0.0"));
            });

            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();

            string domain = Configuration["AppSettings:JwtIssuer"];

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = domain,
                        ValidAudience = domain,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
            //.AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/cms";
            });

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(600);//You can set Time  
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });

            services.AddWebOptimizer(pipeline =>
            {
                //CMS
                pipeline.AddCssBundle("/cms/main.css",
                    "cms/styles.*.css",
                    "/assets/css/all-font-awesome-6.css,");
                pipeline.AddJavaScriptBundle("/cms/main.js",
                    "cms/runtime.*.js",
                    "cms/polyfills.*.js",
                    "cms/scripts.*.js",
                    "cms/main.*.js"
                    );

                // Creates a CSS and a JS bundle. Globbing patterns supported.
                pipeline.AddCssBundle("/css/main.css", "css/*.css");

                pipeline.AddJavaScriptBundle("/js/main.js",
                    "js/js/jquery.min.js",
                    "js/js/bootstrap.min.js",
                    "js/js/asidebar.jquery.js",
                    "js/js/wow.min.js",
                    "js/js/owl.carousel.min.js");

                pipeline.AddJavaScriptBundle("/js/app.js",
                    "js/angular/angular.min.js",
                    "js/angular/angular-animate.min.js",
                    "js/angular/angular-aria.min.js",
                    "js/angular/angular-material.min.js",
                    "js/angular/loading-bar.min.js",
                    "js/app/app.js",
                    "js/app/customer.js",
                    "js/app/search.js");

                //pipeline.AddJavaScriptBundle("/dist/angular.js",
                //    "dist/*.js");

                // This bundle uses source files from the Content Root and uses a custom PrependHeader extension
                //pipeline.AddJavaScriptBundle("/js/scripts.js", "scripts/a.js", "wwwroot/js/plus.js")
                //        .UseContentRoot()
                //        .PrependHeader("My custom header")
                //        .AddResponseHeader("x-test", "value");

                // This will minify any JS and CSS file that isn't part of any bundle
                //pipeline.MinifyCssFiles();
                //pipeline.MinifyJsFiles();

                // This will automatically compile any referenced .scss files
                //pipeline.CompileScssFiles();

                // AddFiles/AddBundle allow for custom pipelines
                pipeline.AddBundle("/text.txt", "text/plain", "random/*.txt")
                        .AdjustRelativePaths()
                        .Concatenate()
                        .FingerprintUrls()
                        .MinifyCss();
            });

            services.AddResponseCaching();
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddMemoryCache();
            //services.AddControllersWithViews();
            services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization()
            .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
            .AddWebApiConventions();

            services.AddTransient<EFStringLocalizerFactory>();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                                                       .WithOrigins("https://demo.econtract.vn"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, EFStringLocalizerFactory localizerFactory)
        {

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                string path = context.Request.Path;

                if (path.EndsWith(".css") || path.EndsWith(".js"))
                {

                    //Set css and js files to be cached for 30 days
                    TimeSpan maxAge = new TimeSpan(30, 0, 0, 0);     //7 days
                    context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));

                }
                else if (path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".jpeg")
                || path.EndsWith(".png") || path.EndsWith(".webp"))
                {
                    //custom headers for images goes here if needed
                    TimeSpan maxAge = new TimeSpan(30, 0, 0, 0);     //30days
                    context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
                }
                else
                {
                    if (path != "/" && path != "/rss" && path != "/error.html" && path != "" && !path.StartsWith("/api")
                        && !path.StartsWith("/cms") && !path.StartsWith("/xem-truoc-tin-van-ban")
                        && !path.StartsWith("/xem-truoc-tin-hinh-anh") && !path.StartsWith("/xem-truoc-tin-video")
                        && !path.StartsWith("/xem-truoc-tin-su-kien") && !path.StartsWith("/xem-truoc-tin-bai-viet")
                        && !path.StartsWith("/danh-sach-to-chuc") && !path.StartsWith("/nguoi-dung-to-chuc")
                        && path != ("/" + Const.PAGE_ALL_PRODUCT) && path != ("/" + Const.PAGE_ALL_PRODUCT_VN)
                        && path != ("/" + Const.PAGE_SOLUTION) && path != ("/" + Const.PAGE_SOLUTION_VN)
                        && path != ("/" + Const.PAGE_NOMAL_CONTACT) && path != "/" + Const.PAGE_NOMAL_CONTACT_VN
                        && path != ("/" + Const.PAGE_NOMAL_ABOUT) && path != ("/" + Const.PAGE_NOMAL_ABOUT_VN)
                        //&& path != ("/" + Const.PAGE_ABOUT_HTML) && path != ("/" + Const.PAGE_ABOUT_HTML_VN)
                        && path != ("/" + Const.PAGE_NOMAL_TIMELINE) && path != ("/" + Const.PAGE_NOMAL_TIMELINE_VN))
                    //&& path != ("/" + Const.PAGE_NOMAL_FAQ) && path != ("/" + Const.PAGE_NOMAL_FAQ_VN))
                    {
                        //check xem có phân trang ko
                        //string[] path2 = path.Split('?');
                        //Tính toán ở đây
                        using (var db = new IOITDataContext())
                        {
                            var link = db.PermaLink.Where(e => e.Slug == path.Substring(1, path.Length - 1) && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (link != null)
                            {
                                string p = context.Request.Query["p"].ToString() != "" ? context.Request.Query["p"].ToString() : "1";

                                //if (path2.Count() > 1)
                                //{
                                //    p = path2[1].Split('=')[1];
                                //}
                                if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_PAGE_NOMAL)
                                {
                                    context.Request.Path = "/" + Const.PAGE_NOMAL + "/" + link.Slug + "-1-" + link.TargetId + ".html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_NEWS_TEXT)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_NEWS + "/" + link.Slug + "-1-" + link.TargetId + "-" + p + ".html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_NEWS_NOTIFICATION)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_NOTIFICATION + "/" + link.Slug + "-1-" + link.TargetId + "-1.html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_NEWS_IMAGE)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_IMAGE + "/" + link.Slug + "-1-" + link.TargetId + "-1.html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_NEWS_VIDEO)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_VIDEO + "/" + link.Slug + "-1-" + link.TargetId + "-1.html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_NEWS_ATTACTMENT)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_ATTACTMENT + "/" + link.Slug + "-1-" + link.TargetId + "-1.html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_PRODUCT)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_PRODUCT + "/" + link.Slug + "-1-" + link.TargetId + ".html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_CATEGORY_LEGAL_DOC)
                                {
                                    context.Request.Path = "/" + Const.CATEGORY_LEGAL_DOC + "/" + link.Slug + "-1-" + link.TargetId + "-1.html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS)
                                {
                                    context.Request.Path = "/" + Const.DETAIL_NEWS + "/" + link.Slug + "-1-" + link.TargetId + ".html";
                                }
                                else if (link.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_PRODUCT)
                                {
                                    context.Request.Path = "/" + Const.DETAIL_PRODUCT + "/" + link.Slug + "-" + link.TargetId + ".html";
                                }
                            }
                            //else
                            //{
                            //    context.Response.Redirect("/error.html");
                            //    return;
                            //}

                        }
                    }
                    else
                    {
                        if (path == ("/" + Const.PAGE_ALL_PRODUCT))
                        {
                            string p = context.Request.Query["p"].ToString() != "" ? context.Request.Query["p"].ToString() : "1";
                            context.Request.Path = "/" + Const.PAGE_ALL_PRODUCT + "-" + p + ".html";
                        }
                        else if (path == ("/" + Const.PAGE_ALL_PRODUCT_VN))
                        {
                            string p = context.Request.Query["p"].ToString() != "" ? context.Request.Query["p"].ToString() : "1";
                            context.Request.Path = "/" + Const.PAGE_ALL_PRODUCT_VN + "-" + p + ".html";
                        }
                        else if (path == ("/" + Const.PAGE_SOLUTION))
                        {
                            string p = context.Request.Query["p"].ToString() != "" ? context.Request.Query["p"].ToString() : "1";
                            context.Request.Path = "/" + Const.PAGE_SOLUTION + "-" + p + ".html";
                        }
                        else if (path == ("/" + Const.PAGE_SOLUTION_VN))
                        {
                            string p = context.Request.Query["p"].ToString() != "" ? context.Request.Query["p"].ToString() : "1";
                            context.Request.Path = "/" + Const.PAGE_SOLUTION_VN + "-" + p + ".html";
                        }
                        else if (path == ("/" + Const.PAGE_NOMAL_ABOUT))
                        {
                            context.Request.Path = "/" + Const.PAGE_NOMAL_ABOUT + "-4245.html";
                        }
                        else if (path == ("/" + Const.PAGE_NOMAL_ABOUT_VN))
                        {
                            context.Request.Path = "/" + Const.PAGE_NOMAL_ABOUT_VN + "-4172.html";
                        }
                        //else if (path == ("/" + Const.PAGE_ABOUT_HTML))
                        //{
                        //    context.Request.Path = "/" + Const.PAGE_ABOUT_HTML + "-4248.html";
                        //}
                        //else if (path == ("/" + Const.PAGE_ABOUT_HTML_VN))
                        //{
                        //    context.Request.Path = "/" + Const.PAGE_ABOUT_HTML_VN + "-4175.html";
                        //}
                    }
                    //Request for views fall here.
                    context.Response.Headers.Append("Cache-Control", "no-cache");
                    context.Response.Headers.Append("Cache-Control", "private, no-store");

                }
                await next();
            });

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".cache"] = "image/jpg";
            provider.Mappings[".woff"] = "font/woff";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 7;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            _lang = localizerFactory.Create(null);

            // a list of all available languages
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("vi-VN"),
                new CultureInfo("en-US"),
                //new CultureInfo("fa-IR")
            };

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };
            requestLocalizationOptions.RequestCultureProviders.Insert(0, new JsonRequestCultureProvider());
            app.UseRequestLocalization(requestLocalizationOptions);
            //app.UseDetection();
            app.UseWebOptimizer();
            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    "CMS",
                    "cms",
                        new { controller = "Home", action = "CMS" }
                );
                endpoints.MapControllerRoute(
                    "ListMerchant",
                    "ListMerchant",
                        new { controller = "Home", action = "ListMerchant" }
                );

                endpoints.MapControllerRoute(
                    "Rss",
                    "rss",
                        new { controller = "Home", action = "Rss" }
                );

                endpoints.MapControllerRoute(
                   "RssItem",
                    "rss/{name}-{id}.rss",
                       new { controller = "Home", action = "RssItem" },
                       new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );

                endpoints.MapControllerRoute(
                    "Partner",
                    "doi-tac.html",
                        new { controller = "Home", action = "Partner" }
                );
                // giai phap
                endpoints.MapControllerRoute(
                   "SolutionsServices",
                    Const.PAGE_SOLUTION + "-{p}.html",
                       new { controller = "Category", action = "SolutionsServices" },
                       new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );
                endpoints.MapControllerRoute(
                   "SolutionsServicesVn",
                    Const.PAGE_SOLUTION_VN + "-{p}.html",
                       new { controller = "Category", action = "SolutionsServices" },
                       new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );
                // list all san pham
                endpoints.MapControllerRoute(
                   "ListAllProduct",
                   Const.PAGE_ALL_PRODUCT + "-{p}.html",
                       new { controller = "Category", action = "ListAllProduct" },
                        new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );
                endpoints.MapControllerRoute(
                   "ListAllProductVn",
                   Const.PAGE_ALL_PRODUCT_VN + "-{p}.html",
                       new { controller = "Category", action = "ListAllProduct" },
                        new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );


                //router search
                endpoints.MapControllerRoute(
                    "SearchInfo",
                    "tim-kiem-thong-tin",
                    new { controller = "Search", action = "ResulftSearch" }
                    //new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                    "SearchDataSet",
                    "tim-kiem-dataset",
                    new { controller = "Search", action = "ResulftSearchData" }
                    //new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                   "SearchAr",
                   "tim-kiem-du-lieu-pham-vi-ung-dung",
                   new { controller = "Search", action = "ResulftSearchAr" }
               //new string[] { "IOITWebApp31.Controllers" }
               );
                endpoints.MapControllerRoute(
                   "LinkAllAr",
                   "pham-vi-ung-dung-{id}",
                   new { controller = "Search", action = "LinkAllAr" },
                   new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );

                endpoints.MapControllerRoute(
                    "ShippingAddress",
                    "tien-hanh-dat-hang.html",
                    new { controller = "Checkout", action = "ShippingAddress" }
                );

                endpoints.MapControllerRoute(
                   "Payments",
                   "hinh-thuc-thanh-toan.html",
                   new { controller = "Checkout", action = "Payments" }
               );

                endpoints.MapControllerRoute(
                   "RePayments",
                   "thuc-hien-thanh-toan.html",
                   new { controller = "Checkout", action = "RePayments" }
               );

                endpoints.MapControllerRoute(
                   "OrderResults",
                   "ket-qua-don-hang.html",
                   new { controller = "Checkout", action = "OrderResults" }
               );

                endpoints.MapControllerRoute(
                   "ResulftSearch",
                   "tim-kiem.html",
                   new { controller = "Search", action = "ResulftSearch" }
                   //new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router category
                endpoints.MapControllerRoute(
                "CategoryGroupProduct",
                "san-pham.html",
                new { controller = "Category", action = "GroupProduct" }
               );

                endpoints.MapControllerRoute(
               "CategoryGroupNews",
               "tin-tuc.html",
               new { controller = "Category", action = "GroupNews" }
              );
                // van ban
                endpoints.MapControllerRoute(
                 "LegalDoc",
                 "van-ban.html",
                 new { controller = "Home", action = "LegalDoc" }
                );

                endpoints.MapControllerRoute(
                 "DetailLegalDoc",
                 "chi-tiet-van-ban-{LegalDocId}.html",
                 new { controller = "Detail", action = "DetailLegalDoc" },
                 new { LegalDocId = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //trang tin
                endpoints.MapControllerRoute(
                 "News",
                 "/tin-tuc-{p}.html",
                 new { controller = "Home", action = "News", seoName = "" },
                 new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "CategoryNews",
                 Const.CATEGORY_NEWS + "/{seoName}-{idw}-{id}-{p}.html",
                 new { controller = "Category", action = "News", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$", p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "CategoryImage",
                  Const.CATEGORY_IMAGE + "/{seoName}-{idw}-{id}-{p}.html",
                 new { controller = "Category", action = "Image", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$", p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "CategoryVideo",
                 Const.CATEGORY_VIDEO + "/{seoName}-{idw}-{id}-{p}.html",
                 new { controller = "Category", action = "Video", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$", p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "CategoryNotification",
                 Const.CATEGORY_NOTIFICATION + "/{seoName}-{idw}-{id}-{p}.html",
                 new { controller = "Category", action = "Notification", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$", p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "CategoryAttactment",
                 Const.CATEGORY_ATTACTMENT + "/{seoName}-{idw}-{id}-{p}.html",
                 new { controller = "Category", action = "Attactment", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$", p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "CategoryProduct",
                 Const.CATEGORY_PRODUCT + "/{seoName}-{id}.html",
                 new { controller = "Category", action = "Product", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                "CategoryProductChild",
                Const.CATEGORY_PRODUCT_CHILD + "/{seoName}-{id}-{p}.html",
                new { controller = "Category", action = "ProductChild", seoName = "" },
                new { id = @"^\d+$", p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );

                //endpoints.MapControllerRoute(
                // "CategoryProduct",
                // "danh-muc-san-pham.html",
                // new { controller = "Category", action = "Product", seoName = "" }
                //);

                //router page
                endpoints.MapControllerRoute(
                 "Page",
                 Const.PAGE_NOMAL + "/{seoName}-{idw}-{id}.html",
                 new { controller = "Category", action = "Page", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                // trang gioi thieu
                endpoints.MapControllerRoute(
                "PageParent",
                Const.PAGE_NOMAL_ABOUT + "-{id}.html",
                new { controller = "Category", action = "PageParent" },
                new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );

                endpoints.MapControllerRoute(
                "PageParentVn",
                Const.PAGE_NOMAL_ABOUT_VN + "-{id}.html",
                new { controller = "Category", action = "PageParent" },
                new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );
                //  // co cau to chuc
                //endpoints.MapControllerRoute(
                //   "PageHTML",
                //   Const.PAGE_ABOUT_HTML + "-{id}.html",
                //   new { controller = "Category", action = "PageHTML" },
                //      new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                //  );
                //endpoints.MapControllerRoute(
                //   "PageHTMLVn",
                //   Const.PAGE_ABOUT_HTML_VN + "-{id}.html",
                //   new { controller = "Category", action = "PageHTML" },
                //      new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                //  );

                endpoints.MapControllerRoute(
                 "ListGallery",
                 Const.CATEGORY_IMAGE + "-{p}.html",
                 new { controller = "Category", action = "ListGallery" },
                 new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                "ListVideo",
                Const.CATEGORY_VIDEO + "-{p}.html",
                new { controller = "Category", action = "ListVideo" },
                new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
               );

                //router tag
                endpoints.MapControllerRoute(
                 "TagNews",
                 Const.TAG_NEWS + "/{seoName}-{idw}-{p}.html",
                 new { controller = "Tag", action = "TagNews", seoName = "" },
                 new { p = @"^\d+$", idw = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "TagProduct",
                 Const.TAG_PRODUCT + "/{seoName}-{idw}-{p}.html",
                 new { controller = "Tag", action = "TagProduct", seoName = "" },
                 new { p = @"^\d+$", idw = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router detail
                endpoints.MapControllerRoute(
                 "DetailNews",
                 Const.DETAIL_NEWS + "/{seoName}-{idc}-{id}.html",
                 new { controller = "Detail", action = "News", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "PreviewDetailNews",
                 "xem-truoc-tin-van-ban/{seoName}-{id}.html",
                 new { controller = "Preview", action = "News", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "PreviewDetailNewsText",
                 "xem-truoc-tin-bai-viet/{seoName}-{id}.html",
                 new { controller = "Preview", action = "News", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "DetailImage",
                 Const.DETAIL_IMAGE + "/{seoName}-{idw}-{id}.html",
                 new { controller = "Detail", action = "Image", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "PreviewDetailNewsImage",
                 "xem-truoc-tin-hinh-anh/{seoName}-{id}.html",
                 new { controller = "Preview", action = "Image", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "DetailVideo",
                 Const.DETAIL_VIDEO + "/{seoName}-{id}.html",
                 new { controller = "Detail", action = "Video", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "PreviewDetailNewsVideo",
                 "xem-truoc-tin-video/{seoName}-{id}.html",
                 new { controller = "Preview", action = "Video", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "PreviewDetailNewsEvent",
                 "xem-truoc-tin-su-kien/{seoName}-{id}.html",
                 new { controller = "Preview", action = "Event", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "DetailAttactment",
                 Const.DETAIL_ATTACTMENT + "/{seoName}-{id}.html",
                 new { controller = "Detail", action = "Attactment", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "DetailNotification",
                 Const.DETAIL_ATTACTMENT + "/{seoName}-{id}.html",
                 new { controller = "Detail", action = "Notification", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "DetailProduct",
                 Const.DETAIL_PRODUCT + "/{seoName}-{id}.html",
                 new { controller = "Detail", action = "Product", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                endpoints.MapControllerRoute(
                 "DetailPartner",
                 Const.DETAIL_PARTNER + "/{seoName}-{idw}-{id}.html",
                 new { controller = "Detail", action = "Partner", seoName = "" },
                 new { id = @"^\d+$", idw = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router customer
                endpoints.MapControllerRoute(
                    "GuideDisableAccount",
                    "huong-dan-xoa-tai-khoan-cashplus",
                    new { controller = "Customer", action = "GuideDisableAccount" }
                );
                endpoints.MapControllerRoute(
                "Login",
                "dang-nhap",
                new { controller = "Customer", action = "Login" }
                );
                endpoints.MapControllerRoute(
                    "Register",
                    "dang-ky",
                    new { controller = "Customer", action = "Register" }
                ); 
                endpoints.MapControllerRoute(
                    "Register",
                    "dang-ky-thong-tin",
                    new { controller = "Customer", action = "RegisterInfo" }
                );
                endpoints.MapControllerRoute(
                   "CashplusInfo",
                   "ve-cashplus",
                   new { controller = "Customer", action = "CashplusInfo" }
               );
                endpoints.MapControllerRoute(
                   "ListMerchant",
                   "danh-sach-merchant",
                   new { controller = "Customer", action = "ListMerchant" }
               );
                endpoints.MapControllerRoute(
                "RegisterPartner",
                "dang-ky-doi-tac",
                new { controller = "Customer", action = "RegisterPartner" }
                );
                endpoints.MapControllerRoute(
                 "RegisterPartnerStepthree",
                 "xac-nhan-dang-ky-doi-tac",
                 new { controller = "Customer", action = "RegisterPartnerStepthree" }
                 );
                endpoints.MapControllerRoute(
                   "PartnerUpdate",
                   "nhap-thong-tin-dang-ky",
                   new { controller = "Customer", action = "PartnerUpdate" }
                   );
                endpoints.MapControllerRoute(
                 "ListRegister",
                 "danh-sach-nguoi-dang-ky",
                 new { controller = "Customer", action = "ListRegister" }
                 );
                endpoints.MapControllerRoute(
                "ProvincePartner",
                "dang-ky-dai-dien-tinh-thanh",
                new { controller = "Customer", action = "ProvincePartner" }
                );

                endpoints.MapControllerRoute(
                "Dowloadnow",
                "tai-cashplus-ngay",
                new { controller = "Customer", action = "Dowloadnow" }
                );
                endpoints.MapControllerRoute(
               "DowloadMernow",
               "tai-cashplus-merchant-ngay",
               new { controller = "Customer", action = "DowloadMernow" }
               );
                endpoints.MapControllerRoute(
                "DetailTeam",
                "chi-tiet-nhom",
                new { controller = "Customer", action = "DetailTeam" }
                );
                endpoints.MapControllerRoute(
                "DetailMerchant",
                "shop/{idmerchant}",
                new { controller = "Customer", action = "DetailInfo" }
                );
                endpoints.MapControllerRoute(
                  "RegisterPartner",
                  "dang-ky-doi-tac",
                  new { controller = "Customer", action = "RegisterPartner" }
                  );
                endpoints.MapControllerRoute(
                 "RegisterCode",
                 "dang-ky-cashplus",
                 new { controller = "Customer", action = "RegisterCode" }
                 );

                //endpoints.MapControllerRoute(
                //"Condition",
                //"dieu-khoan-va-dieu-kien",
                //new { controller = "Customer", action = "Condition" }
                //);
                endpoints.MapControllerRoute(
                 "InfoQR",
                 "dang-ky-thong-tin-nguoi-gioi-thieu",
                 new { controller = "Customer", action = "InfoQR" }
                 );

                endpoints.MapControllerRoute(
                "a",
                "a",
                new { controller = "Customer", action = "Affiliate" }
                );
                endpoints.MapControllerRoute(
                  "DetailCashplus",
                  "chien-binh-khoi-nghiep",
                  new { controller = "Customer", action = "DetailCashplus" }
                  );
                endpoints.MapControllerRoute(
                   "PrivacyPolicy",
                   "tin-chinh-sach-bao-mat",
                   new { controller = "Customer", action = "PrivacyPolicy" }
                );
                endpoints.MapControllerRoute(
                     "News",
                     "noi-dung-bai-viet",
                     new { controller = "Customer", action = "News" }
                );

                endpoints.MapControllerRoute(
                 "Contact",
                 "lien-he-cashplus",
                 new { controller = "Customer", action = "Contact" }
                 );
                endpoints.MapControllerRoute(
                    name: "DetailUser",
                    pattern: "chi-tiet",
                    defaults: new { controller = "Customer", action = "DetailUser" }
                );
                endpoints.MapControllerRoute(
                    name: "DetailUser",
                    pattern: "chi-tiet/{id}",
                    defaults: new { controller = "Customer", action = "DetailUser" }
                );
                endpoints.MapControllerRoute(
                "RecoverPassword",
                "lay-lai-mat-khau",
                new { controller = "Customer", action = "RecoverPassword" }
                );
                endpoints.MapControllerRoute(
                "ThankYouPage",
                "thankyou",
                new { controller = "Customer", action = "ThankYouPage" }
                );
                endpoints.MapControllerRoute(
                "SettingPassword",
                "xac-thuc-tai-khoan-{key}-{id}",
                new { controller = "Customer", action = "SettingPassword", key = "" },
                new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                "ResetPassword",
                "thiet-lap-mat-khau-{key}-{id}",
                new { controller = "Customer", action = "ResetPassword", key = "" },
                new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                "InfoUserPage",
                "ho-so",
                new { controller = "Customer", action = "InfoUserPage" }
                );
                endpoints.MapControllerRoute(
                "InfoGeneralPage",
                "thong-tin-chung",
                new { controller = "Customer", action = "InfoGeneralPage" }
                );
                endpoints.MapControllerRoute(
                    "ManageUser",
                    "nguoi-dung-to-chuc",
                    new { controller = "Customer", action = "ManageUser" }
                );
                endpoints.MapControllerRoute(
                    "AddCustomerPage",
                    "tao-moi-nguoi-dung",
                    new { controller = "Customer", action = "AddCustomerPage" }
                );
                endpoints.MapControllerRoute(
                    "ViewCustomerPage",
                    "xem-chi-tiet-nguoi-dung-{id}",
                    new { controller = "Customer", action = "ViewCustomerPage" },
                    new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                    "UpdateCustomerPage",
                    "cap-nhat-nguoi-dung-{id}",
                    new { controller = "Customer", action = "UpdateCustomerPage" },
                    new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                    "ManageUnit",
                    "danh-sach-to-chuc",
                    new { controller = "Customer", action = "ManageUnit" }
                );
                endpoints.MapControllerRoute(
                    "AddCompanyPage",
                    "tao-moi-co-quan-to-chuc",
                    new { controller = "Customer", action = "AddCompanyPage" }
                );
                endpoints.MapControllerRoute(
                    "ViewCompanyPage",
                    "xem-chi-tiet-co-quan-to-chuc-{id}",
                    new { controller = "Customer", action = "ViewCompanyPage" },
                    new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                    "UpdateCompanyPage",
                    "cap-nhat-co-quan-to-chuc-{id}",
                    new { controller = "Customer", action = "UpdateCompanyPage" },
                    new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                "AccountInformation",
                "thong-tin-tai-khoan",
                new { controller = "Customer", action = "AccountInformation" }
                );
                endpoints.MapControllerRoute(
                    "ManageLink",
                    "quan-tri-du-lieu",
                    new { controller = "Customer", action = "ManageLink" }
                );
                //router Timeline
                endpoints.MapControllerRoute(
                 "Timeline",
                  "/" + Const.PAGE_NOMAL_TIMELINE_VN,
                 new { controller = "Category", action = "YearTimeline" }
                );
                //router Timeline
                endpoints.MapControllerRoute(
                 "TimelineEn",
                 "/" + Const.PAGE_NOMAL_TIMELINE,
                 new { controller = "Category", action = "YearTimeline" }
                );

                //router xac nhan dang ky thanh vien thanh cong
                endpoints.MapControllerRoute(
                 "ConfirmRegister",
                 "xac-nhan-dang-ky-{key}-{id}.html",
                 new { controller = "Customer", action = "ConfirmRegister", key = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router nguoi dung check trong email
                endpoints.MapControllerRoute(
                 "ConfirmEmailRegister",
                 "email-xac-nhan-dang-ky-thanh-vien-{key}-{id}.html",
                 new { controller = "Customer", action = "ConfirmEmailRegister", key = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );



                endpoints.MapControllerRoute(
                "UserAuthentication",
                "xac-thuc-tai-khoan.html",
                new { controller = "Customer", action = "UserAuthentication" }
               );

                endpoints.MapControllerRoute(
                "Logout",
                "dang-xuat",
                new { controller = "Customer", action = "Logout" }
               );

                endpoints.MapControllerRoute(
                "AddressCustomer",
                "danh-sach-dia-chi.html",
                new { controller = "Customer", action = "AddressCustomer" }
               );
                endpoints.MapControllerRoute(
                "ChangePass",
                "doi-mat-khau",
                new { controller = "Customer", action = "ChangePass" }
               );

                endpoints.MapControllerRoute(
               "EditUser",
               "sua-tai-khoan.html",
               new { controller = "Customer", action = "EditUser" }
              );
                endpoints.MapControllerRoute(
              "MyOrder",
              "don-hang-cua-toi.html",
              new { controller = "Customer", action = "MyOrder" }
             );

                endpoints.MapControllerRoute(
              "DetailOrder",
              "chi-tiet-don-hang-{OrderId}.html",
              new { controller = "Customer", action = "DetailOrder" },
              new { OrderId = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
             );

                endpoints.MapControllerRoute(
                   "InfoUser",
                   "trang-ca-nhan.html",
                   new { controller = "Customer", action = "InfoUser" }
                  );

                endpoints.MapControllerRoute(
                   "ReviewProduct",
                   "danh-gia-san-pham.html",
                   new { controller = "Customer", action = "ReviewProduct" }
                  );

                endpoints.MapControllerRoute(
                   "Wishlists",
                   "san-pham-yeu-thich.html",
                   new { controller = "Customer", action = "Wishlists" }
                  );

                endpoints.MapControllerRoute(
                   "FollowOrder",
                   "theo-doi-don-hang.html",
                   new { controller = "Customer", action = "FollowOrder" }
                  );

                endpoints.MapControllerRoute(
                   "KoiFollow",
                   "koi-theo-doi.html",
                   new { controller = "Customer", action = "KoiFollow" }
                  );

                endpoints.MapControllerRoute(
                   "Compare",
                   "so-sanh-san-pham.html",
                   new { controller = "Customer", action = "Compare" }
                  );

                //router cart
                endpoints.MapControllerRoute(
                    "ShoppingCart",
                    "gio-hang.html",
                    new { controller = "ShoppingCart", action = "ShoppingCart" }
                );

                endpoints.MapControllerRoute(
                    "Checkout",
                    "thuc-hien-mua-hang",
                    new { controller = "Checkout", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    "ComplateOrder",
                    "hoan-tat-dat-hang-{id}.html",
                    new { controller = "Checkout", action = "ComplateOrder" }
                );

                //router error
                endpoints.MapControllerRoute(
                 "Error",
                 "error.html",
                 new { controller = "Home", action = "Error" }
                );

                //router othor
                //endpoints.MapControllerRoute(
                // "Contact",
                // Const.PAGE_NOMAL_CONTACT,
                // new { controller = "Home", action = "Contact" }
                //);

                endpoints.MapControllerRoute(
                 "ContactVn",
                 Const.PAGE_NOMAL_CONTACT_VN,
                 new { controller = "Home", action = "Contact" }
                );

                // endpoints.MapControllerRoute(
                //    "PostFeed",
                //    "Feed/{type}",
                //    new { controller = "Home", action = "PostFeed", type = "rss" }
                // );

                //router home
                endpoints.MapControllerRoute(
                 "IndexHome",
                 "trang-chu.html",
                 new { controller = "Home", action = "Index" }
                 //new string[] { "IOITWebApp31.Controllers" }
                );

                //router Dành cho người mới bắt đầu
                endpoints.MapControllerRoute(
                 "ForNewBie",
                 "danh-cho-nguoi-moi.html",
                 new { controller = "Home", action = "ForNewBie" }
                );

                //router Hướng dẫn sử dụng
                endpoints.MapControllerRoute(
                 "UserManual",
                 "huong-dan-dau-gia.html",
                 new { controller = "Home", action = "UserManual" }
                );

                //router Câu hỏi thường gặp
                endpoints.MapControllerRoute(
                 "FAQVn",
                 "/" + Const.PAGE_NOMAL_FAQ_VN,
                 new { controller = "Home", action = "FAQ" }
                );
                endpoints.MapControllerRoute(
                "FAQ",
                "tin-tuc-faq",
                new { controller = "Home", action = "FAQ" }
                );
                //router Chính sách vận chuyển
                endpoints.MapControllerRoute(
                 "ShippingPolicy",
                 "chinh-sach-van-chuyen.html",
                 new { controller = "Home", action = "ShippingPolicy" }
                );

                //router Hướng dẫn thanh toán
                endpoints.MapControllerRoute(
                 "PaymentGuide",
                 "huong-dan-thanh-toan.html",
                 new { controller = "Home", action = "PaymentGuide" }
                );

                //router Chính sách bảo mật
                endpoints.MapControllerRoute(
                 "PrivacyPolicy",
                 "chinh-sach-bao-mat.html",
                 new { controller = "Home", action = "PrivacyPolicy" }
                );

                //router Hướng dẫn mua hàng
                endpoints.MapControllerRoute(
                 "ShoppingGuide",
                 "huong-dan-mua-hang.html",
                 new { controller = "Home", action = "ShoppingGuide" }
                );

                //router Dịch vụ
                endpoints.MapControllerRoute(
                 "Service",
                 "dich-vu.html",
                 new { controller = "Home", action = "Service" }
                );

                //router Thư viện ảnh
                endpoints.MapControllerRoute(
                 "LibraryImage",
                 "thu-vien-anh-{p}.html",
                 new { controller = "Home", action = "LibraryImage" },
                 new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router Thư viện video
                endpoints.MapControllerRoute(
                 "LibraryVideo",
                 "thu-vien-video-{p}.html",
                 new { controller = "Home", action = "LibraryVideo" },
                 new { p = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router Trang profile
                endpoints.MapControllerRoute(
                 "Profile",
                 "thong-tin-tai-khoan.html",
                 new { controller = "Home", action = "Profile" }
                );

                //router detail auction
                endpoints.MapControllerRoute(
                 "DetailAuction",
                 Const.DETAIL_AUCTION + "/{seoName}-{id}.html",
                 new { controller = "Aution", action = "Index", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");


                //endpoints.MapControllerRoute(
                //   name: "index",
                //   template: "{*url}");
                //defaults: new { controller = "Home", action = "Index" });


                //router Trang ấn phẩm
                endpoints.MapControllerRoute(
                 "FAQ",
                 "an-pham",
                 new { controller = "Home", action = "FAQ" }
                );
                endpoints.MapControllerRoute(
                 "FAQ",
                 "an-pham" + "/{seoName}-{Id}",
                 new { controller = "Detail", action = "FAQ" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );

                //router Trang Văn Bản Pháp Luật
                endpoints.MapControllerRoute(
                 "LegalDocs",
                 "de-tai-van-ban",
                 new { controller = "Home", action = "LegalDocs" }
                );
                endpoints.MapControllerRoute(
                 "DetailsLegalDocs",
                 "van-ban" + "/{seoName}-{Id}",
                 new { controller = "Detail", action = "LegalDocs" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "DataPage",
                 "tong-quan-trang-du-lieu",
                 new { controller = "Home", action = "DataPage" }
                );
                endpoints.MapControllerRoute(
                 "DataPageAll",
                 "toan-bo-du-lieu",
                 new { controller = "Home", action = "DataPageAll" }
                );
                endpoints.MapControllerRoute(
                 "DetailData",
                 "chi-tiet-du-lieu/{seoName}-{id}",
                 new { controller = "Detail", action = "DetailData", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "DataForUser",
                 "du-lieu-ca-nhan/{id}",
                 new { controller = "Home", action = "DataForUser", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "DataForUnit",
                 "du-lieu-co-quan-to-chuc/{id}",
                 new { controller = "Home", action = "DataForUnit", seoName = "" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "ViewInfoUser",
                 "thong-tin-ca-nhan",
                 new { controller = "Home", action = "ViewInfoUser" }
                );
                endpoints.MapControllerRoute(
                 "DataAuthor",
                 "du-lieu-tac-gia",
                 new { controller = "Home", action = "DataAuthor" }
                );
                endpoints.MapControllerRoute(
                 "ManageData",
                 "quan-ly-du-lieu",
                 new { controller = "Customer", action = "ManageData" }
                );
                endpoints.MapControllerRoute(
                 "ViewData",
                 "xem-chi-tiet-du-lieu-{id}",
                 new { controller = "Customer", action = "ViewData" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "CreateNewData",
                 "tao-moi-du-lieu",
                 new { controller = "Customer", action = "CreateNewData" }
                );
                endpoints.MapControllerRoute(
                 "CreateDataFromCeph",
                 "xac-thuc-bo-du-lieu",
                 new { controller = "Customer", action = "CreateDataFromCeph" }
                );
                endpoints.MapControllerRoute(
                 "UpdateData",
                 "cap-nhat-du-lieu-{id}",
                 new { controller = "Customer", action = "UpdateData" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "ManageNoitify",
                 "quan-ly-thong-bao",
                 new { controller = "Customer", action = "ManageNoitify" }
                );
                endpoints.MapControllerRoute(
                    "ViewNoitify",
                    "xem-chi-tiet-thong-bao-{id}",
                    new { controller = "Customer", action = "ViewNoitify" }
                );
                endpoints.MapControllerRoute(
                 "SettingNoitify",
                 "cai-dat-thong-bao",
                 new { controller = "Customer", action = "SettingNoitify" }
                );
                endpoints.MapControllerRoute(
                 "ContactFaQ",
                 "faqs",
                 new { controller = "Home", action = "ContactFaQ" }
                );
                endpoints.MapControllerRoute(
                 "DetailFaq",
                 "chi-tiet-faq" + "/{seoName}-{Id}",
                 new { controller = "Detail", action = "DetailFaq" },
                 new { id = @"^\d+$" }, new string[] { "IOITWebApp31.Controllers" }
                );
                endpoints.MapControllerRoute(
                 "ContactPage",
                 "lien-he",
                 new { controller = "Home", action = "ContactPage" }
                );
                endpoints.MapControllerRoute(
                 "RouterDev",
                 "lien-ket-template",
                 new { controller = "Home", action = "RouterDev" }
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                   name: "index",
                   pattern: "{*url}");

                //endpoints.MapFallbackToFile("cms");
                //endpoints.MapFallbackToPage("spa-fallback", new { controller = "Home", action = "CMS" });
                // su kien
                endpoints.MapControllerRoute(
                 "revenueGrowth",
                 "ho-tro-tang-truong-doanh-thu",
                 new { controller = "Customer", action = "revenueGrowth" }
                 );
                //Xử lý đăng ký OTP Test
                endpoints.MapControllerRoute(
                 "RecoverOTP",
                 "dang-ky-tai-khoan-otp",
                 new { controller = "Customer", action = "RecoverOTP" }
                 );
                endpoints.MapControllerRoute(
                 "RecoverPhoneOTP",
                 "thay-doi-dien-thoai-otp",
                 new { controller = "Customer", action = "RecoverPhoneOTP" }
                 );
                endpoints.MapControllerRoute(
                 "RecoverPassOTP",
                 "quen-mat-khau-otp",
                 new { controller = "Customer", action = "RecoverPassOTP" }
                 );                
                endpoints.MapControllerRoute(
                 "RecoverSecurityOTP",
                 "quen-ma-bao-mat-otp",
                 new { controller = "Customer", action = "RecoverSecurityOTP" }
                 );
                endpoints.MapControllerRoute(
                 "RecoverChangeSecurityOTP",
                 "thay-doi-ma-bao-mat-otp",
                 new { controller = "Customer", action = "RecoverChangeSecurityOTP" }
                 );
                endpoints.MapControllerRoute(
                 "RecoverSetupSecurityOTP",
                 "cai-dat-ma-bao-mat-otp",
                 new { controller = "Customer", action = "RecoverSetupSecurityOTP" }
                 );
                //Kết thúc đăng ký OTP Test
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot/cms";

                //if (env.IsDevelopment())
                //{
                //    spa.UseProxyToSpaDevelopmentServer(npmScript: "start");
                //}
            });

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials());
        }

        public class JsonRequestCultureProvider : RequestCultureProvider
        {
            public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
            {
                if (httpContext == null)
                {
                    throw new ArgumentNullException(nameof(httpContext));
                }

                string culture = httpContext.Request.Cookies["Language"];
                string uiCulture = culture;

                culture = culture ?? "vi-VN";
                uiCulture = uiCulture ?? culture;

                return Task.FromResult(new ProviderCultureResult(culture, uiCulture));
            }
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        public static bool DisallowsSameSiteNone(string userAgent)
        {
            // Check if a null or empty string has been passed in, since this
            // will cause further interrogation of the useragent to fail.
            if (String.IsNullOrWhiteSpace(userAgent))
                return false;

            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking
            // stack.
            if (userAgent.Contains("CPU iPhone OS 12") ||
                userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // Cover Mac OS X based browsers that use the Mac OS networking stack. 
            // This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }

    }
}
