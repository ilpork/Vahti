using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Vahti.Mobile.Forms.Localization;
using Vahti.Mobile.Forms.Theme;
using Vahti.Shared.Enum;
using Xamarin.Essentials;

namespace Vahti.Mobile.Forms.Models
{
    /// <summary>
    /// Provides graph models used in measurement history graphs
    /// </summary>
    public class GraphModel
    {
        private static NumberFormatInfo DotNumberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        public static IPlotModel GetPlotModel(MeasurementHistory historyData, SensorClass sensorClass, string sensorName, bool showMinMax)
        {
            var graphDataPoints = new List<GraphData>();

            foreach (var item in historyData.Values.OrderBy(i => i.Timestamp))
            {
                graphDataPoints.Add(new GraphData()
                {
                    Y = double.Parse(item.Value, CultureInfo.InvariantCulture),
                    X = item.Timestamp
                });
            }

            var theme = (ColorThemeEnum)Preferences.Get(ColorTheme.ColorThemePreferenceName, 0);

            var plotModel = new ViewResolvingPlotModel() { Title = $"{sensorName} ({historyData.Unit})" };
            var ls = new AreaSeries() { DataFieldX = "X", DataFieldY = "Y", ItemsSource = graphDataPoints };
            plotModel.Series.Add(ls);

            // If values are all same, use custom min/max values to show graph
            var minValue = GetMinimumValue(graphDataPoints, sensorClass);
            var maxValue = GetMaximumValue(graphDataPoints, sensorClass);
            if (minValue == maxValue)
            {
                minValue = minValue - 5;
                maxValue = maxValue + 5;
            }

            plotModel.Axes.Add(GetLinearAxis(theme, AxisPosition.Left, minValue, maxValue, GetMajorStep(minValue, maxValue, sensorClass)));
            plotModel.Axes.Add(GetLinearAxis(theme, AxisPosition.Right, minValue, maxValue, GetMajorStep(minValue, maxValue, sensorClass)));
            plotModel.Axes.Add(GetDateTimeAxis(theme));
            
            if (showMinMax)
            {                
                try
                {
                    var minValue24h = Get24hMinimumValue(graphDataPoints, sensorClass);
                    var maxValue24h = Get24hMaximumValue(graphDataPoints, sensorClass);

                    plotModel.Annotations.Add(new CustomTextAnnotation()
                    {
                        X = 6,
                        Y = 6,
                        Text = string.Format(Resources.AppResources.Graph_MinMax,
                        DisplayValueFormatter.GetMeasurementDisplayValue(sensorClass, minValue24h.ToString(DotNumberFormatInfo)),
                        DisplayValueFormatter.GetMeasurementDisplayValue(sensorClass, maxValue24h.ToString(DotNumberFormatInfo)), historyData.Unit),
                        FontSize = 12,
                        TextColor = OxyColors.White
                    });
                }
                catch (InvalidOperationException)
                {
                    // No values from last 24h, do not add min/max annotation
                }
            }
            
            switch (theme)
            {
                default:
                case ColorThemeEnum.Gray:
                    plotModel.TitleColor = OxyColors.WhiteSmoke;
                    plotModel.PlotAreaBorderColor = OxyColors.DimGray;
                    ((LineSeries)plotModel.Series[0]).Color = OxyColor.Parse("#FF9FA8DA");
                    break;               
                case ColorThemeEnum.Light:
                    plotModel.TitleColor = OxyColors.Black;
                    plotModel.PlotAreaBorderColor = OxyColors.Black;
                    ((LineSeries)plotModel.Series[0]).Color = OxyColor.Parse("#FF3F51B5");
                    break;
            }
            return plotModel;
        }

        private static double GetMajorStep(double minValue, double maxValue, SensorClass sensorClass)
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

        private static  double GetMinimumValue(List<GraphData> graphData, SensorClass category)
        {
            var dataMinValue = graphData.Min(t => t.Y);
            var dataMaxValue = graphData.Max(t => t.Y);
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
                    return minValue;
                case SensorClass.Temperature:
                    return minValue - 5;
                case SensorClass.Humidity:
                    return 0;
                default:
                    return minValue;
            }
        }

        private static double Get24hMinimumValue(List<GraphData> graphData, SensorClass category)
        {
            var now = DateTime.Now;
            var dataMinValue = graphData.Where(t => (now - t.X).Days == 0).Min(t => t.Y);

            return dataMinValue;
        }

        private static double Get24hMaximumValue(List<GraphData> graphData, SensorClass category)
        {
            var now = DateTime.Now;           
            var dataMaxValue = graphData.Where(t => (now - t.X).Days == 0).Max(t => t.Y);

            return dataMaxValue;
        }

        private static double GetMaximumValue(List<GraphData> graphData, SensorClass category)
        {
            var dataMinValue = graphData.Min(t => t.Y);
            var dataMaxValue = graphData.Max(t => t.Y);
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
                    return maxValue;
                case SensorClass.Temperature:
                    return maxValue + 5;
                case SensorClass.Humidity:
                    return 100;
                default:
                    return maxValue;
            }
        }


        private static DateTimeAxis GetDateTimeAxis(ColorThemeEnum theme)
        {
            OxyColor textColor, tickLineColor, axisLineColor, majorGridLineColor, minorGridLineColor;
            textColor = tickLineColor = axisLineColor = OxyColors.White;
            switch (theme)
            {
                case ColorThemeEnum.Gray:
                    majorGridLineColor = OxyColors.DimGray;
                    minorGridLineColor = OxyColor.FromRgb(0x50, 0x50, 0x50);
                    textColor = tickLineColor = axisLineColor = OxyColors.White;
                    break;
                case ColorThemeEnum.Light:
                    majorGridLineColor = OxyColors.LightGray;
                    minorGridLineColor = OxyColor.FromRgb(0xDF, 0xDF, 0xDF);
                    textColor = tickLineColor = axisLineColor = OxyColors.Black;
                    break;
                default:
                    majorGridLineColor = OxyColors.Gray;
                    minorGridLineColor = OxyColors.DimGray;
                    break;
            }

            return new DateTimeAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Angle = 0,
                StringFormat = "ddd\nHH:mm",
                MajorStep = 1.0 / 4, // 1/24 = 1 hour, 1/24/2 = 30 minutes                    
                IsZoomEnabled = false,
                MaximumPadding = 0,
                MinimumPadding = 0,
                TickStyle = TickStyle.Outside,
                MinorStep = 1.0 / 4,
                TextColor = textColor,
                TicklineColor = tickLineColor,
                AxislineColor = axisLineColor,
                MinimumMajorStep = 1.0 / 1,
                MajorGridlineColor = majorGridLineColor,
                MinorGridlineColor = minorGridLineColor

            };
        }

        private static LinearAxis GetLinearAxis(ColorThemeEnum theme, AxisPosition position, double minimum, double maximum, double majorStep)
        {
            OxyColor textColor, tickLineColor, axisLineColor, majorGridLineColor, minorGridLineColor;
            
            switch (theme)
            {
                case ColorThemeEnum.Gray:
                    majorGridLineColor = OxyColors.DimGray;
                    minorGridLineColor = OxyColors.DimGray;
                    textColor = tickLineColor = axisLineColor = OxyColors.White;
                    break;
                default:
                    majorGridLineColor = OxyColors.LightGray;
                    minorGridLineColor = OxyColors.LightGray;
                    textColor = tickLineColor = axisLineColor = OxyColors.Black;
                    break;
            }

            return new LinearAxis()
            {
                Position = position,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineThickness = 0.6,
                TickStyle = TickStyle.Outside,
                Minimum = minimum,
                Maximum = maximum,
                MajorStep = majorStep,
                TextColor = textColor,
                TicklineColor = tickLineColor,
                AxislineColor = axisLineColor,
                MajorGridlineColor = majorGridLineColor,
                MinorGridlineColor = minorGridLineColor
            };
        }

        private class CustomTextAnnotation : Annotation
        {
            public CustomTextAnnotation()
            { }

            public string Text { get; set; }
            public double X { get; set; }
            public double Y { get; set; }

            public override void Render(IRenderContext rc)
            {
                base.Render(rc);
                double pX = PlotModel.PlotArea.Left + X;
                double pY = PlotModel.PlotArea.Top + Y;
                rc.DrawMultilineText(new ScreenPoint(pX, pY), Text, TextColor, Font, FontSize, FontWeight);
            }
        }
    }
}
