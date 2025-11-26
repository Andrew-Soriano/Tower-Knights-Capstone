using UnityEngine;

public class BuildableTile : MonoBehaviour, IClickable, ISelectable
{
    protected int _currentMode;
    [SerializeField] protected int _defaultMode;

    void Awake()
    {
        _switchMode(_defaultMode);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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
