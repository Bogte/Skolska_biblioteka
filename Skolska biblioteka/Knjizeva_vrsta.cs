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
    public partial class Knjizevne_vrste : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Knjizevne_vrste()
        {
            InitializeComponent();
        }

        private void Knjizevne_vrste_Load(object sender, EventArgs e)
        {
            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT id, naziv AS 'Knjizevna vrsta' FROM Knjizevna_vrsta");

            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjizevna vrsta"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE FROM Knjizevna_vrsta WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable();

                    podaci = Konekcija.Unos("SELECT * FROM Knjizevna_vrsta WHERE naziv = '" + textBox2.Text + "'");
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Knjizevna_vrsta SET naziv = '" + textBox2.Text + "' WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Knjizevna_vrsta WHERE naziv = '" + textBox2.Text + "'");
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Knjizevna_vrsta VALUES ('" + textBox2.Text + "')");

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
