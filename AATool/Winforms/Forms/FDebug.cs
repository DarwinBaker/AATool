using AATool.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FDebug : Form
    {
        public FDebug()
        {
            InitializeComponent();

            using (var stream = new MemoryStream())
            {
                SpriteSheet.Atlas.SaveAsPng(stream, SpriteSheet.Atlas.Width, SpriteSheet.Atlas.Height);
                atlas.Image = new Bitmap(stream);
            }
        }
    }
}
