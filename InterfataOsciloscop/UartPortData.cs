using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfataOsciloscop
{
    enum ModulesHeader
    {
        VOLTAGES = 10
    }
    class UartPortData
    {
        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        public static SerialPort Port = new SerialPort();
        public static bool shouldBeOpen = false;
        public static string comPort = "COM1";
        public static int baudrate = 115200;
        public static string error = "";
        public static int mesajeTotale = 0;
        public static int octetiTotali = 0;
        public static int mesajeValide = 0;
        public static long timpUltimMesaj = 0;
        public static int headereInvalide = 0;
        public static int crcGresite = 0;
        public static int moduleInvalide = 0;
        public static int eroriInvalide = 0;
        public static int dateInvalide = 0;
        public static int octetiPierduti = 0;
        //uart buffer variables
        private const int BUFFER_SIZE = 3000;
        public static BufferCircular bufferUart = new BufferCircular(BUFFER_SIZE);
        private static byte[] bufferTemporar = new byte[18];
        private static byte calculateCRC(uint length, byte[] buffer)
        {
            uint divisor = 0x8D, dividend;
            int i, j;

            dividend = ((uint)buffer[0] << 8) | buffer[1];
            for (j = 15; j >= 8; j--)
                if ((dividend & (1 << j)) != 0)
                    dividend ^= divisor << (j - 8);

            for (i = 2; i < length; i++)
            {
                dividend = (dividend << 8) | buffer[i];

                for (j = 15; j >= 8; j--)
                    if ((dividend & (1 << j)) != 0)
                        dividend ^= divisor << (j - 8);
            }
            return (byte)(dividend & 0xFF);
        }

        private static void ParseBuffer()
        {
            bool redoParse = true;
            while (redoParse && !bufferUart.IsEmpty())
            {
                redoParse = false;
                switch (bufferUart.GetItem(0))
                {
                    case (byte)ModulesHeader.VOLTAGES:
                        if (bufferUart.ItemsInside() >= 18)
                        {
                            for (uint i = 0; i < 18; i++)
                            {
                                bufferTemporar[i] = bufferUart.GetItem(i);
                            }
                            if (calculateCRC(18, bufferTemporar) == 0)
                            {
                                ushort[] tensiuniPrimite = new ushort[10];
                                ushort indexTensiuniPrimite = bufferTemporar[1];
                                tensiuniPrimite[0] = (ushort)((((uint)bufferTemporar[2]) << 4) + (((uint)bufferTemporar[3]) >> 4));
                                tensiuniPrimite[1] = (ushort)(((((uint)bufferTemporar[3]) % 0x10) << 8) + (((uint)bufferTemporar[4])));
                                tensiuniPrimite[2] = (ushort)((((uint)bufferTemporar[5]) << 4) + (((uint)bufferTemporar[6]) >> 4));
                                tensiuniPrimite[3] = (ushort)(((((uint)bufferTemporar[6]) % 0x10) << 8) + (((uint)bufferTemporar[7])));
                                tensiuniPrimite[4] = (ushort)((((uint)bufferTemporar[8]) << 4) + (((uint)bufferTemporar[9]) >> 4));
                                tensiuniPrimite[5] = (ushort)(((((uint)bufferTemporar[9]) % 0x10) << 8) + (((uint)bufferTemporar[10])));
                                tensiuniPrimite[6] = (ushort)((((uint)bufferTemporar[11]) << 4) + (((uint)bufferTemporar[12]) >> 4));
                                tensiuniPrimite[7] = (ushort)(((((uint)bufferTemporar[12]) % 0x10) << 8) + (((uint)bufferTemporar[13])));
                                tensiuniPrimite[8] = (ushort)((((uint)bufferTemporar[14]) << 4) + (((uint)bufferTemporar[15]) >> 4));
                                tensiuniPrimite[9] = (ushort)(((((uint)bufferTemporar[15]) % 0x10) << 8) + (((uint)bufferTemporar[16])));

                                for (int i = 0; i < 10; i++) {
                                    if(indexTensiuniPrimite == 0)
                                    {
                                        mesajeValide++;
                                    }
                                    ProgramData.Instance.Data.Tensiuni[i + indexTensiuniPrimite * 10] = tensiuniPrimite[i];
                                }
                                mesajeValide++;
                                bufferUart.ScoateValori(18);
                                redoParse = true;
                                mesajeTotale++;
                            }
                            else
                            {
                                crcGresite++;
                                bufferUart.ScoateValori(1);
                                redoParse = true;
                            }
                        }
                        break;
                    default:
                        headereInvalide++;
                        bufferUart.ScoateValori(1);
                        redoParse = true;
                        break;
                }
            }
        }
        public static void Update()
        {
            if (shouldBeOpen)
            {
                try
                {
                    if (Port.IsOpen)
                    {
                        int OctetiDisponibiliBuffer = Port.BytesToRead;
                        if (OctetiDisponibiliBuffer > 0)
                        {
                            //citirea si mutarea din bufferul de serial in bufferul circular local
                            while (OctetiDisponibiliBuffer != 0)
                            {
                                if (!bufferUart.IsFull())
                                {
                                    bufferUart.InsereazaValoare((byte)Port.ReadByte());
                                    octetiTotali++;
                                    OctetiDisponibiliBuffer--;
                                }
                                else
                                {
                                    OctetiDisponibiliBuffer = 0;
                                    octetiTotali += Port.BytesToRead;
                                    octetiPierduti += Port.BytesToRead;
                                    Port.DiscardInBuffer();
                                }
                            }
                            //parsare valori primite
                            ParseBuffer();
                            watch.Restart();
                        }
                        else
                        {
                            timpUltimMesaj = watch.ElapsedMilliseconds;
                        }
                    }
                    else
                    {
                        Port.BaudRate = baudrate;
                        Port.PortName = comPort;
                        Port.Open();
                        Port.DiscardInBuffer();
                        watch.Restart();
                    }
                }
                catch (Exception ex)
                {
                    shouldBeOpen = false;
                    error = ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.InnerException;
                    timpUltimMesaj = watch.ElapsedMilliseconds;
                }
            }
            else
            {
                if (Port.IsOpen)
                {
                    Port.Close();
                }
                timpUltimMesaj = 0;
                watch.Stop();
            }
        }
    }
}