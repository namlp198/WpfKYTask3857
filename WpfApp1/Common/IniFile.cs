using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defaut, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        static extern uint GetPrivateProfileSectionNames(IntPtr pszReturnBuffer, uint nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);


        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public static void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, FilePath.FilePath_AOIGUISetup);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public static string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, FilePath.FilePath_AOIGUISetup);
            return temp.ToString();

        }


        /// <summary>
        /// Lay tat ca cac section trong ini file
        /// </summary>
        /// <returns></returns>
        private static string[] SectionNames()
        {
            //FilePath.FilePath_AOIGUISetup = Path.GetFullPath(FilePath.FilePath_AOIGUISetup);
            uint MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
            uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, FilePath.FilePath_AOIGUISetup);
            if (bytesReturned == 0)
                return null;
            string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);
            //use of Substring below removes terminating null for split
            return local.Substring(0, local.Length - 1).Split('\0');
        }
        public static string[] GetSectionNames()
        {
            return SectionNames();
        }

        /// <summary>
        /// Lay key-value theo section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetKeys(string section)
        {
            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(section, buffer, 2048, FilePath.FilePath_AOIGUISetup);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (String entry in tmp)
            {
                string key = entry.Substring(0, entry.IndexOf("="));
                string value = entry.Substring(entry.IndexOf("=") + 1, entry.Length - 1 - entry.IndexOf("="));
                result.Add(key, value);
            }
            
            return result;
        }
        public static Dictionary<string, string> GetKeysWithSection(string section)
        {
            return GetKeys(section);
        }

        /// <summary>
        /// Lay tat ca key-value theo mang section truyen vao
        /// </summary>
        /// <returns></returns>
        private static List<Dictionary<string, string>> GetAllKeys()
        {
            byte[] buffer = new byte[2048];
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            string[] temp = SectionNames(); // Lay tat ca cac section trong ini file
            try
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    GetPrivateProfileSection(temp[i], buffer, 2048, FilePath.FilePath_AOIGUISetup);
                    String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (String entry in tmp)
                    {
                        string key = entry.Substring(0, entry.IndexOf("="));
                        string value = entry.Substring(entry.IndexOf("=") + 1, entry.Length - 1 - entry.IndexOf("="));
                        dic.Add(key, value);
                    }
                    result.Add(dic);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return result;
        }
        public static List<Dictionary<string, string>> GetAllKeysWithAllSection()
        {
            return GetAllKeys();
        }
    }
}
