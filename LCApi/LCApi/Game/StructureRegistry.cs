using LCApi.Game.Structures;
using System.Collections.Generic;

namespace LCApi.Game
{
    /// <summary>
    /// List of all registered structures for world generation
    /// </summary>
    public class StructureRegistry
    {
        public List<GenStructureBase> Structures = new List<GenStructureBase>();

        private static StructureRegistry _instance;

        public static StructureRegistry GetInstance()
        {
            return _instance;
        }

        public StructureRegistry()
        {
            if (_instance != null)
            {
                return;
            }
            _instance = this;
        }

        public void RegisterStructure(GenStructureBase structure)
        {
            Structures.Add(structure);
            LCApi.LogInfo($"Registered new structure {structure.GetType().Name}");
        }

        public bool TryGenerateChunk(byte[] blocks, int chunkX, int chunkZ, MoonData moonData)
        {
            // Save last random state
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(GenHelper.GetStructureSeed(moonData.seed - 16, chunkX, chunkZ));
            foreach (GenStructureBase structure in Structures)
            {
                if (structure.GenerateChunk(blocks, chunkX, chunkZ, moonData))
                {
                    // Restore random state
                    UnityEngine.Random.state = state;
                    return true;
                }
            }
            // Restore random state
            UnityEngine.Random.state = state;
            return false;
        }

    }
}
