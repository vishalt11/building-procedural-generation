using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tests2 : MonoBehaviour{

    public List<Vector3> lotPoints;

    void Start(){
        List<Vector3> testBound = new List<Vector3>{
            new Vector3(9,0,8.1f),
            new Vector3(3.9f,0,9.3f),
            new Vector3(1.8f,0,0.9f),
            new Vector3(6.5f,0,0.1f),
        };
        Vector2 testPoint = Helpers.getRandomPointInBoundary(testBound);

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        plane.GetComponent<MeshFilter>().mesh = Helpers.triangulate(testBound);
        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point.transform.position = new Vector3(testPoint.x,0,testPoint.y);

        Debug.Log(Helpers.isPointInside(testPoint,testBound));
        

    }
        
}