using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    CharacterController cc; //캐릭터 컨트롤러 컴포넌트
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector3 dir = new Vector3(h, 0, v);

        //카메라가 보는 방향으로 이동해야 한다.
        dir = Camera.main.transform.TransformDirection(dir);
        //transform.Translate(dir * speed * Time.deltaTime);
        dir.Normalize();

        //심각한 문제 : 하늘 날아다님, 땅 뚫음, 충돌처리 안됨
        //캐릭터 컨트롤러 컴포넌트를 사용한다.
        //캐릭터 컨트롤러는 충돌감지만 하고 물리가 적용 안된다.
        //따리서 충돌감지를 하기 위해서는 반드시
        //캐릭터 컨트롤러 컴포넌트가 제공해주는 함술 이동처리해야 한다.
        cc.Move(dir * Time.deltaTime * speed);
    }
}
