using HarmonyLib;
using LCApi.Game;
using LCApi.Items;
using LCApi.LCData;
using LCApi.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LCApi
{
    public class LCCore : MonoBehaviour
    {
        private ItemRegistry _itemManager;
        private PlayerManager _playerManager;
        private WorldManager _worldManager;
        public StructureRegistry _structureRegistry;
        private GUIStyle _style;
        //private BlockRegistry _blockRegistry;
        private static LCCore _instance;
        private bool _loading;
        public static LCCore GetInstance()
        {
            return _instance;
        }
        public LCCore()
        {
            if (_instance != null)
            {
                LCApi.LogError("TRIED TO CREATE AN INSTANCE OF LCCore AGAIN!!!");
                return;
            }
            _instance = this;

            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontSize = 16;
            _style.fontStyle = FontStyle.Bold;
            _style.normal = new GUIStyleState();
            _style.normal.textColor = Color.white;
            _loading = true;
            DoPatches();

            //_blockRegistry = new BlockRegistry();
            _itemManager = new ItemRegistry();
            _structureRegistry = new StructureRegistry();

            SceneManager.sceneLoaded += OnSceneChanged;

        }

        private void Start()
        {
            StartCoroutine(LoadDelayed());
        }

        IEnumerator LoadDelayed()
        {
            yield return new WaitForSeconds(0.5f);
            CreateScriptsOfType(typeof(OnLoadScript));
            yield return new WaitForSeconds(0.5f);
            LoadCustomBlockMaterials();

            _loading = false;
        }

        private void OnSceneChanged(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex != 0)
            {
                _playerManager = new PlayerManager();
                _worldManager = new WorldManager();

                CreateScriptsOfType(typeof(InGameScript));
            }
        }

        private void DoPatches()
        {
            Harmony harmony = new Harmony("LCApiPatcher");

            harmony.PatchAll(typeof(LCPatches));
            harmony.PatchAll(typeof(PlayerPatches));
            harmony.PatchAll(typeof(ChunkPatches));

            LCApi.LogInfo($"The game has been patched");
        }

        private void CreateScriptsOfType(Type t)
        {
            LCApi.LogInfo($"Loading InGameScripts...");
            IEnumerable<Type> types = AccessTools.AllTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(t))
                {
                    GameObject newObj = new GameObject();
                    newObj.name = "_" + type.Name;
                    newObj.Internal_AddComponentWithType(type);

                    LCApi.LogInfo($"Created {type.FullName}");
                }
            }
        }

        private void OnGUI()
        {
            if (_loading)
            {
                Rect screen = new Rect(0, 0, Screen.width, Screen.height);
                GUI.DrawTexture(screen, Texture2D.blackTexture, ScaleMode.StretchToFill, false, 1f, Color.black, 0, 0);
                Rect text = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 200);
                GUI.Label(text, "LCApi loading...", _style);
                return;
            }

            /*
            FieldInfo field = AccessTools.DeclaredField(typeof(MeshData), "blockMaterials");
            List<Material> internalblockMaterials = (List<Material>)field.GetValue(null);
            if (internalblockMaterials != null)
            {
                int j = 0;
                for (int i = 0; i < internalblockMaterials.Count; i++)
                {
                    Rect rect = new Rect(j * 16, 0, 16, 16);
                    if (internalblockMaterials[i].HasTexture("_BaseTexture"))
                    {
                        Texture tex = internalblockMaterials[i].GetTexture("_BaseTexture");
                        if (tex != null)
                        {
                            GUI.DrawTexture(rect, tex);
                        }
                        GUI.Label(rect, i.ToString());
                        j++;
                    }
                    else
                    {
                        Texture tex = internalblockMaterials[0].GetTexture("_BaseTexture");
                        if (tex != null)
                        {
                            GUI.DrawTexture(rect, tex);
                            GUI.Label(rect, i.ToString());
                            j++;
                        }
                    }
                }
            }
            */
            /*
            int j = 0;
            for (int i = 0; i < _itemManager.ItemDefs.Count; i++)
            {
                Rect rect = new Rect(j * 128, 0, 128, 128);
                if (_itemManager.ItemDefs[i].Block != null)
                {
                    Texture tex = _itemManager.ItemDefs[i].Block.BlockTexture;
                    if (tex != null)
                    {
                        GUI.DrawTexture(rect, tex);
                    }
                    GUI.Label(rect, i.ToString());
                    j++;
                }
            }

            FieldInfo field = AccessTools.DeclaredField(typeof(MeshData), "sprites");
            List<Sprite> internalSprites = (List<Sprite>)field.GetValue(null);
            if (internalSprites != null)
            {
                for (int i = 0; i < internalSprites.Count; i++)
                {
                    if (internalSprites[i] != null)
                    {
                        Texture2D tex = internalSprites[i].texture;
                        if (tex != null)
                        {
                            Rect rect = new Rect(16 * i, 64, 16, 16);
                            GUI.DrawTexture(rect, tex);
                        }
                    }
                }
            }*/
        }

        private void LoadCustomBlockMaterials()
        {
            FieldInfo field = AccessTools.DeclaredField(typeof(MeshData), "blockMaterials");
            List<Material> internalblockMaterials = (List<Material>)field.GetValue(null);
            // Add custom block materials
            LCApi.LogInfo($"Loading custom block materials...");
            Material placeHolder = internalblockMaterials[0];

            for (int i = 0; i < 300; i++)
            {
                internalblockMaterials.Add(placeHolder);
            }

            for (int i = 0; i < _itemManager.ItemDefs.Count; i++)
            {
                Material material;
                ItemDef itemDef = _itemManager.ItemDefs[i];
                BlockDef blockDef = itemDef.Block;

                if (blockDef != null)
                {
                    material = new Material(placeHolder);
                    material.SetTexture("_BaseTexture", blockDef.BlockTexture);
                    if (material != null)
                    {
                        material.enableInstancing = true;
                        internalblockMaterials[itemDef.id] = material;
                    }
                }
                else
                {
                    material = new Material(Shader.Find("Custom/OpaqueShader"));
                    material.SetTexture("_BaseTexture", itemDef.Icon.texture);
                    if (material != null)
                    {
                        material.enableInstancing = true;
                        internalblockMaterials[itemDef.id] = material;
                    }
                }
                LCApi.LogInfo($"Added {itemDef.Name} block material");
            }
        }
    }
}
