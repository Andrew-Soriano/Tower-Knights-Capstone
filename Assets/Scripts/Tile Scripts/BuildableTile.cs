using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BuildableTile : MonoBehaviour, IClickable, ISelectable, IArtilleryTarget
{
    [SerializeField] private Renderer _tileRenderer;
    private Color originalColor;

    public Vector3 WorldPosition { get => transform.position; }

    void Awake()
    {
        originalColor = _tileRenderer.sharedMaterial.color;
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
