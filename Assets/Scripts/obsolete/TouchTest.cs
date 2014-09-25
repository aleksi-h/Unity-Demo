using UnityEngine;
using System.Collections;

public class TouchTest : MonoBehaviour
{

    public float speed;
    private Vector3 newDestination;
    private bool hasNewDestination;

    void Start()
    {

    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 2000))
            {
                newDestination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                hasNewDestination = true;
            }

            Debug.DrawRay(ray.origin, ray.direction * 10, Color.blue, 2.0f);
        }


        if (hasNewDestination)
        {
            transform.position = Vector3.MoveTowards(transform.position, newDestination, speed * Time.deltaTime);
            if (transform.position == newDestination)
            {
                hasNewDestination = false;
            }
        }
         * */
    }
}