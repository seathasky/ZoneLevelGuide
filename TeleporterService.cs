using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Game;
using System;
using System.Collections.Generic;
using Dalamud.Logging;

namespace ZoneLevelGuide
{
    public interface ITeleporterIpc
    {
        void Teleport(uint aetheryteId);
    }

    public partial class TeleporterService : ITeleporterIpc
    {
        private readonly IChatGui chatGui;
        private readonly IDalamudPluginInterface pluginInterface;
        private DateTime lastTeleport = DateTime.MinValue;
        private Dalamud.Plugin.Ipc.ICallGateSubscriber<uint, byte, bool>? teleportSubscriber;

        // I hated doing this, did all these fucking manually
        private readonly Dictionary<uint, string> aetheryteDisplayNames = new()
        {
            { 2, "New Gridania" },
            { 3, "Bentbranch Meadows" },
            { 4, "The Hawthorne Hut" },
            { 5, "Quarrymill" },
            { 6, "Camp Tranquil" },
            { 7, "Fallgourd Float" },
            { 8, "Limsa Lominsa Lower Decks" },
            { 9, "Ul'dah - Steps of Nald" },
            { 10, "Moraby Drydocks" },
            { 11, "Costa del Sol" },
            { 12, "Wineport" },
            { 13, "Swiftperch" },
            { 14, "Aleport" },
            { 15, "Camp Bronze Lake" },
            { 16, "Camp Overlook" },
            { 17, "Horizon" },
            { 18, "Camp Drybone" },
            { 19, "Little Ala Mhigo" },
            { 20, "Forgotten Springs" },
            { 21, "Camp Bluefog" },
            { 22, "Ceruleum Processing Plant" },
            { 23, "Camp Dragonhead" },
            { 24, "Revenant's Toll" },
            { 52, "Summerford Farms" },
            { 53, "Black Brush Station" },
            { 54, "Wolves' Den Pier" },
            { 61, "The Gold Saucer" },
            { 70, "Foundation" },
            { 71, "Falcon's Nest" },
            { 72, "Camp Cloudtop" },
            { 73, "Ok' Zundu" },
            { 74, "Helix" },
            { 75, "Idyllshire" },
            { 76, "Tailfeather" },
            { 77, "Anyx Trine" },
            { 78, "Moghome" },
            { 79, "Zenith" },
            { 96, "Estate Hall (Free Company)" },
            { 97, "Estate Hall (Private)" },
            { 98, "Castrum Oriens" },
            { 99, "The Peering Stones" },
            { 100, "Ala Gannha" },
            { 101, "Ala Ghiri" },
            { 102, "Porta Praetoria" },
            { 103, "The Ala Mhigan Quarter" },
            { 104, "Rhalgr's Reach" },
            { 105, "Tamamizu" },
            { 106, "Onokoro" },
            { 107, "Namai" },
            { 108, "The House of the Fierce" },
            { 109, "Reunion" },
            { 110, "The Dawn Throne" },
            { 111, "Kugane" },
            { 127, "The Doman Enclave" },
            { 128, "Dhoro Iloh" },
            { 133, "Fort Jobb" },
            { 134, "The Crystarium" },
            { 135, "Eulmore" },
            { 136, "The Ostall Imperative" },
            { 137, "Stilltide" },
            { 138, "Wright" },
            { 139, "Tomra" },
            { 140, "Mord Souq" },
            { 141, "Twine" },
            { 142, "Slitherbough" },
            { 143, "Fanow" },
            { 144, "Lydha Lran" },
            { 145, "Pla Enni" },
            { 146, "Wolekdorf" },
            { 147, "The Ondo Cups" },
            { 148, "The Macareneses Angle" },
            { 161, "The Inn at Journey's Head" },
            { 163, "Estate Hall (Free Company)" },
            { 164, "Estate Hall (Private)" },
            { 165, "The Archeion" },
            { 166, "Sharlayan Hamlet" },
            { 167, "Aporia" },
            { 168, "Yedlihmad" },
            { 169, "The Great Work" },
            { 170, "Palaka's Stand" },
            { 171, "Camp Broken Glass" },
            { 172, "Tertium" },
            { 173, "Sinus Lacrimarum" },
            { 174, "Bestways Burrow" },
            { 175, "Anagnorisis" },
            { 176, "The Twelve Wonders" },
            { 177, "Poieten Oikos" },
            { 178, "Reah Tahra" },
            { 179, "Abode of the Ea" },
            { 180, "Base Omicron" },
            { 181, "Old Sharlayan" },
            { 182, "Radz-at-Han" },
            { 200, "Wachunpelo" },
            { 201, "Worlar's Echo" },
            { 202, "Ok'hanu" },
            { 203, "Many Fires" },
            { 204, "Earthenshire" },
            { 205, "Iq Br'aax" },
            { 206, "Mamook" },
            { 207, "Hhusatahwi" },
            { 208, "Sheshenewezi Springs" },
            { 209, "Mehwahhetsoan" },
            { 210, "Yyasulani Station" },
            { 211, "The Outskirts" },
            { 212, "Electrope Strike" },
            { 213, "Leynode Mnemo" },
            { 214, "Leynode Pyro" },
            { 215, "Leynode Aero" },
            { 216, "Tuliyollal" },
            { 217, "Solution Nine" },
            { 238, "Dock Poga" }
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
    }
}