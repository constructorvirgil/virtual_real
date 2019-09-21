using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UnityClientA
{
    class ProAnalyser
    {
        const byte START = 0x0e;
        const UInt16 OPS_VERIFY = 0x0000;
        const UInt16 OPS_SET = 0x0100;
        const UInt16 OPS_RESET = 0x0101;
        const UInt16 OPS_QUERY = 0x0102;
        const UInt16 OPR_QUERY = 0x0200;
        const byte TYPE_UNITY = 0x00;
        const byte TYPE_STM = 0x01;
        const byte LEN_VERIFY = 0x02;
        const byte LEN_NORMAL = 0x0c;
        const byte LEN_QUERY = 0x02;
        const byte MYINDEX = 0x00;

        public byte start;
        public UInt16 op;
        public byte len;
        public byte from;
        public byte to;
        public byte[] io = new byte[10];
        public UInt16 crc;
        public byte type;
        public byte index;

        public ProAnalyser()
        {

        }

        public int Decode(byte[] t_buf)
        {
            start = t_buf[0];
            op = t_buf[1];
            op <<= 8;
            op |= t_buf[2];
            len = t_buf[3];

            if (op == OPR_QUERY)
            {
                from = t_buf[4];
                to = t_buf[5];
                int i = 0;
                for (; i < len - 2; i++)
                {
                    io[i] = t_buf[6 + i];
                }
                crc = t_buf[6 + i];
                crc <<= 8;
                crc |= t_buf[6 + i + 1];
            }

            return 0;
        }

        public int OpVerifyInit()
        {
            this.start = START;
            this.op = OPS_VERIFY;
            this.len = LEN_VERIFY;
            this.type = TYPE_UNITY;
            this.index = MYINDEX;
            return 0;
        }
        public int OpSetInit(byte[] io, int wcStm)
        {
            this.start = START;
            this.op = OPS_SET;
            this.len = LEN_NORMAL;
            this.from = MYINDEX;
            this.to = (byte)wcStm;
            this.io = io;
            return 0;
        }
        public int OpResetInit(byte[] io, int wcStm)
        {
            this.start = START;
            this.op = OPS_RESET;
            this.len = LEN_NORMAL;
            this.from = MYINDEX;
            this.to = (byte)wcStm;
            this.io = io;
            return 0;
        }

        public int OpQueryInit(int wcStm)
        {
            this.start = START;
            this.op = OPS_QUERY;
            this.len = LEN_QUERY;
            this.from = MYINDEX;
            this.to = (byte)wcStm;
            return 0;
        }
        public byte[] Encode()
        {
            if (this.op == OPS_VERIFY)
            {
                byte[] buf = new byte[8];
                buf[0] = this.start;
                buf[1] = (byte)((this.op & 0xff00) >> 8);
                buf[2] = (byte)(0x00FF & this.op);
                buf[3] = this.len;
                buf[4] = type;
                buf[5] = index;
                ushort check = crc_check(buf, buf.Length - 2);
                buf[6] = (byte)((check & ((ushort)(0xFF00))) >> 8);
                buf[7] = (byte)(check & ((ushort)(0x00FF)));
                return buf;
            }
            else if (this.op == OPS_SET)
            {
                byte[] buf = new byte[18];
                buf[0] = this.start;
                buf[1] = (byte)((this.op & 0xff00) >> 8);
                buf[2] = (byte)(0x00FF & this.op);
                buf[3] = this.len;
                buf[4] = from;
                buf[5] = to;
                int i = 0;
                for (; i < this.len - 2; i++)
                {
                    buf[6 + i] = this.io[i];
                }
                ushort check = crc_check(buf, buf.Length - 2);
                buf[6 + i] = (byte)((check & ((ushort)(0xFF00))) >> 8);
                buf[6 + i + 1] = (byte)(check & ((ushort)(0x00FF)));
                return buf;
            }
            else if (this.op == OPS_RESET)
            {
                byte[] buf = new byte[18];
                buf[0] = this.start;
                buf[1] = (byte)((this.op & 0xff00) >> 8);
                buf[2] = (byte)(0x00FF & this.op);
                buf[3] = this.len;
                buf[4] = from;
                buf[5] = to;
                int i = 0;
                for (; i < this.len - 2; i++)
                {
                    buf[6 + i] = this.io[i];
                }
                ushort check = crc_check(buf, buf.Length - 2);
                buf[6 + i] = (byte)((check & ((ushort)(0xFF00))) >> 8);
                buf[6 + i + 1] = (byte)(check & ((ushort)(0x00FF)));
                return buf;
            }
            else if (this.op == OPR_QUERY)
            {
                byte[] buf = new byte[18];
                buf[0] = this.start;
                buf[1] = (byte)((this.op & 0xff00) >> 8);
                buf[2] = (byte)(0x00FF & this.op);
                buf[3] = this.len;
                buf[4] = from;
                buf[5] = to;
                int i = 0;
                for (; i < this.len - 2; i++)
                {
                    buf[6 + i] = this.io[i];
                }
                ushort check = crc_check(buf, buf.Length - 2);
                buf[6 + i] = (byte)((check & ((ushort)(0xFF00))) >> 8);
                buf[6 + i + 1] = (byte)(check & ((ushort)(0x00FF)));
                return buf;
            }

            return null;
        }
        //CRC  Table
        private static ushort[] crc16_table = new ushort[256] {
        0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
        0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
        0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
        0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
        0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
        0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
        0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
        0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
        0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
        0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
        0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
        0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
        0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
        0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
        0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
        0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
        0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
        0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
        0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
        0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
        0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
        0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
        0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
        0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
        0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
        0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
        0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
        0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
        0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
        0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
        0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
        0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
        };

        public static ushort crc_check(byte[] data, int length)
        {
            ushort crc_reg = 0xFFFF;
            int i = 0;
            while (length-- != 0)
            {
                crc_reg = (ushort)((crc_reg >> 8) ^ crc16_table[(crc_reg ^ data[i++]) & 0xFF]);
            }
            return (ushort)(~crc_reg);
        }


    };
    public class UnityClient
    {
        public UnityClient() { }
        public const int BUF_SIZE = 256;
        private Socket lisSock;
        public byte[] recvBuf = new byte[BUF_SIZE];
        private byte[] io = new byte[10];
        private Task inforHandleTask;
        private object mtx = new object();
        private bool isUpdate = false;

        private void InforHandle()
        {
            while (true)
            {

                try
                {
                    lisSock.Receive(recvBuf, BUF_SIZE, SocketFlags.None);
                }
                catch (Exception)
                {
                    return;
                }
                ProAnalyser proAnalyser = new ProAnalyser();
                ushort check = ProAnalyser.crc_check(recvBuf, recvBuf[3] + 4);

                if (((check & (ushort)(0xFF00)) >> 8) != recvBuf[recvBuf[3] + 4] ||
                     (check & (ushort)(0x00FF)) != recvBuf[recvBuf[3] + 4 + 1])
                {
                    //Incorrect CRC , discard ...
                    continue;
                }
                proAnalyser.Decode(recvBuf);

                lock (mtx)
                {
                    io = proAnalyser.io;
                    isUpdate = true;
                }


            }
        }

        public int Init(string ip, int port)
        {
            if (lisSock != null) return -1;

            lisSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                lisSock.Connect(ipEndPoint);
            }
            catch (SocketException)
            {
                return -1;
            }
            Verify();

            inforHandleTask = Task.Factory.StartNew(() => this.InforHandle(), TaskCreationOptions.LongRunning);

            return 0;
        }

        private int Verify()
        {

            ProAnalyser proAnalyserSend = new ProAnalyser();
            proAnalyserSend.OpVerifyInit();
            try
            {
                lisSock.Send(proAnalyserSend.Encode());
            }
            catch (Exception)
            {
                return -1;
            }

            return 0;
        }

        public int SetBit(int wcStm, int wcBit)
        {
            ProAnalyser proAnalyserSend = new ProAnalyser();
            byte[] io = new byte[10];
            io[wcBit / 8] = (byte)(0x80 >> (wcBit % 8));
            proAnalyserSend.OpSetInit(io, wcStm);
            try
            {
                lisSock.Send(proAnalyserSend.Encode());
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;

        }

        public int ResetBit(int wcStm, int wcBit)
        {
            ProAnalyser proAnalyserSend = new ProAnalyser();
            byte[] io = new byte[10];
            io[wcBit / 8] = (byte)(0x80 >> (wcBit % 8));
            proAnalyserSend.OpResetInit(io, wcStm);
            try
            {
                lisSock.Send(proAnalyserSend.Encode());
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;
        }

        public byte[] GetBits(int wcStm)
        {
            ProAnalyser proAnalyserSend = new ProAnalyser();
            proAnalyserSend.OpQueryInit(wcStm);
            try
            {
                lisSock.Send(proAnalyserSend.Encode());
            }
            catch (Exception)
            {
                return null;
            }

            if (isUpdate == true)
            {
                lock (mtx)
                {
                    isUpdate = false;
                    return this.io;
                }
            }
            return null;
        }

        public void Close()
        {
            lock (mtx)
            {
                lisSock.Shutdown(SocketShutdown.Both);
                lisSock.Close();
                lisSock = null;
            }
        }
    };

}
