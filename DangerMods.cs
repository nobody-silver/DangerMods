using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Cache;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;

namespace DangerMods {
    public class DangerMods : BaseSettingsPlugin<DangerModsSettings> {
        private HashSet<long> alertedEntities = new HashSet<long>();
        private const string ALERT_SOUND_FILE = "danger.wav";
        private List<string> cachedAlertModifiers;
        private string lastModifierSettings = string.Empty;

        private List<string> GetAlertModifiers() {
            var currentSettings = Settings.ModifiersToAlert.Value;
            if (cachedAlertModifiers == null || currentSettings != lastModifierSettings) {
                cachedAlertModifiers = ParseAlertModifiers();
                lastModifierSettings = currentSettings;
            }
            return cachedAlertModifiers;
        }

        private void PlayAlertSound(string name) {
            try {
                GameController.SoundController.SetVolume(Settings.Volume.Value / 100f);
                GameController.SoundController.PlaySound(name);
            } catch (Exception ex) {
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
            if (Settings.DebugMessages) {
                DebugWindow.LogMsg("Cleared alert cache for new area");
            }
        }

        public override void OnUnload() {
            if (GameController?.Area != null) {
                GameController.Area.OnAreaChange -= OnAreaChange;
            }
        }

        public override void OnLoad() {
            GameController.SoundController.PreloadSound(ALERT_SOUND_FILE);
        }

        public override void Tick() {
            foreach (var validEntity in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster]) {
                try {
                    if (validEntity.IsAlive) {
                        CheckForModifierAlert(validEntity);
                    }
                } catch (Exception e) {
                    DebugWindow.LogError(e.Message);
                }
            }
        }

        private List<string> ParseAlertModifiers() {
            return Settings.ModifiersToAlert.Value
                .Split(new[] { '\n', '\r', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        private void CheckForModifierAlert(Entity entity) {
            if (!Settings.EnableSoundAlerts || alertedEntities.Contains(entity.Address)) {
                return;
            }

            var rarity = entity.GetComponent<ObjectMagicProperties>();
            if (rarity == null || !rarity.Rarity.Equals(MonsterRarity.Rare)) {
                return;
            }

            var mods = rarity.Mods;
            if (mods == null) {
                return;
            }

            var alertModifiers = GetAlertModifiers();
            if (Settings.DebugMessages) {
                DebugWindow.LogMsg("Alert Modifiers: " + string.Join(", ", alertModifiers));
            }
        
            var allMatchingMods = mods
                .Where(mod => alertModifiers
                    .Any(alertMod => mod.Contains(alertMod, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (allMatchingMods.Any()) {
                if (Settings.DebugMessages) {
                    DebugWindow.LogMsg($"Found matching modifiers: {string.Join(", ", allMatchingMods)}");
                }
                PlayAlertSound(ALERT_SOUND_FILE);
                alertedEntities.Add(entity.Address);
            }
        }
    }
}