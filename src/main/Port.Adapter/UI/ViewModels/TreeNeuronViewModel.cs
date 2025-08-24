using ei8.Cortex.Diary.Application.Mirrors;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public enum ExpansionType
    {
        None,
        PostsynapticUntilExternalReferences,
        FarthestPresynaptic
    }

    public class TreeNeuronViewModel
    {
        private string avatarUrl;
        private INeuronQueryService neuronQueryService;
        private Timer expansionTimer;
        private ExpansionType currentExpansionType = ExpansionType.None;
        private readonly IEnumerable<MirrorConfigFile> mirrorConfigFiles;

        public TreeNeuronViewModel(
            Neuron neuron,
            string avatarUrl,
            INeuronQueryService neuronQueryService,
            IEnumerable<MirrorConfigFile> mirrorConfigFiles
        ) : this
        (
            neuron,
            avatarUrl,
            neuronQueryService,
            mirrorConfigFiles,
            0
        )
        {
        }

        public TreeNeuronViewModel(
            Neuron neuron,
            string avatarUrl,
            INeuronQueryService neuronQueryService,
            IEnumerable<MirrorConfigFile> mirrorConfigFiles,
            int rootIndex = 0
        )
        {
            this.Neuron = neuron;
            this.avatarUrl = avatarUrl;
            this.neuronQueryService = neuronQueryService;
            this.mirrorConfigFiles = mirrorConfigFiles;
            this.Children = new List<TreeNeuronViewModel>();
            this.RootIndex = rootIndex;
        }

        public IList<TreeNeuronViewModel> Children { get; set; }

        public Neuron Neuron { get; private set; }

        public ExpansionState ExpansionState { get; private set; }

        public int RootIndex { get; set; }

        public ExpansionType CurrentExpansionType => this.currentExpansionType;

        public async Task Toggle()
        {
            this.ExpansionState = this.ExpansionState == ExpansionState.Collapsed ? ExpansionState.Expanding : ExpansionState.Collapsed;

            if (this.ExpansionState == ExpansionState.Expanding)
            {
                var children = new List<TreeNeuronViewModel>();
                if (Library.Client.QueryUrl.TryParse(this.avatarUrl, out QueryUrl result))
                {
                    NeuronQuery.TryParse(result.QueryString, out NeuronQuery query);
                    var childrenQuery = new NeuronQuery()
                    {
                        PageSize = Constants.TreeNodeChildrenQueryPageSize,
                        RelativeValues = query != null ? query.RelativeValues : null
                    };
                    (await this.neuronQueryService.GetNeurons(result.AvatarUrl, this.Neuron.Id, childrenQuery))
                        .Items
                        .ToList().ForEach(n =>
                        children.Add(new TreeNeuronViewModel(
                            new Neuron(n),
                            this.avatarUrl,
                            this.neuronQueryService,
                            this.mirrorConfigFiles,
                            this.RootIndex
                        ))
                    );

                    this.Children = children.ToArray();
                }
                this.ExpansionState = ExpansionState.Expanded;
            }
        }

        public void ConfigureExpansionTimer(Timer timer, ExpansionType type, double interval, ElapsedEventHandler handler)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            this.expansionTimer = timer;
            this.currentExpansionType = type;
            this.expansionTimer.Interval = interval;
            this.expansionTimer.Elapsed -= handler;
            this.expansionTimer.Elapsed += handler;
        }

        public IEnumerable<Tuple<string, string>> GetMirrorKeys()
        {
            var result = this.mirrorConfigFiles
                .Where(mco => mco.Mirrors.Any(m => m.Url == this.Neuron.ExternalReferenceUrl))
                .SelectMany(mco => 
                    mco.Mirrors.Where(mi => mi.Url == this.Neuron.ExternalReferenceUrl).Select(mi2 =>
                        Tuple.Create(
                            mi2.Key,
                            mco.Path
                        )
                    )
                );

            return result;
        }

        public void StartExpansionTimer()
        {
            if (this.expansionTimer == null)
                throw new InvalidOperationException("Expansion timer not set. Call SetExpansionTimer first.");

            this.expansionTimer.Start();
        }

        public void StopExpansionTimer()
        {
            if (this.expansionTimer == null)
                throw new InvalidOperationException("Expansion timer not set. Call SetExpansionTimer first.");

            this.expansionTimer.Stop();
            this.currentExpansionType = ExpansionType.None;
        }

        public void RestartExpansionTimer()
        {
            if (this.expansionTimer == null)
                throw new InvalidOperationException("Expansion timer not set. Call SetExpansionTimer first.");

            this.expansionTimer.Enabled = false;
            this.StartExpansionTimer();
        }

        public bool IsExpansionTimerEnabled()
        {
            if (this.expansionTimer == null)
                return false;

            return this.expansionTimer.Enabled;
        }

        public bool IsChild(string id, RelativeType type)
        {
            var result = this.Children.Any(x => (x.Neuron.Id == id && x.Neuron.Type == type) || x.IsChild(id, type));
            return result;
        }

        public bool IsChild(string id)
        {
            var result = this.Children.Any(x => (x.Neuron.Id == id && x.Neuron.Type == RelativeType.Postsynaptic) || x.IsChild(id));
            return result;
        }
    }
}
