using System;

namespace InvertMouse.Inverter
{
    public static class InvertMouseFactory
    {
        public static InvertMouseBase Create(DriverType driverType)
        {
            switch (driverType)
            {
                case DriverType.Interception:
                    return new InvertMouseInterception();

                case DriverType.RawAccel:
                    return new InvertMouseRawAccel();

                case DriverType.InvertMouse:
                    return new InvertMouseDriver();

                default:
                    throw new NotSupportedException($"Driver type is not supported {driverType}");
            }
        }
    }
}
