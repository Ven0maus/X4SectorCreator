#!/usr/bin/env python3

import json
import sys
import xml.etree.ElementTree as ET
from collections import Counter, defaultdict
from pathlib import Path


def load_xml(path: Path):
    return ET.fromstring(path.read_text())


def audit_mod(mod_dir: Path) -> dict:
    report = {
        "mod_dir": str(mod_dir),
        "content": {},
        "maps": {},
        "mapdefaults": {},
        "translations": {},
        "risks": [],
    }

    content_path = mod_dir / "content.xml"
    if content_path.exists():
        content_root = load_xml(content_path)
        report["content"] = {
            "id": content_root.attrib.get("id"),
            "name": content_root.attrib.get("name"),
            "version": content_root.attrib.get("version"),
        }

    map_files = sorted((mod_dir / "maps").glob("**/*.xml"))
    cluster_macros = sector_macros = zone_macros = gate_connections = destination_connections = 0
    sector_names = Counter()
    cluster_names = Counter()
    sector_layouts = Counter()
    zone_gate_counts = []

    for xml_path in map_files:
        root = load_xml(xml_path)
        cluster_macros += len(root.findall(".//macro[@class='cluster']"))
        sector_nodes = root.findall(".//macro[@class='sector']")
        zone_nodes = root.findall(".//macro[@class='zone']")
        sector_macros += len(sector_nodes)
        zone_macros += len(zone_nodes)
        gate_connections += len(root.findall(".//connection[@ref='gates']"))
        destination_connections += len(root.findall(".//connection[@ref='destination']"))

        for macro in root.findall(".//macro[@class='cluster']"):
            cluster_names[macro.attrib.get("name", "")] += 1

        for macro in sector_nodes:
            sector_names[macro.attrib.get("name", "")] += 1
            layout = []
            for conn in macro.findall("./connections/connection[@ref='zones']"):
                pos = conn.find("./offset/position")
                if pos is not None:
                    layout.append((pos.attrib.get("x", "0"), pos.attrib.get("z", "0")))
            sector_layouts[tuple(sorted(layout))] += 1

        for macro in zone_nodes:
            zone_gate_counts.append(len(macro.findall("./connections/connection[@ref='gates']")))

    report["maps"] = {
        "xml_files": len(map_files),
        "cluster_macros": cluster_macros,
        "sector_macros": sector_macros,
        "zone_macros": zone_macros,
        "gate_connections": gate_connections,
        "destination_connections": destination_connections,
        "duplicate_sector_macro_names": sum(1 for _, count in sector_names.items() if count > 1),
        "duplicate_cluster_macro_names": sum(1 for _, count in cluster_names.items() if count > 1),
        "reused_sector_layouts": sum(1 for _, count in sector_layouts.items() if count > 1),
        "zones_with_gates_histogram": dict(sorted(Counter(zone_gate_counts).items())),
    }

    mapdefaults_path = mod_dir / "libraries" / "mapdefaults.xml"
    if mapdefaults_path.exists():
        root = load_xml(mapdefaults_path)
        datasets = root.findall("./dataset")
        music_refs = 0
        image_refs = 0
        descriptions = 0
        sunlight = economy = security = factionlogic = tags = 0
        resourceareas = 0
        macro_prefix_counter = Counter()

        for dataset in datasets:
            macro = dataset.attrib.get("macro", "")
            macro_prefix_counter[macro.split("_")[0]] += 1
            props = dataset.find("./properties")
            if props is None:
                continue
            ident = props.find("./identification")
            if ident is not None:
                if ident.attrib.get("image"):
                    image_refs += 1
                if ident.attrib.get("description"):
                    descriptions += 1
            area = props.find("./area")
            if area is not None:
                sunlight += int("sunlight" in area.attrib)
                economy += int("economy" in area.attrib)
                security += int("security" in area.attrib)
                factionlogic += int("factionlogic" in area.attrib)
                tags += int("tags" in area.attrib)
            music = props.find("./music")
            if music is None:
                music = props.find("./system/music")
            if music is not None and music.attrib.get("ref"):
                music_refs += 1
            resourceareas += len(props.findall("./resourceareas/resourcearea"))

        report["mapdefaults"] = {
            "datasets": len(datasets),
            "datasets_with_image": image_refs,
            "datasets_with_description": descriptions,
            "datasets_with_music": music_refs,
            "datasets_with_sunlight": sunlight,
            "datasets_with_economy": economy,
            "datasets_with_security": security,
            "datasets_with_factionlogic": factionlogic,
            "datasets_with_tags": tags,
            "resourcearea_entries": resourceareas,
            "macro_prefix_histogram": dict(sorted(macro_prefix_counter.items())),
        }

    t_dir = mod_dir / "t"
    if t_dir.exists():
        files = sorted(t_dir.glob("*.xml"))
        pages = 0
        entries = 0
        languages = []
        for xml_path in files:
            root = load_xml(xml_path)
            pages += len(root.findall("./page"))
            entries += len(root.findall("./page/t"))
            if root.attrib.get("language"):
                languages.append(root.attrib["language"])
        report["translations"] = {
            "files": len(files),
            "pages": pages,
            "entries": entries,
            "languages": languages,
        }

    risks = report["risks"]
    if report["maps"].get("reused_sector_layouts", 0) > 0:
        risks.append("reused_sector_layouts_make_name_or_layout_based_matching_ambiguous")
    if report["mapdefaults"].get("datasets_with_music", 0) > 0:
        risks.append("cluster_soundtrack_metadata_present")
    if report["mapdefaults"].get("datasets_with_image", 0) > 0:
        risks.append("cluster_or_sector_image_metadata_present")
    if report["mapdefaults"].get("datasets_with_description", 0) > 0:
        risks.append("translation_backed_descriptions_present")
    if report["mapdefaults"].get("datasets_with_sunlight", 0) > 0:
        risks.append("sector_area_metadata_present")
    if report["mapdefaults"].get("resourcearea_entries", 0) > 0:
        risks.append("resource_areas_present")
    if report["translations"].get("files", 0) > 1:
        risks.append("multiple_translation_files_present")
    if report["maps"].get("destination_connections", 0) > 0:
        risks.append("explicit_gate_pair_links_present")

    return report


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: import_audit.py MOD_DIR [MOD_DIR ...]", file=sys.stderr)
        return 2

    reports = [audit_mod(Path(arg).resolve()) for arg in sys.argv[1:]]
    print(json.dumps(reports, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
