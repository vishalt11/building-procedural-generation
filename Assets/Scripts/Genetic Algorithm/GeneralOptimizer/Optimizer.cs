using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
class Optimizer<T> where T:Gene<T>{
    public float mutateAdd;
    public float mutateRemove;
    public float mutateChange;
    public float mutateScale;
    public int iterations;
    public List<Constraint<T>> constraints;
    public List<T> population;
    

    public Optimizer(List<T> initPop, List<Constraint<T>> constraints, int iter,  float muteAdd, float muteRemove, float muteChange, float muteScale){
        population = initPop;
        this.constraints = constraints;
        iterations = iter;
        mutateAdd = muteAdd;
        mutateRemove = muteRemove;
        mutateChange = muteChange;
        mutateScale = muteScale;
    }

    public T getOptimizedResult(){
        for(int i =0;i<iterations;i++){
            iterate();
            float score = applyConstraints(population.OrderBy(j => -1*applyConstraints(j)).ElementAt(0));
            Debug.Log(score);
            // Debug.Log("iteration: " + i);
        }
        return population.OrderBy(i => -1*applyConstraints(i)).ElementAt(0);
    }
    float applyConstraints(T individual){
        float score = 0f;

        for(int i=0;i<constraints.Count;i++){
            score+=constraints[i].getScore(individual);
        }

        return score;
    }
    void iterate(){
        List<T> sorted = population.OrderBy(i => -1*applyConstraints(i)).ToList<T>();
        List<T> newGen = new List<T>();
        int split = population.Count/2;
        for(int i =0;i<split;i++){
            int parent1 = Random.Range(0,split);
            int parent2 = Random.Range(0,population.Count);
            newGen.Add(sorted[i].createOffspring(sorted[parent1]));
            newGen.Add(sorted[i].createOffspring(sorted[parent2]));

        }

        foreach(T individual in newGen){
            if(Random.value<mutateAdd){
                individual.mutateAdd();
            }
            if(Random.value<mutateChange){
                individual.mutateChange(mutateScale);
            }
            if(Random.value<mutateRemove){
                individual.mutateRemove();
            }
        }
        population = newGen;
    }
    
}