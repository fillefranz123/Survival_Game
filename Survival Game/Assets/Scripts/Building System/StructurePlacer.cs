using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StructurePlacer : MonoBehaviour
{

    [SerializeField] float placeRange = 5;
    [SerializeField] float raycastTolerance = 1f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] LayerMask structureLayers;

    [Header("Debugging")]
    [SerializeField] bool isLookingAtGround;
    [SerializeField] bool isLookingAtStructure;
    [SerializeField] Structure lookedAtStructure;
    Camera playerCamera;

    [SerializeField] StructureData[] structures;

    Dictionary<Item, Structure> getStructure = new Dictionary<Item, Structure>();
    Dictionary<Item, GameObject> previews = new Dictionary<Item, GameObject>();

    [SerializeField] Item heldItem;

    RaycastHit hit;


    //Observer variables
    Structure previousStructure;
    Item previousItem;


    void Awake()
    {
        PopulateDictonary();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        
    }

    void PopulateDictonary()
    {
        for (int i = 0; i < structures.Length; i++)
        {
            getStructure.Add(structures[i].item, structures[i].prefab.GetComponent<Structure>());
            Debug.Log(getStructure[structures[i].item].structure);
        }

        for (int i = 0; i < structures.Length; i++)
        {
            GameObject preview = Instantiate(structures[i].prefab);
            preview.GetComponent<MeshRenderer>().material.color = Color.white;
            preview.transform.parent = transform;
            if (preview.TryGetComponent<Collider>(out Collider collider))
            {
                Destroy(collider);
            }
            preview.SetActive(false);
            previews.Add(structures[i].item, preview);

        }
    }

    // Update is called once per frame
    void Update()
    {
        isLookingAtGround = IsLookingAtGround();
        isLookingAtStructure = IsLookingAtStructure();

        if (isLookingAtStructure)
        {
            lookedAtStructure = GetLookedAtStructure();
            SnapPoint point = GetLookedAtSnapPoint();
            ProjectSnapPreview(point);

        }

        else if (isLookingAtGround && getStructure[heldItem].canBePlacedOnGround)
        {
            ProjectGroundPreview();
            lookedAtStructure = null;
        }

        else if(!isLookingAtStructure && !isLookingAtGround)
        {
            DisableProjection();
            lookedAtStructure = null;
        }

        StructureReseter();
        DisableUnwantedProjections();
    }

    void StructureReseter()
    {
        if (lookedAtStructure != previousStructure )
        {
            if (previousStructure != null)
            {
                for (int i = 0; i < previousStructure.snapPoints.Length; i++)
                {
                    previousStructure.snapPoints[i].snapPoint.GetComponent<Renderer>().material.color = Color.white;
                }
            }

            previousStructure = lookedAtStructure;
        }
    }


    bool IsLookingAtStructure()
    {
        bool lookingAtStructure = false;


        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, placeRange, structureLayers))
        {
            lookingAtStructure = true; 
        }

        else if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, placeRange) && Physics.CheckSphere(hit.point, raycastTolerance, structureLayers))
        {
            lookingAtStructure = true;
        }

        return lookingAtStructure;
    }

    Structure GetLookedAtStructure()
    {
        if (hit.transform.gameObject.TryGetComponent<Structure>(out Structure structure))
        {
            return structure;
        }

        else 
        {

            Collider[] structures = Physics.OverlapSphere(hit.point, raycastTolerance, structureLayers);
            if (structures[0].gameObject.TryGetComponent<Structure>(out Structure structure1))
            {
                return structure1;
            }

            else
            {
                return null;
            }
        }

        
    }

    SnapPoint GetLookedAtSnapPoint()
    {
        SnapPoint[] points = lookedAtStructure.snapPoints;
        float minDistance = float.MaxValue;
        SnapPoint closest = new SnapPoint();

        for (int i = 0; i < points.Length; i++)
        {
            if (Vector3.Distance(hit.point, points[i].snapPoint.transform.position) < minDistance)
            {
                closest = points[i];

                minDistance = Vector3.Distance(hit.point, closest.snapPoint.transform.position);
            }
        }

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].snapPoint != closest.snapPoint)
            {
                points[i].snapPoint.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        closest.snapPoint.GetComponent<Renderer>().material.color = Color.red;
        return closest;

    }

    void ProjectSnapPreview(SnapPoint point)
    {
        
        GameObject preview = previews[heldItem];

        if (preview != null)
        {
            Debug.Log("no preview");
        }

        List<SnapPoint> compatiblePoints = GetCompatiblePoints();


        if (HeldIsCompatible(point))
        {
            Debug.Log("Held is comptabile");
            SnapPoint closestPoint = new SnapPoint();
            float minDistance = float.MaxValue;

            if (compatiblePoints.Count > 1)
            {
                for (int i = 0; i < compatiblePoints.Count; i++)
                {
                    if (Vector3.Distance(point.snapPoint.transform.localPosition, compatiblePoints[i].snapPoint.transform.localPosition) < minDistance)
                    {
                        closestPoint = compatiblePoints[i];
                        minDistance = Vector3.Distance(point.snapPoint.transform.localPosition, compatiblePoints[i].snapPoint.transform.localPosition);
                    }
                }
            }

            else if (compatiblePoints.Count == 0)
            {
                return;
            }

            else
            {
                closestPoint = compatiblePoints[0];
            }

            preview.transform.parent = lookedAtStructure.transform;
            Vector3 projectPos = point.snapPoint.transform.localPosition + closestPoint.snapPoint.transform.localPosition + getStructure[heldItem].placementOffset;
            preview.transform.localPosition = projectPos;
            preview.transform.rotation = lookedAtStructure.transform.rotation;

            preview.SetActive(true);


            if (Input.GetButtonDown("Place"))
            {
                Debug.Log("Press");

                SetOccupied(closestPoint);
                Place(preview.transform);
                preview.transform.parent = transform;
                Vector3 scaleCorrecter = new Vector3((preview.transform.localScale.x * lookedAtStructure.transform.localScale.x) / lookedAtStructure.transform.localScale.x, (preview.transform.localScale.y * lookedAtStructure.transform.localScale.y) / lookedAtStructure.transform.localScale.y, (preview.transform.localScale.z * lookedAtStructure.transform.localScale.z) / lookedAtStructure.transform.localScale.z);

                preview.transform.localScale = scaleCorrecter;

                SnapPoint placementPoint = GetLookedAtSnapPoint();
            }
        }
        
    }

    bool HeldIsCompatible(SnapPoint point)
    {
        for (int i = 0; i < point.snapGroups.Length; i++)
        {
            for (int j = 0; j < point.snapGroups[i].compatibleStructures.Length; j++)
            {
                if (point.snapGroups[i].compatibleStructures[j] == getStructure[heldItem].structure)
                {
                    return true;
                }
            }
        }

        return false;
    }



    List <SnapPoint> GetCompatiblePoints()
    {
        Structure heldStructure = getStructure[heldItem];
        List<SnapPoint> compatiblePoints = new List<SnapPoint>();   

        for (int i = 0; i < heldStructure.snapPoints.Length; i++)
        {
            for (int j = 0; j < heldStructure.snapPoints[i].snapGroups.Length; j++)
            {
                for (int k = 0; k< heldStructure.snapPoints[i].snapGroups[j].compatibleStructures.Length; k++)
                {
                    if (heldStructure.snapPoints[i].snapGroups[j].compatibleStructures[k] == lookedAtStructure.structure )
                    {
                        compatiblePoints.Add(heldStructure.snapPoints[i]);
                    }
                }
            }
        }

        return compatiblePoints;

    }

    void SetOccupied(SnapPoint closestPoint)
    {
        for (int i = 0; i < closestPoint.snapGroups.Length; i++)
        {
            for (int j = 0; j < closestPoint.snapGroups[i].compatibleStructures.Length; j++)
            {
                if (closestPoint.snapGroups[i].compatibleStructures[j] == getStructure[heldItem].structure)
                {
                    closestPoint.snapGroups[i].isOccupied = true;
                    return;
                }
            }
        }
    }


    #region GroundPlacing
    bool IsLookingAtGround()
    {
        return Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, placeRange, groundLayers);
    }

    void ProjectGroundPreview()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, placeRange, groundLayers))
        {
            Vector3 projectPos = hit.point;
            GameObject preview = previews[heldItem];
            preview.transform.position = projectPos;
            preview.SetActive(true);
            if (Input.GetButtonDown("Place"))
            {
                Place(preview.transform);
            }
        }
    }
    #endregion

    void Place(Transform projection)
    {
         Structure placedStructure = Instantiate(getStructure[heldItem], projection.position, projection.rotation);


        if (isLookingAtStructure)
        {
            SnapPoint closestPoint = new SnapPoint();
            float minDistance = float.MaxValue;

            for (int i = 0; i < placedStructure.snapPoints.Length; i++)
            {
                if (Vector3.Distance(placedStructure.snapPoints[i].snapPoint.transform.position, lookedAtStructure.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(placedStructure.snapPoints[i].snapPoint.transform.position, lookedAtStructure.transform.position);
                    closestPoint = placedStructure.snapPoints[i];
                }
            }

            for (int i = 0; i < closestPoint.snapGroups.Length; i++)
            {
                for (int j = 0; j < closestPoint.snapGroups[i].compatibleStructures.Length; j++)
                {
                    if (closestPoint.snapGroups[i].compatibleStructures[j] == lookedAtStructure.structure)
                    {
                        closestPoint.snapGroups[i].isOccupied = true;
                        return;
                    }
                }
            }
        }
    }

    void DisableProjection()
    {
        previews[heldItem].SetActive(false); 
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward);
        Gizmos.DrawWireSphere(hit.point, raycastTolerance);

    }

    void DisableUnwantedProjections()
    {
        if (heldItem != previousItem)
        {
            for (int i = 0; i < structures.Length; i++)
            {
                if (structures[i].item != heldItem)
                {
                    previews[structures[i].item].SetActive(false);
                }
                
            }

            previousItem = heldItem;
        }
        
    }

    public void SetHeldItem(Item item)
    {
        heldItem = item;
    }

    void OnDisable()
    {
        DisableProjection();
    }

}

[System.Serializable]
public struct StructureData
{
    public GameObject prefab;
    public Item item;
}


