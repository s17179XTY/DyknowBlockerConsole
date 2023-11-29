using com.sun.org.apache.xml.@internal.resolver.helpers;
using com.sun.org.apache.xml.@internal.security.keys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    public static void Main(string[] args)
    {
        var pause = false;
        var IsServerSocket = false;

        var INITIAL_DELAY = 5;
        var MAX_DELAY = 10;
        var backoffDelay = INITIAL_DELAY;
        var MONITOR_INTERVAL = 10;

        if (!IsServerSocket)
        {
            try
            {
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 22222);
                serverSocket.Start();
                IsServerSocket = true;

                if (!serverSocket.Server.IsBound)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Another instance of the application is already running. Exiting...");
                    Console.ForegroundColor = ConsoleColor.White;
                    Environment.Exit(0);
                }
            }
            catch (IOException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Unable to start the application. Exiting...");
                Console.ForegroundColor = ConsoleColor.White;
                Environment.Exit(0);
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Blockerc, created by s17179XTY");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("----------------------------");

        Console.WriteLine("Blockerc working.....");

        while (true)
        {
            if (!pause)
            {
                List<string> blacklist = GetBlacklist();

                if (blacklist != null)
                {
                    foreach (var processName in blacklist)
                    {
                        Process[] processes = Process.GetProcessesByName(processName);

                        foreach (var process in processes)
                        {
                            try
                            {
                                Thread.Sleep(backoffDelay);
                                process.Kill();
                                string logMessage = $"Blocked and terminated process '{processName}'";
                                Console.WriteLine(logMessage);
                                backoffDelay = INITIAL_DELAY;
                            }
                            catch (Exception ex)
                            {
                                string errorMessage = $"Error blocking process '{processName}': {ex.Message}";
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(errorMessage);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }

                        Thread.Sleep(MONITOR_INTERVAL);
                        if (backoffDelay < MAX_DELAY)
                        {
                            backoffDelay *= 2;
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error finding files");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                GC.Collect();
            }

            Thread.Sleep(200);
        }
    }

    private static List<string>? GetBlacklist()
    {
        List<string> fileNames = new List<string>();

        try
        {
            var paths = Directory.GetFiles("C:\\Program Files\\DyKnow\\", "*.exe", SearchOption.AllDirectories);
            foreach (var path in paths)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (!fileName.Equals("DyKnow", StringComparison.OrdinalIgnoreCase))
                {
                    fileNames.Add(fileName);
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.StackTrace);
        }

        if (fileNames.Count == 0)
        {
            Thread.Sleep(1000);
            return null;
        }

        return fileNames;
    }
}
