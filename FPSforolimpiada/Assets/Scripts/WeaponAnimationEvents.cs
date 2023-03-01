using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    [SerializeField] private Weapon Weapon;

    public void isReadyThrowing()
    {
        Weapon.SetReadyThrowing(true);
    }

    public void Throw()
    {
        Weapon.Throw();
    }
}
