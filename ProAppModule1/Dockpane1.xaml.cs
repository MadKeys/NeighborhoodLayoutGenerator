using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
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
            InitializeComponent();
            _viewModel = FrameworkApplication.DockPaneManager.Find(_dockPaneID) as Dockpane1ViewModel;
            this.DataContext = _viewModel;
        }

        private async void mapProjectItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _viewModel.UpdateLayerNamesAsync(mapProjectItemComboBox.SelectedItem as string).ConfigureAwait(false);
        }

        private async void layerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _viewModel.UpdateNeighborhoodNamesAsync(mapProjectItemComboBox.SelectedItem as string,
                layerComboBox.SelectedItem as string).ConfigureAwait(false);
        }

        private async void neighborhoodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _viewModel.ZoomToNeighborhood(mapProjectItemComboBox.SelectedItem as string, 
                layerComboBox.SelectedItem as string,
                neighborhoodComboBox.SelectedItem as string).ConfigureAwait(false);
        }
    }
}
