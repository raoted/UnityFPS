using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFollow : MonoBehaviour
{
    //카메라가 플레이어를 따라다니기
    //플레이어한테 바로 카메라를 붙여서 이동해도 상관없다
    //하지만 게임에 따라서 드라마틱한 연출이 필요한 경우에
    //타겟을 따라다니도록 하는게 1인칭에서 3인칭으로 또는 그 반대로 변경이 쉽다.
    //또한 순간이동이 아닌 슈팅게임에서 꼬랑지가 따라다니는 것 같은 효과도 연출이 가능하다.

    public Transform target;    //카메라가 따라다닐 타겟
    public float followSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 위치를 강제로 타겟 위치에 고정시킨다.
        //transform.position = target.position;
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.position = target.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Vector3 vec = target.position;
            vec += new Vector3(0, 2, -3);

            transform.position = vec;
        }
        //FollowTarget();
    }

    private void FollowTarget()
    {
        //타겟방향 구하기 (벡터의 뺄셈)
        //방향 = 타겟 - 자기자신

        Vector3 dir = target.position - transform.position;
        
        dir.Normalize();
        transform.Translate(dir * followSpeed * Time.deltaTime);

        if(Vector3.Distance(target.position, transform.position) >= 1.0f)
        {
            transform.position = target.position;
        }
    }
}
