using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareDoctor
{
    class DataManager
    {
        private List<TestClient> clients = new List<TestClient>();
        public DataManager()
        {

        }


        public List<TestClient> GetClients()
        {
            //test waarden
            TestClient client1 = new TestClient(1123421, "levi");
            TestClient client2 = new TestClient(2124, "test");
            TestClient client3 = new TestClient(1241243, "Kenley");

            clients.Add(client1);
            clients.Add(client2);
            clients.Add(client3);

            return clients;
        }



        public class TestClient
        {
            private int id;
            private string name;
           
            public TestClient(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public int GetId()
            {
                return id;
            }

            public string GetName()
            {
                return name;
            }
        }
    }
}
