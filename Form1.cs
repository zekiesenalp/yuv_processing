using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Yuv2Bmp
{

    /*
     Ahmet Manga - 160202008
     Zeki Esenalp - 160202033
         */
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string folder;
        string file_path;
        int width, height;
        byte[] ms;
        int file_type;
        byte[] y, u, v;
        int bmp_isim = 1;
        int oynatilan = 1;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || !IsNumeric(textBox1.Text) || !IsNumeric(textBox2.Text))
            {
                MessageBox.Show("Tüm alanları doldurun.");
            }
            else
            {
               
                file_type = int.Parse(comboBox1.SelectedItem.ToString());
                openFileDialog1.Title = "Yuv Dosyası Seçin";
                openFileDialog1.Filter = "yuv files (*.yuv) |*.yuv";
                openFileDialog1.FileName = "";
                label3.Text = "Devam ediyor.";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    
                    button1.Enabled = false;
                    file_path = openFileDialog1.FileName;
                    width = int.Parse(textBox1.Text);
                    height = int.Parse(textBox2.Text);
                    StreamReader s = new StreamReader(file_path);
                    Stream stream = File.OpenRead(file_path);
                    ms = ReadFile(stream);
                    // 444 için
                    
                    if (file_type == 444)
                    {
                        int length = ms.Length;
                        int frameSize = length / (width * height);
                        
                        for (int f = 0; f < frameSize; f++)
                        {
                            if (f % 3 == 0)
                            {
                                y = new byte[width * height];
                                u = new byte[width * height];
                                v = new byte[width * height];
                            }
                            int sira = 0;
                            for (int l = f * (width * height); l < (f + 1) * (width * height); l++)
                            {
                                if (f % 3 == 0)
                                {
                                    y[sira] = ms[l];
                                }
                                if (f % 3 == 1)
                                {
                                    u[sira] = ms[l];
                                }
                                else
                                {
                                    v[sira] = ms[l];
                                }
                                sira++;
                            }

                            if (f % 3 == 2)
                            {
                                YuvToRgb(y, u, v, bmp_isim.ToString());
                                bmp_isim++;

                            }
                        }
                    }
                    else if (file_type == 422)
                    {
                        int length = ms.Length;
                        int frameSize = length / (width * height);
                        
                        for (int f = 0; f < frameSize; f++)
                        {
                            if (f % 2 == 0)
                            {
                                y = new byte[width * height];
                                u = new byte[(width * height)];
                                v = new byte[(width * height)];
                            }
                            int sira = 0;
                            bool gecis = false;
                            for (int l = f * (width * height); l < (f + 1) * (width * height); l++)
                            {
                                if (f % 2 == 0)
                                {
                                    y[sira] = ms[l];
                                }
                                else
                                {
                                    if (((f+1)*width*height - l) < (width*height)/2 )
                                    {
                                       
                                        if (!gecis)
                                        {
                                            sira = 0;
                                            gecis = true;
                                        }
                                        v[sira] = ms[l];  
                                    }
                                    else
                                    {
                                  
                                      
                                        u[sira] = ms[l];
                                    }
                                }
                                sira++;
                            }

                            if (f % 2 == 1)
                            {
                                YuvToRgb(y, u, v, bmp_isim.ToString());
                                bmp_isim++;

                            }
                        }

                    }
                    else if (file_type == 420)
                    {
                        int length = ms.Length;
                        double frameSize2 = length / ((width * height)*(1.5)); int frameSize = Convert.ToInt32(frameSize2);

                        for(int f = 0; f < frameSize; f++)
                        {
                            y = new byte[width * height];
                            u = new byte[(width * height)];
                            v = new byte[(width * height)];
                            int sira = 0;
                            double l = 0;
                            for (l = f * ((width * height)*1.5); l < (width * height) + ((width * height) * f * 1.5); l++)
                            {
                                int l2 = Convert.ToInt32(l);
                                y[sira] = ms[l2];
                                sira++;
                            }
                          //  MessageBox.Show(" f : " + f + " - sira : " + sira.ToString() + " l : " + l);
                            double l_u = 0;
                            sira = 0; 
                           for(l_u = l; l_u < ((width * height) + ((width*height)/4)) + ((width * height) * f); l_u++)
                            {
                                int l_u2 = Convert.ToInt32(l_u);
                                u[sira] = ms[l_u2];
                                sira++;
                            }
                            double l_v = 0;
                            sira = 0;
                            for (l_v = l_u; l_v < (f + 1) * ((width * height) * (1.5)); l_v++)
                            {
                                int l_v2 = Convert.ToInt32(l_v);
                                v[sira] = ms[l_v2];
                                sira++;
                            }
                            // MessageBox.Show("f: " + f + " y- " + y[0] + " u- " + u[0] + " v " + v[0] + " ---- l_v " + l_v );
                            YuvToRgb(y, u, v, bmp_isim.ToString());
                            bmp_isim++;
                        }
                      
                    }
                    label3.Text = "Tamamlandı";
                    label3.ForeColor = Color.Green;
                    System.Diagnostics.Process.Start(folder);
                    if (comboBox2.Items[0].ToString() == "1")
                    {
                        pictureBox1.Image = Image.FromFile(folder + "\\1.bmp");
                    }
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        bool IsNumeric(string text)
        {
            foreach (char chr in text)
            {
                if (!Char.IsNumber(chr)) return false;
            }
            return true;
        }
        public void YuvToRgb(byte[] y, byte[] u, byte[] v, string isim)
        {
            byte[] rgb = new byte[width * height * 4];
            Bitmap bm = new Bitmap(width, height);

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int i = h * width + w;
                   
                    double r = (y[i] + 1.4075);
                    double g = (y[i] - 0.3455);
                    double b = (y[i] + 1.7790);

                    rgb[4 * i + 3] = 255;
                    rgb[4 * i + 2] = (byte)Math.Round(r > 255 ? 255 : r < 0 ? 0 : r);
                    rgb[4 * i + 1] = (byte)Math.Round(g > 255 ? 255 : g < 0 ? 0 : g);
                    rgb[4 * i + 0] = (byte)Math.Round(b > 255 ? 255 : b < 0 ? 0 : b);
                    Color c = Color.FromArgb(rgb[4 * i + 3], rgb[4 * i + 2], rgb[4 * i + 1], rgb[4 * i + 0]);
                    bm.SetPixel(w, h, c);
                }
            }
            bm.Save(folder+"\\" + isim + ".bmp");
            comboBox2.Items.Add(isim);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bitmap oku = new Bitmap(folder + "\\" + comboBox2.SelectedItem + ".bmp");
            pictureBox1.Image = Image.FromFile(folder + "\\" + comboBox2.SelectedItem + ".bmp");
            //MessageBox.Show(folder + "\\" + comboBox2.SelectedItem + ".bmp");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            oynatilan = 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            try
            {
                pictureBox1.Image = Image.FromFile(folder + "\\" + oynatilan + ".bmp");
            }
            catch { }
            
            oynatilan++;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            Random rand = new Random();
            int sayi = rand.Next(0, 99999999);
            folder = "D:\\YuvtoBmp_" + sayi;
            Directory.CreateDirectory(folder);
        }

        public static byte[] ReadFile(Stream input)
        {
            byte[] buf = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buf, 0, buf.Length)) > 0)
                {
                    ms.Write(buf, 0, read);
                }
                return ms.ToArray();
            }

        }
       
    }
}