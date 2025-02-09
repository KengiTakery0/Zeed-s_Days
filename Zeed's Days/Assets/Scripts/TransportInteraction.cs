using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportInteraction : MonoBehaviour, IInteractable
{
    Rigidbody rb;
    bool isActive;
   public bool canMove;
    [SerializeField] UnityEvent FindedInteractionIvent;
    [SerializeField] UnityEvent LostInteractionIvent;
    public InputController controller;
    public Transform PlayerSit;
    public void EnterInteract()
    {
        FindedInteractionIvent?.Invoke();
    }

    public void Interact(GameObject interactor)
    {
        interactor.GetComponent<PlayerController>().SeatOnTransport(gameObject);
    }

    public void LeaveInteract()
    {
       LostInteractionIvent?.Invoke();
    }

   public  IEnumerator Move()
    {
        while (rb != null && canMove)
        {
            rb.velocity = new Vector3(0,0,controller.MoveDirection.y);
            yield return new WaitForSeconds(0.01f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
