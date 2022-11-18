using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Gbookv2
{
    /// <summary>
    /// Library of helper functions and useful shortcuts for common operations throughout C# and Encompass
    /// </summary>
    public class BC_Fmt
    {
        /// <summary>
        /// *1. 前端以post方式传过来的json字符串,将其转为Dictionary类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async ValueTask<IDictionary<string, string>> GetRequestParameter(HttpRequest request)
        {
            IDictionary<string, string> RequestParameter = new Dictionary<string, string>();
            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                string body = await reader.ReadToEndAsync();
                request.Body.Position = 0;//以后可以重复读取
                if (body != "" && body.IndexOf("form-data") == -1)  // 可以同时接受FormData和PostJson请求
                {
                    RequestParameter = JsonConvert.DeserializeObject<IDictionary<string, string>>(body);
                }
            }
            return RequestParameter;
        }
		
		/// <summary>
        /// *2. Serialize a C# object to a Json string (将C#对象转为Json字符串)
        /// </summary>
        /// <example>
        /// <code>
        /// class TestObj {
        ///    string name;
        ///    int value;
        /// }
        /// TestObj myObj = new TestObj();
        /// myObj.name = "John";
        /// myObj.value = 1;
        /// BC_Fmt.ConvertToJson(myObj);
        /// // "{\"name\":\"John\",\"value\":1}"
        /// </code>
        /// </example>
        public static string ConvertToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
		
        /// <summary>
        /// Deserialize the Json string into a C# object (将Json字符串转为C#对象,但需要创建对应的类或结构)
        /// </summary>
        /// <example>
        /// <code>
        /// class TestObj {
        ///    string name;
        ///    int value;
        /// }
        /// string json = "{\"name\":\"John\",\"value\":1}";
        /// TestObj myObj = BC_Fmt.JsonDeserialize&ltTestObj>(json);
        /// // myObj.name = "John"
        /// // myObj.value = 1
        /// </code>
        /// </example>
        public static T JsonDeserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
       

        public static string JsonDocumentToString(System.Text.Json.JsonDocument jDoc)
        {
            string results;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                System.Text.Json.Utf8JsonWriter writer = new System.Text.Json.Utf8JsonWriter(stream, new System.Text.Json.JsonWriterOptions { SkipValidation = true });
                jDoc.WriteTo(writer);
                writer.Flush();
                results = Encoding.UTF8.GetString(stream.ToArray());
            }
            return results;
        }

    }
}
