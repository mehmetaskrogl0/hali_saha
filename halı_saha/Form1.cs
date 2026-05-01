using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace halı_saha
{
    public partial class Form1 : Form

    {
        // MySQL bağlantısı (Senin bilgilerine göre düzenlendi)
        MySqlConnection con = new MySqlConnection("Server=localhost;Database=halı_saha;Uid=root;Pwd=Mehmet042");
        DataTable dt = new DataTable();

        // Listeleme fonksiyonu (Aynen senin mantığın)
        public void listele()
        {
            dt.Clear();
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter("select * from oyuncular", con);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
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
            public string Seviye { get; set; }
            // Checkbox için bu property'e ihtiyacımız yok, onu DataGridView kendisi yönetecek.
        }
        public class SecilenOyuncu
        {
            public string Isim { get; set; }
            public string Bolge { get; set; }
            public int SeviyePuani { get; set; } // 1, 2, 3 gibi sayısal değer
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Veritabanı bağlantısını açıyoruz
            con.Open();

            // SQL sorgusu (ID otomatik artıyor, onu yazmıyoruz)
            // Parametreleri (p1, p2, p3) senin görseldeki sırana göre ayarladım
            MySqlCommand komut = new MySqlCommand("INSERT INTO oyuncular (isim, bolge, seviye) VALUES (@p1, @p2, @p3)", con);

            komut.Parameters.AddWithValue("@p1", textBox1.Text);   // Oyuncu Adı
            komut.Parameters.AddWithValue("@p2", comboBox1.Text);  // Mevki (Defans, Forvet vs)
            komut.Parameters.AddWithValue("@p3", comboBox2.Text); // Yetenek Seviyesi

            komut.ExecuteNonQuery(); // Sorguyu çalıştır
            con.Close(); // Bağlantıyı kapat

            listele(); // DataGridView'i güncelle

            // Temizlik: Alanları boşaltalım
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

            MessageBox.Show("Oyuncu başarıyla kadroya eklendi.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Eğer tabloda seçili bir satır varsa işleme başla
            if (dataGridView1.CurrentRow != null)
            {
                con.Open();

                // Tablodaki "id" sütunundan değeri alıyoruz
                MySqlCommand komut = new MySqlCommand("DELETE FROM oyuncular WHERE OyuncuID = @id", con);
                komut.Parameters.AddWithValue("@id", dataGridView1.CurrentRow.Cells["OyuncuID"].Value);

                komut.ExecuteNonQuery();
                con.Close();

                listele(); // Tabloyu yenile
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
                    if (Convert.ToBoolean(row.Cells[0].Value) == true)
                    {
                        secilenSayisi++;
                    }
                }

                // Formdaki label'ı güncelle (Görseldeki 0/14 kısmı)
                lblSayac.Text = $"Seçili oyuncu sayısı: {secilenSayisi} / 14";

                if (secilenSayisi > 14)
                {
                    MessageBox.Show("Maksimum 14 kişi seçebilirsiniz!");
                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = false; // Seçimi geri al
                }
                if (secilenSayisi == 14)
                    button3.Enabled = true;
                else
                    button3.Enabled = false;
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
                // Satırın yeni satır olmadığını ve seçili olduğunu kontrol et
                if (!row.IsNewRow && row.Cells[0].Value != null && Convert.ToBoolean(row.Cells[0].Value) == true)
                {
                    // DBNull ve Null kontrolleri ile verileri güvenli al
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
                Random rnd = new Random();

                // Önce mevkilere göre grupla, sonra mevkiler içinde yeteneğe göre sırala
                // Yetenekleri aynı olanları rastgele karıştır (OrderBy(x => rnd.Next()))
                var dengeliSiraliListe = secilenler
                    .OrderBy(o => o.Bolge)
                    .ThenByDescending(o => o.SeviyePuani)
                    .ThenBy(x => rnd.Next())
                    .ToList();

                List<string> takimA = new List<string>();
                List<string> takimB = new List<string>();

                // Zig-zag dağıtım: 1. oyuncu A'ya, 2. oyuncu B'ya, 3. oyuncu B'ya, 4. oyuncu A'ya (Daha adil denge)
                for (int i = 0; i < dengeliSiraliListe.Count; i++)
                {
                    // Basit mod alma yerine çift/tek mantığıyla dağıt
                    if (i % 2 == 0) takimA.Add(dengeliSiraliListe[i].Isim);
                    else takimB.Add(dengeliSiraliListe[i].Isim);
                }

                Form2 frm2 = new Form2(takimA, takimB);
                frm2.Show();
            }
            else
            {
                MessageBox.Show($"Lütfen tam 14 kişi seçin! (Şu an seçilen: {secilenler.Count})");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // 0. sütundaki CheckBox değerlerini false yap
                if (row.Cells[0].Value != null)
                {
                    row.Cells[0].Value = false;
                }
            }

            // Sayaç ve butonu da sıfırla
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

                // DataGridView'i güncelle (Veritabanı boş olduğu için içi boşalacak)
                listele();

                lblSayac.Text = "Seçili oyuncu sayısı: 0 / 14";
                button3.Enabled = false;
            }
        }
    }
}
