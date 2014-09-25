using UnityEngine;
using System.Collections;

public class CameraTest : MonoBehaviour
{

    private float posX;
    private float posZ;
    private float posY;
    public int camera_velocity = 10;

    void Start()
    {
        posX = transform.position.x;
        posY = transform.position.y;
        posZ = transform.position.z;
    }

    void Update()
    {

        //Left
        if ((Input.GetKey(KeyCode.LeftArrow)))
        {
            posX -= camera_velocity * Time.deltaTime;
            posZ += camera_velocity * Time.deltaTime;
            transform.position = new Vector3(posX, posY, posZ);
        }

        //Rigth
        if ((Input.GetKey(KeyCode.RightArrow)))
        {
            posX += camera_velocity * Time.deltaTime;
            posZ -= camera_velocity * Time.deltaTime;
            transform.position = new Vector3(posX, posY, posZ);
        }
        //UP
        if ((Input.GetKey(KeyCode.UpArrow)))
        {

            posY += camera_velocity * Time.deltaTime;
            transform.position = new Vector3(posX, posY, posZ);
        }

        //Down
        if ((Input.GetKey(KeyCode.DownArrow)))
        {
            posY -= camera_velocity * Time.deltaTime;
            transform.position = new Vector3(posX, posY, posZ);
        }
    }
}
