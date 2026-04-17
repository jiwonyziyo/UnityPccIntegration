using UnityEngine;
using System.Diagnostics; 
using System.IO;

public class PccDecoder : MonoBehaviour
{
    [Header("Decoder Settings")]
    public string decoderPath = "D:/3DGS/UnityPccIntegration/Assets/Plugins/PccAppDecoder.exe"; 

    void Start()
    {
        string binFolder = Path.Combine(Application.dataPath, "Data", "binData");
        
        // Ensure the input directory exists
        if (!Directory.Exists(binFolder))
        {
            Directory.CreateDirectory(binFolder);
            UnityEngine.Debug.LogWarning("Input directory not found. Created: " + binFolder);
            return;
        }

        string[] files = Directory.GetFiles(binFolder, "*.bin");

        if (files.Length > 0)
        {
            string inputBinPath = files[0];
            string fileNameOnly = Path.GetFileNameWithoutExtension(inputBinPath);
        
            string outputFolder = Path.Combine(Application.dataPath, "Data", "outPlyData");
            
            // Safety: Ensure output directory exists
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Path with %04d for the decoder's naming convention
            string outputPlyPath = Path.Combine(outputFolder, fileNameOnly + "_%04d.ply");

            ExecuteDecoding(inputBinPath, outputPlyPath);
        }
        else
        {
            UnityEngine.Debug.LogError("Error: No .bin files found in " + binFolder);
        }
    }

    public void ExecuteDecoding(string binPath, string outPlyPath)
    {
        if (!File.Exists(decoderPath))
        {
            UnityEngine.Debug.LogError("Execution Failed: Decoder not found at " + decoderPath);
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = decoderPath;
        startInfo.Arguments = $"--compressedStreamPath=\"{binPath}\" --reconstructedDataPath=\"{outPlyPath}\"";
    
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true; 
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;

        UnityEngine.Debug.Log("Decoding in progress: " + binPath);
        
        using (Process process = Process.Start(startInfo))
        {
            string stdOutput = process.StandardOutput.ReadToEnd();
            string stdError = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(stdOutput)) UnityEngine.Debug.Log("Decoder Output: " + stdOutput);
            if (!string.IsNullOrEmpty(stdError)) UnityEngine.Debug.LogError("Decoder Error: " + stdError);

            UnityEngine.Debug.Log("Process finished. Exit Code: " + process.ExitCode + " (0 = Success)");
        }
    }
}