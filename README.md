# Project KB

Project KB (codename, no official name yet) is a game based on the mechanic of merging tiles, like the well-known game *2048*, to which it adds elements of pace-based survival - the further you go, the faster you have to act to survive.

The game, while in a somewhat playable state, is still very much a WIP.

## Gameplay

- Use keyboard keys to select a column or row, and to move tiles in the selected column or row. The default keys are:
 - Arrows - move tiles in column/row
 - `1`/`Q`/`2`/`W`/`3` - select row, specified from top to bottom
 - `A`/`Z`/`S`/`X`/`D` - select column, specified from left to right

- There is a limited number (6) of tiers of tiles that can be merged.

- A Tier 1 tile spawns in a specific corner once that corner is empty. There are green indicators on corners that correspond to the next few spawns.

- Garbage tiles spawn on randomly chosen spaces, prioritizing empty spaces. A red \[!\] indicator appears on the space a garbage tile is about to spawn on. Garbage tiles can be moved, but not merged.

- To merge tiles, move a column or row so that a tile hits a wall, and another tile of the same tier hits it from behind. This clears garbage around the newly created tile, which itself is 1 tier higher than each of the merged tiles. When max-tier tiles are merged, the resulting tile is the same tier, but all garbage on the board is cleared.

- Progress towards higher levels is gained by merging tiles, with higher-tier merges giving more progress, and decays over time, proportionally to current total progress. Each higher level requires more progress from the previous level, and causes more garbage to spawn. Staying on the same level for too long (2 minutes and 30 seconds) further increases the garbage spawn rate. (Level 0 has no garbage before 2:30 so you can get the hang of it without worrying about garbage.)