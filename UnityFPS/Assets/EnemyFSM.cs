using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

//몬스터 유한 상태 머신
public class EnemyFSM : MonoBehaviour
{
    //몬스터 상태 이넘문
    enum EnemyState
    {
        Idle, Move, Attack, Return, Damaged, Die
    }


    [SerializeField] EnemyState state;   //몬스터 상태 변수
    GameObject objectPlayer;
    float distance;

    #region "Idle 상태에 필요한 변수들"
    #endregion

    #region "Move 상태에 필요한 변수들"
    #endregion
    float moveSpeed = 5.0f;
    CharacterController cc;
    #region "Attack 상태에 필요한 변수들"
    #endregion
    float attackRate = 3.0f;
    float attackTime = 0.0f;
    #region "Return 상태에 필요한 변수들"
    #endregion
    Vector3 startPoint;
    #region "Damaged 상태에 필요한 변수들"
    #endregion
    int HP = 100;
    #region "Die 상태에 필요한 변수들"
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //상태 초기화
        state = EnemyState.Idle;
        objectPlayer = GameObject.Find("Player");
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, objectPlayer.transform.position);

        //상태에 따른 행동처리
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            default:
                break;
        }
    }

    private void Idle()
    { 
        /**
         * 1. 플레이어와 일정범위가 되면 이동상태로 변경(탐지)
         *  플레이어 찾기 (GameObject.Find("Player"))
         *  일정거리 20m (거리비교 : Distanc 등)
         *  상태변경
         *  상태전환 출력
         */

        if(distance <= 20.0f)
        {
            Debug.Log("추적 시작");
            state = EnemyState.Move;
            startPoint = transform.position;
        }
    }

    private void Move()
    {
        /**
         * 1. 플레이어를 향해 이동후 공격범위 안에 들어오면 공격상태로 전환
         * 2. 플레이어를 추격하더라도 처음 위치에서 일정범위를 넘어가면 돌아가야 한다.
         *  플레이어처럼 캐릭터컨트롤러를 이용하기
         *  공격범위 1미터
         *  상태변경
         *  상태전환 출력
         */

        if(distance <= 1.1f)
        {
            state = EnemyState.Attack;
            Debug.Log("Enemy : 공격");
        }
        else if(Vector3.Distance(startPoint, transform.position) >= 100)
        {
            state = EnemyState.Return;
            Debug.Log("Enemy : 귀환");
        }
        else
        {
            transform.LookAt(objectPlayer.transform);
            Vector3 dir = objectPlayer.transform.position - transform.position;
            dir.Normalize();
            cc.SimpleMove(dir);
        }
    }

    private void Attack()
    {
        /**
         * 1. 플레이어가 공격범위 안에 있다면 일정한 시간 간격으로 플레이어 공격
         * 2. 플레이어가 공격범위를 벗어나면 이동상태로 변경
         *  공격범위 1미터
         *  상태변경
         *  상태전환 출력
         */
         
        if(distance > 1.2f)
        {
            state = EnemyState.Move;
            attackTime = 0f;            
            Debug.Log("Enemy : 추격 시작");
        }
        else if(Vector3.Distance(startPoint, transform.position) >= 100.0f)
        {
            state = EnemyState.Return;
            attackTime = 0f;
            Debug.Log("Enemy : 복귀");
        }
        else
        {
            if (attackTime >= attackRate)
            {
                Debug.Log("Enemy : 공격");
                attackTime = 0.0f;
            }
            else
            {
                attackTime += Time.deltaTime;
            }
        }
    }

    private void Return()
    {
        /**
         * 1. 몬스터가 플레이어를 추격하더라도 처음 위치에서 일정범위 이상 벗어나면 원래 위치로 돌아감
         *  처음 위치에서 일정범위 30미터
         *  상태변경
         *  상태전환 출력
         */

        //시작 위치까지 도달하지 않을 때는 이동
        
        transform.GetComponent<CharacterController>().Move(startPoint);
        if(transform.position == startPoint)
        {
            state = EnemyState.Idle;
            Debug.Log("Enemy : 대기");
        }
        else if(distance <= 20)
        {
            state = EnemyState.Move;
            Debug.Log("추격 시작");
        }
    }
    //플레이어쪽에서 충돌감지를 할 수 있으니 이 함수는 퍼블릭으로 선언
    public void hitDamage(int value)
    {
        //예외처리
        //피격상태이거나 죽은 상태일 경우에는 데미지 중첩으로 주지 않는다.
        if (state == EnemyState.Die || state == EnemyState.Damaged) { return; }

        HP -= value;

        //몬스터의 체력이 1 이상이면 피겨강태
        if(HP > 0)
        {
            state = EnemyState.Damaged;
            Debug.Log("Enemy :  공격받음. HP = " + HP);
            Demaged();
        }
        //0이하면 죽은 상태
        else
        {
            state = EnemyState.Die;
            Debug.Log("Enemy : 사망");
            Die();
        }
    }

    IEnumerator DamageProc()
    {
        //피격모션 시간만큼 기다리기
        yield return new WaitForSeconds(1.0f);
        //현재상태를 이동으로 전환
        state = EnemyState.Move;
        Debug.Log("공격받음");
    }
    //피격상태 (Any State)
    private void Demaged()
    {
        /**
         * 코루틴을 사용하자.
         * 1. 몬스터 체력이 1이상
         * 2. 다시 이전상태로 변경
         *  상태변경
         *  상태전환 출력
         */
        StartCoroutine(DemagedProc());
    }

    //사망상태 (Any State)
    private void Die()
    {
        /**
         * 코루틴을 이용하자
         * 1. 체력이 0이하
         * 2. 몬스터 오브젝트 삭제
         *  상태변경
         *  상태전환 출력(죽었다)
         */

        StopAllCoroutines();

        //죽음 상태를 처리하기 위한 코르틴 실행
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        //캐릭터 컨르롤러 비활성화
        cc.enabled = false;

        //2초 후에 자기 자신을 제거한다.
        yield return new WaitForSeconds(2.0f);
        Debug.Log("죽없다!!");
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Contains("Clone"))
        {
            HP -= 2;
            state = EnemyState.Damaged;
        }
    }
}
