﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
namespace GW2MutexKiller 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string MutexName = "AN-Mutex-Window-Guild Wars 2";
            const string ProcessName = "gw2";

            try
            {
                Process process = Process.GetProcessesByName(ProcessName)[0];
                List<Win32API.SYSTEM_HANDLE_INFORMATION> handles = new List<Win32API.SYSTEM_HANDLE_INFORMATION>();
                for (int i = 0; i < 99; i++)
                {
                    handles = Win32Processes.GetHandles(process, "Mutant", "\\Sessions\\" + i + "\\BaseNamedObjects\\" + MutexName);
                    if (handles.Count > 0)
                    { 
                        break; 
                    }
                }
               
                foreach (var handle in handles)
                {
                    IntPtr ipHandle = IntPtr.Zero;
                    if (!Win32API.DuplicateHandle(Process.GetProcessById(handle.ProcessID).Handle, handle.Handle, Win32API.GetCurrentProcess(), out ipHandle, 0, false, Win32API.DUPLICATE_CLOSE_SOURCE))
                    {
                        Console.WriteLine("DuplicateHandle() failed, error = {0}", Marshal.GetLastWin32Error()); 
                    }

                    Console.WriteLine("Mutex was killed");
                }

                for (int i = 3; i > 0; i--)
                {
                    Console.WriteLine("Closing in..." + i);
                    Thread.Sleep(1000);
                }

                return;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("The process name '{0}' is not currently running", ProcessName);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("The Mutex '{0}' was not found in the process '{1}'", MutexName, ProcessName);
            }

            Console.WriteLine("Press any key to close.");
            Console.ReadLine();
        }
    }
}