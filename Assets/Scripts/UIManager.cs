using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private bool ignoreNextUIInput = false;

    private VisualElement root;
    public VisualElement currentMenu;


    //Status Bar
    public VisualElement statusBar;
    private Button _nextRoundButton;
    private Label _roundCountText;
    private Dictionary<ResourceType, Label> _resourceLabels;
    private Label _castleHPLabel;
    public static event Action nextRoundClicked;

    //Build Menu
    public VisualElement buildMenu;
    public VisualElement towerGrid;
    private TabView _buildTabView;
    private Dictionary<towerID, (Button button, Dictionary<ResourceType, Label> resourceLabels, Resources cost)> _buildTowers;
    private VisualElement _buildHoverMenu;
    private Label _buildHoverName;
    private Label _buildHoverDescription;

    //Resource Builder Menu
    public VisualElement resourceMenu;
    private Button _resourceButton;

    //Upgrade Menu
    public VisualElement upgradeMenu;
    private Dictionary<RotateDirection, Button> _rotateButtons;
    private enum RotateDirection { Left, Right }
    private Dictionary<int, UIBuildingActionButton> _upgradeButtons;
    private VisualElement _hoverMenu;
    private Label _hoverMenuName;
    private Label _hoverMenuDescription;
    private Dictionary<VisualElement, EventCallback<PointerEnterEvent>> _hoverEnterCallbacks = new ();
    private Dictionary<VisualElement, EventCallback<PointerLeaveEvent>> _hoverLeaveCallbacks = new ();
    private Dictionary<Button, Action> _safeClickHandlers = new();
    public Button HoveredButton { get; private set; }
    private Button _targetModeButton;


    //Castle Menu
    public VisualElement castleMenu;

    private Dictionary<Label, Coroutine> _activeFlashes = new();

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
        _castleHPLabel = statusBar.Q<Label>("HPCount");
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
        towerGrid = buildMenu.Q<VisualElement>("TowerGrid");
        _buildTowers = new Dictionary<towerID, (Button button, Dictionary<ResourceType, Label> resourceLabels, Resources cost)>();

        foreach (towerID id in Enum.GetValues(typeof(towerID)))
        {
            Resources cost = TowerData.GetCosts(id);

            var towerGroup = buildMenu.Q<VisualElement>($"{id}Group") ?? buildMenu.Q<VisualElement>($"{id}");
            if (towerGroup == null)
            {
                Debug.LogWarning($"UIManager: Could not find UI group for {id}");
                continue;
            }

            var button = towerGroup.Q<Button>($"Build{id}Button");
            if (button == null)
            {
                Debug.LogWarning($"UIManager: Could not find button for {id}");
                continue;
            }
            var labels = new Dictionary<ResourceType, Label>();

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (cost.GetAmount(type) <= 0) continue;

                var label = towerGroup.Q<Label>($"{id}{type}Count");
                if (label != null)
                {
                    label.text = cost.GetAmount(type).ToString("D2");
                    labels[type] = label;
                }
            }

            _buildTowers[id] = (button, labels, cost);
        }

        _buildHoverMenu = Q<VisualElement>("BuildHoverMenu", buildMenu);
        _buildHoverName = Q<Label>("BuildHoverName", _buildHoverMenu);
        _buildHoverDescription = Q<Label>("BuildHoverDescription", _buildHoverMenu);
        _buildHoverMenu.style.display = DisplayStyle.None;


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
            { 0, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton1")) },
            { 1, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton2")) },
            { 2, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton3")) },
            { 3, new UIBuildingActionButton(upgradeMenu.Q<VisualElement>("UpgradeButton4")) }
        };
        _targetModeButton = new Button()
        {
            text = "Target Mode"
        };
        _targetModeButton.style.display = DisplayStyle.None;
        upgradeMenu.Add(_targetModeButton);
        _hoverMenu = Q<VisualElement>("UpgradeHoverMenu", upgradeMenu);
        _hoverMenu.style.display = DisplayStyle.None;
        _hoverMenuName = Q<Label>("UpgradeHoverName", _hoverMenu);
        _hoverMenuDescription = Q<Label>("UpgradeHoverDescription", _hoverMenu);

        //Castle Menu Initialize
        castleMenu = root.Q<VisualElement>("Castle");

        _input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _safeClickHandlers.Clear();
        _hoverEnterCallbacks.Clear();
        _hoverLeaveCallbacks.Clear();

        //Build buttons
        foreach (var kvp in _buildTowers)
        {
            towerID id = kvp.Key;
            RegisterSafeClick(kvp.Value.button, () => OnBuildTowerPressed(id));
            kvp.Value.button.RegisterCallback<PointerEnterEvent>(evt => ShowBuildHoverMenu(id, kvp.Value.button));
            kvp.Value.button.RegisterCallback<PointerLeaveEvent>(evt => HideBuildHoverMenu());
        }

        RegisterSafeClick(_resourceButton, OnBuildResourcePress);      //Resource Button
        RegisterSafeClick(_nextRoundButton, OnNextRoundButtonClicked); //Next Round Button

        //Rotate buttons
        foreach (var kvp in _rotateButtons)
        {
            RotateDirection dir = kvp.Key;
            RegisterSafeClick(kvp.Value, () => OnTowerRotateClicked(dir));
        }

        //Upgrade Buttons
        foreach (var kvp in _upgradeButtons)
        {
            var upgradeButton = kvp.Value.button;

            EventCallback<PointerEnterEvent> enterCallback = evt => ShowHoverMenu(upgradeButton, kvp.Value);
            EventCallback<PointerLeaveEvent> leaveCallback = evt => HideHoverMenu();

            upgradeButton.RegisterCallback<PointerEnterEvent>(enterCallback);

            upgradeButton.RegisterCallback<PointerLeaveEvent>(leaveCallback);

            _hoverEnterCallbacks[upgradeButton] = enterCallback;
            _hoverLeaveCallbacks[upgradeButton] = leaveCallback;

            RegisterSafeClick(upgradeButton, () => kvp.Value.action?.applyEffect?.Invoke());
        }
        RegisterSafeClick(_targetModeButton, EnterTargetingMode);

        EnemyBaseController.endOfRound += EndRoundUI;
    }

    private void OnDisable()
    {
        //Remove all SafeButtonClicks
        foreach (var kvp in _safeClickHandlers)
        {
            kvp.Key.clicked -= kvp.Value;
        }

        _safeClickHandlers.Clear();

        //Remove hovers
        foreach (var kvp in _upgradeButtons)
        {
            var button = kvp.Value.button;

            if (_hoverEnterCallbacks.TryGetValue(button, out var enterCb))
                button.UnregisterCallback(enterCb);

            if (_hoverLeaveCallbacks.TryGetValue(button, out var leaveCb))
                button.UnregisterCallback(leaveCb);
        }

        _hoverEnterCallbacks.Clear();
        _hoverLeaveCallbacks.Clear();

        //Stop active flash coroutines
        foreach (var kvp in _activeFlashes)
            StopCoroutine(kvp.Value);
        
        _activeFlashes.Clear();

        EnemyBaseController.endOfRound -= EndRoundUI;
    }
    private void RegisterSafeClick(Button button, Action action)
    {
        if (_safeClickHandlers.TryGetValue(button, out var existing))
            button.clicked -= existing;

        _safeClickHandlers[button] = () => SafeButtonClick(action);
        button.clicked += _safeClickHandlers[button];
    }

    private T Q<T>(string name, VisualElement parent = null) where T : VisualElement
    {
        var element = (parent ?? root).Q<T>(name);
        if (element == null)
            Debug.LogError($"UIManager: Failed to find {typeof(T).Name} named '{name}' in {(parent != null ? parent.name : "root")}");
        return element;
    }

    private void ShowBuildHoverMenu(towerID id, VisualElement button)
    {
        var towerData = TowerData.GetTowerInfo(id); // Your struct/class containing name, desc, costs
        _buildHoverName.text = towerData.name;
        _buildHoverDescription.text = towerData.description;

        // Populate costs
        PopulateHoverCost(towerData.cost, null, _buildHoverMenu.Q<VisualElement>("BuildHoverCostIcons"));

        _buildHoverMenu.style.display = DisplayStyle.Flex;

        // Optional: position near button
        var worldPos = button.worldBound.position;
        _buildHoverMenu.style.left = worldPos.x;
        _buildHoverMenu.style.top = worldPos.y + button.resolvedStyle.height;
    }

    private void HideBuildHoverMenu()
    {
        _buildHoverMenu.style.display = DisplayStyle.None;
    }

    // Overload of PopulateHoverCost to specify a container
    private void PopulateHoverCost(Resources cost, BuildingUpgradeAction action, VisualElement container)
    {
        if (cost == null || container == null) return;

        container.Clear();

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int amount = cost.GetAmount(type);
            if (amount <= 0) continue;

            var group = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center, marginRight = 6 } };

            var icon = new VisualElement
            {
                style =
            {
                width = 20,
                height = 20,
                backgroundImage = new StyleBackground(GetResourceIcon(type)),
                marginRight = 2
            }
            };

            var label = new Label(amount.ToString("D2"))
            {
                style =
            {
                fontSize = 12,
                color = Color.white,
                unityFontStyleAndWeight = FontStyle.Bold
            }
            };

            group.Add(icon);
            group.Add(label);
            container.Add(group);
        }
    }


    public void PopulateUpgradeMenu(BuildingBase tower)
    {
        var actions = tower.actions;

        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            if (i < actions.Length)
            {
                var button = _upgradeButtons[i];
                button.button.style.display = DisplayStyle.Flex;

                button.Setup(tower, i);
                button.UpdateDisplay(actions[i]);
            }
            else
            {
                _upgradeButtons[i].button.style.display = DisplayStyle.None;
            }
        }
    }


    public void RefreshUpgradeMenu(BuildingBase building)
    {
        var actions = building.actions;

        for (int i = 0; i < actions.Length; i++)
            _upgradeButtons[i].UpdateDisplay(actions[i]);
    }

    public void ShowUpgradeError(Resources missing)
    {
        FlashMissingLabel(missing);
    }


    public void OpenMenu(VisualElement menu)
    {
        if (currentMenu != null)
            CloseMenu();
        currentMenu = menu;
        currentMenu.style.display = DisplayStyle.Flex;

        _input.SwitchCurrentActionMap("Menu");

        // Ignore any UI click on this frame
        ignoreNextUIInput = true;
        StartCoroutine(DisarmButtonSafety());
    }

    private void SafeButtonClick(Action action)
    {
        if (ignoreNextUIInput)
        {
            ignoreNextUIInput = false; // consume the “opening click”
            return;
        }

            action?.Invoke();
    }

    private IEnumerator DisarmButtonSafety()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        ignoreNextUIInput = false;
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

    public void OpenUpgradeMenu(BuildingBase building)
    {
        PopulateUpgradeMenu(building);
        OpenMenu(upgradeMenu);
        if (building is TowerCatapult)
            _targetModeButton.style.display = DisplayStyle.Flex;
        else
            _targetModeButton.style.display = DisplayStyle.None;
    }

    private void EnterTargetingMode()
    {
        currentMenu.style.display = DisplayStyle.None;
        currentMenu = null;
        _input.SwitchCurrentActionMap("Camera");
    }

    private void ShowHoverMenu(VisualElement button, UIBuildingActionButton actionButton)
    {
        if (actionButton == null || actionButton.action == null) return;

        HoveredButton = actionButton.button;

        var upgrade = actionButton.action;

        _hoverMenuName.text = upgrade.name;
        _hoverMenuDescription.text = upgrade.description;

        PopulateHoverCost(upgrade.CurrentLevel >= upgrade.maxLevel ? null : upgrade.levelCosts[upgrade.CurrentLevel], upgrade);

        _hoverMenu.style.display = DisplayStyle.Flex;
    }

    private void HideHoverMenu()
    {
        HoveredButton = null;
        _hoverMenu.style.display = DisplayStyle.None;
    }

    public void PopulateHoverCost(Resources cost, BuildingUpgradeAction action)
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
            _input.SwitchCurrentActionMap("Camera");

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

    public void RefreshCastleHP(int currentHP, int maxHP)
    {
        if (_castleHPLabel != null)
        {
            _castleHPLabel.text = $"{currentHP}/{maxHP}";
        }
    }

    public void FlashLabel(Label label, Color color, float duration = 0.25f, int repeat = 3)
    {
        if (_activeFlashes.TryGetValue(label, out Coroutine existing))
            StopCoroutine(existing);
        _activeFlashes[label] = StartCoroutine(FlashCoroutine(label, duration, repeat, color));
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

    public void FlashCastleHP()
    {
        FlashLabel(_castleHPLabel, Color.red);
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

        _activeFlashes.Remove(label);
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
