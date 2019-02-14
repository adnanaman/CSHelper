using System.ComponentModel;
using System.IO;
using System.Xml;
using ATPI.TWME.Helpers;



public static class CustomAttributeXmlSerializer
{ 
    public static string XmlSerializeToString(this object objectInstance)
    {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
    }
    public static string AsString(this XmlDocument xmlDoc)
    {
        using (var sw = new StringWriter())
        {
            using (var tx = new XmlTextWriter(sw))
            {
                xmlDoc.WriteTo(tx);
                return sw.ToString();
            }
        }
    }
    public static string Serialize<T>(T classObj)
    {
        var rootName = typeof(T).Name;
        var strXml = classObj.XmlSerializeToString();
        var xDoc = new XmlDocument();
        xDoc.LoadXml(strXml);

        var props = typeof(T).GetProperties();
        foreach (var prop in props)
        {
            var attrs = prop.GetCustomAttributes(true);
            foreach (XmlElement node in xDoc.SelectNodes($"/{rootName}/{prop.Name}"))
            {
                foreach (var attr in attrs)
                {
                    if (attr is DisplayNameAttribute attribute)
                    {
                        var value = attribute.DisplayName;
                        var newAttribute = xDoc.CreateAttribute("", "DisplayName", null);
                        newAttribute.Value = value;
                        node.Attributes.Append(newAttribute);
                    }
                    else if (attr is DescriptionAttribute)
                    {
                        var value = ((System.ComponentModel.DescriptionAttribute)attr).Description;
                        var newAttribute = xDoc.CreateAttribute("", "Description", null);
                        newAttribute.Value = value;
                        node.Attributes.Append(newAttribute);
                    }
                    else if (attr is CategoryAttribute)
                    {
                        var value = ((System.ComponentModel.CategoryAttribute)attr).Category;
                        var newAttribute = xDoc.CreateAttribute("", "Category", null);
                        newAttribute.Value = value;
                        node.Attributes.Append(newAttribute);
                    }
                    else if (attr is BrowsableAttribute)
                    {
                        var value = ((System.ComponentModel.BrowsableAttribute)attr).Browsable;
                        var newAttribute = xDoc.CreateAttribute("", "Browsable", null);
                        newAttribute.Value = value.ToString();
                        node.Attributes.Append(newAttribute);
                    }
                }
            }
        }
        return xDoc.AsString();
    }
}
