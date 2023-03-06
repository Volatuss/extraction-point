using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatExample : MonoBehaviour
{
    [SerializeField] GameObject Blood;

    private void Update() {
        if(Input.GetKey(KeyCode.Space)){
            Instantiate(Blood, transform.position, Quaternion.identity);
        }
    }
}
