namespace PostAdcData
{
    public class VoltData
    {
        /// <summary>
        /// 生の値です。
        /// </summary>
        public int RawValue { get; private set; }

        /// <summary>
        /// 電圧です。
        /// </summary>
        public double Voltage { get; private set; }

        public VoltData(int rawValue, double voltage)
        {
            RawValue = rawValue;
            Voltage = voltage;
        }
    }
}
