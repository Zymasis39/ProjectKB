using Microsoft.Xna.Framework.Input;
using ProjectKB.Modules;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectKB
{
    public class Config
    {
        public int displayWidth = 1280;
        public int displayHeight = 960;
        public Tuple<int, int>[] validDimensions = new Tuple<int, int>[] // TODO
        {
            new Tuple<int, int>(1280, 720),
            new Tuple<int, int>(1280, 960)
        };

        public bool fullscreen = false;
        public int fps = 60;
        public string playerName = "PLAYER";

        public Dictionary<KeyAction, Keys> keybinds = new()
        {
            { KeyAction.MoveLeft, Keys.Left },
            { KeyAction.MoveRight, Keys.Right },
            { KeyAction.MoveUp, Keys.Up },
            { KeyAction.MoveDown, Keys.Down },
            { KeyAction.PickRow1, Keys.D1 },
            { KeyAction.PickRow2, Keys.Q },
            { KeyAction.PickRow3, Keys.D2 },
            { KeyAction.PickRow4, Keys.W },
            { KeyAction.PickRow5, Keys.D3 },
            { KeyAction.PickColumn1, Keys.A },
            { KeyAction.PickColumn2, Keys.Z },
            { KeyAction.PickColumn3, Keys.S },
            { KeyAction.PickColumn4, Keys.X },
            { KeyAction.PickColumn5, Keys.D },
            { KeyAction.Pause, Keys.P },
            { KeyAction.MenuDown, Keys.Down },
            { KeyAction.MenuUp, Keys.Up },
            { KeyAction.MenuEnter, Keys.Enter },
            { KeyAction.Exit, Keys.Escape },
        };

        public string server = "DEFAULT";

        static Regex lineRegex = new(@"^(.+?)=(.+)$", RegexOptions.IgnoreCase);
        static Regex kvpRegex = new(@"^(.+?):(.+)$", RegexOptions.IgnoreCase);

        private Config() {
            
        }

        public static Config Load()
        {
            Config config = new Config();
            string fn = AppPaths.GetPath(".cfg");
            if (File.Exists(fn))
            {
                string[] lines = File.ReadAllLines(fn);
                foreach (string line in lines)
                {
                    Match match = lineRegex.Match(line);
                    if (match.Success)
                    {
                        try
                        {
                            switch (match.Groups[1].Value)
                            {
                                case "DISPW":
                                    config.displayWidth = int.Parse(match.Groups[2].Value);
                                    break;
                                case "DISPH":
                                    config.displayHeight = int.Parse(match.Groups[2].Value);
                                    break;
                                case "FULLS":
                                    config.fullscreen = match.Groups[2].Value == "1";
                                    break;
                                case "KEYS":
                                    string[] keys = match.Groups[2].Value.Split(";");
                                    foreach (string key in keys)
                                    {
                                        Match match2 = kvpRegex.Match(key);
                                        KeyAction k = (KeyAction)int.Parse(match2.Groups[1].Value);
                                        Keys v = (Keys)int.Parse(match2.Groups[2].Value);
                                        if (match2.Success)
                                        {
                                            if (config.keybinds.ContainsKey(k))
                                                config.keybinds[k] = v;
                                            else config.keybinds.Add(k, v);
                                        }
                                    }
                                    break;
                                case "FPS":
                                    config.fps = int.Parse(match.Groups[2].Value);
                                    break;
                                case "PNAME":
                                    config.playerName = match.Groups[2].Value;
                                    break;
                                case "SERVE":
                                    config.server = match.Groups[2].Value;
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            // log
                        }
                    }
                }
            }
            return config;
        }

        public bool ValidateKeyActionList(List<KeyAction> list)
        {
            Dictionary<Keys, KeyAction> kbc = new();
            foreach (KeyValuePair<KeyAction, Keys> kvp in keybinds)
            {
                if (kbc.ContainsKey(kvp.Value))
                {
                    return false;
                }
                else kbc.Add(kvp.Value, kvp.Key);
            }
            return true;
        }

        public void Save()
        {
            string fn = AppPaths.GetPath(".cfg");
            if (File.Exists(fn)) File.Delete(fn);
            FileStream fs = File.OpenWrite(fn);
            fs.Write(Encoding.ASCII.GetBytes("DISPW=" + displayWidth.ToString() + "\n"));
            fs.Write(Encoding.ASCII.GetBytes("DISPH=" + displayHeight.ToString() + "\n"));
            fs.Write(Encoding.ASCII.GetBytes("FULLS=" + (fullscreen ? "1" : "0") + "\n"));
            string keys = "";
            KeyValuePair<KeyAction, Keys>[] kvps = keybinds.ToArray();
            for (int i = 0; i < kvps.Length; i++)
            {
                keys += (int)kvps[i].Key + ":" + (int)kvps[i].Value;
                if (i < kvps.Length - 1) keys += ";";
            }
            fs.Write(Encoding.ASCII.GetBytes("KEYS=" + keys + "\n"));
            fs.Write(Encoding.ASCII.GetBytes("FPS=" + fps.ToString() + "\n"));
            fs.Write(Encoding.ASCII.GetBytes("PNAME=" + playerName + "\n"));
            fs.Write(Encoding.ASCII.GetBytes("SERVE=" + server + "\n"));
            fs.Close();
        }

        private class ConfigReadException : Exception
        {
            private ConfigReadException(string message) : base(message) { }
        }
    }
}
