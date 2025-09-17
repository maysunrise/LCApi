using HarmonyLib;
using LCApi.Game;
using LCApi.Items;
using LCApi.LCData;
using System.Collections.Generic;
using UnityEngine;

namespace LCApi.Patches
{
    internal class PlayerPatches
    {

        [HarmonyPatch(typeof(Player), "ThrowStack")]
        [HarmonyPostfix]
        static void ThrowStackPatched(Player __instance)
        {
            PlayerManager manager = PlayerManager.GetInstance();
        }

        [HarmonyPatch(typeof(Player), "Update")]
        [HarmonyPrefix]
        static void UpdatePatched(Player __instance)
        {
            PlayerManager manager = PlayerManager.GetInstance();
            ItemRegistry registry = ItemRegistry.GetInstance();
            if (manager.Health > 0)
            {
                int currentItem = (int)manager.Inventory.GetSelectedItem();

                ItemDef itemDef = registry.GetItemById(currentItem);
                if (itemDef != null && itemDef.Logic != null)
                {
                    ItemLogicBase logic = itemDef.Logic;
                    logic.Holding();
                    if (manager.CanUseItems)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            logic.PrimaryAction(PlayerItemAction.StartAction);
                        }
                        else if (Input.GetMouseButton(0))
                        {
                            logic.PrimaryAction(PlayerItemAction.UpdateAction);
                        }
                        else if (Input.GetMouseButtonUp(0))
                        {
                            logic.PrimaryAction(PlayerItemAction.EndAction);
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            logic.SecondaryAction(PlayerItemAction.StartAction);
                        }
                        else if (Input.GetMouseButton(1))
                        {
                            logic.SecondaryAction(PlayerItemAction.UpdateAction);
                        }
                        else if (Input.GetMouseButtonUp(1))
                        {
                            logic.SecondaryAction(PlayerItemAction.EndAction);
                        }
                    }
                }
            }

        }

        [HarmonyPatch(typeof(Player), "Update")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> UpdatePatched(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            ReplacedMethods.DoBlockEnumPatch(ref codes);
            return codes;
        }

        [HarmonyPatch(typeof(Player), "ThrowStack")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ThrowStackPatched(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            ReplacedMethods.DoBlockEnumPatch(ref codes);
            return codes;
        }
    }
}
