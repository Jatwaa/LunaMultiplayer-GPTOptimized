// Decompiled with JetBrains decompiler
// Type: LmpCommon.Xml.LunaXmlSerializer
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace LmpCommon.Xml
{
  public class LunaXmlSerializer
  {
    public static T ReadXmlFromPath<T>(string path) where T : class, new()
    {
      if (!File.Exists(path))
        return default (T);
      try
      {
        using (TextReader textReader = (TextReader) new StreamReader(path))
          return (T) new XmlSerializer(typeof (T)).Deserialize(textReader);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Could not open and read file from path {0}. Details: {1}", (object) path, (object) ex));
      }
    }

    public static T ReadXmlFromString<T>(string content) where T : class, new()
    {
      if (string.IsNullOrEmpty(content))
        return default (T);
      try
      {
        using (TextReader textReader = (TextReader) new StringReader(content))
          return (T) new XmlSerializer(typeof (T)).Deserialize(textReader);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Could not open and read file content. Details: {0}", (object) ex));
      }
    }

    public static object ReadXmlFromPath(Type classType, string path)
    {
      if (!File.Exists(path))
        return (object) null;
      try
      {
        using (TextReader textReader = (TextReader) new StreamReader(path))
          return new XmlSerializer(classType).Deserialize(textReader);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Could not open and read file from path {0}. Details: {1}", (object) path, (object) ex));
      }
    }

    public static void WriteToXmlFile(object objectToSerialize, string path)
    {
      string xml = LunaXmlSerializer.SerializeToXml(objectToSerialize);
      if (ContentChecker.ContentsAreEqual(xml, path))
        return;
      File.WriteAllText(path, xml);
    }

    public static string SerializeToXml(object objectToSerialize)
    {
      string xml;
      try
      {
        using (StringWriter w = new StringWriter())
        {
          using (XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w))
          {
            xmlTextWriter.Formatting = Formatting.Indented;
            new XmlSerializer(objectToSerialize.GetType()).Serialize((XmlWriter) xmlTextWriter, objectToSerialize);
            string s = LunaXmlSerializer.WriteComments(objectToSerialize, w.ToString());
            using (StringWriter output = new StringWriter())
            {
              using (StringReader input = new StringReader(s))
              {
                using (XmlTextReader reader = new XmlTextReader((TextReader) input))
                {
                  XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, new XmlWriterSettings()
                  {
                    Indent = true
                  });
                  while (reader.Read())
                  {
                    switch (reader.NodeType)
                    {
                      case XmlNodeType.Element:
                        xmlWriter.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        xmlWriter.WriteAttributes((XmlReader) reader, true);
                        if (reader.IsEmptyElement)
                        {
                          xmlWriter.WriteFullEndElement();
                          break;
                        }
                        break;
                      case XmlNodeType.Text:
                        xmlWriter.WriteString(reader.Value);
                        break;
                      case XmlNodeType.CDATA:
                        xmlWriter.WriteCData(reader.Value);
                        break;
                      case XmlNodeType.EntityReference:
                        xmlWriter.WriteEntityRef(reader.Name);
                        break;
                      case XmlNodeType.ProcessingInstruction:
                      case XmlNodeType.XmlDeclaration:
                        xmlWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                        break;
                      case XmlNodeType.Comment:
                        xmlWriter.WriteComment(reader.Value);
                        break;
                      case XmlNodeType.DocumentType:
                        xmlWriter.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                        break;
                      case XmlNodeType.Whitespace:
                      case XmlNodeType.SignificantWhitespace:
                        xmlWriter.WriteWhitespace(reader.Value);
                        break;
                      case XmlNodeType.EndElement:
                        xmlWriter.WriteFullEndElement();
                        break;
                    }
                  }
                  xmlWriter.WriteEndDocument();
                  xmlWriter.Flush();
                  xmlWriter.Close();
                  xml = output.ToString();
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Could not write xml. Details: {0}", (object) ex));
      }
      return xml;
    }

    private static string WriteComments(object objectToSerialize, string contents)
    {
      try
      {
        using (StringWriter writer = new StringWriter())
        {
          Dictionary<string, string> propertyComments = LunaXmlSerializer.GetPropertiesAndComments(objectToSerialize);
          if (!propertyComments.Any<KeyValuePair<string, string>>())
            return contents;
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(contents);
          XmlNode xmlNode = xmlDocument.SelectSingleNode(objectToSerialize.GetType().Name);
          if (xmlNode == null)
            return contents;
          foreach (XmlNode refChild in xmlNode.ChildNodes.Cast<XmlNode>().Where<XmlNode>((Func<XmlNode, bool>) (n => propertyComments.ContainsKey(n.Name))))
            xmlNode.InsertBefore((XmlNode) xmlDocument.CreateComment(propertyComments[refChild.Name]), refChild);
          xmlDocument.Save((TextWriter) writer);
          return writer.ToString();
        }
      }
      catch (Exception ex)
      {
      }
      return contents;
    }

    private static Dictionary<string, string> GetPropertiesAndComments(
      object objectToSerialize)
    {
      return ((IEnumerable<PropertyInfo>) objectToSerialize.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => ((IEnumerable<object>) p.GetCustomAttributes(typeof (XmlCommentAttribute), false)).Any<object>())).Select(v => new
      {
        Name = v.Name,
        Value = ((XmlCommentAttribute) v.GetCustomAttributes(typeof (XmlCommentAttribute), false)[0]).Value
      }).ToDictionary(t => t.Name, t => t.Value);
    }
  }
}
