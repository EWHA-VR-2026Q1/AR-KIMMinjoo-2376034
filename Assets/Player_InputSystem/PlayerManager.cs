using System;
using UnityEngine;

public enum TargetPlatform { PC, Quest }

// [이동 상태] 현재 어떤 이동 컴포넌트가 주도권을 가졌는가?
public enum PlayerMoveState { Ground, Air, Climb, Teleport }

// [상호작용 상태] 손의 상태가 어떠한가?
public enum PlayerInteractionState { Idle, Grab, Item }

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    [Header("Instance")]
    public static PlayerManager Instance; // 싱글톤 패턴 (선택 사항: 물체들이 PlayerManager.Instance로 쉽게 접근 가능)

    [Header("Platform Switch")]
    // 인스펙터에 드롭다운 메뉴로 등장합니다.
    public TargetPlatform CurrentPlatform;

    [Header("플랫폼 설정 에셋 (SO)")]
    public PlatformConfigData PlatformConfigData_PC; // 여기에 PC_Config 또는 Quest_Config만 갈아 끼웁니다.
    public PlatformConfigData PlatformConfigData_Quest; // 여기에 PC_Config 또는 Quest_Config만 갈아 끼웁니다.
    private PlatformConfigData activeConfig; // 런타임에 최종적으로 채택되어 사용될 활성화 SO


    // 다른 스크립트들이 가져다 쓸 최종 변수들
    private Transform playerCamera;
    private Transform pointingHand;
    private Transform teleportHand;
    private Transform grabHolder;
    private Transform itemHolder;


    [Header("Current States")]
    public PlayerMoveState MoveState = PlayerMoveState.Ground;
    public PlayerInteractionState CurrentInteractionState = PlayerInteractionState.Idle;

    //[Header("Event")]
    public event Action<PlayerMoveState> OnMoveStateChanged;
    public event Action<PlayerInteractionState> OnInteractionStateChanged;


    [Header("Core Values")]
    public float Gravity = -9.81f;


    [Header("Interaction Data")]
    public GameObject CurrentObject; // 현재 잡고 있는 물체 (없으면 null)


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DeterminePlatform(); // 1. 어떤 SO를 쓸지 먼저 결정
        InitializePlatform(); // 2. 결정된 SO로 리그(Rig) 오브젝트 세팅
    }

    private void DeterminePlatform()
    {
        
        // 링크 모드 및 빌드 환경 자동 감지를 적용하되, 수동 드롭다운 설정도 존중하는 구조
#if UNITY_ANDROID && !UNITY_EDITOR
            CurrentPlatform = TargetPlatform.Quest;
#elif UNITY_EDITOR
        // 메타 링크로 VR이 켜져 있다면 자동으로 퀘스트 모드, 아니면 드롭다운 설정을 따름
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            CurrentPlatform = TargetPlatform.Quest;
        }
#endif
        
        // 최종 선택된 플랫폼에 맞춰 activeConfig를 매핑합니다.
        if (CurrentPlatform == TargetPlatform.PC)
        {
            activeConfig = PlatformConfigData_PC;
            Debug.Log("<color=white>[Platform]</color> <b>PC 환경 활성화</b>");
        }
        else
        {
            activeConfig = PlatformConfigData_Quest;
            Debug.Log("<color=cyan>[Platform]</color> <b>Quest VR 환경 활성화</b>");
        }
    }

    private void InitializePlatform()
    {
        if (activeConfig == null)
        {
            Debug.LogError("[PlayerManager] 활성화된 PlatformConfig 데이터가 없습니다! 인스펙터를 확인하세요.");
            return;
        }

        PlatformRigReferences[] allRigs = GetComponentsInChildren<PlatformRigReferences>(true);

        foreach (var rig in allRigs)
        {
            // activeConfig에 적힌 플랫폼 타입과 일치하는지 검사합니다.
            if (rig.Platform == activeConfig.CurrentPlatform)
            {
                rig.gameObject.SetActive(true);

                // 최종 주소록 등록
                playerCamera = rig.MainCamera;
                pointingHand = rig.PointingHand;
                teleportHand = rig.TeleportHand;
                grabHolder = rig.GrabHolder;
                itemHolder = rig.ItemHolder;
            }
            else
            {
                rig.gameObject.SetActive(false); // 안 쓰는 플랫폼 끄기
            }
        }
    }

    /*
    private void InitializePlatform()
    {
        if (PlatformConfig == null) return;

        // 1. 자식들 중에서 '주소록(PlatformRigReferences)'을 가진 오브젝트들을 다 찾습니다.
        // true를 넣으면 켜져있든 꺼져있든 다 찾아옵니다.
        PlatformRigReferences[] allRigs = GetComponentsInChildren<PlatformRigReferences>(true);

        foreach (var rig in allRigs)
        {
            // 2. SO 에셋에 적힌 플랫폼과 일치하는지 확인
            if (rig.Platform == PlatformConfig.CurrentPlatform)
            {
                rig.gameObject.SetActive(true); // 켜기

                // 최종 주소록 등록
                playerCamera = rig.MainCamera;
                pointingHand = rig.PointingHand;
                teleportHand = rig.TeleportHand;
                grabHand = rig.GrabHand;
                itemHand = rig.ItemHand;
            }
            else
            {
                rig.gameObject.SetActive(false); // 안 쓰는 플랫폼은 자동으로 끄기
            }
        }
    }
    */

    public Transform GetCamera() => playerCamera;
    public Transform GetPointingHand() => pointingHand;
    public Transform GetTeleportHand() => teleportHand;
    public Transform GetGrabHolder() => grabHolder;
    public Transform GetItemHolder() => itemHolder;

    public void SetMoveState(PlayerMoveState newState)
    {
        if (MoveState == newState) return;

        MoveState = newState;
        Debug.Log($"<color=yellow>[MoveState]</color> <b>{newState}</b>");

        OnMoveStateChanged?.Invoke(newState);
    }

    public PlayerMoveState GetCurrentMoveState() => MoveState;

    public void SetInteractionState(PlayerInteractionState newState)
    {
        Debug.Log($"<color=yellow>[InteractionState]</color> SetInteractionState: <b>newState = {newState}</b>");
        if (CurrentInteractionState == newState) return;
        CurrentInteractionState = newState;
        Debug.Log($"<color=yellow>[InteractionState]</color> SetInteractionState: <b>CurrentInteractionState = {CurrentInteractionState}</b>");

        OnInteractionStateChanged?.Invoke(newState);
    }

    public PlayerInteractionState GetInteractionState()
    {
        return CurrentInteractionState;
    }
}