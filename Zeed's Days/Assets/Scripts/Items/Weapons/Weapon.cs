using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum WeaponType
{
    Melee,
    AutomaticRifle,
    Rifle,
    Pistol
}
public class Weapon : MonoBehaviour
{
    [SerializeField] public int bullets;
    [SerializeField] public int allbullet;
    [SerializeField] public int magLenght;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] public Transform shootHole;


    [SerializeField] UnityEvent<Weapon> OnShootEventUI;
    public float fireRate = 0.5f; // Время между выстрелами
    private bool canShoot;

    private void Awake()
    {
        OnShootEventUI?.Invoke(this);
    }
    public void Reload()
    {
        if (bullets == magLenght || allbullet == 0) return;
        if (bullets > 0 && bullets < magLenght)
        {
            int nedbullets = magLenght - bullets;
            allbullet -= nedbullets;
            bullets += nedbullets;
        }
        if (allbullet - magLenght < 0)
        {
            bullets = allbullet;
            allbullet = 0;
        }
        else
        {
            allbullet -= magLenght;
            bullets = magLenght;
        }
        OnShootEventUI?.Invoke(this);
    }
    public void Shoot(Vector3 aimDir)
    {
        StartCoroutine(CanShoot(aimDir));
        if (bullets > 0 && canShoot)
        {
            Instantiate(bulletPrefab, shootHole.position, Quaternion.LookRotation(aimDir, Vector3.up));
            bullets--;
            OnShootEventUI?.Invoke(this);
            canShoot = false;
        }

    }
    IEnumerator CanShoot(Vector3 aimDir)
    {
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}
