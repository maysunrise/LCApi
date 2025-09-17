using LCApi;
using LCApi.Game;
using LCApi.Items;
using LCApi.LCData;

namespace LunacraftModTemplate
{
    // Instantiated in the game scene
    public class GiveItemsToPlayer : InGameScript
    {
        private void Start()
        {
            // Get our item definition
            ItemDef myItem = ItemRegistry.GetInstance().GetItemByName("super_dirt");

            // Player manager is a bridge to the game internal classes of the player
            PlayerManager playerManager = PlayerManager.GetInstance();

            if (playerManager.IsCreative)
            {
                // Add stack of our item with amount 30
                playerManager.Player.AddToInventory(myItem.GetItemID(), 30, true);

                // Redraw inventory
                playerManager.UpdateInventoryUI();
            }
        }
    }
}
