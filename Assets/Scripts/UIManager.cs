using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private VisualElement root;
    private VisualElement currentMenu;


    //Status Bar
    public VisualElement statusBar;
    private Button _nextRoundButton;
    private Label _roundCountText;
    private Dictionary<ResourceType, Label> _resourceLabels;
    public static event Action nextRoundClicked;

    //Build Menu
    public VisualElement buildMenu;
    private TabView _buildTabView;
    private Dictionary<towerID, (Button button, Dictionary<ResourceType, Label> resourceLabels, Resources cost)> _buildTowers;
    private Dictionary<towerID, Action> _buildTowerActions;

    //Resource Builder Menu
    public VisualElement resourceMenu;
    private Button _resourceButton;

    //Upgrade Menu
    public VisualElement upgradeMenu;
    private Dictionary<RotateDirection, Button> _rotateButtons;
    private Dictionary<RotateDirection, Action> _rotateActions;
    private enum RotateDirection { Left, Right }
    private Dictionary<int, UIBuildingActionButton> _upgradeButtons;
    private VisualElement _hoverMenu;
    private Label _hoverMenuName;
    private Label _hoverMenuDescription;
    private Dictionary<VisualElement, EventCallback<PointerEnterEvent>> _hoverEnterCallbacks = new ();
    private Dictionary<VisualElement, EventCallback<PointerLeaveEvent>> _hoverLeaveCallbacks = new ();
    public Button HoveredButton { get; private set; }


    //Castle Menu
    public VisualElement castleMenu;

    private InputAction _clickAction;
    private InputAction _pointAction;
    private InputAction _cancelAction;
    private PlayerInput _input;

    private void Awake()
    {
        //Singleton pattern
        if (instance == null)
        {
            instance = this;
        } else { 
            Destroy(gameObject);
        }

        root = GetComponent<UIDocument>().rootVisualElement;

        //Status Bar Initialize
        statusBar = root.Q<VisualElement>("StatusBar");
        _nextRoundButton = statusBar.Q<Button>("NextRoundButton");
        _roundCountText = statusBar.Q<Label>("RoundCount");
        _resourceLabels = new Dictionary<ResourceType, Label>
        {
            { ResourceType.Wood, statusBar.Q<Label>("WoodCount") },
            { ResourceType.Planks, statusBar.Q<Label>("PlanksCount") },
            { ResourceType.Stone, statusBar.Q<Label>("StoneCount") },
            { ResourceType.Bricks, statusBar.Q<Label>("BricksCount") },
            { ResourceType.Ore, statusBar.Q<Label>("OreCount") },
            { ResourceType.Metal, statusBar.Q<Label>("MetalCount") },
            { ResourceType.Parts, statusBar.Q<Label>("PartsCount") }
        };

        //Build Menu Initialize
        buildMenu = root.Q<VisualElement>("Build");
        buildMenu.style.display = DisplayStyle.None;
        _buildTabView = buildMenu.Q<TabView>("BuildTabView");
        _buildTabView.activeTab = _buildTabView.GetTab(0);
        _buildTowers = new Dictionary<towerID, (Button button, Dictionary<ResourceType, Label> resourceLabels, Resources cost)>();
        foreach(towerID id in Enum.GetValues(typeof(towerID)))
        {
            Resources cost = TowerData.GetCosts(id);
            Button button = Q<Button>($"Build{id}Button");
            var labels = new Dictionary<ResourceType, Label>();

            foreach(ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if(cost.GetAmount(type) > 0)
                {
                    labels[type] = Q<Label>($"{id}{type}Count", buildMenu);
                    labels[type].text = cost.GetAmount(type).ToString("D2");
                }
            }

            _buildTowers[id] = (button, labels, cost);
        }

        //Resource Tile Menu Initialize
        resourceMenu = root.Q<VisualElement>("Resource");
        resourceMenu.style.display = DisplayStyle.None;
        _resourceButton = resourceMenu.Q<Button>("BuildResourceButton");

        //Upgrade Menu Initialize
        upgradeMenu = root.Q<VisualElement>("Upgrade");
        upgradeMenu.style.display = DisplayStyle.None;
        _rotateButtons = new Dictionary<RotateDirection, Button>
        {
                { RotateDirection.Left, Q<Button>("RotateLeft", upgradeMenu) },
                { RotateDirection.Right, Q<Button>("RotateRight", upgradeMenu) }
        };
        _upgradeButtons = new Dictionary<int, UIBuildingActionButton>
        {
            { 0, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton1"), null) },
            { 1, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton2"), null) },
            { 2, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton3"), null) },
            { 3, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton4"), null) }
        };
        _hoverMenu = Q<VisualElement>("UpgradeHoverMenu", upgradeMenu);
        _hoverMenu.style.display = DisplayStyle.None;
        _hoverMenuName = Q<Label>("UpgradeHoverName", _hoverMenu);
        _hoverMenuDescription = Q<Label>("UpgradeHoverDescription", _hoverMenu);

        //Castle Menu Initialize
        castleMenu = root.Q<VisualElement>("Castle");

        _input = GetComponent<PlayerInput>();

        _pointAction = _input.actions["Point"];
        _clickAction = _input.actions["Click"];
        _cancelAction = _input.actions["Cancel"];
    }

    private void OnEnable()
    {
        _buildTowerActions = new Dictionary<towerID, Action>();

        foreach (var kvp in _buildTowers)
        {
            towerID id = kvp.Key;
            Action handler = () => OnBuildTowerPressed(id);
            _buildTowerActions[id] = handler;
            kvp.Value.button.clicked += handler;
        }

        _resourceButton.clicked += OnBuildResourcePress;
        _nextRoundButton.clicked += OnNextRoundButtonClicked;

        _rotateActions = new Dictionary<RotateDirection, Action>();
        foreach (var kvp in _rotateButtons)
        {
            RotateDirection dir = kvp.Key;
            Action handler = () => OnTowerRotateClicked(dir);
            _rotateActions[dir] = handler;
            kvp.Value.clicked += handler;
        }

        foreach (var kvp in _upgradeButtons)
        {
            var upgradeButton = kvp.Value.button;

            // Capture local references for lambda
            int index = kvp.Key;

            EventCallback<PointerEnterEvent> enterCallback = evt => ShowHoverMenu(upgradeButton, kvp.Value);
            EventCallback<PointerLeaveEvent> leaveCallback = evt => HideHoverMenu();

            upgradeButton.RegisterCallback<PointerEnterEvent>(enterCallback);

            upgradeButton.RegisterCallback<PointerLeaveEvent>(leaveCallback);

            _hoverEnterCallbacks[upgradeButton] = enterCallback;
            _hoverLeaveCallbacks[upgradeButton] = leaveCallback;
        }
    }

    private void OnDisable()
    {
        //Build Menu Events
        foreach (var kvp in _buildTowers)
        {
            towerID id = kvp.Key;
            kvp.Value.button.clicked -= _buildTowerActions[id];
        }

        //Resource Menu Events
        _resourceButton.clicked -= OnBuildResourcePress;

        //Tower Menu Events
        foreach (var kvp in _rotateButtons)
        {
            kvp.Value.clicked -= _rotateActions[kvp.Key];
        }

        //Statusbar Events
        _nextRoundButton.clicked -= OnNextRoundButtonClicked;

        foreach (var kvp in _upgradeButtons)
        {
            var button = kvp.Value.button;

            if (_hoverEnterCallbacks.TryGetValue(button, out var enterCb))
                button.UnregisterCallback(enterCb);

            if (_hoverLeaveCallbacks.TryGetValue(button, out var leaveCb))
                button.UnregisterCallback(leaveCb);
        }
    }

    private T Q<T>(string name, VisualElement parent = null) where T : VisualElement
    {
        return (parent ?? root).Q<T>(name);
    }

    public void PopulateUpgradeMenu(IBuildingActions building)
    {
        List<BuildingActionData> actions = building.GetActions();
        Dictionary<int, Resources> upgradeData = building.GetUpgradeData();

        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            if (i < actions.Count)
            {
                var action = actions[i];
                UpgradeType type = TowerData.MapStringToUpgradeType(action.name);
                int currentLevel = building.GetCurrentUpgradeLevel(type);

                Resources cost = TowerData.GetUpgradeCost(building.ID, type, currentLevel);

                if (cost == null)
                {
                    _upgradeButtons[i].button.SetEnabled(false);
                    _upgradeButtons[i].button.tooltip = "Max Level Reached";
                }
                else
                {
                    _upgradeButtons[i].button.SetEnabled(true);
                    _upgradeButtons[i].SetData(action, cost);
                }

                _upgradeButtons[i].button.style.display = DisplayStyle.Flex;
            }
            else
            {
                _upgradeButtons[i].button.style.display = DisplayStyle.None;
            }
        }
    }

    public void OpenMenu(VisualElement menu)
    {
        if (currentMenu != null)
            CloseMenu();
        currentMenu = menu;
        currentMenu.style.display = DisplayStyle.Flex;
    }

    public void OpenBuildMenu()
    {
        OpenMenu(buildMenu);

        foreach (towerID id in Enum.GetValues(typeof(towerID)))
        {
            foreach (var kvp in _buildTowers[id].resourceLabels)
            {
                ResourceType type = kvp.Key;
                Label label = kvp.Value;
                label.style.color = new StyleColor(CastleController.instance.stockpile.GetAmount(type) >= _buildTowers[id].cost.GetAmount(type)
                    ? Color.white : Color.gray);
            }
        }
    }

    public void OpenResourceMenu(ResourceTile tile)
    {
        OpenMenu(resourceMenu);
        _resourceButton.text = tile.tileName;
    }

    public void OpenUpgradeMenu(IBuildingActions building)
    {
        PopulateUpgradeMenu(building);
        OpenMenu(upgradeMenu);
    }

    private void ShowHoverMenu(VisualElement button, UIBuildingActionButton actionButton)
    {
        if (actionButton == null || actionButton.BuildingData == null) return;

        HoveredButton = actionButton.button;

        var data = actionButton.BuildingData;

        _hoverMenuName.text = actionButton.BuildingData.name;
        _hoverMenuDescription.text = actionButton.BuildingData.description;

        PopulateHoverCost(actionButton.upgradeCost);

        _hoverMenu.style.display = DisplayStyle.Flex;
    }

    private void HideHoverMenu()
    {
        HoveredButton = null;
        _hoverMenu.style.display = DisplayStyle.None;
    }

    public void PopulateHoverCost(Resources cost)
    {
        if (cost == null)
        {
            Debug.LogError("PopulateHoverCost received NULL cost!");
            return;
        }
        var container = _hoverMenu.Q<VisualElement>("HoverCostIcons");
        container.Clear();

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int amount = cost.GetAmount(type);
            if (amount <= 0)
                continue;

            // Cost group container
            var group = new VisualElement();
            group.style.flexDirection = FlexDirection.Row;
            group.style.alignItems = Align.Center;
            group.style.marginRight = 6;

            // Resource icon
            var icon = new VisualElement();
            icon.style.width = 20;
            icon.style.height = 20;
            icon.style.backgroundImage = new StyleBackground(GetResourceIcon(type));
            icon.style.marginRight = 2;

            // Amount
            var label = new Label(amount.ToString("D2"));
            label.style.fontSize = 12;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = Color.white;

            group.Add(icon);
            group.Add(label);
            container.Add(group);
        }
    }

    public void RefreshHoverText(string name, string description)
    {
        _hoverMenuName.text = name;
        _hoverMenuDescription.text = description;
    }

    private Texture2D GetResourceIcon(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Wood"),
            ResourceType.Planks => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Planks"),
            ResourceType.Stone => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Stone"),
            ResourceType.Bricks => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Bricks"),
            ResourceType.Ore => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Ore"),
            ResourceType.Metal => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Metal"),
            ResourceType.Parts => UnityEngine.Resources.Load<Texture2D>("Icons/Resources/Icon_Parts"),
            _ => null
        };
    }

    public void OpenCastleMenu()
    {

    }

    public void CloseMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.style.display = DisplayStyle.None;
            currentMenu = null;
            
            // Deselect current selectable
            if (SelectionManager.instance.GetCurrent() != null)
                SelectionManager.instance.Deselect();
        }
    }

    public void StartRoundUI(int roundNum)
    {
        _nextRoundButton.visible = false;

        _roundCountText.text = roundNum.ToString("D2");
    }

    public void EndRoundUI()
    {
        _nextRoundButton.visible = true;
    }

    public void RefreshResourceUI(Resources stockpile)
    {
        foreach (var kvp in _resourceLabels)
        {
            ResourceType type = kvp.Key;
            Label label = kvp.Value;

            int amount = 0;
            switch (type)
            {
                case ResourceType.Wood: amount = stockpile.Wood; break;
                case ResourceType.Planks: amount = stockpile.Planks; break;
                case ResourceType.Stone: amount = stockpile.Stone; break;
                case ResourceType.Bricks: amount = stockpile.Bricks; break;
                case ResourceType.Ore: amount = stockpile.Ore; break;
                case ResourceType.Metal: amount = stockpile.Metal; break;
                case ResourceType.Parts: amount = stockpile.Parts; break;
            }

            label.text = amount.ToString("D3");
        }
    }
    public void FlashLabel(Label label, Color color, float duration = 0.25f, int repeat = 3)
    {
        StopCoroutine(FlashCoroutine(label, duration, repeat, color));
        StartCoroutine(FlashCoroutine(label, duration, repeat, color));
    }

    private void FlashMissingLabel(Resources missing)
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int amountMissing = type switch
            {
                ResourceType.Wood => missing.Wood,
                ResourceType.Planks => missing.Planks,
                ResourceType.Stone => missing.Stone,
                ResourceType.Bricks => missing.Bricks,
                ResourceType.Ore => missing.Ore,
                ResourceType.Metal => missing.Metal,
                ResourceType.Parts => missing.Parts,
                _ => 0
            };

            if (amountMissing > 0)
                FlashLabel(_resourceLabels[type], Color.red);
        }
    }

    private IEnumerator FlashCoroutine(Label label, float duration, int repeat, Color color)
    {
        Color originalColor = label.resolvedStyle.color;
        for(int i = 0; i < repeat; i++)
        {
            label.style.color = new StyleColor(color);
            yield return new WaitForSeconds(duration);
            label.style.color = new StyleColor(originalColor);
            yield return new WaitForSeconds(duration);
        }
    }

    private void OnBuildTowerPressed(towerID id)
    {
        if (SelectionManager.instance.GetCurrent() is TowerTile tile)
        {
            if (CastleController.instance.BuildTower(tile, id))
            {
                RefreshResourceUI(CastleController.instance.stockpile);
                SelectionManager.instance.Deselect();
            }
            else
            {
                Resources missing = CastleController.instance.stockpile.MissingResources(_buildTowers[id].cost);
                FlashMissingLabel(missing);
            }
        }
    }

    private void OnBuildResourcePress()
    {
        if(SelectionManager.instance.GetCurrent() is ResourceTile tile)
        {
            tile.BuildResource();

            SelectionManager.instance.Deselect();
        }
    }

    private void OnTowerRotateClicked(RotateDirection direction)
    {
        if(SelectionManager.instance.GetCurrent() is TowerBase tower)
        {
            switch (direction)
            {
                case RotateDirection.Left:
                    tower.RotateLeft();
                    break;
                case RotateDirection.Right:
                    tower.RotateRight();
                    break;
            }
        }
    }

    public void OnNextRoundButtonClicked()
    {
        nextRoundClicked?.Invoke();
    }
}
