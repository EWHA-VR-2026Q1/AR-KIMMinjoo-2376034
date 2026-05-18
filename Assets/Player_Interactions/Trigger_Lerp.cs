using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System 네임스페이스 추가

public class Trigger_Lerp : MonoBehaviour
{
    public List<Transform> TargetPoints;
    public Transform NextPoint;
    private bool isLerping = false;
    private int index = 0;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        // 시작 시 NextPoint가 없다면 첫 번째 타겟으로 설정
        if (NextPoint == null && TargetPoints.Count > 0)
        {
            NextPoint = TargetPoints[index];
        }
    }

    private void Update()
    {
        // 1. 입력 감지 (New Input System)
        HandleInput();

        // 2. Lerp 이동 로직
        if (isLerping && NextPoint != null)
        {
            float dist = Vector3.Distance(transform.position, NextPoint.position);
            if (dist > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, NextPoint.position, 0.03f);
            }
            else
            {
                // 목적지 도착 시 위치 고정 및 정지
                transform.position = NextPoint.position;
                isLerping = false;
            }
        }
    }

    private void HandleInput()
    {
        var pointer = Pointer.current;
        if (pointer == null) return;

        // 클릭(또는 터치) 순간 감지
        if (pointer.press.wasPressedThisFrame)
        {
            // 화면의 포인터 위치로부터 레이(Ray) 발사
            Vector2 screenPosition = pointer.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 클릭된 물체가 이 스크립트가 붙은 오브젝트인지 확인
                if (hit.transform == transform)
                {
                    StartLerping();
                }
            }
        }
    }

    private void StartLerping()
    {
        if (TargetPoints.Count == 0) return;

        isLerping = true;
        index++;

        if (index >= TargetPoints.Count)
        {
            index = 0;
        }

        NextPoint = TargetPoints[index];
        Debug.Log($"Next Target: {NextPoint.name}");
    }
}