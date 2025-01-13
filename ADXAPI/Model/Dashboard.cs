namespace ADXAPI.Model
{
    public class Dashboard
    {
        public List<StateData> Data { get; set; }
        public List<ChartData> ChartData { get; set; }
        public Maximum Maximum { get; set; }
        public Minimum Minimum { get; set; }
        public double Average { get; set; }
    }

    public class ChartData
    {
        public string Name { get; set; }
        public List<int> Data { get; set; }
    }

    public class StateData
    {
        public string State { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public int DailyDamage { get; set; }
    }

    public class Maximum
    {
        public string State { get; set; }
        public int DamageCost { get; set; }
    }

    public class Minimum
    {
        public string State { get; set; }
        public int DamageCost { get; set; }
    }
}
