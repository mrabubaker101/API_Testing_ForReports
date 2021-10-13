 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System; 

namespace DEMO.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/report")]
    public class ReportController : ControllerBase
    {
        IConfiguration _con;
        ReportManager mgr;
        public ReportController(IConfiguration con)
        {
            _con = con;
            mgr = new ReportManager(_con);
        }


        #region TEST
        [AllowAnonymous]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(contentType: "application/pdf")]
        [HttpGet]
        [Route("test")]
        public IActionResult Reports(string name )
        {
            try
            { 
                string repname = "CaseListing";
                MyReportModel param = new MyReportModel();
                param.repname = repname;
                string path = _con.GetSection("RDLC").GetSection("path").Value;
                byte[] result = mgr.Generate_RDLC(param, path);
                if (result != null)
                {
                    return File(result, "application/pdf");
                }
                return NotFound();

            }
            catch (Exception e)
            { 
                return BadRequest();
            }
        }
        #endregion
    }
}
