using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mouse_DoubleClickFixer
{
    class Settings
    {
        public static bool _left = false;

        public static bool _right = false;

        private static string cfg_name = "mdcf_config.ini";

        public static void Init()
        {
            if (File.Exists(cfg_name))
            {
                string text = File.ReadAllText(cfg_name);
                if (text.Contains("left=true"))
                    _left = true;
                if (text.Contains("right=true"))
                    _right = true;
            }
        }

        public static void Save()
        {
            string cfg_text = "left=" + _left +
                              Environment.NewLine +
                              "right=" + _right;
            File.WriteAllText(cfg_name, cfg_text);
        }
    }
}
