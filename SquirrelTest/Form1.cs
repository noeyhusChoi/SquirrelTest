using Squirrel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SquirrelTest
{
    public partial class Form1 : Form
    {
        private Update update = new Update();
        private Timer updateTimer;

        public Form1()
        {
            InitializeComponent();
            // Load 이벤트 핸들러 등록
            this.Load += Form1_Load;


            // 2. 타이머로 주기적 체크
            updateTimer = new Timer();
            updateTimer.Interval = 10000; // 10초
            updateTimer.Tick += async (s, e) => await update.CheckForUpdatesAsync();
            updateTimer.Start();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await update.CheckForUpdatesAsync();
        }

    }
}