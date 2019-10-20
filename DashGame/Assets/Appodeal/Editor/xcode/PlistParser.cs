using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

<<<<<<< HEAD
namespace Unity.Appodeal.Xcode {

    public class PlistElement {
        protected PlistElement () { }

        // convenience methods
        public string AsString () { return ((PlistElementString) this).value; }
        public int AsInteger () { return ((PlistElementInteger) this).value; }
        public bool AsBoolean () { return ((PlistElementBoolean) this).value; }
        public PlistElementArray AsArray () { return (PlistElementArray) this; }
        public PlistElementDict AsDict () { return (PlistElementDict) this; }
        public float AsReal () { return ((PlistElementReal) this).value; }
        public DateTime AsDate () { return ((PlistElementDate) this).value; }

        public PlistElement this [string key] {
            get { return AsDict () [key]; }
            set { AsDict () [key] = value; }
        }
    }

    public class PlistElementString : PlistElement {
        public PlistElementString (string v) { value = v; }
=======
namespace Unity.Appodeal.Xcode
{

    public class PlistElement
    {
        protected PlistElement() {}

        // convenience methods
        public string AsString() { return ((PlistElementString)this).value; }
        public int AsInteger()   { return ((PlistElementInteger)this).value; }
        public bool AsBoolean()  { return ((PlistElementBoolean)this).value; }
        public PlistElementArray AsArray() { return (PlistElementArray)this; }
        public PlistElementDict AsDict()   { return (PlistElementDict)this; }
        public float AsReal() { return ((PlistElementReal)this).value; }
        public DateTime AsDate() { return ((PlistElementDate)this).value; }

        public PlistElement this[string key]
        {
            get { return AsDict()[key]; }
            set { AsDict()[key] = value; }
        }
    }

    public class PlistElementString : PlistElement
    {
        public PlistElementString(string v) { value = v; }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        public string value;
    }

<<<<<<< HEAD
    public class PlistElementInteger : PlistElement {
        public PlistElementInteger (int v) { value = v; }
=======
    public class PlistElementInteger : PlistElement
    {
        public PlistElementInteger(int v) { value = v; }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        public int value;
    }

<<<<<<< HEAD
    public class PlistElementReal : PlistElement {
        public PlistElementReal (float v) { value = v; }
=======
    public class PlistElementReal : PlistElement
    {
        public PlistElementReal(float v) { value = v; }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        public float value;
    }

<<<<<<< HEAD
    public class PlistElementBoolean : PlistElement {
        public PlistElementBoolean (bool v) { value = v; }
=======
    public class PlistElementBoolean : PlistElement
    {
        public PlistElementBoolean(bool v) { value = v; }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        public bool value;
    }

<<<<<<< HEAD
    public class PlistElementDate : PlistElement {
        public PlistElementDate (DateTime date) { value = date; }
=======
    public class PlistElementDate : PlistElement
    {
        public PlistElementDate(DateTime date) { value = date; }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        public DateTime value;
    }

<<<<<<< HEAD
    public class PlistElementDict : PlistElement {
        public PlistElementDict () : base () { }

        private SortedDictionary<string, PlistElement> m_PrivateValue = new SortedDictionary<string, PlistElement> ();
        public IDictionary<string, PlistElement> values { get { return m_PrivateValue; } }

        new public PlistElement this [string key] {
            get {
                if (values.ContainsKey (key))
=======
    public class PlistElementDict : PlistElement
    {
        public PlistElementDict() : base() {}

        private SortedDictionary<string, PlistElement> m_PrivateValue = new SortedDictionary<string, PlistElement>();
        public IDictionary<string, PlistElement> values { get { return m_PrivateValue; }}

        new public PlistElement this[string key]
        {
            get {
                if (values.ContainsKey(key))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    return values[key];
                return null;
            }
            set { this.values[key] = value; }
        }

<<<<<<< HEAD
        // convenience methods
        public void SetInteger (string key, int val) {
            values[key] = new PlistElementInteger (val);
        }

        public void SetString (string key, string val) {
            values[key] = new PlistElementString (val);
        }

        public void SetBoolean (string key, bool val) {
            values[key] = new PlistElementBoolean (val);
        }

        public void SetDate (string key, DateTime val) {
            values[key] = new PlistElementDate (val);
        }

        public void SetReal (string key, float val) {
            values[key] = new PlistElementReal (val);
        }

        public PlistElementArray CreateArray (string key) {
            var v = new PlistElementArray ();
=======

        // convenience methods
        public void SetInteger(string key, int val)
        {
            values[key] = new PlistElementInteger(val);
        }

        public void SetString(string key, string val)
        {
            values[key] = new PlistElementString(val);
        }

        public void SetBoolean(string key, bool val)
        {
            values[key] = new PlistElementBoolean(val);
        }

        public void SetDate(string key, DateTime val)
        {
            values[key] = new PlistElementDate(val);
        }

        public void SetReal(string key, float val)
        {
            values[key] = new PlistElementReal(val);
        }

        public PlistElementArray CreateArray(string key)
        {
            var v = new PlistElementArray();
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            values[key] = v;
            return v;
        }

<<<<<<< HEAD
        public PlistElementDict CreateDict (string key) {
            var v = new PlistElementDict ();
=======
        public PlistElementDict CreateDict(string key)
        {
            var v = new PlistElementDict();
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            values[key] = v;
            return v;
        }
    }

<<<<<<< HEAD
    public class PlistElementArray : PlistElement {
        public PlistElementArray () : base () { }
        public List<PlistElement> values = new List<PlistElement> ();

        // convenience methods
        public void AddString (string val) {
            values.Add (new PlistElementString (val));
        }

        public void AddInteger (int val) {
            values.Add (new PlistElementInteger (val));
        }

        public void AddBoolean (bool val) {
            values.Add (new PlistElementBoolean (val));
        }

        public void AddDate (DateTime val) {
            values.Add (new PlistElementDate (val));
        }

        public void AddReal (float val) {
            values.Add (new PlistElementReal (val));
        }

        public PlistElementArray AddArray () {
            var v = new PlistElementArray ();
            values.Add (v);
            return v;
        }

        public PlistElementDict AddDict () {
            var v = new PlistElementDict ();
            values.Add (v);
=======
    public class PlistElementArray : PlistElement
    {
        public PlistElementArray() : base() {}
        public List<PlistElement> values = new List<PlistElement>();

        // convenience methods
        public void AddString(string val)
        {
            values.Add(new PlistElementString(val));
        }

        public void AddInteger(int val)
        {
            values.Add(new PlistElementInteger(val));
        }

        public void AddBoolean(bool val)
        {
            values.Add(new PlistElementBoolean(val));
        }

        public void AddDate(DateTime val)
        {
            values.Add(new PlistElementDate(val));
        }

        public void AddReal(float val)
        {
            values.Add(new PlistElementReal(val));
        }

        public PlistElementArray AddArray()
        {
            var v = new PlistElementArray();
            values.Add(v);
            return v;
        }

        public PlistElementDict AddDict()
        {
            var v = new PlistElementDict();
            values.Add(v);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            return v;
        }
    }

<<<<<<< HEAD
    public class PlistDocument {
=======
    public class PlistDocument
    {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        public PlistElementDict root;
        public string version;

        private XDocumentType documentType;

<<<<<<< HEAD
        public PlistDocument () {
            root = new PlistElementDict ();
=======
        public PlistDocument()
        {
            root = new PlistElementDict();
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            version = "1.0";
        }

        // Parses a string that contains a XML file. No validation is done.
<<<<<<< HEAD
        [Obsolete]
        internal static XDocument ParseXmlNoDtd (string text) {
            XmlReaderSettings settings = new XmlReaderSettings ();
            settings.ProhibitDtd = false;
            settings.XmlResolver = null; // prevent DTD download

            XmlReader xmlReader = XmlReader.Create (new StringReader (text), settings);
            return XDocument.Load (xmlReader);
=======
        internal static XDocument ParseXmlNoDtd(string text)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            settings.XmlResolver = null; // prevent DTD download

            XmlReader xmlReader = XmlReader.Create(new StringReader(text), settings);
            return XDocument.Load(xmlReader);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        // LINQ serializes XML DTD declaration with an explicit empty 'internal subset'
        // (a pair of square brackets at the end of Doctype declaration).
        // Even though this is valid XML, XCode does not like it, hence this workaround.
<<<<<<< HEAD
        internal static string CleanDtdToString (XDocument doc, XDocumentType documentType) {
=======
        internal static string CleanDtdToString(XDocument doc, XDocumentType documentType)
        {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            // LINQ does not support changing the DTD of existing XDocument instances,
            // so we create a dummy document for printing of the Doctype declaration.
            // A single dummy element is added to force LINQ not to omit the declaration.
            // Also, utf-8 encoding is forced since this is the encoding we use when writing to file in UpdateInfoPlist.
<<<<<<< HEAD
            if (documentType != null) {
                XDocument tmpDoc =
                    new XDocument (new XDeclaration ("1.0", "utf-8", null),
                        new XDocumentType (documentType.Name, documentType.PublicId, documentType.SystemId, null),
                        new XElement (doc.Root.Name));
                return "" + tmpDoc.Declaration + "\n" + tmpDoc.DocumentType + "\n" + doc.Root + "\n";
            } else {
                XDocument tmpDoc = new XDocument (new XDeclaration ("1.0", "utf-8", null), new XElement (doc.Root.Name));
=======
            if (documentType != null)
            {
                XDocument tmpDoc =
                    new XDocument(new XDeclaration("1.0", "utf-8", null),
                                  new XDocumentType(documentType.Name, documentType.PublicId, documentType.SystemId, null),
                                  new XElement(doc.Root.Name));
                return "" + tmpDoc.Declaration + "\n" + tmpDoc.DocumentType + "\n" + doc.Root + "\n";
            }
            else
            {
                XDocument tmpDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(doc.Root.Name));
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                return "" + tmpDoc.Declaration + Environment.NewLine + doc.Root + "\n";
            }
        }

<<<<<<< HEAD
        internal static string CleanDtdToString (XDocument doc) {
            return CleanDtdToString (doc, doc.DocumentType);
        }

        private static string GetText (XElement xml) {
            return String.Join ("", xml.Nodes ().OfType<XText> ().Select (x => x.Value).ToArray ());
        }

        private static PlistElement ReadElement (XElement xml) {
            switch (xml.Name.LocalName) {
                case "dict":
                    {
                        List<XElement> children = xml.Elements ().ToList ();
                        var el = new PlistElementDict ();

                        if (children.Count % 2 == 1)
                            throw new Exception ("Malformed plist file");

                        for (int i = 0; i < children.Count - 1; i++) {
                            if (children[i].Name != "key")
                                throw new Exception ("Malformed plist file. Found '" + children[i].Name + "' where 'key' was expected.");
                            string key = GetText (children[i]).Trim ();
                            var newChild = ReadElement (children[i + 1]);
                            if (newChild != null) {
                                i++;
                                el[key] = newChild;
                            }
                        }
                        return el;
                    }
                case "array":
                    {
                        List<XElement> children = xml.Elements ().ToList ();
                        var el = new PlistElementArray ();

                        foreach (var childXml in children) {
                            var newChild = ReadElement (childXml);
                            if (newChild != null)
                                el.values.Add (newChild);
                        }
                        return el;
                    }
                case "string":
                    return new PlistElementString (GetText (xml));
                case "integer":
                    {
                        int r;
                        if (int.TryParse (GetText (xml), out r))
                            return new PlistElementInteger (r);
                        return null;
                    }
                case "real":
                    {
                        float f;
                        if (float.TryParse (GetText (xml), out f))
                            return new PlistElementReal (f);
                        return null;
                    }
                case "date":
                    {
                        DateTime date;
                        if (DateTime.TryParse (GetText (xml), out date))
                            return new PlistElementDate (date.ToUniversalTime ());
                        return null;
                    }
                case "true":
                    return new PlistElementBoolean (true);
                case "false":
                    return new PlistElementBoolean (false);
=======
        internal static string CleanDtdToString(XDocument doc)
        {
            return CleanDtdToString(doc, doc.DocumentType);
        }

        private static string GetText(XElement xml)
        {
            return String.Join("", xml.Nodes().OfType<XText>().Select(x => x.Value).ToArray());
        }

        private static PlistElement ReadElement(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "dict":
                {
                    List<XElement> children = xml.Elements().ToList();
                    var el = new PlistElementDict();

                    if (children.Count % 2 == 1)
                        throw new Exception("Malformed plist file");

                    for (int i = 0; i < children.Count - 1; i++)
                    {
                        if (children[i].Name != "key")
                            throw new Exception("Malformed plist file. Found '"+children[i].Name+"' where 'key' was expected.");
                        string key = GetText(children[i]).Trim();
                        var newChild = ReadElement(children[i+1]);
                        if (newChild != null)
                        {
                            i++;
                            el[key] = newChild;
                        }
                    }
                    return el;
                }
                case "array":
                {
                    List<XElement> children = xml.Elements().ToList();
                    var el = new PlistElementArray();

                    foreach (var childXml in children)
                    {
                        var newChild = ReadElement(childXml);
                        if (newChild != null)
                            el.values.Add(newChild);
                    }
                    return el;
                }
                case "string":
                    return new PlistElementString(GetText(xml));
                case "integer":
                {
                    int r;
                    if (int.TryParse(GetText(xml), out r))
                        return new PlistElementInteger(r);
                    return null;
                }
                case "real":
                {
                    float f;
                    if (float.TryParse(GetText(xml), out f))
                        return new PlistElementReal(f);
                    return null;
                }
                case "date":
                {
                    DateTime date;
                    if (DateTime.TryParse(GetText(xml), out date))
                        return new PlistElementDate(date.ToUniversalTime());
                    return null;
                }
                case "true":
                    return new PlistElementBoolean(true);
                case "false":
                    return new PlistElementBoolean(false);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                default:
                    return null;
            }
        }

<<<<<<< HEAD
        [Obsolete]
        public void Create () {
            const string doc = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" +
                "<plist version=\"1.0\">" +
                "<dict>" +
                "</dict>" +
                "</plist>";
            ReadFromString (doc);
        }

        [Obsolete]
        public void ReadFromFile (string path) {
            ReadFromString (File.ReadAllText (path));
        }

        [Obsolete]
        public void ReadFromStream (TextReader tr) {
            ReadFromString (tr.ReadToEnd ());
        }

        [Obsolete]
        public void ReadFromString (string text) {
            XDocument doc = ParseXmlNoDtd (text);
            version = (string) doc.Root.Attribute ("version");
            XElement xml = doc.XPathSelectElement ("plist/dict");

            var dict = ReadElement (xml);
            if (dict == null)
                throw new Exception ("Error parsing plist file");
            root = dict as PlistElementDict;
            if (root == null)
                throw new Exception ("Malformed plist file");
            documentType = doc.DocumentType;
        }

        private static XElement WriteElement (PlistElement el) {
            if (el is PlistElementBoolean) {
                var realEl = el as PlistElementBoolean;
                return new XElement (realEl.value ? "true" : "false");
            }
            if (el is PlistElementInteger) {
                var realEl = el as PlistElementInteger;
                return new XElement ("integer", realEl.value.ToString ());
            }
            if (el is PlistElementString) {
                var realEl = el as PlistElementString;
                return new XElement ("string", realEl.value);
            }
            if (el is PlistElementReal) {
                var realEl = el as PlistElementReal;
                return new XElement ("real", realEl.value.ToString ());
            }
            if (el is PlistElementDate) {
                var realEl = el as PlistElementDate;
                return new XElement ("date", realEl.value.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ssZ"));
            }
            if (el is PlistElementDict) {
                var realEl = el as PlistElementDict;
                var dictXml = new XElement ("dict");
                foreach (var kv in realEl.values) {
                    var keyXml = new XElement ("key", kv.Key);
                    var valueXml = WriteElement (kv.Value);
                    if (valueXml != null) {
                        dictXml.Add (keyXml);
                        dictXml.Add (valueXml);
=======
        public void Create()
        {
            const string doc = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                               "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" +
                               "<plist version=\"1.0\">" +
                               "<dict>" +
                               "</dict>" +
                               "</plist>";
            ReadFromString(doc);
        }

        public void ReadFromFile(string path)
        {
            ReadFromString(File.ReadAllText(path));
        }

        public void ReadFromStream(TextReader tr)
        {
            ReadFromString(tr.ReadToEnd());
        }

        public void ReadFromString(string text)
        {
            XDocument doc = ParseXmlNoDtd(text);
            version = (string) doc.Root.Attribute("version");
            XElement xml = doc.XPathSelectElement("plist/dict");

            var dict = ReadElement(xml);
            if (dict == null)
                throw new Exception("Error parsing plist file");
            root = dict as PlistElementDict;
            if (root == null)
                throw new Exception("Malformed plist file");
            documentType = doc.DocumentType;
        }

        private static XElement WriteElement(PlistElement el)
        {
            if (el is PlistElementBoolean)
            {
                var realEl = el as PlistElementBoolean;
                return new XElement(realEl.value ? "true" : "false");
            }
            if (el is PlistElementInteger)
            {
                var realEl = el as PlistElementInteger;
                return new XElement("integer", realEl.value.ToString());
            }
            if (el is PlistElementString)
            {
                var realEl = el as PlistElementString;
                return new XElement("string", realEl.value);
            }
            if (el is PlistElementReal)
            {
                var realEl = el as PlistElementReal;
                return new XElement("real", realEl.value.ToString());
            }
            if (el is PlistElementDate)
            {
                var realEl = el as PlistElementDate;
                return new XElement("date", realEl.value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            if (el is PlistElementDict)
            {
                var realEl = el as PlistElementDict;
                var dictXml = new XElement("dict");
                foreach (var kv in realEl.values)
                {
                    var keyXml = new XElement("key", kv.Key);
                    var valueXml = WriteElement(kv.Value);
                    if (valueXml != null)
                    {
                        dictXml.Add(keyXml);
                        dictXml.Add(valueXml);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    }
                }
                return dictXml;
            }
<<<<<<< HEAD
            if (el is PlistElementArray) {
                var realEl = el as PlistElementArray;
                var arrayXml = new XElement ("array");
                foreach (var v in realEl.values) {
                    var elXml = WriteElement (v);
                    if (elXml != null)
                        arrayXml.Add (elXml);
=======
            if (el is PlistElementArray)
            {
                var realEl = el as PlistElementArray;
                var arrayXml = new XElement("array");
                foreach (var v in realEl.values)
                {
                    var elXml = WriteElement(v);
                    if (elXml != null)
                        arrayXml.Add(elXml);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                }
                return arrayXml;
            }
            return null;
        }

<<<<<<< HEAD
        public void WriteToFile (string path) {
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding (false);
            File.WriteAllText (path, WriteToString (), utf8WithoutBom);
        }

        public void WriteToStream (TextWriter tw) {
            tw.Write (WriteToString ());
        }

        public string WriteToString () {
            var el = WriteElement (root);
            var rootEl = new XElement ("plist");
            rootEl.Add (new XAttribute ("version", version));
            rootEl.Add (el);

            var doc = new XDocument ();
            doc.Add (rootEl);
            return CleanDtdToString (doc, documentType).Replace ("\r\n", "\n");
        }
    }
}
=======
        public void WriteToFile(string path)
        {
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
            File.WriteAllText(path, WriteToString(), utf8WithoutBom);
        }

        public void WriteToStream(TextWriter tw)
        {
            tw.Write(WriteToString());
        }

        public string WriteToString()
        {
            var el = WriteElement(root);
            var rootEl = new XElement("plist");
            rootEl.Add(new XAttribute("version", version));
            rootEl.Add(el);

            var doc = new XDocument();
            doc.Add(rootEl);
            return CleanDtdToString(doc, documentType).Replace("\r\n", "\n");
        }
    }

} // namespace UnityEditor.iOS.XCode
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
