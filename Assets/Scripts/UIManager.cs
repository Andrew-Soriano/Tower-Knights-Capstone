using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

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
    private Button _ArcherButton;
    private Label _ArcherWood;

    //Resource Builder Menu
    public VisualElement resourceMenu;
    private Button _resourceButton;

    //Tower Menu
    public VisualElement towerMenu;
    private Button _rotateTowerLeft;
    private Button _rotateTowerRight;

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
        _ArcherButton = buildMenu.Q<Button>("BuildArcherButton"); //Archer Tower Components
        _ArcherWood = buildMenu.Q<Label>("ArcherWoodCount");
        _ArcherWood.text = TowerData.ArcherCost.Wood.ToString("D2");

        //Resource Tile Menu Initialize
        resourceMenu = root.Q<VisualElement>("Resource");
        resourceMenu.style.display = DisplayStyle.None;
        _resourceButton = resourceMenu.Q<Button>("BuildResourceButton");

        //Tower Menu Initialize
        towerMenu = root.Q<VisualElement>("Tower");
        towerMenu.style.display = DisplayStyle.None;
        _rotateTowerLeft = towerMenu.Q<Button>("RotateLeft");
        _rotateTowerRight = towerMenu.Q<Button>("RotateRight");

        //Castle Menu Initialize
        castleMenu = root.Q<VisualElement>("Castle");

        _input = GetComponent<PlayerInput>();

        _pointAction = _input.actions["Point"];
        _clickAction = _input.actions["Click"];
        _cancelAction = _input.actions["Cancel"];
    }

    private void OnEnable()
    {
        _ArcherButton.clicked += OnBuildArcherPressed;
        _resourceButton.clicked += OnBuildResourcePress;
        _nextRoundButton.clicked += OnNextRoundButtonClicked;
        _rotateTowerLeft.clicked += OnTowerRotateLeftClicked;
        _rotateTowerRight.clicked += OnTowerRotateRightClicked;
    }

    private void OnDisable()
    {
        _ArcherButton.clicked -= OnBuildArcherPressed;
        _resourceButton.clicked -= OnBuildResourcePress;
        _nextRoundButton.clicked -= OnNextRoundButtonClicked;
        _rotateTowerLeft.clicked -= OnTowerRotateLeftClicked;
        _rotateTowerRight.clicked -= OnTowerRotateRightClicked;
    }

    public void OpenBuildMenu()
    {
        if (currentMenu != null)
            CloseMenu();

        currentMenu = buildMenu;
        currentMenu.style.display = DisplayStyle.Flex;

        if(TowerData.ArcherCost.Wood > CastleController.instance.stockpile.Wood)
        {
            _ArcherWood.style.color = new StyleColor(Color.gray);
        }
        else
        {
            _ArcherWood.style.color = new StyleColor(Color.white);
        }
    }

    public void OpenResourceMenu(ResourceTile tile)
    {
        if (currentMenu != null)
            CloseMenu();

        currentMenu = resourceMenu;
        _resourceButton.text = tile.tileName;
        currentMenu.style.display = DisplayStyle.Flex;
    }

    public void OpenTowerMenu()
    {
        if (currentMenu != null)
            CloseMenu();

        currentMenu = towerMenu;
        currentMenu.style.display = DisplayStyle.Flex;
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
        StartCoroutine(FlashCoroutine(label, duration, repeat, color));
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

    private void OnBuildArcherPressed()
    {
        if(SelectionManager.instance.GetCurrent() is TowerTile tile)
        {
            if (CastleController.instance.BuildTower(tile, towerID.Archer))
            {
                RefreshResourceUI(CastleController.instance.stockpile);
                SelectionManager.instance.Deselect();
            }
            else
            {
                FlashLabel(_resourceLabels[ResourceType.Wood], Color.red);
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

    private void OnTowerRotateLeftClicked()
    {
        if(SelectionManager.instance.GetCurrent() is TowerBase tower)
        {
            tower.RotateLeft();
        }
    }

    private void OnTowerRotateRightClicked()
    {
        if(SelectionManager.instance.GetCurrent() is TowerBase tower)
        {
            tower.RotateRight();
        }
    }

    public void OnNextRoundButtonClicked()
    {
        nextRoundClicked?.Invoke();
    }
}
