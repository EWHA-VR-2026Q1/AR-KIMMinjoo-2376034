using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HW20 - 제한된 신체와 현실 판단 실험
/// Drag / Snap / Lerp 인터랙션 구현
/// 
/// 필수 설정:
///   - 이 스크립트가 붙은 오브젝트에 Box Collider (Is Trigger: OFF) 필요
///   - Inspector에서 snapPoints 배열에 SnapPoint 오브젝트들 연결
///   - arCamera에 AR Camera 연결 (비워두면 Camera.main 자동 사용)
/// </summary>
public class DragSnapLerpInteraction : MonoBehaviour
{
    [Header("=== Snap 설정 ===")]
    public Transform[] snapPoints;          // Inspector에서 연결할 SnapPoint들
    public float snapDistance = 0.3f;       // 이 거리 이내면 Snap 발동

    [Header("=== Lerp 설정 ===")]
    public float lerpSpeed = 8f;            // 클수록 빠르게 이동

    [Header("=== 카메라 설정 ===")]
    public Camera arCamera;                 // 비워두면 Camera.main 자동 사용

    [Header("=== 색상 피드백 ===")]
    public Color normalColor = Color.white;
    public Color draggingColor = Color.yellow;
    public Color snappedColor = Color.green;

    // ── 내부 상태 ──────────────────────────────────────────────
    private bool isDragging = false;
    private bool isSnapped = false;
    private Transform snappedTo = null;

    private Vector3 targetPosition;          // Lerp 목표 위치
    private Vector3 dragOffset;              // 터치 시작 시 오브젝트 중심과 터치점의 거리
    private float dragPlaneY;              // 드래그 평면 Y 좌표 (오브젝트 높이 고정)

    private Renderer rend;
    private Camera Cam => arCamera != null ? arCamera : Camera.main;

    // ── 디버그 로그 (빌드에서도 확인 가능하게 OnGUI 활용) ──────
    private string debugMsg = "Ready";

    // ───────────────────────────────────────────────────────────
    void Start()
    {
        rend = GetComponent<Renderer>();
        targetPosition = transform.position;
        SetColor(normalColor);

        // Collider 자동 확인
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            Debug.LogWarning("[HW20] Collider 없어서 BoxCollider 자동 추가함");
        }

        Debug.Log("[HW20] DragSnapLerp Start() 완료 / Camera: " + (Cam != null ? Cam.name : "NULL"));
    }

    // ───────────────────────────────────────────────────────────
    void Update()
    {
        HandleInput();
        LerpToTarget();
    }

    // ── 입력 처리 ────────────────────────────────────────────────
    void HandleInput()
    {
        // 모바일
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            debugMsg = "Touch phase: " + touch.phase; // 이게 뜨는지 먼저 확인

            if (touch.phase == TouchPhase.Began)
                OnTouchBegan(touch.position);
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                OnTouchMoved(touch.position);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                OnTouchEnded();
        }
        else if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
                OnTouchBegan(Input.mousePosition);
            else if (Input.GetMouseButton(0))
                OnTouchMoved(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0))
                OnTouchEnded();
        }
        else
        {
            debugMsg = "No touch input"; // 터치 자체가 0개면 여기 뜸
        }
    }

    // ── 터치 시작 ────────────────────────────────────────────────
    void OnTouchBegan(Vector2 screenPos)
    {
        if (Cam == null) { debugMsg = "ERR: Camera NULL"; return; }

        Ray ray = Cam.ScreenPointToRay(screenPos);
        debugMsg = "Ray cast...";

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            debugMsg = "Hit: " + hit.collider.name;

            // 이 오브젝트가 맞았을 때만 드래그 시작
            if (hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                isSnapped = false;
                snappedTo = null;
                dragPlaneY = transform.position.y;        // 현재 높이 고정

                // 터치 → 월드 좌표로 변환 후 오프셋 계산
                Vector3 touchWorld = GetWorldOnPlane(screenPos, dragPlaneY);
                dragOffset = transform.position - touchWorld;

                SetColor(draggingColor);
                debugMsg = "Drag START";
                Debug.Log("[HW20] Drag Start on " + gameObject.name);
            }
        }
        else
        {
            debugMsg = "No hit";
        }
    }

    // ── 터치 이동 ────────────────────────────────────────────────
    void OnTouchMoved(Vector2 screenPos)
    {
        if (!isDragging) return;

        Vector3 touchWorld = GetWorldOnPlane(screenPos, dragPlaneY);
        targetPosition = touchWorld + dragOffset;

        // Snap 검사
        CheckSnap();

        debugMsg = "Dragging → " + targetPosition.ToString("F2");
    }

    // ── 터치 종료 ────────────────────────────────────────────────
    void OnTouchEnded()
    {
        if (!isDragging) return;
        isDragging = false;

        if (isSnapped && snappedTo != null)
        {
            targetPosition = snappedTo.position;    // Snap 위치로 Lerp
            SetColor(snappedColor);
            debugMsg = "Snapped to " + snappedTo.name;
            Debug.Log("[HW20] Snapped to " + snappedTo.name);
        }
        else
        {
            SetColor(normalColor);
            debugMsg = "Released (no snap)";
        }
    }

    // ── Snap 검사 ────────────────────────────────────────────────
    void CheckSnap()
    {
        if (snapPoints == null || snapPoints.Length == 0) return;

        float closestDist = float.MaxValue;
        Transform closestSnap = null;

        foreach (Transform sp in snapPoints)
        {
            if (sp == null) continue;
            float d = Vector3.Distance(targetPosition, sp.position);
            if (d < closestDist)
            {
                closestDist = d;
                closestSnap = sp;
            }
        }

        if (closestDist <= snapDistance)
        {
            isSnapped = true;
            snappedTo = closestSnap;
            // 드래그 중 Snap 범위 안이면 색상 미리 변경
            SetColor(snappedColor);
        }
        else
        {
            isSnapped = false;
            snappedTo = null;
            SetColor(draggingColor);
        }
    }

    // ── Lerp 이동 ────────────────────────────────────────────────
    void LerpToTarget()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * lerpSpeed
        );
    }

    // ── 화면 좌표 → 고정 Y 평면 위 월드 좌표 ────────────────────
    Vector3 GetWorldOnPlane(Vector2 screenPos, float planeY)
    {
        if (Cam == null) return transform.position;

        Ray ray = Cam.ScreenPointToRay(screenPos);

        // 수평 평면(y = planeY)과의 교점 계산
        float t = (planeY - ray.origin.y) / ray.direction.y;
        if (Mathf.Abs(ray.direction.y) < 0.0001f || t < 0f)
        {
            // 카메라가 평면과 거의 평행하거나 뒤에 있을 때 → 일정 거리 앞 지점 사용
            return ray.GetPoint(2.0f);
        }
        return ray.GetPoint(t);
    }

    // ── 색상 설정 ────────────────────────────────────────────────
    void SetColor(Color c)
    {
        if (rend != null)
            rend.material.color = c;
    }

    // ── 화면 디버그 (빌드에서 확인) ─────────────────────────────
    void OnGUI()
    {
        // 빌드 디버그가 필요 없으면 이 메서드 전체 삭제해도 됨
        GUI.color = Color.black;
        GUI.Label(new Rect(12, 12, 400, 30), "[HW20] " + debugMsg);
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 400, 30), "[HW20] " + debugMsg);

        GUI.color = Color.white;
        GUI.Label(new Rect(10, 40, 400, 25),
            $"isDrag={isDragging} | isSnap={isSnapped} | Cam={(Cam != null ? Cam.name : "NULL")}");
        GUI.Label(new Rect(10, 65, 400, 25),
            $"target={targetPosition.ToString("F2")}");
        GUI.Label(new Rect(10, 90, 400, 25),
            $"TouchCount={Input.touchCount}");
    }
}