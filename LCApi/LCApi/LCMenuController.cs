using BepInEx.Bootstrap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LCApi
{
    internal class LCMenuController : MonoBehaviour
    {
        private Canvas _canvas;
        private EventSystem _eventSystem;
        private bool _showedModMenu;
        private Texture2D _menuBg;
        private Texture2D _mechaIcon;
        private GUIStyle _style;
        private Vector2 _mainScroll = Vector2.zero;

        private void Start()
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            GameObject helpButtonRef = GameObject.Find("Help Button");
            GameObject copy = Instantiate(helpButtonRef);
            copy.transform.SetParent(_canvas.transform, false);
            copy.name = "ModSettingsButton";
            //1071.3f , -572.8f
            copy.GetComponent<RectTransform>().anchoredPosition = new Vector2(760f, -572.8f);
            Destroy(copy.gameObject.GetComponent<HelpButton>());
            copy.GetComponentInChildren<Text>().text = "Mods";

            copy.GetComponent<Button>().onClick.AddListener(OpenModsMenu);

            _menuBg = Resources.Load<Texture2D>("images/menu_bg");
            _mechaIcon = Resources.Load<Texture2D>("textures/items/mechanism_item");

            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontSize = 18;
            _style.fontStyle = FontStyle.Normal;
            _style.normal = new GUIStyleState();
            _style.normal.textColor = Color.black;
        }

        private void OpenModsMenu()
        {
            _showedModMenu = true;
            _eventSystem.enabled = false;
        }

        private void OnGUI()
        {
            if (!_showedModMenu)
            {
                return;
            }
            float scaleFactor = _canvas.scaleFactor;
            float centerX = Screen.width / 2;
            float centerY = Screen.height / 2;

            Vector2 windowSize = new Vector2(_menuBg.width / 1.5f, _menuBg.height / 1.5f) * scaleFactor;

            Rect windowRect = new Rect(
                centerX - windowSize.x / 2,
                centerY - windowSize.y / 2,
                windowSize.x, windowSize.y);
            GUI.DrawTexture(windowRect, _menuBg);

            float margin = 60 * scaleFactor;
            Rect viewRect = new Rect(
                centerX - windowSize.x / 2 + margin / 2,
                centerY - windowSize.y / 2 + margin / 2,
                windowSize.x - margin, windowSize.y - margin);

            GUILayout.BeginArea(viewRect);
            GUILayout.Label($"Installed mods [{Chainloader.PluginInfos.Count}]", _style);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll, false, true);

            GUIStyle modContainerStyle = new GUIStyle(GUI.skin.button);
            modContainerStyle.normal.textColor = Color.black;
            modContainerStyle.normal.background = Texture2D.whiteTexture;
            modContainerStyle.hover = modContainerStyle.normal;
            modContainerStyle.focused = modContainerStyle.normal;
            modContainerStyle.active = modContainerStyle.normal;
            modContainerStyle.fontSize = 18;
            //modContainerStyle.fixedWidth = 64f * scaleFactor;
            //modContainerStyle.fixedHeight = 64f * scaleFactor;
            modContainerStyle.alignment = TextAnchor.MiddleLeft;

            GUIStyle configButtonStyle = new GUIStyle(modContainerStyle);
            configButtonStyle.alignment = TextAnchor.MiddleCenter;
            configButtonStyle.normal.background = Texture2D.normalTexture;
            configButtonStyle.fixedWidth = 64f * scaleFactor;
            configButtonStyle.fixedHeight = 64f * scaleFactor;
            foreach (var plugin in Chainloader.PluginInfos)
            {
                var info = plugin.Value;
                var metadata = info.Metadata;
                GUILayout.BeginHorizontal(modContainerStyle);
                GUILayout.Label($"{metadata.Name} ({metadata.Version})", modContainerStyle);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_mechaIcon, configButtonStyle))
                {
                    LCApi.LogInfo("Plugin config placeholder button");
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUIStyle okButtonStyle = new GUIStyle(GUI.skin.button);
            okButtonStyle.normal.textColor = Color.black;
            okButtonStyle.normal.background = Texture2D.whiteTexture;
            okButtonStyle.fontSize = 18;
            okButtonStyle.fixedWidth = 200f * scaleFactor;
            okButtonStyle.fixedHeight = 60f * scaleFactor;
            okButtonStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", okButtonStyle))
            {
                _showedModMenu = false;
                _eventSystem.enabled = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }
    }
}
