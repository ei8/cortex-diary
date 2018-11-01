//The MIT License(MIT)

//Copyright(c) 2016 Chris Condron

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
// https://github.com/condron/ReactiveUI-TreeView
// Modifications copyright(C) 2018 ei8/Elmer Bool

using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public abstract class NeuronViewModelBase : ReactiveObject, IDisposable, IEquatable<NeuronViewModelBase>
    {
        private readonly IDisposable cleanUp;
        private int id;
        private string neuronId;
        private string data;
        private bool isExpanded;
        private bool isSelected;
        private ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private IExtendedSelectionService selectionService;
        private readonly INeuronService neuronService;
        private readonly INeuronQueryService neuronQueryService;

        protected NeuronViewModelBase(Node<Neuron, int> node, SourceCache<Neuron, int> cache, NeuronViewModelBase parent = null, INeuronService neuronService = null, INeuronQueryService neuronQueryService = null, IExtendedSelectionService selectionService = null)
        {
            this.Id = node.Key;
            this.NeuronId = node.Item.NeuronId;
            this.Data = node.Item.Data;
            this.Parent = parent;
            this.Neuron = node.Item;

            this.ReloadCommand = ReactiveCommand.Create(async () => await this.Reload(cache));
            this.AddPostsynapticCommand = ReactiveCommand.Create(() => this.neuronService.AddPostsynaptic(cache, this.Neuron));
            this.AddPresynapticCommand = ReactiveCommand.Create(() => this.neuronService.AddPresynaptic(cache, this.Neuron));
            this.DeleteCommand = ReactiveCommand.Create(() => this.neuronService.Delete(cache, this.Neuron));

            this.neuronService = neuronService ?? Locator.Current.GetService<INeuronService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>();

            var childrenLoader = new Lazy<IDisposable>(() => node.Children.Connect()
                .Transform(e => 
                    e.Item.Type == RelativeType.Postsynaptic ? 
                    (NeuronViewModelBase)(new PostsynapticViewModel(e.Item.Data, e, cache, this)) : 
                    (NeuronViewModelBase)(new PresynapticViewModel(e.Item.Data, e, cache, this)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe()
                );

            var shouldExpand = node.IsRoot ?
                Observable.Return(true) :
                Parent.Value.WhenValueChanged(t => t.IsExpanded);

            var expander = shouldExpand
                .Where(isExpanded => isExpanded)
                .Take(1)
                .Subscribe(_ =>
                {
                    var x = childrenLoader.Value;
                });

            var childrenCount = node.Children.CountChanged
                .Select(count =>
                {
                    if (count == 0)
                        return "0 Synapses";
                    else
                        return $"{node.Children.Items.Count(n => n.Item.Type == RelativeType.Postsynaptic)} Postsynaptic; " +
                            $"{node.Children.Items.Count(n => n.Item.Type == RelativeType.Presynaptic)} Presynaptic";
                })
                .Subscribe(text => this.ChildrenCountText = text);

            var changeData = this.WhenPropertyChanged(p => p.Data, false)
                .Subscribe(x => this.neuronService.ChangeData(cache, this.Neuron, x.Value));

            var selector = this.WhenPropertyChanged(p => p.IsSelected)
                .Where(p => p.Value)
                .Subscribe(x => this.selectionService.SetSelectedComponents(new object[] { x.Sender })); 

            this.cleanUp = Disposable.Create(() =>
            {
                expander.Dispose();
                childrenCount.Dispose();
                if (childrenLoader.IsValueCreated)
                    childrenLoader.Value.Dispose();
                selector.Dispose();
            });
        }

        private async Task Reload(SourceCache<Neuron, int> cache)
        {
            cache.Remove(cache.Items.Where(i => i.CentralId == this.Neuron.Id));
            var relatives = await this.neuronQueryService.GetAll(this.Neuron);
            cache.AddOrUpdate(relatives);
        }

        public Neuron Neuron { get; }

        private string childrenCountText;

        public string ChildrenCountText
        {
            get => this.childrenCountText;
            set => this.RaiseAndSetIfChanged(ref this.childrenCountText, value);
        }

        public int Id
        {
            get => this.id;
            set => this.RaiseAndSetIfChanged(ref this.id, value);
        }
                
        [ParenthesizePropertyName(true)]
        public string NeuronId
        {
            get => this.neuronId;
            set => this.RaiseAndSetIfChanged(ref this.neuronId, value);
        }
                
        public string Data
        {
            get => this.data;
            set => this.RaiseAndSetIfChanged(ref this.data, value);
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.RaiseAndSetIfChanged(ref isExpanded, value);
        }

        public bool IsSelected
        {
            get => this.isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        public ReactiveCommand ReloadCommand { get; }
        
        public ReactiveCommand AddPostsynapticCommand { get; }

        public ReactiveCommand AddPresynapticCommand { get; }

        public ReactiveCommand DeleteCommand { get; }

        public abstract object ViewModel { get; }

        public ReadOnlyObservableCollection<NeuronViewModelBase> Children => this.children;

        public Optional<NeuronViewModelBase> Parent { get; }

        public override string ToString()
        {
            return $"{this.NeuronId}:Neuron '{this.Data}'";
        }

        public void Dispose()
        {
            this.cleanUp.Dispose();
        }

        #region Equality Members

        public bool Equals(NeuronViewModelBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NeuronViewModelBase) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(NeuronViewModelBase left, NeuronViewModelBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NeuronViewModelBase left, NeuronViewModelBase right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
