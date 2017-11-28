using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.DataVisualization.Charting;

namespace Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<double, double> sagCurve;
        private Dictionary<string, double> summary;
        private FlowData flowData;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            BindData();
            PlotFigure(null, null);
        }

        public void InitializeData()
        {
            flowData = new FlowData
            {
                River = new FlowData.RiverData
                {
                    Flow = 0.5,
                    UltimateBOD = 19,
                    DO = 5.85,
                    Temperature = 25,
                    Speed = 0.1,
                    AverageDepth = 4,
                    BedActivityCoefficient = 0.2
                },
                Effluent = new FlowData.EffluentData
                {
                    Flow = 0.05,
                    UltimateBOD = 129.6,
                    DO = 0.9,
                    Temperature = 25,
                    K = 0.05
                }
            };

            sagCurve = new Dictionary<double, double>();
            summary = new Dictionary<string, double>();
            FlowData.CalculateSagCurve(flowData, sagCurve, summary);
        }

        public void BindData()
        {
            EffluentGroupBox.DataContext = flowData;
            RiverGroupBox.DataContext = flowData;
        }

        private void PlotFigure(object sender, RoutedEventArgs e)
        {
            FlowData.CalculateSagCurve(flowData, sagCurve, summary);

            Axis xaxis = ResultFigure.ChartAreas["sag"].AxisX;
            Axis yaxis = ResultFigure.ChartAreas["sag"].AxisY;
            xaxis.Minimum = 0;
            xaxis.Maximum = sagCurve.Last().Key;
            xaxis.Interval = 1;
            xaxis.Title = "Time (day)";
            yaxis.Minimum = 0;
            yaxis.Maximum = 10;
            yaxis.Interval = 1;
            yaxis.Title = "DO (mg/L)";

            Series sagseries = ResultFigure.Series["sag"];
            Series sagcurveseries = ResultFigure.Series["sagcurve"];
            sagseries.MarkerColor = System.Drawing.Color.Black;
            sagseries.MarkerStyle = MarkerStyle.Circle;
            sagcurveseries.Color = System.Drawing.Color.Black;

            ResultFigure.Series["sag"].Points.DataBindXY(sagCurve.Keys, sagCurve.Values);
            ResultFigure.Series["sagcurve"].Points.DataBindXY(sagCurve.Keys, sagCurve.Values);

            SummaryKd.Text = summary["kd"].ToString();
            SummaryKr.Text = summary["kr"].ToString();
            SummaryTc.Text = summary["tc"].ToString();
            SummaryD.Text = summary["dc"].ToString();
            SummaryDO.Text = summary["doc"].ToString();
        }
    }
}
