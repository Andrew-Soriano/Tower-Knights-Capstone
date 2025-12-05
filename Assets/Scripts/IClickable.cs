using UnityEngine;

public enum ClickableObjectType
{
    Buildable,
    Resource
}

public interface IClickable
{
    public void OnClicked();
}

public interface IArtilleryTarget
{

}
