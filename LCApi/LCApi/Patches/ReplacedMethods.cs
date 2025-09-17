using HarmonyLib;
using LCApi.Items;
using LCApi.LCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LCApi.Patches
{
    internal class ReplacedMethods
    {
        // Makes a simple IL patch to replace TryParse<BlockID> with an alternative method to fix custom items
        public static void DoBlockEnumPatch(ref List<CodeInstruction> codes)
        {
            MethodInfo originalMethod = typeof(Enum).GetMethods()
           .First(m => m.Name == "TryParse" &&
           m.IsGenericMethodDefinition &&
           m.GetParameters().Length == 2).MakeGenericMethod(typeof(BlockID));

            MethodInfo myMethod = AccessTools.Method(typeof(ReplacedMethods), "TryParseItem");

            for (int i = 1; i < codes.Count; i++)
            {
                CodeInstruction pastInstr = codes[i - 1];
                CodeInstruction instr = codes[i];
                if (instr.opcode == OpCodes.Call &&
                    instr.Calls(originalMethod))
                {
                    instr.opcode = OpCodes.Call;
                    instr.operand = myMethod;
                }
            }
        }

        private static bool TryParseItem(string str, out BlockID itemId)
        {
            //Plugin.LogInfo($"Parsing {str}");
            ItemRegistry registry = ItemRegistry.GetInstance();
            bool isModdedItem = int.TryParse(str, out int moddedId);

            BlockID result = BlockID.air;
            if (!isModdedItem)
            {
                if (Enum.TryParse<BlockID>(str, out result))
                {
                    itemId = result;
                    return true;
                }
            }
            else
            {
                ItemDef itemDef = registry.GetItemById(moddedId);
                result = (BlockID)moddedId;
                if (itemDef.Block != null)
                {
                    itemId = result;
                    return true;
                }
            }

            itemId = result;
            return false;
        }
    }
}
