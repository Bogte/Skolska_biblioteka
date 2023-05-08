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
    public partial class Primerci : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Primerci()
        {
            InitializeComponent();
        }

        private void Primerci_Load(object sender, EventArgs e)
        {
            podaci = new DataTable();//Dodavanje knjiga
            podaci = Konekcija.Unos("SELECT DISTINCT naziv AS 'Knjiga' FROM Knjiga");
            string[] pomocna = new string[podaci.Rows.Count];
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
            podaci = Konekcija.Unos("SELECT Primerak.id AS 'Id', Knjiga.naziv AS 'Knjiga', polica, broj, slobodna FROM Primerak\r\nJOIN Knjiga ON Knjiga.id = Primerak.id_knjige");
            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["polica"].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["broj"].Value);
                comboBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjiga"].Value);
                if (Convert.ToString(dataGridView1.Rows[indeks].Cells["Slobodna"].Value) == "True")
                {
                    comboBox2.Text = "Slobodna";
                }
                else
                {
                    comboBox2.Text = "Nije slobodna";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE FROM Primerak WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || textBox3.Text == "" )
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable(); //Trazenje id-a za knjigu
                    podaci = Konekcija.Unos("SELECT id FROM Knjiga WHERE naziv = '" + comboBox1.Text + "'");
                    int id_knjige = (int)podaci.Rows[0][0];

                    int slobodna;
                    if(comboBox2.Text == "Slobodna")
                    {
                        slobodna = 1;
                    }
                    else
                    {
                        slobodna = 0;
                    }

                    podaci = Konekcija.Unos("SELECT polica, broj FROM Primerak WHERE id <> " + textBox1.Text);
                    for (int i = 0; i < podaci.Rows.Count; i++)
                    {
                        if (textBox2.Text == Convert.ToString(podaci.Rows[i]["polica"]) && textBox3.Text == Convert.ToString(podaci.Rows[i]["broj"]))
                        {
                            MessageBox.Show("Polica je zauzeta! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Osvezi();
                            return;
                        }
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Primerak WHERE id_knjige = " + id_knjige + " AND polica = " + textBox2.Text + " AND broj = " + textBox3.Text + " AND slobodna = " + slobodna);
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Primerak SET id_knjige = " + id_knjige + " WHERE id = " + textBox1.Text +
                        " UPDATE Primerak SET polica = " + textBox2.Text + " WHERE id = " + textBox1.Text +
                        " UPDATE Primerak SET broj = " + textBox3.Text + " WHERE id = " + textBox1.Text +
                        " UPDATE Primerak SET slobodna = " + slobodna + " WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || textBox3.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable(); //Trazenje id-a za knjigu
                    podaci = Konekcija.Unos("SELECT id FROM Knjiga WHERE naziv = '" + comboBox1.Text + "'");
                    int id_knjige = (int)podaci.Rows[0][0];

                    int slobodna;
                    if (comboBox2.Text == "Slobodna")
                    {
                        slobodna = 1;
                    }
                    else
                    {
                        slobodna = 0;
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Primerak WHERE id_knjige = " + id_knjige + " AND polica = " + textBox2.Text + " AND broj = " + textBox3.Text + " AND slobodna = " + slobodna);
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Primerak VALUES ('" + id_knjige + "', " + textBox2.Text + ", " + textBox3.Text + ", " + slobodna + ")");
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
