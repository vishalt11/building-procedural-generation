using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Foundation:Gene<Foundation>{

    public List<Vector2> genes;
    public List<Vector3> lot;

    public Foundation(List<Vector2> genes,List<Vector3> lot){
        this.lot = lot;
        this.genes = genes;
    }

    public Foundation(List<Vector3> lot){
        this.lot = lot;
        List<Vector2> randomGenes = new List<Vector2>();
        for(int i =0;i<3;i++){
            randomGenes.Add(Helpers.getRandomPointInBoundary(this.lot));
        }
        this.genes = randomGenes;

    }

    public override void setGenes(List<Vector2> newGenes){
        this.genes = newGenes;
    }

    public override int getSize(){
        return genes.Count;
    }
    public override void mutateAdd(){
        genes.Add(Helpers.getRandomPointInBoundary(this.lot));
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
        while(!Helpers.isPointInside(mutated,lot)){
            mutated = original + new Vector2(scale*Random.Range(-1.0f,1.0f),scale*Random.Range(-1.0f,1.0f));
        }
        this.genes[change] = mutated;
    }

    public override Foundation createOffspring(Foundation otherParent){
        Foundation child = new Foundation(this.lot);
        List<Vector2> childGenes = new List<Vector2>(this.genes);
        for(int i = 0;i<childGenes.Count;i++){
            if(Random.value > 0.5f){
                int swap = Mathf.Min(i,otherParent.genes.Count-1);
                childGenes[i] = otherParent.genes[swap];
            }
        }

        child.genes = childGenes.Where(x => Helpers.isPointInside(x,child.lot)).ToList();

        // Debug.Log(string.Join(",", this.genes));
        // Debug.Log(string.Join(",", otherParent.genes));
        // Debug.Log(string.Join(",", child.genes));

        return child;

    }

    public List<Vector3> getBoundary(){
        List<Vector3> verts = new List<Vector3>();
        for(int i =0;i<genes.Count;i++){
            verts.Add(new Vector3(genes[i].x,0.0f,genes[i].y));
        }
        return verts;
    }


}


