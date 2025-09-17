using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace LCApi.Game
{
    /// <summary>
    /// Public API for managing world/moon state.
    /// Has useful references to internal game classes.
    /// Only available during game, NULL in menu scene
    /// </summary>
    public class WorldManager
    {
        /// <summary>
        /// Internal game class that manages game time
        /// </summary>
        public WorldClock WorldClock { get; private set; }

        /// <summary>
        /// Internal game class with current moon data
        /// </summary>
        public MoonData MoonData { get; private set; }

        /// <summary>
        /// Internal game class that manages chunk
        /// </summary>
        public ChunkManager ChunkManager { get; private set; }
        /// <summary>
        /// Parent for all chunk gameobjects
        /// </summary>
        public GameObject ChunkParent { get; private set; }
        private static WorldManager _instance;

        public readonly static int ChunkSize = 32;
        public readonly static int WorldHeight = 128;

        private Type _blockDataType;
        private Vector3[] _dirs = new Vector3[6]
        {
            Vector3.up,
            Vector3.right,
            Vector3.forward,
            Vector3.down,
            Vector3.left,
            Vector3.back
        };
        private MethodInfo _spawnBlockMethod;

        public static WorldManager GetInstance()
        {
            return _instance;
        }

        public WorldManager()
        {
            _instance = this;
            WorldClock = GameObject.Find("WorldClock").GetComponent<WorldClock>();
            ChunkManager = GameObject.Find("ChunkManager").GetComponent<ChunkManager>();
            ChunkParent = GameObject.Find("ChunkParent");
            _blockDataType = AccessTools.TypeByName("BlockData");
            _spawnBlockMethod = AccessTools.DeclaredMethod(typeof(ChunkManager), "SpawnBlock");

            MoonData = PlayerManager.GetInstance().MoonData;
        }

        // This won't work well...
        // For better performance we should probably use our own implementations of these methods
        /// <summary>
        /// Sets a block in world coordinates. Not very fast for large manipulations
        /// </summary>
        public void SetBlockAt(int x, int y, int z, int id)
        {
            ChunkData chunkData = GetChunkAt(x, z);
            if (chunkData == null)
            {
                return;
            }
            int chunkX = Mathf.FloorToInt(x / ChunkSize);
            int chunkZ = Mathf.FloorToInt(z / ChunkSize);

            (int, int, int) localBlockPos = ChunkHelpers.GetLocalBlockPos(x, y, z);
            int currentBlockId = GetBlockAt(x, y, z);
            if (id == 39) // Air
            {
                if (currentBlockId != 39)
                {
                    GameObject currentBlock = GetBlockDataAt(x, y, z);
                    if (currentBlock != null)
                    {
                        GameObject.Destroy(currentBlock);
                    }

                    for (int i = 0; i < _dirs.Length; i++)
                    {
                        Vector3 dir = _dirs[i];
                        int neighborId = GetBlockAt(x + (int)dir.x, y + (int)dir.y, z + (int)dir.z);
                        if (neighborId != 39)
                        {
                            GameObject neighborBlock = GetBlockDataAt(x + (int)dir.x, y + (int)dir.y, z + (int)dir.z);
                            if (neighborBlock == null)
                            {
                                _spawnBlockMethod.Invoke(ChunkManager,
                                    new object[] {chunkData.gameObject, chunkX, chunkZ, (BlockID)neighborId,
                            localBlockPos.Item1 + (int)dir.x, localBlockPos.Item2 + (int)dir.y, localBlockPos.Item3 + (int)dir.z });
                            }
                        }
                    }
                    // TODO update neighbor water and crystal blocks
                }
            }
            else // Solid block
            {
                if (currentBlockId != 39)
                {
                    GameObject currentBlock = GetBlockDataAt(x, y, z);
                    if (currentBlock != null)
                    {
                        GameObject.Destroy(currentBlock);
                    }
                }

                for (int i = 0; i < _dirs.Length; i++)
                {
                    Vector3 dir = _dirs[i];
                    if (GetBlockAt(x + (int)dir.x, y + (int)dir.y, z + (int)dir.z) == 39)
                    {
                        _spawnBlockMethod.Invoke(ChunkManager,
                            new object[] {chunkData.gameObject, chunkX, chunkZ, (BlockID)id,
                            localBlockPos.Item1, localBlockPos.Item2, localBlockPos.Item3 });
                        break;
                    }
                }
            }

            chunkData.blocks[ChunkHelpers.GetChunkIndex(
                localBlockPos.Item1, localBlockPos.Item2, localBlockPos.Item3)] = (byte)id;
            //ChunkManager.UpdateNeighborChunkBorders(BlockID.air, component4.globalPosX, component4.globalPosZ, component3.localPosX, component3.localPosY, component3.localPosZ);
        }

        public void SetFastBlockAt(int x, int y, int z, int id)
        {
            ChunkData chunkData = GetChunkAt(x, z);
            if (chunkData == null)
            {
                return;
            }
            int chunkX = Mathf.FloorToInt(x / ChunkSize);
            int chunkZ = Mathf.FloorToInt(z / ChunkSize);
            (int, int, int) localBlockPos = ChunkHelpers.GetLocalBlockPos(x, y, z);
            chunkData.blocks[ChunkHelpers.GetChunkIndex(
                localBlockPos.Item1, localBlockPos.Item2, localBlockPos.Item3)] = (byte)id;
        }

        public int GetBlockAt(int x, int y, int z)
        {
            ChunkData chunkData = GetChunkAt(x, z);
            if (chunkData == null)
            {
                return 39;
            }

            (int, int, int) localBlockPos = ChunkHelpers.GetLocalBlockPos(x, y, z);
            return chunkData.blocks[ChunkHelpers.GetChunkIndex(
                localBlockPos.Item1, localBlockPos.Item2, localBlockPos.Item3)];
        }

        public int GetHighestPos(int x, int z)
        {
            ChunkData chunkData = GetChunkAt(x, z);
            if (chunkData == null)
            {
                return 0;
            }
            for (int i = 0; i < WorldHeight; i++)
            {
                int height = WorldHeight - i;
                (int, int, int) localBlockPos = ChunkHelpers.GetLocalBlockPos(x, height, z);
                byte block = chunkData.blocks[ChunkHelpers.GetChunkIndex(localBlockPos.Item1, localBlockPos.Item2, localBlockPos.Item3)];
                if (block != 39)
                {
                    return height;
                }
            }
            return 0;
        }

        public GameObject GetBlockDataAt(int x, int y, int z)
        {
            Collider[] colls = Physics.OverlapBox(new Vector3(x, y, z), new Vector3(0.5f, 0.5f, 0.5f),
                Quaternion.identity, LayerMask.GetMask("Block"));

            if (colls.Length == 0)
            {
                return null;
            }

            return colls[0].gameObject;
        }

        // Dictionary/hashmap is worth considering instead of looking by name lol
        /// <summary>
        /// Gets a chunk at world coordinates
        /// </summary>
        public ChunkData GetChunkAt(int x, int z)
        {
            int chunkX = Mathf.FloorToInt(x / ChunkSize);
            int chunkZ = Mathf.FloorToInt(z / ChunkSize);
            Transform transform = ChunkParent.transform.Find($"Chunk ({chunkX},{chunkZ})");
            if (transform == null)
            {
                return null;
            }
            return transform.GetComponent<ChunkData>();
        }
    }
}
