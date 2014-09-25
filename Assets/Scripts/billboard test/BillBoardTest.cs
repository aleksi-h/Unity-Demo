using UnityEngine;
using System.Collections;

public class BillBoardTest : MonoBehaviour
{
    public Camera m_Camera;
 
    void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back,
            m_Camera.transform.rotation * Vector3.up);
    }
}