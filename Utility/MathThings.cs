using System.Numerics;

namespace BlockGame.Utility;

static class MathThings {
    public static void ToAxisAngle(this Quaternion q, out Vector3 axis, out float angle) {
        q = Quaternion.Normalize(q);

        float w = Math.Clamp(q.W, -1.0f, 1.0f);
        angle = 2.0f * MathF.Acos(w);

        float den = MathF.Sqrt(1.0f - w * w);

        if (den > 0.0001f) {
            axis = new Vector3(q.X, q.Y, q.Z) / den;
        } else {
            axis = Vector3.UnitX;
        }
    }
}