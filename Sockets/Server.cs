using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ProjectIboxDb.Sockets
{
    using static System.Console;
    class Server
    {
        IPEndPoint end;
        Socket sock;
        public Server()
        {
            end = new IPEndPoint(IPAddress.Any, 2014);
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            sock.Bind(end);
        }

        public static string path = "E:/Code Life/UIT/CSDLPT/iboxDb/ProjectIboxDb/bin/Debug/netcoreapp3.1/ibox";
        public static string MessageCurrent = "Stopped";

        public void StartServer()
        {
            try
            {
                WriteLine("-----------------------------");
                MessageCurrent = "Starting...";
                sock.Listen(1000);
                MessageCurrent = "It Works and looks for files";
                WriteLine(MessageCurrent);
                Socket clientSock = sock.Accept();
                byte[] clientData = new byte[1024 * 5000];
                BinaryWriter write = null;
                int receiveByteLen;
                bool isFirstPacket = true;
                string fName = "NULL";
                MessageCurrent = "Receiving file ....";
                WriteLine(MessageCurrent);
                do
                {
                    receiveByteLen = clientSock.Receive(clientData);
                    WriteLine("byteLen: " + receiveByteLen);
                    if (isFirstPacket)
                    {
                        isFirstPacket = false;
                        int fNameLen = BitConverter.ToInt32(clientData, 0);
                        fName = Encoding.ASCII.GetString(clientData, 4, fNameLen);
                        Checked(fName);
                        write = new BinaryWriter(File.Open(path + "/" + fName, FileMode.Append));
                        write.Write(clientData, fNameLen + 4, receiveByteLen - 4 - fNameLen);
                    }
                    else
                    {
                        write.Write(clientData, 0, receiveByteLen);
                    }
                } while (receiveByteLen != 0);
                MessageCurrent = "Saving file....";
                WriteLine(MessageCurrent);
                write.Close();
                clientSock.Close();
                MessageCurrent = $"The file {fName} was Received";
                WriteLine(MessageCurrent);
                WriteLine("-----------------------------");
            }
            catch
            {
                MessageCurrent = "Error! File not received";
                WriteLine(MessageCurrent);
                WriteLine("-----------------------------");
            }
        }

        private void Checked(string fName)
        {
            string receiveFilePath = @$"E:\Code Life\UIT\CSDLPT\iboxDb\ProjectIboxDb\bin\Debug\netcoreapp3.1\ibox\{fName}";
            if(File.Exists(receiveFilePath))
                File.Delete(receiveFilePath);
        }
    }
}
