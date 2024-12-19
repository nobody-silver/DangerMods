# DangerMods Plugin

An ExileCore2 plugin that plays an alert sound when dangerous rare monsters with specific modifiers are encountered.

## Features

- Plays a customizable sound alert when encountering rare monsters with specific modifiers
- Configurable list of dangerous modifiers to watch for
- Adjustable alert volume
- Debug mode for troubleshooting
- Test button to preview the alert sound

## Configuration

### Settings

- **Enable**: Toggle the plugin on/off
- **Sound Alerts**: Enable/disable sound alerts
- **Volume**: Adjust the alert volume (0-100)
- **Modifiers to Alert**: List of monster modifiers to watch for (comma or newline separated)
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
2. Enable Sound Alerts
3. Adjust the volume to your preference
4. Modify the modifier list if needed
5. Use the Play Alert button to test your volume settings

The plugin will automatically alert you when encountering rare monsters with any of the specified modifiers.

## Tips

- Keep the volume at a noticeable but not overwhelming level
- Add modifiers that are particularly dangerous to your build
- Use debug messages to verify the plugin is detecting modifiers correctly