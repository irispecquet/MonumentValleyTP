using UnityEngine;

namespace LuniLib.Maths
{
    using static System.Math;

    public static class Geometry
    {
        #region CIRCLE
        /// <summary>
        /// Computes the area of a circle.
        /// </summary>
        /// <param name="r">Radius.</param>
        /// <returns>Circle's area.</returns>
        public static double ComputeCircleArea(this double r)
        {
            return PI * r * r;
        }

        /// <summary>
        /// Computes the circumference of a circle.
        /// </summary>
        /// <param name="r">Radius.</param>
        /// <returns>Circle's circumference.</returns>
        public static double ComputeCircleCircumference(this double r)
        {
            return 2 * PI * r;
        }

        /// <summary>
        /// Computes the distance between points around a circle.
        /// </summary>
        /// <param name="r">Circle's radius.</param>
        /// <param name="n">Number of points.</param>
        /// <returns>Computed distance.</returns>
        public static double ComputePointsDistanceAroundCircle(double r, double n)
        {
            return 2 * r * Sin(PI / n);
        }

        /// <summary>
        /// Checks if a point is inside a circle of center (0,0).
        /// </summary>
        /// <param name="px">Point x.</param>
        /// <param name="py">Point y.</param>
        /// <param name="r">Circle's radius.</param>
        /// <param name="strictly">Point can't be right on the circle.</param>
        /// <returns>True if the point is inside.</returns>
        public static bool IsPointInsideCircle(float px, float py, float r, bool strictly = false)
        {
            return IsPointInsideCircle(px, py, r, 0, 0, strictly);
        }

        /// <summary>
        /// Checks if a point is inside a circle.
        /// </summary>
        /// <param name="px">Point x.</param>
        /// <param name="py">Point y.</param>
        /// <param name="r">Circle's radius.</param>
        /// <param name="cx">Circle's center x.</param>
        /// <param name="cy">Circle's center y.</param>
        /// <param name="strictly">Point can't be right on the circle.</param>
        /// <returns>True if the point is inside.</returns>
        public static bool IsPointInsideCircle(float px, float py, float r, float cx, float cy, bool strictly = false)
        {
            float sqrDist = (cx - px) * (cx - px) + (cy - py) * (cy - py);
            return strictly ? sqrDist < r * r : sqrDist <= r * r;
        }

        /// <summary>
        /// Checks if a point is outside of a circle.
        /// </summary>
        /// <param name="px">Point x.</param>
        /// <param name="py">Point y.</param>
        /// <param name="r">Circle's radius.</param>
        /// <param name="strictly">Point can't be right on the circle.</param>
        /// <returns>True if the point is outside.</returns>
        public static bool IsPointOutsideCircle(float px, float py, float r, bool strictly = false)
        {
            return IsPointOutsideCircle(px, py, r, 0, 0, strictly);
        }

        /// <summary>
        /// Checks if a point is outside of a circle.
        /// </summary>
        /// <param name="px">Point x.</param>
        /// <param name="py">Point y.</param>
        /// <param name="r">Circle's radius.</param>
        /// <param name="cx">Circle's center x.</param>
        /// <param name="cy">Circle's center y.</param>
        /// <param name="strictly">Point can't be right on the circle.</param>
        /// <returns>True if the point is outside.</returns>
        public static bool IsPointOutsideCircle(float px, float py, float r, float cx, float cy, bool strictly = false)
        {
            return !IsPointInsideCircle(px, py, r, cx, cy, strictly);
        }

        /// <summary>
        /// Computes points positions around a circle.
        /// </summary>
        /// <param name="pointsCount">Points count to compute, that must be 3 or higher.</param>
        /// <param name="radius">Circle radius.</param>
        /// <param name="angleOffset">Angle offset applied to circle in degrees.</param>
        /// <returns>Array of points around circle.</returns>
        public static Vector2[] ComputePointsAroundCircle(int pointsCount, float radius, float angleOffset = 0f)
        {
            if (pointsCount < 3)
            {
                Debug.LogWarning($"A minimum of 3 points are required to compute points positions around circle (current count is {pointsCount})! Returning a null points array.");
                return System.Array.Empty<Vector2>();
            }

            Vector2[] points = new Vector2[pointsCount];
            float angleOffsetRad = angleOffset * Mathf.Deg2Rad;
            
            for (int i = 0; i < pointsCount; ++i)
            {
                float theta = (Mathf.PI * 2 * i) / pointsCount + angleOffsetRad;
                float x = Mathf.Sin(theta) * radius;
                float y = Mathf.Cos(theta) * radius;
                points[i] = new Vector2(x, y);
            }

            return points;
        }
        #endregion // CIRCLE

        #region HEXAGON
        private static float HexagonCenterDistance(Vector2 position)
        {
            position = new Vector2(Mathf.Abs(position.x), Mathf.Abs(position.y));
            return Mathf.Max(ComputeDotProduct(position, new Vector2(1f, 1.732f).normalized), position.x);
        }
        
        /// <summary>
        /// Computes a random point inside a hexagon with a radius of 1.
        /// </summary>
        /// <returns>Computed random point.</returns>
        public static Vector2 RandomPointInHexagon()
        {
            bool isPointValid = false;
            Vector2 point = Vector2.zero;

            do
            {
                Vector2 position = new(-0.866f + UnityEngine.Random.value * 1.732f,
                                       -1f + UnityEngine.Random.value * 2f);

                if (HexagonCenterDistance(position) <= 0.866f)
                    isPointValid = true;
            }
            while (!isPointValid);

            return point;
        }
        
        /// <summary>
        /// Computes a random point inside a hexagon with a radius of 1.
        /// </summary>
        /// <param name="random">Random to use.</param>
        /// <returns>Computed random point.</returns>
        public static Vector2 RandomPointInHexagon(System.Random random)
        {
            bool isPointValid = false;
            Vector2 point = Vector2.zero;

            do
            {
                Vector2 position = new(-0.866f + (float)random.NextDouble() * 1.732f,
                                       -1f + (float)random.NextDouble() * 2f);

                if (HexagonCenterDistance(position) <= 0.866f)
                {
                    point = position;
                    isPointValid = true;
                }
            }
            while (!isPointValid);

            return point;
        }
        #endregion // HEXAGON
        
        #region DOT PRODUCT
        /// <summary>
        /// Computes the rounded dot product of two vectors.
        /// </summary>
        /// <param name="ax">First vector x.</param>
        /// <param name="ay">First vector y.</param>
        /// <param name="bx">Second vector x.</param>
        /// <param name="by">Second vector y.</param>
        /// <returns>Rounded dot product.</returns>
        public static float ComputeDotProduct(float ax, float ay, float bx, float by)
        {
            return ax * bx + ay * by;
        }

        /// <summary>
        /// Computes the rounded dot product of two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Rounded dot product.</returns>
        public static float ComputeDotProduct(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }
        #endregion // DOT PRODUCT
        
        #region WINDING NUMBER
        /// <summary>
        /// Computes a point winding number according to a given polygon.
        /// </summary>
        /// <param name="polygon">Polygon to check.</param>
        /// <param name="point">Point position to compute the winding number from.</param>
        /// <returns>Winding number (0 means the point is outside).</returns>
        public static int ComputeWindingNumber(Vector2[] polygon, Vector2 point)
        {
            int windingNumber = 0;

            Vector2[] polygonCopy = new Vector2[polygon.Length + 1];
            for (int i = 0; i < polygon.Length; ++i)
                polygonCopy[i] = polygon[i];

            polygonCopy[polygon.Length] = polygon[0];

            for (int i = polygon.Length - 1; i >= 0; --i)
            {
                if (polygonCopy[i].x <= point.x)
                {
                    if (polygonCopy[i + 1].x > point.x)
                    {
                        if (IsPointLeftToSegment(polygonCopy[i].x, polygonCopy[i].y, polygonCopy[i + 1].x, polygonCopy[i + 1].y, point.x, point.y) > 0)
                            windingNumber++;
                    }
                }
                else
                {
                    if (polygonCopy[i + 1].x <= point.x)
                    {
                        if (IsPointLeftToSegment(polygonCopy[i].x, polygonCopy[i].y, polygonCopy[i + 1].x, polygonCopy[i + 1].y, point.x, point.y) < 0)
                            windingNumber--;
                    }
                }
            }

            return windingNumber;
        }

        /// <summary>
        /// Computes a point winding number according to a given polygon, not considering z coordinate.
        /// </summary>
        /// <param name="polygon">Polygon to check.</param>
        /// <param name="point">Point position to compute the winding number from.</param>
        /// <returns>Winding number (0 means the point is outside).</returns>
        public static int ComputeWindingNumber(Vector3[] polygon, Vector3 point)
        {
            int windingNumber = 0;

            Vector3[] polygonCopy = new Vector3[polygon.Length + 1];
            for (int i = 0; i < polygon.Length; ++i)
                polygonCopy[i] = polygon[i];

            polygonCopy[polygon.Length] = polygon[0];

            for (int i = polygon.Length - 1; i >= 0; --i)
            {
                if (polygonCopy[i].x <= point.x)
                {
                    if (polygonCopy[i + 1].x > point.x)
                    {
                        if (IsPointLeftToSegment(polygonCopy[i].x, polygonCopy[i].y, polygonCopy[i + 1].x, polygonCopy[i + 1].y, point.x, point.y) > 0)
                            windingNumber++;
                    }
                }
                else
                {
                    if (polygonCopy[i + 1].x <= point.x)
                    {
                        if (IsPointLeftToSegment(polygonCopy[i].x, polygonCopy[i].y, polygonCopy[i + 1].x, polygonCopy[i + 1].y, point.x, point.y) < 0)
                            windingNumber--;
                    }
                }
            }

            return windingNumber;
        }
        #endregion // WINDING NUMBER
        
        #region GENERAL
        /// <summary>
        /// Returns the closest point on a segment to a reference point using an algorithm explained here: https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd.
        /// </summary>
        /// <param name="ax">Segment first point x.</param>
        /// <param name="ay">Segment first point y.</param>
        /// <param name="bx">Segment second point x.</param>
        /// <param name="by">Segment second point y.</param>
        /// <param name="px">Reference point x.</param>
        /// <param name="py">Reference point y.</param>
        /// <returns>Closest point to point p on the segment.</returns>
        public static System.Tuple<float, float> ComputeClosestPointOnSegment(float ax, float ay, float bx, float by, float px, float py)
        {
            float d = ComputeDotProduct(px - ax, py - ay, bx - ax, by - ay) / ComputeDotProduct(bx - ax, by - ay, bx - ax, by - ay);
            d = d < 0f ? 0f : d > 1f ? 1f : d;
            return System.Tuple.Create(ax + d * (bx - ax), ay + d * (by - ay));
        }

        /// <summary>
        /// Returns the closest point on a segment to a reference point using an algorithm explained here: https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd.
        /// </summary>
        /// <param name="a">Segment first point.</param>
        /// <param name="b">Segment second point.</param>
        /// <param name="p">Reference point.</param>
        /// <returns>Closest point to point p on the segment.</returns>
        public static Vector2 ComputeClosestPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            float d = Vector2.Dot(p - a, b - a) / Vector2.Dot(b - a, b - a);
            d = d < 0f ? 0f : d > 1f ? 1f : d;
            return a + d * (b - a);
        }

        /// <summary>
        /// Returns the distance from a point to its closest point on a segment.
        /// </summary>
        /// <param name="a">Segment first point.</param>
        /// <param name="b">Segment second point.</param>
        /// <param name="p">Reference point.</param>
        /// <returns>Distance from closest point on the segment to point p.</returns>
        public static float ComputePointDistanceToSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            return Mathf.Sqrt(ComputePointSqrDistanceToSegment(a, b, p));
        }

        /// <summary>
        /// Returns the squared distance from a point to its closest point on a segment.
        /// </summary>
        /// <param name="a">Segment first point.</param>
        /// <param name="b">Segment second point.</param>
        /// <param name="p">Reference point.</param>
        /// <returns>Squared distance from closest point on the segment to point p.</returns>
        public static float ComputePointSqrDistanceToSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            float dx = b.x - a.x;
            float dy = b.y - a.y;

            if (dx == 0 && dy == 0) // Segment is actually a point.
            {
                dx = p.x - a.x;
                dy = p.y - a.y;
                return dx * dx + dy * dy;
            }

            float t = ((p.x - a.x) * dx + (p.y - a.y) * dy) / (dx * dx + dy * dy);

            if (t < 0f)
            {
                dx = p.x - a.x;
                dy = p.y - a.y;
            }
            else if (t > 1f)
            {
                dx = p.x - b.x;
                dy = p.y - b.y;
            }
            else
            {
                Vector2 closest = new Vector2(a.x + t * dx, a.y + t * dy);
                dx = p.x - closest.x;
                dy = p.y - closest.y;
            }

            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Computes the intersection point of two segments in 2D space.
        /// </summary>
        /// <param name="a1">First segment first point.</param>
        /// <param name="a2">First segment second point.</param>
        /// <param name="b1">Second segment first point.</param>
        /// <param name="b2">Second segment second point.</param>
        /// <param name="intersection">Intersection point (equal to (-1,-1) if there's no intersection).</param>
        /// <returns>True if there is an intersection, else false.</returns>
        public static bool ComputeSegmentsIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.one * -1f;

            float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);
            if (d == 0f)
                return false;

            float u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
            float v = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;

            if (u < 0f || u > 1f || v < 0f || v > 1f)
                return false;

            intersection.x = a1.x + u * (a2.x - a1.x);
            intersection.y = a1.y + u * (a2.y - a1.y);
            return true;
        }

        /// <summary>
        /// Checks if a point is left to an edge, using an algorithm explained here http://geomalgorithms.com/a03-_inclusion.html.
        /// </summary>
        /// <param name="ax">Segment first point x.</param>
        /// <param name="ay">Segment first point y.</param>
        /// <param name="bx">Segment second point x.</param>
        /// <param name="by">Segment second point y.</param>
        /// <param name="px">Point x.</param>
        /// <param name="py">Point y.</param>
        /// <returns>1 if it is left, -1 if not, 0 if it is right on the segment.</returns>
        public static int IsPointLeftToSegment(float ax, float ay, float bx, float by, float px, float py)
        {
            float positionFactor = (by - ay) * (px - ax) - (py - ay) * (bx - ax);
            return positionFactor > 0f ? 1 : positionFactor < 0f ? -1 : 0;
        }

        /// <summary>
        /// Checks if a point is left to an edge, using an algorithm explained here http://geomalgorithms.com/a03-_inclusion.html.
        /// </summary>
        /// <param name="a">Segment first point.</param>
        /// <param name="b">Segment second point.</param>
        /// <param name="p">Reference point.</param>
        /// <returns>1 if it is left, -1 if not, 0 if it is right on the segment.</returns>
        public static int IsPointLeftToSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            float positionFactor = (b.y - a.y) * (p.x - a.x) - (p.y - a.y) * (b.x - a.x);
            return positionFactor > 0f ? 1 : positionFactor < 0f ? -1 : 0;
        }

        /// <summary>
        /// Returns the orientation of a triangle.
        /// </summary>
        /// <param name="a">Triangle first point.</param>
        /// <param name="b">Triangle second point.</param>
        /// <param name="c">Triangle third point.</param>
        /// <returns>Triangle orientation (collinear, clockwise or counterclockwise).</returns>
        public static E_TriangleOrientation ComputeTriangleOrientation(Vector3 a, Vector3 b, Vector3 c)
        {
            // Maths from https://www.geeksforgeeks.org/orientation-3-ordered-points/
            return ((b.z - a.z) * (c.x - b.x) - (b.x - a.x) * (c.z - b.z)) switch
            {
                0f => E_TriangleOrientation.COLLINEAR,
                > 0f => E_TriangleOrientation.CLOCKWISE,
                < 0f => E_TriangleOrientation.COUNTERCLOCKWISE,
                _ => E_TriangleOrientation.NONE,
            };
        }
        #endregion // GENERAL
        
        public enum E_TriangleOrientation
        {
            NONE,
            COLLINEAR,
            CLOCKWISE,
            COUNTERCLOCKWISE,
        }
    }
}