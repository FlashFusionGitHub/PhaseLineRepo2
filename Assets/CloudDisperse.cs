using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CloudDisperse : MonoBehaviour {

    public Transform target;
    public float force = 10.0f;
    public float effectStrength;
    ParticleSystem ps;

	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        float deltaTime = Time.deltaTime;
        if (ps && /*m_moveAroundThese.Length > 0*/ target)
        {
            ParticleSystem.Particle[] particles =
                new ParticleSystem.Particle[ps.particleCount];

            ps.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.Particle p = particles[i];

                Vector3 particleWorldPosition;
                if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                {
                    particleWorldPosition = transform.TransformPoint(p.position);
                }
                else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
                {
                    particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
                }
                else
                {
                    particleWorldPosition = p.position;
                }
                Vector3 directionToTarget = (target.position - particleWorldPosition).normalized;

                Vector3 seekForce = (directionToTarget * force) * deltaTime;
                if (Vector3.Distance(particleWorldPosition, target.position) < effectStrength)
                {
                    p.velocity += seekForce;

                }

                particles[i] = p;
            }
            ps.SetParticles(particles, particles.Length);
        }
        
	}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(target.position, effectStrength);
    }
}
