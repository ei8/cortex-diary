using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Peripheral
{
    public partial class NeuronGraphView : UserControl, IViewFor<NeuronGraphViewModel>
    {
        private List<string> edges = new List<string>();

        public NeuronGraphView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x =>
                    {
                        this.ViewModel = (NeuronGraphViewModel)x;
                        this.ViewModel.PropertyChanged += this.ViewModel_PropertyChanged;
                    });
            });
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NeuronGraphViewModel.SelectedNeuron))
            {
                this.ContentPanel.Children.Clear();
                this.edges.Clear();

                GraphViewer graphViewer = new GraphViewer();
                graphViewer.BindToPanel(this.ContentPanel);
                Graph graph = new Graph();

                NeuronViewModelBase root = this.ViewModel.SelectedNeuron;

                while (root.Parent.HasValue)
                    root = root.Parent.Value;

                NeuronGraphView.AddNeuronAndChildren(root, this.ViewModel.SelectedNeuron, root, graph, this.edges);
                graph.Attr.LayerDirection = LayerDirection.TB;
                graphViewer.Graph = graph;
            }
        }

        private static void AddNeuronAndChildren(NeuronViewModelBase root, NeuronViewModelBase selectedNeuron, NeuronViewModelBase value, Graph graph, List<string> edges)
        {
            NeuronGraphView.AddSingleNeuron(root, selectedNeuron, value, graph);

            if (value.Parent.HasValue)
            {
                if (graph.FindNode(value.Parent.Value.NeuronId) == null)
                    NeuronGraphView.AddSingleNeuron(root, selectedNeuron, value.Parent.Value, graph);

                switch (value.Neuron.Type)
                {
                    case Domain.Model.Neurons.RelativeType.Postsynaptic:
                        NeuronGraphView.AddEdge(value.Parent.Value.NeuronId, value.NeuronId, graph, edges);
                        break;
                    case Domain.Model.Neurons.RelativeType.Presynaptic:
                    case Domain.Model.Neurons.RelativeType.NotSet:
                        NeuronGraphView.AddEdge(value.NeuronId, value.Parent.Value.NeuronId, graph, edges);
                        break;
                }
            }

            foreach (var c in value.Children)
                NeuronGraphView.AddNeuronAndChildren(root, selectedNeuron, c, graph, edges);
        }

        private static void AddEdge(string source, string target, Graph graph, List<string> edges)
        {
            string edgeId = $"{source}-{target}";
            if (!edges.Contains(edgeId))
            {
                var e = graph.AddEdge(source, target);
                edges.Add(edgeId);
            }
        }

        private static void AddSingleNeuron(NeuronViewModelBase root, NeuronViewModelBase selectedNeuron, NeuronViewModelBase value, Graph graph)
        {
            var n = graph.AddNode(value.NeuronId);
            n.LabelText = value.Tag;
            if (selectedNeuron == value)
            {
                var mfc = SystemColors.HighlightColor;
                var mtc = SystemColors.HighlightTextColor;
                n.Attr.FillColor = new Color(mfc.A, mfc.R, mfc.G, mfc.B);
                n.Label.FontColor = new Color(mtc.A, mtc.R, mtc.G, mtc.B);
            }
            else if (root == value)
            {
                var mc = SystemColors.HighlightColor;
                n.Attr.Color = new Color(mc.A, mc.R, mc.G, mc.B);
                n.Attr.LineWidth *= 1.5;
            }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NeuronGraphViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(NeuronGraphViewModel), typeof(NeuronGraphView), new PropertyMetadata(default(NeuronGraphViewModel)));

        public NeuronGraphViewModel ViewModel
        {
            get { return (NeuronGraphViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
