using System.Diagnostics;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using java.awt;
using java.awt.@event;

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

        if (IsServerSocket == false)
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
        Console.WriteLine("" +
            "---------------------" +
            "");

        if (!pause)
        {
            Console.WriteLine("Blockerc working.....");
            while (true)
            {

                List<string> blacklist = GetBlacklist();

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
                    try
                    {
                        Thread.Sleep(MONITOR_INTERVAL);
                        if (backoffDelay < MAX_DELAY)
                        {
                            backoffDelay *= 2;
                        }
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                }
                //Thread.Sleep(500);
            }
        }
        else
        {
            try
            {
                Console.Clear();
                Thread.Sleep(200);
            }
            catch (ThreadInterruptedException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            GC.Collect();
        }

        List<string> GetBlacklist()
        {
            List<string> fileNames = new List<string>();
            try
            {
                var paths = Directory.GetFiles("C:\\Program Files\\DyKnow\\", "*.exe", SearchOption.AllDirectories);
                foreach (var path in paths)
                {
                    string fileName = Path.GetFileName(path);
                    fileName = Path.GetFileNameWithoutExtension(fileName);
                    //fileNames.Add(fileName);
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
            return fileNames;
        }
    }
}