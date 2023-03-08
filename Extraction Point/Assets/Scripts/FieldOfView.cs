using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    private Vector3 origin;
    private float startingAngle;
    private float fov;
    [SerializeField] private LayerMask layerMask;




    private void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
        fov = 120f;
    }
    private void LateUpdate() {

        int rayCount = 240;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 15f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int triangleIndex = 0;
        int vertexIndex = 1;
        for(int i = 0; i <= rayCount; i++){
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if(raycastHit2D.collider == null){
                //noHit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }else{
                //hit
                
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;
            if(i>0){
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex-1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
           
            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

    }

    public void SetFoV(float fov){
        this.fov = fov;
    }

    public void SetOrigin(Vector3 origin){
        this.origin = origin;
    }
    
    public void SetAimDirection(Vector3 aimDirection){
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }


    public static Vector3 GetVectorFromAngle(float angle){ //get vector from angle
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir){ //calculate angle from vector
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(n<0) n += 360;

        return n;
    }
}
