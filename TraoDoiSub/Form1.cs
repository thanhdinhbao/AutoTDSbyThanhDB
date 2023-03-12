using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;
using System.Windows.Forms;

namespace TraoDoiSub
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            ComboBox.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        #region Class
        public class Root
        {
            public string id { get; set; }
            public string error { get; set; }
            public double countdown { get; set; }
        }
        public class Data
        {
            public int xu { get; set; }
            public string id { get; set; }
            public string msg { get; set; }
            public string error { get; set; }
        }
        public class GetCoinClass
        {
            public int success { get; set; }
            public Data data { get; set; }
        }
        #endregion

        public string TypeMisson()
        {
            string type = "";
            switch (cbType.SelectedIndex)
            {
                case 0:
                    type = "follow";
                    break;
                case 1:
                    type = "like";
                    break;
                case 2:
                    type = "reaction";
                    break;
                case 3:
                    type = "comment";
                    break;
                case 4:
                    type = "share";
                    break;
                case 5:
                    type = "reactcmt";
                    break;
                case 6:
                    type = "group";
                    break;
                case 7:
                    type = "page";
                    break;
            }
            return type;
        }

        public async void GetMisson()
        {
            string type = TypeMisson();
            var options = new RestClientOptions()
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://traodoisub.com/api/?fields=" + type + "&access_token=TDSQfiYjclZXZzJiOiIXZ2V2ciwiIvFmYo5WakhmbhhGdiojIyV2c1Jye", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            var json = response.Content;
            try
            {
                Root[] parsed_json = JsonConvert.DeserializeObject<Root[]>(json);

                try
                {
                    for (int i = 0; i < 9; i++)
                    {
                        string id = parsed_json[i].id;
                        Selenium(id);
                        Thread.Sleep(2000);
                        GetCoin(id);
                    }
                }
                catch (Exception)
                {

                }
            }
            catch (Exception)
            {
                Root parsed_json = JsonConvert.DeserializeObject<Root>(json);
                string msg = parsed_json.error;
                double time = parsed_json.countdown;
                MessageBox.Show(msg + Environment.NewLine + "Thử lại sau:"+ time);
            }

        }

        void Selenium(string id)
        {
            string type = TypeMisson();

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            string path_profile = "user-data-dir=" + Environment.CurrentDirectory + @"\Profile";
            options.AddArguments(path_profile);
            options.AddArgument("--disable-web-security");
            options.AddExcludedArgument("enable-automation");
            options.AddArguments("--disable-notifications");
            options.AddArgument("start-maximized");
            var driver = new ChromeDriver(driverService, options);

            driver.Url = "https://www.facebook.com/" + id;
            driver.Navigate();

            if (type == "like")
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(15000));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[aria-label='Thích']")));
                element.Click();
            }
            else if (type == "follow")
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(15000));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[aria-label='Theo dõi']")));
                element.Click();
            }

            Thread.Sleep(1500);

            driver.Close();
            driver.Quit();
            driver.Dispose();

        }

        public async void GetCoin(string id)
        {
            try
            {
                string type = TypeMisson();
                var options = new RestClientOptions("https://traodoisub.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/coin/?type=" + type.ToUpper() + "&id=" + id + "&access_token=TDSQfiYjclZXZzJiOiIXZ2V2ciwiIvFmYo5WakhmbhhGdiojIyV2c1Jye", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);
                var json = response.Content;
                GetCoinClass parsed_json = JsonConvert.DeserializeObject<GetCoinClass>(json);
                if (parsed_json.success == 200)
                {
                    int xu = parsed_json.data.xu;
                    string msg = parsed_json.data.msg + " | Tổng số: " + xu + Environment.NewLine;
                    UpdateStatus(msg);
                }
                else
                {
                    UpdateStatus("Lỗi, thử lại!");
                }
            }
            catch(Exception)
            {
                
            }
            
            
            


        }

        void UpdateStatus(string msg)
        {
            Thread t1 = new Thread(() => {
                txtStatus.Text += msg;
            });
            t1.Start();
            
        }

        void Test()
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-web-security");
            options.AddExcludedArgument("enable-automation");
            options.AddArguments("--disable-notifications");
            options.AddArgument("start-maximized");
            var driver = new ChromeDriver(driverService, options);
            driver.Url = "https://www.tiktok.com/@matatogames/video/7181260941680545029";
            driver.Navigate();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(15000));
            var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div[2]/div[1]/div[1]/div[1]/div[4]/div/button[1]")));
            element.Click();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Thread t = new Thread(() => {
            //    GetMisson();
            //});
            //t.Start();
            Test();

        }
    }

}

