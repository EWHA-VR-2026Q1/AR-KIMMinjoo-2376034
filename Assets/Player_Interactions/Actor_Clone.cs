using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Clone : MonoBehaviour
{
    public GameObject ObjectToClone;
    public Transform ClonesParent;
    public float Force = 10f;

    public void Act_Clone()
    {
        Vector3 pos = ClonesParent.position + ClonesParent.forward * 0.1f;
        //Vector3 pos = ClonesParent.position;
        Quaternion rot = ClonesParent.rotation;
        GameObject clone = Instantiate(ObjectToClone, pos, rot);
        clone.transform.parent = ClonesParent;
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * Force);
        Destroy(clone, 2f);
    }

    public void Act_Clone(GameObject Target)
    {
        ObjectToClone = Target;
        Act_Clone();
    }
}
