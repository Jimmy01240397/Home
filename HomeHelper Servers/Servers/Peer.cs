using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNetwork;
using UnityNetwork.Server;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.Data.Odbc;

namespace Servers
{
    public class Peer:PeerTCPBase
    {
        Appllication TheAppllication;
        bool Logie = false;
        bool Host = false;
        public Peer(TcpClient peer, NetTCPServer _server, Appllication appllication) :base(peer, _server)
        {
            TheAppllication = appllication;
        }

        public override void OnDisconnect()
        {
            try
            {
                if (Host)
                {
                    if (TheAppllication.HostPeerID.ContainsKey(this))
                    {
                        try
                        {
                            for (int i = 0; i < TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]].Count; i++)
                            {
                                TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i].Logie = false;
                                TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i].Reply(4, new Dictionary<byte, object>(), 0, "因主機端登出因此將進行強制登出");
                                TheAppllication.ClientPeerID.Remove(TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i]);
                                TheAppllication.GuidPeer.Remove(TheAppllication.PeerGuid[TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i]]);
                                TheAppllication.PeerGuid.Remove(TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i]);
                            }
                        }
                        catch(Exception)
                        {

                        }
                        TheAppllication.HostIDPeer.Remove(TheAppllication.HostPeerID[this]);
                        TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]].Clear();
                        TheAppllication.ClientIDPeer.Remove(TheAppllication.HostPeerID[this]);
                        TheAppllication.HostPeerID.Remove(this);
                        TheAppllication.GuidPeer.Remove(TheAppllication.PeerGuid[this]);
                        TheAppllication.PeerGuid.Remove(this);
                        Logie = false;
                        Reply(4, new Dictionary<byte, object>(), 0, "登出成功");
                    }
                }
                else
                {
                    if (TheAppllication.ClientPeerID.ContainsKey(this))
                    {
                        TheAppllication.ClientIDPeer[TheAppllication.ClientPeerID[this]].Remove(this);
                        TheAppllication.ClientPeerID.Remove(this);
                        TheAppllication.GuidPeer.Remove(TheAppllication.PeerGuid[this]);
                        TheAppllication.PeerGuid.Remove(this);
                        Logie = false;
                        Reply(4, new Dictionary<byte, object>(), 0, "登出成功");
                    }
                }
            }
            catch(Exception)
            {

            }
        }

        public override void OnOperationRequest(Response response)
        {
            switch (response.Code)
            {
                case 1:
                    {
                        OdbcDataReader reader = null;
                        try
                        {
                            TheAppllication.command.CommandText = "select * from IDPassward where ID like '" + response.Parameters[0] + "' and Passward like '" + response.Parameters[1] + "';";
                            reader = TheAppllication.command.ExecuteReader();
                            if (reader.Read())
                            {
                                string ID = reader.GetString(0);
                                string Passward = reader.GetString(1);
                                if (Convert.ToBoolean(response.Parameters[2].ToString()))
                                {
                                    if (!TheAppllication.HostIDPeer.ContainsKey(response.Parameters[0].ToString()))
                                    {
                                        TheAppllication.HostIDPeer.Add(response.Parameters[0].ToString(), this);
                                        TheAppllication.HostPeerID.Add(this, response.Parameters[0].ToString());
                                        TheAppllication.ClientIDPeer.Add(response.Parameters[0].ToString(), new List<Peer>());
                                        TheAppllication.PeerGuid.Add(this, Guid.NewGuid());
                                        TheAppllication.GuidPeer.Add(TheAppllication.PeerGuid[this], this);
                                        Logie = true;
                                        Host = Convert.ToBoolean(response.Parameters[2].ToString());
                                        Reply(1, response.Parameters, 0, "成功");
                                    }
                                    else
                                    {
                                        Reply(1, response.Parameters, 1, "主機已有人登入");
                                    }
                                }
                                else
                                {
                                    if (TheAppllication.HostIDPeer.ContainsKey(response.Parameters[0].ToString()))
                                    {
                                        if (!TheAppllication.ClientIDPeer.ContainsKey(response.Parameters[0].ToString()))
                                        {
                                            TheAppllication.ClientIDPeer.Add(response.Parameters[0].ToString(), new List<Peer>() { this });
                                        }
                                        else
                                        {
                                            TheAppllication.ClientIDPeer[response.Parameters[0].ToString()].Add(this);
                                        }
                                        TheAppllication.ClientPeerID.Add(this, response.Parameters[0].ToString());
                                        TheAppllication.PeerGuid.Add(this, Guid.NewGuid());
                                        TheAppllication.GuidPeer.Add(TheAppllication.PeerGuid[this], this);
                                        Logie = true;
                                        Host = Convert.ToBoolean(response.Parameters[2].ToString());
                                        Reply(1, response.Parameters, 0, "成功");
                                    }
                                    else
                                    {
                                        Reply(1, response.Parameters, 1, "沒有已登入的主機");
                                    }
                                }
                            }
                            else
                            {
                                Reply(1, response.Parameters, 1, "帳號或密碼錯誤");
                            }
                        }
                        catch (Exception e)
                        {
                            Reply(1, response.Parameters, 1, "錯誤：" + e.ToString());
                        }
                        reader.Close();
                        reader = null;
                        break;
                    }
                case 2:
                    {
                        if (Host)
                        {
                            TheAppllication.GuidPeer[Guid.Parse(response.Parameters[2].ToString())].Reply(2, response.Parameters, 0, response.Parameters[1].ToString());
                        }
                        else
                        {
                            response.Parameters.Add(2, TheAppllication.PeerGuid[this]);
                            TheAppllication.HostIDPeer[TheAppllication.ClientPeerID[this]].Reply(2, response.Parameters, 0, "");
                        }
                        break;
                    }
                case 3:
                    {
                        if (Host)
                        {
                            TheAppllication.GuidPeer[Guid.Parse(response.Parameters[2].ToString())].Reply(3, response.Parameters, 0, response.Parameters[1].ToString());
                        }
                        else
                        {
                            response.Parameters.Add(2, TheAppllication.PeerGuid[this]);
                            TheAppllication.HostIDPeer[TheAppllication.ClientPeerID[this]].Reply(3, response.Parameters, 0, "");
                        }
                        break;
                    }
                case 4:
                    {
                        try
                        {
                            if (Convert.ToBoolean(response.Parameters[0].ToString()))
                            {
                                if (TheAppllication.HostPeerID.ContainsKey(this))
                                {
                                    for (int i = 0; i < TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]].Count; i++)
                                    {
                                        TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i].Logie = false;
                                        TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i].Reply(4, response.Parameters, 0, "因主機端登出因此將進行強制登出");
                                        TheAppllication.ClientPeerID.Remove(TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i]);
                                        TheAppllication.GuidPeer.Remove(TheAppllication.PeerGuid[TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i]]);
                                        TheAppllication.PeerGuid.Remove(TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]][i]);
                                    }
                                    TheAppllication.HostIDPeer.Remove(TheAppllication.HostPeerID[this]);
                                    TheAppllication.ClientIDPeer[TheAppllication.HostPeerID[this]].Clear();
                                    TheAppllication.ClientIDPeer.Remove(TheAppllication.HostPeerID[this]);
                                    TheAppllication.HostPeerID.Remove(this);
                                    TheAppllication.GuidPeer.Remove(TheAppllication.PeerGuid[this]);
                                    TheAppllication.PeerGuid.Remove(this);
                                    Logie = false;
                                    Reply(4, response.Parameters, 0, "登出成功");
                                }
                            }
                            else
                            {
                                if (TheAppllication.ClientPeerID.ContainsKey(this))
                                {
                                    TheAppllication.ClientIDPeer[TheAppllication.ClientPeerID[this]].Remove(this);
                                    TheAppllication.ClientPeerID.Remove(this);
                                    TheAppllication.GuidPeer.Remove(TheAppllication.PeerGuid[this]);
                                    TheAppllication.PeerGuid.Remove(this);
                                    Logie = false;
                                    Reply(4, response.Parameters, 0, "登出成功");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Reply(4, response.Parameters, 1, "錯誤：" + e.Message);
                        }
                        break;
                    }
                case 5:
                    {
                        OdbcDataReader reader = null;
                        try
                        {
                            TheAppllication.command.CommandText = "select * from IDPassward where ID like '" + response.Parameters[0] + "';";
                            reader = TheAppllication.command.ExecuteReader();
                            if (reader.Read())
                            {
                                string ID = reader.GetString(0);
                                Reply(5, response.Parameters, 1, "此ID已有人註冊");
                            }
                            else
                            {
                                reader.Close();
                                TheAppllication.command.CommandText = "insert into IDPassward(ID,Passward) values('" + response.Parameters[0] + "','" + response.Parameters[1] + "');";
                                TheAppllication.command.ExecuteNonQuery();
                                TheAppllication.HostIDPeer.Add(response.Parameters[0].ToString(), this);
                                TheAppllication.HostPeerID.Add(this, response.Parameters[0].ToString());
                                TheAppllication.ClientIDPeer.Add(response.Parameters[0].ToString(), new List<Peer>());
                                TheAppllication.PeerGuid.Add(this, Guid.NewGuid());
                                TheAppllication.GuidPeer.Add(TheAppllication.PeerGuid[this], this);
                                Logie = true;
                                Host = Convert.ToBoolean(response.Parameters[2].ToString());
                                Reply(5, response.Parameters, 0, "註冊成功，帳號已完成登入");
                            }
                        }
                        catch (Exception e)
                        {
                            Reply(1, response.Parameters, 1, "錯誤：" + e.Message);
                        }
                        reader = null;
                        break;
                    }
                case 6:
                    {
                        if (Host)
                        {
                            TheAppllication.GuidPeer[Guid.Parse(response.Parameters[1].ToString())].Reply(6, response.Parameters, 0, response.Parameters[0].ToString());
                        }
                        else
                        {
                            response.Parameters.Add(1, TheAppllication.PeerGuid[this]);
                            TheAppllication.HostIDPeer[TheAppllication.ClientPeerID[this]].Reply(6, response.Parameters, 0, "");
                        }
                        break;
                    }
            }
        }
    }
}
