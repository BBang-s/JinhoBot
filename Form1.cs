using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //FindWindow (최상위 창 핸들값 가져오는 API)
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //FindWindowEX (인자로 받아온 핸들의 자식의 핸들값 가져오는 API)
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        //SendMessage
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, string lParam);
       
        //PostMessage
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        //window 여부 체크
        [DllImport("user32.dll")]
        static extern bool IsWindow(IntPtr hWnd);

        //window Text 가져오기
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
                    
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendKaKaoTalk(txtChattingRoom.Text, txtMsg.Text);
        }


        public void SendKaKaoTalk(string title, string msg)
        {
            IntPtr hd01 = FindWindow(null, title);
            if (IsValidChatWindow(hd01))
            {
                IntPtr hd03 = FindWindowEx(hd01, IntPtr.Zero, "RichEdit20W", "");
                if (hd03 != IntPtr.Zero)
                {
                    SendMessage(hd03, 0x000c, IntPtr.Zero, msg);
                    PostMessage(hd03, 0x0100, new IntPtr(0x0D), null);
                }
            }
        }

        public static bool IsValidKakaotalkWindow(IntPtr hWnd)
        {
            // 창 핸들인지 확인한다.
            if (!IsWindow(hWnd)) return false;

            // 윈도우의 제목을 가져온다.
            StringBuilder sbWinText = new StringBuilder(260);
            GetWindowText(hWnd, sbWinText, 260);
            if (sbWinText.ToString() == null || sbWinText.ToString() == string.Empty ) return false;

            // 총 2개의 하위 다이얼로그가 있으므로
            // 핸들을 가져온다.
            IntPtr hChildDialog1 = FindWindowEx(hWnd, IntPtr.Zero, "#32770", null);
            IntPtr hChildDialog2 = FindWindowEx(hWnd, hChildDialog1, "#32770", null);

            // 두 개의 다이얼로그 중 하나의 값이라도 받아오지 못한 경우
            // 정상적인 카카오톡 창이 아니다.
            if (hChildDialog1 == IntPtr.Zero || hChildDialog2 == IntPtr.Zero) return false;

            // 이제 다이얼로그의 하위 구조를 확인한다.
            IntPtr hDialogChildDialog1 = FindWindowEx(hChildDialog1, IntPtr.Zero, "#32770", null);
            IntPtr hDialogChildDialog2 = FindWindowEx(hChildDialog1, hDialogChildDialog1, "#32770", null);
            IntPtr hDialogChildEvaWindow1 = FindWindowEx(hChildDialog1, IntPtr.Zero, "EVA_Window", null);
            IntPtr hDialogChildEvaWindow2 = FindWindowEx(hChildDialog1, hDialogChildEvaWindow1, "EVA_Window", null);

            // 네 개의 윈도우 핸들이 유효하다면 정상적인 카카오톡 창이다.
            // 더 깊게 들어가야 하지만, 이 정도만 검사하면 된다.
            return
                hDialogChildDialog1 != IntPtr.Zero &&
                hDialogChildDialog2 != IntPtr.Zero &&
                hDialogChildEvaWindow1 != IntPtr.Zero &&
                hDialogChildEvaWindow2 != IntPtr.Zero;
        }

        public static bool IsValidChatWindow(IntPtr hWnd)
        {
            if (!IsWindow(hWnd)) return false;

            IntPtr hEdit;
            hEdit = FindWindowEx(hWnd, IntPtr.Zero, "RichEdit20W", null);
            return (hEdit != IntPtr.Zero);
        }
    }
}
