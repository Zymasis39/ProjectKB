using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Modules
{
    public static class KeyActionLists
    {
        public static List<KeyAction> Menu = new()
        {
            KeyAction.MenuDown,
            KeyAction.MenuUp,
            KeyAction.MenuEnter
        };

        public static List<KeyAction> GameplayAbsolute = new() // Absolute refers to column selection
        // as opposed to Relative (WASD) which will be introduced later
        {
            KeyAction.MoveLeft,
            KeyAction.MoveRight,
            KeyAction.MoveUp,
            KeyAction.MoveDown,
            KeyAction.PickColumn1,
            KeyAction.PickColumn2,
            KeyAction.PickColumn3,
            KeyAction.PickColumn4,
            KeyAction.PickColumn5,
            KeyAction.PickRow1,
            KeyAction.PickRow2,
            KeyAction.PickRow3,
            KeyAction.PickRow4,
            KeyAction.PickRow5,
            KeyAction.Pause
        };
    }
}
