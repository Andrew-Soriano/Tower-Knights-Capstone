using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("Data")]
    public TowerDatabase towerDatabase;

    [Header("Controllers")]
    [SerializeField] private BuildMenuController _buildMenuControler;
    [SerializeField] private StatusBarController _statusBarContoller;
    [SerializeField] private UpgradeMenuController _upgradeMenuContoller;
    [SerializeField] private MainMenuController _MainMenuContoller;

    public static UIManager instance;
    private bool ignoreNextUIInput = false;

    private VisualElement _root;
    private VisualElement _currentMenu;

    private Dictionary<Label, Coroutine> _activeFlashes = new();
    private Dictionary<Button, Action> _safeClickHandlers = new();
    public Dictionary<VisualElement, EventCallback<PointerEnterEvent>> _hoverEnterCallbacks = new();
    public Dictionary<VisualElement, EventCallback<PointerLeaveEvent>> _hoverLeaveCallbacks = new();

    private PlayerInput _input;

    public VisualElement Root { get => _root; }
    public VisualElement CurrentMenu { get => _currentMenu; }
    public BuildMenuController BuildMenuControler { get => _buildMenuControler; }
    public StatusBarController StatusBarContoller { get => _statusBarContoller; }
    public UpgradeMenuController UpgradeMenuContoller { get => _upgradeMenuContoller; }
    private void Awake()
    {
        //Singleton pattern
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (towerDatabase != null && TowerDatabase.Instance == null)
            TowerDatabase.SetInstance(towerDatabase);

        _root = GetComponent<UIDocument>().rootVisualElement;

        _input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _safeClickHandlers.Clear();
        _hoverEnterCallbacks.Clear();
        _hoverLeaveCallbacks.Clear();
    }

    private void OnDisable()
    {
        //Remove all SafeButtonClicks
        foreach (var kvp in _safeClickHandlers)
        {
            kvp.Key.clicked -= kvp.Value;
        }

        _safeClickHandlers.Clear();

        _hoverEnterCallbacks.Clear();
        _hoverLeaveCallbacks.Clear();

        //Stop active flash coroutines
        foreach (var kvp in _activeFlashes)
            StopCoroutine(kvp.Value);
        
        _activeFlashes.Clear();
    }
    public void RegisterSafeClick(Button button, Action action)
    {
        if (button == null) return;

        if (_safeClickHandlers.TryGetValue(button, out var existing))
            button.clicked -= existing;

        void handler() => SafeButtonClick(action);
        _safeClickHandlers[button] = handler;
        button.clicked += handler;
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
        for (int i = 0; i < 16; i++) yield return null;

        ignoreNextUIInput = false;
    }

    public Texture2D GetResourceIcon(ResourceType type)
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

    public void PopulateHoverCost(Resources cost, BuildingUpgradeAction action, VisualElement container)
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

    public void OpenMenu(VisualElement menu)
    {
        if (_currentMenu != null)
            CloseMenu();
        _currentMenu = menu;
        _currentMenu.style.display = DisplayStyle.Flex;

        _input.SwitchCurrentActionMap("Menu");

        // Ignore any UI click on this frame
        ignoreNextUIInput = true;
        StartCoroutine(DisarmButtonSafety());
    }

    public void CloseMenu()
    {
        if (_currentMenu == null)
            return;

        if (_currentMenu != null)
            _currentMenu.style.display = DisplayStyle.None;
        
        _currentMenu = null;
        _input.SwitchCurrentActionMap("Camera");
        
        // Deselect current selection
        if (SelectionManager.instance.GetCurrent() != null)
            SelectionManager.instance.Deselect();
    }

    public void EnterTargetingMode()
    {
        _currentMenu.style.display = DisplayStyle.None;
        _currentMenu = null;
        _input.SwitchCurrentActionMap("Camera");
    }

    public void FlashLabel(Label label, Color color, float duration = 0.25f, int repeat = 3)
    {
        if (_activeFlashes.TryGetValue(label, out Coroutine existing))
            StopCoroutine(existing);
        _activeFlashes[label] = StartCoroutine(FlashCoroutine(label, duration, repeat, color));
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
}
