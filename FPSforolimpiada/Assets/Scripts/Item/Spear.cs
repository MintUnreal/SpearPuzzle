using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spear : Interactable 
{
    public Rigidbody Rigidbody;
    public Rigidbody ParentPhysics;
    public Collider Collider;
    public Button button;

    [SerializeField] private GameObject plus;
    [SerializeField] private GameObject minus;

    [SerializeField] private GameObject magnet;

    public bool CanStuck { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        StuckSpear(other);
        
    }
    private void Start()
    {

    }
    private void Update()
    {
        SpearEndDirection();
    }

    private void FixedUpdate()
    {

    }

    public MagnetSpear currentMagnet;
    private void StuckSpear(Collider _object)
    {
        if (CanStuck)
        {
            if (_object.GetComponent<Rigidbody>())
            {
                ParentPhysics = _object.GetComponent<Rigidbody>();
                ParentPhysics.AddForce(Rigidbody.velocity * 50f);
            }
            Rigidbody.isKinematic = true;
            Collider.isTrigger = false;
            transform.SetParent(_object.transform);
            gameObject.layer = 11;
            magnet.gameObject.SetActive(true);
            if (!currentMagnet)
            {
                currentMagnet = Instantiate(magnet, transform.position, Quaternion.identity).GetComponent<MagnetSpear>();
                currentMagnet.mode = magnetMode;
            }
            StartCoroutine(CanStuckIEnum());
        }
    }
    private IEnumerator CanStuckIEnum()
    {
        yield return new WaitForSeconds(0.1f);
        CanStuck = false;
    }

    public Weapon currentWeapon;
    public void ThrowSpear(float force, Magnet.MagnetMode magnetMode, Weapon weapon)
    {
        CanStuck = true;
        Rigidbody.AddForce(transform.forward * force);
        currentWeapon = weapon;
        SetMagnet(magnetMode);
    }


    private float delayTimer = 0;
    private void SpearEndDirection()
    {
        if (delayTimer > 0.1f)
        {
            if (!Rigidbody.isKinematic && Rigidbody.velocity.magnitude > 5f) transform.rotation = Quaternion.LookRotation(Rigidbody.velocity.normalized);
        }
        else
        {
            delayTimer += Time.deltaTime;
        }
    }

    private Magnet.MagnetMode magnetMode;
    public void SetMagnet(Magnet.MagnetMode mode)
    {
        magnetMode = mode;
        switch(mode)
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
        if(currentMagnet) currentMagnet.mode = mode;
        
    }

    private void OnDestroy()
    {
       if(currentMagnet) Destroy(currentMagnet.gameObject);
    }
    //interact
    public override void InteractableAction()
    {
        currentWeapon.Pickup();
    }
}
