using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Analytics.v3;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string keyFilePath = "C:\\SSH\\GAGetWebsiteStats-02463188aff3.p12";
            string serviceAccountEmail = "getstatsuser@gagetwebsitestats.iam.gserviceaccount.com";
            string keyPassword = "notasecret";
            string websiteCode = "149032205";
            AnalyticsService service = null;

            X509Certificate2 certificate = new X509Certificate2(keyFilePath, keyPassword, X509KeyStorageFlags.Exportable);


            string[] scopes =
                            new string[] {
                     AnalyticsService.Scope.Analytics,              // view and manage your analytics data    
                     AnalyticsService.Scope.AnalyticsEdit,          // edit management actives    
                     AnalyticsService.Scope.AnalyticsManageUsers,   // manage users    
                     AnalyticsService.Scope.AnalyticsReadonly};


            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes
            }.FromCertificate(certificate));

            service = new AnalyticsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });


            DataResource.GaResource.GetRequest request = service.Data.Ga.Get(
               "ga:" + websiteCode,
               DateTime.Today.AddDays(-15).ToString("yyyy-MM-dd"),
               DateTime.Today.ToString("yyyy-MM-dd"),
               "ga:sessions");
            request.Dimensions = "ga:year,ga:month,ga:day";
            var data = request.Execute();

            List<ChartRecord> visitsData = new List<ChartRecord>();



            foreach (var row in data.Rows)
            {
                //MessageBox.Show(row[0].ToString());
                visitsData.Add(new ChartRecord(new DateTime(int.Parse(row[0]), int.Parse(row[1]), int.Parse(row[2])).ToString("MM-dd-yyyy"), int.Parse(row[3])));
            }

            chart1.Series[0].XValueMember = "Date";
            chart1.Series[0].YValueMembers = "Visits";
            chart1.DataSource = visitsData;
            chart1.DataBind();
        }
    }
}
