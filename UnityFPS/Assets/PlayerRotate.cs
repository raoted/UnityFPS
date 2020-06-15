using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float speed = 150;
    float angleX;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //플레이어 회전
        Rotate();
    }

    private void Rotate()
    {
        float h = Input.GetAxis("Mouse X");       
        angleX += h * speed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, angleX, 0);
    }
}
