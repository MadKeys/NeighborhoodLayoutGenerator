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
using System.Collections.Concurrent;

namespace ProAppModule1
{
    internal class Dockpane1ViewModel : DockPane
    {
        private const string _layoutName = "Neighborhood_Stabilization";
        private const string _layerName = "OH_Blocks";

        private const string _dockPaneID = "ProAppModule1_Dockpane1";

        private Object thisLock;

        protected Dockpane1ViewModel()
        {
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            ProjectOpenedEvent.Subscribe(new Action<ProjectEventArgs>((e) => { OnProjectOpened(e); } ));
            ProjectItemsChangedEvent.Subscribe(new Action<ProjectItemsChangedEventArgs>((e) => { OnProjectItemsChanged(e); }));

            thisLock = new Object();
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane _dockPane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (_dockPane == null)
                return;

            _dockPane.Activate();
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

        private string _cityZoomCompleted;
        public string CityZoomCompleted
        {
            get { return _cityZoomCompleted; }
            set { SetProperty(ref _cityZoomCompleted, value, () => CityZoomCompleted); }
        }

        private List<string> _cityNames;
        public List<string> CityNames
        {
            get { return _cityNames; }
            private set { SetProperty(ref _cityNames, value, () => CityNames); }
        }

        private string _neighborhoodZoomCompleted;
        public string NeighborhoodZoomCompleted
        {
            get { return _neighborhoodZoomCompleted; }
            set { SetProperty(ref _neighborhoodZoomCompleted, value, () => NeighborhoodZoomCompleted); }
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
            UpdateCityNames(e.Project);
        }

        private void OnProjectItemsChanged(ProjectItemsChangedEventArgs e)
        {
            UpdateCityNames(Project.Current);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("---UNOBSERVED TASK EXCEPTION---");
            Console.WriteLine(e.Exception.Message);
            foreach (Exception ex in e.Exception.InnerExceptions)
            {
                Console.WriteLine(ex.ToString());
            }
            e.SetObserved();
        }

        #endregion

        #region Task Wrappers

        private static Task<MapFrame> WrapTask(Task<MapFrame> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<MapFrame>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new FieldAccessException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<bool> WrapTask(Task<bool> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<bool>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new BadImageFormatException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<RowCursor> WrapTask(Task<RowCursor> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<RowCursor>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new GeodatabaseCursorException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<Envelope> WrapTask(Task<Envelope> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<Envelope>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new DuplicateWaitObjectException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<Dictionary<BasicFeatureLayer, List<long>>> WrapTask(Task<Dictionary<BasicFeatureLayer, List<long>>> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<Dictionary<BasicFeatureLayer, List<long>>>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new MulticastNotSupportedException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<Geometry> WrapTask(Task<Geometry> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<Geometry>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new EntryPointNotFoundException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<double> WrapTask(Task<double> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<double>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new KeyNotFoundException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        private static Task<Task<bool>> WrapTask(Task<Task<bool>> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<Task<bool>>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new EncoderFallbackException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        Task<EnvelopeBuilder> WrapTask(Task<EnvelopeBuilder> task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<EnvelopeBuilder>();

            task.ContinueWith(async x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new TypeLoadException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult(await task);
                }
            });

            return taskCompletionSource.Task;
        }

        Task WrapTask(Task task)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var taskCompletionSource = new TaskCompletionSource<Object>();

            task.ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    taskCompletionSource.TrySetException(new TypeLoadException("Stack Trace: " + stackTrace, x.Exception.GetBaseException()));
                }
                else if (x.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
            });

            return taskCompletionSource.Task;
        }

        #endregion

        #region Update Properties

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
            int i = 0;
            setMapTasks[i] = QueuedTask.Run(() => mapFrames[i].SetMap(maps[i]));
            i += 1;
            setMapTasks[i] = QueuedTask.Run(() => mapFrames[i].SetMap(maps[i]));
            await Task.WhenAll(setMapTasks).ConfigureAwait(false);
        }

        public async Task ChangeCitySelection(string cityName)
        {
            CityZoomCompleted = "Focusing...";
            await UpdateNeighborhoodNamesAsync(cityName).ConfigureAwait(false);
            await UpdateLayoutCityElementsAsync(cityName).ConfigureAwait(false);
            await ZoomToCity(cityName).ConfigureAwait(false);
        }

        public async Task ChangeNeighborhoodSelection(string cityName, string neighborhoodName)
        {
            NeighborhoodZoomCompleted = "Focusing...";
            await ZoomToNeighborhood(cityName, neighborhoodName).ConfigureAwait(false);
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
            return await QueuedTask.Run(() => featureClass.Search(queryFilter)).ConfigureAwait(false);
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
            ConcurrentDictionary<string, double> extentBounds = new ConcurrentDictionary<string, double>();
            extentBounds.TryAdd("xMin", 0.0);
            extentBounds.TryAdd("xMax", 0.0);
            extentBounds.TryAdd("yMin", 0.0);
            extentBounds.TryAdd("yMax", 0.0);

            List<Task> calculateExtentTasks = new List<Task>();
            do
            {
                calculateExtentTasks.Add(Task.Run(async () =>
                {
                    double xMin, xMax, yMin, yMax;
                    bool xMinFound = extentBounds.TryGetValue("xMin", out xMin);
                    bool xMaxFound = extentBounds.TryGetValue("xMax", out xMax);
                    bool yMinFound = extentBounds.TryGetValue("yMin", out yMin);
                    bool yMaxFound = extentBounds.TryGetValue("yMax", out yMax);

                    Feature feature = rowCursor.Current as Feature;
                    if (feature != null)
                    {
                        Task<Geometry> getShapeTask = QueuedTask.Run(() => feature.GetShape());
                        Envelope extent = (await getShapeTask.ConfigureAwait(false)).Extent;

                        if (xMin == 0.0 || extent.XMin < xMin)
                        {
                            bool xMinUpdated = extentBounds.TryUpdate("xMin", extent.XMin, xMin);
                        }
                        if (xMax == 0.0 || extent.XMax > xMax)
                        {
                            bool xMaxUpdated = extentBounds.TryUpdate("xMax", extent.XMax, xMax);
                        }
                        if (yMin == 0.0 || extent.YMin < yMin)
                        {
                            bool yMinUpdated = extentBounds.TryUpdate("yMin", extent.YMin, yMin);
                        }
                        if (yMax == 0.0 || extent.YMax > yMax)
                        {
                            bool yMaxUpdate = extentBounds.TryUpdate("yMax", extent.YMax, yMax);
                        }
                    }
                }));
            } while (await QueuedTask.Run(() => rowCursor.MoveNext()));

            EnvelopeBuilder eb = await QueuedTask.Run(() => new EnvelopeBuilder());
            var setEnvelopePropertyTasks = new List<Task>();
            double xMinFinal, xMaxFinal, yMinFinal, yMaxFinal;
            await Task.WhenAll(calculateExtentTasks);

            bool xMinFinalFound = extentBounds.TryGetValue("xMin", out xMinFinal);
            bool xMaxFinalFound = extentBounds.TryGetValue("xMax", out xMaxFinal);
            bool yMinFinalFound = extentBounds.TryGetValue("yMin", out yMinFinal);
            bool yMaxFinalFound = extentBounds.TryGetValue("yMax", out yMaxFinal);

            setEnvelopePropertyTasks.Add(QueuedTask.Run(() => eb.XMin = xMinFinal));
            setEnvelopePropertyTasks.Add(QueuedTask.Run(() => eb.XMax = xMaxFinal));
            setEnvelopePropertyTasks.Add(QueuedTask.Run(() => eb.YMin = yMinFinal));
            setEnvelopePropertyTasks.Add(QueuedTask.Run(() => eb.YMax = yMaxFinal));

            await Task.WhenAll(setEnvelopePropertyTasks.ToArray());
            return await QueuedTask.Run(() => eb.ToGeometry());
        }

        private async Task<MapFrame> GetMapFrameAsync(string layoutElementName)
        {
            Task<Layout> layoutTask = GetLayoutAsync("Neighborhood_Stabilization");
            var mapFrame = (await layoutTask.ConfigureAwait(false)).FindElement(layoutElementName) as MapFrame;
            return mapFrame;
        }

        #endregion

        #region Zoom to Features

        public async Task ZoomToCity(string cityName)
        {
            string layoutElementName = "Inset Map Frame";
            QueryFilter queryFilter = new QueryFilter()
            {
                WhereClause = "NOT(Neighood IS NULL)"
            };
            CityZoomCompleted = (await ZoomTo(queryFilter, layoutElementName)).ToString();
        }

        public async Task ZoomToNeighborhood(string cityName, string neighborhoodName)
        {
            string layoutElementName = "Neighborhood Map Frame";
            QueryFilter queryFilter = new QueryFilter()
            {
                WhereClause = "Neighood ='" + neighborhoodName + "'"
            };
            NeighborhoodZoomCompleted = (await ZoomTo(queryFilter, layoutElementName)).ToString();
        }

        private async Task<bool> ZoomTo(QueryFilter queryFilter, string layoutElementName)
        {
            Task<MapFrame> mapFrameTask = GetMapFrameAsync(layoutElementName);
            Task<bool> zoomToExtentTask = ZoomToExtent(queryFilter, (await mapFrameTask.ConfigureAwait(false)));
            bool navigationCompleted = await zoomToExtentTask.ConfigureAwait(false);
            return navigationCompleted;
        }

        private async Task<bool> ZoomToExtent(QueryFilter queryFilter, MapFrame mapFrame)
        {
            Task<RowCursor> rowCursorTask = GetRowCursorAsync(mapFrame.Map.Name, queryFilter);
            Task<Envelope> extentTask = GetEnvelopeAsync(await rowCursorTask.ConfigureAwait(false));
            var mapView = mapFrame.MapView;
            Envelope envelope = await extentTask;

            
            /*Task<Dictionary<BasicFeatureLayer, List<long>>> selectFeaturesTask = 
                QueuedTask.Run(() => mapView.SelectFeatures(envelope));*/
            Task<bool> zoomToTask = QueuedTask.Run(() => mapView.ZoomToAsync(envelope));
            bool navigationCompleted = await zoomToTask.ConfigureAwait(false);
            return navigationCompleted;
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
