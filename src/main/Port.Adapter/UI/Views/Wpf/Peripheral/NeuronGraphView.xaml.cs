using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Peripheral
{
    public partial class NeuronGraphView : UserControl, IViewFor<NeuronGraphViewModel>
    {
        private List<string> edges = new List<string>();
        private GraphViewer graphViewer;

        public NeuronGraphView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => (this.ViewModel = (NeuronGraphViewModel)x).PropertyChanged += this.ViewModel_PropertyChanged);

                d(this.OneWayBind(this.ViewModel, vm => vm.LayoutOptions, v => v.Layout.ItemsSource, vmp => vmp.Select(s => new MenuItem() { Header = s, IsCheckable = true, Style=Resources["MenuItemStyle1"] as System.Windows.Style })));

                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    ev => this.Layout.Click += ev,
                    ev => this.Layout.Click -= ev
                    ).Subscribe(ep =>
                    {
                        this.ViewModel.Layout = this.Layout.Items.IndexOf(ep.EventArgs.OriginalSource);
                        this.Layout.Items.Cast<MenuItem>().ToList().ForEach(mi => mi.IsChecked = false);
                        ((MenuItem)ep.EventArgs.OriginalSource).IsChecked = true;
                    });
            });
        }

        // DEL: Use Observable.FromEventPattern
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NeuronGraphViewModel.ExternallySelectedNeuron))
            {
                this.ContentPanel.Children.Clear();
                this.edges.Clear();

                if (this.graphViewer != null)
                    this.graphViewer.MouseDown -= this.GraphViewer_MouseDown;

                this.graphViewer = new GraphViewer();
                this.graphViewer.MouseDown += this.GraphViewer_MouseDown;
                this.graphViewer.BindToPanel(this.ContentPanel);

                Graph graph = new Graph();

                NeuronViewModelBase root = this.ViewModel.ExternallySelectedNeuron;

                while (root.Parent.HasValue)
                    root = root.Parent.Value;

                NeuronGraphView.AddNeuronAndChildren(root, this.ViewModel.ExternallySelectedNeuron, root, graph, this.edges);
                graph.Attr.LayerDirection = (LayerDirection) this.ViewModel.Layout;
                this.graphViewer.Graph = graph;
            }
        }

        // DEL: Use Observable.FromEventPattern
        private void GraphViewer_MouseDown(object sender, MsaglMouseEventArgs e)
        {
            if (this.graphViewer != null && this.graphViewer.ObjectUnderMouseCursor != null && this.graphViewer.ObjectUnderMouseCursor.DrawingObject is Node)
            {
                var vo = this.graphViewer.ObjectUnderMouseCursor;
                if (vo != null && vo.DrawingObject != null && vo.DrawingObject is Node)
                    this.ViewModel.InternallySelectedNeuronId = ((Node)vo.DrawingObject).Id;

                this.ViewModel.SelectCommand.Execute().Subscribe();
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
