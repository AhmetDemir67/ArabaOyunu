using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arabaOyunu
{
    public partial class Form1 : Form
    {
        // Araba ile ilgili durumları saklayan değişkenler
        bool arabaCalısıyorMu = false;  // Araba çalışıyor mu?
        int arabaHızı = 0;  // Araba hızını tutar
        bool arabaDuruyorMu = true;  // Araba duruyor mu?
        bool arabaLeft = false;  // Araba sola gitmekte mi?
        bool arabaRight = false;  // Araba sağa gitmekte mi?
        bool arabaBreak = false;  // Araba fren yapıyor mu?

        // Rakip arabaların hareket yönlerini tutan diziler
        bool[] RivalCarsLeft = { false, false, false };
        bool[] RivalCarsRight = { false, false, false };

        // Rakip arabaların görsellerini tutan bir dizi
        PictureBox[] pictureBoxes = new PictureBox[3];
        Image[] RivalsCarsImages = new Image[] { Properties.Resources.images, Properties.Resources.images__1_, Properties.Resources.pngtree_blue_car_top_view_png_image_6564078 };

        // Random nesnesi
        Random ran = new Random();

        // Form oluşturulurken yapılacak işlemler
        public Form1()
        {
            InitializeComponent();

            // Rakip arabaları oluşturur
            CreateRivals();
        }

        // Rakip arabaları oluşturma fonksiyonu
        public void CreateRivals()
        {
            // 3 adet rakip araba oluşturuyoruz
            for (int i = 0; i < 3; i++)
            {
                // Yeni bir PictureBox nesnesi (rakip araba) oluşturuluyor
                var rivalcar = new PictureBox();
                pictureBoxes[i] = rivalcar;
                rivalcar.Name = "Rivalcar" + (i + 1).ToString() + "_pictureBox1";  // Resmin adı
                rivalcar.Size = new Size(52, 60);  // Resmin boyutları
                rivalcar.BackColor = Color.Black;  // Arka plan rengini siyah yapıyoruz
                rivalcar.Location = new Point(124, (-500 * i) - 60);  // Arabayı farklı başlangıç konumlarına yerleştiriyoruz
                rivalcar.BackgroundImageLayout = ImageLayout.Stretch;  // Görselin boyutunu strech yapıyoruz
                rivalcar.Visible = true;  // Resmi görünür yapıyoruz

                // pictureBoxes dizisine rakip arabayı ekliyoruz
                mainPanel1.Controls.Add(pictureBoxes[i]);

                // Zamanlayıcıyı ayarlıyoruz
                Timer RivalcarsTimer = new Timer();
                RivalcarsTimer.Tag = "RivalcarTimer" + i;

                RivalcarsTimer.Interval = 100;  // Zamanlayıcıyı 100 ms aralıklarla çalıştırıyoruz
                RivalcarsTimer.Tick += new EventHandler(RivalcarsTimerTick);  // Zamanlayıcı tetiklendiğinde yapılacak işlem
                RivalcarsTimer.Start();  // Zamanlayıcıyı başlatıyoruz
            }
        }

        // Rakip arabaların hareketini kontrol eden zamanlayıcı tick eventi
        public void RivalcarsTimerTick(object sender, EventArgs e)
        {
            // Gönderilen zamanlayıcıyı alıyoruz
            Timer tm = (Timer)sender;

            // Zamanlayıcı etiketinden araba numarasını çıkarıyoruz
            int othercar = Convert.ToInt32(tm.Tag.ToString().Substring(15));

            // Rakip arabaların yönünü rastgele belirliyoruz
            int othercarRandom = ran.Next(0, 3);

            if (othercarRandom == 0)
            {
                RivalCarsRight[othercar] = false;
                RivalCarsLeft[othercar] = false;
            }
            else if (othercarRandom == 1)
            {
                RivalCarsRight[othercar] = false;
                RivalCarsLeft[othercar] = true;
            }
            else if (othercarRandom == 2)
            {
                RivalCarsRight[othercar] = true;
                RivalCarsLeft[othercar] = false;
            }

            // Eğer rakip araba sola gitmekteyse, sola kaydır
            if (RivalCarsLeft[othercar] && pictureBoxes[othercar].Left > 30)
            {
                pictureBoxes[othercar].Left -= 5;
            }
            // Eğer rakip araba sağa gitmekteyse, sağa kaydır
            if (RivalCarsRight[othercar] && pictureBoxes[othercar].Left < 218)
            {
                pictureBoxes[othercar].Left += 5;
            }

            // Rakip arabayı aşağı doğru hareket ettiriyoruz (yol boyunca)
            pictureBoxes[othercar].Top -= 50 + arabaHızı;

            // Eğer rakip araba kendi aracınızla çarpışırsa, hızınızı sıfırlayın
            if (pictureBoxes[othercar].Bounds.IntersectsWith(pictureBox1.Bounds))
            {
                arabaHızı = 0;
            }
        }

        // Araba hızını güncelleyen zamanlayıcı tick eventi
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Eğer araba çalışıyorsa ve hızı 80'e ulaşmadıysa, hızını arttır
            if (arabaCalısıyorMu && arabaHızı < 80)
            {
                arabaHızı += 5;
            }
            // Eğer araba çalışmıyorsa ve hız sıfırlanmadıysa, hızını düşür
            if (!arabaCalısıyorMu && arabaHızı > 0)
            {
                arabaHızı -= 5;
            }
            // Eğer fren yapılmışsa, hızını düşür
            if (arabaBreak && arabaHızı > 0)
            {
                arabaHızı -= 5;
            }
            // Eğer hız sıfırsa, araba duruyor
            if (arabaHızı == 0)
            {
                arabaDuruyorMu = true;
            }
            else
            {
                arabaDuruyorMu = false;
            }
        }

        // Araba panellerini hareket ettiren zamanlayıcı tick eventi
        private void timer2_Tick(object sender, EventArgs e)
        {
            // Eğer araba durmuyorsa, panelleri hareket ettir
            if (!arabaDuruyorMu)
            {
                Paneller();

                // Araba sola gitmekteyse, sola kaydır
                if (arabaLeft && pictureBox1.Left > 30)
                {
                    pictureBox1.Left -= 5;
                }
                // Araba sağa gitmekteyse, sağa kaydır
                if (arabaRight && pictureBox1.Right < 218)
                {
                    pictureBox1.Left += 5;
                }
            }
        }

        // Klavye tuşlarına basıldığında yapılan işlemler
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                arabaBreak = false;  // Freni kaldır
                arabaCalısıyorMu = true;  // Araba çalışmaya başlasın
            }
            if (e.KeyCode == Keys.Down)
            {
                arabaBreak = true;  // Freni çek
                arabaCalısıyorMu = false;  // Araba durmaya başlasın
            }
            if (e.KeyCode == Keys.Left)
            {
                arabaRight = false;  // Sağ yönü kapat
                arabaLeft = true;  // Sola gitmeye başla
            }
            if (e.KeyCode == Keys.Right)
            {
                arabaLeft = false;  // Sol yönü kapat
                arabaRight = true;  // Sağa gitmeye başla
            }
        }

        // Klavye tuşu bırakıldığında yapılan işlemler
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                arabaCalısıyorMu = false;  // Araba duracak
            }
            if (e.KeyCode == Keys.Down)
            {
                arabaBreak = false;  // Freni kaldır
            }
            if (e.KeyCode == Keys.Left)
            {
                arabaLeft = false;  // Sola gitmeyi durdur
            }
            if (e.KeyCode == Keys.Right)
            {
                arabaRight = false;  // Sağa gitmeyi durdur
            }
        }

        // Panellerin hareketi (yol panelleri)
        public void Paneller()
        {
            pictureBox2.Top = pictureBox2.Top + arabaHızı;  // Panelin yukarı doğru hareketi
            pictureBox3.Top = pictureBox3.Top + arabaHızı;  // Diğer panelin yukarı doğru hareketi
        }
    }
}
