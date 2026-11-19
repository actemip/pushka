using UnityEngine;

namespace CannonTargets
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class TargetBehaviour : MonoBehaviour
    {
        public float Mass { get; private set; }
        public float Radius { get; private set; }
        public System.Action<TargetBehaviour> HitCallback;
        private Renderer targetRenderer;
        private Color originalColor;
        private bool hit;
        private bool trajectoryHighlighted;
        private float directionX;
        private float speedX;
        private Rigidbody body;

        public void Initialize(float mass, float radius, Vector3 velocity)
        {
            Mass = mass;
            Radius = radius;
            body = GetComponent<Rigidbody>();
            body.mass = mass;
            body.useGravity = false;
            body.linearVelocity = velocity;
            body.linearDamping = 0f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            GetComponent<SphereCollider>().radius = radius;
            directionX = Mathf.Sign(velocity.x);
            speedX = Mathf.Abs(velocity.x);
            targetRenderer = GetComponent<Renderer>();
            originalColor = targetRenderer.material.color;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.GetComponent<TargetWall>()) return;
            directionX = -directionX;
            body.linearVelocity = Vector3.right * directionX * speedX;
        }

        public bool TryHit()
        {
            if (hit) return false;
            hit = true;
            HitCallback?.Invoke(this);
            return true;
        }

        public void SetTrajectoryHighlighted(bool highlighted)
        {
            if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
            if (!targetRenderer || trajectoryHighlighted == highlighted) return;
            trajectoryHighlighted = highlighted;
            targetRenderer.material.color = highlighted ? Color.white : originalColor;
        }
    }
}
