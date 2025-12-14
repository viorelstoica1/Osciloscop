using System;
using System.IO.Ports;


namespace InterfataOsciloscop
{
    class ProgramData
    {
        private static readonly ProgramData instance = new ProgramData();
        public OsciloscopeData Data;
        static Random rand = new Random();
        static int valoareAdaugata = 0;

        private ProgramData()
        {
            Data = new OsciloscopeData();
        }
        public static ProgramData Instance
        {
            get
            {
                return instance;
            }
        }
        public void SimulateDebugValues()
        {
            for(int i=0; i<OsciloscopeData.MarimeBufferTensiuni; i++)
            {
                Instance.Data.Tensiuni[i] = (ushort)((i*100 + valoareAdaugata) % 4095);
            }
            valoareAdaugata+=10;
        }
        public void ReadDebugValuesFromUart()
        {
            UartPortData.Update();
        }
    }
}