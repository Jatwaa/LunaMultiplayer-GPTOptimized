using System;

namespace LmpCommon.Message.Base
{
    public static class QuantizationHelpers
    {
        public const float MaxPositionRange = 10000f; // 10 km - matches cell size
        public const ushort MaxUShort = 65535;

        public static ushort QuantizeFloat(float value, float min, float max)
        {
            float t = (value - min) / (max - min);
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;
            return (ushort)(t * ushort.MaxValue);
        }

        public static float DequantizeFloat(ushort quantized, float min, float max)
        {
            float t = quantized / (float)ushort.MaxValue;
            return min + t * (max - min);
        }

        public static ushort[] QuantizeVector3(double[] vector, float range)
        {
            float min = -range;
            float max = range;
            return new ushort[]
            {
                QuantizeFloat((float)vector[0], min, max),
                QuantizeFloat((float)vector[1], min, max),
                QuantizeFloat((float)vector[2], min, max)
            };
        }

        public static void DequantizeVector3(ushort[] quantized, float range, double[] output)
        {
            float min = -range;
            float max = range;
            output[0] = DequantizeFloat(quantized[0], min, max);
            output[1] = DequantizeFloat(quantized[1], min, max);
            output[2] = DequantizeFloat(quantized[2], min, max);
        }

        public static ushort QuantizeVelocity(float value)
        {
            // KSP max orbital velocity is roughly 10 km/s = 10000 m/s
            return QuantizeFloat(value, -10000f, 10000f);
        }

        public static float DequantizeVelocity(ushort quantized)
        {
            return DequantizeFloat(quantized, -10000f, 10000f);
        }
    }
}