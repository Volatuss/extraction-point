using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodRemoval : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 3f);
    }
}
