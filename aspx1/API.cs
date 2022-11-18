using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;   //需要加上
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Gbookv2
{
    public class BC_API
    {
        public static async Task ProcessAPIRequest(HttpContext context, IApplicationBuilder app)
        {
            string APICommand = context.Request.Query["APICommand"];
            string ReturnStr = "";
            Mysql mysql = new Mysql();
            StringBuilder sqlB = new StringBuilder();
            MySqlDataReader DataReader = null;

            // 执行方法获取请求参数
            IDictionary<string, string> RequestParameter = await BC_Fmt.GetRequestParameter(context.Request);
            List<object> list = new List<object>();

            switch (APICommand)
            {
                case "Login":
                    {
                        string UserName = RequestParameter["UserName"];
                        string Password = RequestParameter["Password"];


                        sqlB.Length = 0;
                        sqlB.AppendLine("SELECT ");
                        sqlB.AppendLine("   UserID ");
                        sqlB.AppendLine(" , UserName ");
                        sqlB.AppendLine(" , RoleID ");
                        sqlB.AppendLine("FROM users ");
                        sqlB.AppendLine("WHERE UserName = '" + UserName + "' ");
                        sqlB.AppendLine("AND EncryptedPassword = '" + Password + "' ");
                        sqlB.AppendLine("LIMIT 1");
                        sqlB.AppendLine(";");

                        string sql = sqlB.ToString().Replace("\r\n", "");

                        DataReader = mysql.getmysqlread(sql);

                        Dictionary<string, string> userInfo = new Dictionary<string, string>();//1
                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                userInfo.Add("RoleID", DataReader["RoleID"].ToString());
                                userInfo.Add("UserID", DataReader["UserID"].ToString());
                                userInfo.Add("UserName", DataReader["UserName"].ToString());
                                // showInMenuUserInfo.TryAdd("UserName", DataReader["UserName"].ToString());
                                // showInMenuUserInfo.TryAdd("");
                            }

                            ReturnStr = BC_Fmt.ConvertToJson(new SuccessReturnData(userInfo));
                            context.Session.SetString("UserInfo", BC_Fmt.ConvertToJson(userInfo));
                        }
                        else
                        {
                            ReturnStr = "0";
                            ReturnStr = BC_Fmt.ConvertToJson(new FailedReturnData(-1, "用户名或密码不正确"));
                        }
                        break;
                    }
                case "DemoRegister2":
                    {
                        string UserName3 = RequestParameter["UserName"];
                        string Password3 = RequestParameter["Password"];
                        string Email3 = RequestParameter["Email"];
                        string Phone3 = RequestParameter["Phone"];
                        string NickName3 = RequestParameter["NickName"];

                        sqlB.Length = 0;
                        sqlB.AppendLine("INSERT INTO users ( ");
                        sqlB.AppendLine("   UserName ");
                        sqlB.AppendLine(" , NickName ");
                        sqlB.AppendLine(" , Email ");
                        sqlB.AppendLine(" , Phone ");
                        sqlB.AppendLine(" , EncryptedPassword ");
                        sqlB.AppendLine(" , TimeUpdated ");
                        sqlB.AppendLine(" , TimeCreated  ");
                        sqlB.AppendLine(")  ");
                        sqlB.AppendLine("VALUES (  ");
                        sqlB.AppendLine("   '" + UserName3 + "'");
                        sqlB.AppendLine(" , '" + NickName3 + "'");
                        sqlB.AppendLine(" , '" + Email3 + "'");
                        sqlB.AppendLine(" , '" + Phone3 + "'");
                        sqlB.AppendLine(" , '" + Password3 + "'");
                        sqlB.AppendLine(" , NOW() ");
                        sqlB.AppendLine(" , NOW() ");
                        sqlB.AppendLine(")");
                        sqlB.AppendLine(";");

                        string sql3 = sqlB.ToString();
                        ReturnStr = mysql.getmysqlcom(sql3).ToString();
                        break;
                    }
                // case "DemoRegister":
                //     {
                //         string UserName1 = RequestParameter["UserName"];
                //         string Password1 = RequestParameter["Password"];
                //         string Phone = RequestParameter["Phone"];
                //         string Email = RequestParameter["Email"];
                //         string NickName = RequestParameter["NickName"];
                //         sqlB.Length = 0;
                //         sqlB.AppendLine("INSERT INTO users ( ");
                //         sqlB.AppendLine("   UserName ");
                //         sqlB.AppendLine(" , NickName ");
                //         sqlB.AppendLine(" , Email ");
                //         sqlB.AppendLine(" , Phone ");
                //         sqlB.AppendLine(" , EncryptedPassword ");
                //         sqlB.AppendLine(" , TimeUpdated ");
                //         sqlB.AppendLine(" , TimeCreated  ");
                //         sqlB.AppendLine(")  ");
                //         sqlB.AppendLine("VALUES (  ");
                //         sqlB.AppendLine("   '" + UserName1 + "'");
                //         sqlB.AppendLine(" , '" + NickName + "'");
                //         sqlB.AppendLine(" , '" + Email + "'");
                //         sqlB.AppendLine(" , '" + Phone + "'");
                //         sqlB.AppendLine(" , '" + Password1 + "'");
                //         sqlB.AppendLine(" , NOW() ");
                //         sqlB.AppendLine(" , NOW() ");
                //         sqlB.AppendLine(")");
                //         sqlB.AppendLine(";");
                //         string sql1 = sqlB.ToString().Replace("\r\n", "");
                //         ReturnStr = mysql.getmysqlcom(sql1).ToString();

                //         break;
                //     }
                case "GetTask":
                    {
                        sqlB.Length = 0;
                        sqlB.AppendLine("SELECT ");
                        sqlB.AppendLine("   TaskID ");
                        sqlB.AppendLine(" , Task ");
                        sqlB.AppendLine(" , AssignedToUserID ");
                        sqlB.AppendLine("FROM tasks ");

                        string sqlTaks = sqlB.ToString();
                        DataReader = mysql.getmysqlread(sqlTaks);

                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                Dictionary<string, string> taskDic = new Dictionary<string, string>();
                                taskDic.Add("TaskID", DataReader["TaskID"].ToString());
                                taskDic.Add("Task", DataReader["Task"].ToString());
                                taskDic.Add("AssignTo", DataReader["AssignedToUserID"].ToString());

                                list.Add(taskDic);
                            }
                        }

                        ReturnStr = BC_Fmt.ConvertToJson(list);
                        list.Clear();   // 一旦拿到了我们想要的数据后, 再将list清空
                        break;
                    }
                case "AddTask":
                    {
                        string Task = RequestParameter["Task"];
                        string UserID = RequestParameter["UserID"];
                        string ProjectID = RequestParameter["ProjectID"];
                        string TaskDetail = RequestParameter["TaskDetail"];
                        Dictionary<string,object> userInfo =BC_Fmt.JsonDeserialize<Dictionary<string,object>>(context.Session.GetString("UserInfo"));
                        string userID = userInfo["UserID"].ToString();
                        if(TaskDetail.Length !=0){

                        sqlB.Length = 0;
                        sqlB.AppendLine("INSERT INTO tasks ( ");
                        sqlB.AppendLine("   Task ");
                        sqlB.AppendLine(" , AssignedToUserID ");
                        sqlB.AppendLine(" , ProjectID ");
                        sqlB.AppendLine(" , TimeUpdated ");
                        sqlB.AppendLine(" , TimeCreated  ");
                        sqlB.AppendLine(")  ");
                        sqlB.AppendLine("VALUES (  ");
                        sqlB.AppendLine($"   '{Task}'");
                        sqlB.AppendLine($" , {UserID}");
                        sqlB.AppendLine($" , {ProjectID}");
                        sqlB.AppendLine(" , NOW() ");
                        sqlB.AppendLine(" , NOW() ");
                        sqlB.AppendLine(")");
                        sqlB.AppendLine(";");
                        sqlB.AppendLine("INSERT INTO");
                        sqlB.AppendLine("taskdetails(");
                        sqlB.AppendLine("   taskdetails.TaskID");
                        sqlB.AppendLine(" , taskdetails.TaskDetail");
                        sqlB.AppendLine(" , taskdetails.UpdatedBy");
                        sqlB.AppendLine(" , taskdetails.TimeUpdated");
                        sqlB.AppendLine(" , taskdetails.CreatedBy");
                        sqlB.AppendLine(" , taskdetails.TimeCreated");
                        sqlB.AppendLine(")");
                        sqlB.AppendLine("VALUES(");
                        sqlB.AppendLine(" @@Identity ");
                        sqlB.AppendLine($", '{TaskDetail}' ");
                        sqlB.AppendLine($", {userID} ");
                        sqlB.AppendLine(" , Now()");
                        sqlB.AppendLine($" , {userID} ");
                        sqlB.AppendLine(" , Now()");
                        sqlB.AppendLine(") ");
                        sqlB.AppendLine(";");
                        ReturnStr = mysql.getmysqlcom(sqlB.ToString()).ToString();
                        }
                        else{
                        sqlB.Length = 0;
                        sqlB.AppendLine("INSERT INTO tasks ( ");
                        sqlB.AppendLine("   Task ");
                        sqlB.AppendLine(" , AssignedToUserID ");
                        sqlB.AppendLine(" , ProjectID ");
                        sqlB.AppendLine(" , TimeUpdated ");
                        sqlB.AppendLine(" , TimeCreated  ");
                        sqlB.AppendLine(")  ");
                        sqlB.AppendLine("VALUES (  ");
                        sqlB.AppendLine($"   '{Task}'");
                        sqlB.AppendLine($" , {UserID}");
                        sqlB.AppendLine($" , {ProjectID}");
                        sqlB.AppendLine(" , NOW() ");
                        sqlB.AppendLine(" , NOW() ");
                        sqlB.AppendLine(")");
                        sqlB.AppendLine(";");
                        string sql2 = sqlB.ToString().Replace("\r\n", "");
                        ReturnStr = mysql.getmysqlcom(sql2).ToString();
                        }
                        break;
                    }
                case "GetMessageList":
                    {
                        sqlB.Length = 0;
                        sqlB.AppendLine("SELECT ");
                        sqlB.AppendLine("   users.UserID ");
                        sqlB.AppendLine(" , users.UserName ");
                        sqlB.AppendLine(" , messages.MessageID ");
                        sqlB.AppendLine(" , messages.Message ");
                        sqlB.AppendLine(" , messages.TimeUpdated ");
                        sqlB.AppendLine("FROM messages ");
                        sqlB.AppendLine("INNER JOIN users ON  messages.UserID = users.UserID ");
                        sqlB.AppendLine("ORDER BY messages.TimeUpdated DESC ");
                        sqlB.AppendLine("LIMIT 20");
                        sqlB.AppendLine(";");

                        DataReader = mysql.getmysqlread(sqlB.ToString());
                        StringBuilder JsonB = new StringBuilder();
                        JsonB.Length = 0;

                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                if (JsonB.Length > 0)
                                {
                                    JsonB.AppendLine(",");
                                }

                                JsonB.AppendLine("{");
                                JsonB.AppendLine("\"Userid\":" + DataReader["Userid"].ToString());
                                JsonB.AppendLine(",\"UserName\":\"" + DataReader["UserName"].ToString() + "\"");
                                JsonB.AppendLine(",\"MessageID\":" + DataReader["MessageID"].ToString());
                                JsonB.AppendLine(",\"Message\":\"" + DataReader["Message"].ToString() + "\"");
                                JsonB.AppendLine(",\"TimeUpdated\":\"" + DataReader["TimeUpdated"].ToString() + "\"");
                                JsonB.AppendLine("}");
                            }
                        }

                        ReturnStr += "{\"MessageList\":[";
                        ReturnStr += JsonB.ToString();
                        ReturnStr += "]}";

                        break;
                    }
                case "Task":
                    {
                        sqlB.Length = 0;
                        sqlB.AppendLine("SELECT ");
                        sqlB.AppendLine("   tasks.TaskID");
                        sqlB.AppendLine(" , projects.Project");
                        sqlB.AppendLine(" , tasks.Task");
                        sqlB.AppendLine(" , users.UserName AS AssignTo");
                        sqlB.AppendLine(" , tasks.TimeUpdated");
                        sqlB.AppendLine(" , UpdateUser.UserName AS UpdatedBy");
                        sqlB.AppendLine("FROM tasks ");
                        sqlB.AppendLine("INNER JOIN projects ON projects.ProjectID = tasks.ProjectID ");
                        sqlB.AppendLine("INNER JOIN users ON users.UserID = tasks.AssignedToUserID ");
                        sqlB.AppendLine("INNER JOIN users AS UpdateUser ON UpdateUser.UserID = tasks.UpdatedBy ");
                        sqlB.AppendLine("LIMIT 30");
                        sqlB.AppendLine(";");

                        DataReader = mysql.getmysqlread(sqlB.ToString());

                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                Dictionary<string, string> taskDic = new Dictionary<string, string>();
                                taskDic.Add("TaskID", DataReader["TaskID"].ToString());
                                taskDic.Add("Project", DataReader["Project"].ToString());
                                taskDic.Add("Task", DataReader["Task"].ToString());
                                taskDic.Add("AssignTo", DataReader["AssignTo"].ToString());
                                taskDic.Add("TimeUpdated", DataReader["TimeUpdated"].ToString());
                                taskDic.Add("UpdatedBy", DataReader["UpdatedBy"].ToString());

                                list.Add(taskDic);
                            }
                        }
                        ReturnStr = BC_Fmt.ConvertToJson(list);
                        list.Clear();
                        break;
                    }
                case "deleteTask":
                    {
                        Dictionary<string, object> userInfo = BC_Fmt.JsonDeserialize<Dictionary<string, object>>(context.Session.GetString("UserInfo"));
                        string RoleID = (string)userInfo["RoleID"];
                        if (RoleID == null || (RoleID != null && RoleID != "1"))
                        {
                            ReturnStr = BC_Fmt.ConvertToJson("当前用户没有管理权限，请联系管理员");

                        }
                        else
                        {
                            string taskID = RequestParameter["TaskID"];
                            sqlB.AppendLine("DELETE FROM tasks ");
                            sqlB.AppendLine("   WHERE");
                            sqlB.AppendLine("  tasks.TaskID = " + taskID + "");
                            sqlB.AppendLine(";");
                            Dictionary<string, object> returnData = new Dictionary<string, object>();
                            int effectRecord = mysql.getmysqlcom(sqlB.ToString());
                            returnData.Add("retCode", 1);

                            returnData.Add("retMsg", "删除成功");

                            returnData.Add("data", "Y");

                            ReturnStr = BC_Fmt.ConvertToJson(returnData);
                        }
                        break;
                    }
                case "QueryTask":
                    {
                        Dictionary<string, int> pagingInfo = getPagingParam(RequestParameter);
                        string taskID = RequestParameter["TaskID"];
                        string task = RequestParameter["Task"];
                        string assignTo = RequestParameter["AssignedToUserID"];
                        //条件相关
                        StringBuilder wherePart = new StringBuilder();
                        wherePart.AppendLine(" WHERE 1 = 1 ");
                        if (taskID.Length != 0)
                        {
                            wherePart.AppendLine(" AND tasks.TaskID = " + int.Parse(taskID) + " ");
                        }
                        if (task.Length != 0)
                        {
                            wherePart.AppendLine(" AND tasks.task like '%" + task + "%' ");
                        }
                        if (assignTo.Length != 0)
                        {
                            wherePart.AppendLine(" AND users.UserName like '%" + assignTo + "%'");
                        }
                        //分页相关
                        sqlB.Length = 0;
                        sqlB.AppendLine("SELECT ");
                        sqlB.AppendLine("   tasks.TaskID");
                        sqlB.AppendLine(" , projects.Project");
                        sqlB.AppendLine(" , tasks.Task");
                        sqlB.AppendLine(" , users.UserName AS AssignTo");
                        sqlB.AppendLine(" , tasks.TimeUpdated");
                        sqlB.AppendLine(" , UpdateUser.UserName AS UpdatedBy ");
                        sqlB.AppendLine("FROM tasks ");
                        sqlB.AppendLine("INNER JOIN projects ON projects.ProjectID = tasks.ProjectID ");
                        sqlB.AppendLine("INNER JOIN users ON users.UserID = tasks.AssignedToUserID ");
                        sqlB.AppendLine("INNER JOIN users AS UpdateUser ON UpdateUser.UserID = tasks.UpdatedBy ");
                        sqlB.AppendLine(wherePart.ToString());
                        sqlB.AppendLine(getPagingSql(pagingInfo));
                        sqlB.Append(";");
                        DataReader = mysql.getmysqlread(sqlB.ToString());

                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                Dictionary<string, string> taskDic = new Dictionary<string, string>();
                                taskDic.Add("TaskID", DataReader["TaskID"].ToString());
                                taskDic.Add("Project", DataReader["Project"].ToString());
                                taskDic.Add("Task", DataReader["Task"].ToString());
                                taskDic.Add("AssignTo", DataReader["AssignTo"].ToString());
                                taskDic.Add("TimeUpdated", DataReader["TimeUpdated"].ToString());
                                taskDic.Add("UpdatedBy", DataReader["UpdatedBy"].ToString());

                                list.Add(taskDic);
                            }
                        }
                        //分页
                        sqlB.Length = 0;
                        sqlB.AppendLine("SElECT ");
                        sqlB.AppendLine(" count(1) recordNum ");
                        sqlB.AppendLine("FROM tasks ");
                        sqlB.AppendLine("INNER JOIN projects ON projects.ProjectID = tasks.ProjectID ");
                        sqlB.AppendLine("INNER JOIN users ON users.UserID = tasks.AssignedToUserID ");
                        sqlB.AppendLine("INNER JOIN users AS UpdateUser ON UpdateUser.UserID = tasks.UpdatedBy ");
                        sqlB.AppendLine(wherePart.ToString());
                        DataReader = mysql.getmysqlread(sqlB.ToString());
                        DataReader.Read();
                        int recordNum = int.Parse(DataReader["recordNum"].ToString());
                        Dictionary<string, object> returnData = new Dictionary<string, object>();
                        returnData.Add("allCount", recordNum);
                        returnData.Add("pageNum", pagingInfo["pageNum"]);
                        returnData.Add("pageSize", pagingInfo["pageSize"]);
                        returnData.Add("dataList", list);
                        ReturnStr = BC_Fmt.ConvertToJson(returnData);
                        list.Clear();
                        break;
                    }
                case "GetUsersToAssign":
                    {
                        sqlB.Length = 0;
                        sqlB.AppendLine("SELECT ");
                        sqlB.AppendLine("   UserID ");
                        sqlB.AppendLine(" , UserName ");
                        sqlB.AppendLine("FROM users ");
                        sqlB.AppendLine("LIMIT 10 ");
                        sqlB.AppendLine("; ");

                        DataReader = mysql.getmysqlread(sqlB.ToString());
                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                Dictionary<string, string> userDic = new Dictionary<string, string>();
                                userDic.Add("UserID", DataReader["UserID"].ToString());
                                userDic.Add("UserName", DataReader["UserName"].ToString());

                                list.Add(userDic);
                            }
                        }
                        ReturnStr = BC_Fmt.ConvertToJson(list);
                        list.Clear();
                        break;
                    }
                default:
                    {
                        ReturnStr = "";
                        break;
                    }
            }
            await context.Response.WriteAsync(ReturnStr);
        }
        private static Dictionary<string, int> getPagingParam(IDictionary<string, string> param)
        {
            Dictionary<string, int> pageInfo = new Dictionary<string, int>();
            pageInfo.Add("pageNum", param.ContainsKey("pageNum") ? int.Parse(param["pageNum"]) : 0);
            pageInfo.Add("pageSize", param.ContainsKey("pageSize") ? int.Parse(param["pageSize"]) : 10);
            return pageInfo;
        }
        private static string getPagingSql(Dictionary<string, int> pageInfo)
        {
            return "  Limit " + pageInfo["pageNum"] * pageInfo["pageSize"] + "," + pageInfo["pageSize"];
        }
    }
}