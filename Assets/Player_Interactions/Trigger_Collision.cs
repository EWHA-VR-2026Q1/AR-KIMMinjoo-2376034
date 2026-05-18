using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger_Collision : MonoBehaviour
{
    [Header("충돌을 감지하여 Interface(Event)로 전달하는 클래스")]
    [Header("--------")]
    [Header("충돌 진입 --> Interface의 OnEnterEvent 유발")]
    [Header("충돌 지속 --> Interface의 OnActionEvent 유발")]
    [Header("충돌 이탈 --> Interface의 OnExitEvent 유발")]
    [Header("--------")]

    [Header("충돌 Tag 특정")]
    [Tooltip("체크 시 특정 태그를 가진 물체에게만 반응.")]
    public bool useTagFilter = true;
    [Tooltip("반응할 태그 기입")]
    public List<string> targetTags = new List<string> { "Player" };

    [Header("충돌 대상(게임오브젝트) 특정")]
    [Tooltip("할당시 특정 게임오브젝트에게만 반응")]
    public GameObject specificTarget;

    [Header("Interface가 붙어있는 게임오브젝트")]
    [Tooltip("Interface가 붙어있는 게임오브젝트 할당 \nInterface 컴포넌트는 할당할 수 없음 \n미할당시 현재 게임오브젝트에서 Interface 컴포넌트를 찾음")]
    public GameObject InterfaceObject; 
    IInteractable Interface;

    void Awake(){
        // Debug.unityLogger.logEnabled = false;

        if(InterfaceObject == null){
            InterfaceObject = gameObject;
        }
            
        if(specificTarget == null && targetTags.Count == 0){
            Debug.LogWarning($"<color=yellow>[Trigger_Collision]</color> {gameObject.name}에 필터가 설정되지 않았습니다. 모든 충돌체에 반응합니다.");
        }
        
        Interface = InterfaceObject.GetComponent<IInteractable>();
    }

    // 필터링 로직: 아래 함수가 true를 반환할 때만 이벤트 실행함
    private bool IsValidTarget(GameObject obj)
    {
        // 1. 특정 오브젝트와 일치하는지 확인
        if (specificTarget != null && obj == specificTarget)
        {
            return true;
        }

        // 2. 태그 필터 조건에 맞는지 확인
        if (useTagFilter)
        {
            foreach (string tag in targetTags)
            {
                if (obj.CompareTag(tag)) return true;
            }
            return false; // 리스트에 있는 어떤 태그와도 맞지 않음
        }

        // 3. 필터가 아예 설정되지 않은 경우 (옵션)
        // 만약 특정 타겟도 없고 태그 필터도 안 쓴다면 모두 허용할지 결정해야 합니다.
        if (specificTarget == null && !useTagFilter)
        {
            return true;
        }

        return false;
    }

    #region Trigger Events
    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log($"OnTriggerEnter:{other.name} detected.");
        if (IsValidTarget(other.gameObject))
        {
            Debug.Log($"{other.name} Is Valid Target");            
            Interface?.OnEnter(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if (IsValidTarget(other.gameObject))
        {
            Interface?.OnStay(other.gameObject);  
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        Debug.Log($"OnTriggerExit:{other.name} detected.");
        if (IsValidTarget(other.gameObject))
        {
            Debug.Log($"{other.name} Is Valid Target");
            Interface?.OnExit(other.gameObject);  
        }
    }
    #endregion

    #region Collision Events (동일 로직 적용)
    private void OnCollisionEnter(Collision collision) 
    {
        Debug.Log($"OnCollisionEnter:{collision.gameObject.name} detected.");
        if (IsValidTarget(collision.gameObject))
        {
            Debug.Log($"{collision.gameObject.name} Is Valid Target");
            Interface?.OnEnter(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision) 
    {
        if (IsValidTarget(collision.gameObject))
        { 
            Interface?.OnStay(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision) 
    {
        Debug.Log($"OnCollisionExit:{collision.gameObject.name} detected.");
        if (IsValidTarget(collision.gameObject))
        {
            Debug.Log($"{collision.gameObject.name} Is Valid Target");
            Interface?.OnExit(collision.gameObject);
        }
    }
    #endregion
}
