using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject granadeFactory;
    public GameObject bulletImpactFactory;
    public GameObject firePoint;
    public float throwPower = 20.0f;

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    private void Fire()
    {

        //마우스왼쪽버튼 클릭시 레이캐스트로 총알 발사
        if (Input.GetMouseButtonDown(0))
        {
            //if (Physics.Raycast(transform.position, transform.GetChild(1).TransformDirection(Vector3.forward), out ray, Mathf.Infinity))
            //{
            //    Debug.DrawRay(transform.position, transform.GetChild(1).TransformDirection(Vector3.forward) * ray.distance, Color.yellow);
            //    Debug.Log(ray.transform.gameObject);
            //    GameObject newObject = Instantiate(bulletHoleFactory, ray.point, transform.rotation);
            //}

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;
            //ray와 충돌했는가
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.Log(hitInfo.transform.name);
                GameObject bulletImpact = Instantiate(bulletImpactFactory);
                bulletImpact.transform.position = hitInfo.point;
                //파편이펙트
                //파편이 부딛힌 지점이 향하는 방향으로 튀게 해줘야 한다.
                bulletImpact.transform.forward = hitInfo.normal;
            }

            int layer = gameObject.layer;
            layer = 1 << 8 | 1 << 9 | 1 << 12;

        }
        //마우스우측버튼 클릭시 수류탄투척 하기
        if (Input.GetMouseButtonDown(1))
        {
            GameObject bomb = Instantiate(granadeFactory);
            bomb.transform.position = firePoint.transform.position;

            Rigidbody rb = bomb.GetComponent<Rigidbody>();

            Vector3 dir = (Camera.main.transform.forward + Camera.main.transform.up);
            dir.Normalize();
            rb.AddForce( dir * throwPower, ForceMode.Impulse);
        }
    }
}
