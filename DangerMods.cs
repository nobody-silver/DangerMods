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
using System.IO;

namespace DangerMods {
    public class DangerMods : BaseSettingsPlugin<DangerModsSettings> {
        private HashSet<long> alertedEntities = new HashSet<long>();
        private const string ALERT_SOUND_FILE = "danger.wav";
        private List<string> cachedAlertModifiers;
        private string lastModifierSettings = string.Empty;
        private Dictionary<Entity, List<string>> currentAlerts = new Dictionary<Entity, List<string>>();
        private readonly HashSet<string> observedModifiers = new HashSet<string>();
        private string ObservedModifiersPath => Path.Combine(DirectoryFullName, Settings.ObservedModifiersFile.Value);

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

            // Always draw the fixed position alerts
            DrawFixedPositionAlerts();

            // Additionally draw healthbar-anchored alerts if enabled
            if (Settings.AnchorToHealthbar) {
                DrawHealthbarAnchoredAlerts();
            }
        }

        private void DrawFixedPositionAlerts() {
            var lineHeight = 25;
            var position = new Vector2(Settings.AlertPositionX, Settings.AlertPositionY);
            
            foreach (var alert in currentAlerts) {
                // Format text with entity name and indented modifiers
                var text = $"{alert.Key.RenderName}\n    {string.Join("\n    ", alert.Value)}";
                var textSize = Graphics.MeasureText(text);
                
                Graphics.DrawBox(
                    position,
                    position + textSize,
                    Color.FromArgb(180, 0, 0, 0)
                );
                
                Graphics.DrawText(
                    text,
                    position,
                    Color.Red,
                    Settings.Font.Value,
                    FontAlign.Left
                );
                
                position.Y += textSize.Y + lineHeight;
            }
        }

        private void DrawHealthbarAnchoredAlerts() {
            foreach (var alert in currentAlerts.ToList()) {  // ToList to avoid modification during enumeration
                var entity = alert.Key;
                
                // Skip if entity is dead
                if (!entity.IsAlive) {
                    currentAlerts.Remove(entity);
                    alertedEntities.Remove(entity.Id);
                    continue;
                }
                
                // Get world coordinates
                var worldCoords = entity.Pos;
                
                // Match HealthBars.cs Z-axis logic
                if (entity.GetComponent<Render>()?.Bounds is { } boundsNum)
                {
                    worldCoords.Z -= 2 * boundsNum.Z;
                }
                worldCoords.Z += Settings.HealthBarYOffset;
                
                // Convert to screen coordinates
                var screenCoords = GameController.IngameState.Camera.WorldToScreen(worldCoords);
                if (screenCoords == Vector2.Zero) continue;

                // Format text with entity name and indented modifiers
                var text = $"{entity.RenderName}\n    {string.Join("\n    ", alert.Value)}";
                var textSize = Graphics.MeasureText(text);
                
                var position = new Vector2(
                    screenCoords.X - (textSize.X / 2),
                    screenCoords.Y
                );

                // Draw background
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

                // Track new modifiers if enabled
                if (Settings.TrackModifiers) {
                    bool newModifiersFound = false;
                    foreach (var mod in mods) {
                        var normalizedMod = mod.ToLower().Trim();
                        if (observedModifiers.Add(normalizedMod)) {
                            newModifiersFound = true;
                            if (Settings.DebugMessages) {
                                LogMessage($"New modifier found: {normalizedMod}");
                            }
                        }
                    }
                    
                    if (newModifiersFound) {
                        SaveObservedModifiers();
                    }
                }

                // Existing alert logic
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
            LoadObservedModifiers();
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

        private void LoadObservedModifiers() {
            try {
                if (File.Exists(ObservedModifiersPath)) {
                    var lines = File.ReadAllLines(ObservedModifiersPath);
                    foreach (var line in lines) {
                        if (!string.IsNullOrWhiteSpace(line)) {
                            observedModifiers.Add(line.Trim().ToLower());
                        }
                    }
                }
            }
            catch (Exception e) {
                LogError($"Failed to load observed modifiers: {e.Message}");
            }
        }

        private void SaveObservedModifiers() {
            try {
                File.WriteAllLines(ObservedModifiersPath, 
                    observedModifiers.OrderBy(x => x).ToList());
            }
            catch (Exception e) {
                LogError($"Failed to save observed modifiers: {e.Message}");
            }
        }

        public override void EntityRemoved(Entity entity)
        {
            currentAlerts.Remove(entity);
            alertedEntities.Remove(entity.Id);
        }
    }
}