using Squirrel;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


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
        string apiUrl = "https://api.github.com/repos/CSH/SquirrelTest/releases/latest";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Squirrel-Updater");
            var json = await client.GetStringAsync(apiUrl);
            var obj = JObject.Parse(json);
            return (string)obj["tag_name"];
        }
    }

    public async Task CheckForUpdatesAsync()
    {
        try
        {
            string latestTag = await GetLatestReleaseTagAsync();
            string updateUrl = $"https://github.com/CSH/SquirrelTest/releases/download/{latestTag}/"; // 마지막 슬래시 중요!

            using (var mgr = new UpdateManager(updateUrl))
            {
                var updateInfo = await mgr.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    await mgr.UpdateApp();
                    MessageBox.Show("최신 릴리즈로 업데이트 완료! 앱을 재시작합니다.");
                    UpdateManager.RestartApp();
                }
                else
                {
                    // 이미 최신
                    // MessageBox.Show("최신 버전입니다.");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("업데이트 오류: " + ex.Message);
        }
    }
}