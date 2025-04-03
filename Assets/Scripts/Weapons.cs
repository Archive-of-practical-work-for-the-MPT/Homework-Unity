using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    public WeaponsEnum WeaponType;

    public int WeaponDamage;

    public LayerMask EnemyLayer;

    public int MaxMagazineBulletCount;

    public int CurrentMagazineBulletCount;

    public int MaxAmmoSupply;

    public float TimeBetweenShots;

    public float TimeForReloading;

    public AudioSource ShotSound;

    public AudioSource ReloadSound;

    bool CanFire = true;

    bool IsReloading = false;

    private IEnumerator LockFire(float Time)
    {
        yield return new WaitForSeconds(Time);
        CanFire = true;
    }

    private IEnumerator LockFireForReloading(float Time)
    {
        ReloadSound.Play();
        yield return new WaitForSeconds(Time);
        CurrentMagazineBulletCount = MaxMagazineBulletCount;
        CanFire = true;
        IsReloading = false;
        Debug.LogWarning("Перезарядка завершена!");
    }

    public bool Reload()
    {
        if (!IsReloading)
        {
            Debug.LogWarning("Перезарядка!");
            IsReloading = true;
            CanFire = false;

            StartCoroutine(LockFireForReloading(TimeForReloading));

            return true;
        }
        return false;
    }

    public bool IsMagazineEmpty()
    {
        if (CurrentMagazineBulletCount == 0) return true;
        else return false;
    }

    public bool Fire()
    {
        if (CanFire && !IsMagazineEmpty() && !IsReloading)
        {
            Debug.LogWarning(CurrentMagazineBulletCount);
            ShotSound.Play();
            CurrentMagazineBulletCount--;

            RaycastHit HitInfo = new RaycastHit();

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo))
            {
                if (HitInfo.transform.gameObject.layer == 6)
                {
                    EnemyController EC = HitInfo.transform.gameObject.GetComponent<EnemyController>();
                    EC.DealDamage(WeaponDamage);
                    Debug.Log("Вы попали по " + HitInfo.transform.name + " и нанесли ему урона " + WeaponDamage);
                }

            }

            CanFire = false;

            StartCoroutine(LockFire(TimeBetweenShots));

            return true;
        }
        return false;
    }
}   