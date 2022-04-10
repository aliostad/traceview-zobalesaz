using System;
using System.Collections.Generic;
using System.Security;

namespace TraceviewZobalesaz
{
  public class TraceDto
  {
    /*
     *
     * type Trace struct {
				TraceId       string
				Timestamp     time.Time
				Message       string
				CorrelationId string
				Level         string
				Metrics       map[string]float64
				Properties    map[string]string
				TimeIndex     string
			}
     */

    public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();

    public string Message { get; set; } = "";

    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");

    public string Level { get; set; } = "";
    
    public double ElapsedNanos { get; set; }

    public string Category { get; set; } = "";
  }
}