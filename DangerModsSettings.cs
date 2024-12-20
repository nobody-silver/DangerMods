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

    [Menu("Alert Display")]
    public ToggleNode ShowAlertText { get; set; } = new ToggleNode(true);

    [Menu("Display Position X", "Alert Display")]
    public RangeNode<float> AlertPositionX { get; set; } = new RangeNode<float>(10f, 0f, 2000f);

    [Menu("Display Position Y", "Alert Display")]
    public RangeNode<float> AlertPositionY { get; set; } = new RangeNode<float>(10f, 0f, 2000f);

    [Menu("Y Offset from Health Bar", "Alert Display")]
    public RangeNode<int> HealthBarYOffset { get; set; } = new RangeNode<int>(40, -100, 100);

    [Menu("Font", "Alert Display")]
    public TextNode Font { get; set; } = new TextNode("FrizQuadrataITC:22");

    [Menu("Debug Messages")]
    public ToggleNode DebugMessages { get; set; } = new ToggleNode(false);

    public ButtonNode PlayAlert { get; set; } = new ButtonNode();

    [Menu("Track Observed Modifiers")]
    public ToggleNode TrackModifiers { get; set; } = new ToggleNode(true);

    public TextNode ObservedModifiersFile { get; set; } = new TextNode("observed_modifiers.txt");

    [Menu("Anchor to Healthbar", "Alert Display")]
    public ToggleNode AnchorToHealthbar { get; set; } = new ToggleNode(false);
}
