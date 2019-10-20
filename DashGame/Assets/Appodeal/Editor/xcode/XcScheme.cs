using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

<<<<<<< HEAD
namespace Unity.Appodeal.Xcode {
    internal class XcScheme {
        XDocument m_Doc;

        // Returns the current build configuration. Returns null if it is not set.
        public string GetBuildConfiguration () {
            var el = m_Doc.Root.XPathSelectElement ("./LaunchAction");
            if (el == null)
                throw new Exception ("The xcscheme document does not contain build configuration setting");
            var attr = el.Attribute ("buildConfiguration");
=======
namespace Unity.Appodeal.Xcode
{
    internal class XcScheme
    {
        XDocument m_Doc;

        // Returns the current build configuration. Returns null if it is not set.
        public string GetBuildConfiguration()
        {
            var el = m_Doc.Root.XPathSelectElement("./LaunchAction");
            if (el == null)
                throw new Exception("The xcscheme document does not contain build configuration setting");
            var attr = el.Attribute("buildConfiguration");
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            if (attr == null)
                return null;
            return attr.Value;
        }

<<<<<<< HEAD
        public void SetBuildConfiguration (string buildConfigName) {
            var el = m_Doc.Root.XPathSelectElement ("./LaunchAction");
            if (el == null)
                throw new Exception ("The xcscheme document does not contain build configuration setting");
            el.SetAttributeValue ("buildConfiguration", buildConfigName);
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
            m_Doc = PlistDocument.ParseXmlNoDtd (text);
        }

        public void WriteToFile (string path) {
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding (false);
            File.WriteAllText (path, WriteToString (), utf8WithoutBom);
        }

        public void WriteToStream (TextWriter tw) {
            tw.Write (WriteToString ());
        }

        public string WriteToString () {
            return PlistDocument.CleanDtdToString (m_Doc, null).Replace ("\r\n", "\n");
        }
    }
}
=======
        public void SetBuildConfiguration(string buildConfigName)
        {
            var el = m_Doc.Root.XPathSelectElement("./LaunchAction");
            if (el == null)
                throw new Exception("The xcscheme document does not contain build configuration setting");
            el.SetAttributeValue("buildConfiguration", buildConfigName);
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
            m_Doc = PlistDocument.ParseXmlNoDtd(text);
        }

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
            return PlistDocument.CleanDtdToString(m_Doc, null).Replace("\r\n", "\n");
        }        
    }

} // namespace UnityEditor.iOS.XCode
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
