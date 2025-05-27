using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Shakirova_tomogram_visualizer
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        Bin bin = new Bin();
        View view = new View();
        int currentLayer = 0;

        public Form1()
        {
            InitializeComponent();
            trackBar1.Maximum = 10;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;

            trackBar2.Minimum = 0;
            trackBar2.Maximum = 2000;
            trackBar2.Value = 0;

            trackBar3.Minimum = 1;
            trackBar3.Maximum = 2000;
            trackBar3.Value = 2000;

            trackBar2.Scroll += trackBar2_Scroll;
            trackBar3.Scroll += trackBar3_Scroll;
        }

        //заставляет кадр рендериться заново
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps = {0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);

                trackBar2.Maximum = Bin.Z - 1;

                view.generateTextureImage(currentLayer);
                view.Load2DTexture();
                loaded = true;
                glControl1.Invalidate();
            }

            trackBar1.Maximum = 10;
        }

        bool needReload = false;

        //она рисовала томограмму
        //с помощью текстуры, а загружала текстуру
        //только когдапеременная needReload будет установлена в true
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (radioButton1.Checked)
                {
                    view.DrawQuads(currentLayer);
                }
                if (radioButton2.Checked)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                if (radioButton1.Checked)
                {
                    view.DrawQuadStrip(currentLayer);
                }
                if (radioButton4.Checked)
                {
                    view.DrawTriangleStrip(currentLayer);
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            glControl1.Invalidate();
            needReload = true;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.TFmin = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.TFwidth = trackBar3.Value;
            needReload = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
