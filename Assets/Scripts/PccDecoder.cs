using UnityEngine;
using System.Diagnostics; 
using System.IO;

public class PccDecoder : MonoBehaviour
{
    [Header("1. Decoder Settings")]
    public string decoderPath = "D:/3DGS/UnityPccIntegration/Assets/Plugins/PccAppDecoder.exe";
    public string videoDecoderPath = "D:/3DGS/UnityPccIntegration/Assets/Plugins/TAppDecoder.exe";
    public string hdrConvertPath = "D:/3DGS/UnityPccIntegration/Assets/Plugins/HDRConvert.exe";
    public string colorConfigPath = "D:/3DGS/UnityPccIntegration/Assets/Plugins/yuv420torgb444.cfg";

    [Header("2. Data Folder Settings")]
    public string inputSubFolder = "Data/binData";
    public string outputSubFolder = "Data/outPlyData";

    void Start()
    {
        string binFolder = Path.Combine(Application.dataPath, inputSubFolder);
        string outputRootFolder = Path.Combine(Application.dataPath, outputSubFolder);

        string[] files = Directory.GetFiles(binFolder, "*.bin");

        if (files.Length > 0)
        {
            foreach (string inputBinPath in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(inputBinPath);
                string specificOutputFolder = Path.Combine(outputRootFolder, fileName);
                if (!Directory.Exists(specificOutputFolder)) Directory.CreateDirectory(specificOutputFolder);

                string outputPlyPattern = Path.Combine(specificOutputFolder, fileName + "_dec_%04d.ply");

                ExecuteDecoding(inputBinPath, outputPlyPattern);
            }
        }
    }

    public void ExecuteDecoding(string binPath, string outPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = decoderPath;

        startInfo.Arguments = 
            $"--compressedStreamPath=\"{binPath}\" " +
            $"--videoDecoderPath=\"{videoDecoderPath}\" " +
            $"--colorSpaceConversionPath=\"{hdrConvertPath}\" " +
            $"--inverseColorSpaceConversionConfig=\"{colorConfigPath}\" " +
            $"--reconstructedDataPath=\"{outPath}\"";
    
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true; 
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;

        UnityEngine.Debug.Log("[PCC] Full Argument: " + startInfo.Arguments);

        using (Process process = Process.Start(startInfo))
        {
            string stdOutput = process.StandardOutput.ReadToEnd();
            string stdError = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode == 0)
                UnityEngine.Debug.Log("<color=green>[PCC] Decoding Success!</color>");
            else
                UnityEngine.Debug.LogError("[PCC Error] " + stdError + "\nOutput: " + stdOutput);
        }
    }
}

