using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Cache;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using System.Numerics;

namespace DangerMods {
    public class DangerMods : BaseSettingsPlugin<DangerModsSettings> {
        private HashSet<long> alertedEntities = new HashSet<long>();
        private const string ALERT_SOUND_FILE = "danger.wav";
        private List<string> cachedAlertModifiers;
        private string lastModifierSettings = string.Empty;
        private Dictionary<Entity, List<string>> currentAlerts = new Dictionary<Entity, List<string>>();

        private List<string> GetAlertModifiers() {
            var currentSettings = Settings.ModifiersToAlert.Value;
            if (cachedAlertModifiers == null || currentSettings != lastModifierSettings) {
                cachedAlertModifiers = ParseAlertModifiers();
                lastModifierSettings = currentSettings;
            }
            return cachedAlertModifiers;
        }

        private List<string> ParseAlertModifiers() {
            return Settings.ModifiersToAlert.Value
                .Split(',')
                .Select(x => x.Trim().ToLower())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        public override void Render() {
            if (!Settings.Enable || !Settings.ShowAlertText)
                return;

            // Update current alerts
            currentAlerts = currentAlerts
                .Where(kvp => kvp.Key.IsValid && kvp.Key.IsAlive)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (currentAlerts.Count == 0)
                return;

            // Draw alerts in the configured position
            var lineHeight = 25; // Fixed line height
            var position = new Vector2(Settings.AlertPositionX, Settings.AlertPositionY);
            
            foreach (var alert in currentAlerts) {
                var entity = alert.Key;
                var mods = alert.Value;
                
                // Format text: EntityName: Mod1, Mod2, etc
                var text = $"{entity.RenderName}: {string.Join(", ", mods)}";
                
                // Draw background box
                var textSize = Graphics.MeasureText(text);
                Graphics.DrawBox(
                    position,
                    position + textSize,
                    Color.FromArgb(180, 0, 0, 0)
                );
                
                // Draw text
                Graphics.DrawText(
                    text,
                    position,
                    Color.Red,
                    Settings.Font.Value,
                    FontAlign.Left
                );
                
                position.Y += lineHeight;
            }
        }

        public override void EntityAdded(Entity entity) {
            if (!Settings.Enable)
                return;

            try {
                if (!entity.IsAlive)
                    return;

                var rarity = entity.GetComponent<ObjectMagicProperties>();
                if (rarity == null || !rarity.Rarity.Equals(MonsterRarity.Rare))
                    return;

                var mods = rarity.Mods;
                if (mods == null)
                    return;

                var alertModifiers = GetAlertModifiers();
                var matchingMods = mods
                    .Where(mod => alertModifiers
                        .Any(alertMod => mod.Contains(alertMod, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (matchingMods.Any()) {
                    currentAlerts[entity] = matchingMods;
                    
                    if (Settings.EnableSoundAlerts && !alertedEntities.Contains(entity.Id)) {
                        PlayAlertSound(ALERT_SOUND_FILE);
                        alertedEntities.Add(entity.Id);
                    }
                }
            }
            catch (Exception e) {
                if (Settings.DebugMessages) {
                    DebugWindow.LogError(e.Message);
                }
            }
        }

        private void PlayAlertSound(string name) {
            try {
                GameController.SoundController.PreloadSound(name);
                GameController.SoundController.SetVolume(Settings.Volume.Value / 100f);
                GameController.SoundController.PlaySound(name);
            }
            catch (Exception ex) {
                DebugWindow.LogError($"Failed to play sound: {ex.Message}");
            }
        }

        public override bool Initialise() {
            Settings.PlayAlert.OnPressed += () => {
                PlayAlertSound(ALERT_SOUND_FILE);
            };

            GameController.Area.OnAreaChange += OnAreaChange;
            return true;
        }

        private void OnAreaChange(AreaInstance area) {
            alertedEntities.Clear();
            currentAlerts.Clear();
        }
    }
}