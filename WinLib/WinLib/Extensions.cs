//using Jint.Native;
using Jint.Native;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace WinLib;
public static class Extensions
{
    public static dynamic? ToJSONNode(this JsValue x)
    {
        if (x == null) return null;
        return Util.FromObject(x.ToObject());
    }
    public static void SetTypeReference(this Jint.Engine engine, string name, Type type)
    {
        engine.SetValue(name, Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, type));
    }
#if false
    public static dynamic? FromJson(this string x)
    {
        if (x == null) return null;
        return Util.FromJson(x);
    }
#endif
    public static XmlDocument ToXmlDocument(this XDocument xDocument)
    {
        var xmlDocument = new XmlDocument();
        using (var xmlReader = xDocument.CreateReader())
        {
            xmlDocument.Load(xmlReader);
        }
        return xmlDocument;
    }
    public static XDocument ToXDocument(this XmlDocument xmlDocument)
    {
        using (var nodeReader = new XmlNodeReader(xmlDocument))
        {
            nodeReader.MoveToContent();
            return XDocument.Load(nodeReader);
        }
    }
    public static string ToStringWithDeclaration(this XDocument doc)
    {
        if (doc == null)
        {
            throw new ArgumentNullException("doc");
        }
        using (TextWriter writer = new Utf8StringWriter())
        {
            doc.Save(writer);
            return writer.ToString();
        }
    }
    public static void Add(this ExpandoObject? o, string key, object? value)
    {
        if (o == null) return;
        ((IDictionary<String, Object>)o).Add(key, value);
    }
    public static void Put(this ExpandoObject? o, string key, object? value)
    {
        if (o == null) return;
        ((IDictionary<String, Object>)o)[key] = value;
    }
    public static dynamic? Get(this ExpandoObject? o, string key)
    {
        if (o == null) return null;
        try
        {
            return ((IDictionary<String, Object>)o)[key];
        }
        catch (KeyNotFoundException ex)
        {
            return null;
        }
    }
    public static bool ContainsKey(this ExpandoObject? o, string key)
    {
        if (o == null) return false;
        return ((IDictionary<String, Object>)o).ContainsKey(key);
    }
    internal class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}