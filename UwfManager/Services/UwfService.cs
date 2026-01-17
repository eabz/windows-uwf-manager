using System;
using System.Diagnostics;
using System.Text;

namespace UwfManager.Services
{
    public class UwfService
    {
        private const string UwfMgrExe = "uwfmgr.exe";

        public struct UwfStatus
        {
            public bool IsEnabled;
            public string OverlayUsed;
            public string OverlayMax;
        }

        public string GetRawStatus()
        {
            return RunUwfCommand("get-config");
        }

        public UwfStatus GetStatus()
        {
            var output = RunUwfCommand("get-config");
            var status = new UwfStatus();
            
            // Simple parsing logic - relies on standard English output of uwfmgr
            // This is brittle but common for wrapping CLI tools without API
            if (output.Contains("Current Session Settings", StringComparison.OrdinalIgnoreCase))
            {
                // Looking for "Filter: ON" or "Filter: OFF"
                // This is a naive check, improved by splitting lines
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("Filter:", StringComparison.OrdinalIgnoreCase))
                    {
                        status.IsEnabled = line.Contains("ON", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            
            return status;
        }

        public void EnableInfo()
        {
            RunUwfCommand("filter enable");
        }

        public void DisableInfo()
        {
            RunUwfCommand("filter disable");
        }
        
        public void Restart()
        {
            Process.Start("shutdown", "/r /t 0");
        }

        private string RunUwfCommand(string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = UwfMgrExe,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas" // Request elevation (though manifest handles app level)
                };

                using var process = Process.Start(psi);
                if (process == null) return "Failed to start uwfmgr";
                
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    return $"Error: {error}\nOutput: {output}";
                }

                return output;
            }
            catch (Exception ex)
            {
                // Handle case where uwfmgr is not found (e.g. dev machine)
                return $"Exception executing uwfmgr: {ex.Message}. (Is UWF installed?)";
            }
        }
    }
}
