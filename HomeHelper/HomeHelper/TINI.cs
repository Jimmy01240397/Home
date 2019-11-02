using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

using System.Runtime.InteropServices;

namespace HomeHelper
{
    public class TINI : IDisposable
    {
        private bool bDisposed = false;
        private string _FilePath = string.Empty;
        public string FilePath
        {
            get
            {
                if (_FilePath == null)
                    return string.Empty;
                else
                    return _FilePath;
            }
            set
            {
                if (_FilePath != value)
                    _FilePath = value;
            }
        }

        /// <summary>
        /// 建構子。
        /// </summary>
        /// <param name="path">檔案路徑。</param>      
        public TINI(string path)
        {
            _FilePath = path;
        }

        /// <summary>
        /// 解構子。
        /// </summary>
        ~TINI()
        {
            Dispose(false);
        }

        /// <summary>
        /// 釋放資源(程式設計師呼叫)。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); //要求系統不要呼叫指定物件的完成項。
        }

        /// <summary>
        /// 釋放資源(給系統呼叫的)。
        /// </summary>        
        protected virtual void Dispose(bool IsDisposing)
        {
            if (bDisposed)
            {
                return;
            }
            if (IsDisposing)
            {
                //補充：

                //這裡釋放具有實做 IDisposable 的物件(資源關閉或是 Dispose 等..)
                //ex: DataSet DS = new DataSet();
                //可在這邊 使用 DS.Dispose();
                //或是 DS = null;
                //或是釋放 自訂的物件。
                //因為我沒有這類的物件，故意留下這段 code ;若繼承這個類別，
                //可覆寫這個函式。
            }

            bDisposed = true;
        }


        /// <summary>
        /// 設定 KeyValue 值。
        /// </summary>
        /// <param name="IN_Section">Section。</param>
        /// <param name="IN_Key">Key。</param>
        /// <param name="IN_Value">Value。</param>
        public void SetKeyValue(string IN_Section, string IN_Key, string IN_Value)
        {
            string[] line = null;
            try
            {
                line = File.ReadAllLines(_FilePath);
            }
            catch(Exception)
            {
                File.WriteAllLines(_FilePath, new string[] { "[" + IN_Section + "]", IN_Key + "=" + IN_Value });
                return;
            }
            if (line == null)
            {
                File.WriteAllLines(_FilePath, new string[] { "[" + IN_Section + "]", IN_Key + "=" + IN_Value });
                return;
            }
            try
            {
                int a = 0;
                bool b = false;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == "[" + IN_Section + "]")
                    {
                        b = true;
                        int ii = i;
                        string[] line2 = null;
                        do
                        {
                            ii++;
                            line2 = line[ii].Split('=');
                        }
                        while (!(line2[0] == IN_Key || line2[0] == "" || line2[0][0] == '[' || line2[0] == null || line2 == null || ii == line.Length - 1));
                        if (line2[0] == IN_Key)
                        {
                            line[ii] = line2[0] + "=" + IN_Value;
                        }
                        else if(line2[0][0] == '[')
                        {
                            List<string> line3 = new List<string>();
                            for(int iii = 0; iii < ii; iii++)
                            {
                                line3.Add(line[iii]);
                            }
                            line3.Add(IN_Key + "=" + IN_Value);
                            for(int iii = ii; ii < line.Length; iii++)
                            {
                                line3.Add(line[iii]);
                            }
                            line = line3.ToArray();
                        }
                        else if(line2[0] == "" || line2[0] == null || line2 == null || ii == line.Length - 1)
                        {
                            List<string> line3 = new List<string>();
                            line3.AddRange(line);
                            line3.Add(IN_Key + "=" + IN_Value);
                            line = line3.ToArray();
                        }
                        else
                        {
                            List<string> line3 = new List<string>();
                            line3.AddRange(line);
                            line3.Add(IN_Key + "=" + IN_Value);
                            line = line3.ToArray();
                        }
                        break;
                    }
                }
                if(!b)
                {
                    List<string> line3 = new List<string>();
                    line3.AddRange(line);
                    line3.Add("[" + IN_Section + "]");
                    line3.Add(IN_Key + "=" + IN_Value);
                    line = line3.ToArray();
                }
            }
            catch (Exception e)
            {
                File.WriteAllLines(_FilePath, new string[] { "[" + IN_Section + "]", IN_Key + "=" + IN_Value , e.ToString()});
                return;
            }
            File.WriteAllLines(_FilePath, line);
        }

        /// <summary>
        /// 取得 Key 相對的 Value 值。
        /// </summary>
        /// <param name="IN_Section">Section。</param>
        /// <param name="IN_Key">Key。</param>        
        public string GetKeyValue(string IN_Section, string IN_Key)
        {
            using (StreamReader sr = new StreamReader(_FilePath))
            {
                try
                {
                    string line = "";
                    do
                    {
                        line = sr.ReadLine();
                        if (line == "" || line == null)
                        {
                            return "";
                        }
                    }
                    while (!(line == "[" + IN_Section + "]" || line == "" || line == null));
                    string[] line2;
                    int i = 0;
                    do
                    {
                        line2 = sr.ReadLine().Split('=');
                        if (line2[0][0] == '[' || line2[0] == "" || line2[0] == null)
                        {
                            return "";
                        }
                        else if(line2[0] == IN_Key)
                        {
                            return line2[1];
                        }
                        i++;
                    }
                    while (!(line2[0] == IN_Key || line2[0][0] == '[' || line2[0] == "" || line2[0] == null));
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
                return "";
            }
        }
    }
}