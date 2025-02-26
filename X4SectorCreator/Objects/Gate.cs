namespace X4SectorCreator.Objects
{
    public class Gate
    {
        public int Id { get; set; }
        public string ParentSectorName { get; set; }
        public string DestinationSectorName { get; set; }
        public string Source { get; set; } // format: c000_s000_z000
        public string Destination { get; set; } // format: c000_s000_z000
        public string SourcePath { get; private set; } // format: prefix_c000_connection/prefix_c000_s000_connection/prefix_c000_s000_z000_connection/prefix_g000_source_destination_connection
        public string DestinationPath { get; private set; } // format: prefix_c000_connection/prefix_c000_s000_connection/prefix_c000_s000_z000_connection/prefix_g000_source_destination_connection
        public Point Position { get; set; }
        public int Yaw { get; set; }
        public int Pitch { get; set; }
        public int Roll { get; set; }
        public GateType Type { get; set; }

        /// <summary>
        /// Used to make a connection with new sectors.
        /// </summary>
        /// <param name="modPrefix"></param>
        /// <param name="cluster"></param>
        /// <param name="sector"></param>
        /// <param name="zone"></param>
        public void SetSourcePath(string modPrefix, Cluster cluster, Sector sector, Zone zone)
        {
            string clusterConnection = $"{modPrefix}_CL_c{cluster.Id:D3}_connection";
            string sectorConnection = $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_connection";
            string zoneConnection = $"{modPrefix}_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_connection";
            string gateConnection = $"{modPrefix}_GA_g{Id:D3}_{Source}_{Destination}_connection";
            SourcePath = $"{clusterConnection}/{sectorConnection}/{zoneConnection}/{gateConnection}";
        }

        /// <summary>
        /// Used to make a connection with new sectors.
        /// </summary>
        /// <param name="modPrefix"></param>
        /// <param name="cluster"></param>
        /// <param name="sector"></param>
        /// <param name="zone"></param>
        public void SetDestinationPath(string modPrefix, Cluster cluster, Sector sector, Zone zone, Gate gate)
        {
            string clusterConnection = $"{modPrefix}_CL_c{cluster.Id:D3}_connection";
            string sectorConnection = $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_connection";
            string zoneConnection = $"{modPrefix}_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_connection";
            string gateConnection = $"{modPrefix}_GA_g{gate.Id:D3}_{gate.Source}_{gate.Destination}_connection";
            DestinationPath = $"{clusterConnection}/{sectorConnection}/{zoneConnection}/{gateConnection}";
        }

        /// <summary>
        /// Used to make a connection with existing sectors.
        /// </summary>
        /// <param name="path"></param>
        public void SetCustomSourcePath(string path)
        {
            SourcePath = path;
        }

        /// <summary>
        /// Used to make a connection with existing sectors.
        /// </summary>
        /// <param name="path"></param>
        public void SetCustomDestinationPath(string path)
        {
            DestinationPath = path;
        }

        public enum GateType
        {
            // Gate
            props_gates_anc_gate_macro,
            // Accelerator
            props_gates_orb_accelerator_01_macro
        }

        public override string ToString()
        {
            return ParentSectorName;
        }
    }
}
