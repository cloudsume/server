namespace ExceptionMetrics;

using System;

public interface IExceptionAnalyzer
{
    AnalyzingResult Analyze(Exception exception);
}
