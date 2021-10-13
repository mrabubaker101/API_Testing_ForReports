using AspNetCore.Reporting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DEMO.Controllers
{
    public class ReportManager
    {
        IConfiguration _con;
        public ReportManager(IConfiguration con )
        {
            _con = con;
        }
        public byte[] Generate_RDLC(MyReportModel model, string path)
        { 
            try
            { 
                model.RepDataSet = CaseListing();
                string mimtype = "";
                int extension = 1;
                string RptLocation = path;
                string _reportPath = $"{RptLocation + model.repname}.rdlc";
                LocalReport localReport = new LocalReport(_reportPath);
                DataTable dta = model.RepDataSet.Tables[0];
                model.RepDataSet.DataSetName = "DataSet1";
                dta.TableName = "DataSet1";
                localReport.AddDataSource(model.RepDataSet.DataSetName, dta);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                byte[] bytes = null;
                var result = localReport.Execute(RenderType.Pdf, extension, parameters: model.repParams, mimtype);
                bytes = result.MainStream;
                var dss = new System.Net.Http.ByteArrayContent(bytes);
                return bytes;
            }
            catch (Exception ex)
            {
                string r = ex.InnerException.InnerException.ToString();
                return null;
            }
        }

        public DataSet CaseListing()
        {
            DataSet Set; new DataSet();
            DataTable dt = new DataTable(); 
            string connectionString = _con.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                //string dd = "Data Source=51.210.154.102,11950;Initial Catalog=TWCDB;User=twc;Password=Stretto!2021;";
                string Query = $@"SELECT 'L-1222' as file_no,DIST_LEDGBAL as ledger_bal, 3 as Acounter,1 as NCounter,2 as PCounter,
		c.trustee_no + IIF('llc_Sort' = '', 
		'', c.case_No) AS c_Order, 
	t.[name], c.trustee_no, c.case_no, c.case_name, c.file_no, c.close_reason, 
 	c.date_opened AS d_petition, 
	c.judge, c.fee_paid, c.Asset, c.fee_amount, c.fee_date, c.d_RND,
	IIF(Charindex('F', c.close_reason) > 0, '', c.date_closed) AS d_dismiss, 
  	IIF(Charindex('F', c.close_reason) > 0, c.date_closed, '') AS d_fin_decr,	
  	IIF(Year(c.date_cont) = 1900, IIF(Year(c.Date_341) = 1900, '          ', Cast(c.date_341 as varchar(max)) + '  '), Cast(c.date_cont as varchar(max)) + ' C') AS x341_date,
  	IIF(Year(c.date_cont) = 1900, c.time_341, c.time_cont) AS x341_time, c.Attention,  
	c.UST_Memo 
  	FROM [Case] c, Trustee t
 		WHERE  
		t.trustee_no = c.trustee_no AND  
				IIF(c.Asset = 'A' OR  
				1=0, 1, 
			 		iif(Year(c.d_RND) = 1900 AND (Dateadd(Day, 20, c.d_RND) <= GETDATE()), 1 ,0)) = 1 
 			ORDER BY c_Order";
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connectionString);
                Set = new DataSet();
                adapter.Fill(Set);
            }
            catch (SqlException e)
            {
                Set = null;
            }
            finally
            {
                con.Close();
            }
            return Set;
        }

        public DataTable SqlView(string queryString )
        {
            string connectionString = "Data Source=DESKTOP-RL01AD4\\SQLEXPRESS;Initial Catalog=TWCDB;Integrated Security=True;";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(queryString, connectionString);
                adapter.Fill(ds);
                dt = ds.Tables[0];
            }
            catch (SqlException e)
            {

                dt = null;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

    }
    
    public class MyReportModel
    {
        public int repId { get; set; }
        public string repname { get; set; }
        public DataSet RepDataSet { get; set; }
        public string repformat { get; set; }
        public Dictionary<string, string> repParams { get; set; }

    }
}
