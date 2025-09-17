using HarmonyLib;
using LCApi.Game;
using LCApi.Items;
using LCApi.LCData;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LCApi.Patches
{
    internal class LCPatches
    {
        static internal bool loadedIconSprites = false;

        [HarmonyPatch(typeof(ChunkManager), "Awake")]
        [HarmonyPostfix]
        static void AwakePatched(ChunkManager __instance)
        {
            FieldInfo field = AccessTools.DeclaredField(typeof(ChunkManager), "blocks");

            Array internalBlocksArray = (Array)field.GetValue(__instance);
            internalBlocksArray = Array.CreateInstance(typeof(int), 255);
            for (int i = 0; i < internalBlocksArray.Length; i++)
            {
                internalBlocksArray.SetValue(i, i);
            }

            FieldInfo field2 = AccessTools.DeclaredField(typeof(ChunkManager), "numberOfBlocks");
            field2.SetValue(__instance, (int)internalBlocksArray.Length);
        }

        [HarmonyPatch(typeof(MeshData), "GetItemSprite")]
        [HarmonyPostfix]
        static void GetItemSpritePatched(MeshData __instance, ItemID itemID)
        {
            ItemRegistry registry = ItemRegistry.GetInstance();

            FieldInfo field = AccessTools.DeclaredField(typeof(MeshData), "sprites");
            List<Sprite> internalSprites = (List<Sprite>)field.GetValue(null);
            if (!loadedIconSprites)
            {
                for (int i = 0; i < 255; i++)
                {
                    internalSprites.Add(Resources.Load<Sprite>("Textures/Items/astronaut_egg_item"));
                }
                LCApi.LogInfo($"Loading custom sprites...");
                for (int i = 0; i < registry.ItemDefs.Count; i++)
                {
                    ItemDef itemDef = registry.ItemDefs[i];
                    if (itemDef.Icon != null)
                    {
                        //internalSprites.Add(itemDef.Icon);
                        internalSprites[itemDef.id] = itemDef.Icon;
                        LCApi.LogInfo($"Added {itemDef.Name} sprite");
                    }
                    else
                    {
                        // if null then set icon to some weird texture and log error, so we can notice a problem
                        //internalSprites.Add(Resources.Load<Sprite>("Textures/Items/astronaut_egg_item"));
                        internalSprites[itemDef.id] = Resources.Load<Sprite>("Textures/Items/astronaut_egg_item");
                        LCApi.LogError($"Sprite is null for item {itemDef.Name} - [{itemDef.id}]");
                    }
                }
                field.SetValue(null, internalSprites);
                loadedIconSprites = true;
            }
            //return internalSprites[(int)itemID];
        }

        [HarmonyPatch(typeof(Dropped), "Drop")]
        [HarmonyPrefix]
        static void DropPatched(Dropped __instance)
        {
            GameEvents.DroppedItem(__instance);
        }
    }
}
