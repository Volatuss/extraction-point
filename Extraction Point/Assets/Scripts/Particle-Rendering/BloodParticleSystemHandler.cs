using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleSystemHandler : MonoBehaviour
{
    
    
    public static BloodParticleSystemHandler Instance {  get; private set; }
    [SerializeField] private MeshParticleSystem meshParticleSystem;
    private List<Splat> splatterList;

    private void Awake() {
        
        Instance = this;
        splatterList = new List<Splat>();
    }

    private void LateUpdate() {
        if(splatterList == null){ return; }
        for(int i = 0; i<splatterList.Count; i++){
            Splat singleSplat = splatterList[i];
            singleSplat.Update();
            if(singleSplat.IsMovementComplete()){
                splatterList.RemoveAt(i);
                i--;
            }
        }
    }

    public void SpawnGore(Vector3 position, Vector3 direction){
        for(int i = 0; i<8; i++){
            splatterList.Add(new Splat(position, PlayerWeaponHandler.ApplyRotationToVector(direction, Random.Range(-5f, 5f)), meshParticleSystem));
        }
    }

    private class Splat { //represents single gore sprite
        private MeshParticleSystem meshParticleSystem;
        private Vector3 position, direction, quadSize;
        private int quadIndex, uvIndex;
        private float rotation;   
        private float moveSpeed;
        
        
        
        public Splat(Vector3 position, Vector3 direction, MeshParticleSystem meshParticleSystem){
            this.position = position;
            this.direction = direction;
            this.meshParticleSystem = meshParticleSystem;
            
            quadSize = new Vector3(.2f, .2f);
            rotation = Random.Range(0, 360f);
            moveSpeed = Random.Range(24f, 48f);
            uvIndex = Random.Range(0,7);

            quadIndex = meshParticleSystem.AddQuad(position, rotation, quadSize, uvIndex);
        }

        public void Update() {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(position, direction, 1f);
            if(raycastHit2D.collider != null && raycastHit2D.collider.CompareTag("Obstacle")){
                moveSpeed = 0;
            }
            position += direction * moveSpeed * Time.deltaTime;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;
            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, quadSize, uvIndex);
            float slowDownFactor = 14f;
            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;           
        }

        public bool IsMovementComplete(){
            return moveSpeed < .5f;

        }
    }
}
