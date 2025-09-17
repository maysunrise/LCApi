using LCApi.LCData;

namespace LCApi.Items
{
    /// <summary>
    /// Listen player actions when the item is in the main hand. It can be used for in-game tools
    /// </summary>
    public abstract class ItemLogicBase
    {
        internal ItemDef itemDef;
        public abstract void PrimaryAction(PlayerItemAction state);
        public abstract void SecondaryAction(PlayerItemAction state);
        public abstract void Holding();

        public ItemDef GetItemDef()
        {
            return itemDef;
        }
    }
}
