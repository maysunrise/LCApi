using HarmonyLib;
using LCApi.LCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LCApi.Items
{
    /// <summary>
    /// List of all registered items, assembler recipes and alchemy recipes in the game
    /// </summary>
    public class ItemRegistry
    {
        public List<ItemDef> ItemDefs = new List<ItemDef>();

        private Type _craftingRecipes;
        private static ItemRegistry _instance;
        private static int _nextItemId;
        private static int _nextBlockId;
        public static ItemRegistry GetInstance()
        {
            return _instance;
        }

        public ItemRegistry()
        {
            if (_instance != null)
            {
                //Plugin.LogError("TRIED TO CREATE AN INSTANCE OF ItemRegistry AGAIN!!!");
                return;
            }
            _instance = this;

            _craftingRecipes = AccessTools.TypeByName("CraftingRecipes");

            _nextItemId = (int)(ItemID.astronaut_egg) + 1;
            _nextBlockId = (int)(BlockID.minilight_ny) + 1;
        }

        /// <summary>
        /// Registering items in the game, the item gets an ID that can change between sessions. Register items before loading the moon!
        /// </summary>
        public int RegisterItem(ItemDef def)
        {
            def.id = _nextItemId;
            if (def.Block != null)
            {
                def.Block.Id = _nextBlockId;
            }
            if (def.Logic != null)
            {
                def.Logic.itemDef = def;
            }
            ItemDefs.Add(def);
            _nextItemId++;
            LCApi.LogInfo($"Registered new item {def.Name} [{def.id}]");
            return def.id;
        }

        /// <summary>
        /// Registering craft recipes for assembler in the game. It is safe to register during the game
        /// </summary>
        public void RegisterCraftRecipe(CraftRecipeDef recipe)
        {
            AddToAssemblerListInternal(recipe.Items);
            LCApi.LogInfo("Registered new assembler recipe");
        }

        /// <summary>
        /// Registering alchemy transmutations in the game. It is safe to register during the game
        /// </summary>
        public void RegisterAlchemyRecipe(int fromBlockID, int toBlockID)
        {
            AddToAlchemyListInternal(fromBlockID, toBlockID);
            LCApi.LogInfo("Registered new alchemy recipe");
        }

        /// <summary>
        /// Searching custom item by name, for built-in items use ItemID and BlockID enums
        /// </summary>
        public ItemDef GetItemByName(string name)
        {
            return ItemDefs.FirstOrDefault(item => item.Name == name);
        }

        /// <summary>
        /// IDs can change between sessions, use serach by name.
        /// </summary>
        public ItemDef GetItemById(int id)
        {
            return ItemDefs.FirstOrDefault(item => item.id == id);
        }

        private void AddToAssemblerListInternal(List<(ItemID, int)> list)
        {
            FieldInfo field = AccessTools.DeclaredField(_craftingRecipes, "RECIPES");

            var internalList = (List<List<(ItemID, int)>>)field.GetValue(null);
            internalList.Add(list);

            field.SetValue(null, internalList);
        }

        private void AddToAlchemyListInternal(int A, int B)
        {
            FieldInfo field = AccessTools.DeclaredField(typeof(Alchemy), "TRANSMUTATIONS");

            var internalList = (Dictionary<BlockID, BlockID>)field.GetValue(null);
            internalList.Add((BlockID)A, (BlockID)B);

            field.SetValue(null, internalList);
        }
    }
}
