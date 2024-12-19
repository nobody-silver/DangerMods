# DangerMods Plugin

An ExileCore2 plugin that alerts you to dangerous rare monsters with specific modifiers through both sound and visual alerts.

## Features

- Plays a customizable sound alert when encountering rare monsters with specific modifiers
- Displays dangerous monster names and their mods in a configurable screen position
- Configurable list of dangerous modifiers to watch for
- Adjustable alert volume
- Debug mode for troubleshooting
- Test button to preview the alert sound

## Configuration

### Settings

- **Enable**: Toggle the plugin on/off
- **Sound Alerts**: Enable/disable sound alerts
- **Volume**: Adjust the alert volume (0-100)
- **Modifiers to Alert**: List of monster modifiers to watch for (comma separated)
- **Alert Display**: Visual alert settings
  - **Show Alert Text**: Toggle the on-screen text display
  - **Display Position X**: Horizontal position of alerts (0-2000)
  - **Display Position Y**: Vertical position of alerts (0-2000)
  - **Font**: Font settings (e.g., "FrizQuadrataITC:16")
- **Debug Messages**: Toggle debug messages in the game overlay

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
   - Position the text display where you want it on screen
3. Modify the modifier list if needed
4. Use the Play Alert button to test your sound settings

The plugin will automatically alert you when encountering rare monsters with any of the specified modifiers, both with a sound alert and by displaying the monster's name and dangerous mods on screen.

## Tips

- Position the text display where it won't interfere with important game information
- Keep the volume at a noticeable but not overwhelming level
- Add modifiers that are particularly dangerous to your build
- Use debug messages to verify the plugin is detecting modifiers correctly