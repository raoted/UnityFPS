using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//몬스터 유한상태머신
public class EnemyFSM : MonoBehaviour
{
    //몬스터 상태 이넘문
    enum EnemyState
    {
        Idle, Move, Attack, Return, Damaged, Die
    }

    EnemyState state; //몬스터 상태변수

    /// 유용한 기능
    #region "Idel 상태에 필요한 변수들"

    #endregion

    #region "Move 상태에 필요한 변수들"
    #endregion

    #region "Attack 상태에 필요한 변수들"
    #endregion

    #region "Return 상태에 필요한 변수들"
    #endregion

    #region "Damaged 상태에 필요한 변수들"
    #endregion

    #region "Die 상태에 필요한 변수들"
    #endregion

    Animator anim;
    ///필요한 변수들
    public float findRange = 15f; //플레이어를 찾는 범위
    public float moveRange = 30f; //시작지점에서 최대 이동가능한 범위
    public float attackRange = 2f; //공격 가능 범위
    Vector3 startPoint; //몬스터 시작위치
    Transform player;   //플레이어를 찾기위해(안그럼 모든 몬스터에 다 드래그앤드랍 해줘야 한다 걍 코드로 찾아서 처리하기)
    CharacterController cc; //몬스터 이동을 위해 캐릭터컨트롤러 컴포넌트

    ///몬스터 일반변수
    int hp = 100; //체력
    int att = 5; //공격력
    float speed = 5.0f; //이동속도

    //공격 딜레이
    float attTime = 2f; //2초에 한번 공격
    float timer = 0f; //타이머


    void Start()
    {
        //몬스터 상태 초기화
        state = EnemyState.Idle;
        //시작지점 저장
        startPoint = transform.position;
        //플레이어 트렌스폼 컴포넌트
        player = GameObject.Find("Player").transform;
        //캐릭터 컨트롤러 컴포넌트
        cc = GetComponent<CharacterController>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
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
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }
    }//end of void Update()

    //대기상태
    private void Idle()
    {
        //1. 플레이어와 일정범위가 되면 이동상태로 변경 (탐지범위)
        //- 플레이어 찾기 (GameObject.Find("Player"))
        //- 일정거리 20미터 (거리비교 : Distance, magnitue 아무거나)
        //- 상태변경 //state = EnemyState.Move;
        //- 상태전환 출력

        //Vector3 distance = transform.position - player.position;
        //float distance = dir.magnitude;
        //if(distance.magnitude < findRange)
        //if(distance < findRange)

        if (Vector3.Distance(transform.position, player.position) < findRange)
        {
            state = EnemyState.Move;
            print("상태전환 : Idle -> Move");

            //애니메이션
            anim.SetTrigger("Move");
        }

    }

    //이동상태
    private void Move()
    {
        //1. 플레이어를 향해 이동 후 공격범위 안에 들어오면 공격상태로 변경
        //2. 플레이어를 추격하더라도 처음위치에서 일정범위를 넘어가면 리턴상태로 변경
        //- 플레이어 처럼 캐릭터컨트롤러를 이용하기
        //- 공격범위 1미터
        //- 상태변경
        //- 상태전환 출력

        //이동중 이동할 수 있는 최대범위에 들어왔을때
        if (Vector3.Distance(transform.position, startPoint) > moveRange)
        {
            state = EnemyState.Return;
            anim.SetTrigger("Return");
            print("상태전환 : Move -> Return");
        }
        //리턴상태가 아니면 플레이어를 추격해야 한다
        else if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            //플레이어를 추격
            //이동방향 (벡터의 뺄셈)
            Vector3 dir = (player.position - transform.position).normalized;
            //dir.Normalize();

            //몬스터가 백스텝으로 쫓아온다
            //몬스터가 타겟을 바라보도록 하자
            //방법1
            //transform.forward = dir;
            //방법2
            //transform.LookAt(player);

            //좀더 자연스럽게 회전처리를 하고 싶다
            //transform.forward = Vector3.Lerp(transform.forward, dir, 10 * Time.deltaTime);
            //여기도 문제가 있다 지금 회전처리를 하면서 벡터의 러프를 사용한 상태라서
            //타겟과 본인이 일직선상일경우 백덤블링으로 회전을 한다

            //최종적으로 자연스런 회전처리를 하려면 결국 쿼터니온을 사용해야 한다
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(dir),
                10 * Time.deltaTime);

            //캐릭터 컨트롤러를 이용해서 이동하기
            //cc.Move(dir * speed * Time.deltaTime);
            //중력이 적용안되는 문제가 있다

            //중력문제를 해결하기 위해서 심플무브를 사용한다
            //심플무브는 최소한의 물리가 적용되어 중력문제를 해결할 수 있다
            //단 내부적으로 시간처리를 하기때문에 
            //Time.deltaTime을 사용하지 않는다
            cc.SimpleMove(dir * speed);
        }
        else //공격범위 안에 들어옴
        {
            state = EnemyState.Attack;
            print("상태전환 : Move -> Attack");
            anim.SetTrigger("Attack");
        }
    }

    //공격상태
    private void Attack()
    {
        //1. 플레이어가 공격범위 안에 있다면 일정한 시간 간격으로 플레이어 공격
        //2. 플레이어가 공격범위를 벗어나면 이동상태(재추격)로 변경
        //- 공격범위 1미터
        //- 상태변경
        //- 상태전환 출력

        //공격범위안에 들어옴
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            //일정 시간마다 플레이어를 공격하기
            timer += Time.deltaTime;
            if (timer > attTime)
            {
                print("공격");
                //플레이어의 필요한 스크립트 컴포넌트를 가져와서 데미지를 주면 된다
                //player.GetComponent<PlayerMove>().hitDamage(att);

                //타이머 초기화
                timer = 0f;
                anim.SetTrigger("Attack");
            }
        }
        else//현재상태를 무브로 전환하기 (재추격)
        {
            state = EnemyState.Move;
            anim.SetTrigger("Move");
            print("상태전환 : Attack -> Move");
            //타이머 초기화
            timer = 0f;
        }
    }

    //복귀상태
    private void Return()
    {
        //1. 몬스터가 플레이어를 추격하더라도 처음 위치에서 일정 범위를 벗어나면 다시 돌아옴
        //- 처음위치에서 일정범위 30미터
        //- 상태변경
        //- 상태전환 출력

        //시작위치까지 도달하지 않을때는 이동
        //도착하면 대기상태로 변경
        if (Vector3.Distance(transform.position, startPoint) > 0.1)
        {
            Vector3 dir = (startPoint - transform.position).normalized;
            cc.SimpleMove(dir * speed);
        }
        else
        {
            //위치값을 초기값으로 
            transform.position = startPoint;
            transform.rotation = Quaternion.identity;
            //Quaternion.idedtity => 쿼터니온 회전값을 초기값으로 
            state = EnemyState.Idle;
            anim.SetTrigger("Idle");
            print("상태전환 : Return -> Idle");
        }
    }
    //플레이어쪽에서 충돌감지를 할 수 있으니 이함수는 퍼블릭으로 만들자
    public void hitDamage(int value)
    {
        //예외처리
        //피격상태이거나, 죽은 상태일때는 데미지 중첩으로 주지 않는다
        if (state == EnemyState.Damaged || state == EnemyState.Die) return;

        //체력깍기
        hp -= value;

        //몬스터의 체력이 1이상이면 피격상태
        if (hp > 0)
        {
            state = EnemyState.Damaged;
            anim.SetTrigger("Damaged");
            print("상태전환 : AnyState -> Damaged");
            print("HP : " + hp);

            Damaged();
        }
        //0이하이면 죽음상태
        else
        {
            state = EnemyState.Die;
            anim.SetTrigger("Die");
            print("상태전환 : AnyState -> Die");

            Die();
        }
    }

    //피격상태 (Any State)
    private void Damaged()
    {
        //코루틴을 사용하자
        //1. 몬스터 체력이 1이상
        //2. 다시 이전상태로 변경
        //- 상태변경
        //- 상태전환 출력

        //피격 상태를 처리하기 위한 코루틴을 실행한다
        StartCoroutine(DamageProc());
    }

    //피격상태 처리용 코루틴
    IEnumerator DamageProc()
    {
        //피격모션 시간만큼 기다리기
        yield return new WaitForSeconds(1.0f);
        //현재상태를 이동으로 전환
        state = EnemyState.Move;
        anim.SetTrigger("Idle");
        print("상태전환 : Damaged -> Move");
    }

    //죽음상태 (Any State)
    private void Die()
    {
        //코루틴을 사용하자
        //1. 체력이 0이하
        //2. 몬스터 오브젝트 삭제
        //- 상태변경
        //- 상태전환 출력 (죽었다)

        //진행중인 모든 코루틴은 정지한다
        StopAllCoroutines();

        //죽음상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        //캐릭터컨트롤러 비활성화
        cc.enabled = false;

        //2초후에 자기자신을 제거한다
        yield return new WaitForSeconds(2.0f);
        print("죽었다!!");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        //공격가능범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        //플레이어 찾을 수 있는 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, findRange);
        //이동가능한 최대 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint, moveRange);
    }
}
