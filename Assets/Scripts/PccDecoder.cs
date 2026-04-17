using UnityEngine;
using System.Diagnostics; 
using System.IO;

public class PccDecoder : MonoBehaviour
{
    
    public string decoderPath = "C:/TMC2/bin/PccAppDecoder.exe"; 

    public void RunDecoder(string binPath, string outPlyPath)
    {
        
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = decoderPath;
        

        startInfo.Arguments = $"--videoBitstreamPath=\"{binPath}\" --reconstructionPath=\"{outPlyPath}\"";
        
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true; 


        UnityEngine.Debug.Log("decoding ...");
        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
            UnityEngine.Debug.Log("finished: " + outPlyPath);
        }
    }
}