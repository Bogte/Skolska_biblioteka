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
    public partial class Zaposleni : Form
    {
        DataTable podaci;
        SqlCommand menjanja;

        public Zaposleni()
        {
            InitializeComponent();
        }

        private void Zaposleni_Load(object sender, EventArgs e)
        {
            Osvezi();
        }

        private void Osvezi()
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("SELECT id, ime + ' ' + prezime AS 'Zaposleni', convert(varchar(10), datum_zaposlenja) AS 'Datum zaposlenja', JMBG, email, plata, Lozinka FROM Zaposleni");
            dataGridView1.DataSource = podaci;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da obrisete ove podatake?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("DELETE FROM Zaposleni WHERE id = " + textBox1.Text);

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

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["id"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Zaposleni"].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Datum zaposlenja"].Value);
                textBox4.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["JMBG"].Value);
                textBox5.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["email"].Value);
                textBox6.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["plata"].Value);
                textBox7.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Lozinka"].Value);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Da li ste sigurni da zelite da izmenite ove podatke?", "Skolska biblioteka", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    if (textBox4.Text.Length != 13)
                    {
                        MessageBox.Show("JMBG mora imati 13 cifara! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT JMBG FROM Zaposleni WHERE id <> " + textBox1.Text);
                    for (int i = 0; i < podaci.Rows.Count; i++)
                    {
                        if (textBox4.Text == Convert.ToString(podaci.Rows[i]["JMBG"]))
                        {
                            MessageBox.Show("Dva zaposlena ne smeju imati isti JMBG! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Osvezi();
                            return;
                        }
                    }

                    podaci = Konekcija.Unos("SELECT email FROM Zaposleni WHERE id <> " + textBox1.Text);
                    for (int i = 0; i < podaci.Rows.Count; i++)
                    {
                        if (textBox5.Text == Convert.ToString(podaci.Rows[i]["email"]))
                        {
                            MessageBox.Show("Dva zaposlena ne smeju imati isti Email! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Osvezi();
                            return;
                        }
                    }

                    string[] zaposleni = textBox2.Text.Split();
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Zaposleni WHERE ime = '" + zaposleni[0] + "' AND prezime = '" + zaposleni[1] + "' AND JMBG = '" + textBox4.Text + "' AND email = '" + textBox5.Text + "' AND datum_zaposlenja = '" + textBox3.Text + "' AND plata = " + textBox6.Text + " AND Lozinka = " + textBox7.Text);
                    if (podaci.Rows.Count >= 1) throw new Exception();

                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("UPDATE Zaposleni SET ime = '" + zaposleni[0] + "' WHERE id = " + textBox1.Text +
                        " UPDATE Zaposleni SET prezime = '" + zaposleni[1] + "' WHERE id = " + textBox1.Text +
                        " UPDATE Zaposleni SET datum_zaposlenja = '" + textBox3.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Zaposleni SET JMBG = '" + textBox4.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Zaposleni SET email = '" + textBox5.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Zaposleni SET plata = '" + textBox6.Text + "' WHERE id = " + textBox1.Text +
                        " UPDATE Zaposleni SET Lozinka = '" + textBox7.Text + "' WHERE id = " + textBox1.Text);

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
                    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "")
                    {
                        MessageBox.Show("Sva polja moraju biti popunjena - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    if (textBox4.Text.Length != 13)
                    {
                        MessageBox.Show("JMBG mora imati 13 cifara! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Zaposleni WHERE JMBG = '" + textBox4.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva zaposlena ne smeju imati isti JMBG! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    podaci = Konekcija.Unos("SELECT * FROM Zaposleni WHERE email = '" + textBox5.Text + "'");
                    if (podaci.Rows.Count >= 1)
                    {
                        MessageBox.Show("Dva zaposlena ne smeju imati isti Email! - Skolska biblioteka", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Osvezi();
                        return;
                    }

                    string[] zaposleni = textBox2.Text.Split();
                    podaci = new DataTable();
                    podaci = Konekcija.Unos("SELECT * FROM Zaposleni WHERE ime = '" + zaposleni[0] + "' AND prezime = '" + zaposleni[1] + "' AND JMBG = '" + textBox4.Text + "' AND email = '" + textBox5.Text + "' AND datum_zaposlenja = '" + textBox3.Text + "' AND plata = " + textBox6.Text + " AND Lozinka = " + textBox7.Text);
                    if (podaci.Rows.Count >= 1) throw new Exception();


                    menjanja = new SqlCommand();
                    menjanja.CommandText = ("INSERT INTO Zaposleni VALUES ('" + zaposleni[0] + "', '" + zaposleni[1] + "', '" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "', " + textBox6.Text + ", " + textBox7.Text + ")");
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
