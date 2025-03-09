# X4 Sector Creator

A tool to help mod new sectors and gate connections in *X4: Foundations*. This tool provides an easy way to create clusters, define sectors within those clusters, and establish gate connections between them. It generates a fully working mod directory with all necessary XML files, making the modding process seamless.

## Features
- Cluster creation / editing / removing
- Sector creation / editing / removing (up to 3 per cluster)
- Gate creation between custom & basegame sectors (dragging + rotating)
- Region creation (dragging + resizing + ability to fully choose what is inside the region)
- Fully functional sector map that visualizes dlcs content and gate connections
- Possibility to create a complete custom universe if you wish
- Auto-detect new versions and will show a popup if new version available
- Easily edit each component by double clicking on them (clusters, sectors, gates, regions)
- Detect when you have connections with DLC sectors and will add dependency to content.xml automatically
- Export/Import configuration (to continue working on your changes later)
- Export to XML (creates a mod folder with all the required files, it can be directly put in extensions folder and run)

## Installation
1. Go to the releases page of this repository and download the latest release. (https://github.com/Ven0maus/X4SectorCreator/releases)
2. Unpack the compressed zip archive
3. Run the executable file

## Quick Guide

### Creating a New Cluster
1. Click the **"New"** button under the **Cluster** section.
2. This will automatically create a **sector** with the same name.
3. You can edit the sector by **double-clicking** on the new sector entry.

### Creating a Connection Between Sectors
1. Select the **source sector**.
2. Click the **"New"** button under the **Connections** section.
3. A prompt will appear where you can:
   - Select the **target sector**.
   - Set the **gate position** and **rotation**.

### Positioning the Gate
- **Left-click** within the hexagons to **move** the gate.
- **Right-click** to **rotate** the gate.

### Generating the XML
1. After adding your connection, press **"Generate XML"**.
2. A prompt will appear asking for:
   - The **name of your mod**.
   - A **unique prefix** to prevent overlap with other mods.

Make sure your **prefix is unique** to avoid conflicts with other mods.

Add the resulting folder in your gamedirectory/extensions folder and enable it in-game to see the results.

## License
This project is licensed under the GNU V2.0 License.

---

Happy modding!

## Screenshots

![Image](https://github.com/user-attachments/assets/69380644-430c-4c82-b223-c48753190483)
![Image](https://github.com/user-attachments/assets/be1090f2-3b0d-4305-ae49-93906add6f66)
![Image](https://github.com/user-attachments/assets/9b48c899-a98d-4929-8308-09bfcefdaba9)
![Image](https://github.com/user-attachments/assets/c7bcad70-7a85-4b74-9908-43b0ed7ff0d2)
![Image](https://github.com/user-attachments/assets/114ce7e2-1673-41ae-846e-a9d154667bd5)
