using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string vClientIP = GetIPAddress();//取得ClientIP 
            Response.Write(vClientIP);

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["grapipConnectionString"].ToString());
            SqlCommand insert = new SqlCommand("INSERT INTO [grapip].[dbo].[ipgrap] ([ip]) VALUES (@vClientIP)", conn);
            //string cmdst = "INSERT INTO [grapip].[dbo].[ipgrap] ([ip]) VALUES (@vClientIP)"; //T_SQL 插入語法
            insert.Parameters.AddWithValue("@vClientIP", vClientIP);            
            conn.Open();
            insert.ExecuteNonQuery();    //重點在這行!! 針對連接執行 Transact-SQL 陳述式
            conn.Close();    //同上 , 有開就有關 !!

            string filePath  = "http://localhost:80/";
            //Response.Redirect(filePath);

            string filename = "123.jpg";
            //從Server端取得檔案 
            Stream FileStream;
            FileStream = File.OpenRead(Server.MapPath("~/123.jpg"));
            downloadBook(filename, FileStream);

        }
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        protected void downloadBook(string filename, Stream FileStream)
        {
            Byte[] Buf = new byte[FileStream.Length];
            FileStream.Read(Buf, 0, int.Parse(FileStream.Length.ToString()));
            FileStream.Close();

            //準備下載檔案 
            Response.ClearHeaders();
            Response.Clear();
            Response.Expires = 0;
            Response.Buffer = false;
            Response.ContentType = "Application/save-as";
            Response.Charset = "utf-8";
            //透過Header設定檔名 
            Response.AddHeader("Content-Disposition", "Attachment; filename=" + HttpUtility.UrlEncode(filename));
            Response.BinaryWrite(Buf);
            Response.End();
        }
    }
   
}