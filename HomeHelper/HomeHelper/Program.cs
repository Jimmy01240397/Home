using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityNetwork;
using UnityNetwork.Client;

namespace HomeHelper
{
    class Program : ClientListenTCP
    {
        ClientLinkerTCP listener;
        bool link;
        public Program()
        {
            listener = new ClientLinkerTCP(this);
            link = true;
        }
        static void Main(string[] args)
        {
            Program program = new Program();
            program.run();
            program.Update();
            program.Update2();
        }
        public void run()
        {
            link = true;
            if (listener.Connect("59.127.53.197", 8081))
            {
                
            }
        }
        public void Update()
        {
            do
            {
                if (link)
                {
                    listener.Update(false);
                    System.Threading.Thread.Sleep(50);
                }
            }
            while (link);
        }
        public void Update2()
        {
            do
            {
                if (link)
                {
                    listener.ask(6, new Dictionary<byte, object>() { { 0, File.ReadAllText("warning.txt") } });
                    System.Threading.Thread.Sleep(1000);
                }
            }
            while (link);
        }
        public void DebugReturn(string message)
        {
            Console.WriteLine("錯誤:" + message);
        }

        public void Loading(string message)
        {
            Console.WriteLine("Load:" + message);
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
                        if (response.ReturnCode == 0)
                        {
                            Console.WriteLine("登入成功");
                            using (TINI oTINI = new TINI(Path.Combine(Directory.GetCurrentDirectory(), "IDPassward.ini")))
                            {
                                oTINI.SetKeyValue("IDPassward", "ID", response.Parameters[0].ToString());
                                oTINI.SetKeyValue("IDPassward", "Passward", response.Parameters[1].ToString());
                            }
                        }
                        else if (response.ReturnCode == 1)
                        {
                            string a = "n";
                            Console.WriteLine(response.DebugMessage);
                            Console.Write("是否重瓣帳號(y/n)");
                            a = Console.ReadLine();
                            while (!(a == "y" || a == "Y" || a == "n" || a == "N"))
                            {
                                a = Console.ReadLine();
                            }
                            Console.Write("帳號：");
                            response.Parameters[0] = Console.ReadLine();
                            Console.Write("密碼：");
                            response.Parameters[1] = Console.ReadLine();
                            if (a == "y" || a == "Y")
                            {
                                listener.ask(5, response.Parameters);
                            }
                            else if (a == "n" || a == "N")
                            {
                                listener.ask(1, response.Parameters);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        switch (response.Parameters[0])
                        {
                            case "一樓電燈":
                                {
                                    File.WriteAllText("command.txt", "led1\r\non");
                                    response.Parameters.Add(1, "成功開啟一樓電燈");
                                    Console.WriteLine("成功開啟一樓電燈");
                                    listener.ask(2, response.Parameters);
                                    break;
                                }
                            case "二樓電燈":
                                {
                                    File.WriteAllText("command.txt", "led2\r\non");
                                    response.Parameters.Add(1, "成功開啟二樓電燈");
                                    Console.WriteLine("成功開啟二樓電燈");
                                    listener.ask(2, response.Parameters);
                                    break;
                                }
                            case "屋頂風扇":
                                {
                                    File.WriteAllText("command.txt", "fan1\r\non");
                                    response.Parameters.Add(1, "成功開啟屋頂風扇");
                                    Console.WriteLine("成功開啟屋頂風扇");
                                    listener.ask(2, response.Parameters);
                                    break;
                                }
                        }
                        break;
                    }
                case 3:
                    {
                        switch (response.Parameters[0])
                        {
                            case "一樓電燈":
                                {
                                    File.WriteAllText("command.txt", "led1\r\noff");
                                    response.Parameters.Add(1, "成功關閉一樓電燈");
                                    Console.WriteLine("成功關閉一樓電燈");
                                    listener.ask(3, response.Parameters);
                                    break;
                                }
                            case "二樓電燈":
                                {
                                    File.WriteAllText("command.txt", "led2\r\noff");
                                    response.Parameters.Add(1, "成功關閉二樓電燈");
                                    Console.WriteLine("成功關閉二樓電燈");
                                    listener.ask(3, response.Parameters);
                                    break;
                                }
                            case "屋頂風扇":
                                {
                                    File.WriteAllText("command.txt", "fan1\r\noff");
                                    response.Parameters.Add(1, "成功關閉屋頂風扇");
                                    Console.WriteLine("成功關閉屋頂風扇");
                                    listener.ask(3, response.Parameters);
                                    break;
                                }
                            case "警報":
                                {
                                    File.WriteAllText("warning.txt", "0");
                                    response.Parameters.Add(1, "成功關閉警報");
                                    Console.WriteLine("成功關閉警報");
                                    listener.ask(3, response.Parameters);
                                    break;
                                }
                        }
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine(response.DebugMessage);
                        break;
                    }
                case 5:
                    {
                        if (response.ReturnCode == 0)
                        {
                            Console.WriteLine(response.DebugMessage);
                            using (TINI oTINI = new TINI(Path.Combine(Directory.GetCurrentDirectory(), "IDPassward.ini")))
                            {
                                oTINI.SetKeyValue("IDPassward", "ID", response.Parameters[0].ToString());
                                oTINI.SetKeyValue("IDPassward", "Passward", response.Parameters[1].ToString());
                            }
                        }
                        else if (response.ReturnCode == 1)
                        {
                            string a = "n";
                            Console.WriteLine(response.DebugMessage);
                            Console.Write("是否重瓣帳號(y/n)");
                            a = Console.ReadLine();
                            while (!(a == "y" || a == "Y" || a == "n" || a == "N"))
                            {
                                a = Console.ReadLine();
                            }
                            Console.Write("帳號：");
                            response.Parameters[0] = Console.ReadLine();
                            Console.Write("密碼：");
                            response.Parameters[1] = Console.ReadLine();
                            if (a == "y" || a == "Y")
                            {
                                listener.ask(5, response.Parameters);
                            }
                            else if (a == "n" || a == "N")
                            {
                                listener.ask(1, response.Parameters);
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        string a = "";
                        try
                        {
                            a = File.ReadAllText("warning.txt");
                        }
                        catch(Exception)
                        {
                            File.WriteAllText("warning.txt", "0");
                            a = File.ReadAllText("warning.txt");
                        }
                        response.Parameters.Add(0, a);
                        listener.ask(6, response.Parameters);
                        break;
                    }
            }
        }
        public void OnStatusChanged(LinkCobe connect)
        {
            Console.WriteLine(connect.ToString());
            switch (connect)
            {
                case LinkCobe.Connect:
                    {
                        link = true;
                        Console.WriteLine("GetLink");
                        string ID = "";
                        string Passward = "";
                        string a = "n";
                        using (TINI oTINI = new TINI(Path.Combine(Directory.GetCurrentDirectory(), "IDPassward.ini")))
                        {
                            try
                            {
                                ID = oTINI.GetKeyValue("IDPassward", "ID");
                                Passward = oTINI.GetKeyValue("IDPassward", "Passward");
                                if (ID == "" || ID == null || Passward == "" || Passward == null)
                                {
                                    Console.Write("第一次使用請先進行登入，是否重瓣帳號(y/n)");
                                    a = Console.ReadLine();
                                    while (!(a == "y" || a == "Y" || a == "n" || a == "N"))
                                    {
                                        a = Console.ReadLine();
                                    }
                                    Console.Write("帳號：");
                                    ID = Console.ReadLine();
                                    Console.Write("密碼：");
                                    Passward = Console.ReadLine();
                                }
                            }
                            catch (Exception)
                            {
                                Console.Write("第一次使用請先進行登入，是否重瓣帳號(y/n)");
                                a = Console.ReadLine();
                                while (!(a == "y" || a == "Y" || a == "n" || a == "N"))
                                {
                                    a = Console.ReadLine();
                                }
                                Console.Write("帳號：");
                                ID = Console.ReadLine();
                                Console.Write("密碼：");
                                Passward = Console.ReadLine();
                            }
                            if (a == "y" || a == "Y")
                            {
                                listener.ask(5, new Dictionary<byte, object>() { { 0, ID }, { 1, Passward }, { 2, true } });
                            }
                            else if (a == "n" || a == "N")
                            {
                                listener.ask(1, new Dictionary<byte, object>() { { 0, ID }, { 1, Passward }, { 2, true } });
                            }
                        }
                        break;
                    }
                case LinkCobe.Lost:
                    {
                        link = false;
                        Console.WriteLine("LinkLost");
                        Console.Write("是否進行重連？(y/n)");
                        string a = Console.ReadLine();
                        while (!(a == "y" || a == "Y" || a == "n" || a == "N"))
                        {
                            a = Console.ReadLine();
                        }
                        if(a == "y" || a == "Y")
                        {
                            run();
                        }
                        break;
                    }
                case LinkCobe.Failed:
                    {
                        link = false;
                        Console.WriteLine("LinkFailed");
                        Console.Write("是否進行重連？(y/n)");
                        string a = Console.ReadLine();
                        while (!(a == "y" || a == "Y" || a == "n" || a == "N"))
                        {
                            a = Console.ReadLine();
                        }
                        if (a == "y" || a == "Y")
                        {
                            run();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
