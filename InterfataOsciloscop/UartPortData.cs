using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfataOsciloscop
{
    enum ModulesHeader  /*IMPORTANT - valorile sunt folosite intr-un calcul de index mai jos, nu le schimba*/
    {
        VOLTAGES_0 = 10,
        VOLTAGES_1 = 11,
        VOLTAGES_2 = 12,
        VOLTAGES_3 = 13,
        VOLTAGES_4 = 14,
        VOLTAGES_5 = 15,
        VOLTAGES_6 = 16,
        VOLTAGES_7 = 17,
        VOLTAGES_8 = 18,
        VOLTAGES_9 = 19
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
        private const int BUFFER_SIZE = 200;
        public static BufferCircular bufferUart = new BufferCircular(BUFFER_SIZE);
        private static byte[] bufferTemporar = new byte[17];
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
                    case (byte)ModulesHeader.VOLTAGES_0:
                    case (byte)ModulesHeader.VOLTAGES_1:
                    case (byte)ModulesHeader.VOLTAGES_2:
                    case (byte)ModulesHeader.VOLTAGES_3:
                    case (byte)ModulesHeader.VOLTAGES_4:
                    case (byte)ModulesHeader.VOLTAGES_5:
                    case (byte)ModulesHeader.VOLTAGES_6:
                    case (byte)ModulesHeader.VOLTAGES_7:
                    case (byte)ModulesHeader.VOLTAGES_8:
                    case (byte)ModulesHeader.VOLTAGES_9:
                        if (bufferUart.ItemsInside() >= 17)
                        {
                            for (uint i = 0; i < 17; i++)
                            {
                                bufferTemporar[i] = bufferUart.GetItem(i);
                            }
                            if (calculateCRC(17, bufferTemporar) == 0)
                            {
                                ushort[] tensiuniPrimite = new ushort[10];
                                tensiuniPrimite[0] = (ushort)((((uint)bufferTemporar[1]) << 4) + (((uint)bufferTemporar[2]) >> 4));
                                tensiuniPrimite[1] = (ushort)(((((uint)bufferTemporar[2]) % 0x10) << 8) + (((uint)bufferTemporar[3])));
                                tensiuniPrimite[2] = (ushort)((((uint)bufferTemporar[4]) << 4) + (((uint)bufferTemporar[5]) >> 4));
                                tensiuniPrimite[3] = (ushort)(((((uint)bufferTemporar[5]) % 0x10) << 8) + (((uint)bufferTemporar[6])));
                                tensiuniPrimite[4] = (ushort)((((uint)bufferTemporar[7]) << 4) + (((uint)bufferTemporar[8]) >> 4));
                                tensiuniPrimite[5] = (ushort)(((((uint)bufferTemporar[8]) % 0x10) << 8) + (((uint)bufferTemporar[9])));
                                tensiuniPrimite[6] = (ushort)((((uint)bufferTemporar[10]) << 4) + (((uint)bufferTemporar[11]) >> 4));
                                tensiuniPrimite[7] = (ushort)(((((uint)bufferTemporar[11]) % 0x10) << 8) + (((uint)bufferTemporar[12])));
                                tensiuniPrimite[8] = (ushort)((((uint)bufferTemporar[13]) << 4) + (((uint)bufferTemporar[14]) >> 4));
                                tensiuniPrimite[9] = (ushort)(((((uint)bufferTemporar[14]) % 0x10) << 8) + (((uint)bufferTemporar[15])));

                                for (int i = 0; i < 10; i++) {
                                    ProgramData.Instance.Data.Tensiuni[i + (bufferTemporar[0]-10) * 10] = tensiuniPrimite[i];
                                }
                                mesajeValide++;
                                bufferUart.ScoateValori(17);
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