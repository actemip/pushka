using UnityEngine;

namespace CannonTargets
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class QuadraticDrag : MonoBehaviour
    {
        public float Mass { get; private set; }
        public float Radius { get; private set; }
        public static Vector3 Wind = new Vector3(0.25f, 0f, 0f);
        private Rigidbody body;
        private bool counted;

        public void SetPhysicalParams(float mass, float radius)
        {
            Mass = mass;
            Radius = radius;
            body = GetComponent<Rigidbody>();
            body.mass = mass;
            body.useGravity = false;
            body.linearDamping = 0f;
            body.angularDamping = 0.05f;
            var sphere = GetComponent<SphereCollider>();
            sphere.radius = radius;
            transform.localScale = Vector3.one;
        }

        public void Launch(Vector3 velocity)
        {
            if (!body) body = GetComponent<Rigidbody>();
            body.linearVelocity = velocity;
        }

        private void FixedUpdate()
        {
            if (body && Mass > 0f)
                body.AddForce(Ballistics.Acceleration(body.linearVelocity, Mass, Radius, Wind) * Mass, ForceMode.Force);
            if (transform.position.y < -5f) Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            TargetBehaviour target = collision.collider.GetComponentInParent<TargetBehaviour>();
            if (target && !counted && target.TryHit())
            {
                counted = true;
                GameStats.RegisterHit();
                Destroy(gameObject);
                Destroy(target.gameObject);
            }
        }
    }
}
