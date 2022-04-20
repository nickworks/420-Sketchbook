using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class SimpleViz2 : MonoBehaviour
{
    static public SimpleViz2 viz { get; private set; }

    public float ringRadius = 500;
    public float ringHeight = 4;

    public float orbHeight = 10;
    public int numBands = 512;
    public BetterBoid prefabOrb;

    private AudioSource player;
    private LineRenderer line;

    private List<BetterBoid> orbs = new List<BetterBoid>();

    public PostProcessing ppShader;

    public float avgAmp = 0;

    void Start()
    {
        if(viz != null){
            Destroy(gameObject);
            return;
        }
        viz = this;

        player = GetComponent<AudioSource>();
        line = GetComponent<LineRenderer>();

        // spawn 1 orb for each frequency band:
        Quaternion q = Quaternion.identity;
        for(int i = 0; i < numBands; i++){
            Vector3 p = new Vector3(0, (-orbHeight) + i * 2 * orbHeight / numBands, 0);
            orbs.Add( Instantiate(prefabOrb, p, q, transform) );
        }
    }
    void OnDestroy(){
        if(viz == this) viz = null;
    }
    
    void Update()
    {
        UpdateWaveform();
        UpdateFreqBands();
    }
    private void UpdateFreqBands(){

        float[] bands = new float[numBands];
        player.GetSpectrumData(bands, 0, FFTWindow.BlackmanHarris);

        avgAmp = 0;
        
        for (int i = 0; i < orbs.Count; i++)
        {
            float bias = (i + 1) / (float)numBands;
            orbs[i].UpdateAudioData(bands[i] * bias * 10000);

            avgAmp += bands[i]; // add to average
        }
        avgAmp /= numBands;
        avgAmp *= 10000;
        if(ppShader) ppShader.UpdateAmp(avgAmp);

    }
    private void UpdateWaveform()
    {
        int samples = 1024;
        float[] data = new float[samples];
        player.GetOutputData(data, 0);

        Vector3[] points = new Vector3[samples];


        for (int i = 0; i < data.Length; i++)
        {
            float sample = data[i];

            float rads = Mathf.PI * 2 * i / samples;

            float x = Mathf.Cos(rads) * ringRadius;
            float z = Mathf.Sin(rads) * ringRadius;

            float y = sample * ringHeight;

            points[i] = new Vector3(x, y, z);
        }


        line.positionCount = points.Length;
        line.SetPositions(points);
    }
}