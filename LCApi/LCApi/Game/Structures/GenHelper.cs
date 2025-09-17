namespace LCApi.Game.Structures
{
    public static class GenHelper
    {
        public static int GetStructureSeed(ulong worldSeed, int chunkX, int chunkZ)
        {
            ulong num = (ulong)(chunkX + 341873128712L);
            ulong num2 = (ulong)(chunkZ + 132897987541L);
            long num3 = (long)(worldSeed ^ (num * 5871781006564002453L)) ^ ((long)num2 * -7046029254386353131L);
            long num4 = (num3 ^ (num3 >>> 33)) * -49064778989728563L;
            long num5 = (num4 ^ (num4 >>> 33)) * -4265267296055464877L;
            return (int)((num5 ^ (num5 >>> 33)) & 0x7FFFFFFF);
        }

        public static int GetHighestPos(byte[] blocks, int x, int z)
        {
            for (int i = 1; i < WorldManager.WorldHeight; i++)
            {
                int height = (WorldManager.WorldHeight - i) - 1;
                byte block = blocks[ChunkHelpers.GetChunkIndex(x, height, z)];
                if (block != 39)
                {
                    return height;
                }
            }
            return 0;
        }

    }
}
