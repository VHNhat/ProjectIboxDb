using IBoxDB.LocalServer;
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
            if (option == 1)
            {
                box = InitAndCreateAutoBox(slaveAddress);
            }
            else if(option == 2)
            {
                box = InitAndCreateAutoBox(masterAddress);
                slave_box = 
            }
            else
            {
                Console.WriteLine("Invalid choice");
                return;
            }

            while (loop)
            {
                var choice = Menu();
                switch (choice)
                {
                    case 0:
                        break;
                    case 1:
                        service.Insert(box);
                        break;
                    case 2:
                        service.Update(box);
                        break;
                    case 3:
                        service.Delete(box);
                        break;
                    case 4:
                        service.Select(box);
                        break;
                    case 5:
                        Distribute(option,masterAddress,box);
                        return;
                    case 6:
                        option = Setting();
                        break;
                    default:
                        Console.WriteLine("Choice invalid!");
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
            Console.Write("Enter DB address: ");
            masterAddress = long.Parse(Console.ReadLine());
            slaveAddress = masterAddress * -1;
            return option;
        }
        private static void Distribute(int option, long addressDb, DB.AutoBox box)
        {
            Console.WriteLine("-----------------Distribute Data-----------------");
            Console.Write("Enter DB address: ");
            addressDb = long.Parse(Console.ReadLine());
            if (option == 1)
            {
                box.GetDatabase().Dispose();
                Server server = new Server();
                server.StartServer();
            }
            else if (option == 2)
            {
                Client client = new Client();
                Console.Write("Enter server IP address: ");
                string ipa = Console.ReadLine();
                box.GetDatabase().Dispose();
                Client.SendFile($"E:/Code Life/UIT/CSDLPT/iboxDb/ProjectIboxDb/bin/Debug/netcoreapp3.1/ibox/db{addressDb}.box", ipa);
            }
        }

        public static int Menu()
        {
            Console.WriteLine("Menu");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Insert");
            Console.WriteLine("2. Update");
            Console.WriteLine("3. Delete");
            Console.WriteLine("4. Select");
            Console.WriteLine("5. Distribute data");
            Console.WriteLine("6. Setting");
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
    }
}
