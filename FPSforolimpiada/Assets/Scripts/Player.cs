using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform camera;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    private Camera cameraComponent;

    [Header("Interactable trigger")]
    [SerializeField] private float rayDistance;
    [SerializeField] private float sphereRadius;

    [SerializeField] private RectTransform PickupNotice;

    private void Awake()
    {
        if (camera)
        {
            cameraComponent = camera.GetComponent<Camera>();
            update += InteractableCheck;
        }
        else
        {
            Debug.LogWarning("В Player не подключена camera, многие функции не доступны!");
        }

        if (virtualCamera)
        {
            startFOV = virtualCamera.m_Lens.FieldOfView;
            update += SmoothFOV;
        }
        else
        {
            Debug.LogWarning("В Player не подключена virtualCamera");
        }

        if(weapon)
        {
            weapon.ConnectPlayer(this);
        }
        else
        {
            Debug.LogWarning("В Player не подключен weapon, возможны ошибки!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        RaycastHit hit;
        Vector3 endLinePosition = Vector3.zero;
        if (Physics.Raycast(camera.position, camera.transform.forward, out hit, rayDistance))
        {
            endLinePosition = hit.point;
        }
        else
        {
            endLinePosition = camera.transform.position + camera.transform.forward * rayDistance;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(camera.position, endLinePosition);
    }

    private delegate void updateDelegate();
    private updateDelegate update;
    private void Update()
    {
        update();
        InteractableAction();
    }

    private Interactable currentInteractable;
    private void InteractableCheck()
    {
        Collider[] cols;
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, rayDistance))
        {
            cols = Physics.OverlapSphere(hit.point, sphereRadius);
        }
        else
        {
            cols = Physics.OverlapSphere(camera.position + camera.forward * rayDistance, sphereRadius);
        }

        bool have = false;
        foreach (Collider i in cols)
        {
            if (i.GetComponent<Interactable>())
            {
                currentInteractable = i.GetComponent<Interactable>();
                have = true;
                break;
            }
        }
        if (!have) currentInteractable = null;
    }
    private void InteractableAction()
    {
        if (currentInteractable)
        {
            PickupNotice.gameObject.SetActive(true);
            PickupNotice.position = cameraComponent.WorldToScreenPoint(currentInteractable.transform.position);
            if (Keyboard.current.eKey.wasReleasedThisFrame)
            {
                currentInteractable.InteractableAction();
            }
        }
        else
        {
            PickupNotice.gameObject.SetActive(false);
        }
    }

    public float startFOV { get; private set; }
    private float dynamicFOV = 70;
    private void SmoothFOV()
    {
        if (virtualCamera)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, dynamicFOV, 0.01f);
        }
    }
    public void SetFOV(float newFOV)
    {
        dynamicFOV = newFOV;
    }

}
