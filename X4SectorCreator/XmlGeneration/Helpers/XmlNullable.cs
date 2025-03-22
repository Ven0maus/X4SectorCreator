using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace X4SectorCreator.XmlGeneration.Helpers
{
    internal class XmlNullable : IXmlSerializable
    {
        // Write the object to XML using custom serialization logic
        public void WriteXml(XmlWriter writer)
        {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                var isNullable = property.GetCustomAttribute<XmlNullableAttribute>() != null;
                var value = property.GetValue(this);

                if (isNullable)
                {
                    if (value != null)
                    {
                        writer.WriteStartElement(property.Name);
                        writer.WriteString(value.ToString());
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    // Serialize non-nullable properties
                    writer.WriteStartElement(property.Name);
                    writer.WriteString(value?.ToString() ?? string.Empty); // Handle null reference types with empty string
                    writer.WriteEndElement();
                }
            }
        }

        // Read the object from XML (for deserialization)
        public void ReadXml(XmlReader reader)
        {
            var properties = this.GetType().GetProperties();

            // Move to the first element inside the object
            reader.ReadStartElement();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                // Read the property name and its value
                string propertyName = reader.LocalName;
                var property = this.GetType().GetProperty(propertyName);

                if (property != null && reader.Read())
                {
                    var value = reader.Value;

                    // Check if the property is nullable and handle accordingly
                    if (property.GetCustomAttribute<XmlNullableAttribute>() != null)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            property.SetValue(this, null); // Set null for empty values
                        }
                        else
                        {
                            property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                    else
                    {
                        // Non-nullable properties are directly set
                        property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                    }
                }

                reader.ReadEndElement();
            }

            // Move past the end of the current element
            reader.ReadEndElement();
        }

        public XmlSchema GetSchema() => null; // Not needed for custom serialization logic
    }
}
