using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Inheritance;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallPaperRetain
{
    public class MainModClass : Mod {
        private List<int> wallPaper;
        private List<int> floors;
        private static int NO_CHANGE = -333000;

        public override void Entry(params object[] objects) {
            GameEvents.OneSecondTick += OneSecondTick;
            PlayerEvents.InventoryChanged += InventoryChanged;            
            Log.Verbose("Wallpaper Retainer by Iceburg333 => Initialized");
        }


        private void OneSecondTick(object sender, EventArgs e) {
            if (!Game1.hasLoadedGame) {
                return;
            }
            if (Game1.player.currentLocation == null || Game1.player.currentLocation.GetType() != typeof(FarmHouse)) {
                return;
            }
            FarmHouse farmHouse = Game1.player.currentLocation as FarmHouse;
            if (wallPaper == null || wallPaper.Count == 0) {
                if (farmHouse.wallPaper.Count != 0) {
                    Log.Verbose("Initializing wallpapers");
                    saveWallPapers(farmHouse);
                }
            }
            if (floors == null || floors.Count == 0) {
                if (farmHouse.floor.Count != 0) {
                    Log.Verbose("Initializing floors");
                    saveFloors(farmHouse);
                }
            }
        }

        private void InventoryChanged(object sender, EventArgs e) {
            if (!Game1.hasLoadedGame) {
                return;
            }
            if (Game1.player.currentLocation == null || Game1.player.currentLocation.GetType() != typeof(FarmHouse)) {
                return;
            }
            //if in a menu, do nothing
            if (Game1.activeClickableMenu != null) {
                return;
            }
            FarmHouse farmHouse = Game1.player.currentLocation as FarmHouse;
            EventArgsInventoryChanged eaic = (EventArgsInventoryChanged) e;
            List<ItemStackChange> changedItems = eaic.Removed;

            Wallpaper newPaper = GetRemovedWallpaper(changedItems);
            if (newPaper == null) {
                return;
            }
            int changed = 0;
            if (newPaper.isFloor) {
                changed = FloorsChangedFor(farmHouse);
                saveFloors(farmHouse);
            }
            else {
                changed = WallPapersChangedFor(farmHouse);
                saveWallPapers(farmHouse);
            }
            if (changed != NO_CHANGE) {
                getAndAddOldPaper(changed, newPaper.isFloor);
            } else {
                Game1.player.addItemToInventory(newPaper);
            }
        

    }



        private void saveWallPapers(FarmHouse farmHouse) {
            List<int> farmPaper = farmHouse.wallPaper;
            wallPaper = new List<int>();
            for (int i = 0; i < farmPaper.Count; i++) {
                wallPaper.Add(farmPaper[i]);
            }
            //Log.Verbose("Saved " + wallPaper.Count + " of " + farmPaper.Count + " wallpapers");
        }
        private void saveFloors(FarmHouse farmHouse) {
            List<int> farmFloor = farmHouse.floor;
            floors = new List<int>();
            for (int i = 0; i < farmFloor.Count; i++) {
                floors.Add(farmFloor[i]);
            }
            //Log.Verbose("Saved " + floors.Count + " of " + farmFloor.Count + " floors");
        }

        private int WallPapersChangedFor(FarmHouse farmHouse) {
            List<int> farmPaper = farmHouse.wallPaper;
            if(farmPaper == null || farmPaper.Count == 0){
                return 0;
            }
            if (wallPaper == null || wallPaper.Count == 0) {
                return 0;
            }
            for (int i=0; i < farmPaper.Count; i++) {
                if (!wallPaper[i].Equals(farmPaper[i])) {
                    return wallPaper[i];
                }
            }
            return NO_CHANGE;
        }
        private int FloorsChangedFor(FarmHouse farmHouse) {
            List<int> farmFloor = farmHouse.floor;
            if (farmFloor == null || farmFloor.Count == 0) {
                return 0;
            }
            if (floors == null || floors.Count == 0) {
                return 0;
            }
            for (int i = 0; i < farmFloor.Count; i++) {
                if (!floors[i].Equals(farmFloor[i])) {
                    return floors[i];
                }
            }
            return NO_CHANGE;
        }

        private Wallpaper GetRemovedWallpaper(List<ItemStackChange> changedItems) {
            foreach (ItemStackChange itemStack in changedItems) {
                //Log.Verbose(itemStack.Item.Name +" "+ itemStack.Item.parentSheetIndex + " of type " + itemStack.Item.GetType().Name + " was " + itemStack.ChangeType);
                if (itemStack.Item.GetType() == typeof(Wallpaper) && itemStack.ChangeType == ChangeType.Removed) {
                    return (Wallpaper) itemStack.Item;                    
                }
            }
            //Log.Verbose("No wallpaper changed in inventory");
            return null;
        }

        private void getAndAddOldPaper(int which, bool isFloor) {
            Wallpaper oldPaper = new Wallpaper(which, isFloor);
            Game1.player.addItemToInventory(oldPaper);
            string item = "wallpaper ";
            if (isFloor) {
                item = "floor ";
            }
            Log.Verbose("Added " + item + oldPaper.parentSheetIndex + " to player");
        }

    }
}
