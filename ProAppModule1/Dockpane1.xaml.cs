using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Threading;


namespace ProAppModule1
{
    /// <summary>
    /// Interaction logic for Dockpane1View.xaml
    /// </summary>
    /// 

    public partial class Dockpane1View : UserControl
    {
        private const string _dockPaneID = "ProAppModule1_Dockpane1";
        private Dockpane1ViewModel _viewModel;

        public Dockpane1View()
        {
            _viewModel = FrameworkApplication.DockPaneManager.Find(_dockPaneID) as Dockpane1ViewModel;
            this.DataContext = _viewModel;
            InitializeComponent();
        }
        
        private async void cityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DispatcherOperation<Task> dispatcherOperation = 
                Dispatcher.InvokeAsync(() => _viewModel.ChangeCitySelection(cityComboBox.SelectedItem as string));
            Task task = await dispatcherOperation;
            await task;
        }

        private async void neighborhoodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => _viewModel.ChangeNeighborhoodSelection(cityComboBox.SelectedItem as string, neighborhoodComboBox.SelectedItem as string));
        }

        private async void cityComboBox_Initialized(object sender, EventArgs e)
        {
            await Dispatcher.InvokeAsync(() => _viewModel.UpdateCityNames(Project.Current));
        }

        private async void neighborhoodComboBox_Initialized(object sender, EventArgs e)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                if (cityComboBox.SelectedItem != null)
                    await _viewModel.UpdateNeighborhoodNamesAsync(cityComboBox.SelectedItem as string);
            });
        }
    }
}
