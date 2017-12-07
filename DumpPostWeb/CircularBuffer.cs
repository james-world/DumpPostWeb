using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumpPostWeb
{
    public class CircularBuffer : IEnumerable<string>
    {
        private object gate = new object();
        private List<string> items;
        public int Length { get; }
        

        public CircularBuffer(int length)
        {
            Length = length;
            items = new List<string>(length);
        }

        public void Add(string item)
        {
            lock (gate)
            {
                items.Add(item);
                if (items.Count > Length)
                {
                    items.RemoveAt(0);
                }
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            lock (gate)
            {
                return items.ToList().AsReadOnly().AsEnumerable().GetEnumerator();
            }
            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
