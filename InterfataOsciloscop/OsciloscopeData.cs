using System;


namespace InterfataOsciloscop
{
    class OsciloscopeData
    {
        public const int MarimeBufferTensiuni = 1000;
        public ushort[] Tensiuni;

        public OsciloscopeData()
        {
            Tensiuni = new ushort[MarimeBufferTensiuni];
        }
    }
}