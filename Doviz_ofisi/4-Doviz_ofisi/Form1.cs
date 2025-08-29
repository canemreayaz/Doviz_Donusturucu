using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml; // bunu ekledik
namespace _4_Doviz_ofisi
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("Data Source=AYAZ;Initial Catalog=dovizUygulaması;User ID=sa; password = 1");
        private void Form1_Load(object sender, EventArgs e)
        {
            string bugun = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var xmlDosya = new XmlDocument();
            xmlDosya.Load(bugun);

            // yazdırma işlemi
            string dolarAlis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod = 'USD']/BanknoteBuying").InnerXml;
            lblDolarAlis.Text = dolarAlis;
            string dolarSatis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod = 'USD']/BanknoteSelling").InnerXml;
            lblDolarSatis.Text = dolarSatis;

            string euroAlis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod = 'EUR']/BanknoteBuying").InnerXml;
            lblEuroAlis.Text = euroAlis;
            string euroSatis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod = 'EUR']/BanknoteSelling").InnerXml;
            lblEuroSatis.Text = euroSatis;


        }

        private void btnDolarAlis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarAlis.Text;
        }

        private void btnDolarSatis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarSatis.Text;
        }

        private void btnEuroAlis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroAlis.Text ;
        }

        private void btnEuroSatis_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroSatis.Text ;
        }

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            double kur, miktar, tutar;

            kur = Convert.ToDouble(txtKur.Text);
            miktar = Convert.ToDouble(txtMiktar.Text);
            tutar = kur * miktar;
            txtTutar.Text = tutar.ToString();

         

            con.Open();
            SqlCommand cmd = new SqlCommand("update dovizTablosu set TL = TL - @azaltmaMiktari where id = @id", con);
            cmd.Parameters.AddWithValue("@azaltmaMiktari", Convert.ToDouble(txtTutar.Text));
            cmd.Parameters.AddWithValue("@id", 1);
            int sonuc = cmd.ExecuteNonQuery();
            con.Close();

            if (sonuc == 1)
                MessageBox.Show("TL başarılı bir şekilde azaltıldı.");
            else
                MessageBox.Show("TL azaltılamadı. Yeterli bakiye olmayabilir.");
        
           

        }

        private void txtKur_TextChanged(object sender, EventArgs e)
        {
            // replace -> istediğin karakteri istediğin karakterle değiştiriyor noktayı virgül ile değiştir
            txtKur.Text = txtKur.Text.Replace(".", ",");
        }

        private void btn_dovizAl_Click(object sender, EventArgs e)
        {
            try
            {
                // Giriş değerlerini kontrol et ve dönüştür
                double kur = Convert.ToDouble(txtKur.Text);
                int miktar = Convert.ToInt32(txtMiktar.Text);

                // Tutar ve kalan hesapla
                int tutar = Convert.ToInt32(miktar / kur);
                txtTutar.Text = tutar.ToString();

                double kalan = miktar % kur;
                txtKalan.Text = kalan.ToString();

                // Kullanıcının döviz seçimini kontrol et
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Lütfen bir döviz türü seçin.");
                    return;
                }

                // Seçilen döviz türünü al
                string selectedCurrency = comboBox1.SelectedItem.ToString();

                // Veritabanı bağlantısını aç
                con.Open();

                // SQL sorgusunu dinamik olarak oluştur
                SqlCommand cmd = new SqlCommand($"UPDATE dovizTablosu SET {selectedCurrency} = {selectedCurrency} - @azaltmaMiktari WHERE id = @id", con);
                cmd.Parameters.AddWithValue("@azaltmaMiktari", Convert.ToDouble(txtTutar.Text));
                cmd.Parameters.AddWithValue("@id", 1);

                int sonuc = cmd.ExecuteNonQuery();
                con.Close();

                // İşlem sonucunu kontrol et
                if (sonuc == 1)
                    MessageBox.Show($"{selectedCurrency} başarılı bir şekilde azaltıldı.");
                else
                    MessageBox.Show($"{selectedCurrency} azaltılamadı. Yeterli bakiye olmayabilir.");
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen geçerli bir sayı girin.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }

        }
        /*
         *Bu durumda, selectedCurrency değişkeninin değeri (örneğin, "DOLAR" veya "EURO"), stringin içinde
         *{selectedCurrency} şeklinde kullanılarak dinamik olarak SQL sorgusuna yerleştirilir.

                Özet
                Dolar işareti ($) ile başlayan stringler, interpolated stringlerdir.
                Bu özellik, stringler içinde değişken veya ifadeleri kolayca kullanmanıza olanak tanır.
                String interpolasyonu, kodunuzu daha okunabilir ve yönetilebilir hale getirir.
         */
        private void btnDolarSatis_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}
