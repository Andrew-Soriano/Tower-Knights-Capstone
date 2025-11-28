using System;
using System.Collections.Generic;
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
public class UIBuildingActionButton
{
    public Button button;
    public VisualElement icon;
    public Resources upgradeCost;
    public BuildingActionData BuildingData { get; private set; }

    public UIBuildingActionButton(VisualElement root, Resources data)
    {
        button = root.Q<Button>("Button");
        icon = root.Q<VisualElement>("Icon");
        upgradeCost = data;
    }

    public void SetData(BuildingActionData data, Resources cost)
    {
        BuildingData = data;

        if (data.icon != null)
            icon.style.backgroundImage = new StyleBackground(data.icon);

        // Clear old action
        if (button.userData is Action oldAction)
            button.clicked -= oldAction;

        button.userData = data.action;

        if (data.action != null)
            button.clicked += data.action;

        upgradeCost = cost;

        if (UIManager.instance != null &&
        UIManager.instance.HoveredButton == button)
        {
            UIManager.instance.PopulateHoverCost(upgradeCost);
            UIManager.instance.RefreshHoverText(BuildingData.name, BuildingData.description);
        }
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
}