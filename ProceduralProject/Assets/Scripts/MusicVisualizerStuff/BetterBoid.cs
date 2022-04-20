using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BetterBoid : MonoBehaviour
{
    public class Response {
        public enum Type {
            isFriendly,
            isPredator,
            isPrey
        }
        public Type type;
        public float radiusAlignment;
        public float radiusSeparation;
        public float radiusCohesion;
        public float forceAlignment;
        public float forceSeparation;
        public float forceCohesion;
    }
    static List<BetterBoid> boids = new List<BetterBoid>();
    public BoidType type { get; private set; }
    private Rigidbody body;
    public Vector3 dir { get; private set; }

    public float speed;
    public float maxForce;

    public float radiusAlignment = 4;
    public float radiusCohesion = 20;
    public float radiusSeparation = 4;
    public float forceAlignment = 1;
    public float forceCohesion = 100;
    public float forceSeparation = 1;
    public float radiusSeparationPredator = 3;
    public float forceSeparationPredator = 1;
    public float radiusSeekPrey = 3;
    public float forceSeekPrey = 1;
    
    private SimpleViz2 viz;
    public void Init(BoidType t){
        type = t;
        transform.localScale = Vector3.one * (t == BoidType.Bitty ? .2f : 1.0f);
    }
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", Random.Range(0, 2 * Mathf.PI));
        viz = SimpleViz2.viz;
        body = GetComponent<Rigidbody>();
        body.AddForce(Random.onUnitSphere * 5, ForceMode.Impulse);
        boids.Add(this);
    }
    void OnDestroy(){
        boids.Remove(this);
    }
    void Update(){
        CalcForces();
    }
    public void UpdateAudioData(float value){
        //value *= 100;
        maxForce = (maxForce + value) / 2;

    }
    void LateUpdate(){

        Vector3 toCenter = viz.transform.position - transform.position;
        float dToCenter = toCenter.magnitude;

        Vector3 desiredSpeed = speed * toCenter/dToCenter;
        Vector3 forceTurnToCenter = desiredSpeed - body.velocity;
        if(forceTurnToCenter.sqrMagnitude > maxForce * maxForce) forceTurnToCenter = forceTurnToCenter.normalized * maxForce;
        body.AddForce(forceTurnToCenter);

        float mag = body.velocity.magnitude;
        dir = mag > 0 ? body.velocity / mag : transform.forward;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 30 * (mag/10));

        //transform.position = BoidManager.WrapToWorld(transform.position);
    }

    Response.Type ResponseTo(BetterBoid b){
        float ratio = transform.localScale.x / b.transform.localScale.x;
        if(ratio >= 2.0f) return Response.Type.isPrey;
        if(ratio <= 0.5f) return Response.Type.isPredator;
        return Response.Type.isFriendly;
    }
    public void CalcForces(){

        Vector3 avgCenter = new Vector3();
        Vector3 avgAlign = new Vector3();

        int countCohesion = 0;
        int countAlignment = 0;

        foreach(BetterBoid boid in boids){

            if(boid == this) continue; // skip self-check
            Response.Type response = ResponseTo(boid);
            Vector3 dif = boid.transform.position - transform.position;
            
            float dis = dif.magnitude;
            if(dis > 0){
                switch(response){
                    case Response.Type.isFriendly:
                        if(dis < radiusAlignment){
                            countAlignment++;
                            avgAlign += boid.dir;
                        }
                        if(dis < radiusCohesion){
                            countCohesion++;
                            avgCenter += boid.transform.position;
                        }
                        if(dis < radiusSeparation){
                            body.AddForce(-Time.deltaTime * forceSeparation * dif/dis/dis);
                        }
                        break;
                    case Response.Type.isPredator:
                        if(dis < radiusSeparationPredator){
                            body.AddForce(-Time.deltaTime * forceSeparationPredator * dif/dis/dis);
                        }
                    break;
                    case Response.Type.isPrey:
                        if(dis < radiusSeekPrey){
                            body.AddForce(Time.deltaTime * forceSeekPrey * dif/dis/dis);
                        }
                    break;
                } // switch
            } // if
        } // foreach
        
        if(countCohesion > 0){
            avgCenter /= countCohesion;
            Vector3 vToCenter = avgCenter - transform.position;
            Vector3 desiredVelocity = vToCenter.normalized * speed;
            Vector3 force =  desiredVelocity - body.velocity;

            if(force.sqrMagnitude > maxForce*maxForce)
                force = force.normalized * maxForce;

            body.AddForce(Time.deltaTime * forceCohesion * force);
        }
        if(countAlignment > 0){
            avgAlign /= countAlignment;
            Vector3 desiredVelocity = avgAlign * speed;
            Vector3 force =  desiredVelocity - body.velocity;

            if(force.sqrMagnitude > maxForce*maxForce)
                force = force.normalized * maxForce;

            body.AddForce(Time.deltaTime * forceAlignment * force);
        }
        
    }
}
