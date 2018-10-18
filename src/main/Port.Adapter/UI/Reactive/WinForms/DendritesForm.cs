// DEL: using Spiker.Neurons;
// using Spiker.SpikeResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms
{
    public partial class DendritesForm : DockContent
    {
        // DEL: private NeuronCollection neurons;
        private ISelectionService selectionService;

        public DendritesForm(ISelectionService selectionService/*, NeuronCollection neurons */)
        {
            InitializeComponent();

            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
            // DEL: this.neurons = neurons;
        }

        private void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (Helper.IsSelectionNeuron(this.selectionService))
            {
                this.listView1.Items.Clear();
                // DEL: this.selectionService.SelectedObjects.ToList()
                //    .SelectMany(n =>
                //    {
                //        var ne = (Neuron)n;
                //        var ds = DendritesForm.GetDendrites(ne.Id, neurons);
                //        return ds.Select(d => new string[] {
                //        d.Data,
                //        d.Terminals.First(t => t.TargetId == ne.Id).Strength.ToString(),
                //        d.Threshold.ToString(),
                //        d.Id
                //        }
                //        );
                //    })
                //    .ToList().ForEach(ss => this.listView1.Items.Add(new ListViewItem(ss)));

                this.countToolStripStatusLabel.Text = $"{this.listView1.Items.Count} dendrite(s)";
            }
        }

        // DEL: private static IEnumerable<Neuron> GetDendrites(string id, NeuronCollection neurons)
        //{
        //    return neurons.ToList().Where(n => n.Terminals.Where(t => t.TargetId == id).Count() > 0);
        //}

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                // DEL: this.selectionService.SetSelectedObjects(new Neuron[] { (Neuron)this.neurons[this.listView1.SelectedItems[0].SubItems[3].Text] });
            }
        }

        private void showAllToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
            {
                // DEL: this.selectionService.SetSelectedObjects(this.listView1.Items.Cast<ListViewItem>().Select(lvi => (Neuron)this.neurons[lvi.SubItems[3].Text]));
            }
        }
    }
}
