using Autodesk.DesignScript.Geometry;

using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;

namespace AnyCAD.CoreNodes.GeometryInterop
{
    [SupressImportIntoVM]
    [IsVisibleInDynamoLibrary(false)]
    public static class ProtoCurveToTopoShape
    {
        public static TopoShape To(this Curve crv)
        {
            // TODO: host unit convert
            dynamic dyCrv = crv;
            return Convert(dyCrv);
        }

        private static TopoShape Convert(this PolyCurve pcrv)
        {
            if (!pcrv.IsClosed)
            {
                throw new Exception("The input PolyCurve must be closed");
            }

            Curve[] crvs = crvs = pcrv.Curves();

            TopoShapeList edges = [];
            crvs.ForEach(crv => edges.Add(crv.To()));

            // dispose
            crvs.ForEach(x => x.Dispose());

            return CurveBuilder.MakeWire(edges);
        }

        private static TopoShape Convert(Curve crvCurve)
        {
            Curve[] curves = crvCurve.ApproximateWithArcAndLineSegments();
            if (curves.Length == 1)
            {
                //line or arc?
                var point0 = crvCurve.PointAtParameter(0.0);
                var point1 = crvCurve.PointAtParameter(1.0);
                var pointMid = crvCurve.PointAtParameter(0.5);
                if (point0.DistanceTo(point1) > 1e-7)
                {
                    using (var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(point0, point1))
                    {
                        if (pointMid.DistanceTo(line) < 1e-7)
                        {
                            var result = Convert(line);
                            curves[0].Dispose();
                            return result;
                        }
                    }
                }
                //then arc
                if (point0.DistanceTo(point1) < 1e-7)
                    point1 = crvCurve.PointAtParameter(0.9);
                var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(point0, pointMid, point1);
                var resultArc = Convert(arc);
                arc.Dispose();
                curves[0].Dispose();
                return resultArc;
            }
            curves.ForEach(x => x.Dispose());

            using (var nc = crvCurve.ToNurbsCurve())
            {
                return Convert(nc);
            }
        }

        private static TopoShape Convert(Line line)
        {
            return CurveBuilder.MakeLine(line.StartPoint.To(), line.EndPoint.To());
        }

        private static TopoShape Convert(Arc arc)
        {
            return CurveBuilder.MakeArc(arc.StartPoint.To(), arc.EndPoint.To(), arc.CenterPoint.To(), arc.Normal.To());
        }

        private static TopoShape Convert(Circle circ)
        {
            var center = circ.CenterPoint.To();
            var start = circ.StartPoint.To();

            // get the axis
            var theVx = new GDir(start.XYZ().Subtracted(center.XYZ()));
            var theN = circ.Normal.To();
            var ax2 = new GAx2(center, theN, theVx);

            return CurveBuilder.MakeCircle(ax2, circ.Radius);
        }
    }
}
