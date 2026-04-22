using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class SampleToolWindowController : WindowFeatureControllerBase
    {
        [SerializeField] private bool openOnStart = false;
        [SerializeField] private VisualTreeAsset sampleToolContentUxml;
        [SerializeField] private StyleSheet[] sampleToolStyleSheets;

        private SampleToolWindow _window;

        // サンプルデータ
        private readonly List<string> _dropdownChoices = new()
        {
            "Item A",
            "Item B",
            "Item C",
            "Item D",
        };

        private readonly List<string> _listItems = new()
        {
            "Potion",
            "Hi-Potion",
            "Ether",
            "Phoenix Down",
            "Elixir",
            "Antidote",
            "Remedy",
        };

        private readonly List<TreeViewItemData<string>> _treeItems = new()
        {
            new TreeViewItemData<string>(1, "Weapons", new List<TreeViewItemData<string>>
            {
                new TreeViewItemData<string>(11, "Sword"),
                new TreeViewItemData<string>(12, "Axe"),
                new TreeViewItemData<string>(13, "Bow"),
            }),
            new TreeViewItemData<string>(2, "Armor", new List<TreeViewItemData<string>>
            {
                new TreeViewItemData<string>(21, "Shield"),
                new TreeViewItemData<string>(22, "Helmet"),
                new TreeViewItemData<string>(23, "Plate Armor"),
            }),
            new TreeViewItemData<string>(3, "Magic", new List<TreeViewItemData<string>>
            {
                new TreeViewItemData<string>(31, "Fire"),
                new TreeViewItemData<string>(32, "Ice"),
                new TreeViewItemData<string>(33, "Thunder"),
            }),
        };

        private void Start()
        {
            if (!openOnStart)
                return;

            OpenSampleWindow();
        }

        public void OpenSampleWindow()
        {
            if (sampleToolContentUxml == null)
            {
                Debug.LogError("SampleToolWindowController: content UXML is not assigned.");
                return;
            }

            if (Context == null)
            {
                Debug.LogError("SampleToolWindowController: Window context is not ready.");
                return;
            }

            if (_window != null && _window.IsOpen)
            {
                Context.WindowManager.Focus(_window);
                RefreshWindowData();
                return;
            }

            _window = new SampleToolWindow(
                Context,
                sampleToolContentUxml,
                sampleToolStyleSheets);

            Context.WindowManager.Open(_window);

            // Open直後にVisualTreeが取れる設計ならこれでOK
            // もしまだ生成前なら schedule で1フレーム後に呼んでください
            RefreshWindowData();
        }

        public void FocusSampleWindow()
        {
            if (_window != null && _window.IsOpen)
            {
                Context.WindowManager.Focus(_window);
            }
        }

        public void CloseSampleWindow()
        {
            if (_window != null && _window.IsOpen)
            {
                _window.RequestClose();
            }
        }

        private void RefreshWindowData()
        {
            if (_window == null || !_window.IsOpen)
                return;

            // ここはあなたの SampleToolWindow 実装に合わせて変更
            var root = _window.Root;
            if (root == null)
            {
                Debug.LogWarning("SampleToolWindowController: window root is null.");
                return;
            }

            SetupRadioButtons(root);
            SetupDropdown(root);
            SetupListView(root);
            SetupTreeView(root);
            SetupButtons(root);
        }

        private void SetupRadioButtons(VisualElement root)
        {
            var radioGroup = root.Q<RadioButtonGroup>("mode-radio-group");
            if (radioGroup == null)
                return;

            radioGroup.value = 0;

            radioGroup.UnregisterValueChangedCallback(OnRadioGroupChanged);
            radioGroup.RegisterValueChangedCallback(OnRadioGroupChanged);
        }

        private void OnRadioGroupChanged(ChangeEvent<int> evt)
        {
            Debug.Log($"Radio selected index: {evt.newValue}");
        }

        private void SetupDropdown(VisualElement root)
        {
            var dropdown = root.Q<DropdownField>("category-dropdown");
            if (dropdown == null)
                return;

            dropdown.choices = _dropdownChoices;
            dropdown.index = 0;

            dropdown.UnregisterValueChangedCallback(OnDropdownChanged);
            dropdown.RegisterValueChangedCallback(OnDropdownChanged);
        }

        private void OnDropdownChanged(ChangeEvent<string> evt)
        {
            Debug.Log($"Dropdown selected: {evt.newValue}");
        }

        private void SetupListView(VisualElement root)
        {
            var listView = root.Q<ListView>("sample-listbox");
            if (listView == null)
                return;

            listView.itemsSource = _listItems;
            listView.selectionType = SelectionType.Single;
            listView.fixedItemHeight = 22;

            listView.makeItem = () =>
            {
                var label = new Label();
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.paddingLeft = 4;
                return label;
            };

            listView.bindItem = (element, index) =>
            {
                if (element is Label label)
                {
                    label.text = _listItems[index];
                }
            };

            listView.onSelectionChange -= OnListSelectionChanged;
            listView.onSelectionChange += OnListSelectionChanged;

            listView.Rebuild();
        }

        private void OnListSelectionChanged(IEnumerable<object> selectedItems)
        {
            foreach (var item in selectedItems)
            {
                Debug.Log($"ListView selected: {item}");
            }
        }

        private void SetupTreeView(VisualElement root)
        {
            var treeView = root.Q<TreeView>("sample-treeview");
            if (treeView == null)
                return;

            treeView.fixedItemHeight = 22;
            treeView.selectionType = SelectionType.Single;

            treeView.SetRootItems(_treeItems);

            treeView.makeItem = () =>
            {
                var label = new Label();
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.paddingLeft = 4;
                return label;
            };

            treeView.bindItem = (element, index) =>
            {
                if (element is Label label)
                {
                    label.text = treeView.GetItemDataForIndex<string>(index);
                }
            };

            treeView.onSelectionChange -= OnTreeSelectionChanged;
            treeView.onSelectionChange += OnTreeSelectionChanged;

            treeView.Rebuild();
            treeView.ExpandAll();
        }

        private void OnTreeSelectionChanged(IEnumerable<object> selectedItems)
        {
            foreach (var item in selectedItems)
            {
                Debug.Log($"TreeView selected: {item}");
            }
        }

        private void SetupButtons(VisualElement root)
        {
            var messageButton = root.Q<Button>("message-button");
            var confirmButton = root.Q<Button>("confirm-button");

            if (messageButton != null)
            {
                messageButton.clicked -= OnMessageButtonClicked;
                messageButton.clicked += OnMessageButtonClicked;
            }

            if (confirmButton != null)
            {
                confirmButton.clicked -= OnConfirmButtonClicked;
                confirmButton.clicked += OnConfirmButtonClicked;
            }
        }

        private void OnMessageButtonClicked()
        {
            Debug.Log("Show Message clicked");
        }

        private void OnConfirmButtonClicked()
        {
            Debug.Log("Show Confirm clicked");
        }
    }
}