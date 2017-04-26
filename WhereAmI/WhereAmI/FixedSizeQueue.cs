using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereAmI
{
    public class FixedSizeQueue<T> : Queue<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public FixedSizeQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    base.Dequeue();
                }
            }
        }
    }
}
