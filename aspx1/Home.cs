using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Gbookv2
{
    public class BC_Home
    {
        public static async Task ProcessRequest(HttpContext context, IApplicationBuilder app)
        {
            string DashboardID = context.Request.Query["DashboardID"];
            string ReturnStr = "";

            switch (DashboardID)
            {
                case "-1":
                    ReturnStr = LoadHtml("index");
                    break;
                case "0":
                    ReturnStr = LoadHtml("welcome");
                    break;
                case "1":
                    ReturnStr = LoadHtml("Task/Task");
                    break;
                case "2":
                    ReturnStr = LoadHtml("System/Register");
                    break;
                case "3":
                    ReturnStr = LoadHtml("System/DemoRegister");
                    break;
                case "4":
                    ReturnStr = LoadHtml("System/DemoRegiter2");
                    break;
                case "5":
                    ReturnStr = LoadHtml("Task/AddTask");
                    break;
                case "6":
                    ReturnStr = LoadHtml("Task/DemoTask1");
                    break;
                default:
                    ReturnStr = LoadHtml("System/Login");
                    break;
            }

            await context.Response.WriteAsync(ReturnStr);
        }

        public static string LoadHtml(string HtmlName)
        {

            StringBuilder HtmlB = new StringBuilder();

            using (StreamReader sr = new StreamReader(Startup.ApplicationBasePath + "/wwwroot/html/" + HtmlName + ".html"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    HtmlB.AppendLine(line);
                }
            }
            return HtmlB.ToString();
        }
    }
}
