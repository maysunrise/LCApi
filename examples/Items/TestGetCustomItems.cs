using LCApi.Game;
using LCApi.Items;
using LCApi.LCData;
using System;
using System.Collections.Generic;
using System.Text;

public class TestRegisterCustomItems : OnLoadScript
    {
        private void Start()
        {
            ItemRegistry registry = ItemRegistry.GetInstance();
			
			// Item 1
            BlockDef blockDef = new BlockDef("super_dirt", "assets/test_block.png");
            ItemDef itemDef = new ItemDef("super_dirt", "assets/test_item.png", blockDef);
            registry.RegisterItem(itemDef);
			
			// Item 2
            TestExplosiveItemLogic explosiveLogic = new TestExplosiveItemLogic();
            ItemDef itemDef2 = new ItemDef("explosive", "assets/explosive_item.png", null, explosiveLogic);
            registry.RegisterItem(itemDef2);
			
			// New crafting recipe
            CraftRecipeDef recipe = new CraftRecipeDef();
			
            recipe.AddCell(ItemID.dirt, 2);
            recipe.AddCell(ItemID.slug_pistol_t1, 1);
            recipe.SetResult(itemDef.GetItemID(), 10);
			
            registry.RegisterCraftRecipe(recipe);
        }
 }

public class TestGetCustomItems : InGameScript
{
	private void Start()
	{
		ItemDef item1 = ItemRegistry.GetInstance().GetItemByName("super_dirt");
		ItemDef item2 = ItemRegistry.GetInstance().GetItemByName("explosive");

		// Get local player manager
		PlayerManager playerManager = PlayerManager.GetInstance();

		if (playerManager.IsCreative)
		{
			// Get player instance and add our items to the inventory
			playerManager.Player.AddToInventory(item1.GetItemID(), 1, true);
			playerManager.Player.AddToInventory(item2.GetItemID(), 100, true);
			playerManager.UpdateInventoryUI();
		}

		GameEvents.OnDroppedItem += DroppedItemTest;
	}

	private void DroppedItemTest(object sender, EventArgs args)
	{
		Dropped dropped = (Dropped)sender;
		LCApi.LogInfo($"Dropped item {dropped.itemID.ToString()} - {dropped.amount}");
	}
}
