using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {

        yield return new WaitForSeconds(10f);
        Destroy(gameObject);

    }

}
