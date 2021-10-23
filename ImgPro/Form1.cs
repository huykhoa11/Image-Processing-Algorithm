using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgPro
{
    public partial class Form1 : Form
    {
        //Graphics g;
        Bitmap loadBmp, newBmp;
        int imgWidth, imgHeight;
        Color c;

        private void HistogramEqualzation()
        {
            //HistogramEqualzation method based on https://www.youtube.com/watch?v=uqeOrtAzSyU

            double[] monoValue = new double[256];
            double[] newMonoValue = new double[256];
            int[] finalMonoValue = new int[256];
            for (int x=0; x<monoValue.Length; x++)
            {
                monoValue[x] = 0;
                newMonoValue[x] = 0;
                finalMonoValue[x] = 0;
            }
            double tmp, tmp1, tmp2;
            int a, Y, C1, C2;

            double sum = 0;
            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth; j++)
                {
                    c = loadBmp.GetPixel(j, i);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero); //grayscale of pixel
                    a = Convert.ToInt32(tmp1);
                    monoValue[a] += 1;
   
                }
            }

            sum = monoValue.Sum();
            Console.WriteLine("sum = {0}", sum);
            for (int x = 0; x < monoValue.Length; x++)
            {
                monoValue[x] = monoValue[x] / sum;
                for(int y=0; y<x; y++)
                {
                    newMonoValue[x] += monoValue[y];
                }
                newMonoValue[x] *= 255;
                
                //Round the value of finalMonoValue
                if(newMonoValue[x] - Convert.ToInt32(newMonoValue[x]) > 0.5)
                {
                    finalMonoValue[x] = Convert.ToInt32(newMonoValue[x]) + 1;
                }
                else if(newMonoValue[x] - Convert.ToInt32(newMonoValue[x]) <= 0.5)
                {
                    finalMonoValue[x] = Convert.ToInt32(newMonoValue[x]);
                }
            }


            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth; j++)
                {
                    //Convert RGB to YCbCr formula and reverse, Reference links
                    //https://www.fourcc.org/fccyvrgb.php
                    //Visual C# .NET & Visual Basic .Netによるディジタル画像処理の基礎と応用  酒井　幸せ

                    c = loadBmp.GetPixel(j, i);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp2 = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero); //grayscale of pixel
                    a = Convert.ToInt32(tmp1);
                    Y = finalMonoValue[a];

                    /////////////////
                    tmp = (c.B - tmp2);// * 0.565;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    C1 = a;

                    /////////////////
                    tmp = (c.R - tmp2);// * 0.713;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    C2 = a;

                    int RR, GG, BB;

                    RR = Convert.ToInt32(Y + C2);
                    GG = Convert.ToInt32(Y - (0.299*C2 + 0.114*C1) / 0.587);
                    BB = Convert.ToInt32(Y + C1);


                    if (RR < 0) RR = 0;
                    else if (RR > 255) RR = 255;

                    if (GG < 0) GG = 0;
                    else if (GG > 255) GG = 255;

                    if (BB < 0) BB = 0;
                    else if(BB > 255) BB = 255;
                    


                    newBmp.SetPixel(j, i, Color.FromArgb(RR, GG, BB));
                }
            }
        }


        private void MonoChrome()
        {
            int a;
            double tmp, tmp1;
            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth; j++)
                {
                    c = loadBmp.GetPixel(j, i);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero); 
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    //a = Convert.ToInt32( 0.299 * c.R + 0.587 * c.G + 0.114 * c.);
                    newBmp.SetPixel(j, i, Color.FromArgb(a, a, a));
                }
            }
        }

        private void Binarization()
        {
            int avg = 128;
            int a;
            double tmp, tmp1;
            
            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth; j++)
                {
                    c = loadBmp.GetPixel(j, i);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    if (a >= avg) a = 255;
                    else if (a < avg) a = 0;
                    newBmp.SetPixel(j, i, Color.FromArgb(a, a, a));
                }
            }
            
        }


        public void adaptiveThresholding()
        {
            int avg = 128;
            int a;
            double tmp, tmp1;
            
            int[,] tmpPixel = new int[imgHeight + 2, imgWidth + 2];
            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    c = loadBmp.GetPixel(j - 1, i - 1);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    if (a >= avg) a = 255;
                    tmpPixel[i, j] = a;
                }
            }

 
            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    int x = tmpPixel[i, j];
                    tmp1 = (tmpPixel[i - 1, j - 1] + tmpPixel[i - 1, j] + tmpPixel[i - 1, j + 1] +
                        tmpPixel[i, j - 1] + tmpPixel[i, j + 1] + tmpPixel[i + 1, j - 1] + tmpPixel[i + 1, j] + tmpPixel[i + 1, j + 1]) / 8;
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    if (x >= a) x = 255;
                    else if (x < a) x = 0;
                    newBmp.SetPixel(j - 1, i - 1, Color.FromArgb(x, x, x));
                }
            }
            
        }

        public void MedianFilter()
        {
            double tmp, tmp1, tmp2;
            int a, Y, C1, C2;

            int[,] tmpPixel = new int[imgHeight + 2, imgWidth + 2];
            int[,] savePixel = new int[imgHeight, imgWidth];
            var mask = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    c = loadBmp.GetPixel(j - 1, i - 1);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    tmpPixel[i, j] = a;
                }
            }


            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    int x = tmpPixel[i, j];

                    mask[0] = tmpPixel[i - 1, j - 1];
                    mask[1] = tmpPixel[i - 1, j];
                    mask[2] = tmpPixel[i - 1, j + 1];
                    mask[3] = tmpPixel[i, j - 1];
                    mask[4] = tmpPixel[i, j + 1];
                    mask[5] = tmpPixel[i + 1, j - 1];
                    mask[6] = tmpPixel[i + 1, j];
                    mask[7] = tmpPixel[i + 1, j + 1];
                    mask[8] = tmpPixel[i, j];
                    mask.Sort();

                    savePixel[i - 1, j - 1] = mask[4];

                }
            }

            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth; j++)
                {
                    //Convert RGB to YCbCr formula and reverse, Reference links
                    //https://www.fourcc.org/fccyvrgb.php
                    //Visual C# .NET & Visual Basic .Netによるディジタル画像処理の基礎と応用  酒井　幸せ

                    c = loadBmp.GetPixel(j, i);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp2 = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero); //grayscale of pixel
                    a = Convert.ToInt32(tmp1);
                    //Y = finalMonoValue[a];
                    Y = savePixel[i, j];

                    /////////////////
                    tmp = (c.B - tmp2);// * 0.565;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    C1 = a;

                    /////////////////
                    tmp = (c.R - tmp2);// * 0.713;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    C2 = a;

                    int RR, GG, BB;

                    RR = Convert.ToInt32(Y + C2);
                    GG = Convert.ToInt32(Y - (0.299 * C2 + 0.114 * C1) / 0.587);
                    BB = Convert.ToInt32(Y + C1);

                    /*
                    RR = Convert.ToInt32(Y + 1.403*C2);
                    GG = Convert.ToInt32(Y - 0.344*C1 - 0.714*C2);
                    BB = Convert.ToInt32(Y + 1.770*C1);
                    */

                    if (RR < 0) RR = 0;
                    else if (RR > 255) RR = 255;

                    if (GG < 0) GG = 0;
                    else if (GG > 255) GG = 255;

                    if (BB < 0) BB = 0;
                    else if (BB > 255) BB = 255;


                    newBmp.SetPixel(j, i, Color.FromArgb(RR, GG, BB));
                }
            }
        }

        public int[,] calcSum(int[,] arr1, int[,] arr2)
        {
            int[,] result = new int[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = arr1[i, j] + arr2[i, j];
                }
            }

            return result;
        }

        public int calcMulti(int[,] arr1, int[,] arr2)
        {
            int[,] result = new int[3, 3];
            int multi = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    multi += arr1[i, j] * arr2[i, j];
                }

            }

            return Math.Abs(multi);
        }


        public int Sobel(int[,] root)
        {
            int[,] afterSobelX = new int[3, 3];
            int[,] afterSobelY = new int[3, 3];
            int[,] sumOfSobel = new int[3, 3];
            int[,] result = new int[3, 3];

            int[,] sobelX = new int[3, 3]{
                { -1, -2, -1} ,   /*  initializers for row indexed by 0 */
                {  0,  0, 0} ,   /*  initializers for row indexed by 1 */
                {  1,  2, 1}
            };

            int[,] sobelY = new int[3, 3]{
                { -1,  0, 1} ,   /*  initializers for row indexed by 0 */
                { -2,  0, 2} ,   /*  initializers for row indexed by 1 */
                { -1,  0, 1}
            };


            //afterSobelX = calcMulti(sobelX, root);
            //afterSobelY = calcMulti(sobelY, root);
            //sumOfSobel = calcSum(afterSobelX, afterSobelY);
            //result = calcSum(sumOfSobel, root);
            int g = calcMulti(sobelX, root) + calcMulti(sobelY, root);

            return g;
        }

        public void EdgeDetection()
        {
            double tmp, tmp1;
            int a;

            int[,] tmpPixel = new int[imgHeight + 2, imgWidth + 2];
            int[,] savePixel = new int[3, 3];


            int[,] root = new int[3, 3];

            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    c = loadBmp.GetPixel(j - 1, i - 1);
                    tmp = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    tmp1 = Math.Round(tmp, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(tmp1);  //grayscale of pixel
                    tmpPixel[i, j] = a;
                }
            }
            

            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    root[0,0] = tmpPixel[i - 1, j - 1];
                    root[0,1] = tmpPixel[i - 1, j];
                    root[0,2] = tmpPixel[i - 1, j + 1];
                    root[1,0] = tmpPixel[i, j - 1];
                    root[1,1] = tmpPixel[i, j];
                    root[1,2] = tmpPixel[i, j + 1];
                    root[2,0] = tmpPixel[i + 1, j - 1];
                    root[2,1] = tmpPixel[i + 1, j];
                    root[2,2] = tmpPixel[i + 1, j + 1];

                    int x = Sobel(root);
                    if (x > 255) x = 255;
                    else if (x < 0) x = 0;
                    newBmp.SetPixel(j - 1, i - 1, Color.FromArgb(x, x, x));
                }
            }
        }


        public void Labeling()
        {
            int[,] arr = new int[imgHeight + 2, imgWidth + 2];
            int avg = 128;
            int a;
            double t, t1;
            int labelCount = 0;

            List<KeyValuePair<int, int>> dict = new List<KeyValuePair<int, int>>();

            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    c = loadBmp.GetPixel(j - 1, i - 1);
                    t = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    t1 = Math.Round(t, MidpointRounding.AwayFromZero);
                    a = Convert.ToInt32(t1);  //grayscale of pixel
                    if (a >= avg) a = 1;
                    else if (a < avg) a = 0;
                    arr[i, j] = a;
                }
            }


            //First Scan
            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    var X = new List<int> { };
                    if (arr[i, j - 1] != 0) X.Add(arr[i, j - 1]);
                    if(arr[i - 1, j - 1] != 0) X.Add(arr[i-1, j - 1]);
                    if(arr[i - 1, j] != 0) X.Add(arr[i - 1, j]);
                    if(arr[i - 1, j + 1] != 0) X.Add(arr[i - 1, j + 1]);
                    
                    if (arr[i, j] != 0)
                    {
                        if (arr[i, j - 1] == 0 && arr[i - 1, j - 1] == 0 && arr[i - 1, j] == 0 && arr[i - 1, j + 1] == 0)
                        {
                            arr[i, j] = labelCount * 100 + 100;
                            labelCount += 1;
                        }
                        else if (arr[i, j - 1] != 0 || arr[i - 1, j - 1] != 0 || arr[i - 1, j] != 0 || arr[i - 1, j + 1] != 0)
                        {   
                            arr[i, j] = X.Min();
                            foreach( int k in X) { 
                                if(k != 0 && k != arr[i, j])
                                {
                                    bool test = false;
                                    foreach (KeyValuePair<int, int> kvp in dict)
                                    {
                                        if (kvp.Key == arr[i,j] && kvp.Value == k)
                                        {
                                            test = true;
                                            break;
                                        }
                                    }

                                    if (test == false) dict.Add(new KeyValuePair<int, int>(arr[i, j], k));
                                }
                            }
                        }
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////
            
            //dict processing
            var first = dict.First();

            List<List<int>> myList = new List<List<int>>();
            myList.Add(new List<int> { first.Key, first.Value});

            bool teest = false;
            foreach (KeyValuePair<int, int> kvpp in dict)
            {
                foreach (List<int> l in myList)
                {
                    foreach (int subl in l)
                    {
                        if (kvpp.Key == subl || kvpp.Value == subl)
                        {
                            if (kvpp.Key == subl)
                            {
                                l.Add(kvpp.Value);
                                teest = false;
                                break;
                            }
                            if (kvpp.Value == subl)
                            {
                                l.Add(kvpp.Key);
                                teest = false;
                                break;
                            }
                        }
                        if (kvpp.Key != subl && kvpp.Value != subl)
                        {
                            teest = true;
                        }
                    }
                }
                if (teest == true) myList.Add(new List<int> { kvpp.Key, kvpp.Value });
                teest = false;
            }



            foreach (List<int> item in myList)
            {
                item.Sort();
            }
            
            ////////////////////////////////////////////////////////////////////////////////



            //Second Scan
            for (int i = 1; i < imgHeight + 1; i++)
            {
                for (int j = 1; j < imgWidth + 1; j++)
                {
                    foreach (List<int> l in myList)
                    {
                        foreach (int subl in l)
                        {
                            if (arr[i, j] == subl)
                            {
                                arr[i, j] = l[0];
                            }
                        }
                    }
                }
            }
            

            var R = new List<int>();
            var G = new List<int>();
            var B = new List<int>();
            var myyList = new List<int>();

            Random rnd = new Random();

            foreach (KeyValuePair<int, int> kkvp in dict)
            {
                R.Add(rnd.Next(0,255));
                B.Add(rnd.Next(0,255));
                G.Add(rnd.Next(0,255));
                myyList.Add(kkvp.Key);
            }

 
            for (int i = 1; i < imgHeight + 1; i++)
                {
                    for (int j = 1; j < imgWidth + 1; j++)
                    {
                    foreach (KeyValuePair<int, int> kvp in dict)
                    {
                            if (arr[i,j] == kvp.Key)
                            {
                                newBmp.SetPixel(j-1, i-1, Color.FromArgb(R.ElementAt(myyList.IndexOf(kvp.Key)), G.ElementAt(myyList.IndexOf(kvp.Key)),  B.ElementAt(myyList.IndexOf(kvp.Key))));
                            }
                        }
                    }
                }
            

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }     

        public Form1()
        {
            InitializeComponent();
            imgWidth = 400;
            imgHeight = 300;
            loadBmp = new Bitmap(imgWidth, imgHeight);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            newBmp.Save(saveFileDialog1.FileName);
            MessageBox.Show("File saved successfully", "Inform");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Histogram Equalzation")
            {
                HistogramEqualzation();
            }

            else if (comboBox1.Text == "Binarization")
            {
                Binarization();
            }

            else if (comboBox1.Text == "Adaptive Thresholding")
            {
                adaptiveThresholding();
            }

            else if (comboBox1.Text == "Monochrome")
            {

                MonoChrome();
            }

            else if (comboBox1.Text == "Labeling")
            {
                Labeling();
            }

            else if (comboBox1.Text == "Edge Detection (Sobel)")
            {
                EdgeDetection();
            }

            else if (comboBox1.Text == "Median Filter")
            {
                MedianFilter();
            }


            pictureBox2.Image = new Bitmap(newBmp, new Size(imgWidth, imgHeight));
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBox1.SelectedIndex = -1;
                Bitmap tmpBmp = new Bitmap(openFileDialog1.FileName);
                loadBmp = new Bitmap(tmpBmp, new Size(imgWidth, imgHeight));

                int w = imgWidth;
                int h = imgHeight;

                newBmp = new Bitmap(loadBmp.Width, loadBmp.Height);

                pictureBox1.Image = loadBmp;
            }
        }
    }
}
