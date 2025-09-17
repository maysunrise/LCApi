using LCApi.Items;
using System.IO;
using UnityEngine;

namespace LCApi.LCData
{
    /// <summary>
    /// Unique container with properties for each new item. ItemRegistry assigns a unique ID used in the game. CAN change between sessions
    /// </summary>
    public class ItemDef
    {
        internal int id;
        public string Name { get; private set; }
        public BlockDef Block { get; private set; }
        public Sprite Icon { get; private set; }
        public ItemLogicBase Logic { get; private set; }

        public ItemDef(string name, string pathToIcon, BlockDef block = null, ItemLogicBase logic = null)
        {
            Name = name;
            Block = block;
            Logic = logic;

            byte[] texData = File.ReadAllBytes(pathToIcon);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(texData, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.Apply();
            Icon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                Vector2.zero, 1f, 0, SpriteMeshType.FullRect, Vector4.zero, false);
        }

        public int GetID()
        {
            return id;
        }

        public ItemID GetItemID()
        {
            return (ItemID)id;
        }
    }
}
