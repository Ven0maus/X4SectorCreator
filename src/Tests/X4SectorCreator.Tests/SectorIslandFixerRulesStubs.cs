using System.Drawing;

namespace X4SectorCreator.Objects;

public class Gate
{
    public int Id { get; set; }
    public string ConnectionName { get; set; }
    public string ParentSectorName { get; set; }
    public string DestinationSectorName { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public string SourcePath { get; set; }
    public string DestinationPath { get; set; }
    public bool IsHighwayGate { get; set; }

    public enum GateType
    {
        props_gates_anc_gate_macro,
        props_gates_anc_gate_anim_macro,
        props_ter_gate_01_macro,
        props_gates_orb_accelerator_01_macro,
        props_gates_orb_accelerator_02_macro
    }
}

public class Cluster
{
    public int Id { get; set; }
    public Point Position { get; set; }
    public List<Sector> Sectors { get; set; } = [];
    public string BaseGameMapping { get; set; }
    public string ImportedMacroName { get; set; }
    public string ImportedConnectionName { get; set; }
    public string Dlc { get; set; }
    public string Name { get; set; }
    public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);
}

public class Sector
{
    public int Id { get; set; }
    public string BaseGameMapping { get; set; }
    public string Name { get; set; }
    public List<Zone> Zones { get; set; } = [];
    public Point? CustomOffset { get; set; }
    public (long X, long Y) Offset { get; set; }
    public (long X, long Y) SectorRealOffset { get; set; }
    public SectorPlacement Placement { get; set; }
    public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);
}

public class Zone
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Point Position { get; set; }
    public List<Gate> Gates { get; set; } = [];
    public List<Station> Stations { get; set; } = [];
    public string ImportedMacroName { get; set; }
    public bool IsBaseGame => Name != null;
}

public class Station
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Faction { get; set; }
    public string Owner { get; set; }
    public string Race { get; set; }
    public string Type { get; set; }
    public Point Position { get; set; }
    public string LocationType { get; set; }
    public string Location { get; set; }
}

public class ClusterCollection
{
    public List<Cluster> Clusters { get; set; } = [];
}

public class VanillaChanges
{
    public List<Cluster> RemovedClusters { get; set; } = [];
    public List<ModifiedCluster> ModifiedClusters { get; set; } = [];
    public List<RemovedSector> RemovedSectors { get; set; } = [];
    public List<RemovedConnection> RemovedConnections { get; set; } = [];
}

public class ModifiedCluster
{
    public Cluster Old { get; set; } = new();
    public Cluster New { get; set; } = new();
}

public class RemovedSector
{
    public Cluster VanillaCluster { get; set; } = new();
    public Sector Sector { get; set; } = new();
}

public class RemovedConnection
{
    public Cluster VanillaCluster { get; set; } = new();
    public Sector Sector { get; set; } = new();
    public Zone Zone { get; set; } = new();
    public Gate Gate { get; set; } = new();
}

public enum SectorPlacement
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    MiddleLeft,
    MiddleRight,
    MiddleTop,
    MiddleBottom,
}
