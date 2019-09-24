using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class Model
    {
        public string Filename { get; }
        public bool CullbackFaces { get; }
        public bool Animated { get; }
        public string AnimationName { get; }

        public Model(string filename, bool cullbackFaces, string animationName)
        {
            this.Filename = filename;
            this.CullbackFaces = cullbackFaces;
            this.AnimationName = animationName;
            this.Animated = (String.IsNullOrEmpty(animationName)) ? false : true;
        }

        public Model(string filename, bool cullbackFaces)
            : this(filename, cullbackFaces, "") { }

        public JObject GetJsonObject()
        {
            JObject model = new JObject();
            model.Add("file", this.Filename);
            model.Add("cullbackfaces", this.CullbackFaces);
            model.Add("animated", this.Animated);

            if (this.Animated && !String.IsNullOrEmpty(this.AnimationName))
            {
                model.Add("animation", this.AnimationName);
            }

            return model;
        }
    }
}
