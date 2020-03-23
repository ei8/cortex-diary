using DynamicData.Binding;
using org.neurul.Cortex.Common;
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
using works.ei8.Cortex.Diary.Common;
using works.ei8.Cortex.Diary.Nucleus.Client.Out;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Common;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications
{
    public class NotificationsPaneViewModel : PaneViewModel, IAvatarViewer
    {
        private readonly INotificationApplicationService notificationApplicationService;
        private readonly INeuronQueryClient neuronGraphQueryClient;
        private IExtendedSelectionService selectionService;
        private readonly IStatusService statusService;
        private readonly IDialogService dialogService;
        private static readonly IDictionary<string, Neuron> neuronCache = new Dictionary<string, Neuron>();

        public NotificationsPaneViewModel(INotificationApplicationService neuronApplicationService = null, INeuronQueryClient neuronGraphQueryClient = null, IExtendedSelectionService selectionService = null, IStatusService statusService = null, IDialogService dialogService = null)
        {
            this.notificationApplicationService = neuronApplicationService ?? Locator.Current.GetService<INotificationApplicationService>();
            this.neuronGraphQueryClient = neuronGraphQueryClient ?? Locator.Current.GetService<INeuronQueryClient>();
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
            this.IconSourcePath = @"pack://application:,,,/d23-wpf;component/images/notification.ico";            
        }

        private static EditorNeuronData ConvertNotificationViewModelToEditorNeuron(NotificationViewModel n)
        {
            return new EditorNeuronData(
                n.Id,
                n.Tag,
                (NeurotransmitterEffect?)null,
                (float?)null,
                (RelativeType?)null,
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
                this.Notifications = (await Common.Helper.UpdateCacheGetNotifications(this.NotificationLog, this.neuronGraphQueryClient, this.AvatarUrl, NotificationsPaneViewModel.neuronCache)).Select(nd => new NotificationViewModel(
                    nd.Timestamp,
                    nd.AuthorId,
                    nd.Author,
                    nd.TypeName,
                    nd.Type,
                    nd.Version,
                    nd.ExpectedVersion,
                    nd.Id,
                    nd.Tag,
                    nd.Data,
                    nd.Details
                    ));

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
                    this.LayerId = result.First().Id;
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

        private async Task OnMoreClicked()
        {
            this.Loading = true;

            await ViewModels.Neurons.Helper.SetStatusOnComplete(async () =>
            {
                this.NotificationLog = await this.notificationApplicationService.GetNotificationLog(this.AvatarUrl, this.NotificationLog.PreviousNotificationLogId);
                this.Notifications = (await Common.Helper.UpdateCacheGetNotifications(this.NotificationLog, this.neuronGraphQueryClient, this.AvatarUrl, NotificationsPaneViewModel.neuronCache)).Select(nd => new NotificationViewModel(
                    nd.Timestamp,
                    nd.AuthorId,
                    nd.Author,
                    nd.TypeName,
                    nd.Type,
                    nd.Version,
                    nd.ExpectedVersion,
                    nd.Id,
                    nd.Tag,
                    nd.Data,
                    nd.Details
                    )).Concat(this.Notifications);
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
