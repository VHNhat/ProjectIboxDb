using IBoxDB.LocalServer;
using ProjectIboxDb.Models;
using ProjectIboxDb.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ProjectIboxDb.Services
{
    class Service
    {
        public Service()
        {
        }
        public int Menu()
        {
            Console.WriteLine("Enter table: ");
            Console.WriteLine("1. Account ");
            Console.WriteLine("2. Role ");
            Console.Write("Choice: ");
            var choice = int.Parse(Console.ReadLine());
            return choice;
        }
        public void Insert(DB.AutoBox box)
        {
            try
            {
                var choice = Menu();
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter account: ");
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        Console.Write("Role Id: ");
                        int roleId = int.Parse(Console.ReadLine());
                        using (var cube = box.Cube())
                        {
                            cube["Account"].Insert(new Account()
                            {
                                Id = cube.NewId(),
                                Username = username,
                                Password = password,
                                RoleId = roleId
                            });
                            var result = cube.Commit();
                            Console.WriteLine(result);
                        }
                        break;
                    case 2:
                        Console.WriteLine("Enter role: ");
                        Console.Write("Role name: ");
                        string roleName = Console.ReadLine();
                        Console.Write("Description: ");
                        string description = Console.ReadLine();
                        using (var cube = box.Cube())
                        {
                            cube["Role"].Insert(new Role()
                            {
                                Id = cube.NewId(),
                                RoleName = roleName,
                                Description = description
                            });
                            var result = cube.Commit();
                            Console.WriteLine(result);
                        }
                        break;
                    default:
                        break;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Update(DB.AutoBox box)
        {
            try
            {
                var choice = Menu();
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter account: ");
                        Console.Write("Id: ");
                        long AccountId = int.Parse(Console.ReadLine());
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        Console.Write("Role Id: ");
                        int accountRoleId = int.Parse(Console.ReadLine());
                        var account = box.Get<Account>("Account", AccountId);
                        account.Username = username;
                        account.Password = password;
                        account.RoleId = accountRoleId;
                        box.Update("Account", account);
                        break;
                    case 2:
                        Console.WriteLine("Enter role: ");
                        Console.Write("Id: ");
                        long roleId = int.Parse(Console.ReadLine());
                        Console.Write("Role name: ");
                        string roleName = Console.ReadLine();
                        Console.Write("Description: ");
                        string description = Console.ReadLine();
                        var role = box.Get<Role>("Role", roleId);
                        role.RoleName = roleName;
                        role.Description = description;
                        box.Update("Role", role);
                        break;
                    default:
                        break;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void Delete(DB.AutoBox box)
        {
            try
            {
                var choice = Menu();
                long id;
                switch (choice)
                {
                    case 1:
                        Console.Write("Id: ");
                        id = int.Parse(Console.ReadLine());
                        box.Delete("Account", id);
                        break;
                    case 2:
                        Console.Write("Id: ");
                        id = int.Parse(Console.ReadLine());
                        box.Delete("Role", id);
                        break;
                    default:
                        break;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Select(DB.AutoBox box)
        {

            var choice = Menu();
            Console.Write("Write your query: ");
            var query = Console.ReadLine();
            switch (choice)
            {
                case 1:
                    List<Account> accountObj = new List<Account>();
                    Console.WriteLine("With condition or not? 1.Yes 0.No");
                    Console.Write("Answer: ");
                    var withCon1 = int.Parse(Console.ReadLine());
                    if (withCon1 == 1) 
                    {
                        Console.Write("Enter quantity of condition: ");
                        var quantity = int.Parse(Console.ReadLine());
                        long id;string username, password; int roleId;
                        var arr = new ArrayList() { };
                        for(int i = 0; i < quantity; i++)
                        {
                            Console.Write("Enter condition: 1.Id | 2.Username | 3.Password | 4.Role ID: ");
                            var condChoice = int.Parse(Console.ReadLine());
                            switch (condChoice)
                            {
                                case 1:
                                    Console.Write("Id: ");
                                    id = long.Parse(Console.ReadLine());
                                    arr.Add(id);
                                    break;
                                case 2:
                                    Console.Write("Username: ");
                                    username = Console.ReadLine();
                                    arr.Add(username);
                                    break;
                                case 3:
                                    Console.Write("Password: ");
                                    password = Console.ReadLine();
                                    arr.Add(password);
                                    break;
                                case 4:
                                    Console.Write("Role Id: ");
                                    roleId = int.Parse(Console.ReadLine());
                                    arr.Add(roleId);
                                    break;
                            }
                        }
                        accountObj = box.Select<Account>(query,arr.ToArray());
                    }
                    else
                    {
                        accountObj = box.Select<Account>(query);
                    }
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(accountObj, options);
                    Console.WriteLine(jsonString);
                    break;
                case 2:
                    List<Role> roleObj = new List<Role>();
                    Console.WriteLine("With condition or not? 1.Yes 0.No");
                    Console.Write("Answer: ");
                    var withCon2 = int.Parse(Console.ReadLine());
                    if (withCon2 == 1)
                    {
                        Console.Write("Enter quantity of condition: ");
                        var quantity = int.Parse(Console.ReadLine());
                        long id; string roleName, description;
                        var arr = new ArrayList() { };
                        for (int i = 0; i < quantity; i++)
                        {
                            Console.Write("Enter condition: 1.Id | 2.RoleName | 3.Description: ");
                            var condChoice = int.Parse(Console.ReadLine());
                            switch (condChoice)
                            {
                                case 1:
                                    Console.Write("Id: ");
                                    id = long.Parse(Console.ReadLine());
                                    arr.Add(id);
                                    break;
                                case 2:
                                    Console.Write("RoleName: ");
                                    roleName = Console.ReadLine();
                                    arr.Add(roleName);
                                    break;
                                case 3:
                                    Console.Write("Description: ");
                                    description = Console.ReadLine();
                                    arr.Add(description);
                                    break;
                            }
                        }
                        roleObj = box.Select<Role>(query, arr.ToArray());
                    }
                    else
                    {
                        roleObj = box.Select<Role>(query);
                    }
                    var options1 = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString1 = JsonSerializer.Serialize(roleObj, options1);
                    Console.WriteLine(jsonString1);
                    break;
                default:
                    break;
            }

        }
    }
}
