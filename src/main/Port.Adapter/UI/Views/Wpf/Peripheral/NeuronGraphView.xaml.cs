/*
    This file is part of the d# project.
    Copyright (c) 2016-2018 ei8
    Authors: ei8
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    EI8. EI8 DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS
     This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    https://github.com/ei8/cortex-diary/blob/master/LICENSE
     The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
     You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the d# software without
    disclosing the source code of your own applications.
     For more information, please contact ei8 at this address: 
     support@ei8.works
 */

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
        private GraphViewer graphViewer;

        public NeuronGraphView()
        {
            InitializeComponent();

            this.graphViewer = new GraphViewer();
            this.graphViewer.BindToPanel(this.ContentPanel);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => this.ViewModel = (NeuronGraphViewModel)x);

                d(this.OneWayBind(this.ViewModel, vm => vm.LayoutOptions, v => v.Layout.ItemsSource, vmp => vmp.Select(s => new MenuItem() { Header = s, IsCheckable = true, Style = Resources["MenuItemStyle1"] as System.Windows.Style })));

                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    ev => this.Layout.Click += ev,
                    ev => this.Layout.Click -= ev
                    ).Subscribe(ep =>
                    {
                        this.ViewModel.Layout = this.Layout.Items.IndexOf(ep.EventArgs.OriginalSource);
                        this.Layout.Items.Cast<MenuItem>().ToList().ForEach(mi => mi.IsChecked = false);
                        ((MenuItem)ep.EventArgs.OriginalSource).IsChecked = true;
                    });

                Observable.FromEventPattern<EventHandler<MsaglMouseEventArgs>, MsaglMouseEventArgs>(
                    ev => this.graphViewer.MouseDown += ev,
                    ev => this.graphViewer.MouseDown -= ev
                    ).Subscribe(ep =>
                    {
                        var gv = ep.Sender as GraphViewer;
                        if (gv != null && gv.ObjectUnderMouseCursor != null && gv.ObjectUnderMouseCursor.DrawingObject is Node)
                        {
                            var vo = gv.ObjectUnderMouseCursor;
                            if (vo != null && vo.DrawingObject != null && vo.DrawingObject is Node)
                            {
                                var node = (Node)vo.DrawingObject;
                                this.ViewModel.InternallySelectedNeuronId = node.Id;
                                gv.Graph.Nodes.ToList().ForEach(n => NeuronGraphView.FillIfNotExternallySelected(
                                    n, 
                                    this.ViewModel.ExternallySelectedNeuron.NeuronId, 
                                    NeuronGraphView.ConvertColorToMsaglColor(SystemColors.WindowColor),
                                    1
                                    ));

                                NeuronGraphView.FillIfNotExternallySelected(
                                    node, 
                                    this.ViewModel.ExternallySelectedNeuron.NeuronId, 
                                    NeuronGraphView.ConvertColorToMsaglColor(Color.Yellow, 80),
                                    1
                                    );

                                var poc = NeuronGraphView.ConvertColorToMsaglColor(Color.LightGreen, 90);
                                var prc = NeuronGraphView.ConvertColorToMsaglColor(Color.PowderBlue, 150);
                                node.Edges.ToList().ForEach(e =>
                                {
                                    if (e.SourceNode == node)
                                        NeuronGraphView.FillIfNotExternallySelected(
                                            e.TargetNode,
                                            this.ViewModel.ExternallySelectedNeuron.NeuronId,
                                            poc,
                                            2
                                            );                                        
                                    else
                                        NeuronGraphView.FillIfNotExternallySelected(
                                            e.SourceNode,
                                            this.ViewModel.ExternallySelectedNeuron.NeuronId,
                                            prc,
                                            2
                                            );
                                });
                            }

                            this.ViewModel.SelectCommand.Execute().Subscribe();
                        }
                    });

                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    ev => this.ViewModel.PropertyChanged += ev,
                    ev => this.ViewModel.PropertyChanged -= ev
                    ).Subscribe(ep =>
                    {
                        if (ep.EventArgs.PropertyName == nameof(NeuronGraphViewModel.ExternallySelectedNeuron))
                        {
                            this.graphViewer.Graph = null;
                            Graph graph = new Graph();
                            graph.Attr.LayerDirection = (LayerDirection)this.ViewModel.Layout;

                            NeuronViewModelBase root = this.ViewModel.ExternallySelectedNeuron;

                            while (root.Parent.HasValue)
                                root = root.Parent.Value;

                            NeuronGraphView.AddNeuronAndChildren(root, this.ViewModel.ExternallySelectedNeuron, root, graph);
                            this.graphViewer.Graph = graph;
                        }
                    });
            });
        }

        private static void FillIfNotExternallySelected(Node n, string externalId, Color color, double lineWidth)
        {
            if (n.Id != externalId)
            {
                n.Attr.FillColor = color;
                n.Attr.LineWidth = lineWidth;
            }
        }

        private static Color ConvertColorToMsaglColor(System.Windows.Media.Color value)
        {
            return NeuronGraphView.ConvertColorToMsaglColor(new Color(value.R, value.G, value.B), value.A);
        }

        private static Color ConvertColorToMsaglColor(Color value, int alpha)
        {
            return new Color((byte) alpha, value.R, value.G, value.B);
        }

        private static void AddNeuronAndChildren(NeuronViewModelBase root, NeuronViewModelBase selectedNeuron, NeuronViewModelBase value, Graph graph)
        {
            NeuronGraphView.AddSingleNeuron(root, selectedNeuron, value, graph);

            if (value.Parent.HasValue)
            {
                if (graph.FindNode(value.Parent.Value.NeuronId) == null)
                    NeuronGraphView.AddSingleNeuron(root, selectedNeuron, value.Parent.Value, graph);

                switch (value.Neuron.Type)
                {
                    case Domain.Model.Neurons.RelativeType.Postsynaptic:
                        NeuronGraphView.AddEdge(selectedNeuron, value.TerminalId, value.Parent.Value.NeuronId, value.NeuronId, graph, value.Neuron.Terminal.Strength, value.Neuron.Terminal.Effect == "-1");
                        break;
                    case Domain.Model.Neurons.RelativeType.Presynaptic:
                        NeuronGraphView.AddEdge(selectedNeuron, value.TerminalId, value.NeuronId, value.Parent.Value.NeuronId, graph, value.Neuron.Terminal.Strength, value.Neuron.Terminal.Effect == "-1");
                        break;
                    case Domain.Model.Neurons.RelativeType.NotSet:
                        NeuronGraphView.AddEdge(selectedNeuron, value.NeuronId, value.NeuronId, value.Parent.Value.NeuronId, graph);
                        break;
                }
            }

            if (value.Children != null)
                foreach (var c in value.Children)
                    NeuronGraphView.AddNeuronAndChildren(root, selectedNeuron, c, graph);
        }

        private static void AddEdge(NeuronViewModelBase selectedNeuron, string id, string source, string target, Graph graph, string strength = "1", bool inhibitoryEndpoint = false)
        {
            if (graph.Edges.SingleOrDefault(e => e.Attr.Id == id) == null)                
            {
                Edge e = null;
                if (strength != "1")
                {
                    e = graph.AddEdge(source, strength, target);
                    e.Label.FontSize = e.Label.FontSize * 0.8;
                    e.Attr.AddStyle(Microsoft.Msagl.Drawing.Style.Dashed);
                }
                else
                    e = graph.AddEdge(source, target);

                e.Attr.Id = id;
                if (selectedNeuron.TerminalId == id)
                    e.Attr.Color = NeuronGraphView.GetHighlightColor();
                else if (inhibitoryEndpoint)
                    e.Attr.Color = Color.IndianRed;                
            }
        }

        private static void AddSingleNeuron(NeuronViewModelBase root, NeuronViewModelBase selectedNeuron, NeuronViewModelBase value, Graph graph)
        {
            var n = graph.AddNode(value.NeuronId);
            n.LabelText = value.Tag;
            if (selectedNeuron == value)
            {
                var mtc = SystemColors.HighlightTextColor;
                n.Attr.FillColor = NeuronGraphView.GetHighlightColor();
                n.Label.FontColor = new Color(mtc.A, mtc.R, mtc.G, mtc.B);
            }
            else if (root == value)
            {
                n.Attr.Color = NeuronGraphView.GetHighlightColor();
                n.Attr.LineWidth *= 1.5;
            }
        }

        private static Color GetHighlightColor()
        {
            var mfc = SystemColors.HighlightColor;
            var hc = new Color(mfc.A, mfc.R, mfc.G, mfc.B);
            return hc;
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
