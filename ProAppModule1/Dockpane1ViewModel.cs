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
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Layouts;

namespace ProAppModule1
{
    internal class Dockpane1ViewModel : DockPane
    {
        private const string _dockPaneID = "ProAppModule1_Dockpane1";
        private const string _layoutName = "Neighborhood_Stabilization";
        private const string _layerName = "OH_Blocks";

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

        /* TODO: Delete depricated code when deemed safe
        private List<string> _mapProjectItemNames;
        public List<string> MapProjectItemNames
        {
            get { return _mapProjectItemNames; }
            private set { SetProperty(ref _mapProjectItemNames, value, () => MapProjectItemNames); }
        }

        private List<string> _layoutProjectItemNames;
        public List<string> LayoutProjectItemNames
        {
            get { return _layoutProjectItemNames; }
            private set { SetProperty(ref _layoutProjectItemNames, value, () => LayoutProjectItemNames); }
        }

        private List<string> _layerNames;
        public List<string> LayerNames
        {
            get { return _layerNames; }
            private set { SetProperty(ref _layerNames, value, () => LayerNames); }
        } */

        private List<string> _cityNames;
        public List<string> CityNames
        {
            get { return _cityNames; }
            private set { SetProperty(ref _cityNames, value, () => CityNames); }
        }

        private List<string> _neighborhoodNames;
        public List<string> NeighborhoodNames
        {
            get { return _neighborhoodNames; }
            private set { SetProperty(ref _neighborhoodNames, value, () => NeighborhoodNames); }
        }

        #endregion

        #region Event Handlers

        // TODO: Delete depricated code when deemed safe

        private void OnProjectOpened(ProjectEventArgs e)
        {
            // UpdateMapProjectItemNames(e.Project);
            // UpdateLayoutProjectItemNames(e.Project);
            UpdateCityNames(e.Project);
        }

        private void OnProjectItemsChanged(ProjectItemsChangedEventArgs e)
        {
            // UpdateMapProjectItemNames(Project.Current);
            // UpdateLayoutProjectItemNames(Project.Current);
            UpdateCityNames(Project.Current);
        }

        #endregion

        #region Update Properties

        /* TODO: Remove extraneous code when deemed safe
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

        private void UpdateLayoutProjectItemNames(Project project)
        {
            IEnumerable<LayoutProjectItem> layoutProjecctItems = project.GetItems<LayoutProjectItem>();
            var layoutProjectItemNames = new List<string>();
            foreach(LayoutProjectItem lpi in layoutProjecctItems)
            {
                layoutProjectItemNames.Add(lpi.Name);
            }
            LayoutProjectItemNames = layoutProjectItemNames;
        }

        public async Task UpdateLayerNamesAsync(string mpiName)
        {
            Map map = await GetMapAsync(mpiName).ConfigureAwait(false);
            var layerNames = new List<string>();
            foreach(Layer layer in map.Layers)
            {
                layerNames.Add(layer.Name);
            }
            LayerNames = layerNames;
        }
        */

        public void UpdateCityNames(Project project)
        {
            IEnumerable<MapProjectItem> mapProjectItems = project.GetItems<MapProjectItem>();
            var cityNames = new List<string>();
            foreach(MapProjectItem mpi in mapProjectItems)
            {
                string[] values = mpi.Name.Split(' ');
                if (!cityNames.Contains(values[0]))
                {
                    cityNames.Add(values[0]);
                }
            }
            CityNames = cityNames;
        }

        public async Task UpdateNeighborhoodNamesAsync(string cityName)
        {
            string mpiName = cityName + " Neighborhoods";
            RowCursor rowCursor = await GetRowCursorAsync(mpiName).ConfigureAwait(false);
            int neighoodIndex = await QueuedTask.Run(() => rowCursor.FindField("Neighood")).ConfigureAwait(false);
            string[] neighborhoodNames = await GetRowValuesAsync(rowCursor, neighoodIndex).ConfigureAwait(false);
            var neighborhoodNamesList = neighborhoodNames.Distinct().ToList();
            neighborhoodNamesList.Remove("");
            NeighborhoodNames = neighborhoodNamesList;
        }

        public async Task UpdateLayoutCityElementsAsync(string cityName)
        {
            Task<Layout> getLayoutTask = GetLayoutAsync(_layoutName);
            string insetMpiName = cityName + " Inset", neighborhoodMpiName = cityName + " Neighborhoods";
            Task<Map>[] getMapTasks = { GetMapAsync(insetMpiName), GetMapAsync(neighborhoodMpiName) };
            string insetFrameName = "Inset Map Frame", neighborhoodFrameName = "Neighborhood Map Frame";
            Layout layout = await getLayoutTask.ConfigureAwait(false);
            MapFrame[] mapFrames = {layout.FindElement(insetFrameName) as MapFrame,
                layout.FindElement(neighborhoodFrameName) as MapFrame };
            Map[] maps = await Task.WhenAll(getMapTasks).ConfigureAwait(false);
            Task[] setMapTasks = new Task[2];
            for(int i = 0; i < 2; i++)
            {
                // TODO: Lock this so the silly debugger won't break it 
                setMapTasks[i] = QueuedTask.Run(() => mapFrames[i].SetMap(maps[i]));
            }
            await Task.WhenAll(setMapTasks).ConfigureAwait(false);
        }

        public async Task ChangeCitySelection(string cityName)
        {
            await UpdateNeighborhoodNamesAsync(cityName).ConfigureAwait(false);
            await UpdateLayoutCityElementsAsync(cityName).ConfigureAwait(false);
            await ZoomToCity(cityName).ConfigureAwait(false);
        }

        #endregion

        #region Get Model Data

        private async Task<FeatureClass> GetFeatureClassAsync(Map map)
        {
            Layer layer = map.Layers.FirstOrDefault(i => i.Name.Equals("OH_Blocks"));
            var fLayer = layer as FeatureLayer;
            return await QueuedTask.Run(() => fLayer.GetFeatureClass()).ConfigureAwait(false);
        }

        private async Task<Map> GetMapAsync(string mpiName)
        {
            MapProjectItem mpi = Project.Current.GetItems<MapProjectItem>().FirstOrDefault((i) => i.Name.Equals(mpiName));
            return await QueuedTask.Run(() => mpi.GetMap()).ConfigureAwait(false);
        }

        private async Task<Layout> GetLayoutAsync(string lpiName)
        {
            LayoutProjectItem lpi = Project.Current.GetItems<LayoutProjectItem>().FirstOrDefault((i) => i.Name.Equals(lpiName));
            return await QueuedTask.Run(() => lpi.GetLayout()).ConfigureAwait(false);
        }

        private async Task<RowCursor> GetRowCursorAsync(string mpiName, QueryFilter queryFilter = null)
        {
            Map map = await GetMapAsync(mpiName).ConfigureAwait(false);
            FeatureClass featureClass = await GetFeatureClassAsync(map).ConfigureAwait(false);
            return await QueuedTask.Run(() => featureClass.Search()).ConfigureAwait(false);
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

        private async Task<Envelope> GetEnvelopeAsync(RowCursor rowCursor)
        {
            double xMin = 0.0, xMax = 0.0, yMin = 0.0, yMax = 0.0;
            do
            {
                Feature feature = rowCursor.Current as Feature;
                Task<Geometry> getShapeTask = QueuedTask.Run(()=> feature.GetShape());
                Envelope extent = (await getShapeTask.ConfigureAwait(false)).Extent;

                if (xMin == 0.0 || extent.XMin < xMin)
                    xMin = extent.XMin;
                if (xMax == 0.0 || extent.XMax > xMax)
                    xMax = extent.XMax;
                if (yMin == 0.0 || extent.YMin < yMin)
                    yMin = extent.YMin;
                if (yMax == 0.0 || extent.YMax > yMax)
                    yMax = extent.YMax;
            } while (await QueuedTask.Run(() => rowCursor.MoveNext()));

            EnvelopeBuilder eb = new EnvelopeBuilder();
            eb.XMin = xMin;
            eb.XMax = xMax;
            eb.YMin = yMin;
            eb.YMax = yMax;
            return await QueuedTask.Run(() => eb.ToGeometry());
        }

        #endregion

        #region Zoom to Features

        public async Task ZoomToCity(string cityName)
        {
            string mpiName = cityName + " Inset";
            QueryFilter queryFilter = new QueryFilter()
            {
                WhereClause = "NOT(Neighood IS NULL)"
            };
            await ZoomToFeatures(mpiName, queryFilter);
        }

        public async Task ZoomToNeighborhood(string cityName, string neighborhoodName)
        {
            string mpiName = cityName + " Neighborhoods";
            QueryFilter queryFilter = new QueryFilter()
            {
                WhereClause = "Neighood ='" + neighborhoodName + "'"
            };
            await ZoomToFeatures(mpiName, queryFilter).ConfigureAwait(false);
        }

        private async Task ZoomToFeatures(string mpiName, QueryFilter queryFilter)
        { 
            RowCursor rowCursor = await GetRowCursorAsync(mpiName, queryFilter).ConfigureAwait(false);
            Envelope extent = await GetEnvelopeAsync(rowCursor).ConfigureAwait(false);
            await ZoomToExtent(mpiName, extent).ConfigureAwait(false);
        }

        private async Task ZoomToExtent(string mpiName, Envelope envelope)
        {
            Map map = await GetMapAsync(mpiName).ConfigureAwait(false);
            await QueuedTask.Run(() => map.SetCustomFullExtent(envelope)).ConfigureAwait(false);

            Layout layout = await GetLayoutAsync("Neighborhood_Stabilization").ConfigureAwait(false);
            var mapFrame = layout.FindElement("Neighborhood_Map_Frame") as MapFrame;
            var mapView = mapFrame.MapView;
            await QueuedTask.Run(() => mapView.ZoomToFullExtentAsync()).ConfigureAwait(false);
        }

        #endregion
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
