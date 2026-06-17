namespace X4SectorCreator.Helpers
{
    public static class SectorMapInteractionRules
    {
        public const float GateNodeHitRadiusScale = 1.75f;
        public const float MinimumGateNodeHitRadius = 14f;

        public static float GetGateNodeHitRadius(float zoom, float gateSizeRadius)
        {
            float renderedNodeRadius = gateSizeRadius * zoom;
            return Math.Max(renderedNodeRadius * GateNodeHitRadiusScale, MinimumGateNodeHitRadius);
        }

        public static bool PreserveChildSectorLayoutAfterClusterMove() => true;

        public static bool UseCanonicalSectorDragLayout(int sectorCount) => sectorCount > 1;
    }
}
