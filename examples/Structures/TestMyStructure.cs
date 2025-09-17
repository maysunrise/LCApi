using LCApi.Game;
using LCApi.Game.Structures;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RegisterMyStructures : OnLoadScript
{
	private void Start()
	{
		StructureRegistry structureRegistry = StructureRegistry.GetInstance();

		TestMyStructure myTestStructure = new TestMyStructure();
		structureRegistry.RegisterStructure(myTestStructure);
	}
}

public class TestMyStructure : GenStructureBase
{
	// Spawn random boxes on the terrain
	public override bool GenerateChunk(byte[] blocks, int chunkX, int chunkZ, MoonData moonData)
	{
		if (Random.value < 0.4f)
		{
			return false;
		}
		int spawnPosX = Random.Range(-10, 10) + WorldManager.ChunkSize / 2;
		int spawnPosZ = Random.Range(-10, 10) + WorldManager.ChunkSize / 2;

		int height = GenHelper.GetHighestPos(blocks, spawnPosX, spawnPosZ);;
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 20; y++)
			{
				for (int z = 0; z < 10; z++)
				{
					if (y + height < 128)
					{
						blocks[ChunkHelpers.GetChunkIndex(x + spawnPosX, y + height, z + spawnPosZ)] = (byte)BlockID.silver_ore;
					}
				}
			}
		}

		return true;
	}
}