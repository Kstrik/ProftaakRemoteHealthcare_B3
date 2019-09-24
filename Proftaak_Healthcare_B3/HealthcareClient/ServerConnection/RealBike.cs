using System;
using System.Collections.Generic;
using System.Threading;
using Avans.TI.BLE;

namespace HealthcareClient.Bike
{
    class RealBike : Bike
    {
        public string ModelNumber;

        public RealBike(string ModelNumber, IBikeDataReceiver bikeDataReceiver)
            : base(bikeDataReceiver)
        {
            this.ModelNumber = ModelNumber;
            ThreadStart bikeStart = new ThreadStart(ConnectToBike);
            Thread bikeThread = new Thread(bikeStart);
            bikeThread.Start();
        }

        private async void ConnectToBike()
        {
            Console.WriteLine("Starting connection to bike");
            int errorCode = 0;
            BLE bleBike = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux " + ModelNumber);

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            ReceivedData(e.Data);
        }

        public override void ReceivedData(byte[] data)
        {
            base.ReceivedData(data);
        }

        public override bool ToggleListening()
        {
            throw new NotImplementedException();
        }

        public override bool StartListening()
        {
            throw new NotImplementedException();
        }

        public override bool StopListening()
        {
            throw new NotImplementedException();
        }
    }
}
