using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUIController : MonoBehaviour
{
    [SerializeField] GameObject InteractTip;

    public void ShowInteractTip()
    {
        InteractTip.SetActive(true);
    }
    public void HideInteractTip()
    {
        InteractTip.SetActive(false);
    }
}
