using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.Generic;
using System.Linq;

namespace RevitAPIApp52
{
    public class RevitDataService
    {
        private readonly Document _doc;

        public RevitDataService(Document doc)
        {
            _doc = doc;
        }

        public int GetPipeCount()
        {
            return new FilteredElementCollector(_doc)
                .OfClass(typeof(Pipe))
                .WhereElementIsNotElementType()
                .GetElementCount();
        }

        public double GetTotalWallVolume()
        {
            var walls = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            double totalVolume = 0;

            foreach (Wall wall in walls)
            {
                Parameter volumeParam = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (volumeParam != null && volumeParam.HasValue)
                {
                    totalVolume += volumeParam.AsDouble();
                }
            }

            return UnitUtils.ConvertFromInternalUnits(totalVolume, UnitTypeId.CubicMeters);
        }

        public int GetDoorCount()
        {
            return new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .GetElementCount();
        }
    }
}
