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
    public partial class Izdavaci : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Izdavaci()
        {
            InitializeComponent();
        }

        private void Izdavaci_Load(object sender, EventArgs e)
        {
            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT id, naziv AS 'Izdavac', adresa, veb_sajt AS 'Veb sajt' FROM Izdavac");
            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Izdavac"].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["adresa"].Value);
                textBox4.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Veb sajt"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE FROM Izdavac WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT veb_sajt FROM Izdavac WHERE id <> " + textBox1.Text);
                    for (int i = 0; i < podaci.Rows.Count; i++)
                    {
                        if (textBox4.Text == Convert.ToString(podaci.Rows[i]["veb_sajt"]))
                        {
                            MessageBox.Show("Dva izdavaca ne smeju imati isti veb sajt! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Osvezi();
                            return;
                        }
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Izdavac WHERE naziv = '" + textBox2.Text + "' AND adresa = '" + textBox3.Text + "' AND veb_sajt = '" + textBox4.Text + "'");
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Izdavac SET naziv = '" + textBox2.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Izdavac SET adresa = '" + textBox3.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Izdavac SET veb_sajt = '" + textBox4.Text + "' WHERE id = " + textBox1.Text);

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
                MessageBox.Show("Podatak vec postoji u tabeli - " + ex.Source, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Izdavac WHERE veb_sajt = '" + textBox4.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva izdavaca ne smeju imati isti veb sajt! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Izdavac WHERE naziv = '" + textBox2.Text + "' AND adresa = '" + textBox3.Text + "' AND veb_sajt = '" + textBox4.Text + "'");
                    if (podaci.Rows.Count >= 1) throw new Exception();


                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Izdavac VALUES ('" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox2.Text + "')");
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
                MessageBox.Show("Ne mozete da dodate vec postojece podatke! - " + ex.Source, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SqlConnection con = new SqlConnection(Konekcija.Veza());
                con.Close();
                Osvezi();
            }
        }
    }
}
