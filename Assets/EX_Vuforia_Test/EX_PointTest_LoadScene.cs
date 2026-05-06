using UnityEngine;
using UnityEngine.InputSystem; // Input System 필수 네임스페이스
using UnityEngine.SceneManagement;

public class EX_PointTest_LoadScene : MonoBehaviour
{
    public string SceneName = "MyScene";

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            ExecuteRaycast(screenPosition);
        }
    }

    void ExecuteRaycast(Vector2 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"터치 발생. {SceneName}으로 씬 전환.");
            SceneManager.LoadScene(SceneName);
        }
    }

    void LoadScene(string _SceneName)
    {
        SceneName = _SceneName;                    
        SceneManager.LoadScene(SceneName);
    }
}