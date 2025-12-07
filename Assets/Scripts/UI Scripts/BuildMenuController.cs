using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildMenuController : MonoBehaviour
{
    private VisualElement buildMenu;
    private VisualElement icon; 
    private TabView _buildTabView;
    private VisualElement _buildHoverMenu;
    private Label _buildHoverName;
    private Label _buildHoverDescription;
    /*private Dictionary<towerID, (Button button,
                                 Dictionary<ResourceType, Label> resourceLabels,
                                 Resources cost,
                                 VisualElement icon)> _buildTowers;*/
    private Dictionary<towerID, (Button button, Resources cost, VisualElement icon)> _buildTowers;

    public UIManager UIManager { get; set; }
    public CastleController CastleController { get; set; }
    public Button HoveredButton { get; private set; }

    private void Awake()
    {
        UIManager = UIManager ?? UIManager.instance;
        CastleController = CastleController ?? CastleController.instance;

        buildMenu = UIManager.Root.Q<VisualElement>("Build");
        buildMenu.style.display = DisplayStyle.None;
        _buildTabView = buildMenu.Q<TabView>("BuildTabView");
        _buildTabView.activeTab = _buildTabView.GetTab(0);

        _buildTowers = new ();

        foreach (towerID id in Enum.GetValues(typeof(towerID)))
        {
            var towerData = TowerDatabase.Instance.GetTower(id);

            var towerGroup = buildMenu.Q<VisualElement>($"{id}Group") ?? buildMenu.Q<VisualElement>($"{id}");
            if (towerGroup == null)
            {
                Debug.LogWarning($"UIManager: Could not find UI group for {id}");
                continue;
            }

            var button = towerGroup.Q<Button>("Button");
            if (button == null)
            {
                Debug.LogWarning($"UIManager: Could not find button for {id}");
                continue;
            }

            var icon = towerGroup.Q<VisualElement>("Icon");
            if (button == null)
            {
                Debug.LogWarning($"UIManager: Could not find icon for {id}");
                continue;
            }

            icon.style.backgroundImage = new StyleBackground(towerData.towerIcon.texture);

            _buildTowers[id] = (button, towerData.cost, icon);
        }

        _buildHoverMenu = buildMenu.Q<VisualElement>("BuildHoverMenu");
        _buildHoverName = _buildHoverMenu.Q<Label>("BuildHoverName");
        _buildHoverDescription = _buildHoverMenu.Q<Label>("BuildHoverDescription");
        _buildHoverMenu.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        foreach (var kvp in _buildTowers)
        {
            towerID id = kvp.Key;

            var button = kvp.Value.button;

            EventCallback<PointerEnterEvent> enterCallback = evt => ShowHoverMenu(id, button);
            EventCallback<PointerLeaveEvent> leaveCallback = evt => HideHoverMenu();

            kvp.Value.button.RegisterCallback<PointerEnterEvent>(enterCallback);
            kvp.Value.button.RegisterCallback<PointerLeaveEvent>(leaveCallback);

            UIManager._hoverEnterCallbacks[button] = enterCallback;
            UIManager._hoverLeaveCallbacks[button] = leaveCallback;

            UIManager.RegisterSafeClick(kvp.Value.button, () => OnBuildTowerPressed(id));
        }

        SelectionManager.OnSelectionSwitch += RefreshButtonState;
    }

    private void OnDisable()
    {
        SelectionManager.OnSelectionSwitch -= RefreshButtonState;
    }

    public void OpenBuildMenu()
    {
        UIManager.OpenMenu(buildMenu);

        if (CastleController == null)
            Debug.LogError("BuildMenuController: CastleController is null!");

        if (CastleController?.stockpile == null)
            Debug.LogError("BuildMenuController: CastleController.stockpile is null!");

        RefreshButtonState();
    }

    private void ShowHoverMenu(towerID id, Button button)
    {
        var towerData = TowerDatabase.Instance.GetTower(id);
        HoveredButton = button;
        _buildHoverName.text = towerData.towerName;
        _buildHoverDescription.text = towerData.description;

        UIManager.PopulateHoverCost(towerData.cost, null, _buildHoverMenu.Q<VisualElement>("BuildHoverCostIcons"));

        _buildHoverMenu.style.display = DisplayStyle.Flex;
    }

    private void HideHoverMenu()
    {
        HoveredButton = null;
        _buildHoverMenu.style.display = DisplayStyle.None;
    }

    private void OnBuildTowerPressed(towerID id)
    {
        var current = SelectionManager.instance.GetCurrent();
        if (current is TowerTile tile)
        {
            if (CastleController.BuildTower(tile, id))
            {
                UIManager.StatusBarContoller.RefreshResourceUI(CastleController.stockpile);
                SelectionManager.instance.Deselect();
            }
            else
            {
                Resources missing = CastleController.stockpile.MissingResources(_buildTowers[id].cost);
                UIManager.StatusBarContoller.FlashMissingLabel(missing);
            }
        }
        else if (current is ResourceTile resourceTile)
        {
            if (IsMatchingResourceTower(id, resourceTile.type))
            {
                if (CastleController.BuildResource(resourceTile, id))
                {
                    UIManager.StatusBarContoller.RefreshResourceUI(CastleController.stockpile);
                    SelectionManager.instance.Deselect();
                }
                else
                {
                    Resources missing = CastleController.stockpile.MissingResources(_buildTowers[id].cost);
                    UIManager.StatusBarContoller.FlashMissingLabel(missing);
                }
            }
        }
    }

    private bool IsMatchingResourceTower(towerID id, ResourceType type)
    {
        return (id == towerID.Lumberyard && type == ResourceType.Wood) ||
               (id == towerID.Quarry && type == ResourceType.Stone) ||
               (id == towerID.Mine && type == ResourceType.Ore);
    }

    private void RefreshButtonState()
    {
        foreach (var kvp in _buildTowers)
        {
            var button = kvp.Value.button;
            var id = kvp.Key;

            bool shouldEnable = true;

            var current = SelectionManager.instance.GetCurrent();

            if (current is TowerTile
                && (id == towerID.Lumberyard || id == towerID.Mine || id == towerID.Quarry))
            {
                shouldEnable = false;
            }
            else if(current is ResourceTile resource)
            {
                switch (resource.type)
                {
                    case ResourceType.Wood:
                        if (id != towerID.Lumberyard) shouldEnable = false;
                        break;

                    case ResourceType.Stone:
                        if (id != towerID.Quarry) shouldEnable = false;
                        break;

                    case ResourceType.Ore:
                        if (id != towerID.Mine) shouldEnable = false;
                        break;
                }
            }

            var group = button.parent;
            button.SetEnabled(shouldEnable);
            group.style.opacity = shouldEnable ? 1f : 0.4f;
        }
    }
}
