using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BuildableTile : MonoBehaviour, IClickable, ISelectable, IArtilleryTarget
{
    [SerializeField] private Renderer _tileRenderer;
    private Color originalColor;

    public Vector3 WorldPosition { get => transform.position; }
    public Renderer TileRenderer { get => _tileRenderer; }

    void Awake()
    {
        if (!Application.isPlaying) return;
        if (_tileRenderer == null)
        {
            _tileRenderer = GetComponentInChildren<Renderer>();

            if (_tileRenderer == null)
            {
                Debug.LogError($"{name} has NO Renderer for BuildableTile!", this);
                return;
            }
        }
        originalColor = _tileRenderer.sharedMaterial.color;
    }

    public void Highlight(Color color)
    {
        if (!Application.isPlaying) return;
        if (_tileRenderer == null)
        {
            _tileRenderer = GetComponentInChildren<Renderer>();

            if (_tileRenderer == null)
            {
                Debug.LogError($"{name} has NO Renderer for BuildableTile!", this);
                return;
            }
        }
        _tileRenderer.material.color = color;
    }

    public void ResetHighlight()
    {
        if (!Application.isPlaying) return;
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
