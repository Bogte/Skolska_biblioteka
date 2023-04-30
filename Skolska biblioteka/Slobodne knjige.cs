using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Skolska_biblioteka
{
    public partial class Slobodne_knjige : Form
    {
        DataTable podaci;

        public Slobodne_knjige()
        {
            InitializeComponent();
        }

        private void Slobodne_knjige_Load(object sender, EventArgs e)
        {
            podaci = new DataTable();//Dodavanje knjizevnih vrsta
            podaci = Konekcija.Unos("SELECT DISTINCT Knjizevna_vrsta.naziv AS 'Vrsta' FROM Primerak JOIN Knjiga ON Knjiga.id = Primerak.id_knjige JOIN Knjizevna_vrsta ON Knjizevna_vrsta.id = Knjiga.id_vrste");
            string[] pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Vrsta"]);
                comboBox1.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje knjiga
            podaci = Konekcija.Unos("SELECT DISTINCT Knjiga.naziv AS 'Knjiga' FROM Primerak JOIN Knjiga ON Knjiga.id = Primerak.id_knjige");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Knjiga"]);
                comboBox2.Items.Add(pomocna[i]);
            }

            podaci = new DataTable();//Dodavanje autora
            podaci = Konekcija.Unos("SELECT DISTINCT Autor.ime + ' ' + Autor.prezime AS 'Autor' FROM Primerak JOIN Knjiga ON Knjiga.id = Primerak.id_knjige JOIN Autor ON Autor.id = Knjiga.id_autora");
            pomocna = new string[podaci.Rows.Count];
            for (int i = 0; i < podaci.Rows.Count; i++)
            {
                pomocna[i] = Convert.ToString(podaci.Rows[i]["Autor"]);
                comboBox3.Items.Add(pomocna[i]);
            }

            Osvezi();
        }
        
        private void Osvezi()
        {
            button2.Enabled = false;
            podaci = new DataTable();
            podaci = Konekcija.Unos("exec prikazi_slobodne_knjige");
            dataGridView1.DataSource = podaci;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int indeks = dataGridView1.CurrentRow.Index;

                textBox1.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Id primerka"].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Naziv knjige"].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Autor"].Value);
                textBox4.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["Knjizevna vrsta"].Value);
                textBox5.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["polica"].Value);
                textBox6.Text = Convert.ToString(dataGridView1.Rows[indeks].Cells["broj"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("exec prikazi_slobodne_knjige_po_vrsti '" + comboBox1.Text + "'");
            dataGridView1.DataSource = podaci;
            button2.Enabled = true;
        }
 
        private void button2_Click(object sender, EventArgs e)
        {
            Osvezi();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("exec fn_slobodne_knjige_po_nazivu '" + comboBox2.Text + "'");
            dataGridView1.DataSource = podaci;
            button2.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            podaci = new DataTable();
            podaci = Konekcija.Unos("exec prikazi_slobodne_knjige_po_autoru '" + comboBox3.Text + "'");
            dataGridView1.DataSource = podaci;
            button2.Enabled = true;
        }
    }
}
