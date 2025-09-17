using LCApi;
using LCApi.Items;
using LCApi.LCData;

namespace LunacraftModTemplate
{
    // Instantiated in the main menu scene
    public class InitMyStuff : OnLoadScript
    {
        private void Start()
        {
            // Get item registry instance, it contains everything you need to register items in the game
            ItemRegistry registry = ItemRegistry.GetInstance();

            // Create a block definition that is a child of the item definition
            // All static data about the block is stored here, such as the name, texture and unique ID
            // We can have item without block, but not blocks without an item
            BlockDef blockDef = new BlockDef("super_dirt",
                LCApi.LCApi.GetModPath(PluginInfo.PLUGIN_NAME, "/assets/test_block.png"));

            // Create an item definition that is the parent of the block
            // All static data about the item is stored here, such as the name, sprite and unique ID
            // It can also contain a reference to a 'ItemLogicBase' class to perform an action
            // Adding a block or logic class is optional
            ItemDef itemDef = new ItemDef("super_dirt",
                LCApi.LCApi.GetModPath(PluginInfo.PLUGIN_NAME, "/assets/test_item.png"), blockDef);

            // Add an item to the list, when this function is called, a unique ID is assigned,
            // ID may change in the next session, to refer to an item we use its name
            registry.RegisterItem(itemDef);
        }

        private void Update()
        {

        }
    }
}
