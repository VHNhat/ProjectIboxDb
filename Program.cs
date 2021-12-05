using IBoxDB.LocalServer;
using IBoxDB.LocalServer.Replication;
using ProjectIboxDb.Models;
using ProjectIboxDb.Services;
using ProjectIboxDb.Sockets;
using System;
using System.IO;
using System.Threading;

namespace ProjectIboxDb
{
    class Program
    {
        public static long masterAddress;
        public static long slaveAddress;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Service service = new Service();
            Boolean loop = true;
            var option = Setting();
            DB.AutoBox box = null;
            DB.AutoBox slave_box = null;
            switch (option)
            {
                case 1:
                    slave_box = InitAndCreateSlaveBox(slaveAddress);
                    break;
                case 2:
                    box = InitAndCreateAutoBox(masterAddress);
                    slave_box = InitAndCreateSlaveBox(slaveAddress);
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    return;
            }

            while (loop)
            {
                switch (option)
                {
                    case 1:
                        var slaveChoice = SlaveMenu();
                        switch (slaveChoice)
                        {
                            case 0:
                                break;
                            case 1:
                                service.Select(slave_box);
                                break;
                            case 2:
                                Distribute(option, masterAddress, slave_box);
                                return;
                            case 3:
                                option = Setting();
                                break;
                            case 4:
                                if (option == 1) Console.WriteLine("Slave");
                                else if (option == 2) Console.WriteLine("Master");
                                break;
                            default:
                                Console.WriteLine("Invalid choice");
                                break;
                        }
                        break;
                    case 2:
                        var masterChoice = MasterMenu();
                        switch (masterChoice)
                        {
                            case 0:
                                break;
                            case 1:
                                if(service.Insert(box)==1) Replicate(box, slave_box, masterAddress);
                                break;
                            case 2:
                                if(service.Update(box)) Replicate(box, slave_box, masterAddress);
                                break;
                            case 3:
                                if(service.Delete(box)) Replicate(box, slave_box, masterAddress);
                                break;
                            case 4:
                                service.Select(box);
                                break;
                            case 5:
                                Distribute(option, masterAddress, box);
                                return;
                            case 6:
                                option = Setting();
                                break;
                            case 7:
                                if (option == 1) Console.WriteLine("Slave");
                                else if (option == 2) Console.WriteLine("Master");
                                break;
                            default:
                                Console.WriteLine("Invalid choice");
                                break;
                        }
                        break;
                }
                

                Console.WriteLine("Do you want to continue?");
                Console.WriteLine("1.Yes");
                Console.WriteLine("0.No");
                Console.Write("Answer: ");
                var ans = int.Parse(Console.ReadLine());
                if (ans == 1) loop = true;
                else loop = false;
            }
        }
        
        private static int Setting()
        {
            Console.WriteLine("-----------------Setting-----------------");
            Console.WriteLine("Thiết lập Master/Slave");
            Console.WriteLine("1.Slave");
            Console.WriteLine("2.Master");
            Console.Write("Choice: ");
            var option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    Console.Write("Enter slave DB address: ");
                    slaveAddress = long.Parse(Console.ReadLine());
                    masterAddress = slaveAddress * -1;
                    break;
                case 2:
                    Console.Write("Enter the master DB address: ");
                    masterAddress = long.Parse(Console.ReadLine());
                    slaveAddress = masterAddress * -1;
                    break;
            }
            return option;
        }
        private static void Distribute(int option, long masterAdrress, DB.AutoBox box)
        {
            Console.WriteLine("-----------------Distribute Data-----------------");
            Console.Write("Enter DB address: ");
            masterAdrress = long.Parse(Console.ReadLine());
            // Slave
            if (option == 1)
            {
                box.GetDatabase().Dispose();
                Server server = new Server();
                server.StartServer();
            }
            // Master
            else if (option == 2)
            {
                Client client = new Client();
                Console.Write("Enter server IP address: ");
                string ipa = Console.ReadLine();
                box.GetDatabase().Dispose();
                Client.SendFile($"E:/Code Life/UIT/CSDLPT/iboxDb/ProjectIboxDb/bin/Debug/netcoreapp3.1/ibox/db_{masterAdrress}.box", ipa);
            }
        }

        public static int MasterMenu()
        {
            Console.WriteLine("Menu");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Insert");
            Console.WriteLine("2. Update");
            Console.WriteLine("3. Delete");
            Console.WriteLine("4. Select");
            Console.WriteLine("5. Distribute data");
            Console.WriteLine("6. Setting");
            Console.WriteLine("7. Current state");
            Console.Write("Choice: ");
            var choice = int.Parse(Console.ReadLine());
            return choice;
        }

        public static int SlaveMenu()
        {
            Console.WriteLine("Menu");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Select");
            Console.WriteLine("2. Distribute data");
            Console.WriteLine("3. Setting");
            Console.WriteLine("4. Current state");
            Console.Write("Choice: ");
            var choice = int.Parse(Console.ReadLine());
            return choice;
        }

        public static DB.AutoBox InitAndCreateAutoBox(long addressDb)
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ibox");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            var server = new DB(addressDb, dbPath);
            var config = server.GetConfig();
            server.SetBoxRecycler(new MemoryBoxRecycler());
            config.EnsureTable<Account>("Account", "Id");
            config.EnsureTable<Role>("Role", "Id");

            return server.Open();
        }
        public static DB.AutoBox InitAndCreateSlaveBox(long slaveAddress)
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ibox");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            var server = new DB(slaveAddress, dbPath);
            return server.Open();
        }

        public static void Replicate(DB.AutoBox box, DB.AutoBox slave_box, long masterAddress)
        {
            var recycler = (MemoryBoxRecycler)box.GetDatabase().GetBoxRecycler();
            lock (recycler.Packages)
            {
                foreach (var p in recycler.Packages)
                {
                    if (p.Socket.SourceAddress == masterAddress)
                    {
                        (new BoxData(p.OutBox)).SlaveReplicate(slave_box.GetDatabase()).Assert();
                    }
                }
                recycler.Packages.Clear();
            }
        }
    }
}
