using UnityEngine;
using UnityEngine.InputSystem;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, 0);
    public float followSpeed = 10f;

    // 감도 분리: 마우스(픽셀)와 터치(델타)용
    public float mouseSensitivity = 0.18f;   // 마우스 감도
    public float touchSensitivity = 0.1f;    // 터치 감도
    public float rotationSmooth = 10f;       // 위치/회전 보간 속도
    public float maxUpAngle = 50f;   // 위로(look up) 최대 각도
    public float maxDownAngle = 30f; // 아래로(look down) 최대 각도
    float yaw = 0f;
    float pitch = 10f; // 초기 피치
    float distance;
    Vector3 currentVelocity;

    void Start()
    {
        distance = offset.magnitude;
        Vector3 dir = (transform.position - (target != null ? target.position : Vector3.zero)).normalized;
        // 초기 yaw/pitch를 현재 카메라 방향에서 대략 계산
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        // <-- 추가: Euler 각도를 signed 값으로 변환 (0..360 -> -180..180)
        if (pitch > 180f) pitch -= 360f;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 입력 수집
        Vector2 rawDelta = Vector2.zero;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            rawDelta = Touchscreen.current.primaryTouch.delta.ReadValue() * touchSensitivity;
        }
        else if (Mouse.current != null)
        {
            rawDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;
        }

        // 누적 각도 업데이트 (flip Y as needed)
        yaw += rawDelta.x;
        pitch -= rawDelta.y; // 드래그 위로 올리면 카메라 내려오게 음수
        pitch = Mathf.Clamp(pitch, -maxUpAngle, maxDownAngle);

        // 회전으로 offset 계산
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position + rot * new Vector3(0f, 0f, -distance) + Vector3.up * offset.y;

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref currentVelocity, 1f / rotationSmooth);

        // 타겟을 바라보게 함 (원하면 부드럽게 보간 가능)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}