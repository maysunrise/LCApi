using System.Collections.Generic;

namespace LCApi.LCData
{
    /// <summary>
    /// Contains a list of ingredients for crafting an item in assembler. 9 cells max, use 'ItemID.air' for complex recipes. First item in the list 'Items' is always the result of crafting
    /// </summary>
    public class CraftRecipeDef
    {
        public List<(ItemID, int)> Items { protected set; get; }

        public CraftRecipeDef()
        {
            Items = new List<(ItemID, int)>();
            Items.Add((ItemID.air, 0));
        }

        public void AddCell(ItemID itemId, int count)
        {
            Items.Add((itemId, count));
        }

        public void AddCell(int itemId, int count)
        {
            Items.Add(((ItemID)itemId, count));
        }

        public void SetResult(ItemID itemId, int count)
        {
            Items[0] = (itemId, count);
        }
    }
}
