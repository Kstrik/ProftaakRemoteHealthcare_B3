using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Networking.VrServer
{
    public class Packet : ISerializable
    {
        private Dictionary<string, object> items;

        public Packet()
        {
            this.items = new Dictionary<string, object>();
        }

        public void AddItem(string name, object value)
        {
            this.items.Add(name, value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach(string key in this.items.Keys)
            {
                object temp;
                info.AddValue(key, items.TryGetValue(key, out temp), items.TryGetValue(key, out temp).GetType());
            }
        }
    }
}
