using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public static class Helpers{

    static int maxIter = 100;
    public static bool isPointInside(Vector3 point, List<Vector3> boundary){
        boundary = reorder(boundary);
        int boundSize = boundary.Count;
        for(int i =0;i<boundSize;i++){
            Vector3 a = boundary[(i+1)%boundSize]-boundary[i];
            Vector3 b = point-boundary[i];
            if(Vector3.Dot(Vector3.Cross(a,b),new Vector3(0,1,0))<0){
                return false;
            }
        }
        return true;
    }

    public static bool isPointInside(Vector2 point2d, List<Vector3> boundary){
        boundary = reorder(boundary);
        Vector3 point = new Vector3(point2d.x,0,point2d.y);
        int boundSize = boundary.Count;
        for(int i =0;i<boundSize;i++){
            Vector3 a = boundary[(i+1)%boundSize]-boundary[i];
            Vector3 b = point-boundary[i];
            if(Vector3.Dot(Vector3.Cross(a,b),new Vector3(0,1,0))<0){
                return false;
            }
        }
        return true;
    }

    public static float getArea(List<Vector3> boundary){
        boundary = reorder(boundary);
        float area = 0f;
        int boundSize = boundary.Count;
        Vector3 center = getCenter(boundary);
        for(int i =0;i<boundSize;i++){
            Vector3 a = boundary[i]-center;
            Vector3 b = boundary[(i+1)%boundSize]-center;
            area += Vector3.Cross(a,b).magnitude/2f;
        }
        return area;
    }

    public static Mesh triangulateCorner(List<Vector3> verts){

        int trisnum = 3*(verts.Count-2);
        int[] tris = new int[trisnum];
        
        for(int i =0;i<verts.Count-2;i++){
            int orig=0;
            int old = i+1;
            int newp = i+2;

            tris[3*i] = orig;
            tris[3*i+1] = old;
            tris[3*i+2] = newp;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Mesh triangulate(List<Vector3> verts){
        verts = reorder(verts);
        int trisnum = 3*verts.Count;
        int[] tris = new int[trisnum];
        verts.Add(getCenter(verts));

        for(int i =0;i<verts.Count-1;i++){
            int orig=verts.Count-1;
            int old = i;
            int newp = (i+1)%(verts.Count-1);

            tris[3*i] = orig;
            tris[3*i+1] = old;
            tris[3*i+2] = newp;
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;

    }

    public static List<Vector3> reorder(List<Vector3> points){
        List<Vector3> output = new List<Vector3>(points.ToArray());
        Vector3 center = getCenter(points);
        bool redo = true;
        int counter = 0;
        while(redo){
            redo = false;
            counter +=1;
            for(int i =0;i<output.Count;i++){
                Vector3 a = output[i]-center;
                Vector3 b = output[(i+1)%output.Count]-center;
                if(Vector3.Dot(Vector3.Cross(a,b),new Vector3(0,1,0))<0){
                    
                    redo = true;
                    output[i] = b+center;
                    output[(i+1)%output.Count] = a+center;
                }
            }
            if (counter>maxIter){
                Debug.Log("reorder failed.");
                return output;
            }
        }

        return output;
    }

    public static List<Vector2> reorder(List<Vector2> points){
        List<Vector3> intermediate = new List<Vector3>();
        foreach(Vector2 i in points){
            intermediate.Add(new Vector3(i.x,0,i.y));
        }
        intermediate = reorder(intermediate);
        List<Vector2> output = new List<Vector2>();
        foreach(Vector3 i in intermediate){
            output.Add(new Vector2(i.x,i.z));
        }

        return output;
    }

    public static Vector3 getCenter(List<Vector3> points){
        Vector3 center = new Vector3(0,0,0);
        foreach(Vector3 i in points){
            center = center+i;
        }
        return center/points.Count;

    }

    public static Vector3 getCenter(List<Vector2> points){
        Vector3 center = new Vector3(0,0,0);
        foreach(Vector3 i in points){
            center = center+ new Vector3(i.x,0,i.y);
        }
        return center/points.Count;

    }

    public static Vector2 getRandomPointInBoundary(List<Vector3> boundary){
        float maxX = Mathf.Max(boundary.Select(x => x.x).ToArray());
        float minX = Mathf.Min(boundary.Select(x => x.x).ToArray());
        float maxZ = Mathf.Max(boundary.Select(x => x.z).ToArray());
        float minZ = Mathf.Min(boundary.Select(x => x.z).ToArray());

        int count = 0;
        Vector2 output = new Vector3(Random.Range(minX,maxX),Random.Range(minZ,maxZ));

        while(!isPointInside(output,boundary)){
            output = new Vector3(Random.Range(minX,maxX),Random.Range(minZ,maxZ));
            count +=1;
            if(count>maxIter){
                Debug.Log("randPointFail");
                return getCenter(boundary);
            }
        }
        
        return output;   
    }
}