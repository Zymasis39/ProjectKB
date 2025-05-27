# Project KB

Project KB (codename, no official name yet) is a game based on the mechanic of merging tiles, like the well-known game *2048*, to which it adds elements of pace-based survival - the further you go, the faster you have to act to survive.

The game, while in a somewhat playable state, is still very much a WIP.

## Gameplay

- Use keyboard keys to select a column or row, and to move tiles in the selected column or row. By default:
 - arrow keys move tiles in the selected column or row;
 - in **absolute** row/column selection mode, rows are selected with `1`/`Q`/`2`/`W`/`3` (top to bottom), and columns by `A`/`Z`/`S`/`X`/`D` (left to right).
 - in **relative** row/column selection mode, the current row/column selection can be moved with `W/S/A/D`.
 - **Relative** mode is easier to get used to, but has a lower speed potential, than **absolute** mode. *See the Configuration section for info on setting the row/column selection mode.*
 - Press `P` to pause, and `Esc` to exit while paused.

- There is a limited number (6) of tiers of tiles that can be merged.

- A Tier 1 tile spawns in a specific corner once that corner is empty. There are green indicators on corners that correspond to the next few spawns.

- Garbage tiles spawn on randomly chosen spaces, prioritizing empty spaces. A red \[!\] indicator appears on the space a garbage tile is about to spawn on. Garbage tiles can be moved, but not merged.

- To merge tiles, move a column or row so that a tile hits a wall, and another tile of the same tier hits it from behind. This clears garbage around the newly created tile, which itself is 1 tier higher than each of the merged tiles. When max-tier tiles are merged, the resulting tile is the same tier, but all garbage on the board is cleared.

- Progress towards higher levels is gained by merging tiles, with higher-tier merges giving more progress, and decays over time, proportionally to current total progress. Each higher level requires more progress from the previous level, and causes more garbage to spawn. Staying on the same level for too long (2 minutes and 30 seconds, or 10 minutes on MARATHON) further increases the garbage spawn rate. (Level 0 has no garbage before this effect kicks in so you can get the hang of it without worrying about garbage. Except on EXPERT, of course.)

## Game presets

- STANDARD - Recommended starting preset.

- MARATHON - Like STANDARD, but progression is 4x slower (higher level thresholds, slower progress decay, slower garbage amp).

- EXPERT - Significantly harder. Starts around the difficulty of STANDARD level 12, scales faster, and has 2x shorter garbage warning times.

## Configuration

Upon first launch, the game creates a default configuration file, which stores game settings/preferences, in the AppData folder. The ability to edit these settings in-game hasn't been implemented yet, but you can edit the file directly.

- `DISPW` and `DISPH` - window resolution in pixels (width and height respectively).
- `FULLS` - set `1` to enable fullscreen mode.
- `KEYS` - list of keybinds. *It is recommended not to edit this setting unless you really know what you're doing (the key actions and keys are enum values converted to ints).*
- `FPS` - frame rate in frames/second. *It is recommended to set this to your monitor's refresh rate.*
- `PNAME` - player name, under which new scores will be saved both locally and online. *This is restricted to uppercase letters, digits, dash `-` and underscore `_`. A name longer than 16 characters will cause online score submission to fail.*
- `SERVE` - score server address. Set `DEFAULT` to connect to the default server, or `NONE` to disable online features. If you have set up your own server, you can use its address as well.
- `CSM` - row/column selection mode; `1` for absolute, `2` for relative.
