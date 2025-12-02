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

        button.clicked -= OnClick; //Make sure any previous instance is removed
        button.clicked += OnClick;

        if (action.icon != null && action.icon.texture != null)
            icon.style.backgroundImage = new StyleBackground(action.icon.texture);
        else
            Debug.LogError($"Action '{action?.name}' has no icon or texture!");
    }

    private void OnClick()
    {
        if (_building.TryUpgrade(_index))
        {
            UIManager.instance.RefreshUpgradeMenu(_building);
        }
        else
        {
            UIManager.instance.ShowUpgradeError(_building.GetMissingResources(_building.actions[_index].levelCosts[_index]));
        }
    }

    public void UpdateDisplay(BuildingUpgradeAction action)
    {
        icon.style.backgroundImage = new StyleBackground(action.icon.texture);

        this.action = action;
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
    [SerializeField] private Dictionary<int, Resources> _upgradeData;
    private Dictionary<UpgradeType, int> _currentUpgradeLevels = new();

    public BuildingUpgradeAction[] actions;
    protected Resources Stockpile => CastleController.instance.stockpile;

    public Dictionary<int, Resources> GetUpgradeData() => _upgradeData;
    public List<BuildingActionData> GetActions() => _actions;
    public towerID ID { get => id; }

    public int GetCurrentUpgradeLevel(UpgradeType type) => _currentUpgradeLevels.ContainsKey(type) ? _currentUpgradeLevels[type] : 0;

    [Header("BuildingMenu")]
    [SerializeField] protected Sprite _icon1;
    [SerializeField] protected Sprite _icon2;
    [SerializeField] protected Sprite _icon3;
    [SerializeField] protected Sprite _icon4;
    [SerializeField] protected string _name1;
    [SerializeField] protected string _name2;
    [SerializeField] protected string _name3;
    [SerializeField] protected string _name4;
    [SerializeField] protected string _description1;
    [SerializeField] protected string _description2;
    [SerializeField] protected string _description3;
    [SerializeField] protected string _description4;

    protected virtual void Awake()
    {
        _actions = new List<BuildingActionData>
        {
            new BuildingActionData(_name1, _description1, _icon1),
            new BuildingActionData(_name2, _description2, _icon2),
            new BuildingActionData(_name3, _description3, _icon3),
            new BuildingActionData(_name4, _description4, _icon4)
        };

        actions = new BuildingUpgradeAction[4];

        actions[0] = new BuildingUpgradeAction
        {
            name = _name1,
            description = _description1,
            icon = _icon1,
            levelCosts = TowerData.GetUpgradeLevelsCosts(id, TowerData.MapStringToUpgradeType(_name1))
        };

        actions[1] = new BuildingUpgradeAction
        {
            name = _name2,
            description = _description2,
            icon = _icon2,
            levelCosts = TowerData.GetUpgradeLevelsCosts(id, TowerData.MapStringToUpgradeType(_name2))
        };

        actions[2] = new BuildingUpgradeAction
        {
            name = _name3,
            description = _description3,
            icon = _icon3,
            levelCosts = TowerData.GetUpgradeLevelsCosts(id, TowerData.MapStringToUpgradeType(_name3))
        };

        actions[3] = new BuildingUpgradeAction
        {
            name = _name4,
            description = _description4,
            icon = _icon4,
            levelCosts = TowerData.GetUpgradeLevelsCosts(id, TowerData.MapStringToUpgradeType(_name4))
        };

        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            _currentUpgradeLevels[type] = 0;
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
        throw new NotImplementedException();
    }

    public virtual void OnSelect()
    {
        throw new NotImplementedException();
    }
    public void IncrementUpgradeLevel(UpgradeType type)
    {
        if (_currentUpgradeLevels[type] < TowerData.GetMaxLevels(id, type))
            _currentUpgradeLevels[type]++;
    }

    public bool TryUpgrade(int index)
    {
        var action = actions[index];

        // Prevent going past max
        if (action.CurrentLevel >= action.maxLevel)
            return false;

        Resources cost = action.levelCosts[action.CurrentLevel];

        // Not enough resources?
        if (!Stockpile.HasResources(cost))
            return false;

        // Apply effect
        action.applyEffect?.Invoke();

        // Increase level
        action.CurrentLevel++;

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