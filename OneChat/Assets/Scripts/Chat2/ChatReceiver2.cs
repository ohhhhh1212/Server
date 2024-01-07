using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ChatReceiver2
{
    [Serializable]
    public class PacketData
    {
        public short packetId { get; set; } // Packet Type
        public short size { get; set; } // Packet Type

        public PacketData() { }
        public PacketData(short nPacketId, short nSize)
        {
            this.packetId = nPacketId;
            size = nSize;
        }
    }

    [Serializable]
    public class ReceiveData : PacketData
    {
        public byte[] data { get; set; }

        public ReceiveData(short nPacketId, short size, byte[] kdata) : base(nPacketId, size)
        {
            data = kdata;
        }
        public ReceiveData() { }
    }

    public event EventHandler<ReceiveData> OnChatReceiveHandeler = null;
    public const int HeaderSize = 4;

    public string ipStr { get; private set; }
    public int port { get; private set; }

    Socket m_Socket = null;

    public bool isActive { get; set; } = true;

    public ChatReceiver2(string sIp, int nPort)
    {
        ipStr = sIp;
        port = nPort;
    }

    public bool Start()
    {
        try
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iepoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
            m_Socket.Bind(iepoint);
            m_Socket.Listen(5);

            AcceptLoopAsync();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    public void CloseSocket()
    {
        m_Socket?.Close();
    }

    delegate void AcceptDele();
    private void AcceptLoopAsync()
    {
        AcceptDele dele = AssetLoop;
        dele.BeginInvoke(null, null);
    }

    private void AssetLoop()
    {
        Socket doSocket = null;

        while (isActive)
        {
            doSocket = m_Socket.Accept();
            DoItAsync(doSocket);
        }
    }

    delegate void DoItDele(Socket doSocket);
    private void DoItAsync(Socket doSocket)
    {
        DoItDele dele = DoIt;
        dele.BeginInvoke(doSocket, null, null);
    }

    private void DoIt(Socket doSocket)
    {
        byte[] packet = new byte[1024];

        doSocket.Receive(packet);
        doSocket.Close();

        MemoryStream ms = new MemoryStream(packet);
        BinaryReader br = new BinaryReader(ms);

        short nPacketId = br.ReadInt16();
        short nSize = br.ReadInt16();

        int nbodySize = nSize - (sizeof(short) * 2);

        byte[] body = br.ReadBytes(nbodySize);

        br.Close();
        ms.Close();

        // Thread 때문에 UI가 동작하지 않는다.
        if (OnChatReceiveHandeler != null)
        {
            OnChatReceiveHandeler(this, new ReceiveData(nPacketId, nSize, body));
        }
    }
}
