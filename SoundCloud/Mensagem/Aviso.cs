using System;
using System.Windows.Forms;

namespace SoundCloud.Mensagem
{
    public partial class Aviso : Form
    {
        public Aviso()
        {
            InitializeComponent();

        }

        public void Carrega(string extensao, double tamanho)
        {
            lblExtensao.Text = extensao;
            lblTamanho.Text = $"{tamanho} MB";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
