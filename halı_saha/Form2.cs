using System;
using System.Collections.Generic;
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
        }

        public Form2()
        {
            InitializeComponent();
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
            }
            else
            {
                MessageBox.Show("Oyuncu listesi alınamadı!");
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

