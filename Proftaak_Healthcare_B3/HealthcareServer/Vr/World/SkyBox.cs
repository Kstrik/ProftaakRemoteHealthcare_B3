using HealthcareServer.Vr.Actions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HealthcareServer.Vr.World
{
    public class SkyBox
    {
        private Session session;
        private int time;

        public SkyBox(int time, Session session)
        {
            this.time = time;
            this.session = session;
        }

        public async Task SetTime(int time)
        {
            this.time = time;

            JObject data = new JObject();
            data.Add("time", this.time);

            JObject setTime = new JObject();
            setTime.Add("id", "scene/skybox/settime");
            setTime.Add("data", data);

            await this.session.SendAction(this.session.GetTunnelSendRequest(setTime), new ActionRequest("tunnel/send", "scene/skybox/settime"));
        }
    }
}
