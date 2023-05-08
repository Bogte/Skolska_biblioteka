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

namespace Skolska_biblioteka
{
    public partial class Pozajmica : Form
    {
        DataTable podaci, pom;
        SqlCommand menjanja;

        public Pozajmica()
        {
            InitializeComponent();
        }

        private void Pozajmica_Load(object sender, EventArgs e)
        {
            podaci = new DataTable();//Dodavanje zaposlenik
            podaci = Konekcija.Unos("SELECT DISTINCT ime + ' ' + prezime AS 'Zaposleni' FROM Zaposleni");
            string[] pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Zaposleni"]);
                comboBox1.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje knjiga
            podaci = Konekcija.Unos("SELECT DISTINCT Knjiga.naziv AS 'Primerak' FROM Primerak JOIN Knjiga On Knjiga.id = Primerak.id_knjige");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Primerak"]);
                comboBox2.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje polica
            podaci = Konekcija.Unos("SELECT DISTINCT polica AS 'polica' FROM Primerak");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["polica"]);
                comboBox3.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje brojeva
            podaci = Konekcija.Unos("SELECT DISTINCT broj AS 'broj' FROM Primerak");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["broj"]);
                comboBox4.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje zaposlenik
            podaci = Konekcija.Unos("SELECT DISTINCT ime + ' ' + prezime AS 'Ucenik' FROM Ucenik");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Ucenik"]);
                comboBox5.Items.Add(pomocna[i]);
            }

            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT Pozajmica.id AS 'Id', convert(varchar(10), datum_uzimanja) AS 'Datum uzimanja', convert(varchar(10), datum_vracanja) AS 'Datum vracanja', Zaposleni.id AS 'Oznaka zaposlenog', Zaposleni.ime + ' ' + Zaposleni.prezime AS 'Zaposleni', Knjiga.naziv AS 'Knjiga', Primerak.polica AS 'Polica', Primerak.broj AS 'Broj', Ucenik.id AS 'Broj clanske katre', Ucenik.ime + ' ' + Ucenik.prezime AS 'Ucenik' FROM Pozajmica\r\nJOIN Zaposleni ON Zaposleni.id = Pozajmica.id_zaposlenog\r\nJOIN Primerak ON Primerak.id = Pozajmica.id_primerka\r\nJOIN Knjiga ON Knjiga.id = Primerak.id_knjige\r\nJOIN Ucenik ON Ucenik.id = Pozajmica.id_ucenika");
            dataGridView1.DataSource = podaci;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult poruka = MessageBox.Show("Da li ste pogresno uneli podatak?", "Pedikir manikir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (poruka == DialogResult.Yes)
            {
                menjanja = new SqlCommand();
                menjanja.CommandText = ("UPDATE Pozajmica SET datum_vracanja = GETDATE()");

                menjanja = new SqlCommand();
                menjanja.CommandText = ("DELETE FROM Pozajmica WHERE id = " + textBox1.Text);

                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Open();
                menjanja.Connection = con;
                menjanja.ExecuteNonQuery();
                con.Close();

                Osvezi();
            }
            else if (poruka == DialogResult.No)
            {
                menjanja = new SqlCommand();
                menjanja.CommandText = ("DELETE FROM Pozajmica WHERE id = " + textBox1.Text);

                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Open();
                menjanja.Connection = con;
                menjanja.ExecuteNonQuery();
                con.Close();

                Osvezi();
            }

        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Datum uzimanja"].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Datum vracanja"].Value);
                comboBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Zaposleni"].Value);
                comboBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjiga"].Value);
                comboBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Polica"].Value);
                comboBox4.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Broj"].Value);
                comboBox5.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Ucenik"].Value);
                textBox4.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Broj clanske katre"].Value);
                textBox5.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Oznaka zaposlenog"].Value);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da dodate ove podatke?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "" || comboBox5.Text == "" || textBox2.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    string[] Zaposleni = comboBox1.Text.Split(); //Trazenje id-a za zaposlenog
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Zaposleni WHERE ime = '" + Zaposleni[0] + "' AND prezime = '" + Zaposleni[1] + "' AND id = " + textBox5.Text);
                    int id_zaposlenog = (int)podaci.Rows[0][0];

                    podaci = new DataTable(); //Trazenje id-a za primerak
                    podaci = Konekcija.Unos("SELECT id FROM Primerak WHERE polica = " + comboBox3.Text + " AND broj = " + comboBox4.Text);
                    int id_primerka = (int)podaci.Rows[0][0];

                    string[] Ucenik = comboBox5.Text.Split(); //Trazenje id-a za ucenika
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Ucenik WHERE ime = '" + Ucenik[0] + "' AND prezime = '" + Ucenik[1] + "' AND id = " + textBox4.Text);
                    int id_ucenika = (int)podaci.Rows[0][0];

                    /*podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Knjiga WHERE naziv = '" + textBox2.Text + "' AND id_autora = " + id_autora + " AND id_izdavaca = " + id_izdavaca + " AND id_vrste = " + id_vrste);
                    if (podaci.Rows.Count >= 1) throw new Exception();*/

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Pozajmica (datum_uzimanja, id_zaposlenog, id_primerka, id_ucenika) VALUES ('" + textBox2.Text + "', " + id_zaposlenog + ", " + id_primerka + ", " + id_ucenika + ")");
                    SqlConnection con = new SqlConnection(Konekcija.Veza());
                    con.Open();
                    menjanja.Connection = con;
                    menjanja.ExecuteNonQuery();
                    con.Close();

                    Osvezi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source + " - " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Close();
                Osvezi();
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] pom = comboBox5.Text.Split();
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT id FROM Ucenik WHERE Ime = '" + pom[0] + "' AND Prezime = '" + pom[1] + "'");
            textBox4.Text = Convert.ToString(podaci.Rows[0][0]);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            podaci = new DataTable(); //Trazenje id-a za knjigu
            podaci = Konekcija.Unos("SELECT id FROM Knjiga WHERE naziv = '" + comboBox2.Text + "'");
            int id_knjige = (int)podaci.Rows[0][0];

            string pom = comboBox2.Text;
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT polica, broj FROM Primerak WHERE id_knjige = " + id_knjige + "AND slobodna = 1");
            comboBox3.Text = Convert.ToString(podaci.Rows[0][0]);
            comboBox4.Text = Convert.ToString(podaci.Rows[0][1]);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT id FROM Zaposleni WHERE ime + ' ' + prezime = '" + comboBox1.Text + "'");
            textBox5.Text = Convert.ToString(podaci.Rows[0][0]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da izmenite ove podatke?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "" || comboBox5.Text == "" || textBox2.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    //Trazenje id-a za zaposlenog
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Zaposleni WHERE ime + ' ' + prezime = '" + comboBox1.Text + "'");
                    int id_zaposlenog = (int)(podaci.Rows[0][0]);

                    podaci = new DataTable(); //Trazenje id-a za primerak
                    podaci = Konekcija.Unos("SELECT id FROM Primerak WHERE polica = " + comboBox3.Text + " AND broj = " + comboBox4.Text);
                    int id_primerka = (int)podaci.Rows[0][0];

                    //Trazenje id-a za ucenika
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Ucenik WHERE ime + ' ' + prezime = '" + comboBox5.Text + "'");
                    int id_ucenika = (int)podaci.Rows[0][0];

                    /*podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Pozajmica WHERE naziv = '" + textBox2.Text + "' AND id_autora = " + id_autora + " AND id_izdavaca = " + id_izdavaca + " AND id_vrste = " + id_vrste);
                    if (podaci.Rows.Count >= 1) throw new Exception();*/

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Pozajmica SET datum_uzimanja = '" + textBox2.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Pozajmica SET datum_vracanja = '" + textBox3.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Pozajmica SET id_zaposlenog = " + id_zaposlenog + " WHERE id = " + textBox1.Text +
                        " UPDATE Pozajmica SET id_primerka = " + id_primerka + " WHERE id = " + textBox1.Text +
                        " UPDATE Pozajmica SET id_ucenika = " + id_ucenika + " WHERE id = " + textBox1.Text);

                    SqlConnection con = new SqlConnection(Konekcija.Veza());
                    con.Open();
                    menjanja.Connection = con;
                    menjanja.ExecuteNonQuery();
                    con.Close();

                    Osvezi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source + " - " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Close();
                Osvezi();
            }
        }
    }
}
