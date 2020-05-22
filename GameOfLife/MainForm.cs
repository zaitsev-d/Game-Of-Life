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
        private int currentGeneration = default;
        private Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows;
        private int columns;

        private Color color;
        private Brush brushes;

        public MainForm()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled) return;

            currentGeneration = default;
            Text = $"Generation {currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            cbColorPatterns.Enabled = false;

            resolution = (int)nudResolution.Value;
            rows = pictureBox1.Right / resolution;
            columns = pictureBox1.Width / resolution;
            field = new bool[columns, rows];

            Random random = new Random();
            for(int x = 0; x < columns; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

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

        private void NextGeneration()
        {
            graphics.Clear(color);

            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if(!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if(hasLife && (neighboursCount < 2 || neighboursCount > 3)) 
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];

                    if(hasLife) graphics.FillRectangle(brushes, x * resolution, y * resolution, resolution, resolution);
                }
            }

            field = newField;
            pictureBox1.Refresh();
            Text = $"Generation {++currentGeneration}";
        }

        private int CountNeighbours(int x, int y)
        {
            int count = default;

            for(int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var column = (x + i + columns) % columns;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = column == x && row == y;
                    var hasLife = field[column, row];

                    if (hasLife && !isSelfChecking) count++;
                }
            }
            return count;
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
            NextGeneration();
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
    }
}
