
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TouchController : MonoBehaviour
{

    private GameObject target;
    private bool dragging;
    private RaycastHit hit;

    void Start()
    {
        hit = new RaycastHit();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dragging)
        {
            dragging = false;
            target = null;
        }
        else if (Input.GetMouseButtonDown(0) && !dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1100))
            {
                dragging = true;
                target = hit.transform.gameObject;
            }
        }

        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                target.transform.position = new Vector3(Mathf.Round(hit.point.x),
                                            target.transform.position.y,
                                             Mathf.Round(hit.point.z));

                //target.transform.position = new Vector3(hit.point.x, target.transform.position.y, hit.point.z);
                Debug.Log("pos: " + hit.point.x + ", " + hit.point.z);
            }
        }
    }
}