using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace halı_saha
{
    public partial class Form2 : Form
    {
        private readonly List<Form1.SecilenOyuncu> _takimA;
        private readonly List<Form1.SecilenOyuncu> _takimB;

        public Form2(List<Form1.SecilenOyuncu> takimA, List<Form1.SecilenOyuncu> takimB)
        {
            InitializeComponent();
            _takimA = takimA;
            _takimB = takimB;
            Resize += Form2_Resize;
        }

        public Form2()
        {
            InitializeComponent();
            Resize += Form2_Resize;
        }

        private void Form2_Load_1(object sender, EventArgs e)
        {
            lstTakim1.Items.Clear();
            lstTakim2.Items.Clear();

            if (_takimA != null && _takimB != null)
            {
                foreach (var oyuncu in _takimA)
                {
                    lstTakim1.Items.Add($"{oyuncu.Isim} - {oyuncu.Bolge} - {oyuncu.SeviyePuani}");
                }

                foreach (var oyuncu in _takimB)
                {
                    lstTakim2.Items.Add($"{oyuncu.Isim} - {oyuncu.Bolge} - {oyuncu.SeviyePuani}");
                }

                lblTakim1Ozet.Text = TakimOzetiOlustur(_takimA);
                lblTakim2Ozet.Text = TakimOzetiOlustur(_takimB);

                lstTakim1.Visible = false;
                lstTakim2.Visible = false;
                lblTakim1Ozet.Visible = false;
                lblTakim2Ozet.Visible = false;

                TakimlariSahayaYerlestir();
            }
            else
            {
                MessageBox.Show("Oyuncu listesi alınamadı!");
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            TakimlariSahayaYerlestir();
        }

        private void TakimlariSahayaYerlestir()
        {
            if (_takimA == null || _takimB == null)
            {
                return;
            }

            TemizleSahaEtiketleri();

            List<PointF> takimAKonumlar = new List<PointF>
            {
                new PointF(0.10f, 0.50f),
                new PointF(0.23f, 0.28f),
                new PointF(0.23f, 0.72f),
                new PointF(0.40f, 0.38f),
                new PointF(0.40f, 0.62f),
                new PointF(0.58f, 0.28f),
                new PointF(0.58f, 0.72f)
            };

            List<PointF> takimBKonumlar = new List<PointF>
            {
                new PointF(0.90f, 0.50f),
                new PointF(0.77f, 0.28f),
                new PointF(0.77f, 0.72f),
                new PointF(0.60f, 0.38f),
                new PointF(0.60f, 0.62f),
                new PointF(0.42f, 0.28f),
                new PointF(0.42f, 0.72f)
            };

            List<Form1.SecilenOyuncu> takimASirali = OyunculariPozisyonlaraDagit(_takimA);
            List<Form1.SecilenOyuncu> takimBSirali = OyunculariPozisyonlaraDagit(_takimB);

            OyuncuEtiketleriEkle(takimASirali, takimAKonumlar, 1);
            OyuncuEtiketleriEkle(takimBSirali, takimBKonumlar, 1 + _takimA.Count);
        }

        private List<Form1.SecilenOyuncu> OyunculariPozisyonlaraDagit(List<Form1.SecilenOyuncu> takim)
        {
            List<Form1.SecilenOyuncu> kalan = new List<Form1.SecilenOyuncu>(takim);
            List<Form1.SecilenOyuncu> sirali = new List<Form1.SecilenOyuncu>();

            string[] pozisyonlar = new[] { "kaleci", "defans", "defans", "ofans", "ofans", "ofans", "ofans" };

            foreach (string pozisyon in pozisyonlar)
            {
                Form1.SecilenOyuncu oyuncu = SecPozisyonOyuncusu(kalan, pozisyon);

                if (oyuncu == null)
                {
                    break;
                }

                sirali.Add(oyuncu);
                kalan.Remove(oyuncu);
            }

            return sirali;
        }

        private Form1.SecilenOyuncu SecPozisyonOyuncusu(List<Form1.SecilenOyuncu> kalan, string pozisyon)
        {
            if (kalan.Count == 0)
            {
                return null;
            }

            Form1.SecilenOyuncu oyuncu;

            if (pozisyon == "kaleci")
            {
                oyuncu = kalan.FirstOrDefault(o => IsKaleci(o.Bolge))
                         ?? kalan.FirstOrDefault(o => IsDefans(o.Bolge))
                         ?? kalan.FirstOrDefault(o => IsOfans(o.Bolge));
            }
            else if (pozisyon == "defans")
            {
                oyuncu = kalan.FirstOrDefault(o => IsDefans(o.Bolge))
                         ?? kalan.FirstOrDefault(o => IsOfans(o.Bolge))
                         ?? kalan.FirstOrDefault(o => IsKaleci(o.Bolge));
            }
            else
            {
                oyuncu = kalan.FirstOrDefault(o => IsOfans(o.Bolge))
                         ?? kalan.FirstOrDefault(o => IsDefans(o.Bolge))
                         ?? kalan.FirstOrDefault(o => IsKaleci(o.Bolge));
            }

            return oyuncu ?? kalan.FirstOrDefault();
        }

        private static bool IsKaleci(string bolge)
        {
            string normalized = NormalizeBolge(bolge);
            return normalized == "kaleci"
                || normalized == "kalecı"
                || normalized == "kale"
                || normalized == "gk";
        }

        private static bool IsDefans(string bolge)
        {
            string normalized = NormalizeBolge(bolge);
            return normalized == "defans" || normalized == "defansif";
        }

        private static bool IsOfans(string bolge)
        {
            string normalized = NormalizeBolge(bolge);
            return normalized == "ofans" || normalized == "ofansif";
        }

        private void TemizleSahaEtiketleri()
        {
            foreach (Control kontrol in Controls.OfType<Control>().Where(c => c.Tag as string == "SahaOyuncu").ToList())
            {
                Controls.Remove(kontrol);
                kontrol.Dispose();
            }
        }

        private void OyuncuEtiketleriEkle(List<Form1.SecilenOyuncu> takim, List<PointF> konumlar, int baslangicNo)
        {
            int genislik = ClientSize.Width;
            int yukseklik = ClientSize.Height;

            for (int i = 0; i < takim.Count && i < konumlar.Count; i++)
            {
                PointF oran = konumlar[i];
                int x = (int)(genislik * oran.X);
                int y = (int)(yukseklik * oran.Y);

                Label numara = new Label
                {
                    Tag = "SahaOyuncu",
                    Text = (baslangicNo + i).ToString(),
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Size = new Size(26, 26),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(x - 13, y - 13)
                };

                Label isim = new Label
                {
                    Tag = "SahaOyuncu",
                    Text = takim[i].Isim,
                    BackColor = Color.FromArgb(160, Color.Black),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(x - 18, y + 16)
                };

                Controls.Add(numara);
                Controls.Add(isim);
                numara.BringToFront();
                isim.BringToFront();
            }
        }

        private static string TakimOzetiOlustur(List<Form1.SecilenOyuncu> takim)
        {
            if (takim == null || takim.Count == 0)
            {
                return "Toplam Güç: 0 | Pozisyon: yok";
            }

            int toplamPuan = takim.Sum(o => o.SeviyePuani);

            string bolgeOzet = string.Join(", ",
                takim.GroupBy(o => NormalizeBolge(o.Bolge))
                     .OrderBy(g => g.Key)
                     .Select(g => $"{g.Key}:{g.Count()}"));

            return $"Toplam Güç: {toplamPuan} | Pozisyon: {bolgeOzet}";
        }

        private static string NormalizeBolge(string bolge)
        {
            return (bolge ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}

