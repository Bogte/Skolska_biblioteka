using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skolska_biblioteka
{


    public partial class Pocetna_stranica : Form
    {
        public Pocetna_stranica()
        {
            InitializeComponent();
        }

        private void Pocetna_stranica_Load(object sender, EventArgs e)
        {

        }

        private void knjizevnaVrstaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Knjizevne_vrste f1 = new Knjizevne_vrste();
            f1.ShowDialog();
        }

        private void autorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Autori f1 = new Autori();
            f1.ShowDialog();
        }

        private void izdavacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Izdavaci f1 = new Izdavaci();
            f1.ShowDialog();
        }

        private void uceniciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ucenici f1 = new Ucenici();
            f1.ShowDialog();
        }

        private void zaposleniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Zaposleni f1 = new Zaposleni();
            f1.ShowDialog();
        }

        private void knjigaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Knjige f1 = new Knjige();
            f1.ShowDialog();
        }
    }
}
