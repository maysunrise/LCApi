namespace LCApi.Game.Structures
{
    /// <summary>
    /// Base class for a structure, runs once every chunk while generation process
    /// </summary>
    public abstract class GenStructureBase
    {
        public abstract bool GenerateChunk(byte[] blocks, int chunkX, int chunkZ, MoonData moonData);
    }
}
