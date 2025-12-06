using UnityEngine;

public class CratingBlacksmith : CraftingBase
{
    protected override void Awake()
    {
        base.Awake();
        if (_type != ResourceType.Metal && _type != ResourceType.Parts)
            _type = ResourceType.Metal;
    }

    public void OnSwitchType()
    {
        _type = _type == ResourceType.Metal ? ResourceType.Parts : ResourceType.Metal;
    }
}
