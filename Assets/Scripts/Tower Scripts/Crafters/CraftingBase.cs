using UnityEngine;

public class CraftingBase : BuildingBase
{
    [SerializeField] private int _amount = 1;
    [SerializeField] private int _cost = 1;
    [SerializeField] protected ResourceType _type = ResourceType.Planks;
    [SerializeField] private ResourceType _costType = ResourceType.Planks;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        StatusBarController.nextRoundClicked += AddResource;
    }

    private void OnDisable()
    {
        StatusBarController.nextRoundClicked -= AddResource;
    }

    private void AddResource()
    {
        if(CastleController.instance.stockpile.GetAmount(_costType) >= _cost)
            if(CastleController.instance.AddResourceToStock(_type, _amount))
                CastleController.instance.SubtractFromStock(_costType, _cost);
    }
}
