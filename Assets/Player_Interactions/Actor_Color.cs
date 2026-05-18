using UnityEngine;

public class Actor_Color : MonoBehaviour
{
    [Header("색깔을 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_SetToColor1(): \n게임오브젝트를 Color1 색깔로 설정함")]
    [Header("Act_SetToColor2(): \n게임오브젝트를 Color2 색깔로 설정함")]
    [Header("Act_SetToColor3(): \n게임오브젝트를 Color3 색깔로 설정함")]
    [Header("Act_SetToEmission1(): \n게임오브젝트를 Color1 기반의 발광 색깔로 설정함")]
    [Header("Act_SetToEmission1(): \n게임오브젝트를 Color2 기반의 발광 색깔로 설정함")]
    [Header("Act_SetToEmission1(): \n게임오브젝트를 Color3 기반의 발광 색깔로 설정함")]
    [Header("Act_DisableEmission(): \n게임오브젝트의 발광색 끄기 (검은색으로 설정)")]
    [Header("Act_SetToOriginalColor(): \n게임오브젝트를 원해색깔로 설정함")]
    [Header("--------")]

    [Tooltip("색깔이 바뀔 게임오브젝트 또는 해당 게임오브젝트의 Renderer 할당. 미할당시 현재 게임오브젝트에서 Renderer를 찾음")]
    public Renderer Renderer;
    private Material Material;
    private Color OriginalColor;
    private Color OriginalEmissionColor; // 원래 Emission 컬러 저장용

    [Tooltip("색깔1")]
    public Color Color1 = Color.red;
    [Tooltip("색깔2")]
    public Color Color2 = Color.green;  
    [Tooltip("색깔3")]
    public Color Color3 = Color.blue;

    [Header("Emission 설정")]
    [Tooltip("Emission 강도 (HDR 컬러처럼 밝게 만들 때 사용. 0~20)")]
    public float EmissionIntensity = 2.0f;

    void Awake()
    {
        if (Renderer == null) Renderer = GetComponent<Renderer>();
        
        if (Renderer != null)
        {
            Material = Renderer.material;
            OriginalColor = Material.color;
            // 기존의 Emission 컬러를 보관 (기본값이 검은색일 수 있음)
            OriginalEmissionColor = Material.GetColor("_EmissionColor");
        }
    }

    /// <summary>
    /// 엔터 색깔 설정
    /// </summary>
    public void Act_SetToColor1() => SetColor(Color1);

    public void Act_SetToColor2() => SetColor(Color2);

    public void Act_SetToColor3() => SetColor(Color3);

    // Color1을 기반으로 발광
    public void Act_SetToEmission1() => SetEmission(Color1);

    // Color2를 기반으로 발광
    public void Act_SetToEmission2() => SetEmission(Color2);

    // Color3을 기반으로 발광
    public void Act_SetToEmission3() => SetEmission(Color3);

    // Emission 끄기 (검은색으로 설정)
    public void Act_DisableEmission() => SetEmission(Color.black);

    /// <summary>
    /// 원래 색깔 지정
    /// </summary>
    public void Act_SetToOriginalColor()
    {
        SetColor(OriginalColor);
        SetEmission(OriginalEmissionColor);
    }

    void SetColor(Color c)
    {
        // Material?.color = c;
        if(Material != null)
        {
            Material.color = c;
        }
    }

    void SetEmission(Color c)
    {
        if (Material != null)
        {
            // 1. 머티리얼의 Emission 키워드 활성화 (필수)
            Material.EnableKeyword("_EMISSION");

            // 2. HDR 효과를 위해 강도(Intensity)를 곱해줌
            // 단순히 Color만 넣으면 빛나는 효과가 약할 수 있습니다.
            Color finalColor = c * Mathf.LinearToGammaSpace(EmissionIntensity);

            // 3. 속성 이름 "_EmissionColor"에 할당
            Material.SetColor("_EmissionColor", finalColor);
        }
    }
}
