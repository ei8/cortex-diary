﻿using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Common;
using IdentityModel.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels
{
    public class NeuronViewModel
    {
        private string avatarUrl;
        private INeuronQueryService neuronQueryService;
        private string bearerToken;

        public NeuronViewModel(UINeuron neuron, string avatarUrl, string bearerToken, INeuronQueryService neuronQueryService)
        {
            this.Neuron = neuron;
            this.Tag = neuron.Tag;
            this.avatarUrl = avatarUrl;
            this.bearerToken = bearerToken;
            this.neuronQueryService = neuronQueryService;
        }

        public NeuronViewModel[] Children { get; set; }

        public UINeuron Neuron { get; private set; }

        public string Tag { get; private set; }

        public bool IsExpanded { get; private set; }

        public bool AreControlsVisible { get; private set; }

        public async Task Toggle()
        {
            this.IsExpanded = !this.IsExpanded;

            if (this.IsExpanded)
                await this.OnReload();
        }

        public void ShowControls(bool visible)
        {
            this.AreControlsVisible = visible;
        }

        public string GetIcon()
        {
            if (this.IsExpanded)
            {
                return "-";
            }

            return "+";
        }

        public async Task OnReload()
        {
            this.IsExpanded = true;
            var children = new List<NeuronViewModel>();
            if (Library.Client.QueryUrl.TryParse(this.avatarUrl, out QueryUrl result))
            {
                (await this.neuronQueryService.GetNeurons(result.AvatarUrl, this.Neuron.Id, new NeuronQuery()))
                    .Neurons
                    .ToList().ForEach(n =>
                    children.Add(new NeuronViewModel(new UINeuron(n), this.avatarUrl, this.bearerToken, this.neuronQueryService))
                );
                this.Children = children.ToArray();
            }
        }
    }
}