using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    [SerializeField] int width, height;
    private int xTolerance, yTolerance;

    private void Awake() {
        xTolerance = (width/2) + (width/3);
        yTolerance = (height/2) + (height/3);
    } 

    private void Update() {
        if(
            Input.mousePosition.x - transform.position.x < -xTolerance ||
            Input.mousePosition.x - transform.position.x > xTolerance ||
            Input.mousePosition.y - transform.position.y < -yTolerance ||
            Input.mousePosition.y - transform.position.y > yTolerance)
        {
            gameObject.SetActive(false);
            
        }
        
    }

    
}
