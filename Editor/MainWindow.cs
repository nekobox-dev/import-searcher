#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Nekobox.ImportSearcher
{
    public class MainWindow : EditorWindow, IHasCustomMenu
    {
        [SerializeField] private List<Package> filteredPackages;
        [SerializeField] private string searchFilter;

        [MenuItem(Defines.MENU_PATH)]
        public static void ShowWindow()
        {
            var window = GetWindow<MainWindow>(Defines.PACKAGE_NAME);
        }

        public void SetSearchFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                searchFilter = string.Empty;
                filteredPackages = Data.Packages;
            }
            else
            {
                searchFilter = filter.ToLower();
                filteredPackages = Data.Packages.Where(p => p.Name.ToLower().Contains(searchFilter) || p.Path.ToLower().Contains(searchFilter)).ToList();
            }
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Defines.STYLESHEET_FOLDER_PATH + "/MainWindow.uss");
            root.styleSheets.Add(styleSheet);

            var toolbar = new Toolbar();
            root.Add(toolbar);

            var searchField = new ToolbarSearchField();
            searchField.tabIndex = 1;
            toolbar.Add(searchField);

            Func<VisualElement> makeItem = () =>
            {
                var container = new VisualElement();
                var packageName = new Label();
                var packagePath = new Label();
                packageName.AddToClassList("package-name");
                packagePath.AddToClassList("package-path");
                container.Add(packageName);
                container.Add(packagePath);
                return container;
            };

            Action<VisualElement, int> bindItem = (elem, index) => 
            {
                var element = elem as VisualElement;
                var name = element.Q(className: "package-name") as Label;
                var path = element.Q(className: "package-path") as Label;
                name.text = filteredPackages[index].Name;
                path.text = filteredPackages[index].Path;

                element.RegisterCallback<ClickEvent>((_) => 
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(filteredPackages[index].Path);
                    EditorGUIUtility.PingObject(obj);
                });
            };

            SetSearchFilter(searchFilter);
            var list = new ListView(filteredPackages, 40f, makeItem, bindItem);
            list.tabIndex = 2;

            Func<KeyCode, char, EventModifiers, Event> createKeyDownEvent = (code, character, modifiers) => 
            {
                var evt = new Event() 
                {
                    type = EventType.KeyDown,
                    keyCode = code,
                    character = character,
                    modifiers = modifiers,
                };
                return evt;
            };

            list.RegisterCallback<KeyDownEvent>((evt) => 
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.Space:
                        var index = list.selectedIndex;
                        var obj = AssetDatabase.LoadAssetAtPath<Object>(filteredPackages[index].Path);
                        EditorGUIUtility.PingObject(obj);
                        break;
                    case KeyCode.W:
                    case KeyCode.A:
                        if (evt.modifiers != EventModifiers.None) break;
                        evt.StopPropagation();
                        using (var keyboardEvent = KeyDownEvent.GetPooled(createKeyDownEvent(KeyCode.UpArrow, '\0', EventModifiers.None)))
                        {
                            list.SendEvent(keyboardEvent);
                        }
                        break;
                    case KeyCode.S:
                    case KeyCode.D:
                        if (evt.modifiers != EventModifiers.None) break;
                        evt.StopPropagation();
                        using (var keyboardEvent = KeyDownEvent.GetPooled(createKeyDownEvent(KeyCode.DownArrow, '\0', EventModifiers.None)))
                        {
                            list.SendEvent(keyboardEvent);
                        }
                        break;
                    default:
                        break;
                }
            });
            Data.OnDataChanged += (_) => 
            {
                SetSearchFilter(searchFilter);
                list.itemsSource = filteredPackages;
                list.Rebuild();
            };
            searchField.RegisterCallback<ChangeEvent<string>>((e) => 
            {
                SetSearchFilter(e.newValue);
                list.itemsSource = filteredPackages;
                list.Rebuild();
            });

            root.Add(list);
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Clear Import Logs"), false, () => 
            {
                Data.Packages.Clear();
                Data.NotifyDataChanged(Defines.LOG_PREFIX + "Clear Import Logs");
            });
        }
    }
}

#endif // UNITY_EDITOR
