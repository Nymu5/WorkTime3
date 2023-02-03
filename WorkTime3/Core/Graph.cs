using DynamicData;
using MyTime.Model;
using Newtonsoft.Json;

namespace MyTime.Core;

public static class Graph
{ 
    public static string BuildEarningsHtml(int year, List<Time> times,
        Employer[] employers, double[,,] earningsCube)
    {
        var chartConfigScript = GetChartScript(year, times, employers, earningsCube);
        var html = GetHtmlWithChartConfig(chartConfigScript);
        return html;
    }

    private static string GetHtmlWithChartConfig(string chartConfig)
    {
        var inlineStyle = "style=\"width:100%;height:100%;overflow:hidden;\"";
        var chartConfigJsScript = $"<script>{chartConfig}</script>";
        var chartContent = $@"
<div style=""position: fixed; height: 100%; width: 100%"">
    <div id=""chart-container"" {inlineStyle}>
      <canvas id=""chart"" />
    </div>
</div>";
        var document = $@"
<html style=""width:97%;height:100%;overflow:hidden;"">
  <head></head>
  <body {inlineStyle}>
    {chartContent}
    {chartConfigJsScript}
  </body>
</html>";
        return document;
    }

    private static string GetChartScript(int year, List<Time> times,
        Employer[] employers, double[,,] earningsCube)
    {
        var chartConfig = GetEarningsChartConfig(year, times, employers, earningsCube);
        var script = $@"
var config = {chartConfig};
config.options.plugins.tooltip.callbacks.label = function(context) {{
    let label = context.dataset.label || '';

    if (label) label += ': ';
    if (context.parsed.y !== null) label += new Intl.NumberFormat('{Constants.LanguageSymbol}', {{ style: 'currency', currency: '{Constants.CurrencySymbol}' }}).format(context.parsed.y);
    return label;
}}
config.options.scales.y.ticks.callback = function(value, index, values) {{
    return new Intl.NumberFormat('{Constants.LanguageSymbol}', {{ style: 'currency', currency: '{Constants.CurrencySymbol}', maximumFractionDigits: 0 }}).format(value.toFixed());
}}
window.onload = function() {{
  {Constants.ChartJs}
  config.plugins = [{{
    beforeInit: function(chart) {{
      const originalFit = chart.legend.fit;
      chart.legend.fit = function fit() {{
          originalFit.bind(chart.legend)();
          this.height += 50;
      }}
    }}
  }}]
  Chart.defaults.font.size = 40;
  Chart.defaults.plugins.legend.labels.padding = 50
  var canvasContext = document.getElementById(""chart"").getContext(""2d"");
  new Chart(canvasContext, config);
}};";
        return script;
    }

    private static object GetBarChartData(int year, List<Time> times,
        Employer[] employers, double[,,] earningsCube)
    {
        if (!(employers.Length > 0 && times.Count > 0)) return String.Empty;
        if (times.Max(t => t.Start.Year) < year || times.Min(t => t.Start.Year) > year)
            year = times.Max(t => t.Start.Year);
        var dataList = new List<object>();
        var colors = Constants.GetDefaultColors();
        for (int i = 0; i < employers.Length; i++)
        {
            double[] values = new double[12];
            for (int j = 0; j < 12; j++)
            {
                values[j] = earningsCube[i, year - times.Min(t => t.Start.Year), j];
            }
            dataList.Add(new
            {
                label = $"{employers[i].Name}",
                data = values.ToArray(),
                backgroundColor = $"rgb({colors[i%10].Item1},{colors[i%10].Item2},{colors[i%10].Item3})"
            });
        }
        
        
        var labels = Constants.Months;
        var data = new
        {
            datasets = dataList,
            labels
        };
        return data;
    }

    private static string GetEarningsChartConfig(int year, List<Time> times,
        Employer[] employers, double[,,] earningsCube)
    {
        var config = new
        {
            type = "bar",
            data = GetBarChartData(year, times, employers, earningsCube),
            options = new
            {
                plugins = new
                {
                    tooltip = new
                    {
                        callbacks = new
                        {
                            
                        }
                    },
                    legend = new
                    {
                        labels = new
                        {
                            font = new
                            {
                                size = 40,
                            }
                        }
                    }
                },
                responsive = true,
                maintainAspectRatio = false,
                legend = new
                {
                    position = "top"
                },
                animation = false,
                scales = new
                {
                    x = new
                    {
                        stacked = true,
                        ticks = new
                        {
                            padding = 20,
                        }
                    },
                    y = new
                    {
                        stacked = true,
                        ticks = new
                        {
                            beginAtZero = true,
                            padding = 0,
                        }
                    }
                },
                layout = new
                {
                    padding = new
                    {
                        top = -40
                    }
                }
            }
        };
        var jsonConfig = JsonConvert.SerializeObject(config);
        Console.WriteLine(jsonConfig);
        return jsonConfig;
    } 

    public static double[,,] CreateEarningsCube(List<Time> times,
        Employer[] employers)
    {
        
        int minYear = times.Min(t => t.Start.Year);
        int maxYear = times.Max(t => t.Start.Year);
        int years = maxYear - minYear + 2;

        double[,,] earningsCube = new double[employers.Length + 1, years, 13];

        foreach (Time time in times)
        {
            int pos1 = employers.IndexOf(employers.FirstOrDefault(e => e.Id == time.Employer.Id));
            int pos2 = time.Start.Year - minYear;
            int pos3 = time.Start.Month - 1;
            
            earningsCube[pos1, pos2, pos3] += time.Earned;
            earningsCube[employers.Length, pos2, pos3] += time.Earned;
            earningsCube[pos1, years - 1, pos3] += time.Earned;
            earningsCube[pos1, pos2, 12] += time.Earned;
            earningsCube[employers.Length, years - 1, pos3] += time.Earned;
            earningsCube[employers.Length, pos2, 12] += time.Earned;
            earningsCube[pos1, years - 1, 12] += time.Earned;
            earningsCube[employers.Length, years - 1, 12] += time.Earned;
        }

        return earningsCube;
    }
    
    
}