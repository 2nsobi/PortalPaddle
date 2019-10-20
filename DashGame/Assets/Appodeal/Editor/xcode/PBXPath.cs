using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Unity.Appodeal.Xcode {
    internal class PBXPath {
        /// Replaces '\' with '/'. We need to apply this function to all paths that come from the user
        /// of the API because we store paths to pbxproj and on windows we may get path with '\' slashes
        /// instead of '/' slashes
        public static string FixSlashes (string path) {
            if (path == null)
                return null;
            return path.Replace ('\\', '/');
        }

        public static void Combine (string path1, PBXSourceTree tree1, string path2, PBXSourceTree tree2,
            out string resPath, out PBXSourceTree resTree) {
            if (tree2 == PBXSourceTree.Group) {
                resPath = Combine (path1, path2);
                resTree = tree1;
                return;
            }

            resPath = path2;
            resTree = tree2;
        }

        // Combines two paths
        public static string Combine (string path1, string path2) {
            if (path2.StartsWith ("/"))
                return path2;
            if (path1.EndsWith ("/"))
=======
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Unity.Appodeal.Xcode
{
    internal class PBXPath
    {
        /// Replaces '\' with '/'. We need to apply this function to all paths that come from the user
        /// of the API because we store paths to pbxproj and on windows we may get path with '\' slashes
        /// instead of '/' slashes
        public static string FixSlashes(string path)
        {
            if (path == null)
                return null;
            return path.Replace('\\', '/');
        }

        public static void Combine(string path1, PBXSourceTree tree1, string path2, PBXSourceTree tree2,
                                   out string resPath, out PBXSourceTree resTree)
        {
            if (tree2 == PBXSourceTree.Group)
            {
                resPath = Combine(path1, path2);
                resTree = tree1;
                return;
            }
            
            resPath = path2;
            resTree = tree2;
        }
        
        // Combines two paths
        public static string Combine(string path1, string path2)
        {
            if (path2.StartsWith("/"))
                return path2;
            if (path1.EndsWith("/"))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                return path1 + path2;
            if (path1 == "")
                return path2;
            if (path2 == "")
                return path1;
            return path1 + "/" + path2;
        }
<<<<<<< HEAD

        public static string GetDirectory (string path) {
            path = path.TrimEnd ('/');
            int pos = path.LastIndexOf ('/');
            if (pos == -1)
                return "";
            else
                return path.Substring (0, pos);
        }

        public static string GetCurrentDirectory () {
            if (Environment.OSVersion.Platform != PlatformID.MacOSX &&
                Environment.OSVersion.Platform != PlatformID.Unix) {
                throw new Exception ("PBX project compatible current directory can only obtained on OSX");
            }

            string path = Directory.GetCurrentDirectory ();
            path = FixSlashes (path);
            if (!IsPathRooted (path))
                return "/" + path;
            return path;
        }

        public static string GetFilename (string path) {
            int pos = path.LastIndexOf ('/');
            if (pos == -1)
                return path;
            else
                return path.Substring (pos + 1);
        }

        public static bool IsPathRooted (string path) {
=======
        
        public static string GetDirectory(string path)
        {
            path = path.TrimEnd('/');
            int pos = path.LastIndexOf('/');
            if (pos == -1)
                return "";
            else
                return path.Substring(0, pos);
        }

        public static string GetCurrentDirectory()
        {
            if (Environment.OSVersion.Platform != PlatformID.MacOSX &&
                Environment.OSVersion.Platform != PlatformID.Unix)
            {
                throw new Exception("PBX project compatible current directory can only obtained on OSX");
            }
                
            string path = Directory.GetCurrentDirectory();
            path = FixSlashes(path);
            if (!IsPathRooted(path))
                return "/" + path;
            return path;
        }
        
        public static string GetFilename(string path)
        {
            int pos = path.LastIndexOf('/');
            if (pos == -1)
                return path;
            else
                return path.Substring(pos + 1);
        }

        public static bool IsPathRooted(string path)
        {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            if (path == null || path.Length == 0)
                return false;
            return path[0] == '/';
        }
<<<<<<< HEAD

        public static string GetFullPath (string path) {
            if (IsPathRooted (path))
                return path;
            else
                return Combine (GetCurrentDirectory (), path);
        }

        public static string[] Split (string path) {
            if (string.IsNullOrEmpty (path))
                return new string[] { };
            return path.Split (new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
=======
        
        public static string GetFullPath(string path)
        {
            if (IsPathRooted(path))
                return path;
            else
                return Combine(GetCurrentDirectory(), path);
        }

        public static string[] Split(string path)
        {
            if (string.IsNullOrEmpty(path))
                return new string[]{};
            return path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
        }
    }

} // UnityEditor.iOS.Xcode
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
