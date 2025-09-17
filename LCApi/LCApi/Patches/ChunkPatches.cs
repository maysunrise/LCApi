using HarmonyLib;
using LCApi.Game;
using System.Reflection;
using UnityEngine;

namespace LCApi.Patches
{
    internal class ChunkPatches
    {
        [HarmonyPatch(typeof(ChunkManager), "SaveAllChunksToFile")]
        [HarmonyPostfix]
        static void SaveAllChunksToFilePatched(ChunkManager __instance)
        {
            FieldInfo field = AccessTools.Field(typeof(ChunkManager), "moon");
            int moonIndex = (int)field.GetValue(__instance);

            string path = string.Format("{0}/moons/moon{1}/custom.dat", Application.persistentDataPath, moonIndex);
        }

        [HarmonyPatch(typeof(ChunkHelpers), "GenerateChunk")]
        [HarmonyPostfix]
        static void GenerateChunkPatched(byte[] chunk, int chunkX, int chunkZ, MoonData moonData)
        {
            StructureRegistry registry = StructureRegistry.GetInstance();
            registry.TryGenerateChunk(chunk, chunkX, chunkZ, moonData);
        }
    }
}
