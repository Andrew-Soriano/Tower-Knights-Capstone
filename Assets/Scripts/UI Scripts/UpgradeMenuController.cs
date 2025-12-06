using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeMenuController : MonoBehaviour
{

    public VisualElement upgradeMenu;
    private Dictionary<RotateDirection, Button> _rotateButtons;
    private enum RotateDirection { Left, Right }
    private Dictionary<int, UIBuildingActionButton> _upgradeButtons;
    private VisualElement _hoverMenu;
    private Label _hoverMenuName;
    private Label _hoverMenuDescription;
    private Button _targetModeButton;

    public UIManager UIManager { get; set; }
    public CastleController CastleController { get; set; }
    public Button HoveredButton { get; private set; }

    private void Awake()
    {
        UIManager = UIManager ?? UIManager.instance;
        CastleController = CastleController ?? CastleController.instance;

        upgradeMenu = UIManager.Root.Q<VisualElement>("Upgrade");
        upgradeMenu.style.display = DisplayStyle.None;

        _rotateButtons = new Dictionary<RotateDirection, Button>
        {
                { RotateDirection.Left, upgradeMenu.Q<Button>("RotateLeft") },
                { RotateDirection.Right, upgradeMenu.Q<Button>("RotateRight") }
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

        _hoverMenu = upgradeMenu.Q<VisualElement>("UpgradeHoverMenu");
        _hoverMenu.style.display = DisplayStyle.None;
        _hoverMenuName = _hoverMenu.Q<Label>("UpgradeHoverName");
        _hoverMenuDescription = _hoverMenu.Q<Label>("UpgradeHoverDescription");
    }

    private void OnEnable()
    {
        //Rotate buttons
        foreach (var kvp in _rotateButtons)
        {
            RotateDirection dir = kvp.Key;
            UIManager.RegisterSafeClick(kvp.Value, () => OnTowerRotateClicked(dir));
        }

        //Upgrade Buttons
        foreach (var kvp in _upgradeButtons)
        {
            var upgradeButton = kvp.Value.button;

            EventCallback<PointerEnterEvent> enterCallback = evt => ShowHoverMenu(upgradeButton, kvp.Value);
            EventCallback<PointerLeaveEvent> leaveCallback = evt => HideHoverMenu();

            upgradeButton.RegisterCallback<PointerEnterEvent>(enterCallback);
            upgradeButton.RegisterCallback<PointerLeaveEvent>(leaveCallback);

            UIManager._hoverEnterCallbacks[upgradeButton] = enterCallback;
            UIManager._hoverLeaveCallbacks[upgradeButton] = leaveCallback;

            UIManager.RegisterSafeClick(upgradeButton, () => kvp.Value.action?.applyEffect?.Invoke());
        }

        UIManager.RegisterSafeClick(_targetModeButton, UIManager.EnterTargetingMode);
    }

    private void OnDisable()
    {

        foreach (var kvp in _upgradeButtons)
        {
            var button = kvp.Value.button;

            if (UIManager._hoverEnterCallbacks.TryGetValue(button, out var enterCb))
                button.UnregisterCallback(enterCb);

            if (UIManager._hoverLeaveCallbacks.TryGetValue(button, out var leaveCb))
                button.UnregisterCallback(leaveCb);
        }
    }

    public void PopulateUpgradeMenu(BuildingBase tower)
    {
        var towerData = TowerDatabase.Instance.GetTower(tower.ID);

        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            if (i < towerData.upgrades.Count)
            {
                var upgradeData = towerData.upgrades[i];
                var button = _upgradeButtons[i];
                button.button.style.display = DisplayStyle.Flex;

                var action = tower.actions[i];
                action.name = upgradeData.upgrade.name;
                action.description = upgradeData.upgrade.description;
                action.icon = upgradeData.upgrade.icon;
                action.levelCosts = upgradeData.levelCosts;

                button.Setup(tower, i);
                button.UpdateDisplay(action);
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

        for (int i = 0; i < building.actions.Length; i++)
        {
            var act = building.actions[i];
            var uiButton = _upgradeButtons[i];

            bool canUpgrade = act.CurrentLevel < act.maxLevel;
            uiButton.SetInteractable(canUpgrade);

            if (canUpgrade)
            {
                uiButton.icon.RemoveFromClassList("disabled");
            }
            else
            {
                uiButton.icon.AddToClassList("disabled");
            }

            uiButton.UpdateDisplay(act);
        }

    }

    public void ShowUpgradeError(Resources missing)
    {
        UIManager.StatusBarContoller.FlashMissingLabel(missing);
    }

    public void OpenUpgradeMenu(BuildingBase building)
    {
        PopulateUpgradeMenu(building);
        UIManager.OpenMenu(upgradeMenu);
        if (building is TowerCatapult || building is TowerAlchemist)
            _targetModeButton.style.display = DisplayStyle.Flex;
        else
            _targetModeButton.style.display = DisplayStyle.None;
    }

    private void ShowHoverMenu(VisualElement button, UIBuildingActionButton actionButton)
    {
        if (actionButton == null || actionButton.action == null) return;

        HoveredButton = actionButton.button;

        var upgrade = actionButton.action;
        _hoverMenuName.text = upgrade.name;
        _hoverMenuDescription.text = upgrade.description;

        UIManager.PopulateHoverCost(upgrade.CurrentLevel >= upgrade.maxLevel ? null : upgrade.levelCosts[upgrade.CurrentLevel],
                                    upgrade,
                                    _hoverMenu.Q<VisualElement>("HoverCostIcons"));

        _hoverMenu.style.display = DisplayStyle.Flex;
    }

    private void HideHoverMenu()
    {
        HoveredButton = null;
        _hoverMenu.style.display = DisplayStyle.None;
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
}
