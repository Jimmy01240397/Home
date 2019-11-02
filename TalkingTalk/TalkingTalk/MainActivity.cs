using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Android.Speech;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Webkit;

namespace TalkingTalk
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        protected const int RESULT_SPEECH = 1;

        private Button button1;
        private TextView textView1;
        private TextView textView2;
        private WebView WebView1;
        private RelativeLayout RelativeLayout1;

        private Timer timer;

        private bool _warning = false;
        private bool InLinking = false;
        private bool colorchange = false;
        private string ip = "http://192.168.43.196";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textView1 = FindViewById<TextView>(Resource.Id.textView1);
            textView2 = FindViewById<TextView>(Resource.Id.textView2);
            button1 = FindViewById<Button>(Resource.Id.button1);
            RelativeLayout1 = FindViewById<RelativeLayout>(Resource.Id.RelativeLayout1);
            button1.Click += button1_Click;

            Thread thread = new Thread(new ThreadStart(Warning));
            thread.Start();

            TimerCallback timerCallback = new TimerCallback(Warning);
            timer = new Timer(timerCallback, null, 0, 1000);
        }

        private void Warning()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    WebClient MyWebClient = new WebClient();
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                    Byte[] pageData = MyWebClient.DownloadData(ip + "/warning.txt");
                    _warning = Convert.ToBoolean(Convert.ToInt32(Encoding.UTF8.GetString(pageData)));
                }
                catch (Exception)
                {

                }
            }
        }

        private void Warning(object state)
        {
            this.RunOnUiThread(() =>
            {
                try
                {
                    if (_warning)
                    {
                        textView2.Text = "有人闖入";
                        if (!colorchange)
                        {
                            RelativeLayout1.SetBackgroundColor(Android.Graphics.Color.Red);
                            colorchange = true;
                        }
                        else
                        {
                            RelativeLayout1.SetBackgroundColor(Android.Graphics.Color.White);
                            colorchange = false;
                        }
                    }
                    else
                    {
                        textView2.Text = "";
                        RelativeLayout1.SetBackgroundColor(Android.Graphics.Color.White);
                        colorchange = false;
                    }
                }
                catch (Exception e)
                {
                    textView2.Text = e.Message;
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);

            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, "zh-tw");

            try
            {
                StartActivityForResult(intent, RESULT_SPEECH);
                textView1.Text = "";
            }
            catch (ActivityNotFoundException a)
            {
                Toast t = Toast.MakeText(this, "哎呀！ 您的設備不支援語音辨識！", ToastLength.Short);
                t.Show();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            switch (requestCode)
            {
                case RESULT_SPEECH:
                    {
                        if (resultVal == Result.Ok && null != data)
                        {

                            IList<string> text = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);

                            textView1.Text = text[0];
                            if(text[0] == "操控模式")
                            {
                                InLinking = true;
                                textView1.Text = "已啟動操控模式";
                            }
                            if(text[0] == "解除操控")
                            {
                                InLinking = false;
                                textView1.Text = "已解除操控模式";
                            }
                            if (text[0][0] == '開' && (text[0][1] == '啟' || text[0][1] == '啓'))
                            {
                                if (InLinking)
                                {
                                    OpenCloseThing(true, text[0]);
                                }
                                else
                                {
                                    OpenAPP(text[0]);
                                }
                            }
                            if(text[0][0] == '關' && text[0][1] == '閉')
                            {
                                if (InLinking)
                                {
                                    OpenCloseThing(false ,text[0]);
                                }
                                else
                                {
                                    
                                }
                            }
                            if (text[0][0] == '連' && (text[0][1] == '接' || text[0][1] == '結'))
                            {
                                if (InLinking)
                                {
                                    OpenCloseThing(true, text[0]);
                                }
                                else
                                {
                                    OpenInternet(text[0]);
                                }
                            }
                        }
                        break;
                    }

            }
        }

        private IList<PackageInfo> GetAllApps(Context context)
        {
            PackageManager pManager = context.PackageManager;
            //獲取手機內所有應用  
            IList<PackageInfo> paklist = pManager.GetInstalledPackages(0);
            return paklist;
        }

        private void OpenCloseThing(bool OpenOrClose, string Text)
        {
            string[] a;
            WebView1 = (WebView)FindViewById(Resource.Id.webView1);
            WebView1.Settings.JavaScriptEnabled = true;
            WebView1.ScrollBarStyle = 0;
            WebSettings webSettings = WebView1.Settings;
            webSettings.AllowFileAccess = true;
            webSettings.BuiltInZoomControls = true;
            if (OpenOrClose)
            {
                if (Text[1] == '啟')
                {
                    a = Text.Split('啟');
                }
                else if (Text[1] == '啓')
                {
                    a = Text.Split('啓');
                }
                else if(Text[1] == '接')
                {
                    a = Text.Split('接');
                }
                else if (Text[1] == '結')
                {
                    a = Text.Split('結');
                }
                else
                {
                    a = new string[2];
                    a[0] = "";
                    a[1] = "";
                }
                switch (a[1])
                {
                    case "一樓電燈":
                        {
                            WebView1.LoadUrl(ip + "/led_1_on.php");
                            //加載數據
                            break;
                        }
                    case "二樓電燈":
                        {
                            WebView1.LoadUrl(ip + "/led_2_on.php");
                            break;
                        }
                    case "屋頂風扇":
                        {
                            WebView1.LoadUrl(ip + "/fan_1_on.php");
                            break;
                        }
                    case "攝影機":
                        {
                            if (a[0] == "連")
                            {
                                WebView1.LoadUrl(ip + ":8080/javascript_simple.html");
                            }
                            break;
                        }
                }
            }
            else
            {
                if (Text[1] == '閉')
                {
                    a = Text.Split('閉');
                }
                else
                {
                    a = new string[2];
                    a[0] = "";
                    a[1] = "";
                }
                switch (a[1])
                {
                    case "一樓電燈":
                        {
                            WebView1.LoadUrl(ip + "/led_1_off.php");
                            break;
                        }
                    case "二樓電燈":
                        {
                            WebView1.LoadUrl(ip + "/led_2_off.php");
                            break;
                        }
                    case "屋頂風扇":
                        {
                            WebView1.LoadUrl(ip + "/fan_1_off.php");
                            break;
                        }
                    case "警報":
                        {
                            WebView1.LoadUrl(ip + "/filereset.php");
                            break;
                        }
                }
            }
            WebView1.SetWebChromeClient(new CustWebViewClient(this));
        }

        private void OpenInternet(string Text)
        {
            string[] a;
            if (Text[1] == '接')
            {
                a = Text.Split('接');
            }
            else if (Text[1] == '結')
            {
                a = Text.Split('結');
            }
            else
            {
                a = new string[2];
                a[0] = "";
                a[1] = "";
            }
            if (String.Compare(a[1], "Google", true) == 0)
            {
                String url = "https://www.google.com.tw";

                Intent ie = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));

                StartActivity(ie);

                WebView1 = (WebView)FindViewById(Resource.Id.webView1);
                WebView1.Settings.JavaScriptEnabled = true;
                WebView1.ScrollBarStyle = 0;
                WebSettings webSettings = WebView1.Settings;
                webSettings.AllowFileAccess = true;
                webSettings.BuiltInZoomControls = true;
                WebView1.LoadUrl("https://www.google.com.tw");
                //加載數據
                WebView1.SetWebChromeClient(new CustWebViewClient(this));
            }
        }

        private void OpenAPP(string Text)
        {
            IList<PackageInfo> App = GetAllApps(this);
            PackageManager pManager = this.PackageManager;
            string[] a;
            if (Text[1] == '啟')
            {
                a = Text.Split('啟');
            }
            else if (Text[1] == '啓')
            {
                a = Text.Split('啓');
            }
            else
            {
                a = new string[2];
                a[0] = "";
                a[1] = "";
            }
            int b = -1;
            PackageInfo d = null;
            try
            {
                for (int i = 0; i < App.Count; i++)
                {
                    string[] c = pManager.GetApplicationLabel(App[i].ApplicationInfo).Split(' ');
                    string[] e = a[1].Split(' ');
                    string cc = "";
                    string ee = "";
                    int f = 0;
                    for (int ii = 0; ii < c.Length; ii++)
                    {
                        cc += c[ii];
                    }
                    for (int ii = 0; ii < e.Length; ii++)
                    {
                        ee += e[ii];
                    }
                    for (int ii = 0; ii < cc.Length; ii++)
                    {
                        try
                        {
                            if (cc[ii] != ee[ii] || ee.Length <= ii)
                            {
                                f++;
                            }
                        }
                        catch (Exception)
                        {
                            f++;
                        }
                        if (ii == cc.Length - 1)
                        {
                            f += ee.Length - (ii + 1);
                        }
                    }
                    if (b >= f || b == -1)
                    {
                        if (f == 0)
                        {
                            try
                            {
                                Intent activityIntent = this.PackageManager.GetLaunchIntentForPackage(App[i].PackageName);
                                StartActivity(activityIntent);
                            }
                            catch (Exception g)
                            {
                                textView1.Text += g.Message;
                            }
                        }
                        d = App[i];
                        b = f;
                    }
                }
                try
                {
                    textView1.Text += b + pManager.GetApplicationLabel(d.ApplicationInfo);
                }
                catch (Exception)
                {

                }
                if (b < 5 && d != null && b != 0)
                {
                    try
                    {
                        Intent activityIntent = this.PackageManager.GetLaunchIntentForPackage(d.PackageName);
                        StartActivity(activityIntent);
                    }
                    catch (Exception e)
                    {
                        textView1.Text += e.Message;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private class CustWebViewClient : WebChromeClient
        {
            MainActivity mainActivity;
            public CustWebViewClient(MainActivity mainActivity) : base()
            {
                this.mainActivity = mainActivity;
            }

            public override void OnProgressChanged(WebView view, int newProgress)
            {
                if (newProgress == 100)
                {
                    mainActivity.textView1.Text += "加載完成";
                }
                else
                {

                }
            }
        }
    }
}

