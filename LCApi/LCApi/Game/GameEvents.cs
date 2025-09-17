using System;

namespace LCApi.Game
{
    /// <summary>
    /// Game events that we can subscribe
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// When any item in the world was dropped
        /// </summary>
        public static EventHandler OnDroppedItem = delegate { };

        internal static void DroppedItem(Dropped dropped)
        {
            OnDroppedItem.Invoke(dropped, null);
        }
    }
}
