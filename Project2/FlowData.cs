using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    public class FlowData
    {
        public class RiverData
        {
            public double Flow { get; set; }
            public double UltimateBOD { get; set; }
            public double DO { get; set; }
            public double Temperature { get; set; }
            public double Speed { get; set; }
            public double AverageDepth { get; set; }
            public double BedActivityCoefficient { get; set; }
        }

        public class EffluentData
        {
            public double Flow { get; set; }
            public double UltimateBOD { get; set; }
            public double DO { get; set; }
            public double Temperature { get; set; }
            public double K { get; set; }
        }

        public RiverData River { get; set; }
        public EffluentData Effluent { get; set; }

        public static void CalculateSagCurve(FlowData flow, Dictionary<double, double> result, Dictionary<string, double> summary)
        {
            try
            {
                Console.WriteLine("Sag Curve Calculation Start.");

                // Convert Ultimate BOD from kg/day to mg/L.
                double lw = flow.Effluent.UltimateBOD / flow.Effluent.Flow / 86.4;
                Console.WriteLine("lw = " + lw);

                // Calculate Mixed BOD.
                double la = (flow.Effluent.Flow * lw + flow.River.Flow * flow.River.UltimateBOD) / (flow.Effluent.Flow + flow.River.Flow);
                Console.WriteLine("la = " + la);

                // Calculate initial deficit.
                double da = 8.38 - (flow.Effluent.Flow * flow.Effluent.DO + flow.River.Flow * flow.River.DO) / (flow.Effluent.Flow + flow.River.Flow);
                Console.WriteLine("da = " + da);

                // Calculate Mixed Temperature.
                double ta = (flow.Effluent.Flow * flow.Effluent.Temperature + flow.River.Flow * flow.River.Temperature) / (flow.Effluent.Flow + flow.River.Flow);
                Console.WriteLine("ta = " + ta);

                // Calculate Kd.
                double kd = flow.Effluent.K + flow.River.Speed * flow.River.BedActivityCoefficient / 2.3 / flow.River.AverageDepth;
                kd = kd * Math.Pow(1.056, ta - 20.0);
                Console.WriteLine("kd = " + kd);

                // Calculate Kr.
                double kr = 1.7 * Math.Pow(flow.River.Speed, 0.5) / Math.Pow(flow.River.AverageDepth, 1.5);
                kr = kr * Math.Pow(1.024, ta - 20.0);
                Console.WriteLine("kr = " + kr);

                // Calculate the critical point.
                double tc = 1 / (kr - kd) * Math.Log10(kr / kd * (1 - da * (kr - kd) / kd / la));
                double dc = kd * la / (kr - kd) * (Math.Pow(10.0, -kd * tc) - Math.Pow(10.0, -kr * tc)) + da * Math.Pow(10.0, -kr * tc);
                double doc = 8.38 - dc;
                Console.WriteLine("tc = " + tc);

                // Insert curve data.
                result.Clear();
                for (double t = 0; t < tc * 4.0; t += tc/5.0)
                {
                    double d = kd * la / (kr - kd) * (Math.Pow(10.0, -kd * t) - Math.Pow(10.0, -kr * t)) + da * Math.Pow(10.0, -kr * t);
                    double y = 8.38 - d;
                    result.Add(t, y);
                }

                // Insert summary data.
                summary.Clear();
                summary.Add("kd", kd);
                summary.Add("kr", kr);
                summary.Add("tc", tc);
                summary.Add("dc", dc);
                summary.Add("doc", doc);

                Console.WriteLine("End of Sag Curve Calculation.");
            }
            catch
            {
                result.Clear();
            }
        }
    }
}
