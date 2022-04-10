// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using RandomGen;

namespace TraceviewZobalesaz
{

  class Program
  {
    public static void Main(string[] args)
    {
      var cts = new CancellationTokenSource();

      var levelGen = Gen.Random.Items(new[] { "Info", "Critical", "Fatal", "Error", "Verbose" });
      var elapsedGen = Gen.Random.Numbers.Doubles(0, 1000_000_000);
      var categoryGen = Gen.Random.Items(new[] { "Application", "System", "Log" });
      var messageGen = Gen.Random.Text.Long();

      Func<TraceDto> getRandomTrace = () =>
      {
        return new TraceDto()
        {
          Category = categoryGen(),
          Level = levelGen(),
          Message = messageGen(),
          ElapsedNanos = elapsedGen()
        };
      };

      CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
        .WithNotParsed(HandleParseError)
        .WithParsed((CommandLineOptions opts) =>
        {
          var udpClient = new UdpClient(opts.Host, opts.Port);
          var delayGen = Gen.Random.Numbers.Integers(0, opts.AverageLatencyMS * 2);
          var tasks = new List<Task>();

          for (int i = 0; i < opts.Concurrency; i++)
          {
            var localI = i;
            var task = Task.Run(async () =>
            {
              while (!cts.IsCancellationRequested)
              {
                await Task.Delay(delayGen(), cts.Token);
                await SendRandomTraceAsync(localI, udpClient, getRandomTrace(), opts.Verbose);
              }
            });

            tasks.Add(task);
          }
        });

      Console.ReadLine();
      Console.WriteLine("About to shut down...");

      cts.Cancel();
    }

    static async Task SendRandomTraceAsync(int workerId, UdpClient client, TraceDto trace, bool verbose)
    {
      try
      {
        var j = JsonSerializer.Serialize(trace);
        var buffer = Encoding.UTF8.GetBytes(j);
        await client.SendAsync(buffer, buffer.Length);
        if (verbose)
          Console.WriteLine($"Sent UDP (Worker {workerId})");
      }
      catch (SocketException se)
      {
        if (se.Message == "Connection refused")
          Console.WriteLine(se.Message);
        else
          Console.WriteLine(se);
      }
      catch (Exception e)
      {
        // should never cause unhandled exception
        Console.WriteLine(e);
      }
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
      Console.WriteLine("Usage: -c [concurrency] -t [average interval]");

      Console.WriteLine("Error parsing command line");
      foreach (var error in errs)
      {
        Console.WriteLine(error);
      }
    }
  }
}