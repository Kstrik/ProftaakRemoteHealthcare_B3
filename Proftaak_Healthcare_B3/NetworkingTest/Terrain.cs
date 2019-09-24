using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTest
{
    public class Terrain
    {
        private Bitmap bitmap;
        private int[] heights;

        public int Width { get; }
        public int Depth { get; }
        public int MaxHeight { get; }

        private bool smoothNormals;

        public Terrain(int width, int depth, int maxHeight, string heightMapFilePath, bool smoothNormals)
        {
            this.Width = width;
            this.Depth = depth;
            this.MaxHeight = maxHeight;

            this.heights = new int[width * depth];
            this.smoothNormals = smoothNormals;

            ConvertImageToMap(heightMapFilePath);
        }

        private void ConvertImageToMap(string heightMapFilePath)
        {
            if (File.Exists(heightMapFilePath))
            {
                this.bitmap = new Bitmap(heightMapFilePath);

                for (int y = 0; y < this.Depth; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        int xPos = x * (this.bitmap.Width / this.Width);
                        int yPos = y * (this.bitmap.Height / this.Depth);
                        this.heights[(y * this.Width) + x] = ((this.MaxHeight * this.bitmap.GetPixel(xPos, yPos).B) / 255);
                    }
                }
            }
        }

        public Bitmap GetBitmap()
        {
            return this.bitmap;
        }

        public int[] GetHeights()
        {
            return this.heights;
        }
    }
}
