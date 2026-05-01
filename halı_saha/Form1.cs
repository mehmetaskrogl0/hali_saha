using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace halı_saha
{
    public partial class Form1 : Form
    {
        // MySQL bağlantısı (Senin bilgilerine göre düzenlendi)
        private readonly MySqlConnection con = new MySqlConnection("Server=localhost;Database=halı_saha;Uid=root;Pwd=Mehmet042");
        private readonly DataTable dt = new DataTable();

        // Listeleme fonksiyonu (Aynen senin mantığın)
        public void listele()
        {
            dt.Clear();
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter("select * from oyuncular", con);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();

            lblSayac.Text = "Seçili oyuncu sayısı: 0 / 14";
            button3.Enabled = false;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listele();
        }

        public class Oyuncu
        {
            public int OyuncuID { get; set; }
            public string Isim { get; set; }
            public string Bolge { get; set; }
            public int Seviye { get; set; }
        }

        public class SecilenOyuncu
        {
            public string Isim { get; set; }
            public string Bolge { get; set; }
            public int SeviyePuani { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string isim = textBox1.Text.Trim();
            string bolge = comboBox1.Text;
            string seviyeText = comboBox2.Text;

            if (string.IsNullOrWhiteSpace(isim) || string.IsNullOrWhiteSpace(bolge) || string.IsNullOrWhiteSpace(seviyeText))
            {
                MessageBox.Show("Lütfen isim, bölge ve seviye alanlarını doldurun.");
                return;
            }

            int seviyePuani;
            if (!int.TryParse(seviyeText, out seviyePuani))
            {
                MessageBox.Show("Seviye sayısal olmalıdır.");
                return;
            }

            con.Open();

            MySqlCommand komut = new MySqlCommand("INSERT INTO oyuncular (isim, bolge, seviye) VALUES (@p1, @p2, @p3)", con);
            komut.Parameters.AddWithValue("@p1", isim);
            komut.Parameters.AddWithValue("@p2", bolge);
            komut.Parameters.AddWithValue("@p3", seviyePuani);

            komut.ExecuteNonQuery();
            con.Close();

            listele();

            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

            MessageBox.Show("Oyuncu başarıyla kadroya eklendi.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                con.Open();

                MySqlCommand komut = new MySqlCommand("DELETE FROM oyuncular WHERE OyuncuID = @id", con);
                komut.Parameters.AddWithValue("@id", dataGridView1.CurrentRow.Cells["OyuncuID"].Value);

                komut.ExecuteNonQuery();
                con.Close();

                listele();
                MessageBox.Show("Oyuncu kaydı silindi.");
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz oyuncuyu listeden seçin.");
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                int secilenSayisi = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && Convert.ToBoolean(row.Cells[0].Value) == true)
                    {
                        secilenSayisi++;
                    }
                }

                lblSayac.Text = $"Seçili oyuncu sayısı: {secilenSayisi} / 14";

                if (secilenSayisi > 14)
                {
                    MessageBox.Show("Maksimum 14 kişi seçebilirsiniz!");
                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = false;
                }

                button3.Enabled = secilenSayisi == 14;
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<SecilenOyuncu> secilenler = new List<SecilenOyuncu>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value != null && Convert.ToBoolean(row.Cells[0].Value) == true)
                {
                    string isim = row.Cells["isim"].Value != DBNull.Value ? row.Cells["isim"].Value.ToString() : "Bilinmiyor";
                    string bolge = row.Cells["bolge"].Value != DBNull.Value ? row.Cells["bolge"].Value.ToString() : "Belirsiz";

                    int seviye = 0;
                    if (row.Cells["seviye"].Value != DBNull.Value && row.Cells["seviye"].Value != null)
                    {
                        int.TryParse(row.Cells["seviye"].Value.ToString(), out seviye);
                    }

                    secilenler.Add(new SecilenOyuncu
                    {
                        Isim = isim,
                        Bolge = bolge,
                        SeviyePuani = seviye
                    });
                }
            }

            if (secilenler.Count == 14)
            {
                List<SecilenOyuncu> takimA;
                List<SecilenOyuncu> takimB;

                TakimlariOlustur(secilenler, out takimA, out takimB);

                Form2 frm2 = new Form2(takimA, takimB);
                frm2.Show();
            }
            else
            {
                MessageBox.Show($"Lütfen tam 14 kişi seçin! (Şu an seçilen: {secilenler.Count})");
            }
        }

        private static void TakimlariOlustur(List<SecilenOyuncu> secilenler, out List<SecilenOyuncu> takimA, out List<SecilenOyuncu> takimB)
        {
            Random rnd = new Random();

            List<SecilenOyuncu> siraliListe = secilenler
                .OrderBy(o => NormalizeBolge(o.Bolge))
                .ThenByDescending(o => o.SeviyePuani)
                .ThenBy(o => rnd.Next())
                .ToList();

            takimA = new List<SecilenOyuncu>();
            takimB = new List<SecilenOyuncu>();

            Dictionary<string, int> sayacA = new Dictionary<string, int>();
            Dictionary<string, int> sayacB = new Dictionary<string, int>();

            int puanA = 0;
            int puanB = 0;

            foreach (SecilenOyuncu oyuncu in siraliListe)
            {
                if (takimA.Count == 7)
                {
                    TakimaEkle(takimB, sayacB, oyuncu, ref puanB);
                    continue;
                }

                if (takimB.Count == 7)
                {
                    TakimaEkle(takimA, sayacA, oyuncu, ref puanA);
                    continue;
                }

                int bolgeA = GetBolgeSayisi(sayacA, oyuncu.Bolge);
                int bolgeB = GetBolgeSayisi(sayacB, oyuncu.Bolge);

                bool ekleA;

                if (puanA == puanB)
                {
                    if (bolgeA == bolgeB)
                    {
                        ekleA = rnd.Next(2) == 0;
                    }
                    else
                    {
                        ekleA = bolgeA < bolgeB;
                    }
                }
                else
                {
                    ekleA = puanA < puanB;
                }

                if (ekleA)
                {
                    TakimaEkle(takimA, sayacA, oyuncu, ref puanA);
                }
                else
                {
                    TakimaEkle(takimB, sayacB, oyuncu, ref puanB);
                }
            }
        }

        private static void TakimaEkle(List<SecilenOyuncu> takim, Dictionary<string, int> sayac, SecilenOyuncu oyuncu, ref int toplamPuan)
        {
            takim.Add(oyuncu);
            toplamPuan += oyuncu.SeviyePuani;
            ArtirBolgeSayisi(sayac, oyuncu.Bolge);
        }

        private static int GetBolgeSayisi(Dictionary<string, int> sayac, string bolge)
        {
            int deger;
            return sayac.TryGetValue(NormalizeBolge(bolge), out deger) ? deger : 0;
        }

        private static void ArtirBolgeSayisi(Dictionary<string, int> sayac, string bolge)
        {
            string key = NormalizeBolge(bolge);

            if (sayac.ContainsKey(key))
            {
                sayac[key]++;
            }
            else
            {
                sayac[key] = 1;
            }
        }

        private static string NormalizeBolge(string bolge)
        {
            return (bolge ?? string.Empty).Trim().ToLowerInvariant();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    row.Cells[0].Value = false;
                }
            }

            lblSayac.Text = "Seçili oyuncu sayısı: 0 / 14";
            button3.Enabled = false;

            MessageBox.Show("Tüm seçimler temizlendi.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Tüm oyuncu listesi silinecek! Emin misiniz?", "Dikkat", MessageBoxButtons.YesNo);

            if (onay == DialogResult.Yes)
            {
                con.Open();
                MySqlCommand komut = new MySqlCommand("TRUNCATE TABLE oyuncular", con);
                komut.ExecuteNonQuery();
                con.Close();

                listele();

                lblSayac.Text = "Seçili oyuncu sayısı: 0 / 14";
                button3.Enabled = false;
            }
        }
    }
}
