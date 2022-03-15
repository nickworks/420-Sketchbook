using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoidSettings {

    public Boid2Type type;

    public Boid2 prefab;
    public float maxSpeed;
    public float maxForce;

    public float radiusAlignment;
    public float radiusCohesion;
    public float radiusSeparation;

    public float forceAlignment;
    public float forceCohesion;
    public float forceSeparation;

}

public class BoidManager2 : MonoBehaviour
{
    
    public BoidSettings[] settings;

    public static BoidManager2 singleton;

    private List<Boid2> boids = new List<Boid2>();

    private void Start(){
        if(singleton != null){
            Destroy(gameObject);
        } else {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static BoidSettings GetSettings(Boid2Type type){
        foreach(BoidSettings bs in singleton.settings){
            if(bs.type == type) return bs;
        }
        return new BoidSettings();
    }

    public static void AddBoid(Boid2 b){
        singleton.boids.Add(b);
    }
    public static void RemoveBoid(Boid2 b){
        singleton.boids.Remove(b);
    }
    void Update(){
        
        // if not enough boids, spawn boids:
        
        if(boids.Count < 2){
            if(settings.Length > 0){
                Boid2 b = Instantiate(settings[0].prefab);
                b.type = settings[0].type;
            }
        }

        // update the boids:

        Boid2[] bArray = boids.ToArray();

        foreach(Boid2 b in boids){
            b.CalcForces(bArray);
        }

    }

}
