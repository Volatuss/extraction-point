using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseFollow : MonoBehaviour
{
    
    [SerializeField] Transform player;
    [SerializeField] public float threshold;
    [SerializeField] private GameObject crossHair;
    private Vector3 targetPositon;
    private float temThresh;

    void Awake()
    {
        Cursor.visible = false;
    }

    
    

    void Update() 
    {
        if(PlayerHealth.isPlayerDead){return; }

        if(GameHandler.isUIOpen){ //if inv is not open, shouldnt be able to move the camera
            this.transform.position = player.transform.position;
        }else{
            if(Input.GetMouseButtonDown(1)){
                threshold = threshold * 2f;
            }

            if(Input.GetMouseButtonUp(1)){
                threshold = threshold / 2f;
            }  

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crossHair.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        Vector3 testPos = mousePosition - player.position;
        
        targetPositon = (player.position + mousePosition) / 2f;
        targetPositon.z = -10;
        targetPositon.x = Mathf.Clamp(targetPositon.x, -threshold + player.position.x, threshold + player.position.x);
        targetPositon.y = Mathf.Clamp(targetPositon.y, -threshold + player.position.y, threshold + player.position.y);
        
        this.transform.position = targetPositon;
    }
        
        
    }
}
