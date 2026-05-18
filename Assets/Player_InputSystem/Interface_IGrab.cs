using UnityEngine;

public interface IGrab
{
    // Hand/Controller 인터랙션 관련
    bool DistanceGrab { get; }
    bool DistancePull { get; }
    bool DistancePoke { get; }
    
    bool Poke { get; }
    bool Pinch { get; }
    bool Grab { get; }
    bool Release { get; }
}