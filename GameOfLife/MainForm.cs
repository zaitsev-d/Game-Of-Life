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

namespace GameOfLife
{
    public partial class MainForm : Form
    {
        private Graphics graphics;
        private int resolution;
        private GameEngine gameEngine;

        private Color color;
        private Brush brushes;

        public MainForm()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled) return;

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            cbColorPatterns.Enabled = false;
            resolution = (int)nudResolution.Value;

            gameEngine = new GameEngine
            (
                _rows: pictureBox1.Right / resolution,
                _columns: pictureBox1.Width / resolution,
                _density: (int)nudDensity.Minimum + (int)nudDensity.Maximum - (int)nudDensity.Value 
            );

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            graphics.FillRectangle(brushes, 0, 0, resolution, resolution);
            timer1.Start();
        }

        private void StopGame()
        {
            if (!timer1.Enabled) return;
            timer1.Stop();

            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
            cbColorPatterns.Enabled = true;
        }
        
        private void DrawNextGeneration()
        {
            graphics.Clear(color);

            var field = gameEngine.GetCurrentGeneration();

            for(int x = 0; x < field.GetLength(0); x++)
            {
                for(int y = 0; y < field.GetLength(1); y++)
                {
                    if(field[x, y])
                        graphics.FillRectangle(brushes, x * resolution, y * resolution, resolution - 1, resolution - 1);
                }
            }

            pictureBox1.Refresh();
            Text = $"Generation {gameEngine.CurrentGeneration}";
            gameEngine.NextGeneration();
        }

        private void ColorPatterns()
        {
            switch(cbColorPatterns.SelectedItem)
            {
                case "Crimson - Black": brushes = Brushes.Crimson; color = Color.Black; break;
                case "Dark Blue": brushes = Brushes.Gray; color = ColorTranslator.FromHtml("#001f3c"); break;
                case "Negative World": brushes = Brushes.Black; color = Color.DarkGray; break;
                case "Infinity Void": brushes = Brushes.MediumPurple; color = ColorTranslator.FromHtml("#2c005d"); break;
                case "Desert World": brushes = Brushes.SandyBrown; color = ColorTranslator.FromHtml("#7e4a27"); break;
                case "Red Room": brushes = Brushes.IndianRed; color = ColorTranslator.FromHtml("#5c0000"); break;
                case "Green - Yellow": brushes = Brushes.Goldenrod; color = Color.DarkOliveGreen; break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawNextGeneration();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (cbColorPatterns.SelectedItem != null) StartGame();
            else 
            { 
                cbColorPatterns.SelectedItem = cbColorPatterns.Items[0]; 
                StartGame(); 
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void cbColorPatterns_SelectedValueChanged(object sender, EventArgs e)
        {
            ColorPatterns();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled) return;

            if(e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;

                gameEngine.AddCell(x, y);
            }

            if(e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;

                gameEngine.RemoveCell(x, y);
            }
        }
    }
}
