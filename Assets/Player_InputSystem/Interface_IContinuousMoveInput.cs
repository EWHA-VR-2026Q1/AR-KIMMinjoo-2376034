using UnityEngine;

// 1. 기본적인 물리 이동 관련
public interface IContinuousMoveInput
{
    Vector2 MoveInput { get; }
    bool SprintInput { get; }
    bool JumpInput { get; }

    float SnapTurnInput { get; }
}