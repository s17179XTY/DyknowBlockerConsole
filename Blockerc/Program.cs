using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
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
                    Console.Error.WriteLine("Another instance of the application is already running. Exiting...");
                    Environment.Exit(0);
                }
            }
            catch (IOException)
            {
                Console.Error.WriteLine("Unable to start the application. Exiting...");
                Environment.Exit(0);
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Blockerc, created by s17179XTY");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("----------------------------");
        Console.WriteLine("Press 'P' to pause, press again to continue");
        Console.WriteLine("Press 'Enter' to refresh log");
        Console.WriteLine("----------------------------");

        Console.WriteLine("Blockerc working.....");

        while (true)
        {
            while (Console.ReadKey().Key == ConsoleKey.P)
            {
                Console.Clear();
                pause = !pause;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Blocker paused, press 'P' to continue");
                Thread.Sleep(200);
                if (!pause)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Blockerc, created by s17179XTY");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("Press 'P' to pause, press again to continue");
                    Console.WriteLine("Press 'Enter' to refresh log");
                    Console.WriteLine("----------------------------");

                    Console.WriteLine("Blockerc working.....");
                }
            }

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
                                Console.WriteLine(errorMessage);
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
                    Console.WriteLine("Error finding files");
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