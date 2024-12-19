using System.Windows.Forms;
using System.Collections.Generic;
using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;

namespace DangerMods;

public class DangerModsSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new(false);

    [Menu("Sound Alerts")]
    public ToggleNode EnableSoundAlerts { get; set; } = new ToggleNode(false);

    public RangeNode<int> Volume { get; set; } = new(50, 0, 100);
    
    [Menu("Modifiers to Alert", "Sound Alerts")]
    public TextNode ModifiersToAlert { get; set; } = new TextNode(
        "volatile, ondeath, speed, proximal, explo, beacon, barrier, storm, immune"
    );

    [Menu("Debug Messages")]
    public ToggleNode DebugMessages { get; set; } = new ToggleNode(false);

    public ButtonNode PlayAlert { get; set; } = new ButtonNode();
}
