using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket sock = null;
            try
            {
                // 소켓 생성
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 인터페이스 연결
                // 연결
                IPAddress addr = IPAddress.Parse("192.168.0.72");
                IPEndPoint iep = new IPEndPoint(addr, 10040);
                sock.Connect(iep);
            }
            catch(Exception e)
            {
                sock?.Close();
                Console.WriteLine(e.ToString());
                return;
            }

            string str;
            string str2;
            byte[] packet = new byte[1024];
            byte[] packet2 = new byte[1024];
            while (true)
            {
                Console.WriteLine("내용을 입력하세요");
                str = Console.ReadLine();
                MemoryStream ms = new MemoryStream(packet);
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(str);
                bw.Close();
                ms.Close();
                sock.Send(packet);

                if (str == "exit")
                    break;

                // 데이터 수신
                sock.Receive(packet2);
                MemoryStream ms2 = new MemoryStream(packet2);
                BinaryReader br = new BinaryReader(ms2);
                str2 = br.ReadString();
                Console.WriteLine("수신한 메시지 : {0}", str2);
                bw.Close();
                ms.Close();
            }

            sock.Close();
        }
    }
}
