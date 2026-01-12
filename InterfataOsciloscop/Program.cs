using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfataOsciloscop
{
    internal static class Program
    {
        private const bool simulateUart = false;        //IMPORTANT: change this to choose between simulated and real UART

        private static bool shouldRun = true;

        /// <summary>
        /// The thread used to simulate debug values without a UART connection.
        /// </summary>
        static void SimulatedUartThread()
        {
            while (shouldRun)
            {
                ProgramData.Instance.SimulateDebugValues();
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// The thread used to read debug values being received through a physical UART.
        /// </summary>
        static void UartThread()
        {
            while (shouldRun)
            {
                ProgramData.Instance.ReadDebugValuesFromUart();
                //Thread.Sleep(0);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread dataThread;
            if (simulateUart == true)
            {
                dataThread = new Thread(new ThreadStart(SimulatedUartThread));
            }
            else
            {
                dataThread = new Thread(new ThreadStart(UartThread));
            }

            //thread nou pentru uart
            dataThread.Start();
            Application.Run(new Form1());
            shouldRun = false;
            dataThread.Join();
        }
    }
}
