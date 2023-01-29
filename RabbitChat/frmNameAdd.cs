using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabbitChat
{
    public partial class frmNameAdd : Form
    {
        public frmNameAdd()
        {
            InitializeComponent();
        }

        public string _name;
        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (txtName.Text == "")
                    return;

                _name = txtName.Text;
                this.Close();
            }
        }
    }
}
