using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[System.Serializable]
public class BuildingActionData
{
    public string name;
    public string description;
    public Sprite icon;
    public Action action;

    public BuildingActionData(string name, string description, Sprite icon, Action action = null)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.action = action;
    }
}

[System.Serializable]
public class BuildingUpgradeAction
{
    public string name;
    public string description;
    public Sprite icon;
    public List<Resources> levelCosts;
    public int maxLevel => levelCosts.Count;

    public int CurrentLevel { get; set; } = 0;

    public Action applyEffect;
}

public class UIBuildingActionButton
{
    public Button button;
    public VisualElement icon;
    public BuildingUpgradeAction action;

    private BuildingBase _building;
    private int _index;

    public UIBuildingActionButton(VisualElement root)
    {
        button = root.Q<Button>("Button");
        icon = root.Q<VisualElement>("Icon");
    }

    public void Setup(BuildingBase building, int index)
    {
        _building = building;
        _index = index;
        action = building.actions[index];

        button.clicked -= OnClick;
        button.clicked += OnClick;

        if (action.icon != null && action.icon.texture != null)
            icon.style.backgroundImage = new StyleBackground(action.icon.texture);
        else
            Debug.LogError($"Action '{action?.name}' has no icon or texture!");
    }

    private void OnClick()
    {
        _building.TryUpgrade(_index);
    }

    public void UpdateDisplay(BuildingUpgradeAction action)
    {
        icon.style.backgroundImage = new StyleBackground(action.icon.texture);

        this.action = action;
    }
    public void SetInteractable(bool enabled)
    {
        button.SetEnabled(enabled);

        var root = button.parent;

        if (enabled)
            root.RemoveFromClassList("disabled");
        else
            root.AddToClassList("disabled");
    }
}

public interface IBuildingActions
{
    List<BuildingActionData> GetActions();
    public Dictionary<int, Resources> GetUpgradeData();
    int GetCurrentUpgradeLevel(UpgradeType type);
    towerID ID { get; }
}

public class BuildingBase : MonoBehaviour, IClickable, ISelectable, IBuildingActions
{
    [SerializeField] private GameObject _buildingModel;
    protected List<BuildingActionData> _actions;
    [SerializeField] protected int _defaultMode;
    [SerializeField] protected towerID id;
    private Dictionary<int, Resources> _upgradeData;
    private Dictionary<UpgradeType, int> _currentUpgradeLevels;

    public BuildingUpgradeAction[] actions;
    protected Resources Stockpile => CastleController.instance.stockpile;

    public Dictionary<int, Resources> GetUpgradeData() => _upgradeData;
    public List<BuildingActionData> GetActions() => _actions;
    public towerID ID { get => id; }

    public int GetCurrentUpgradeLevel(UpgradeType type) => _currentUpgradeLevels.ContainsKey(type) ? _currentUpgradeLevels[type] : 0;

    protected virtual void Awake()
    {
        var towerData = TowerDatabase.Instance.GetTower(ID);
        if (towerData == null)
        {
            Debug.LogError($"TowerDatabase has no data for {id}");
            return;
        }

        actions = new BuildingUpgradeAction[towerData.upgrades.Count];
        _actions = new List<BuildingActionData>();
        _upgradeData = new Dictionary<int, Resources>();
        _currentUpgradeLevels = new Dictionary<UpgradeType, int>();

        for (int i = 0; i < towerData.upgrades.Count; i++)
        {
            var upgradeData = towerData.upgrades[i];
            var upgradeInfo = upgradeData.upgrade;

            var action = new BuildingUpgradeAction
            {
                name = upgradeInfo.name,
                description = upgradeInfo.description,
                icon = upgradeInfo.icon,
                levelCosts = upgradeData.levelCosts,
                applyEffect = GetUpgradeAction(upgradeInfo.type)
            };

            actions[i] = action;

            _actions.Add(new BuildingActionData(action.name, action.description, action.icon, action.applyEffect));
            _upgradeData[i] = action.levelCosts.Count > 0 ? action.levelCosts[0] : null;
            _currentUpgradeLevels[upgradeInfo.type] = 0;
        }
    }

    protected virtual Action GetUpgradeAction(UpgradeType type)
    {
        return () => Debug.LogWarning($"{id} does not implement upgrade {type}");
    }

    public void RotateLeft()
    {
        _buildingModel.transform.rotation *= Quaternion.Euler(0f, -30f, 0f);
    }

    public void RotateRight()
    {
        _buildingModel.transform.rotation *= Quaternion.Euler(0f, 30f, 0f);
    }

    public virtual void OnClicked()
    {
        SelectionManager.instance.Select(this);
    }

    public virtual void OnDeselect()
    {
    }

    public virtual void OnSelect()
    {
    }

    public bool TryUpgrade(int index)
    {
        var action = actions[index];

        if (action.CurrentLevel >= action.maxLevel)
            return false;

        Resources cost = action.levelCosts[action.CurrentLevel];

        if (!Stockpile.HasResources(cost))
        {
            UIManager.instance.UpgradeMenuContoller.ShowUpgradeError(GetMissingResources(cost));
            return false;
        }

        action.applyEffect?.Invoke();

        action.CurrentLevel++;

        UIManager.instance.UpgradeMenuContoller.RefreshUpgradeMenu(this);

        return true;
    }

    public Resources GetCost(int index)
    {
        var action = actions[index];
        if (action.CurrentLevel >= action.maxLevel)
            return null;
        return action.levelCosts[action.CurrentLevel];
    }

    public Resources GetMissingResources(Resources cost)
    {
        return Stockpile.MissingResources(cost);
    }
}