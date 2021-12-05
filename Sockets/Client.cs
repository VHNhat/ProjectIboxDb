using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProjectIboxDb.Sockets
{
    using static System.Console;
    class Client
    {
        public static string MessageCurrent = "Idle";
        public static int SendFile(string fName, string ipa)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipa);
                IPEndPoint end = new IPEndPoint(ip, 2014);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                string path = "";
                fName = fName.Replace("\\", "/");
                while (fName.IndexOf("/") > -1)
                {
                    path += fName.Substring(0, fName.IndexOf("/") + 1);
                    fName = fName.Substring(fName.IndexOf("/") + 1);
                }
                byte[] fNameByte = Encoding.ASCII.GetBytes(fName);

                WriteLine("---------------------------------------------");
                MessageCurrent = "Buffering...";
                WriteLine(MessageCurrent);
                byte[] fileData = File.ReadAllBytes(path + fName);
                byte[] clientData = new byte[4 + fNameByte.Length + fileData.Length];
                WriteLine("Data size: " + clientData.Length + "KB");
                byte[] fNameLen = BitConverter.GetBytes(fNameByte.Length);
                fNameLen.CopyTo(clientData, 0);
                fNameByte.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fNameByte.Length);
                MessageCurrent = "Connect to Server....";
                WriteLine(MessageCurrent);
                sock.Connect(end);
                MessageCurrent = "The File is sent.....";
                WriteLine(MessageCurrent);
                sock.Send(clientData);
                sock.Close();
                MessageCurrent = $"The file {fName} was sent...";
                WriteLine(MessageCurrent);
                WriteLine("---------------------------------------------");
                return clientData.Length;
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                WriteLine("---------------------------------------------");
                return -1;
            }
        }
    }
}
