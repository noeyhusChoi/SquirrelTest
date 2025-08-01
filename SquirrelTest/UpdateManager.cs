﻿using Squirrel;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


//TEST

public class Update
{
    // 이 클래스는 Squirrel을 사용하여 업데이트를 관리합니다.
    public Update()
    {
        // 생성자에서 필요한 초기화 작업을 수행할 수 있습니다.
    }

    // GitHub API를 사용하여 최신 릴리즈 태그를 가져옵니다.
    public async Task<string> GetLatestReleaseTagAsync()
    {
        string apiUrl = "https://api.github.com/repos/noeyhusChoi/SquirrelTest/releases/latest";
        using (var client = new HttpClient())
        {
            string token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            Console.WriteLine(token);
            client.DefaultRequestHeaders.Add("User-Agent", "noeyhusChoi"); // 필수!
            client.DefaultRequestHeaders.Add("Authorization", $"token {token}");

            try
            {
                var json = await client.GetStringAsync(apiUrl);
                var obj = JObject.Parse(json);

                return (string)obj["tag_name"];
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"실패: {ex.Message}");

                // HttpResponseMessage가 필요하면 GetAsync로 시도해도 됨
                // var response = await client.GetAsync(apiUrl);
                // Console.WriteLine(response.StatusCode);
                return null;
            }
        }
    }


    public async Task CheckForUpdatesAsync()
    {
        try
        {
            string latestTag = await GetLatestReleaseTagAsync();
            string updateUrl = $"https://github.com/noeyhusChoi/SquirrelTest/releases/download/{latestTag}/"; // 꼭 download!

            using (var mgr = new UpdateManager(updateUrl))
            {
                var updateInfo = await mgr.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    Console.WriteLine(updateInfo.ReleasesToApply.Count);
                    await mgr.UpdateApp(); // return 문은 제거
                    Console.WriteLine("최신 릴리즈로 업데이트 완료! 앱을 재시작합니다.");
                    UpdateManager.RestartApp();
                }
                else
                {
                    // 이미 최신
                    Console.WriteLine("최신 버전입니다.");
                    Console.WriteLine(GetCurrentFileVersion());
                    Console.WriteLine(GetCurrentVersion());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("업데이트 오류: " + ex.Message);
        }
    }

    public string GetCurrentVersion()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        return version != null ? version.ToString() : "Unknown";
    }

    public string GetCurrentFileVersion()
    {
        var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
        return versionInfo.FileVersion;
    }
}