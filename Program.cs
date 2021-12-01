using IBoxDB.LocalServer;
using ProjectIboxDb.Models;
using ProjectIboxDb.Services;
using System;
using System.IO;

namespace ProjectIboxDb
{
    class Program
    {
        static void Main(string[] args)
        {
            Service service = new Service();
            Boolean loop = true;

            using (var box = InitAndCreateAutoBox())
            {
                while (loop)
                {
                    var choice = Menu();
                    switch (choice)
                    {
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
            
        }

        public static int Menu()
        {
            Console.WriteLine("Menu");
            Console.WriteLine("1. Insert");
            Console.WriteLine("2. Update");
            Console.WriteLine("3. Delete");
            Console.WriteLine("4. Select");
            Console.Write("Choice: ");
            var choice = int.Parse(Console.ReadLine());

            return choice;
        }

        public static DB.AutoBox InitAndCreateAutoBox()
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ibox");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            var server = new DB(1, dbPath);
            var config = server.GetConfig();

            config.EnsureTable<Account>("Account", "Id");
            config.EnsureTable<Role>("Role", "Id");

            return server.Open();
        }
    }
}
