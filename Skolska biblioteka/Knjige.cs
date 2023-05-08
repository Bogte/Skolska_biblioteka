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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Skolska_biblioteka
{
    public partial class Knjige : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Knjige()
        {
            InitializeComponent();
        }

        private void Knjige_Load(object sender, EventArgs e)
        {
            podaci = new DataTable();//Dodavanje autora
            podaci = Konekcija.Unos("SELECT DISTINCT ime + ' ' + prezime AS 'Autor' FROM Autor");
            string[] pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Autor"]);
                comboBox1.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje izdavaca
            podaci = Konekcija.Unos("SELECT DISTINCT naziv AS 'Izdavac' FROM Izdavac");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Izdavac"]);
                comboBox2.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje knjizevnih vrsta
            podaci = Konekcija.Unos("SELECT DISTINCT Naziv AS 'Vrsta' FROM Knjizevna_vrsta");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Vrsta"]);
                comboBox3.Items.Add(pomocna[i]);
            }

            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT Knjiga.id, Knjiga.naziv AS 'Knjiga', Autor.ime + ' ' + Autor.prezime AS 'Autor', Izdavac.naziv AS 'Izdavac', Knjizevna_vrsta.naziv AS 'Knjizevna vrsta' FROM Knjiga\r\nJOIN Izdavac ON Izdavac.id = Knjiga.id_izdavaca\r\nJOIN Autor ON Autor.id = Knjiga.id_autora\r\nJOIN Knjizevna_vrsta ON Knjizevna_vrsta.id = Knjiga.id_vrste");
            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjiga"].Value);
                comboBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Autor"].Value);
                comboBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Izdavac"].Value);
                comboBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjizevna vrsta"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE FROM Knjiga WHERE id = " + textBox1.Text);

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
                MessageBox.Show("Ne mozete da obrisete ove podatake, druge tabele zahtevaju ove podatake! - " + ex.Source, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Close();
                Osvezi();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da izmenite ove podatke?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    //Trazenje id-a za autora
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Autor WHERE ime + ' ' + prezime = '" + comboBox1.Text + "'");
                    int id_autora = (int)podaci.Rows[0][0];

                    podaci = new DataTable(); //Trazenje id-a za izdavaca
                    podaci = Konekcija.Unos("SELECT id FROM Izdavac WHERE naziv = '" + comboBox2.Text + "'");
                    int id_izdavaca = (int)podaci.Rows[0][0];

                    podaci = new DataTable(); //Trazenje id-a za knjizevnu vrstu
                    podaci = Konekcija.Unos("SELECT id FROM Knjizevna_vrsta WHERE naziv = '" + comboBox3.Text + "'");
                    int id_vrste = (int)podaci.Rows[0][0];

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Knjiga WHERE naziv = '" + textBox2.Text + "' AND id_autora = " + id_autora + " AND id_izdavaca = " + id_izdavaca + " AND id_vrste = " + id_vrste);
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Knjiga SET naziv = '" + textBox2.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Knjiga SET id_autora = '" + id_autora + "' WHERE id = " + textBox1.Text +
                        " UPDATE Knjiga SET id_izdavaca = '" + id_izdavaca + "' WHERE id = " + textBox1.Text +
                        " UPDATE Knjiga SET id_vrste = '" + id_vrste + "' WHERE id = " + textBox1.Text);

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
                MessageBox.Show("Podatak vec postoji u tabeli - " + ex.Source + " - " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Close();
                Osvezi();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da dodate ove podatke?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    //Trazenje id-a za autora
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Autor WHERE ime + ' ' + prezime = '" + comboBox1.Text + "'");
                    int id_autora = (int)podaci.Rows[0][0];

                    podaci = new DataTable(); //Trazenje id-a za izdavaca
                    podaci = Konekcija.Unos("SELECT id FROM Izdavac WHERE naziv = '" + comboBox2.Text + "'");
                    int id_izdavaca = (int)podaci.Rows[0][0];

                    podaci = new DataTable(); //Trazenje id-a za knjizevnu vrstu
                    podaci = Konekcija.Unos("SELECT id FROM Knjizevna_vrsta WHERE naziv = '" + comboBox3.Text + "'");
                    int id_vrste = (int)podaci.Rows[0][0];

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Knjiga WHERE naziv = '" + textBox2.Text + "' AND id_autora = " + id_autora + " AND id_izdavaca = " + id_izdavaca + " AND id_vrste = " + id_vrste);
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Knjiga VALUES ('" + textBox2.Text + "', " + id_autora + ", " + id_izdavaca + ", " + id_vrste + ")");
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
                MessageBox.Show("Podatak vec postoji u tabeli - " + ex.Source + " - " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Close();
                Osvezi();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
