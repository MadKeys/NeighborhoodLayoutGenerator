using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace ProAppModule1
{
    internal class Button1 : Button
    {
        protected override void OnClick()
        {
            /* foreach (string neighborhood in neighborhoods)
            {
                QueryFilter qf = new QueryFilter()
                {
                    WhereClause = "Neighood = '" + neighborhood + "'"
                };

                RowCursor rc = fClass.Search(qf);
                ChangeMapExtent(rc);
            }  */
        }
    }
}
