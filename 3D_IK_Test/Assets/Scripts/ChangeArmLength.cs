using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeArmLength : MonoBehaviour
{
    public GameObject Avator;
    float length = 100.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("a"))
        {
            length++;
            Avator.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, length);
        }
        if (Input.GetKey("s"))
        {
            length--;
            Avator.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, length);
        }
    }
}
