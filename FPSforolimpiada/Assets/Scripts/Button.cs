using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool state;

    [SerializeField] private GameObject door;
    [SerializeField] private bool invert;

    private Spear currentSpear;
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Spear" && other.gameObject.GetComponent<Spear>().CanStuck) || other.gameObject.tag == "Box" || other.gameObject.tag == "plus" || other.gameObject.tag == "minus")
        {
            state = true;
            animator.SetBool("state", true);
            if (other.gameObject.tag == "Spear")
            {
                currentSpear = other.gameObject.GetComponent<Spear>();
                currentSpear.button = this;
            }

            door.SetActive(invert);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Box")
        {
            DisableButton();
        }
    }

    private float checkButton;
    private bool checkButtonState;
    private void OnTriggerStay(Collider other)
    {
        checkButtonState = true;
    }

    private void Update()
    {
        checkButton += Time.deltaTime;

        if (checkButton > 1)
        {
            if (!checkButtonState) DisableButton();
            if (currentSpear == null) checkButtonState = false;
            checkButton = 0;
        }
    }

    public void DisableButton()
    {
        state = false;
        animator.SetBool("state", false);
        door.SetActive(!invert);
    }

}
