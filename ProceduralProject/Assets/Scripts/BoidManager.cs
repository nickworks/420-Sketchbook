using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoidType {
    Bitty,
    Big
}

[System.Serializable]
public class BoidClass {
    public BoidType type;
    public Boid prefab;
    
    [Range(0,10)]
    public int limitMin = 2;

    [Range(2,50)]
    public int limitMax = 10;

    [Range(0, 10)]
    public float speed = 1;

    [Range(0,100)]
    public float maxForce = 1;

    public BoidRelationshipTo[] responses;

    public BoidRelationshipTo GetResponse(BoidType b){
        foreach(BoidRelationshipTo response in responses){
            if(response.type == b) return response;
        }
        return new BoidRelationshipTo();
    }

}
[System.Serializable]
public class BoidRelationshipTo {
    public BoidType type;
    [Range(0,10)]
    public float radiusCohesion = 2;
    [Range(0,10)]
    public float radiusSeparation = 1;
    [Range(0,10)]
    public float radiusAlignment = 2;
    [Range(0,1000)]
    public float forceCohesion = 2;
    [Range(0,1000)]
    public float forceSeparation = 1;
    [Range(0,1000)]
    public float forceAlignment = 2;
}

public class BoidManager : MonoBehaviour
{
    
    public BoidClass[] boidTypes;
    int min = 4;
    
    static public BoidManager singleton { get; private set; }

    static Dictionary<BoidType,List<Boid>> boids = new Dictionary<BoidType,List<Boid>>();

    static public void Add(Boid boid){
        BoidType t = boid.type;
        if(!boids.ContainsKey(t)) boids[t] = new List<Boid>();
        boids[t].Add(boid);
    }
    static public void Remove(Boid boid){
        if(boids.ContainsKey(boid.type))
        boids[boid.type].Remove(boid);
    }
    
    static public int HowMany(BoidType t){
        if(!boids.ContainsKey(t)) return 0;
        return boids[t].Count;
    }
    static private void CalcForces(){
        // for each group
        foreach(KeyValuePair<BoidType,List<Boid>> group1 in boids){
            foreach(Boid b1 in group1.Value){
                foreach(KeyValuePair<BoidType,List<Boid>> group2 in boids){
                    b1.CalcForcesFrom(group2);
                }
            }
        }
    }
    static public BoidClass GetSettings(BoidType a){
        foreach(BoidClass type in singleton.boidTypes){
            if(type.type == a){
                return type;
            }
        }
        return new BoidClass();    
    }
    
    void Start()
    {
        if(singleton){
            Destroy(gameObject);
        } else {
            singleton = this;
            boids.Clear();
        }
    }

    
    void Update()
    {
        foreach(BoidClass boidType in boidTypes){
            if(HowMany(boidType.type) < boidType.limitMin) {
                if(boidType.prefab) {
                    Boid b = Instantiate(boidType.prefab);
                    b.Init(boidType.type);
                }
            }
        }
        CalcForces();
    }
    void OnDestroy(){
        if(singleton == this) {
            singleton = null;
            boids.Clear();
        }
    }

    
}
