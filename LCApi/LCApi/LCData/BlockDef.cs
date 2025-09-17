using LCApi.Assets;
using UnityEngine;

namespace LCApi.LCData
{
    /// <summary>
    /// Unique container with properties for each new block dependent on ItemDef.
    /// ItemRegistry assigns a unique ID used in the game, usually matches ItemDef.
    /// CAN change between sessions
    /// </summary>
    public class BlockDef
    {
        public int Id;
        public string Name;
        public Texture2D BlockTexture;
        public BlockDef(string name, string pathToTexture)
        {
            Name = name;

            BlockTexture = TextureHelper.LoadBlockTexture(pathToTexture);
        }
    }
}
