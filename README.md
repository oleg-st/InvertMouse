# InvertMouse
Adds the ability to invert the mouse Y-axis in games that lack this feature, such as Genshin Impact

# Usage

1. Install [Raw Accel v1.6.0](https://github.com/a1xd/rawaccel) or [Interception](https://github.com/oblitum/Interception) driver. Reboot.
2. Run [InvertMouse](https://github.com/oleg-st/InvertMouse/releases/latest)

# Requirements
[Raw Accel v1.6.0](https://github.com/a1xd/rawaccel) or [Interception](https://github.com/oblitum/Interception) driver installed. .NET Framework 4.7.2+

# How it works

InvertMouse uses kernel mode driver and library to invert mouse Y-axis when cursor is hidden. 
This allows interactions in the menu (when cursor is visible) without inverted Y-axis.

# Driver comparison
Raw Accel is [Anti-Cheat Friendly](https://github.com/a1xd/rawaccel#anti-cheat-friendly). Raw Accel has a one second delay when changing settings.  
Interception has no delay but can be detected by Vanguard and FaceIt.  
[Read more](https://www.kovaak.com/mouse-acceleration/)

# InvertMouse Driver

I wrote a separate driver for mouse inversion. It is anti-cheat friendly and has low delay when toggling inversion.  
Looking for someone who can help with getting it signed.

- **Driver source code:** [https://github.com/oleg-st/InvertMouse/tree/dev/src/InvertMouseDriver](https://github.com/oleg-st/InvertMouse/tree/dev/src/InvertMouseDriver)
- **Binaries for signing, built with GitHub Actions:** [https://github.com/oleg-st/InvertMouse/actions](https://github.com/oleg-st/InvertMouse/actions)
