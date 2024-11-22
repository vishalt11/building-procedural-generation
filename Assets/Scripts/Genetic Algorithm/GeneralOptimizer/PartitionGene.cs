using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Input is points of footprint. Output is points of rooms, inner walls, outer walls.
public class RoomPartitioning:Gene<RoomPartitioning>{

    public List<Vector2> genes;
    public Foundation footprint;
    List<Line> outerWalls;
    List<Line> innerWalls;

    public RoomPartitioning(Foundation input,List<Vector2> genes){
        this.footprint = input;
        this.genes = genes;
        outerWalls = new List<Line>();
        innerWalls = new List<Line>();
    }

    public RoomPartitioning(Foundation input){
        int num = Random.Range(2,4);
        List<Vector2> genes = new List<Vector2>();
        for(int i=0;i<num;i++){
            genes.Add(Helpers.getRandomPointInBoundary(input.getBoundary()));
        }
        this.genes = genes.Distinct().ToList();
        this.footprint = input;
        outerWalls = new List<Line>();
        innerWalls = new List<Line>();
    }
    public override void setGenes(List<Vector2> newGenes){
        this.genes = newGenes;
    }

    public void setFootprint(Foundation input){
        this.footprint = input;
    }

    public override int getSize(){
        return genes.Count;
    }
    public override void mutateAdd(){
        genes.Add(Helpers.getRandomPointInBoundary(this.footprint.getBoundary()));
        this.genes = this.genes.Distinct().ToList();

    }

    public override void mutateRemove(){
        if(genes.Count>3){
            int purge = Random.Range(0,genes.Count);
            genes.RemoveAt(purge);
        }
    }

    public override void mutateChange(float scale){
        int change = Random.Range(0,genes.Count);
        Vector2 original = genes[change];
        Vector2 mutated = original + new Vector2(scale*Random.Range(-1.0f,1.0f),scale*Random.Range(-1.0f,1.0f));
        int counter = 0;
        List<Vector3> boundary = footprint.getBoundary();
        while(!Helpers.isPointInside(mutated,boundary)){
            mutated = original + new Vector2(scale*Random.Range(-1.0f,1.0f),scale*Random.Range(-1.0f,1.0f));
            counter +=1;
            if(counter>100){
                Debug.Log("random fail");
                Debug.Log(mutated);
                Debug.Log("boundary for fail");
                foreach(Vector3 i in boundary){
                    Debug.Log(i);
                }
                return;
            }
        }
        this.genes[change] = mutated;
    }

    public override RoomPartitioning createOffspring(RoomPartitioning other){
        List<Vector2> offspringGenes;
        if(this.getSize()>other.getSize()){
            offspringGenes = new List<Vector2>(this.genes.ToArray());
            for(int i=0;i<other.getSize();i++){
                if(Random.value>0.5f){
                    offspringGenes[i] = other.genes[i];
                }
            }
        }else{
            offspringGenes = new List<Vector2>(other.genes.ToArray());
            for(int i=0;i<this.getSize();i++){
                if(Random.value>0.5f){
                    offspringGenes[i] = this.genes[i];
                }
            }
        }
        offspringGenes = offspringGenes.Distinct().ToList();

        RoomPartitioning output = new RoomPartitioning(this.footprint,offspringGenes.GetRange(0,Mathf.Min(10,offspringGenes.Count)));
        return output;

    }

    bool needInnerWall(Vector3 c1, Vector3 c2){
        bool output = true;
        Line testLine = new Line(c1,c2);
        Vector3 _ = new Vector3();
        foreach(Line wall in innerWalls){
            if (testLine.getIntersection(out _,wall)){
                return false;
            }
        }

        return output;
    }

    bool visible(Vector3 c1, Vector3 c2){
        bool output = true;
        float threshold = 0.01f;
        Line testLine = new Line(c1,c2);
        Vector3 _ = new Vector3();
        foreach(Line wall in innerWalls){
            if (testLine.getIntersection(out _,wall)){ 
               if(!((c1-_).magnitude < threshold) && !((c2-_).magnitude < threshold)){
                    //Debug.Log(string.Format("intersection of {0} and inner wall {1} at {2}",testLine,wall,_));
                    return false;
                } 
            }
        }
        foreach(Line wall in outerWalls){
            if (testLine.getIntersection(out _,wall)){
                if(!((c1-_).magnitude < threshold) && !((c2-_).magnitude < threshold)){
                    // Debug.Log(c1);
                    // Debug.Log(c2);
                    // Debug.Log((c1-_).magnitude < threshold);
                    // Debug.Log((c2-_).magnitude < threshold);
                    //Debug.Log(string.Format("intersection of {0} and outer wall {1} at {2}",testLine,wall,_));
                    return false;
                }
            }
        }

        return output;
    }

    Line buildWall(Vector3 c1, Vector3 c2){
        bool horizontal = Mathf.Abs(c1.z-c2.z)>=Mathf.Abs(c1.x-c2.x);
        Vector3 center = (c1+c2)/2;
        Line division;
        if (horizontal){
            division = new Line(new Vector3(-100,0,center.z),new Vector3(100,0,center.z));
        }else{
            division = new Line(new Vector3(center.x,0,-100),new Vector3(center.x,0,100));
        }

        List<Vector3> intersections = new List<Vector3>();

       


        
        foreach(Line wall in innerWalls){
            Vector3 inter = new Vector3();
            if (division.getIntersection(out inter,wall)){
                intersections.Add(inter);                               
            }
        }
        foreach(Line wall in outerWalls){
            Vector3 inter = new Vector3();
            if (division.getIntersection(out inter,wall)){
                intersections.Add(inter);                              
            }
        }

        intersections = intersections.Distinct().ToList();

        
        // Debug.Log("Intersections..");
        // foreach(Vector3 i in intersections){
        //     Debug.Log(i);
        // }

        List<Vector3> visibleIntersections = new List<Vector3>();

        foreach(Vector3 i in intersections){
            if(visible(i,c1)||visible(i,c2)){
                visibleIntersections.Add(i);
            }
        }
        // Debug.Log("Visible Intersections..");
        // foreach(Vector3 i in visibleIntersections){
        //     Debug.Log(i);
        // }

        return(new Line(visibleIntersections[0],visibleIntersections[1]));
    }

    public List<Vector3> getCenters(){
        List<Vector3> output = new List<Vector3>();
        foreach(Vector2 i in genes){
            output.Add(new Vector3(i.x,0,i.y));
        }
        return output;
    }  

    public List<List<Vector3>> getPartitions(){
        outerWalls = new List<Line>();
        innerWalls = new List<Line>();

        List<Vector3> boundary = new List<Vector3>(footprint.getBoundary()); 
        for(int i =0;i<boundary.Count;i++){
            outerWalls.Add(new Line(boundary[i],boundary[(i+1)%boundary.Count]));
        }       
        List<Vector3> centers = new List<Vector3>();
        List<List<Vector3>> output = new List<List<Vector3>>();


        for(int i =0;i<genes.Count;i++){
            Vector3 point = new Vector3(genes[i].x,0.0f,genes[i].y);
            centers.Add(point);    
        }
        // Debug.Log("centers are...");
        // foreach(Vector3 center in centers){
        //     Debug.Log(center);
        // }

        if(centers.Count <= 1){  
            output.Add(boundary);
            return output;
        }

        for(int i = 0; i<centers.Count-1;i++){
            for(int j = i+1;j<centers.Count;j++){
                if(needInnerWall(centers[i],centers[j])){
                    Line newInnerWall = buildWall(centers[i],centers[j]);
                    boundary.Add(newInnerWall.start);
                    boundary.Add(newInnerWall.end);
                    innerWalls.Add(newInnerWall);                                     
                }
            }
        }

        boundary = boundary.Distinct().ToList();

        // Debug.Log("Boundary Points...");
        // foreach(Vector3 boundaryPoint in boundary){
        //     Debug.Log(boundaryPoint);
        // }

        // Debug.Log("inner walls..");
        // foreach(Line i in innerWalls){
        //     Debug.Log(i);
        // }

        // Debug.Log("outer walls..");
        // foreach(Line i in outerWalls){
        //     Debug.Log(i);
        // }

        
        

        foreach(Vector3 roomCenter in centers){
            List<Vector3> points = new List<Vector3>();
            foreach(Vector3 boundaryPoint in boundary){
                if(visible(roomCenter,boundaryPoint)){
                    points.Add(boundaryPoint);
                }
            }
            output.Add(points);
        }

        // Debug.Log("Partitions...");
        // foreach(List<Vector3> room in output){
        //     Debug.Log("Room Points...");
        //     foreach(Vector3 roomPoint in room){
        //         Debug.Log(roomPoint);
        //     }
        // }

        return output;

    }
}