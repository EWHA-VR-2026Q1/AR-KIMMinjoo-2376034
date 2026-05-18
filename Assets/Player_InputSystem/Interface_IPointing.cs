using UnityEngine;

public interface IPointing
{
    // Ray 탐색 관련
    bool Pressed { get; }
    bool IsPressing { get; }
    // bool Poke { get; }
    bool Released { get; }
}