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
    public partial class Ucenici : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Ucenici()
        {
            InitializeComponent();
        }

        private void Ucenici_Load(object sender, EventArgs e)
        {
            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT id, ime + ' ' + prezime AS 'Ucenik', JMBG, email, godina_pocetka AS 'Godina upisa', odeljenje FROM Ucenik");
            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Ucenik"].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["JMBG"].Value);
                textBox4.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["email"].Value);
                textBox5.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Godina upisa"].Value);
                textBox6.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["odeljenje"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE FROM Ucenik WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    if (textBox3.Text.Length != 13)
                    {
                        MessageBox.Show("JMBG mora imati 13 cifara! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Ucenik WHERE JMBG = '" + textBox3.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva ucenika ne smeju imati isti JMBG! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Ucenik WHERE email = '" + textBox4.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva ucenika ne smeju imati isti Email! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    string[] ucenik = textBox2.Text.Split();
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Ucenik WHERE ime = '" + ucenik[0] + "' AND prezime = '" + ucenik[1] + "' AND JMBG = '" + textBox3.Text + "' AND email = '" + textBox4.Text + "' AND godina_pocetka = '" + textBox5.Text + "' AND odeljenje = '" + textBox6.Text + "'");
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Ucenik SET ime = '" + ucenik[0] + "' WHERE id = " + textBox1.Text +
                        " UPDATE Ucenik SET prezime = '" + ucenik[1] + "' WHERE id = " + textBox1.Text +
                        " UPDATE Ucenik SET JMBG = '" + textBox3.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Ucenik SET email = '" + textBox4.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Ucenik SET godina_pocetka = '" + textBox5.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Ucenik SET odeljenje = '" + textBox6.Text + "' WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    if (textBox3.Text.Length != 13)
                    {
                        MessageBox.Show("JMBG mora imati 13 cifara! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Ucenik WHERE JMBG = '" + textBox3.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva ucenika ne smeju imati isti JMBG! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Ucenik WHERE email = '" + textBox4.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva ucenika ne smeju imati isti Email! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    string[] ucenik = textBox2.Text.Split();
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Ucenik WHERE ime = '" + ucenik[0] + "' AND prezime = '" + ucenik[1] + "' AND JMBG = '" + textBox3.Text + "' AND email = '" + textBox4.Text + "' AND godina_pocetka = '" + textBox5.Text + "' AND odeljenje = '" + textBox6.Text + "'");
                    if (podaci.Rows.Count >= 1) throw new Exception();


                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Ucenik VALUES ('" + ucenik[0] + "', '" + ucenik[1] + "', '" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "', '" + textBox6.Text + "')");
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
