using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BuildableTile : MonoBehaviour, IClickable, ISelectable, IArtilleryTarget
{
    protected int _currentMode;
    [SerializeField] protected int _defaultMode;

    [SerializeField] private Renderer _tileRenderer;
    private Color originalColor;

    public Vector3 WorldPosition { get => transform.position; }

    void Awake()
    {
        _switchMode(_defaultMode);
        originalColor = _tileRenderer.material.color;
    }

    protected void _switchMode(int mode)
    {
        _currentMode = mode;
        string modeString1 = new string("Mode_A");
        string modeString2 = new string("Mode_B");

        switch (mode)
        {
            case 1:
                break;
            case 2:
                modeString1 = "Mode_B";
                modeString2 = "Mode_A";
                break;
        }

        foreach (Transform child in transform)
        {
            if (child.CompareTag(modeString1))
            {
                child.gameObject.SetActive(true);
            }
            else if (child.CompareTag(modeString2))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void Highlight(Color color)
    {
        _tileRenderer.material.color = color;
    }

    public void ResetHighlight()
    {
        _tileRenderer.material.color = originalColor;
    }

    public void OnClicked()
    {
        SelectionManager.instance.Select(this);
    }

    public virtual void OnSelect()
    {
    }

    public virtual void OnDeselect()
    {
    }
}
