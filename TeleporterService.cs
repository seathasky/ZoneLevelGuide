using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Game;
using System;
using System.Collections.Generic;

namespace ZoneLevelGuide
{
    public interface ITeleporterIpc
    {
        void Teleport(uint aetheryteId);
    }

    public class TeleporterService : ITeleporterIpc
    {
        private readonly IChatGui chatGui;
        private readonly IDalamudPluginInterface pluginInterface;
        private DateTime lastTeleport = DateTime.MinValue;
        private Dalamud.Plugin.Ipc.ICallGateSubscriber<uint, byte, bool>? teleportSubscriber;

        // Display names with aetheryte IDs annoyingly debugged
        private readonly Dictionary<uint, string> aetheryteDisplayNames = new()
        {
            // === A Realm Reborn (ARR) Aetherytes - IDs 2-24 ===
            { 2, "New Gridania" },
            { 3, "Bentbranch Meadows" },  // Fixed: use actual ID 3
            { 4, "Hawthorne Hut" },       // Fixed: use actual ID 4
            { 5, "Quarrymill" },          // Fixed: use actual ID 5
            { 6, "Camp Tranquil" },       // Fixed: use actual ID 6
            { 7, "Fallgourd Float" },     // Fixed: use actual ID 7
            { 8, "Limsa Lominsa Lower Decks" },
            { 9, "Ul'dah - Steps of Nald" },
            { 10, "Moraby Drydocks" },
            { 11, "Costa del Sol" },      // Fixed: use actual ID 11
            { 12, "Wineport" },           // Fixed: use actual ID 12 for Wineport
            { 13, "Swiftperch" },         // Fixed: use actual ID 13 for Swiftperch
            { 14, "Aleport" },            // Fixed: use actual ID 14 for Aleport
            { 15, "Camp Bronze Lake" },   // Fixed: use actual ID 15 for Camp Bronze Lake
            { 16, "Camp Overlook" },      // Fixed: use actual ID 16 for Camp Overlook
            { 17, "Horizon" },            // Fixed: use actual ID 17 for Horizon
            { 18, "Camp Drybone" },       // Fixed: use actual ID 18 for Camp Drybone
            { 19, "Little Ala Mhigo" },   // Fixed: use actual ID 19 for Little Ala Mhigo
            { 20, "Forgotten Springs" },  // Fixed: use actual ID 20 for Forgotten Springs
            { 21, "Camp Bluefog" },       // Fixed: use actual ID 21 for Camp Bluefog
            { 22, "Ceruleum Processing Plant" }, // Fixed: use actual ID 22 for Ceruleum Processing Plant
            { 23, "Camp Dragonhead" },    // Fixed: use actual ID 23 for Camp Dragonhead
            { 24, "Revenant's Toll" },    // Fixed: use actual ID 24 for Revenant's Toll
            
            // === Heavensward Aetherytes - IDs 70-76 ===
            { 70, "Foundation" },
            { 71, "Moghome" },
            { 72, "Camp Cloudtop" },
            { 73, "Idyllshire" },
            { 74, "Falcon's Nest" },
            { 75, "Tailfeather" },
            { 76, "Helix" },
            
            // === Stormblood Aetherytes - IDs 109-115 ===
            { 109, "Rhalgr's Reach" },
            { 110, "Ala Gannha" },
            { 111, "Kugane" },
            { 112, "Tamamizu" },
            { 113, "The Ala Mhigan Quarter" },
            { 114, "Namai" },
            { 115, "Reunion" },
            
            // === Shadowbringers Aetherytes - IDs 131-138 ===
            { 131, "The Crystarium" },
            { 132, "Fort Jobb" },
            { 133, "Stilltide" },
            { 134, "Eulmore" },
            { 135, "Lydha Lran" },
            { 136, "Slitherbough" },
            { 137, "Mord Souq" },
            { 138, "The Ondo Cups" },
            
            // === Endwalker Aetherytes - IDs 170-177 ===
            { 170, "Old Sharlayan" },
            { 172, "The Archeion" },
            { 173, "Yedlihmad" },
            { 174, "Tertium" },
            { 175, "Bestways Burrow" },
            { 176, "Anagnorisis" },
            { 177, "Base Omicron" },
            
            // === Dawntrail Aetherytes - IDs 180-187 ===
            { 180, "Tuliyollal" },
            { 181, "Wachunpelo" },
            { 182, "Ok'hanu" },
            { 183, "Iq Br'aax" },
            { 184, "Hhusatahwi" },
            { 185, "Solution Nine" },
            { 186, "Electrope Strike" },
            { 187, "Leynode Mnemo" }
        };

        public TeleporterService(IDalamudPluginInterface pluginInterface, IChatGui chatGui, ICommandManager commandManager, ISigScanner sigScanner, IGameInteropProvider gameInterop)
        {
            this.chatGui = chatGui;
            this.pluginInterface = pluginInterface;
            
            // Subscribe to TeleporterPlugin IPC
            try
            {
                this.teleportSubscriber = pluginInterface.GetIpcSubscriber<uint, byte, bool>("Teleport");
            }
            catch (Exception ex)
            {
                chatGui?.Print($"Failed to subscribe to TeleporterPlugin: {ex.Message}");
            }
        }

        public void Teleport(uint aetheryteId)
        {
            try
            {
                if (aetheryteDisplayNames.TryGetValue(aetheryteId, out var displayName))
                {
                    // Rate limiting
                    if ((DateTime.Now - lastTeleport).TotalSeconds < 3)
                    {
                        chatGui?.PrintError("Wait a moment before teleporting again.");
                        return;
                    }

                    // Use the aetheryte ID directly - no more mapping needed!
                    if (TryIpcTeleport(aetheryteId, displayName))
                    {
                        lastTeleport = DateTime.Now;
                    }
                    else
                    {
                        chatGui?.PrintError("Requires Teleporter Plugin for this feature to work");
                    }
                }
                else
                {
                    chatGui?.PrintError("Requires Teleporter Plugin for this feature to work");
                }
            }
            catch (Exception)
            {
                chatGui?.PrintError("Requires Teleporter Plugin for this feature to work");
            }
        }

        private bool TryIpcTeleport(uint aetheryteId, string displayName)
        {
            try
            {
                if (teleportSubscriber != null)
                {
                    // Use the correct aetheryte ID
                    var result = teleportSubscriber.InvokeFunc(aetheryteId, 0);
                    
                    if (result)
                    {
                        chatGui?.Print($"Teleporting to {displayName} (ID: {aetheryteId})");
                    }
                    else
                    {
                        chatGui?.Print($"Teleport failed for {displayName} (ID: {aetheryteId})");
                    }
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                chatGui?.Print($"Error teleporting to {displayName}: {ex.Message}");
                return false;
            }
        }

        public void TestTeleport()
        {
            // Commented out for production
            /*
            // Test teleport to Limsa Lominsa Lower Decks (ID: 8)
            chatGui?.Print("Testing teleport to Limsa Lominsa Lower Decks...");
            Teleport(8);
            */
        }

        // Commented out debug methods for production
        /*
        // Add this method to help discover correct aetheryte IDs
        public void DiscoverAetheryteIds()
        {
            chatGui?.Print("=== Aetheryte ID Discovery ===");
            chatGui?.Print("This will test one ID at a time. Use '/zg' to open window and test next ID.");
            chatGui?.Print("Testing ID 1...");
            
            // Test just one ID at a time to avoid crashes
            TestSingleAetheryteId(1);
        }

        public uint currentTestId = 1;
        
        public void TestSingleAetheryteId(uint id)
        {
            try
            {
                if (teleportSubscriber != null)
                {
                    chatGui?.Print($"Testing ID {id}...");
                    var result = teleportSubscriber.InvokeFunc(id, 0);
                    if (result)
                    {
                        chatGui?.Print($"SUCCESS: ID {id} worked! Check where you teleported.");
                        chatGui?.Print($"Please note down: ID {id} = [Location Name]");
                    }
                    else
                    {
                        chatGui?.Print($"ID {id} failed - no teleport occurred.");
                    }
                }
            }
            catch (Exception ex)
            {
                chatGui?.Print($"ID {id} failed: {ex.Message}");
            }
            
            currentTestId = id + 1;
            if (currentTestId <= 30)
            {
                chatGui?.Print($"Next test will be ID {currentTestId}. Click discovery button again to continue.");
            }
            else
            {
                chatGui?.Print("=== Discovery Complete ===");
                chatGui?.Print("Please share your findings so we can update the mappings!");
                currentTestId = 1; // Reset for next time
            }
        }

        public void ResetDiscovery()
        {
            currentTestId = 1;
            chatGui?.Print("Discovery reset. Starting from ID 1 again.");
        }
        */
    }
}