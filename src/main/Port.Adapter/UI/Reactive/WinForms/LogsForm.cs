// TODO: using Spiker.SpikeResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms
{
    public partial class LogsForm : DockContent
    {
        private TextChangedTimer textChangedTimer;
        public LogsForm(/* ISpikeResultsService resultsService */)
        {
            InitializeComponent();
            this.textChangedTimer = new TextChangedTimer();
            this.textChangedTimer.Idled += this.TextChangedTimer_Idled;
            // DEL: resultsService.Cleared += this.ResultsService_Cleared;
        }

        private void ResultsService_Cleared(object sender, EventArgs e)
        {
            this.clearButton_Click(this, EventArgs.Empty);
        }

        private void TextChangedTimer_Idled(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (this.autoscrollToolStripButton.Checked && this.logTextBox.Text.Length > 0)
                {
                    this.logTextBox.Select(this.logTextBox.Text.Length - 1, 0);
                    this.logTextBox.ScrollToCaret();
                }
            }
            ));
        }

        private void logTextBox_TextChanged(object sender, EventArgs e)
        {
            this.textChangedTimer.StartWaitTimer();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            this.logTextBox.Clear();
        }
    }
}
