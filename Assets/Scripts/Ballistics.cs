using UnityEngine;

namespace CannonTargets
{
    public static class Ballistics
    {
        public const float Gravity = 9.81f;
        public const float AirDensity = 1.225f;
        public const float DragCoefficient = 0.47f;

        public static Vector3 Acceleration(Vector3 velocity, float mass, float radius, Vector3 wind)
        {
            Vector3 relative = velocity - wind;
            float area = Mathf.PI * radius * radius;
            Vector3 drag = relative.sqrMagnitude > 0.0001f
                ? -0.5f * AirDensity * DragCoefficient * area * relative.magnitude * relative
                : Vector3.zero;
            return Vector3.down * Gravity + drag / Mathf.Max(0.01f, mass);
        }

        public static Vector3[] Predict(Vector3 position, Vector3 velocity, float mass, float radius,
            Vector3 wind, float step, int maxSteps, float groundY)
        {
            var points = new System.Collections.Generic.List<Vector3>(maxSteps + 1) { position };
            for (int i = 0; i < maxSteps; i++)
            {
                Vector3 a = Acceleration(velocity, mass, radius, wind);
                Vector3 nextVelocity = velocity + a * step;
                Vector3 nextPosition = position + (velocity + nextVelocity) * (0.5f * step);
                position = nextPosition;
                velocity = nextVelocity;
                points.Add(position);
                if (position.y <= groundY) break;
            }
            return points.ToArray();
        }
    }
}
