using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Modules
{
    public class KeyboardManager
    {
        public Dictionary<Keys, KeyAction> keybinds = new();

        private Queue<KeyAction> actionQueue = new();

        public KeyboardManager()
        {
        
        }

        public void LoadKeyActionList(List<KeyAction> list)
        {
            keybinds.Clear();
            foreach (KeyAction ka in list)
            {
                Keys key = KBModules.Config.keybinds[ka];
                if (keybinds.ContainsKey(key))
                {
                    throw new Exception("One key cannot be used for multiple actions");
                }
                else keybinds.Add(key, ka);
            }
        }

        public Queue<KeyAction> PassQueue()
        {
            Queue<KeyAction> oldQueue = actionQueue;
            actionQueue = new();
            return oldQueue;
        }

        public void OnKeyDown(object sender, InputKeyEventArgs args) { // TODO change way of handling
            Keys key = args.Key;
            if (keybinds.TryGetValue(key, out KeyAction action))
            {
                actionQueue.Enqueue(action);
            }
        }
    }
}
