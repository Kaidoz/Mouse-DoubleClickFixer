using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Mouse_DoubleClickFixer
{
    class Settings
    {
        public static bool _left = false;

        public static bool _right = false;

        public static bool _autorun = false;

        public static double valueTime = 100.0;

        private static string cfg_name = "mdcf_config.ini";

        public static void Init()
        {
            if (File.Exists(cfg_name))
            {
                string text = File.ReadAllText(cfg_name);
                if (text.Contains("right=True"))
                    _right = true;
                if (text.Contains("left=True"))
                    _left = true;
            }

            _autorun = CheckAutoRun();
        }

        public static void Save()
        {
            string cfg_text = "left=" + _left +
                              Environment.NewLine +
                              "right=" + _right;
            try
            {
                File.WriteAllText(cfg_name, cfg_text);
            }
            catch (Exception e)
            {
            }
            AutoRunSet(_autorun);
        }

        #region MyRegion

        public static bool CheckAutoRun()
        {
            Microsoft.Win32.RegistryKey Key =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", false);

            try
            {
                if (string.IsNullOrEmpty((string)Key.GetValue("Mouse-DoubleClickFixer")))
                    return false;

            }
            catch (Exception e)
            {
            }

            return true;
        }

        public static void AutoRunSet(bool enabled)
        {
            Microsoft.Win32.RegistryKey Key =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
            try
            {
                if (enabled)
                {
                    Key.SetValue("Mouse-DoubleClickFixer", System.Windows.Forms.Application.StartupPath);
                }
                else
                {
                    if (!string.IsNullOrEmpty((string)Key.GetValue("Mouse-DoubleClickFixer")))
                        Key.DeleteValue("Mouse-DoubleClickFixer");
                }
                Key.Close();
            }
            catch (Exception e)
            {

            }

        }

        #endregion
    }
}
