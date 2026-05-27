using UnityEngine;

// 이 어트리뷰트 덕분에 유니티 에디터에서 마우스 우클릭으로 파일을 만들 수 있게 됩니다.
[CreateAssetMenu(fileName = "New Platform Config", menuName = "Scriptable Object/Platform Config/Create New Config")]
public class PlatformConfigData : ScriptableObject
{
    [Header("Platform Settings")]
    public PlatformType CurrentPlatform; // PC 또는 Quest 선택
    public string ConfigName;           // 화면에 띄울 이름 (예: "PC Mode", "Quest VR Mode")

    [Header("Core Values")]
    public float Gravity = -9.81f;      // 플랫폼별 중력 수치 (필요시 조절)
}