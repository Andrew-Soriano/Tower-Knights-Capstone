using UnityEngine;
public interface ISelectable
{
    void OnSelect();
    void OnDeselect();
}

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    private ISelectable _current;

    void Awake()
    {
        //Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Select(ISelectable target)
    {
        if (_current == target)
            return;

        // Deselect old
        if (_current != null)
            _current.OnDeselect();

        _current = target;

        if (_current != null)
            _current.OnSelect();
    }

    public void Deselect()
    {
        if (_current != null)
            _current.OnDeselect();

        _current = null;
    }

    public ISelectable GetCurrent() => _current;
}
