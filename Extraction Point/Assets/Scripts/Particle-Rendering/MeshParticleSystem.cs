using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshParticleSystem : MonoBehaviour
{
    private const int MAX_QUAD_AMMOUNT = 15000;

    [System.Serializable]
    public struct ParticleUVPixels{ //stuct to be set in editor to hold pixel values
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVCoords{ //struct for converting pixels to normalized uv coords
        public Vector2 uv00;
        public Vector2 uv11;
        
    }
    private Mesh mesh;

 

    [SerializeField] private ParticleUVPixels[] particleUVPixelsArray;
    private UVCoords[] uVCoordsArray;

    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private int quadIndex;

    private void Awake() {
        mesh = new Mesh();

        vertices = new Vector3[4 * MAX_QUAD_AMMOUNT];
        uv = new Vector2[4 * MAX_QUAD_AMMOUNT];
        triangles = new int[6 * MAX_QUAD_AMMOUNT];

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;

        //setup internal uv normalized array
        Material material = GetComponent<MeshRenderer>().material;
        Texture mainTexture = material.mainTexture;
        int textureHeight = mainTexture.height;
        int textureWidth = mainTexture.width;

        List<UVCoords> uvCoordsList = new List<UVCoords>();
        foreach(ParticleUVPixels particleUVPixels in particleUVPixelsArray){
            UVCoords uVCoords = new UVCoords { 
                uv00 = new Vector2((float)particleUVPixels.uv00Pixels.x / textureWidth, (float)particleUVPixels.uv00Pixels.y / textureHeight),
                uv11 = new Vector2((float)particleUVPixels.uv11Pixels.x / textureWidth, (float)particleUVPixels.uv11Pixels.y / textureHeight),
            };
            uvCoordsList.Add(uVCoords);
        }
        uVCoordsArray = uvCoordsList.ToArray();
    }

    public int AddQuad(Vector3 position, float rotation, Vector3 quadSize, int uvIndex){
        if(quadIndex >= MAX_QUAD_AMMOUNT){return -1; } //mesh full exit gate

        UpdateQuad(quadIndex, position, rotation, quadSize, uvIndex);

        int spawnedQuadIndex = quadIndex;
        quadIndex++;

        return spawnedQuadIndex;
    }

    

    public void UpdateQuad(int quadIndex, Vector3 position, float rotation, Vector3 quadSize, int uvIndex){
        //relocating verticies
        int vIndex = quadIndex * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation - 180) * quadSize; //lower left
        vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation - 270) * quadSize; //up left
        vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation - 0) * quadSize; //low right
        vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation - 90) * quadSize; //up right

        

        //UV 
        UVCoords uvCoords = uVCoordsArray[uvIndex]; 
        uv[vIndex0] = uvCoords.uv00;
        uv[vIndex1] = new Vector2(uvCoords.uv00.x, uvCoords.uv11.y);
        uv[vIndex2] = uvCoords.uv11;
        uv[vIndex3] = new Vector2(uvCoords.uv11.x, uvCoords.uv00.y);
        
        //creating triangles to connect the verticies^
        int tIndex = quadIndex * 6;

        triangles[tIndex + 0] = vIndex0; 
        triangles[tIndex + 1] = vIndex1;
        triangles[tIndex + 2] = vIndex2;
        
        triangles[tIndex + 3] = vIndex0;
        triangles[tIndex + 4] = vIndex2;
        triangles[tIndex + 5] = vIndex3;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

    }
}
