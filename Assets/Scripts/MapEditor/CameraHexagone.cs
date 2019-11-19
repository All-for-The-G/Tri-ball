using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHexagone : MonoBehaviour
{
    public Transform mapContainer;
    public Transform moveMapContainer;
    public float speedRotation = 90;
    public float speedMove = 10;

    public static bool lockCamera = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if(!lockCamera)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                mapContainer.Rotate(-speedRotation * Time.deltaTime * Vector3.up);

            }

            if (Input.GetKey(KeyCode.E))
            {
                mapContainer.Rotate(speedRotation * Time.deltaTime * Vector3.up);

            }

            mapContainer.localScale = (1 + Time.deltaTime * Input.mouseScrollDelta.y) * mapContainer.localScale;

            float cosAngles = Mathf.Cos(Mathf.PI * mapContainer.localEulerAngles.y / 180f);
            float sinAngles = Mathf.Sin(Mathf.PI * mapContainer.localEulerAngles.y / 180f);



            moveMapContainer.localPosition += -Time.deltaTime * speedMove * (new Vector3(Input.GetAxis("Horizontal") * cosAngles - sinAngles * Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal") * sinAngles + cosAngles * Input.GetAxis("Vertical")));
            //moveMapContainer.localPosition += -Time.deltaTime * speedMove * (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

        }





    }
}
