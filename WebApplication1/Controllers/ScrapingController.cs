using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System.Data.SqlClient;
using WebApplication1.Models;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
       public ScrapingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
     
        [HttpGet]
        public ActionResult<IEnumerable<Article>> Get()
        {

            var web1 = new HtmlWeb();
            var doc2 = web1.Load("https://www.crtta.ma/%d8%a7%d8%ae%d8%a8%d8%a7%d8%b1-%d8%a7%d9%84%d8%ac%d9%87%d8%a9/");
            var tex = doc2.DocumentNode.SelectNodes("//div[@class='item']");
            var articls = new List<Article>();
            foreach (var item in tex)
            {
                var articl = new Article
                {
                    link = item.Descendants("h4").FirstOrDefault().FirstChild.ChildAttributes("href").FirstOrDefault().Value,
                    image = item.Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value,
                    titel = item.Descendants("h4").FirstOrDefault().InnerText,
                    text = item.Descendants("p").FirstOrDefault().InnerText,
                    Date = item.Descendants("span").FirstOrDefault().FirstChild.InnerText
                };
                SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("connection"));
                cn.Open();

                string Q = "INSERT INTO Article VALUES('" + articl.link + "','" + articl.image + "','" + articl.titel + "','" + articl.text + "','" + articl.Date + "')";

                SqlCommand cmd = new SqlCommand(Q, cn);
                cmd.ExecuteNonQuery();
                cn.Close();

                articls.Add(articl);
                
            }
          
            return articls;
            


        } 
        }
}