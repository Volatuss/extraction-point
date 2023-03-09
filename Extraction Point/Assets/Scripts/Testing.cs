using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private PlayerAimWeapon playerAimWeapon;
    

    private void Start() {
        
        
    }

    public void OnShoot_Mesh(Vector3 firePoint){
        float rotation = -90f;
        Vector3 quadPosition = firePoint;
        Vector3 quadSize =  new Vector3(.1f,.1f);
        ShellParticleSystemHandler.Instance.SpawnShell(quadPosition, new Vector3(1, 1));
        //int spawnedQuadIndex = AddQuad(firePoint, rotation, quadSize, 0);

    }

    


}
