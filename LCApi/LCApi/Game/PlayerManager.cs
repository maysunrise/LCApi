using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace LCApi.Game
{
    public class PlayerManager
    {
        public Player Player { get; private set; }
        public FPSController FPSController { get; private set; }

        public InventorySystem Inventory { get; private set; }

        public int Health
        {
            get
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "health");
                return (int)field.GetValue(Player);
            }
            set
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "health");
                field.SetValue(Player, value);
            }
        }

        public bool IsCreative
        {
            get
            {
                return MoonData.isCreative;
            }
        }

        public int SuitStatus
        {
            get
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "suitStatus");
                return (int)field.GetValue(Player);
            }
            set
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "suitStatus");
                field.SetValue(Player, value);
            }
        }

        public float JetpackFuel
        {
            get
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "jetpackFuel");
                return (float)field.GetValue(Player);
            }
            set
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "jetpackFuel");
                field.SetValue(Player, value);
            }
        }

        public MoonData MoonData
        {
            get
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "moonData");
                return (MoonData)field.GetValue(Player);
            }
            set
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "moonData");
                field.SetValue(Player, value);
            }
        }

        public bool CanUseItems
        {
            get
            {
                FieldInfo field1 = AccessTools.Field(typeof(Player), "pauseMenu");
                PauseMenu pauseMenu = (PauseMenu)field1.GetValue(Player);
                FieldInfo field2 = AccessTools.Field(typeof(Player), "inventoryUI");
                GameObject invUI = (GameObject)field2.GetValue(Player);
                return !pauseMenu.IsPaused() && !invUI.activeSelf;
            }
        }

        public bool ShowedInventory
        {
            get
            {
                FieldInfo field = AccessTools.Field(typeof(Player), "inventoryUI");
                GameObject invUI = (GameObject)field.GetValue(Player);
                return invUI.activeSelf;
            }
        }

        private static PlayerManager _instance;
        internal Dropped lastDropped;
        public static PlayerManager GetInstance()
        {
            return _instance;
        }

        public PlayerManager()
        {
            _instance = this;
            GameObject player = GameObject.Find("Player");
            Player = player.GetComponent<Player>();
            FPSController = player.GetComponent<FPSController>();

            FieldInfo invField = AccessTools.Field(typeof(Player), "inventorySystem");
            Inventory = invField.GetValue(Player) as InventorySystem;

            GameEvents.OnDroppedItem += (sender, args) => lastDropped = (Dropped)sender;
        }

        public void UpdateInventoryUI()
        {
            MethodInfo method = typeof(Player).GetMethod("UpdateInventoryUI", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(Player, null);
        }

        public ItemID GetHeldItemId()
        {
            FieldInfo field = AccessTools.Field(typeof(Player), "heldItemID");
            return (ItemID)field.GetValue(Player);
        }

        public int GetHeldItemCount()
        {
            FieldInfo field = AccessTools.Field(typeof(Player), "heldItemAmount");
            return (int)field.GetValue(Player);
        }

        public Dropped ThrowStack(int amount)
        {
            if (amount <= 0)
            {
                return null;
            }

            InventorySlot inventorySlot = Inventory.inventory.slots[0][Inventory.selectedHotbarSlot - 1];

            int possibleAmount = Mathf.Clamp(amount, 0, inventorySlot.amount);

            if (inventorySlot.amount > 0)
            {
                MethodInfo method = typeof(Player).GetMethod("ThrowStack", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(Player, new object[] { inventorySlot.itemID, possibleAmount });
            }
            else
            {
                return null;
            }

            inventorySlot.amount = Mathf.Clamp(inventorySlot.amount - possibleAmount, 0, 999);
            if (inventorySlot.amount <= 0)
            {
                inventorySlot.itemID = ItemID.none;
            }
            UpdateInventoryUI();
            return lastDropped;
        }
    }
}
