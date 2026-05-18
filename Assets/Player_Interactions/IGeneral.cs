using UnityEngine;

public interface IGeneral
{
    void OnEnter(GameObject other);
    void OnStay(GameObject other);
    void OnExit(GameObject other);
    void OnClick(GameObject other);
}