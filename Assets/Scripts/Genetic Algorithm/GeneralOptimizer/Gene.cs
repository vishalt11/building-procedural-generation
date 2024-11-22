using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class Gene<T>{
    List<Vector2> genes;

    abstract public void setGenes(List<Vector2> newGenes);

    abstract public int getSize();
    abstract public void mutateAdd();

    abstract public void mutateRemove();

    abstract public void mutateChange(float scale);

    abstract public T createOffspring(T other);

}