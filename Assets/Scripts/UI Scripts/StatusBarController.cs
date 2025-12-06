using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusBarController : MonoBehaviour
{
    public static event Action nextRoundClicked;

    private VisualElement _statusBar;
    private Button _nextRoundButton;
    private Label _roundCountText;
    private Dictionary<ResourceType, Label> _resourceLabels;
    private Label _castleHPLabel;

    public UIManager UIManager { get; set; }
    public CastleController CastleController { get; set; }
    public Button NextRoundButton => _nextRoundButton;

    private void Awake()
    {
        UIManager = UIManager ?? UIManager.instance;
        CastleController = CastleController ?? CastleController.instance;

        _statusBar = UIManager.Root.Q<VisualElement>("StatusBar");
        _nextRoundButton = _statusBar.Q<Button>("NextRoundButton");
        _roundCountText = _statusBar.Q<Label>("RoundCount");
        _castleHPLabel = _statusBar.Q<Label>("HPCount");
        _resourceLabels = new Dictionary<ResourceType, Label>
        {
            { ResourceType.Wood, _statusBar.Q<Label>("WoodCount") },
            { ResourceType.Planks, _statusBar.Q<Label>("PlanksCount") },
            { ResourceType.Stone, _statusBar.Q<Label>("StoneCount") },
            { ResourceType.Bricks, _statusBar.Q<Label>("BricksCount") },
            { ResourceType.Ore, _statusBar.Q<Label>("OreCount") },
            { ResourceType.Metal, _statusBar.Q<Label>("MetalCount") },
            { ResourceType.Parts, _statusBar.Q<Label>("PartsCount") }
        };
    }

    private void OnEnable()
    {
        UIManager.RegisterSafeClick(_nextRoundButton, OnNextRoundButtonClicked);

        EnemyBaseController.endOfRound += EndRoundUI;
    }

    private void OnDisable()
    {
        EnemyBaseController.endOfRound -= EndRoundUI;
    }

    public void StartRoundUI(int roundNum)
    {
        _nextRoundButton.visible = false;

        _roundCountText.text = roundNum.ToString("D2");
    }

    public void EndRoundUI() => _nextRoundButton.visible = true;

    public void RefreshResourceUI(Resources stockpile)
    {
        foreach (var kvp in _resourceLabels)
        {
            int amount = kvp.Key switch
            {
                ResourceType.Wood => stockpile.Wood,
                ResourceType.Planks => stockpile.Planks,
                ResourceType.Stone => stockpile.Stone,
                ResourceType.Bricks => stockpile.Bricks,
                ResourceType.Ore => stockpile.Ore,
                ResourceType.Metal => stockpile.Metal,
                ResourceType.Parts => stockpile.Parts,
                _ => 0
            };
            
            kvp.Value.text = amount.ToString("D3");
        }
    }

    public void RefreshCastleHP(int currentHP, int maxHP)
    {
        if (_castleHPLabel != null)
            _castleHPLabel.text = $"{currentHP}/{maxHP}";
    }

    public void FlashMissingLabel(Resources missing)
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
                UIManager.FlashLabel(_resourceLabels[type], Color.red);
        }
    }

    public void FlashCastleHP() => UIManager.FlashLabel(_castleHPLabel, Color.red);

    public void OnNextRoundButtonClicked() => nextRoundClicked?.Invoke();
}
