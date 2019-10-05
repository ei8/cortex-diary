using DynamicData.Binding;
using Newtonsoft.Json;
using org.neurul.Common.Events;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Notifications;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications
{
    public class NotificationsPaneViewModel : PaneViewModel, IAvatarViewer
    {
        private readonly INotificationApplicationService notificationApplicationService;
        private readonly INeuronGraphQueryClient neuronGraphQueryClient;
        private IExtendedSelectionService selectionService;
        private readonly IStatusService statusService;
        private readonly IDialogService dialogService;
        private static readonly IDictionary<string, Neuron> neuronCache = new Dictionary<string, Neuron>();

        public NotificationsPaneViewModel(INotificationApplicationService neuronApplicationService = null, INeuronGraphQueryClient neuronGraphQueryClient = null, IExtendedSelectionService selectionService = null, IStatusService statusService = null, IDialogService dialogService = null)
        {
            this.notificationApplicationService = neuronApplicationService ?? Locator.Current.GetService<INotificationApplicationService>();
            this.neuronGraphQueryClient = neuronGraphQueryClient ?? Locator.Current.GetService<INeuronGraphQueryClient>();
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>(SelectionContract.Select.ToString());
            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();

            var selector = this.WhenPropertyChanged(p => p.SelectedNotification)
                .Where(p => p != null)
                .Subscribe(x =>
                {
                    this.selectionService.SetSelectedComponents(new object[] { x.Value });
                    if (x.Value != null && Array.IndexOf(new string[] { EventTypeNames.NeuronCreated.ToString(), EventTypeNames.NeuronTagChanged.ToString() }, x.Value.Type) > -1)
                        this.Target = NotificationsPaneViewModel.ConvertNotificationViewModelToEditorNeuron(x.Value);
                });

            this.statusService.WhenPropertyChanged(s => s.Message)
                .Subscribe(s => this.StatusMessage = s.Sender.Message);

            this.LoadCommand = ReactiveCommand.Create(async () => await this.OnLoadClicked());
            var canMore = this.WhenAnyValue<NotificationsPaneViewModel, bool, NotificationLog>(x => x.NotificationLog, nl => nl != null && nl.PreviousNotificationLogId != null);
            this.MoreCommand = ReactiveCommand.Create(async () => await this.OnMoreClicked(), canMore);
            this.SetLayerCommand = ReactiveCommand.Create<object>(async (parameter) => await this.OnSetLayerIdClicked(parameter));

            this.Loading = false;
            this.IconSourcePath = @"pack://application:,,,/Dasz;component/images/notification.ico";            
        }

        private static EditorNeuronData ConvertNotificationViewModelToEditorNeuron(NotificationViewModel n)
        {
            return new EditorNeuronData(
                n.Id,
                n.Tag,
                (NeurotransmitterEffect?)null,
                (float?)null,
                (Domain.Model.Neurons.RelativeType?)null,
                string.Empty,
                string.Empty,
                n.ExpectedVersion
            );
        }

        private async Task OnLoadClicked()
        {
            this.Loading = true;

            await ViewModels.Neurons.Helper.SetStatusOnComplete(async () =>
            {
                this.NotificationLog = await this.notificationApplicationService.GetNotificationLog(this.AvatarUrl, string.Empty);
                this.Notifications = await NotificationsPaneViewModel.UpdateCacheGetNotifications(this.NotificationLog, this.neuronGraphQueryClient, this.AvatarUrl);

                this.InitLayer();
                return true;
            },
                "Load successful.",
                this.statusService
            );

            this.Loading = false;
        }

        private async Task OnSetLayerIdClicked(object parameter)
        {
            await ViewModels.Neurons.Helper.SetStatusOnComplete(async () =>
            {
                bool stat = false;

                if ((await this.dialogService.ShowDialogSelectNeurons("Select Layer Neuron", this.AvatarUrl, parameter, false, out IEnumerable<Neuron> result)).GetValueOrDefault())
                {
                    this.LayerName = result.First().Tag;
                    this.LayerId = result.First().NeuronId;
                    stat = true;
                }
                else
                    this.InitLayer();

                return stat;
            },
                "Layer set successfully.",
                this.statusService,
                "Layer set cancelled."
                );
        }

        private void InitLayer()
        {
            this.LayerName = "[Base]";
            this.LayerId = Guid.Empty.ToString();
        }

        private static async Task<IEnumerable<NotificationViewModel>> UpdateCacheGetNotifications(NotificationLog notificationLog, INeuronGraphQueryClient neuronGraphQueryClient, string avatarUrl)
        {
            var ids = new List<string>();
            var ns = notificationLog.NotificationList.ToList();
            ns.ForEach(n =>
            {
                ids.Add(n.AuthorId);
                ids.Add(n.Id);
                dynamic d = JsonConvert.DeserializeObject(n.Data);
                // NeuronCreated
                if (n.TypeName.Contains(EventTypeNames.NeuronCreated.ToString()))
                {
                    // LayerId                    
                    ids.Add(d.LayerId.ToString());
                }
                // TerminalCreated
                else if (n.TypeName.Contains(EventTypeNames.TerminalCreated.ToString()))
                {
                    // PresynapticNeuronId
                    ids.Add(d.PresynapticNeuronId.ToString());
                    // PostsynapticNeuronId
                    ids.Add(d.PostsynapticNeuronId.ToString());
                }
            }
            );
            ids.RemoveAll(i => NotificationsPaneViewModel.neuronCache.ContainsKey(i));

            (await neuronGraphQueryClient.GetNeurons(avatarUrl, neuronQuery: new NeuronQuery() { Id = ids.ToArray() }))
                .ToList()
                .ForEach(n => NotificationsPaneViewModel.neuronCache.Add(n.NeuronId, n));

            return notificationLog.NotificationList.ToArray().Select(n => new NotificationViewModel(n, NotificationsPaneViewModel.neuronCache));
        }

        private async Task OnMoreClicked()
        {
            this.Loading = true;

            await ViewModels.Neurons.Helper.SetStatusOnComplete(async () =>
            {
                this.NotificationLog = await this.notificationApplicationService.GetNotificationLog(this.AvatarUrl, this.NotificationLog.PreviousNotificationLogId);
                this.Notifications = (await NotificationsPaneViewModel.UpdateCacheGetNotifications(this.NotificationLog, this.neuronGraphQueryClient, this.AvatarUrl)).Concat(this.Notifications);
                return true;
            },
                "Load more successful.",
                this.statusService
            );

            this.Loading = false;
        }

        [Reactive]
        public IEnumerable<NotificationViewModel> Notifications { get; set; }

        [Reactive]
        public NotificationLog NotificationLog { get; set; }

        [Reactive]
        public string LayerId { get; set; }
        
        [Reactive]
        public string LayerName { get; set; }

        [Reactive]
        public string AvatarUrl { get; set; }

        public ReactiveCommand<Unit, Task> LoadCommand { get; }

        public ReactiveCommand<Unit, Task> MoreCommand { get; }

        public ReactiveCommand<object, Unit> SetLayerCommand { get; }

        [Reactive]
        public string StatusMessage { get; set; }

        [Reactive]
        public NotificationViewModel SelectedNotification { get; set; }

        [Reactive]
        public bool Loading { get; set; }

        [Reactive]
        public EditorNeuronData Target { get; set; }
    }
}
