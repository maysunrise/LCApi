using BepInEx;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LCApi
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class LCApi : BaseUnityPlugin
    {
        public static bool CORE_WAS_LOADED { get; private set; }
        private static LCApi _instance;
        private void Awake()
        {
            _instance = this;
            Logger.LogInfo($"LCApi initialized! Version [{PluginInfo.PLUGIN_VERSION}] (By maysunbo)");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 0)
            {
                StartCoroutine(LoadGameInjector());
            }
        }

        IEnumerator LoadGameInjector()
        {
            yield return new WaitForSeconds(0.25f);

            GameObject lcmenu = new GameObject();
            lcmenu.name = "_LCApiMenuInjector";
            lcmenu.AddComponent<LCMenuController>();

            if (!CORE_WAS_LOADED)
            {
                GameObject lccore = new GameObject();
                lccore.name = "_LCApi";
                lccore.AddComponent<LCCore>();
                DontDestroyOnLoad(lccore);
                CORE_WAS_LOADED = true;
            }
        }


        internal static void LogInfo(object data)
        {
            if (data == null)
            {
                data = "NULL";
            }
            _instance.Logger.LogInfo($"[LCApi]: {data}");
        }

        internal static void LogError(object data)
        {
            if (data == null)
            {
                data = "NULL";
            }
            _instance.Logger.LogError($"[LCApi]: {data}");
        }

        /// <summary>
        /// <para>Returns the absolute path to the mod directory, based on a relative one.</para> 
        /// <para>Example 'assets/somefolder/my_cool_block.png' --> 'C:/Users/john/Desktop/Lunacraft/BepInEx/plugins/MY_COOL_MOD/assets/somefolder/my_cool_block.png'</para> 
        /// </summary>
        public static string GetModPath(string modName, string str = null)
        {
            string p = Path.Combine(Paths.BepInExRootPath, "plugins", modName);
            if (str == null)
            {
                return p;
            }
            else
            {
                return p + str;
            }
        }
    }
}
