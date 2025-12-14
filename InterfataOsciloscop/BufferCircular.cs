using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfataOsciloscop
{
    class BufferCircular
    {
        private byte[] buffer;
        private int lungime, cap, coada;
        public BufferCircular(int marime)
        {
            buffer = new byte[marime];
            lungime = marime;
            cap = 0;
            coada = 0;
        }

        public byte GetItem(uint index)
        {
            if (index < ItemsInside())
            {
                int indexReal = (int)((index + cap) % lungime);
                return buffer[indexReal];
            }
            else
            {
                return 0;
            }
        }

        public bool IsFull()
        {
            if ((coada == cap - 1) || ((coada == lungime - 1) && (cap == 0)))
            {
                return true;
            }
            return false;
        }

        public bool IsEmpty()
        {
            if (coada == cap)
            {
                return true;
            }
            return false;
        }

        public int ItemsInside()
        {
            if (cap <= coada)
            {
                return coada - cap;
            }
            return lungime - cap + coada;
        }

        public void InsereazaValoare(byte b)
        {
            if (!IsFull())//daca nu e plin bufferul
            {
                buffer[coada] = b;
                coada++;
                if (coada >= lungime)
                {
                    coada = 0;
                }
            }
        }

        public void ScoateValori(int nrValori)
        {
            for (int i = 0; i < nrValori; i++)
            {
                if (!IsEmpty())
                {
                    buffer[cap] = 0;
                    cap++;
                    if (cap >= lungime)
                    {
                        cap = 0;
                    }
                }
            }
        }

        public int Cap()
        {
            return cap;
        }

        public int Coada()
        {
            return coada;
        }

        public void PrintBuffer()
        {
            Console.WriteLine("cap " + cap + " coada " + coada);
        }
    }
}