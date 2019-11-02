using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNetwork;
using UnityNetwork.Server;
using System.Net.Sockets;
using System.Data.Odbc;
using MySql.Data.MySqlClient;

namespace Servers
{
    public class Appllication:AppllicationTCPBase
    {
        string dbHost = "localhost";//資料庫位址
        string dbPort = "3306";//資料庫卓
        string dbUser = "root";//資料庫使用者帳號
        string dbPass = "ekids178";//資料庫使用者密碼
        string dbName = "idpassward";//資料庫名稱
        public OdbcConnection con;
        public OdbcCommand command;
        public Dictionary<string, Peer> HostIDPeer;
        public Dictionary<string, List<Peer>> ClientIDPeer;
        public Dictionary<Peer, string> ClientPeerID;
        public Dictionary<Peer, string> HostPeerID;
        public Dictionary<Guid, Peer> GuidPeer;
        public Dictionary<Peer, Guid> PeerGuid;
        public override PeerTCPBase AddPeerBase(TcpClient _peer, NetTCPServer server)
        {
            return new Peer(_peer, server, this);
        }
        public override int GetPort()
        {
            return 8081;
        }
        public override void Setup()
        {
            HostIDPeer = new Dictionary<string, Peer>();
            ClientIDPeer = new Dictionary<string, List<Peer>>();
            ClientPeerID = new Dictionary<Peer, string>();
            HostPeerID = new Dictionary<Peer, string>();
            GuidPeer = new Dictionary<Guid, Peer>();
            PeerGuid = new Dictionary<Peer, Guid>();
            try
            {
                con = new OdbcConnection("Driver={MySQL ODBC 5.3 Unicode Driver}" + ";Server=" + dbHost + ";Port=" + dbPort + ";Database=" + dbName + ";UID=" + dbUser + ";Password=" + dbPass + ";OPTION=3");
                command = con.CreateCommand();
                con.Open();
            }
            catch(Exception e)
            {
                this._server_GetMessage(e.ToString());
            }
        }
        public override void TearDown()
        {
            
        }
        public override void CleanUp()
        {
            HostIDPeer.Clear();
            ClientIDPeer.Clear();
            HostPeerID.Clear();
            ClientPeerID.Clear();
        }
    }
}
