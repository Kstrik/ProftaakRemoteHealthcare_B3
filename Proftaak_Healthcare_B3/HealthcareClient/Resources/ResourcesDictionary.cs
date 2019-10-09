using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.Resources
{
    public class ResourcesDictionary
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string resourcesFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Resources");

        public static Dictionary<string, string> ModelResources = new Dictionary<string, string>()
        {
            { @"data\NetworkEngine\models\bike\Bike.fbx", resourcesFolderPath + @"\Models\Bike\bike.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree1.obj", resourcesFolderPath + @"\Models\Trees\tree1.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree2.obj", resourcesFolderPath + @"\Models\Trees\tree2.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree3.obj", resourcesFolderPath + @"\Models\Trees\tree3.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree4.obj", resourcesFolderPath + @"\Models\Trees\tree4.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree5.obj", resourcesFolderPath + @"\Models\Trees\tree5.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree6.obj", resourcesFolderPath + @"\Models\Trees\tree6.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree7.obj", resourcesFolderPath + @"\Models\Trees\tree7.obj" },
            { @"data\NetworkEngine\models\trees\fantasy\tree10.obj", resourcesFolderPath + @"\Models\Trees\tree10.obj" }
        };

        public static Dictionary<string, string> DiffuseTextureResources = new Dictionary<string, string>()
        {
            { @"data\NetworkEngine\textures\tarmac_diffuse.png", resourcesFolderPath + @"\Textures\Diffuse\tarmac_diffuse.png" },
            { @"data\NetworkEngine\textures\terrain\adesert_cracks_d.jpg", resourcesFolderPath + @"\Textures\Diffuse\adesert_cracks_d.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn_d.jpg", resourcesFolderPath + @"\Textures\Diffuse\adesert_mntn_d.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn2_d.jpg", resourcesFolderPath + @"\Textures\Diffuse\adesert_mntn2_d.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn2o_d.jpg", resourcesFolderPath + @"\Textures\Diffuse\adesert_mntn2o_d.jpg" }
        };

        public static Dictionary<string, string> NormalTextureResources = new Dictionary<string, string>()
        {
            { @"data\NetworkEngine\textures\tarmac_normal.png", resourcesFolderPath + @"\Textures\Normal\tarmac_normal.png" },
            { @"data\NetworkEngine\textures\terrain\adesert_cracks_n.jpg", resourcesFolderPath + @"\Textures\Normal\adesert_cracks_n.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn_n.jpg", resourcesFolderPath + @"\Textures\Normal\adesert_mntn_n.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn2_n.jpg", resourcesFolderPath + @"\Textures\Normal\adesert_mntn2_n.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn2o_n.jpg", resourcesFolderPath + @"\Textures\Normal\adesert_mntn2o_n.jpg" }
        };

        public static Dictionary<string, string> SpecularTextureResources = new Dictionary<string, string>()
        {
            { @"data\NetworkEngine\textures\tarmac_specular.png", resourcesFolderPath + @"\Textures\Specular\tarmac_specular.png" },
            { @"data\NetworkEngine\textures\terrain\adesert_cracks_s.jpg", resourcesFolderPath + @"\Textures\Specular\adesert_cracks_s.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn_s.jpg", resourcesFolderPath + @"\Textures\Specular\adesert_mntn_s.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn2_s.jpg", resourcesFolderPath + @"\Textures\Specular\adesert_mntn2_s.jpg" },
            { @"data\NetworkEngine\textures\terrain\adesert_mntn2o_s.jpg", resourcesFolderPath + @"\Textures\Specular\adesert_mntn2o_s.jpg" }
        };

        public static List<string> HeightMaps = new List<string>()
        {
            resourcesFolderPath + @"\Textures\Heightmaps\HeightMap.jpeg",
        };
    }
}
