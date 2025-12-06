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
            var labels = new Dictionary<ResourceType, Label>();

            /*foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                var label = towerGroup.Q<Label>($"{id}{type}Count");
                if (label != null)
                {
                    label.text = towerData.cost.GetAmount(type).ToString("D2");
                    labels[type] = label;
                }
            }*/

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
    }
    public void OpenBuildMenu()
    {
        UIManager.OpenMenu(buildMenu);

        if (CastleController == null)
            Debug.LogError("BuildMenuController: CastleController is null!");

        if (CastleController?.stockpile == null)
            Debug.LogError("BuildMenuController: CastleController.stockpile is null!");

        /*foreach (var kvp in _buildTowers)
        {
            if (kvp.Value.resourceLabels == null)
            {
                Debug.LogWarning($"BuildMenuController: resourceLabels for {kvp.Key} is null!");
                continue;
            }

            foreach (var labelKvp in kvp.Value.resourceLabels)
            {
                if (labelKvp.Value == null)
                {
                    Debug.LogWarning($"BuildMenuController: label for {labelKvp.Key} on {kvp.Key} is null!");
                    continue;
                }

                labelKvp.Value.style.color = new StyleColor(
                    CastleController.stockpile.GetAmount(labelKvp.Key) >= kvp.Value.cost.GetAmount(labelKvp.Key)
                        ? Color.white
                        : Color.gray
                );
            }
        }*/
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
        if (SelectionManager.instance.GetCurrent() is TowerTile tile)
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
    }
}
