// using System.Collections;
// using System.Collections.Generic;
// using System.Numerics;
// using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_ikim : MonoBehaviour
{
    public Transform MainCamera;
    // Start is called before the first frame update
    public float Speed = 5f;
    public float maxSpeed = 10f;
    public int jumpCount = 0;
    public int maxJumpCount = 2; // 최대 점프 횟수

    public int ammo;
    public int maxAmmo;

    public int coin;
    public int maxCoin;
    public int health;
    public int maxHealth;
    
    public int hasGrenade;
    public int maxHasGrenade;

    public int score;

    public GameObject[] weapons;
    // public bool[] hasWeapons = new bool[3] { false, false, false }; // 3개의 무기 슬롯
    public bool[] hasWeapons;
    GameObject nearObject;
    
    
    public float hAxis;
    public float vAxis;
    public float uiVAxis = 0f;
    public float uiHAxis = 0f;
    bool wDown;
    public bool jDown = false;   // 키보드 입력
    public bool uiJDown = false;
    
    bool iDown;
    
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool sDown4;
    bool dDown;

    bool isJump;
    bool isDodge;
    bool isSwap;
    
    Vector3 moveVec;
    Rigidbody rigid;
    Animator anim;

    // Attack
    //bool fDown;
    public bool fDown = false;    // 키보드/마우스 입력
    public bool uiFDown = false;  // UI 입력
    bool isFireReady;
    float fireDelay;
    public Weapon equipWeapon;

    int equipWeaponIndex = -1; // 현재 장착된 무기 인덱스
    
    [SerializeField]
    float maxSlopeAngle = 45f;
    [SerializeField]
    float rayDistance = 1.2f; // Raycast 거리

    //idle 
    float idleTimer = 0f;
    public float actionInterval = 5f;

    private bool cursorVisible = false;
    //public float dodgeForce = 15f;
    public bool requestDodge = false;
    public void RequestDodge()
    {
        requestDodge = true;
    }
    [SerializeField]
    float alignSpeed = 10f; // 인스펙터에서 회전 속도 조절

    // 카메라의 수평(Yaw) 방향으로 플레이어를 정렬합니다.
    // instant = true 로 호출하면 즉시 회전(기존 동작)
    void AlignToCameraYaw(bool instant = false)
    {
        if (MainCamera == null) return;

        Vector3 camForward = MainCamera.forward;
        camForward.y = 0f;
        if (camForward.sqrMagnitude < 0.0001f) return;

        Quaternion targetYaw = Quaternion.Euler(0f, Quaternion.LookRotation(camForward).eulerAngles.y, 0f);

        if (instant || alignSpeed <= 0f)
        {
            transform.rotation = targetYaw;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetYaw, Time.deltaTime * alignSpeed);
        }
    }

    /* 거칠게 회전
     // void AlignToCameraYaw()
    // {
    //     if (MainCamera == null) return;
    //     Vector3 camForward = MainCamera.forward;
    //     camForward.y = 0f; // 수평 방향만 사용
    //     if (camForward.sqrMagnitude < 0.0001f) return;
    //     Quaternion target = Quaternion.LookRotation(camForward);
    //     transform.rotation = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
    // } */

    

    void Start()
    {
        Cursor.visible = cursorVisible;
        Cursor.lockState = CursorLockMode.Locked; // 기본적으로 커서를 잠금 상태로 설정
        // Screen.orientation = ScreenOrientation.LandscapeLeft;
        // // Portrait 자동 회전 비활성화 (안전장치)
        // Screen.autorotateToPortrait = false;
        // Screen.autorotateToPortraitUpsideDown = false;
        // // Landscape 허용(필요하면)
        // Screen.autorotateToLandscapeLeft = true;
        // Screen.autorotateToLandscapeRight = true;
    }
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //PlayerPrefs.SetInt("MaxScore", 111000);
        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 111000);
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("jump" + isJump + ", " + "dodge" + isDodge + ", " + "swap" + isSwap);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        idleTimer += Time.deltaTime;
        if (idleTimer > actionInterval)
        {           
            anim.SetTrigger("goIdle");
            idleTimer = 0f;
        }
        
        //  if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        //     {
        //         anim.SetTrigger("doShot");
        //         idleTimer = 0f;
        //     }

        SetCursorVisibility();
         // Update나 터치-드래그 처리부 상단에:
        // bool anyTouch = Input.touchCount > 0;
        // bool touchOverUI = AnyTouchOverUI(); // Player_ikim에 넣어둔 헬퍼를 참조 가능하면 사용
        // // bool movementUIActive = Mathf.Abs(uiHAxis) > 0f || Mathf.Abs(uiVAxis) > 0f;
        // // UI 이동 조작 중이거나 터치가 UI 위라면 카메라 회전 무시
        // if (anyTouch || touchOverUI)
        // {
        //     // 카메라 회전 처리 스킵
        //     return;
        // }


        GetInput();
        if (Mathf.Abs(uiVAxis) > Mathf.Abs(vAxis))
        {
            vAxis = uiVAxis;
        }
        if (Mathf.Abs(uiHAxis) > Mathf.Abs(hAxis))
        {
            hAxis = uiHAxis;
        }
        Turn();
        Move();

        bool jumpInput = jDown || uiJDown;

        if (jumpInput)
        {
            Jump(); // 원하는 점프 함수 호출
            uiJDown = false; // 점프 후 UI 입력 플래그 초기화, 단발 이벤트
        }
        //Jump();
        bool fireInput = fDown || uiFDown;

        if (fireInput)
        {
            Attack();    // 총알 발사 함수 호출
            uiFDown = false; // UI는 단발 입력이므로 즉시 초기화
        }

        // bool dodgeInput = dDown || requestDodge;
        // if (dodgeInput)
        // {
        //     Dodge();            
        // }
        Dodge();            
        Swap();
        Interact();
    
   
    }

    void SetCursorVisibility()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            cursorVisible = !cursorVisible; // 토글
            Cursor.visible = cursorVisible;
            Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
    /*오류
    bool AnyTouchOverUI()
    {
        if (EventSystem.current == null) return false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                return true;
        }
        // Editor에서 마우스 테스트도 포함하려면 아래도 사용
// #if UNITY_EDITOR
         if (EventSystem.current.IsPointerOverGameObject()) return true;
// #endif
         return false;
    }*/
    

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        iDown = Input.GetButtonDown("Interact");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");
        dDown = Input.GetButtonDown("Dodge");
    }
    void Move()
    {
        Vector3 forward = MainCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = MainCamera.right;
        right.y = 0;
        right.Normalize();

        // 입력에 따라 카메라 기준으로 이동 벡터 변형
        moveVec = (forward * vAxis + right * hAxis).normalized;

        // 2. 발밑(또는 캐릭터 중심 아래)으로 Raycast 쏴서 지형 normal 얻기
        RaycastHit hit;
        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, rayDistance))
        {
            groundNormal = hit.normal;
        }
        // 2. normal 각도 구하기
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

        // 3. 최대 슬로프 각도 초과시 이동 막기
        if (slopeAngle > maxSlopeAngle)
        {
            // 경사 너무 심하면 이동 금지
            moveVec = Vector3.zero;


        }
        // 3. 이동 벡터를 지형 normal 평면에 투영 (slope 타고 이동)
        Vector3 slopeMoveVec = Vector3.ProjectOnPlane(moveVec, groundNormal).normalized;


        Vector3 targetPosition = rigid.position + slopeMoveVec * Speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        rigid.MovePosition(targetPosition);

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);


    }
    void Turn()
    {

        transform.LookAt(transform.position + moveVec);
    }

    // public void MoveUp() {
    //     transform.Translate(Vector3.up * 1f); 
    // }

    public void Jump()
    {
        // if (jDown && !isJump)
        // if (jDown && moveVec == Vector3.zero && !isDodge && !isSwap)

        // if (jDown && jumpCount < maxJumpCount)
        if ((jDown || uiJDown) && !isJump)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
            
        }
          
        
    }
   

    void Attack()
    {
        if (equipWeapon == null)
        {
            return; // 무기가 장착되어 있지 않으면 공격 불가
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            // 현재 UI 위에 마우스 있음 - 공격 입력 무시
            // if(!uiFDown)
            return;
        }

        if (Input.touchCount > 0)
        {
            return;
        }

        // if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        // if (Input.touchCount > 0)
        // {
        //     // // 현재 첫 번째 터치 위치로 UI 터치 확인 (fingerId 꼭 넘기기)
        //     // if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //     // {
        //         // UI 터치이므로 공격 입력 무시
        //         return;
        //     // }
        // }

        // bool isTouchOverUI = false;
// for (int i = 0; i < Input.touchCount; i++)
// {
//     Touch t = Input.GetTouch(i);
//     if (t.phase == TouchPhase.Began)
//     {
//         if (EventSystem.current.IsPointerOverGameObject(t.fingerId))
//         {
//             isTouchOverUI = true;
//             break;
//         }
//     }
// }

// if (isTouchOverUI)
// {
//     // 방금 UI를 터치해서 입력 시작 = 배경 입력 무시
//     return;
// }

        fireDelay += Time.deltaTime; // 공격 후 재사용 대기 시간 증가
        isFireReady = fireDelay >= equipWeapon.rate; // 공격 가능 여부 판단

        if ((fDown || uiFDown) && isFireReady && !isDodge && !isSwap)
        {
            // Debug.Log("Attack with " + equipWeapon.name);
            // 무기 사용
            AlignToCameraYaw(true);
            Vector3 aimDir = MainCamera != null ? MainCamera.forward.normalized : transform.forward;
            
            if (equipWeapon != null) equipWeapon.Use(aimDir);
            // equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            // isFireReady = false; // 공격 후 재사용 대기 시간 설정
            fireDelay = 0; // 공격 속도에 따라 재사용 대기 시간 설정
            // isFireReady = false; // 공격 후 재사용 대기 시간 설정
            uiFDown = false;
        }
        // UI는 단발 입력이므로 즉시 초기화

    }

    public void Attack2()
    {
        if (equipWeapon == null)
        {
            return; // 무기가 장착되어 있지 않으면 공격 불가
        }

        AlignToCameraYaw(true);
        fireDelay += Time.deltaTime; // 공격 후 재사용 대기 시간 증가
        isFireReady = fireDelay >= equipWeapon.rate; // 공격 가능 여부 판단

        // if ((fDown || uiFDown) && isFireReady && !isDodge && !isSwap)
        // {
        // Debug.Log("Attack with " + equipWeapon.name);
        // 무기 사용

        equipWeapon.Use();
        anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
        // isFireReady = false; // 공격 후 재사용 대기 시간 설정
        fireDelay = 0; // 공격 속도에 따라 재사용 대기 시간 설정
                       // isFireReady = false; // 공격 후 재사용 대기 시간 설정
        uiFDown = false;
        // }
        // UI는 단발 입력이므로 즉시 초기화

    }

    // ...existing code...
    // AI 제안 더 고급적인 dodge 방법
    /*Vector3 GetDashDirection()
    {
        // UI 입력(uiHAxis/uiVAxis)이 우선, 그 외엔 물리적 축(hAxis/vAxis) 사용
        float hx = Mathf.Abs(uiHAxis) > Mathf.Abs(hAxis) ? uiHAxis : hAxis;
        float vz = Mathf.Abs(uiVAxis) > Mathf.Abs(vAxis) ? uiVAxis : vAxis;

        Vector3 dir = MainCamera.forward * vz + MainCamera.right * hx;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f)
        {
            // 입력 없으면 플레이어 바라보는 방향으로 대시
            dir = transform.forward;
        }
        else
        {
            dir.Normalize();
        }

        return dir;
    }*/
    /*
    Vector3 GetDashDirection()
    {
        // 1) 이동 입력(조이스틱/버튼)이 있으면 그 방향(moveVec)으로 대시
        if (moveVec.sqrMagnitude > 0.01f)
        {
            Vector3 m = moveVec;
            m.y = 0f;
            return m.normalized;
        }

        // 2) 이동 입력이 없으면 플레이어가 바라보는 방향으로 대시
        Vector3 dir = transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f)
        {
            // 최후 보루: 카메라 앞방향
            dir = MainCamera.forward;
            dir.y = 0f;
        }
        return dir.normalized;
    }

    void Dodge()
    {
        if (dDown && !isSwap && !isDodge)
        {
            isDodge = true;

            // 속도 처리(중첩 방지 필요하면 baseSpeed 사용 권장)
            if (Speed < maxSpeed)
            {
                Speed *= 1.5f;
            }

            Vector3 dashDir = GetDashDirection();
            rigid.AddForce(dashDir * dodgeForce, ForceMode.Impulse);

            anim.SetTrigger("doDodge");

            CancelInvoke(nameof(DodgeOut));
            Invoke(nameof(DodgeOut), 0.4f);
        }
    }

    public void Dodge2()
    {
        if (!isSwap && !isDodge)
        {
            isDodge = true;

            if (Speed < maxSpeed)
            {
                Speed *= 1.5f;
            }

            Vector3 dashDir = GetDashDirection();
            rigid.AddForce(dashDir * dodgeForce, ForceMode.Impulse);

            anim.SetTrigger("doDodge");

            CancelInvoke(nameof(DodgeOut));
            Invoke(nameof(DodgeOut), 0.4f);
        }
    }*/



    void Dodge()
    {

        if ((dDown || requestDodge) && !isSwap && !isDodge)
        {
            requestDodge = false;
            isDodge = true;

            // 대시 속도 증가
            if (Speed < maxSpeed)
            {
                Speed *= 1.5f; // 대시 속도 증가
            }
            rigid.AddForce(transform.forward * 15, ForceMode.Impulse);

            anim.SetTrigger("doDodge");

            CancelInvoke(nameof(DodgeOut));
            Invoke("DodgeOut", 0.4f);
        }
    }
    void DodgeOut()
    {
        Speed /= 1.5f; // 대시 속도 원래대로
        isDodge = false;

    }
    public void Dodge2()
    {
        
            if (!isSwap && !isDodge)
            {
                isDodge = true;
                
                 // 대시 속도 증가
                if (Speed < maxSpeed)
                {
                    Speed *= 1.5f; // 대시 속도 증가
                }
                rigid.AddForce(transform.forward * 15, ForceMode.Impulse);

                anim.SetTrigger("doDodge");
            
                Invoke("DodgeOut", 0.4f);
            }
    
        
    }
    

    void Swap()
    {
        int weaponIndex = -1;

        // if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
        // {
        //     return;
        // }
        // else if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
        // {
        //     return;
        // }
        // else if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
        // {
        //     return;
        // }
        // else if (sDown4 && (!hasWeapons[3] || equipWeaponIndex == 3))
        // {
        //     return;
        // }

        // if (sDown1 && hasWeapons[0]) weaponIndex = 0;
        // if (sDown2 && hasWeapons[1]) weaponIndex = 1;
        // if (sDown3 && hasWeapons[2]) weaponIndex = 2;
        // if (sDown4 && hasWeapons[3]) weaponIndex = 3;

        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        if (sDown4) weaponIndex = 3;
        // if (weaponIndex == equipWeaponIndex)
        // {
        //     return; // 이미 장착된 무기면 교체하지 않음
        // }


        if ((sDown1 || sDown2) && !isJump && !isDodge)
        {
            // weapons[weaponIndex].SetActive(true);
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            // 원래 잘 되는 것
            // weapons[weaponIndex].SetActive(true);
            //   무기 교체
            // if (equipWeapon != null)
            // {
            //     // equipWeapon.gameObject.SetActive(false);
            //     equipWeapon.gameObject.SetActive(false);
            // }

            //equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();

            // equipWeapon.SetActive(true);
            //equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }



    }

    public void Swap1()
    {

        int weaponIndex = 0;
        if (!isJump && !isDodge)
        {
            // weapons[weaponIndex].SetActive(true);
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    public void Swap2()
    {

        int weaponIndex = 1;
        if (!isJump && !isDodge)
        {
            // weapons[weaponIndex].SetActive(true);
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }



        void SwapOut()
    {
        
        isSwap = false;

    }
    void Interact()
    {

        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            // Debug.Log("Interaction with " + nearObject.name);
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                // nearObject = null; // 상호작용 후에는 상호작용 오브젝트 초기화

            }

        }
    }
    public void Interact2()
    {

        if (nearObject != null && !isJump && !isDodge)
        {
            // Debug.Log("Interaction with " + nearObject.name);
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                // nearObject = null; // 상호작용 후에는 상호작용 오브젝트 초기화

            }
            
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
        //rigid.velocity = Vector3.zero; // 이동 멈춤 AI추천
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            jumpCount = 0; // 바닥에 닿으면 점프 횟수 초기화
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other == null || other.gameObject == null)
            return;
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
        if (nearObject != null)  // 안전하게 확인
        {
            Debug.Log(nearObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null) return;

        if (other.tag == "Weapon")
        {
            if (nearObject == other.gameObject) nearObject = null;
            //nearObject = null;
        }
        else if (other.tag == "Shop")
        {
            Shop shop = other.GetComponent<Shop>();
            if (shop != null) shop.Exit();

            if (nearObject == other.gameObject) nearObject = null;
            // Shop shop = nearObject.GetComponent<Shop>();
            // shop.Exit();
            // nearObject = null;
        }
    }    

}
