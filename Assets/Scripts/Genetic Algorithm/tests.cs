using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class tests : MonoBehaviour
{
    List<Vector3> lot;

    List<Vector2> roomPoints;
    // Start is called before the first frame update
    void Start()
    {
       lot = new List<Vector3>{new Vector3(0,0,0),new Vector3(0,0,10),new Vector3(10,0,10), new Vector3(10,0,0)};
    //    List<Vector2> floorGenes = new List<Vector2>{new Vector3(0,0),new Vector3(0,5),new Vector3(5,5), new Vector3(5,0)};
       Foundation footprint = new Foundation(lot);
    //    footprint.genes.Add(Helpers.getRandomPointInBoundary(footprint.lot));
    //    RoomPartitioning rooms = new RoomPartitioning(footprint);
    //    roomPoints = rooms.genes;
       FloorBlockyConstraint con = new FloorBlockyConstraint();
       Debug.Log(con.getScore(footprint));
       displayLot();
       displayFootprint(footprint);
    //    displayRooms(rooms);


    }
    // void OnDrawGizmosSelected(){
        
    //     for(int i =0;i<roomPoints.Count;i++){
    //         Vector2 pos = roomPoints[i];
    //         Gizmos.color = Color.HSVToRGB(i*1f/roomPoints.Count,1,1);
    //         Gizmos.DrawSphere(new Vector3(pos.x,2,pos.y),0.1f);
    //     }
        
    // }

    void displayLot(){
        GameObject lotDis = new GameObject();
        lotDis.name = "Lot";
        lotDis.transform.parent = gameObject.transform;
        MeshRenderer meshr = lotDis.AddComponent<MeshRenderer>();
        MeshFilter meshf = lotDis.AddComponent<MeshFilter>();
        meshf.mesh = Helpers.triangulate(this.lot);
		//meshr.material.mainTexture = (Texture2D)Resources.Load("grassy");
        meshr.material.SetColor("_Color",Color.white);

    }

    void displayFootprint(Foundation footprint){
        // Debug.Log("Footprint points:");
        // for(int i =0;i<footprint.genes.Count;i++){
        //     Debug.Log(footprint.genes[i]);
        // }
        GameObject floor = new GameObject();
        floor.name = "Footprint";
        floor.transform.parent = gameObject.transform;
        floor.transform.position = new Vector3(0,1,0);
        MeshRenderer meshr = floor.AddComponent<MeshRenderer>();
        MeshFilter meshf = floor.AddComponent<MeshFilter>();
        meshf.mesh = Helpers.triangulate(footprint.getBoundary());
        meshr.material.SetColor("_Color",Color.grey);

    }

    void displayRooms(RoomPartitioning partitioning){
        // Debug.Log("room points:");
        // for(int i =0;i<partitioning.genes.Count;i++){
        //     Debug.Log(partitioning.genes[i]);
        // }
        GameObject partitionsContainer = new GameObject();
        partitionsContainer.name = "Rooms";
        partitionsContainer.transform.parent = gameObject.transform;
        partitionsContainer.transform.position = new Vector3(0,2,0);
        List<List<Vector3>> rooms = partitioning.getPartitions();
        List<GameObject> roomObjects = new List<GameObject>();
        for(int i = 0;i<rooms.Count;i++){
			//displayWalls(Helpers.reorder(rooms[i]));
            GameObject room = new GameObject();
            room.name = string.Format("Room {0}",i+1);
            room.transform.parent = partitionsContainer.transform;
            room.transform.position = new Vector3(0,2,0);
            MeshRenderer meshr = room.AddComponent<MeshRenderer>();
            MeshFilter meshf = room.AddComponent<MeshFilter>();
            meshf.mesh = Helpers.triangulate(Helpers.reorder(rooms[i]));
            meshr.material.SetColor("_Color",Color.HSVToRGB(i*1f/rooms.Count,1,1));
        }
    }
	
	void displayWalls(List<Vector3> vertices){
		GameObject partitionsContainer = new GameObject();
        partitionsContainer.name = "Room Walls";
        partitionsContainer.transform.parent = gameObject.transform;
		Vector3 position;
		Vector3 position2;
		float rotato;
		for (int i=0; i<vertices.Count;i++){
			if (i==vertices.Count-1){
				position = vertices[i];
				position2 = vertices[0];
			}
			else{
				position = vertices[i];
				position2 = vertices[i+1];
			}
			Vector3 between = position2 - position;
			float distance = between.magnitude;
			GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
			wall.name = "Wall";
			wall.transform.parent = partitionsContainer.transform;

			//wall.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("brick");
			
			wall.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			wall.transform.localScale = new Vector3(distance, 2f, 0.1f);
			if (position.z>=position2.z){
				rotato = Mathf.Acos((position2.x-position.x)/distance)* 180/Mathf.PI;
			}
			else{
				rotato = Mathf.Acos((position.x-position2.x)/distance)* 180/Mathf.PI;
			}
			wall.transform.Rotate(0,rotato,0);
			wall.transform.position = position + (between/2);
			wall.transform.position += new Vector3(0f,3f,0);
		}
	}

   
}
