using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.WinForms
{
    public partial class EulaForm : Form
    {
        public EulaForm()
        {
            InitializeComponent();
        }

        // Load the EULA file.
        private void EulaForm_Load(object sender, EventArgs e)
        {
            rchEula.LoadFile("Eula.rtf");
        }
    }
}
