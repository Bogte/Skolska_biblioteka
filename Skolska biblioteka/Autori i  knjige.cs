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
    public partial class Autori_i__knjige : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Autori_i__knjige()
        {
            InitializeComponent();
        }

        private void Autori_i__knjige_Load(object sender, EventArgs e)
        {
            podaci = new DataTable();//Dodavanje autora
            podaci = Konekcija.Unos("SELECT DISTINCT ime + ' ' + prezime AS 'Autor' FROM Autor");
            string[] pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Autor"]);
                comboBox2.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje knjiga
            podaci = Konekcija.Unos("SELECT DISTINCT naziv AS 'Knjiga' FROM Knjiga");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Knjiga"]);
                comboBox1.Items.Add(pomocna[i]);
            }

            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT Autor_knjiga.id AS 'Id', Knjiga.naziv AS 'Knjiga', Autor.ime + ' ' + Autor.prezime AS 'Autor' FROM Autor_knjiga\r\nJOIN Autor ON Autor.id = Autor_knjiga.id_autora\r\nJOIN Knjiga ON Knjiga.id = Autor_knjiga.id_knjige");
            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                comboBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjiga"].Value);
                comboBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Autor"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Pedikir manikir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE Autor_knjiga FROM  WHERE id = " + textBox1.Text);

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
                if (MessageBox.Show("Da li ste sigurni da zelite da izmenite ove podatke?", "Pedikir manikir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (comboBox1.Text == "" || comboBox2.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    string[] Autor = comboBox2.Text.Split(); //Trazenje id-a za autora
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT id FROM Autor WHERE ime = '" + Autor[0] + "' AND prezime = '" + Autor[1] + "'");
                    int id_autora = (int)podaci.Rows[0][0];

                    podaci = new DataTable(); //Trazenje id-a za knjizevnu vrstu
                    podaci = Konekcija.Unos("SELECT id FROM Knjiga WHERE naziv = '" + comboBox1.Text + "'");
                    int id_knjige = (int)podaci.Rows[0][0];

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Autor_knjiga WHERE id_autora = " + id_autora + " AND id_knjige = " + id_knjige);
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Autor_knjiga SET id_autora = " + id_autora + " WHERE id = " + textBox1.Text +
                        " UPDATE Autor_knjiga SET id_knjige = " + id_knjige + " WHERE id = " + textBox1.Text);

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
    }
}
