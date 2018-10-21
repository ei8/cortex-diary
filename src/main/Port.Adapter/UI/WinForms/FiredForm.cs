
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.WinForms
{
    public partial class FiredForm : DockContent
    {
        public FiredForm(/*ISpikeResultsService resultsService*/)
        {
            InitializeComponent();

            //resultsService.Cleared += this.ResultsService_Cleared;
            //resultsService.FiredAdded += this.ResultsService_FiredAdded;
        }

        private void ResultsService_Cleared(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            this.toolStripLabel1.Text = string.Empty;
        }

        private void ResultsService_FiredAdded(object sender, /*SpikeResult*/ EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                // DEL: var item = this.listView1.Items.Add(new ListViewItem(new string[] {
                //    e.Result.Data,
                //    e.Result.Charge + " mV",
                //    e.Result.Id,
                //    e.Result.Timestamp.ToString(Properties.Settings.Default.TimestampFormat)
                //}));
                //if (this.autoscrollToolStripButton.Checked)
                //    item.EnsureVisible();

                this.toolStripLabel1.Text = $"{this.listView1.Items.Count} neuron(s)";
            }
            )
            );
        }
    }
}
