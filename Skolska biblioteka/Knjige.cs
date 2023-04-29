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
            Osvezi();
        }

        private void Osvezi()
        {
            {
                podaci = new DataTable();
                podaci = Konekcija.Unos("SELECT Knjiga.id, Knjiga.naziv AS 'Knjiga', Autor.ime + ' ' + Autor.prezime AS 'Autor', Izdavac.naziv AS 'Izdavaca', Knjizevna_vrsta.naziv AS 'Knjizevna vrsta ' FROM Knjiga\r\nJOIN Izdavac ON Izdavac.id = Knjiga.id_izdavaca\r\nJOIN Autor ON Autor.id = Knjiga.id_autora\r\nJOIN Knjizevna_vrsta ON Knjizevna_vrsta.id = Knjiga.id");
                dataGridView1.DataSource = podaci;
            }
        }

    }
}
