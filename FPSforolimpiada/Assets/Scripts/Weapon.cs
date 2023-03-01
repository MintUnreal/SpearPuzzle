using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Animator WeaponAnimator;
    [SerializeField] private Transform SpearPoint;
    [SerializeField] private Transform WeaponPoint;
    [SerializeField] private GameObject Spear;
    [SerializeField] private Transform Camera;
    [SerializeField] private DigitalRuby.LightningBolt.LightningBoltScript LightingBolt;

    [SerializeField] private GameObject plus;
    [SerializeField] private GameObject minus;

    void Update()
    {
        SetThrowing();
    }
    private void FixedUpdate()
    {
        GetSpear();
    }

    //animation events
    public void SetReadyThrowing(bool ready)
    {
        isReadyThrow = ready;
    }
    public void Throw()
    {
        GameObject newSpear = Instantiate(Spear, SpearPoint.position, Camera.rotation);
        currentSpear = newSpear.GetComponent<Spear>();
        currentSpear.ThrowSpear(1000f,magnetMode,this);
        isThrowing = false;
    }
    //----------------

    private bool isThrowing;
    private bool isReadyThrow;
    private bool Throwed;
    private Spear currentSpear;
    private void SetThrowing()
    {
        if(!Throwed && !isThrowing && Mouse.current.leftButton.wasPressedThisFrame)
        {
            WeaponAnimator.SetBool("isThrow", true);
            isThrowing = true;
            isReadyThrow = false;
            Throwed = true;
            if(connectedPlayer) connectedPlayer.SetFOV(connectedPlayer.startFOV-20f);
        }
        if(isReadyThrow && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isReadyThrow = false;
            WeaponAnimator.SetBool("isThrow", false);
            if (connectedPlayer) connectedPlayer.SetFOV(connectedPlayer.startFOV);
        }
    }

    private float GetSpearTimer = 0;
    private void GetSpear()
    {
        if(currentSpear != null && Mouse.current.rightButton.IsPressed())
        {
            GetSpearTimer += Time.fixedDeltaTime;

            LightingBolt.gameObject.SetActive(true);
            LightingBolt.StartObject.transform.position = WeaponPoint.transform.position;
            LightingBolt.EndObject.transform.position = currentSpear.transform.position;

            if (currentSpear.currentMagnet) Destroy(currentSpear.currentMagnet.gameObject);
            if (GetSpearTimer < 1f)
            {
                if (currentSpear.ParentPhysics)
                {
                    currentSpear.ParentPhysics.AddForce((WeaponPoint.position - currentSpear.ParentPhysics.transform.position) * 10f);
                }
                else
                {
                    if (currentSpear.button) currentSpear.button.DisableButton();

                    currentSpear.gameObject.layer = 10;
                    currentSpear.Rigidbody.isKinematic = false;
                    currentSpear.Rigidbody.AddForce((WeaponPoint.position - currentSpear.transform.position) * 7f);
                    currentSpear.transform.rotation = Quaternion.Lerp(currentSpear.transform.rotation, WeaponPoint.transform.rotation, 0.1f);
                }
            }
            else
            {
                if (currentSpear.ParentPhysics) currentSpear.ParentPhysics = null;
                currentSpear.Rigidbody.isKinematic = true;
                currentSpear.transform.position = Vector3.Lerp(currentSpear.transform.position, WeaponPoint.position, 0.25f);
                currentSpear.transform.rotation = Quaternion.Lerp(currentSpear.transform.rotation, WeaponPoint.transform.rotation,0.25f);
                if(GetSpearTimer > 1.5f)
                {
                    Pickup();
                }
            }

        }
        else
        {
            if(currentSpear && GetSpearTimer >= 1f) currentSpear.Rigidbody.isKinematic = false;
            LightingBolt.gameObject.SetActive(false);
            GetSpearTimer = 0;
        }
    }
    public void Pickup()
    {
        Destroy(currentSpear.gameObject);
        GetSpearTimer = 0;
        Throwed = false;
        WeaponAnimator.SetTrigger("Pickup");

        if (currentSpear.button) currentSpear.button.DisableButton();
    }

    private Player connectedPlayer;
    public void ConnectPlayer(Player newPlayer)
    {
        connectedPlayer = newPlayer;
    }

    private Magnet.MagnetMode magnetMode;
    public void SetMagnet(Magnet.MagnetMode mode)
    {
        magnetMode = mode;
        currentSpear.SetMagnet(mode);
        VisualSetMagnet(mode);
        if (unmagnetCoroutine != null)
        {
            StopCoroutine(unmagnetCoroutine);
        }
        unmagnetCoroutine = StartCoroutine(unmagnet());
    }

    private Coroutine unmagnetCoroutine;
    private IEnumerator unmagnet()
    {
        yield return new WaitForSeconds(10f);
        if(currentSpear) currentSpear.SetMagnet(Magnet.MagnetMode.disabled);
        magnetMode = Magnet.MagnetMode.disabled;
        VisualSetMagnet(Magnet.MagnetMode.disabled);
    }

    private void VisualSetMagnet(Magnet.MagnetMode mode)
    {
        switch (mode)
        {
            case Magnet.MagnetMode.disabled:
                plus.SetActive(false);
                minus.SetActive(false);
                break;
            case Magnet.MagnetMode.minus:
                plus.SetActive(false);
                minus.SetActive(true);
                break;
            case Magnet.MagnetMode.plus:
                plus.SetActive(true);
                minus.SetActive(false);
                break;
        }
    }

}
