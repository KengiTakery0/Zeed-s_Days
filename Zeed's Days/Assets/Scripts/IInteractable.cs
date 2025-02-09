using UnityEngine;

public enum InteractItemType
{
    Transport,
    Ammo
}
public interface IInteractable
{
    void Interact(GameObject interactor);
    void EnterInteract();
    void LeaveInteract();
}