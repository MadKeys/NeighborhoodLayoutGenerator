using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Windows;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core;
using System.ComponentModel;
using ArcGIS.Desktop.Core.Events;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;

namespace ProAppModule1
{
    internal class Dockpane1ViewModel : DockPane
    {
        private const string _dockPaneID = "ProAppModule1_Dockpane1";

        protected Dockpane1ViewModel()
        {
            ProjectOpenedEvent.Subscribe(new Action<ProjectEventArgs>((e) => { OnProjectOpened(e); } ));
            ProjectItemsChangedEvent.Subscribe(new Action<ProjectItemsChangedEventArgs>((e) => { OnProjectItemsChanged(e); }));
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        #region Properties and Backing Fields

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "My DockPane";
        public string Heading
        {
            get { return _heading; }
            set { SetProperty(ref _heading, value, () => Heading); }
        }

        private List<string> _mapProjectItemNames;
        public List<string> MapProjectItemNames
        {
            get { return _mapProjectItemNames; }
            private set { SetProperty(ref _mapProjectItemNames, value, () => MapProjectItemNames); }
        }

        private List<string> _layerNames;
        public List<string> LayerNames
        {
            get { return _layerNames; }
            private set { SetProperty(ref _layerNames, value, () => LayerNames); }
        }

        private List<string> _neighborhoodNames;
        public List<string> NeighborhoodNames
        {
            get { return _neighborhoodNames; }
            private set { SetProperty(ref _neighborhoodNames, value, () => NeighborhoodNames); }
        }

        #endregion

        #region Event Handlers

        private void OnProjectOpened(ProjectEventArgs e)
        {
            UpdateMapProjectItemNames(e.Project);
        }

        private void OnProjectItemsChanged(ProjectItemsChangedEventArgs e)
        {
            UpdateMapProjectItemNames(Project.Current);
        }

        #endregion

        #region Update Properties

        private void UpdateMapProjectItemNames(Project project)
        {
            IEnumerable<MapProjectItem> mapProjectItems = project.GetItems<MapProjectItem>();
            var mapProjectItemNames = new List<string>();
            foreach (MapProjectItem mpi in mapProjectItems)
            {
                mapProjectItemNames.Add(mpi.Name);
            }
            MapProjectItemNames = mapProjectItemNames;
        }

        public async Task UpdateLayerNamesAsync(string mpiName)
        {
            Map map = await GetMapFromMPInameAsync(mpiName).ConfigureAwait(false);
            var layerNames = new List<string>();
            foreach(Layer layer in map.Layers)
            {
                layerNames.Add(layer.Name);
            }
            LayerNames = layerNames;
        }

        public async Task UpdateNeighborhoodNamesAsync(string mpiName, string layerName)
        {
            Map map = await GetMapFromMPInameAsync(mpiName).ConfigureAwait(false);
            FeatureClass featureClass = await GetFeatureClassFromMapAndLayerNameAsync(map, layerName).ConfigureAwait(false);
            RowCursor rowCursor = await QueuedTask.Run(() => featureClass.Search()).ConfigureAwait(false);
            int neighoodIndex = await QueuedTask.Run(() => rowCursor.FindField("Neighood")).ConfigureAwait(false);
            string[] neighborhoodNames = await GetRowValuesAsync(rowCursor, neighoodIndex).ConfigureAwait(false);
            var neighborhoodNamesList = neighborhoodNames.Distinct().ToList();
            neighborhoodNamesList.Remove("");
            NeighborhoodNames = neighborhoodNamesList;
        }

        #endregion

        #region Helpers

        private async Task<FeatureClass> GetFeatureClassFromMapAndLayerNameAsync(Map map, string layerName)
        {
            Layer layer = map.Layers.FirstOrDefault(i => i.Name.Equals(layerName));
            var fLayer = layer as FeatureLayer;
            return await QueuedTask.Run(() => fLayer.GetFeatureClass()).ConfigureAwait(false);
        }

        private async Task<Map> GetMapFromMPInameAsync(string mpiName)
        {
            MapProjectItem mpi = Project.Current.GetItems<MapProjectItem>().FirstOrDefault((i) => i.Name.Equals(mpiName));
            return await QueuedTask.Run(() => mpi.GetMap()).ConfigureAwait(false);
        }

        private async Task<string[]> GetRowValuesAsync(RowCursor rowCursor, int textVariableIndex)
        {
            List<Task<string>> getValueTasks = new List<Task<string>>();
            do
            {
                Row row = rowCursor.Current;
                if (row != null)
                {
                    getValueTasks.Add(QueuedTask.Run(() => row.GetOriginalValue(textVariableIndex).ToString()));
                }

            } while (await QueuedTask.Run(() => rowCursor.MoveNext()).ConfigureAwait(false));
            return await Task.WhenAll(getValueTasks).ConfigureAwait(false);
        }

        #endregion

        // TODO: Rework Module1's Zoom to Neighborhood functionality
        public async Task ZoomToNeighborhood(string mpiName, string layerName, string neighborhoodName)
        {

        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Dockpane1_ShowButton : Button
    {
        protected override void OnClick()
        {
            Dockpane1ViewModel.Show();
        }
    }
}
