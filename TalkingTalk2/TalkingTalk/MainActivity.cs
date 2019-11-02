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
using UnityNetwork;
using UnityNetwork.Client;
using Android.Content.Res;

namespace TalkingTalk
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, ClientListenTCP
    {
        protected const int RESULT_SPEECH = 1;

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private TextView textView1;
        private TextView textView2;
        private TextView textView3;
        private WebView WebView1;
        private RelativeLayout RelativeLayout1;
        private ClientLinkerTCP _ClientLinker;
        private EditText editText1;
        private EditText editText2;

        private Timer timer1;
        private Timer timer2;

        private bool _warning = false;
        private bool InLinking = false;
        private bool link = false;
        private bool Logei = false;
        private bool linking = false;
        private bool colorchange = false;
        private string ip = "http://192.168.43.196";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GOTOPassward();

            _ClientLinker = new ClientLinkerTCP(this);
            Thread thread = new Thread(new ThreadStart(InLink));
            thread.Start();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            // TODO Auto-generated method stub
            if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
            {
                // 什麼都不用寫
            }
            else
            {
                // 什麼都不用寫
            }
        }

        private void GOTOPassward()
        {
            SetContentView(Resource.Layout.password);
            textView3 = FindViewById<TextView>(Resource.Id.textView3);
            button2 = FindViewById<Button>(Resource.Id.button2);
            button4 = FindViewById<Button>(Resource.Id.button4);
            editText1 = FindViewById<EditText>(Resource.Id.editText1);
            editText2 = FindViewById<EditText>(Resource.Id.editText2);
            button2.Click += button2_Click;
            button4.Click += button4_Click;
            try
            {
                button1.Click -= button1_Click;
                button3.Click -= button3_Click;
            }
            catch(Exception)
            {

            }
        }

        private void GOTOPassward(string text)
        {
            SetContentView(Resource.Layout.password);
            textView3 = FindViewById<TextView>(Resource.Id.textView3);
            button2 = FindViewById<Button>(Resource.Id.button2);
            button4 = FindViewById<Button>(Resource.Id.button4);
            editText1 = FindViewById<EditText>(Resource.Id.editText1);
            editText2 = FindViewById<EditText>(Resource.Id.editText2);
            button2.Click += button2_Click;
            button4.Click += button4_Click;
            textView3.Text = text;
            try
            {
                button1.Click -= button1_Click;
                button3.Click -= button3_Click;
            }
            catch (Exception)
            {

            }
        }

        private void GOTOMain()
        {
            SetContentView(Resource.Layout.activity_main);
            textView1 = FindViewById<TextView>(Resource.Id.textView1);
            textView2 = FindViewById<TextView>(Resource.Id.textView2);
            button1 = FindViewById<Button>(Resource.Id.button1);
            button3 = FindViewById<Button>(Resource.Id.button3);

            RelativeLayout1 = FindViewById<RelativeLayout>(Resource.Id.RelativeLayout1);
            button1.Click += button1_Click;
            button3.Click += button3_Click;
            try
            {
                button2.Click -= button2_Click;
                button4.Click -= button4_Click;
            }
            catch (Exception)
            {

            }
        }

        private void InLink()
        {
            if (!linking)
            {
                linking = true;
                link = true;
                TimerCallback timerCallback1 = new TimerCallback(OnLink);
                timer1 = new Timer(timerCallback1, null, 0, 50);
                if (_ClientLinker.Connect("59.127.53.197", 8081))
                {
                    Thread thread = new Thread(new ThreadStart(Warning));
                    thread.Start();
                    linking = false;
                }
            }
        }

        private void OnLink(object state)
        {
            this.RunOnUiThread(() =>
            {
                if (link)
                {
                    _ClientLinker.Update(false);
                }
            });
        }

        private void Warning()
        {
            while (link)
            {
                if (Logei)
                {
                    _ClientLinker.ask(6, new Dictionary<byte, object>());
                    Thread.Sleep(1000);
                }
            }
        }

        private void Warning(object state)
        {
            this.RunOnUiThread(() =>
            {
                try
                {
                    WebClient MyWebClient = new WebClient();
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                    Byte[] pageData = MyWebClient.DownloadData(ip + "/warning.txt");
                    _warning = Convert.ToBoolean(Convert.ToInt32(Encoding.UTF8.GetString(pageData)));
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (link)
            {
                _ClientLinker.ask(1, new Dictionary<byte, object> { { 0, editText1.Text }, { 1, editText2.Text }, { 2, false } });
            }
            else
            {
                Thread thread = new Thread(new ThreadStart(InLink));
                thread.Start();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(Logei)
            {
                _ClientLinker.ask(4, new Dictionary<byte, object> { { 0, false } });
            }
            else
            {
                GOTOPassward();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GOTOMain();
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
                                if (Logei)
                                {
                                    InLinking = true;
                                    textView1.Text = "已啟動操控模式";
                                }
                                else
                                {
                                    textView1.Text = "若要啟動操控模式請先登入";
                                }
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
                                    string[] a;
                                    string Text = text[0];
                                    a = Text.Split(Text[1]);
                                    _ClientLinker.ask(2, new Dictionary<byte, object> { { 0, a[1] } });
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
                                    string[] a;
                                    string Text = text[0];
                                    a = Text.Split(Text[1]);
                                    _ClientLinker.ask(3, new Dictionary<byte, object> { { 0, a[1] } });
                                }
                                else
                                {
                                    
                                }
                            }
                            if (text[0][0] == '連' && (text[0][1] == '接' || text[0][1] == '結'))
                            {
                                if (InLinking)
                                {
                                    string[] a;
                                    string Text = text[0];
                                    a = Text.Split(Text[1]);
                                    _ClientLinker.ask(2, new Dictionary<byte, object> { { 0, a[1] } });
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
        public void DebugReturn(string DebugMessage)
        {
            Logei = false;
            timer1.Dispose();
            timer2.Dispose();
            timer1 = null;
            timer2 = null;
            if(InLinking)
            {
                textView1.Text = "因網路斷線，已解除操控模式";
                InLinking = false;
            }
        }
        public void Loading(string LoadMessage)
        {

        }
        public void OnEvent(Response response)
        {

        }
        public void OnOperationResponse(Response response)
        {
            switch (response.Code)
            {
                case 1:
                    {
                        if(response.ReturnCode == 0)
                        {
                            GOTOMain();
                            Logei = true;
                        }
                        else if(response.ReturnCode == 1)
                        {
                            textView3.Text = response.DebugMessage;
                        }
                        break;
                    }
                case 2:
                    {
                        textView1.Text += response.DebugMessage;
                        break;
                    }
                case 3:
                    {
                        textView1.Text += response.DebugMessage;
                        break;
                    }
                case 4:
                    {
                        if (response.ReturnCode == 0)
                        {
                            GOTOPassward(response.DebugMessage);
                            Logei = false;
                        }
                        else if (response.ReturnCode == 1)
                        {
                            textView1.Text = response.DebugMessage;
                        }
                        break;
                    }
                case 6:
                    {
                        try
                        {
                            _warning = Convert.ToBoolean(Convert.ToInt32(response.DebugMessage));
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
                        break;
                    }
            }
        }
        public void OnStatusChanged(LinkCobe linkCobe)
        {
            switch (linkCobe)
            {
                case LinkCobe.Connect:
                    {
                        link = true;
                        button2.Text = "登入";
                        break;
                    }
                case LinkCobe.Lost:
                    {
                        link = false;
                        button2.Text = "重新連線";
                        break;
                    }
                case LinkCobe.Failed:
                    {
                        link = false;
                        button2.Text = "重新連線";
                        break;
                    }
                default:
                    {
                        break;
                    }
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

