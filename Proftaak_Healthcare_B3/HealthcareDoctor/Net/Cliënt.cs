using HealthcareDoctor.UI;
using Networking.HealthCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HealthcareDoctor.Net
{
    public class Cliënt
    {
        public ClientControl ClientControl;
        public string BSN;
        public string Name;

        private HealthCareDoctor healthcareDoctor;

        public Cliënt(string bsn, string name, HealthCareDoctor healthcareDoctor)
        {
            this.BSN = bsn;
            this.Name = name;
            this.ClientControl = new ClientControl(OnSendResistance, OnSendMessage, OnStartSession, OnStopSession, this.BSN);
            this.healthcareDoctor = healthcareDoctor;
        }

        private void OnSendResistance(int resistance, string bsn)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)resistance);
            bytes.AddRange(Encoding.UTF8.GetBytes(bsn));
            this.healthcareDoctor.Transmit(new Message(false, Message.MessageType.CHANGE_RESISTANCE, bytes.ToArray()));
        }

        private void OnSendMessage(string text, string bsn)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)text.Length);
            bytes.AddRange(Encoding.UTF8.GetBytes(text));
            bytes.AddRange(Encoding.UTF8.GetBytes(bsn));
            this.healthcareDoctor.Transmit(new Message(false, Message.MessageType.CHAT_MESSAGE, bytes.ToArray()));
        }

        private void OnStartSession(string bsn)
        {
            this.healthcareDoctor.Transmit(new Message(false, Message.MessageType.START_SESSION, Encoding.UTF8.GetBytes(this.BSN)));
        }

        private void OnStopSession(string bsn)
        {
            this.healthcareDoctor.Transmit(new Message(false, Message.MessageType.STOP_SESSION, Encoding.UTF8.GetBytes(this.BSN)));
        }

        public void HandleBikeData(List<byte> bytes)
        {
            for (int i = 0; i < bytes.Count; i += 20)
            {
                int value = bytes[i + 1];

                switch ((Message.ValueId)bytes[i])
                {
                    case Message.ValueId.HEARTRATE:
                        {
                            this.ClientControl.Heartrate = value;
                            this.ClientControl.UpdateChart(value);
                            break;
                        }
                    case Message.ValueId.DISTANCE:
                        {
                            this.ClientControl.Distance = value;
                            break;
                        }
                    case Message.ValueId.SPEED:
                        {
                            this.ClientControl.Speed = value;
                            break;
                        }
                    case Message.ValueId.CYCLE_RHYTHM:
                        {
                            this.ClientControl.CycleRhythm = value;
                            break;
                        }
                }
            }
        }

        public void StartSessionOK()
        {

        }

        public void StartSessionError()
        {
            this.ClientControl.GetToggleButton().Content = "Start Session";
            MessageBox.Show("Unable to start session!");
        }

        public void StopSessionOk()
        {

        }

        public void StopSessionError()
        {
            this.ClientControl.GetToggleButton().Content = "Stop Session";
            MessageBox.Show("Unable to stop session!");
        }
    }
}
