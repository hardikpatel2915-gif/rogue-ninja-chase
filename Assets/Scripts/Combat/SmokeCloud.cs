using UnityEngine;

namespace RogueNinjaChase.Combat
{
    /// <summary>
    /// A pooled puff. Particles rise from the ground point and the object returns when they die.
    /// </summary>
    public sealed class SmokeCloud : MonoBehaviour
    {
        DistractionLoadout _owner;
        ParticleSystem _system;
        bool _playing;

        public void Bind(DistractionLoadout owner, Material material, float life)
        {
            _owner = owner;
            _system = GetComponent<ParticleSystem>();
            if (_system == null)
                _system = gameObject.AddComponent<ParticleSystem>();

            Configure(_system, material, life);
            _system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void Play(Vector3 groundPosition)
        {
            transform.position = groundPosition;
            _playing = true;
            _system.Play(true);
        }

        void Update()
        {
            if (!_playing || _system.IsAlive(true))
                return;

            _playing = false;
            _owner.ReleaseSmoke(this);
        }

        static void Configure(ParticleSystem system, Material material, float life)
        {
            ParticleSystem.MainModule main = system.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = 0.4f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(life * 0.7f, life);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.35f, 0.9f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.7f);
            main.startColor = new Color(0.85f, 0.85f, 0.85f, 0.55f);
            main.gravityModifier = -0.12f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 40;

            ParticleSystem.EmissionModule emission = system.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 26) });

            ParticleSystem.ShapeModule shape = system.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Hemisphere;
            shape.radius = 0.35f;
            shape.rotation = new Vector3(-90f, 0f, 0f);

            ParticleSystem.ColorOverLifetimeModule color = system.colorOverLifetime;
            color.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(new Color(0.65f, 0.65f, 0.65f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0.5f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            color.color = gradient;

            ParticleSystem.SizeOverLifetimeModule size = system.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                new Keyframe(0f, 0.4f),
                new Keyframe(1f, 1.8f)));

            ParticleSystemRenderer renderer = system.GetComponent<ParticleSystemRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            if (material != null)
                renderer.sharedMaterial = material;
        }
    }
}
