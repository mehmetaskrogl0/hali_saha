using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace halı_saha
{
    public partial class Form2 : Form
    {
        List<string> _takimA;
        List<string> _takimB;

        // Constructor'ı (yapıcı metot) listeyi alacak şekilde değiştiriyoruz
        public Form2(List<string> takimA, List<string> takimB)
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
            // Liste kutularını her ihtimale karşı temizleyelim
            lstTakim1.Items.Clear();
            lstTakim2.Items.Clear();

            // Null Kontrolü: Eğer Form1'den liste gelmediyse program çökmesin
            if (_takimA != null && _takimB != null)
            {
                foreach (var oyuncu in _takimA)
                {
                    lstTakim1.Items.Add(oyuncu);
                }

                foreach (var oyuncu in _takimB)
                {
                    lstTakim2.Items.Add(oyuncu);
                }
            }
            else
            {
                MessageBox.Show("Oyuncu listesi alınamadı!");
            }
        }
      
    }

}

