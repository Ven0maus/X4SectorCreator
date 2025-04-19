using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class CharactersGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var characters = CollectCharacters();
            if (characters == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    characters
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/characters.xml")));
        }

        private static XElement CollectCharacters()
        {
            var xml = File.ReadAllText(Constants.DataPaths.DefaultCharactersPath);
            var defaultCharacters = Characters.Deserialize(xml).Character;

            var mainElement = new XElement("add", new XAttribute("sel", "/characters"));
            int count = 0;
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                foreach (var character in defaultCharacters)
                {
                    // Create a cloned character
                    var newCharacter = character.Clone();

                    // Adjust id
                    newCharacter.Id = newCharacter.Id.Replace("argon", faction.Id);

                    // Set faction correctly
                    if (newCharacter.OwnerObj?.List != null)
                        newCharacter.OwnerObj.List = newCharacter.OwnerObj.List.Replace("argon", faction.Id);
                    if (newCharacter.OwnerObj?.Exact != null)
                        newCharacter.OwnerObj.Exact = newCharacter.OwnerObj.Exact.Replace("argon", faction.Id);
                    if (newCharacter.CategoryObj?.Faction != null)
                        newCharacter.CategoryObj.Faction = newCharacter.CategoryObj.Faction.Replace("argon", faction.Id);

                    // Set race correctly
                    if (newCharacter.CategoryObj?.Race != null)
                        newCharacter.CategoryObj.Race = newCharacter.CategoryObj.Race.Replace("argon", faction.Primaryrace.ToLower());
                    newCharacter.Group = newCharacter.Group.Replace("argon", faction.Primaryrace.ToLower());

                    mainElement.Add(XElement.Parse(newCharacter.Serialize()));
                    count++;
                }
            }
            return count == 0 ? null : mainElement;
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
    }
}
