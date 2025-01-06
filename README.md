# I Am Your TAS mod for I Am Your Beast

This is a (somewhat flawed) TAS tool for I Am Your Beast

What the mod does:

- Allows you to customise the timescale
- Gives further time control options, including frame advance
- Allows you to save and load your position and rotation, and modify the saved values
- Allows you to view the current value of most of the variables in the game
- And some extras, including fullbright and making walls transparent.

What the mod does not do:

- Input recording and playback

Unfortunately the unity engine is far too unstable for input recording/playback to produce a reliable result- frequent desyncs and physics engine weirdness make it unreasonable to include for now.

(that doesn't mean never! just. not for a while)

## Usage

When in game, the menu will open when the game is paused. You can keep the menu open when you unpause the game with the F9 key. The config file at `BepInEx/config/kestrel.iamyourbeast.iamyourtas.cfg` provides a few optional keybinds that are not bound by default, which you might find useful.

Since the config file can be hard to read at times, I recommend using the mod [BepInEx Config Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) alongside this one to simplify the process and allow you to rebind keys while ingame.

### Installation

!!! You need [Bepinex 5](https://github.com/BepInEx/BepInEx/releases/latest) for mono !!!
(if you have no idea what the versions mean try BepInEx_win_x64_5.4.23.2 and it might work. maybe)

Once bepinex is installed, extract the contents of the folder into the BepInEx/plugins folder in the game's root directory.

have fun :3

![](https://files.catbox.moe/9zrygr.png)
