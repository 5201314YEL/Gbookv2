using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace Gbookv2
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 开启 session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.IOTimeout = TimeSpan.FromMinutes(30);
            });
        }
        public static string WebRootPath;
        public static string ApplicationBasePath;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            // 使用session
            app.UseCookiePolicy();
            app.UseSession();
            WebRootPath = env.WebRootPath;
            ApplicationBasePath = env.ContentRootPath;
            app.Use(async (context, next) =>
            {
                try
                {
                    //await next.Invoke();
                    //此处后面添加统一的filter类,已经对filter进行所有的过滤后再执行真正的方法。
                    //此处可以做很多的处理，后面有机会添加更为完善的功能
                    if (context.Request.Path != "/favicon.ico")
                    {
                        if (authFilter(context))
                        {
                            await next.Invoke();
                        }
                        else
                        {
                            context.Response.WriteAsync(BC_Home.LoadHtml("System/Login"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    //这个是后期应该是记录到日志里面，不需要直接打印到控制台
                    //此处后面需要做自定义的异常信息，将参数，sql等数据带到exception里面然后统一放到返回值里面
                    System.Console.WriteLine(ex.ToString());
                    FailedReturnData failedData = new FailedReturnData(-1, ex.Message);
                    context.Response.WriteAsync(BC_Fmt.ConvertToJson(failedData));
                }
            });

            app.Map("/Home", HandleMapHome);
            app.Map("/API", HandleMapAPI);
            app.Map("", ProcessRequest);
        }


        private void ProcessRequest(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                string url = "https://" + context.Request.Host + "/Home";
                context.Response.StatusCode = 307;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Location", url);
            });
        }


        private static void HandleMapHome(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await BC_Home.ProcessRequest(context, app);
            });
        }
        private static void HandleMapAPI(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await BC_API.ProcessAPIRequest(context, app);
            });
        }

        private static bool authFilter(HttpContext context)
        {
            string currentPath = context.Request.Path;
            if (currentPath == "/")
            {
                return true;
            }

            bool needCheck = false;
            if (currentPath == "/Home")
            {
                //此处后面通过数组去检验
                string DashboardID = null2Empty(context.Request.Query["DashboardID"]);
                needCheck = DashboardID != " " && DashboardID !="2"&&DashboardID!="3" && DashboardID!="4";
            }
            else if (currentPath == "/API")
            {
                //此处后面通过数组去检验
                string apiCommand = null2Empty(context.Request.Query["APICommand"]);
                needCheck = apiCommand != "Login" && apiCommand != "Register";
            }

            if (needCheck)
            {
                string userInfoStr = context.Session.GetString("UserInfo");
                if (userInfoStr == null)
                {
                    return false;
                }
                IDictionary<string, string> userInfo = JsonConvert.DeserializeObject<IDictionary<string, string>>(userInfoStr);
                context.Items["UserID"] = userInfo["UserID"];
                context.Items["RoleID"] = userInfo["RoleID"];
                context.Items["UserName"] = userInfo["UserName"];
            }
            return true;
        }

        private static bool isEmpty(object str)
        {
            return str == null || str.ToString() == "";
        }

        private static string null2Empty(object str)
        {
            return isEmpty(str) ? "" : str.ToString();
        }
    }
}