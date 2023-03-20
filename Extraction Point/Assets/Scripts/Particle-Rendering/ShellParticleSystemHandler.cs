using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellParticleSystemHandler : MonoBehaviour
{

    public static ShellParticleSystemHandler Instance {  get; private set; }
    [SerializeField] private MeshParticleSystem meshParticleSystem;
    private List<Single> singleList;

    private void Awake() {
        
        Instance = this;
        singleList = new List<Single>();
    }

    private void LateUpdate() {
        for(int i = 0; i<singleList.Count; i++){
            Single single = singleList[i];
            single.Update();
            if(single.IsMovementComplete()){
                singleList.RemoveAt(i);
                i--;
            }
        }
    }

    public void SpawnShell(Vector3 position, Vector3 direction){
        singleList.Add(new Single(position, direction, meshParticleSystem));
    }

    private class Single { //represents single shell

        private MeshParticleSystem meshParticleSystem;
        private Vector3 position, direction, quadSize;
        private int quadIndex;
        private float rotation;
        private float moveSpeed;
        
        public Single(Vector3 position, Vector3 direction, MeshParticleSystem meshParticleSystem){
            this.position = position;
            this.direction = direction;
            this.meshParticleSystem = meshParticleSystem;
            
            quadSize = new Vector3(.1f, .1f);
            rotation = Random.Range(0, 360f);
            moveSpeed = Random.Range(1f, 3f);

            quadIndex = meshParticleSystem.AddQuad(position, rotation, quadSize, 0);
        }

        public void Update() {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(position, direction, .1f);
            if(raycastHit2D.collider != null && raycastHit2D.collider.CompareTag("Obstacle")){
                
                moveSpeed = 0;
            }
            position += direction * moveSpeed * Time.deltaTime;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;
            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, quadSize, 0);
            float slowDownFactor = 2.5f;
            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
        }

        public bool IsMovementComplete(){
            return moveSpeed < .1f;

        }
    }
}
