using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] string interactMessage = "Interact";
    [SerializeField] LayerMask interactlayers;
    [SerializeField] float interactRange = 4f;
    [SerializeField] float raycastTolerance = 0.25f;

    [SerializeField] bool doGizmos = false;

    GameObject interactObject;

    Camera playerCamera;
    FPSCamera cameraController;

    RaycastHit interactHit;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<InventoryUI>();
        inventoryUI.gameObject.SetActive(false);
        playerCamera = Camera.main;
        cameraController = GetComponent<FPSCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        InventoryCheck();    
        CheckForInteractable();
    }

    void InventoryCheck()
    {
        if (Input.GetButtonDown("Open Inventory"))
        {
            TogggleInventory();
        }
    }


    void TogggleInventory()
    {
        inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeInHierarchy);
        Cursor.visible = inventoryUI.gameObject.activeInHierarchy;
        cameraController.enabled = !inventoryUI.gameObject.activeInHierarchy;
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void CheckForInteractable()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out interactHit, interactRange))
        {
            if (interactHit.transform.gameObject.layer == interactlayers)
            {
                interactObject = interactHit.transform.gameObject;
            }

            else 
            {
                Collider[] hits = Physics.OverlapSphere(interactHit.point, raycastTolerance, interactlayers);
                if (hits.Length > 0)
                {
                    interactObject = hits[0].gameObject;
                }

                else
                {
                    interactObject=null;
                }
            }

            if (interactObject != null &&  Vector3.Distance(transform.position, interactObject.transform.position) > interactRange)
            {
                interactObject = null;
            }


            if (Input.GetButtonDown("Interact") && interactObject != null)
            {
                interactObject.SendMessage(interactMessage, SendMessageOptions.DontRequireReceiver);
            }

        }

    }

    void OnDrawGizmos()
    {
        if (doGizmos)
        {
            Gizmos.DrawWireSphere(interactHit.point, raycastTolerance);
        }
    }

}
