using Dapper;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Configuration;

namespace DataProvider{
    public class DataProviders
    {
        public bool isAdmin( string Email)
        {            
            SqlConnection con = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");
            con.Open();
            DynamicParameters param = new DynamicParameters();
            //       var isAdmn = con.Query<bool>(@"Select isAdmin  from userTable  where Email=@Email", new { Email = Email }).FirstOrDefault();
            param.Add("@Email", Email);
            param.Add("@IsAdmin", dbType: DbType.Boolean, direction: ParameterDirection.Output, size: 5215585);
            con.Execute("CheckAdmins", param, commandType: CommandType.StoredProcedure);
            var IsAdmin = param.Get<bool>("@IsAdmin");
            con.Close();
            return IsAdmin;
        }

        public bool login_dp(string Email, string Password)
        {            
            SqlConnection con = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");
            DynamicParameters param = new DynamicParameters();
            param.Add("@Email", Email);
            param.Add("@Password", Password);
            param.Add("@Id", dbType: DbType.Int64, direction: ParameterDirection.Output, size: 5215585);
            con.Open();
            var k=con.Execute("PersonValid", param, commandType: CommandType.StoredProcedure);
            var Id = param.Get<Int64>("@Id");
            
            try
            {
                if (Id!=0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                return false; ;
            }
            finally
            {
                con.Close();
            }
        }
        public bool Decrypt(string EmailId, string password)
        {
            SqlConnection con = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");
          
            con.Open();
            while (password.Length % 4 != 0) password += '=';
            DynamicParameters param = new DynamicParameters();
            param.Add("@Email", EmailId);
                param.Add("@Password",dbType:DbType.String, direction: ParameterDirection.Output, size: 5215585);
            param.Add("@Salt", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
            con.Execute("Decrypt", param, commandType: CommandType.StoredProcedure);
            var pass = param.Get<string>("@Password");
            var salt = param.Get<string>("@Salt");
            var p=pass.Length;
            var q = salt.Length;
            password = password + salt;
            var  IsValid = Crypto.VerifyHashedPassword(pass, password);
            con.Close();
            return IsValid;
        }
         
        public bool IsValidEmail(string EmailId)
        {
            SqlConnection con = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");

            con.Open();
            DynamicParameters param = new DynamicParameters();
            param.Add("@Email", EmailId);
            param.Add("@RowCount", dbType: DbType.Boolean, direction: ParameterDirection.Output, size: 5215585);
            con.Execute("IsValidEmail", param, commandType: CommandType.StoredProcedure);

            var IdValidEmail = param.Get<bool>("@RowCount");

            con.Close();
            return IdValidEmail;
        }
            
        

        public  void ChangePassword(string Email, string Password)
        {

            SqlConnection con = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");

            string salt = Crypto.GenerateSalt(); // salt key
            while (Password.Length % 4 != 0) Password += '=';
            Password = Password + salt;
            string hashedpassword = Crypto.HashPassword(Password);
            var k = hashedpassword.Length;
            DynamicParameters param = new DynamicParameters();
            param.Add("@Email", Email);
            param.Add("@Password", hashedpassword);
            param.Add("@Salt", salt);
            

            con.Open();
            con.Execute("ChangePasswords", param, commandType: CommandType.StoredProcedure);
            con.Close();

            }


       public int  GetUserId(string Email)
        {
            SqlConnection con = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");
            con.Open();
            DynamicParameters param = new DynamicParameters();

            param.Add("@Email", Email);
            param.Add("@Id", dbType: DbType.Int64, direction: ParameterDirection.Output, size: 5215585);
            con.Execute("GetUerId", param, commandType: CommandType.StoredProcedure);

            var UserId =int.Parse( param.Get<Int64>("@Id").ToString());
            con.Close();
            return UserId;
         
        }


        public void insertJob(Jobs j)
        {
            SqlConnection conString = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");
            conString.Open();
            DateTime CurrentTime = DateTime.Now;
            var procedure = "[InsertJob]";
            var values = new
            {
                StartTime = j.StartTimestamp,
                EndTime = j.EndTimestamp,
                SourceIp = j.SourceIp,
                DestinationIp = j.DestinationIp,
                SourcePath = j.SourcePath,
                DestinationPath = j.DestPath,
                Comment = j.Comment,
                CreatedBy = j.CreatedBy,
                Status = j.Status
            };
            var results = conString.Execute(procedure, values, commandType: CommandType.StoredProcedure);
            conString.Close();
        }

        public List<Jobs> GetJobs(string SourceIP, string DestinationIP)
        {
            SqlConnection conString = new SqlConnection("Data Source=sql-dev.mapline.net,55355;Initial Catalog=Training;Integrated Security=True");
            conString.Open();
            Console.WriteLine(SourceIP);
            Console.WriteLine(DestinationIP);
            SqlCommand cmdQuery = new SqlCommand(String.Format("Select Top 20 * from job j where j.SourceIp='{0}' and j.DestinationIp='{1}' order by EndTime desc", SourceIP, DestinationIP), conString);


            SqlDataAdapter sda = new SqlDataAdapter(cmdQuery);
            DataSet dsData = new DataSet();
            sda.Fill(dsData);
            conString.Close();
            List<Jobs> job_lis = new List<Jobs>();
            foreach (DataRow dr in dsData.Tables[0].Rows)
            {
                Console.WriteLine(dr);
                job_lis.Add(new Jobs()
                {
                    Id = int.Parse(dr[0].ToString()),
                    StartTimestamp = DateTime.Parse(dr[1].ToString()),
                    EndTimestamp = DateTime.Parse(dr[2].ToString()),
                    SourceIp = dr[3].ToString(),
                    DestinationIp = dr[4].ToString(),
                    SourcePath = dr[5].ToString(),
                    DestPath = dr[6].ToString(),
                    CreatedBy = int.Parse(dr[8].ToString()), 
                    Status =dr[9].ToString(),
                    Comment=dr[7].ToString()                   
                });
            }
            return job_lis;
        }
    }
}

    

