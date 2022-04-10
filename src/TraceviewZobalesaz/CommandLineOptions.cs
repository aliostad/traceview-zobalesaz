using CommandLine;

namespace TraceviewZobalesaz
{
  public class CommandLineOptions
  {
    [Option('c', "concurrency", Required = false, HelpText = "Number of workers sending UDP packets")]
    public int Concurrency { get; set; } = 10;

    [Option('t', "interval", Required = false, HelpText = "Average pause/interval between sending each trace - in milliseconds")]
    public int AverageLatencyMS { get; set; } = 500;

    [Option('v', "verbose", Required = false, HelpText = "Verbose output")]
    public bool Verbose { get; set; } = false;

    [Option('h', "host", Required = false, HelpText = "Host name to send UDP packets to")]
    public string Host { get; set; } = "localhost";

    [Option('p', "port", Required = false, HelpText = "UDP port to send the packets to")]
    public int Port { get; set; } = 1969;

    
  }
}