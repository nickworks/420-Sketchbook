using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    
    
    public BoidType type { get; private set; }
    public Rigidbody body;
    public Vector3 dir { get; private set; }
    public void Init(BoidType t){
        type = t;
    }
    void Start()
    {
        body = GetComponent<Rigidbody>();
        BoidManager.Add(this);
    }
    void OnDestroy(){
        BoidManager.Remove(this);
    }
    
    void Update()
    {
        
    }
    void LateUpdate(){
        float speed = body.velocity.magnitude;
        dir = body.velocity / speed;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 90 * speed);

    }
    public void CalcForcesFrom(KeyValuePair<BoidType, List<Boid>> boids){

        BoidClass settings = BoidManager.GetSettings(type);
        BoidRelationshipTo response = settings.GetResponse(boids.Key);
        
        Vector3 avgCenter = new Vector3();
        Vector3 avgAlign = new Vector3();

        int countSeparation = 0;
        int countCohesion = 0;
        int countAlignment = 0;

        for(int i = 0; i < boids.Value.Count; i++){
            Boid boid = boids.Value[i];

            if(boid == this) continue; // skip self-check

            Vector3 dif = boid.transform.position - transform.position;

            float dis = dif.magnitude;

            if(dis < response.radiusAlignment){
                countAlignment++;
                avgAlign += boid.dir;
            }
            if(dis < response.radiusCohesion){
                countCohesion++;
                avgCenter += boid.transform.position;
            }
            if(dis < response.radiusSeparation){
                countSeparation++;
                body.AddForce(-Time.deltaTime * response.forceSeparation * dif/dis/dis);
            }
        }

        if(countCohesion > 0){
            avgCenter /= countCohesion;
            Vector3 vToCenter = avgCenter - transform.position;
            Vector3 desiredVelocity = vToCenter.normalized * settings.speed;
            Vector3 force =  desiredVelocity - body.velocity;

            if(force.sqrMagnitude > settings.maxForce*settings.maxForce)
                force = force.normalized * settings.maxForce;

            body.AddForce(Time.deltaTime * response.forceCohesion * force);
        }
        if(countAlignment > 0){
            avgAlign /= countAlignment;
            Vector3 desiredVelocity = avgAlign * settings.speed;
            Vector3 force =  desiredVelocity - body.velocity;

            if(force.sqrMagnitude > settings.maxForce*settings.maxForce)
                force = force.normalized * settings.maxForce;

            body.AddForce(Time.deltaTime * response.forceAlignment * force);
        }

    }
}
