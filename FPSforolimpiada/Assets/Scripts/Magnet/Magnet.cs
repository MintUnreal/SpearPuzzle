using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private MagnetMode magnetMode;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Spear>())
        {
            other.gameObject.GetComponent<Spear>().currentWeapon.SetMagnet(magnetMode);
        }
    }

    [System.Serializable]
    public enum MagnetMode
    {
        disabled,
        plus,
        minus
    }
}
