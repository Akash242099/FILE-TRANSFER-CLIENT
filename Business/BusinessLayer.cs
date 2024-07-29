using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataProvider;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Business
{
    //Business.BusinessLayer.CompanyList
   public class BusinessLayer
    {
        public List<Jobs> JobList(string SourceIP, string DestinationIP)
        {            
            return new DataProviders().GetJobs(SourceIP,DestinationIP);
        }

        public bool isAdmin(string email)
        {
            return new DataProviders().isAdmin(email);
        }

        public bool login(string email, string password)   
        {
            
            return new DataProviders().login_dp(email, password);
        }

        public string AdminSubmit(Jobs j)
        {
            var myContent = JsonConvert.SerializeObject(j);
            HttpClient client = new HttpClient();
            DateTime dt = DateTime.Now;
            client.BaseAddress = new Uri("http://10.5.5.12:3100/");
            // add an accept header for json format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // list all names.
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);


            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage result = client.PostAsync("", byteContent).Result;
            try
            {
                result.EnsureSuccessStatusCode();

            }
            catch(Exception e)
            {
                return e.Message;
            }
            string responseBody = result.Content.ReadAsStringAsync().Result;
            string s = JObject.Parse(responseBody)["copied"].ToString();
            j.StartTimestamp = dt;
            j.EndTimestamp = DateTime.Now;
            j.Status = s;

            DataProviders dp = new DataProviders();
            dp.insertJob(j);
            return s;

        }

        public int  GetUserId(string Email)
        {
            return new DataProviders().GetUserId(Email);
        }

        public bool Decrypt(string emailId, string password)
        {
            DataProviders d = new DataProviders();

            return d.Decrypt(emailId, password);
        }

        public void ChangePassword(string Email,string Password)
        {
            DataProviders d = new DataProviders();
            
          d.ChangePassword(Email, Password);
        }

        public bool IsValidEmail(string Email)
        {
            DataProviders d = new DataProviders();
            return d.IsValidEmail(Email);
        }


    }
}
