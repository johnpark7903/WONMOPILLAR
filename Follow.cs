using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Follow : MonoBehaviour
{
    public Transform Player;   // 따라갈 플레이어 트랜스폼
    public Vector3 offset = new Vector3(0, 3, -5);     // 카메라 위치 오프셋 (y는 높이)
    public float sensitivity = 3f;  // 마우스 감도
    public float distance = 6f;     // 플레이어와 카메라 거리
    public float minY = -35f;       // 카메라 각도 제한(아래)
    public float maxY = 70f;        // 카메라 각도 제한(위)

    // smoothing 파라미터
    public float smoothTime = 0.08f;    // 각도/위치 스무딩 타임 (작을수록 빠름)
    public float rotationLerp = 10f;    // 회전 보간 속도 (Slerp 계수)
    public float positionLerp = 10f;    // 위치 보간 속도 (Lerp 계수)

    float yawTarget = 0f;   // 목표 좌우 회전 (마우스 누적)
    float pitchTarget = 20f; // 목표 상하 회전

    float yawCurrent = 0f;   // 현재 보간된 yaw
    float pitchCurrent = 20f; // 현재 보간된 pitch

    float yawVel = 0f;      // SmoothDamp 내부값
    float pitchVel = 0f;

    Vector3 posVelocity;    // SmoothDamp 위치용

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 커서 고정 (필요시 변경)
        Vector3 e = transform.eulerAngles;
        yawTarget = yawCurrent = e.y;
        pitchTarget = pitchCurrent = e.x;
    }

    void LateUpdate()
    {
        if (Player == null) return;

        // 마우스 입력 누적(목표값)
        yawTarget += Input.GetAxis("Mouse X") * sensitivity;
        pitchTarget -= Input.GetAxis("Mouse Y") * sensitivity;
        pitchTarget = Mathf.Clamp(pitchTarget, minY, maxY);

        // 목표값 -> 현재값으로 부드럽게 보간 (각도 보간에 SmoothDampAngle 사용)
        yawCurrent = Mathf.SmoothDampAngle(yawCurrent, yawTarget, ref yawVel, smoothTime);
        pitchCurrent = Mathf.SmoothDampAngle(pitchCurrent, pitchTarget, ref pitchVel, smoothTime);

        // 회전 계산
        Quaternion targetRot = Quaternion.Euler(pitchCurrent, yawCurrent, 0f);

        // 카메라 위치 계산 (플레이어 기준 뒤쪽으로 distance 유지 + offset.y 높이)
        Vector3 desiredPos = Player.position + targetRot * new Vector3(0f, 0f, -distance) + Vector3.up * offset.y;

        // 위치/회전 부드럽게 적용
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref posVelocity, smoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLerp);
    }
}


/* 백업 거친 회전.
public class Follow : MonoBehaviour
{
    public Transform Player;   // 따라갈 플레이어 트랜스폼
    public Vector3 offset = new Vector3(0, 3, -5);     // 카메라 위치 오프셋
    public float sensitivity = 3f;  // 마우스 감도
    public float distance = 6f;     // 플레이어와 카메라 거리
    public float minY = -35f;       // 카메라 각도 제한(아래)
    public float maxY = 70f;        // 카메라 각도 제한(위)

    float yaw = 0f;                 // 좌우 회전
    float pitch = 20f;              // 상하 회전

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 커서 고정
    }
    void Update()
    {
        transform.position = Player.position + offset;
    }

    void LateUpdate()
    {
        // 마우스 입력
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        // 상하 각도 제한
        pitch = Mathf.Clamp(pitch, minY, maxY);

        // 카메라 회전 적용
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // 플레이어 뒤쪽으로 오프셋 거리 유지
        Vector3 camPosition = Player.position + rotation * new Vector3(0, 0, -distance) + Vector3.up * offset.y;
        transform.position = camPosition;
        transform.LookAt(Player.position + Vector3.up * offset.y);
    }
    // Start is called before the first frame update

    // Update is called once per frame
    // public Transform target;
    // public Vector3 offset;


    // #2
    // public float sensitivityX = 15.0f; // 좌우 민감도
    // public float sensitivityY = 15.0f; // 상하 민감도

    // public float minimumY = -60.0f; // 상하 각도 제한
    // public float maximumY = 60.0f;

    // float rotationY = 0.0f;

    // void Update()
    // {
    //     float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

    //     rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
    //     rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

    //     transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    // }
} */
