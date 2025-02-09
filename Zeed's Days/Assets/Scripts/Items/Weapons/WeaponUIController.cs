using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bulletText;
    [SerializeField] TextMeshProUGUI allBulletText;

    [SerializeField] Image weaponImage;

    public void SetImage(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }
    public void BulletTextChange(Weapon weapon)
    {
        int bullet = weapon.bullets;
        bulletText.text = $"{bullet}";
    }
    public void AllBulletTextChange(Weapon weapon)
    {
        int allbullet = weapon.allbullet;
        allBulletText.text = $"/{allbullet}";
    }

}
