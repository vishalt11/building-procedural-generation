using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
public class Line{

    public Vector3 start;
    public Vector3 end;
    public Vector3 direction;

    public Line(Vector3 start,Vector3 end){
        this.start = start;
        this.end = end;
        this.direction = end-start;
    }

    public override string ToString(){
        return string.Format("Line from {0} to {1}",start,end);

    }

    public float getLength(){
        return this.direction.magnitude;
    }
	

    public bool getIntersection(out Vector3 intersection, Line other){
        Matrix<double> A = Matrix<double>.Build.DenseOfArray(new double [,]{
            {this.direction.x, -1*other.direction.x},
            {this.direction.z, -1*other.direction.z}
        });

        if(A.Determinant()==0){
            intersection = new Vector3(0,0,0);
            return false;
        }

        Vector<double> b = Vector<double>.Build.Dense(new double[] {other.start.x-this.start.x,other.start.z-this.start.z});
        Vector<double> x = A.Solve(b);

        // Debug.Log(x[0]);
        // Debug.Log(x[1]);

        if(x[0]>=0 && x[0]<=1 && x[1]>=0 && x[1]<=1){
            intersection = this.start + this.direction*(float)x[0];
            return true;
        }else{
            intersection = new Vector3(0,0,0);
            return false;
        }
    }

    
}