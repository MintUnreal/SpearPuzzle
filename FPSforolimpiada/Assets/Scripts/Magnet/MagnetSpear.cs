using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetSpear : MonoBehaviour
{
    private List<Rigidbody> rigidbodyes = new List<Rigidbody>();
    [SerializeField] public Magnet.MagnetMode mode;
    [SerializeField] private float magnetForce;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidbody;
        if(other.TryGetComponent<Rigidbody>(out rigidbody))
        {
            rigidbodyes.Add(rigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigidbody;
        if (other.TryGetComponent<Rigidbody>(out rigidbody))
        {
            rigidbodyes.Remove(rigidbody);
        }
    }

    private void FixedUpdate()
    {
        foreach(Rigidbody i in rigidbodyes)
        {
            if(i)
            {
                Vector3 direction = (i.transform.position - transform.position).normalized;
                if(i.tag == "plus")
                {
                    switch(mode)
                    {
                        case Magnet.MagnetMode.minus:
                            i.AddForce(-direction * magnetForce);
                            break;
                        case Magnet.MagnetMode.plus:
                            i.AddForce(direction * magnetForce);
                            break;
                    }
                }
                if(i.tag == "minus")
                {
                    switch (mode)
                    {
                        case Magnet.MagnetMode.minus:
                            i.AddForce(direction * magnetForce);
                            break;
                        case Magnet.MagnetMode.plus:
                            i.AddForce(-direction * magnetForce);
                            break;
                    }
                }
            }
        }
    }
}
