using System.Drawing;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Helpers
{
    public static class SectorIslandFixerRules
    {
        private const float VertexLerpAmount = 0.75f;
        private const float SideMidpointLerpAmount = 0.75f;

        public static Gate.GateType GetConnectionType(bool sameCluster)
        {
            return sameCluster
                ? Gate.GateType.props_gates_orb_accelerator_01_macro
                : Gate.GateType.props_gates_anc_gate_macro;
        }

        public static Point GetSectorCenter(Cluster cluster, Sector sector)
        {
            long clusterX = cluster.Position.X * 15000L * 1000L;
            long clusterY = cluster.Position.Y * 8660L * 1000L;

            (long x, long y) localOffset = GetSectorLocalOffset(cluster, sector);
            return new Point((int)(clusterX + localOffset.x), (int)(clusterY + localOffset.y));
        }

        public static IReadOnlyList<Point> GetTravelNodeCandidates(int diameterRadius)
        {
            var candidates = new List<Point>(12);
            PointF[] vertices =
            [
                new PointF(1f, 0f),
                new PointF(0.5f, -0.8660254f),
                new PointF(-0.5f, -0.8660254f),
                new PointF(-1f, 0f),
                new PointF(-0.5f, 0.8660254f),
                new PointF(0.5f, 0.8660254f)
            ];

            for (int i = 0; i < vertices.Length; i++)
            {
                PointF vertex = vertices[i];
                PointF nextVertex = vertices[(i + 1) % vertices.Length];
                PointF midpoint = new((vertex.X + nextVertex.X) / 2f, (vertex.Y + nextVertex.Y) / 2f);

                candidates.Add(ToWorldPoint(vertex, VertexLerpAmount, diameterRadius));
                candidates.Add(ToWorldPoint(midpoint, SideMidpointLerpAmount, diameterRadius));
            }

            return candidates;
        }

        public static Point SelectBestTravelNode(int diameterRadius, Point sourceCenter, Point targetCenter)
        {
            Point direction = new(targetCenter.X - sourceCenter.X, targetCenter.Y - sourceCenter.Y);
            IReadOnlyList<Point> candidates = GetTravelNodeCandidates(diameterRadius);

            Point best = candidates[0];
            double bestScore = double.MinValue;
            foreach (Point candidate in candidates)
            {
                double score = (candidate.X * (double)direction.X) + (candidate.Y * (double)direction.Y);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            return best;
        }

        public static int CalculateYaw(Point fromWorld, Point toWorld)
        {
            double dx = toWorld.X - fromWorld.X;
            double dy = toWorld.Y - fromWorld.Y;
            double yaw = Math.Atan2(-dy, dx) * (180.0 / Math.PI) + 90.0;
            if (yaw < 0)
                yaw += 360.0;

            return (int)Math.Round(yaw) % 360;
        }

        private static Point ToWorldPoint(PointF normalizedPoint, float lerpAmount, int diameterRadius)
        {
            float x = normalizedPoint.X * lerpAmount;
            float y = normalizedPoint.Y * lerpAmount;

            float worldX = x * diameterRadius / 2f;
            float worldY = -y * diameterRadius / 2f;
            return new Point((int)Math.Round(worldX), (int)Math.Round(worldY));
        }

        private static (long x, long y) GetSectorLocalOffset(Cluster cluster, Sector sector)
        {
            if (sector.CustomOffset.HasValue)
                return (sector.CustomOffset.Value.X, sector.CustomOffset.Value.Y);

            if (sector.Offset != default)
                return sector.Offset;

            if (sector.SectorRealOffset != default)
                return sector.SectorRealOffset;

            if (cluster.Sectors.Count <= 1)
                return (0, 0);

            const int amount = 1000000;
            return sector.Placement switch
            {
                SectorPlacement.TopLeft => (-amount, amount),
                SectorPlacement.BottomLeft => (-amount, -amount),
                SectorPlacement.TopRight => (amount, amount),
                SectorPlacement.BottomRight => (amount, -amount),
                SectorPlacement.MiddleLeft => (-amount, 0),
                SectorPlacement.MiddleRight => (amount, 0),
                SectorPlacement.MiddleTop => (0, amount),
                SectorPlacement.MiddleBottom => (0, -amount),
                _ => (0, 0)
            };
        }
    }
}
