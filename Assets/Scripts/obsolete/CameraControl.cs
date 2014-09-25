using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    public float damping;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        Vector3 newCameraPos = player.transform.position + offset;
        Vector3 position = Vector3.Lerp(transform.position, newCameraPos, Time.deltaTime * damping);
        transform.position = position;
    }
}
