using System.Globalization;
using Vahti.Mobile.Localization;
using Vahti.Mobile.Theme;
using Vahti.Shared.Enum;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Vahti.Mobile.Models;
using Vahti.Mobile.Constants;

namespace Vahti.Mobile.Services
{
    /// <summary>
    /// Provides graphs used on measurement history page
    /// </summary>
    public class GraphService : IGraphService
    {
        private static NumberFormatInfo DotNumberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        public ChartModel GetChart(MeasurementHistory historyData, SensorClass sensorClass, string sensorName, bool showMinMax)
        {
            var chart = new ChartModel();
            chart.Title = $"{sensorName} ({historyData.Unit})";
            var theme = (ColorTheme)Preferences.Get(ColorThemeChanger.ColorThemePreferenceName, 0);
            var graphDataPoints = new List<DateTimePoint>();

            foreach (var item in historyData.Values.OrderBy(i => i.Timestamp))
            {
                graphDataPoints.Add(new DateTimePoint(item.Timestamp,
                    double.Parse(item.Value, CultureInfo.InvariantCulture)));
            }

            // If values are all same, use custom min/max values to show graph            
            if (showMinMax)
            {
                var minValue24h = Get24hMinimumValue(graphDataPoints);
                var maxValue24h = Get24hMaximumValue(graphDataPoints);

                if (minValue24h != null && maxValue24h != null)
                {
                    chart.LatestMinMax = string.Format(Resources.AppResources.Graph_MinMax,
                            DisplayValueFormatter.GetMeasurementDisplayValue(sensorClass, minValue24h.Value.ToString(DotNumberFormatInfo)),
                            DisplayValueFormatter.GetMeasurementDisplayValue(sensorClass, maxValue24h.Value.ToString(DotNumberFormatInfo)),
                            historyData.Unit);
                }
            }
           
            var fillColor = SKColor.Parse((theme == ColorTheme.Gray ?
                (Color)Application.Current.Resources[ResourceNames.GrayThemeChartFill] :
                (Color)Application.Current.Resources[ResourceNames.LightThemeChartFill]).ToArgbHex());
            var strokeColor = SKColor.Parse((theme == ColorTheme.Gray ?
                (Color)Application.Current.Resources[ResourceNames.GrayThemeChartStroke] :
                (Color)Application.Current.Resources[ResourceNames.LightThemeChartStroke]).ToArgbHex());

            chart.Series = new List<ISeries>()
            {
                new LineSeries<DateTimePoint>
                {
                    XToolTipLabelFormatter = (chartPoint) =>
                        $"{new DateTime((long)chartPoint.SecondaryValue):ddd HH:mm}: {chartPoint.PrimaryValue}",
                    Values = graphDataPoints,
                    Stroke = new SolidColorPaint(strokeColor) { StrokeThickness = 4},
                    Fill = new SolidColorPaint(fillColor),
                    GeometryFill = null,
                    GeometryStroke = null,
                    YToolTipLabelFormatter = (chartPoint) => $"{GetYAxisLabel(chartPoint.PrimaryValue, sensorClass)} {historyData.Unit}",
                    Name = sensorName
                }
            };

            InitializeAxes(chart, sensorClass, theme, graphDataPoints);

            return chart;
        }

        private string GetYAxisLabel(double value, SensorClass sensorClass)
        {
            switch (sensorClass)
            {
                case SensorClass.AccelerationX:
                case SensorClass.AccelerationY:
                case SensorClass.AccelerationZ:
                case SensorClass.BatteryVoltage:
                    return value.ToString("0.000");
                case SensorClass.LastCharged:
                case SensorClass.Pressure:
                case SensorClass.Temperature:
                case SensorClass.Humidity:
                    return value.ToString("0");
                default:
                    return value.ToString("0");
            }
        }

        private double GetMajorStep(double minValue, double maxValue, SensorClass sensorClass)
        {
            var maxRange = maxValue - minValue;
            var majorStep = maxRange / 4;

            switch (sensorClass)
            {
                case SensorClass.AccelerationX:
                case SensorClass.AccelerationY:
                case SensorClass.AccelerationZ:
                case SensorClass.BatteryVoltage:
                case SensorClass.LastCharged:
                case SensorClass.Pressure:
                    return majorStep;
                case SensorClass.Temperature:
                    return 5;
                case SensorClass.Humidity:
                    return 20;
                default:
                    return majorStep;
            }
        }

        private double GetMinimumValue(List<DateTimePoint> graphData, SensorClass category)
        {
            var dataMinValue = graphData.Min(t => t.Value);
            var dataMaxValue = graphData.Max(t => t.Value);
            var maxRange = dataMaxValue - dataMinValue;

            var minValue = dataMinValue - maxRange * 0.3;
            switch (category)
            {
                case SensorClass.AccelerationX:
                case SensorClass.AccelerationY:
                case SensorClass.AccelerationZ:
                case SensorClass.BatteryVoltage:
                case SensorClass.LastCharged:
                case SensorClass.Pressure:
                    return minValue.Value;
                case SensorClass.Temperature:
                    return minValue.Value - 5;
                case SensorClass.Humidity:
                    return 0;
                default:
                    return minValue.Value;
            }
        }

        private double? Get24hMinimumValue(List<DateTimePoint> graphData)
        {
            var values24h = graphData.Where(t => (DateTime.Now - t.DateTime).Days == 0).ToList();
            if (values24h.Count == 0)
            {
                return null;
            }

            return values24h.Min(t => t.Value);
        }

        private double? Get24hMaximumValue(List<DateTimePoint> graphData)
        {
            var values24h = graphData.Where(t => (DateTime.Now - t.DateTime).Days == 0).ToList();
            if (values24h.Count == 0)
            {
                return null;
            }

            return values24h.Max(t => t.Value);
        }

        private double GetMaximumValue(List<DateTimePoint> graphData, SensorClass category)
        {
            var dataMinValue = graphData.Min(t => t.Value);
            var dataMaxValue = graphData.Max(t => t.Value);
            var maxRange = dataMaxValue - dataMinValue;

            var maxValue = dataMaxValue + maxRange * 0.3;
            switch (category)
            {
                case SensorClass.AccelerationX:
                case SensorClass.AccelerationY:
                case SensorClass.AccelerationZ:
                case SensorClass.BatteryVoltage:
                case SensorClass.LastCharged:
                case SensorClass.Pressure:
                    return maxValue.Value;
                case SensorClass.Temperature:
                    return maxValue.Value + 5;
                case SensorClass.Humidity:
                    return 100;
                default:
                    return maxValue.Value;
            }
        }

        private void InitializeAxes(ChartModel chart, SensorClass sensorClass, ColorTheme theme, List<DateTimePoint> graphDataPoints)
        {
            var minValue = GetMinimumValue(graphDataPoints, sensorClass);
            var maxValue = GetMaximumValue(graphDataPoints, sensorClass);
            if (minValue == maxValue)
            {
                minValue = minValue - 5;
                maxValue = maxValue + 5;
            }

            var separatorColor = SKColor.Parse((theme == ColorTheme.Gray ?
               (Color)Application.Current.Resources[ResourceNames.GrayThemeChartSeparator] :
               (Color)Application.Current.Resources[ResourceNames.LightThemeChartSeparator]).ToArgbHex());
            var subSeparatorColor = SKColor.Parse((theme == ColorTheme.Gray ?
                (Color)Application.Current.Resources[ResourceNames.GrayThemeChartSubSeparator] :
                (Color)Application.Current.Resources[ResourceNames.LightThemeChartSubSeparator]).ToArgbHex());
            var textColor = SKColor.Parse((theme == ColorTheme.Gray ?
                (Color)Application.Current.Resources[ResourceNames.GrayThemeOnSurface] :
                (Color)Application.Current.Resources[ResourceNames.LightThemeOnSurface]).ToArgbHex());

            chart.XAxes = new List<Axis>()
            {
                new Axis
                {
                    TextSize = 28,                    
                    Labeler = value => new DateTime((long) value).ToString("ddd HH:mm"),
                    LabelsPaint = new SolidColorPaint(textColor),
                    UnitWidth = TimeSpan.FromHours(1).Ticks,
                    MinStep = TimeSpan.FromHours(24).Ticks,
                    ForceStepToMin = true,
                    ShowSeparatorLines = true,
                    SeparatorsPaint = new SolidColorPaint()
                    {
                        Color = separatorColor,
                        StrokeThickness = 1,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    },
                    SubseparatorsPaint = new SolidColorPaint()
                    {
                        Color = subSeparatorColor,
                        StrokeThickness = 0.5f
                    },
                    TicksPaint = new SolidColorPaint()
                    {
                        Color = textColor,
                        StrokeThickness = 1f
                    },

                }
            };


            chart.YAxes = new List<Axis>()
            {
                new Axis
                {
                    TextSize = 24,
                    Labeler = value => GetYAxisLabel(value, sensorClass),
                    LabelsPaint = new SolidColorPaint(textColor),
                    MinLimit = minValue,
                    MaxLimit = maxValue,
                    Position = LiveChartsCore.Measure.AxisPosition.Start,
                    SeparatorsPaint = new SolidColorPaint()
                    {
                        Color = separatorColor,
                        StrokeThickness = 1,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    },
                    SubseparatorsPaint = new SolidColorPaint()
                    {
                        Color = subSeparatorColor,
                        StrokeThickness = 0.5f
                    },
                },
                new Axis
                {
                    TextSize = 24,
                    Labeler = value => GetYAxisLabel(value, sensorClass),
                    LabelsPaint = new SolidColorPaint(textColor),
                    MinLimit = minValue,
                    MaxLimit = maxValue,
                    Position = LiveChartsCore.Measure.AxisPosition.End,
                    SeparatorsPaint = new SolidColorPaint()
                    {
                        Color = separatorColor,
                        StrokeThickness = 1,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    },
                    SubseparatorsPaint = new SolidColorPaint()
                    {
                        Color = subSeparatorColor,
                        StrokeThickness = 0.5f
                    },
                }
            };
        }        
    }

    public class ChartModel
    {
        public string Title { get; set; }
        public string LatestMinMax { get; set; }
        public List<ISeries> Series { get; set; }
        public List<Axis> XAxes { get; set; }

        public List<Axis> YAxes { get; set; }
    }
}
