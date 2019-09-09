using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Peripheral
{
    /// <summary>
    /// Interaction logic for PropertyGridView.xaml
    /// </summary>
    public partial class PropertyGridView : UserControl, IViewFor<PropertyGridViewModel>
    {
        public PropertyGridView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => this.ViewModel = (PropertyGridViewModel)x);

                d(this.Bind(this.ViewModel, vm => vm.SelectedObject, v => v.PropertyGrid.SelectedObject));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (PropertyGridViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
    "ViewModel", typeof(PropertyGridViewModel), typeof(PropertyGridView), new PropertyMetadata(default(PropertyGridViewModel)));

        public PropertyGridViewModel ViewModel
        {
            get { return (PropertyGridViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
