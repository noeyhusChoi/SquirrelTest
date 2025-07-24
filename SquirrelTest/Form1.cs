using Squirrel;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SquirrelTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Load 이벤트 핸들러 등록
            this.Load += Form1_Load;

            //TEST
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await CheckForUpdatesAsync();
        }

        public async Task CheckForUpdatesAsync()
        {
            // (추천) file:///로 경로 명시
            string updatePath = @"file:///C:/Users/niaci/Downloads/Squirrel/Releases";

            try
            {
                using (var mgr = new UpdateManager(updatePath))
                {
                    var updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        await mgr.UpdateApp();
                        // 반드시 UI 스레드에서 메시지박스
                        this.Invoke((Action)(() =>
                        {
                            var dr = MessageBox.Show("업데이트가 완료되었습니다. 재시작할까요?", "업데이트", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                Application.Restart();
                                Environment.Exit(0);
                            }
                        }));
                    }
                    else
                    {
                        this.Invoke((Action)(() =>
                        {
                            MessageBox.Show("이미 최신 버전입니다.");
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                {
                    MessageBox.Show("업데이트 오류: " + ex.Message);
                }));
            }
        }
    }
}