using IBoxDB.LocalServer;
using IBoxDB.LocalServer.IO;
using IBoxDB.LocalServer.Replication;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Socket = IBoxDB.LocalServer.Replication.Socket;

namespace ProjectIboxDb
{
    public class Package
    {
        public Socket Socket;
        public byte[] OutBox;
    }
    // recycle boxes
    public class MemoryBoxRecycler : IBoxRecycler
    {
        public List<Package> Packages = new List<Package>();
        public Guid lastId = Guid.Empty;

        public MemoryBoxRecycler() { }
        public MemoryBoxRecycler(long name, DatabaseConfig config) : this()
        {
        }

        //3.0 update, change to 'override'
        public override void OnReceived(Socket socket, BoxData outBox, bool normal)
        {
            lock (Packages)
            {
                if (socket.DestAddress == long.MaxValue)
                {
                    // default replication address
                    return;
                }
                if (!normal)
                {
                    // database restart from poweroff, sample
                    if (Packages.Count > 0)
                    {
                        Package last = Packages[Packages.Count - 1];
                        if (last.Socket.ID.Equals(socket.ID))
                        {
                            return;
                        }
                    }
                }

                //Save data
                Packages.Add(new Package { Socket = socket, OutBox = outBox.ToBytes() });
            }
        }

        public override void Dispose()
        {
            Packages = null;
        }
        public BoxData[] AsBoxData()
        {
            List<BoxData> list = new List<BoxData>();
            lock (Packages)
            {
                foreach (var p in Packages)
                {
                    list.Add(new BoxData(p.OutBox));
                }
                Packages.Clear();
            }
            return list.ToArray();
        }

    }

    //ALL in One Config, var server = new ApplicationServer();
    public class ApplicationServer : LocalDatabaseServer
    {
        public class Config : BoxFileStreamConfig
        {
            public Config()
                : base()
            {
                CacheLength = MB(512);
                FileIncSize = (int)MB(4);
                ReadStreamCount = 8;
            }
        }
        class MyConfig : Config
        {
            public MyConfig()
                : base()
            {
            }
        }
        public const int MasterA_DBAddress = 10;
        public const int MasterB_DBAddress = 20;
        public const int SlaveA_DBAddress = -10;

        protected override DatabaseConfig BuildDatabaseConfig(long name)
        {
            if (name == MasterB_DBAddress || name == MasterA_DBAddress)
            {
                return new MyConfig();
            }
            if (name == SlaveA_DBAddress)
            {
                return new Config();
            }
            throw new NotImplementedException();
        }

        protected override IBoxRecycler BuildBoxRecycler(long name, DatabaseConfig config)
        {
            if (name == MasterA_DBAddress || name == MasterB_DBAddress)
            {
                return new MemoryBoxRecycler(name, config);
            }
            return base.BuildBoxRecycler(name, config);
        }
    }
}
