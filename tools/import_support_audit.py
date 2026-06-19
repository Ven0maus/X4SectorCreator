#!/usr/bin/env python3

import json
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


SUPPORT_MATRIX = {
    "cluster_display_name": "supported",
    "cluster_description": "supported",
    "cluster_background_visual": "partial",
    "cluster_soundtrack": "partial",
    "cluster_position": "partial",
    "sector_display_name": "supported",
    "sector_description": "supported",
    "sector_linkage_identity": "partial",
    "sector_import_order": "supported",
    "sector_duplicate_names": "supported",
    "sector_owner": "missing",
    "sector_sunlight": "supported",
    "sector_economy": "supported",
    "sector_security": "supported",
    "sector_factionlogic": "supported",
    "sector_allowrandomanomaly": "supported",
    "sector_resourceareas": "missing",
    "zone_positions": "supported",
    "gate_pairing_by_path": "supported",
    "gate_rotation": "supported",
    "translation_file_order": "supported",
    "translation_multilanguage_selection": "missing",
}


def load_xml(path: Path):
    return ET.fromstring(path.read_text())


def summarize_presence(mod_dir: Path) -> dict:
    mapdefaults_path = mod_dir / "libraries" / "mapdefaults.xml"
    t_dir = mod_dir / "t"

    presence = {
        "cluster_description": 0,
        "cluster_background_visual": 0,
        "cluster_soundtrack": 0,
        "sector_description": 0,
        "sector_owner": 0,
        "sector_sunlight": 0,
        "sector_economy": 0,
        "sector_security": 0,
        "sector_factionlogic": 0,
        "sector_allowrandomanomaly": 0,
        "sector_resourceareas": 0,
        "translation_files": 0,
    }

    if mapdefaults_path.exists():
        root = load_xml(mapdefaults_path)
        for dataset in root.findall("./dataset"):
            props = dataset.find("./properties")
            if props is None:
                continue

            ident = props.find("./identification")
            area = props.find("./area")
            music = props.find("./music") or props.find("./system/music")
            resourceareas = props.findall("./resourceareas/resourcearea")

            macro = dataset.attrib.get("macro", "")
            is_cluster = "_CL_" in macro or macro.endswith("_macro") and "_SE_" not in macro and "_ZO_" not in macro
            is_sector = "_SE_" in macro or "Sector" in macro

            if ident is not None and ident.attrib.get("description"):
                if is_cluster:
                    presence["cluster_description"] += 1
                if is_sector:
                    presence["sector_description"] += 1

            if ident is not None and ident.attrib.get("image") and is_cluster:
                presence["cluster_background_visual"] += 1

            if music is not None and music.attrib.get("ref") and is_cluster:
                presence["cluster_soundtrack"] += 1

            if area is not None and is_sector:
                presence["sector_sunlight"] += int("sunlight" in area.attrib)
                presence["sector_economy"] += int("economy" in area.attrib)
                presence["sector_security"] += int("security" in area.attrib)
                presence["sector_factionlogic"] += int("factionlogic" in area.attrib)
                tags = area.attrib.get("tags", "")
                if "allowrandomanomaly" in tags.lower():
                    presence["sector_allowrandomanomaly"] += 1

            presence["sector_resourceareas"] += len(resourceareas)

    if t_dir.exists():
        presence["translation_files"] = len(list(t_dir.glob("*.xml")))

    return presence


def audit(mod_dir: Path) -> dict:
    presence = summarize_presence(mod_dir)
    rows = []

    for capability, support in SUPPORT_MATRIX.items():
        present = None
        if capability in presence:
            present = presence[capability]
        rows.append(
            {
                "capability": capability,
                "support": support,
                "source_presence": present,
            }
        )

    return {
        "mod_dir": str(mod_dir),
        "rows": rows,
        "high_risk_missing": [
            row["capability"]
            for row in rows
            if row["support"] == "missing" and row["source_presence"] not in (None, 0)
        ],
    }


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: import_support_audit.py MOD_DIR [MOD_DIR ...]", file=sys.stderr)
        return 2

    reports = [audit(Path(arg).resolve()) for arg in sys.argv[1:]]
    print(json.dumps(reports, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
