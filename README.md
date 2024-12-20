# DangerMods Plugin

An ExileCore2 plugin that alerts you to dangerous rare monsters with specific modifiers through both sound and visual alerts.

## Features

- Plays a customizable sound alert when encountering rare monsters with specific modifiers
- Two display modes that can work simultaneously:
  - Fixed position display (always shown in configured screen position)
  - Anchored display that follows monsters in-game
- Configurable list of dangerous modifiers to watch for
- Adjustable alert volume
- Debug mode for troubleshooting
- Test button to preview the alert sound
- Automatic tracking of observed modifiers

## Configuration

### Settings

- **Enable**: Toggle the plugin on/off
- **Sound Alerts**: Enable/disable sound alerts
- **Volume**: Adjust the alert volume (0-100)
- **Modifiers to Alert**: List of monster modifiers to watch for (comma separated)
- **Alert Display**: Visual alert settings
  - **Show Alert Text**: Toggle the on-screen text display
  - **Display Position X**: Horizontal position of fixed alerts (0-2000)
  - **Display Position Y**: Vertical position of fixed alerts (0-2000)
  - **Anchor to Monster**: Enable/disable alerts that follow monsters
  - **Health Bar Y Offset**: Adjust vertical offset for anchored alerts (-100 to 100)
  - **Font**: Font settings (e.g., "FrizQuadrataITC:16")
- **Debug Messages**: Toggle debug messages in the game overlay
- **Track Modifiers**: Save newly encountered modifiers to a file

### Default Dangerous Modifiers

The plugin comes preconfigured with these commonly dangerous modifiers:

- volatile
- ondeath
- speed
- proximal
- explo
- beacon
- barrier
- storm
- immune

You can customize this list in the settings to match your preferences.

## Usage

1. Enable the plugin in ExileCore
2. Configure both sound and visual alerts:
   - Enable Sound Alerts and adjust volume if desired
   - Position the fixed text display where you want it on screen
   - Enable "Anchor to Monster" if you want alerts to follow monsters
   - Adjust the Y Offset for anchored alerts if needed
3. Modify the modifier list if needed
4. Use the Play Alert button to test your sound settings

The plugin will display alerts in this format:

```
Monster Name
    Dangerous Modifier 1
    Dangerous Modifier 2
    etc... 
```

## Tips

- Position the fixed text display where it won't interfere with important game information
- Keep the volume at a noticeable but not overwhelming level
- Add modifiers that are particularly dangerous to your build
- Use debug messages to verify the plugin is detecting modifiers correctly
- The observed_modifiers.txt file can help you discover new modifiers to watch for