using UnityEngine;
using System.Diagnostics; // 외부 프로세스 실행을 위해 필수!
using System.IO;

public class PccDecoder : MonoBehaviour
{
    // 내일 윈도우에서 실제 exe 경로로 수정할 부분
    public string decoderPath = "C:/TMC2/bin/PccAppDecoder.exe"; 

    public void RunDecoder(string binPath, string outPlyPath)
    {
        // 1. 실행 설정
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = decoderPath;
        
        // 2. 하닝의 args (오늘 찾은 두 개의 경로!)
        // --videoBitstreamPath와 --reconstructionPath를 조합합니다.
        startInfo.Arguments = $"--videoBitstreamPath=\"{binPath}\" --reconstructionPath=\"{outPlyPath}\"";
        
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true; // 검은 터미널 창 안 뜨게 설정

        // 3. 실행
        UnityEngine.Debug.Log("디코딩 시작...");
        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit(); // 디코딩이 끝날 때까지 유니티를 잠시 멈춤
            UnityEngine.Debug.Log("디코딩 완료! 생성된 파일: " + outPlyPath);
        }
    }
}