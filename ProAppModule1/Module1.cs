using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Geometry;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ProAppModule1
{
    public class NeighborhoodList : ObservableCollection<string>
    {
        public NeighborhoodList()
        {

        }

        public NeighborhoodList(ObservableCollection<string> observableCollection)
        {
            foreach(string s in observableCollection.ToList())
            {
                this.Add(s);
                PropertyChangedEventArgs eventArgs = new PropertyChangedEventArgs("Neighborhoods");
                OnPropertyChanged(eventArgs);
            }
        }
    }

    internal class Module1 : Module
    {
        #region Class Variables

        public event PropertyChangedEventHandler NewPropertyChanged;

        static double xMin = 0.0, xMax = 0.0, yMin = 0.0, yMax = 0.0;

        private const string LAYOUT_NAME = "Neighborhood_Stabilization";
        // private static Layout layout;

        private const string MAIN_MAP_NAME = "Youngstown Neighborhoods";
        // private static Map mainMap;

        private const string LAYER_NAME = "OH_Blocks";
        // private static FeatureClass featureClass;

        // Make Neighborhoods an observable collection?

        private NeighborhoodList neighborhoods;
        public NeighborhoodList Neighborhoods
        {
            get { return neighborhoods; }
            private set
            {
                neighborhoods = value;
                OnNewPropertyChanged("Neighborhoods");
            }
        }

        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("ProAppModule1_Module"));
            }
        }

        #endregion Class Variables

        private static void OnNewPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = Current.NewPropertyChanged;
            if (handler != null)
            {
                handler(Current, new PropertyChangedEventArgs(name));
            }
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

        #region Setup

        /* Prepares the module for use */
        public static async void SetupModule()
        {
            OpenProject();
            // Action[] setupProjectItemActions = { SetupMainMapAsync, SetupLayoutAsync };
            Task[] tasks = new Task[2];
            tasks[0] = GetNeighborhoodMapAsync();
            tasks[1] = RetrieveLayoutAsync();
            /* List<Task> setupProjectItemTasks = new List<Task>();
            foreach(Action action in setupProjectItemActions)
            {
                setupProjectItemTasks.Add(Task.Run(action));
            } 
            Action<Task[]> setupData = SetupData;
            Task.Factory.ContinueWhenAll(setupProjectItemTasks.ToArray(), setupData).Wait();*/

            await Task.WhenAll(tasks).ConfigureAwait(false);


        }

        /* Ensures that the neighborhood stabilization project is open */
        private static void OpenProject()
        {
            string projectUri = @"C:\Users\mkeister\Documents\ArcGIS\Projects\Neighborhood_Stabilization.aprx";
            string projectVersion = "1.4.0";
            if (Project.Current.URI != projectUri && Project.CanOpen(projectUri, out projectVersion))
            {
                Project.OpenAsync(projectUri);
            }
        }

        /* returns a task whos result is the map of neighborhoods */
        private static async Task<Map> GetNeighborhoodMapAsync()
        {
            var mpi = Project.Current.GetItems<MapProjectItem>().FirstOrDefault(item => item.Name.Equals(MAIN_MAP_NAME));
            return await QueuedTask.Run(() => mpi.GetMap()).ConfigureAwait(false);
        }

        

        /* returns a task whos result is the layout */
        private static async Task<Layout> RetrieveLayoutAsync()
        {
            LayoutProjectItem layoutItem = Project.Current.GetItems<LayoutProjectItem>().FirstOrDefault(item => item.Name.Equals(LAYOUT_NAME));
            if (layoutItem != null)
            {
                return await QueuedTask.Run(() => layoutItem.GetLayout()).ConfigureAwait(false);
            }
            else
            {
                System.Windows.MessageBox.Show("The specified Layout Project Item " + LAYOUT_NAME + " is null.");
                return null;
            }
        }

        /* returns a task whos result is the feature class associated with the layer with specified name */
        private static async Task<FeatureClass> GetFeatureClassAsync(string layerName)
        {
            Map neighborhoodMap = await GetNeighborhoodMapAsync().ConfigureAwait(false);
            Layer layer = neighborhoodMap.Layers.FirstOrDefault(i => i.Name.Equals(LAYER_NAME));
            var fLayer = layer as FeatureLayer;
            return await QueuedTask.Run(() => fLayer.GetFeatureClass()).ConfigureAwait(false);
        }

        /* Initializes the class field of type Feature Class and the list of neighborhoods from which the 
         * user can select one to be mapped 
        private static async Task<NeighborhoodList> GetNeighborhoodListAsync(Task[] setupTasks)
        {
            FeatureClass featureClass = await GetFeatureClassAsync(LAYER_NAME);
            Task<ObservableCollection<string>> listAllNeighborhoodsTask = ListAllNeighborhoodsAsync();
            listAllNeighborhoodsTask.Wait();
            Current.Neighborhoods = new NeighborhoodList(listAllNeighborhoodsTask.Result);
        } */

        /* Returns a Task whose result is the List<string> of neighborhoods in the FeatureClass */
        public static async Task<NeighborhoodList> GetNeighborhoodListAsync()
        {
            var neighborhoods = new NeighborhoodList();
            FeatureClass featureClass = await GetFeatureClassAsync(LAYER_NAME).ConfigureAwait(false);
            RowCursor rowCursor = await QueuedTask.Run(() => featureClass.Search()).ConfigureAwait(false);
            int neighoodIndex = await QueuedTask.Run(() => rowCursor.FindField("Neighood")).ConfigureAwait(false);
            List<Task<string>> rowGetOriginalValueTasks = new List<Task<string>>();
            do
            {
                Row row = rowCursor.Current;
                if(row != null)
                {
                    rowGetOriginalValueTasks.Add(QueuedTask.Run(() => row.GetOriginalValue(neighoodIndex).ToString()));
                }

            } while (await QueuedTask.Run(() => rowCursor.MoveNext()));
            string[] originalValues = await Task.WhenAll(rowGetOriginalValueTasks).ConfigureAwait(false);
            foreach(string value in originalValues)
            {
                if (!neighborhoods.Contains(value))
                {
                    neighborhoods.Add(value);
                }
            }
            return neighborhoods;
        }

        #endregion Setup

        /* Updates the map and layout to show the new neighborhood */
        public static async Task<bool> ChangeNeighborhoodSelection(string neighborhood)
        {
            QueryFilter qf = new QueryFilter()
            {
                WhereClause = "Neighood = '" + neighborhood + "'"
            };

            FeatureClass featureClass = await GetFeatureClassAsync(LAYER_NAME);
            RowCursor rc = featureClass.Search(qf);
            bool navigationCompleted = await ChangeMapExtent(rc);
            return navigationCompleted;
        }

        /* Changes the extent of the neighborhood map to CustomFullExtent specified by the class variables xMin, xMax, yMin, yMax */
        private static async Task<bool> ZoomToExtent()
        {
            Map neighborhoodMap = await GetNeighborhoodMapAsync();

            Layout layout = await RetrieveLayoutAsync();
            var hoodMapFrame = layout.FindElement("Neighborhood_Map_Frame") as MapFrame;
            var hoodMapView = hoodMapFrame.MapView;

            EnvelopeBuilder eb = new EnvelopeBuilder();
            eb.XMin = xMin;
            eb.XMax = xMax;
            eb.YMin = yMin;
            eb.YMax = yMax;
            Envelope newExtent = await QueuedTask.Run(() => eb.ToGeometry());
            xMin = 0.0; xMax = 0.0; yMin = 0.0; yMax = 0.0;
            await QueuedTask.Run(() => neighborhoodMap.SetCustomFullExtent(newExtent));
            bool navigationCompleted = await QueuedTask.Run(() => hoodMapView.ZoomToFullExtentAsync());
            return navigationCompleted;
        }

        /* Shifts the extent of the map to encompass the features contained by the RowCursor rc */
        private static async Task<bool> ChangeMapExtent(RowCursor rc)
        {
            var tasks = new List<Task>();
            do
            {
                Feature feature = rc.Current as Feature;
                Geometry geometry;

                Task t1 = QueuedTask.Run(() =>
                {
                    geometry = feature.GetShape();
                    Envelope extent = geometry.Extent;

                    if (xMin == 0.0 || extent.XMin < xMin)
                        xMin = extent.XMin;
                    if (xMax == 0.0 || extent.XMax > xMax)
                        xMax = extent.XMax;
                    if (yMin == 0.0 || extent.YMin < yMin)
                        yMin = extent.YMin;
                    if (yMax == 0.0 || extent.YMax > yMax)
                        yMax = extent.YMax;
                });
                tasks.Add(t1);
            } while (rc.MoveNext());

            Task[] taskArray = tasks.ToArray();
            await Task.WhenAll(taskArray);
            bool navigationCompleted = await ZoomToExtent();
            return navigationCompleted;
        }

    }
}
