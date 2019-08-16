using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mouse_DoubleClickFixer
{
    public class UserActivityHook
    {
        private const int WH_MOUSE_LL = 14;


        private const int WH_KEYBOARD_LL = 13;


        private const int WH_MOUSE = 7;


        private const int WH_KEYBOARD = 2;


        private const int WM_MOUSEMOVE = 512;


        private const int WM_LBUTTONDOWN = 513;


        private const int WM_RBUTTONDOWN = 516;


        private const int WM_MBUTTONDOWN = 519;


        private const int WM_LBUTTONUP = 514;


        private const int WM_RBUTTONUP = 517;


        private const int WM_MBUTTONUP = 520;


        private const int WM_LBUTTONDBLCLK = 515;


        private const int WM_RBUTTONDBLCLK = 518;


        private const int WM_MBUTTONDBLCLK = 521;


        private const int WM_MOUSEWHEEL = 522;


        private const int WM_KEYDOWN = 256;


        private const int WM_KEYUP = 257;


        private const int WM_SYSKEYDOWN = 260;


        private const int WM_SYSKEYUP = 261;


        private const byte VK_SHIFT = 16;


        private const byte VK_CAPITAL = 20;


        private const byte VK_NUMLOCK = 144;


        private static HookProc MouseHookProcedure;


        private static HookProc KeyboardHookProcedure;

        private int hKeyboardHook;

        private int hMouseHook;

        private DateTime tlast;

        private DateTime tnow;

        public UserActivityHook()
        {
            Start();
        }

        public UserActivityHook(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            Start(InstallMouseHook, InstallKeyboardHook);
        }


        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);


        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);


        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey,
            int fuState);


        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern short GetKeyState(int vKey);


        ~UserActivityHook()
        {
            Stop(true, true, false);
        }


        public event MouseEventHandler OnMouseActivity;


        public event KeyEventHandler KeyDown;


        public event KeyPressEventHandler KeyPress;


        public event KeyEventHandler KeyUp;


        public void Start()
        {
            Start(true, true);
        }


        public void Start(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            if (hMouseHook == 0 && InstallMouseHook)
            {
                MouseHookProcedure = MouseHookProc;
                hMouseHook = SetWindowsHookEx(14, MouseHookProcedure,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (hMouseHook == 0)
                {
                    var lastWin32Error = Marshal.GetLastWin32Error();
                    Stop(true, false, false);
                    throw new Win32Exception(lastWin32Error);
                }
            }

            if (hKeyboardHook == 0 && InstallKeyboardHook)
            {
                KeyboardHookProcedure = KeyboardHookProc;
                hKeyboardHook = SetWindowsHookEx(13, KeyboardHookProcedure,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (hKeyboardHook == 0)
                {
                    var lastWin32Error = Marshal.GetLastWin32Error();
                    Stop(false, true, false);
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }


        public void Stop()
        {
            Stop(true, true, true);
        }


        public void Stop(bool UninstallMouseHook, bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            if (hMouseHook != 0 && UninstallMouseHook)
            {
                var num = UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
                if (num == 0 && ThrowExceptions)
                {
                    var lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }

            if (hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                var num2 = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
                if (num2 == 0 && ThrowExceptions)
                {
                    var lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }


        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            var flag = false;
            if (nCode >= 0 && OnMouseActivity != null)
            {
                var mouseLLHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));
                var mouseButtons = MouseButtons.None;

                short delta = 0;
                if (wParam != 513)
                {
                    if (wParam != 516)
                    {
                        if (wParam == 522) delta = (short)((mouseLLHookStruct.mouseData >> 16) & 65535);
                    }
                    else
                    {
                        if (Settings._right)
                        {
                            tnow = DateTime.UtcNow;
                            if ((tnow - tlast).TotalMilliseconds < 100.0)
                                flag = true;

                            tlast = tnow;
                            mouseButtons = MouseButtons.Right;
                        }
                    }
                }
                else
                {
                    if (Settings._left)
                    {
                        tnow = DateTime.UtcNow;
                        if ((tnow - tlast).TotalMilliseconds < 100.0)
                            flag = true;

                        tlast = tnow;
                        mouseButtons = MouseButtons.Left;
                    }
                }

                var clicks = 0;
                if (mouseButtons != MouseButtons.None)
                {
                    if (wParam == 515 || wParam == 518)
                        clicks = 2;
                    else
                        clicks = 1;
                }

                if (flag) clicks = 5;
                var e = new MouseEventArgs(mouseButtons, clicks, mouseLLHookStruct.pt.x, mouseLLHookStruct.pt.y, delta);
                OnMouseActivity(this, e);
            }

            int result;
            if (!flag)
                result = CallNextHookEx(hMouseHook, nCode, wParam, lParam);
            else
                result = 1;
            return result;
        }


        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            var flag = false;
            if (nCode >= 0 && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                var keyboardHookStruct =
                    (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (KeyDown != null && (wParam == 256 || wParam == 260))
                {
                    var vkCode = (Keys)keyboardHookStruct.vkCode;
                    var keyEventArgs = new KeyEventArgs(vkCode);
                    KeyDown(this, keyEventArgs);
                    flag = flag || keyEventArgs.Handled;
                }

                if (KeyPress != null && wParam == 256)
                {
                    var flag2 = (GetKeyState(16) & 128) == 128;
                    var keyState = GetKeyState(20) != 0;
                    var array = new byte[256];
                    GetKeyboardState(array);
                    var array2 = new byte[2];
                    if (ToAscii(keyboardHookStruct.vkCode, keyboardHookStruct.scanCode, array, array2,
                            keyboardHookStruct.flags) == 1)
                    {
                        var c = (char)array2[0];
                        if (keyState ^ flag2 && char.IsLetter(c)) c = char.ToUpper(c);
                        var keyPressEventArgs = new KeyPressEventArgs(c);
                        KeyPress(this, keyPressEventArgs);
                        flag = flag || keyPressEventArgs.Handled;
                    }
                }

                if (KeyUp != null && (wParam == 257 || wParam == 261))
                {
                    var vkCode = (Keys)keyboardHookStruct.vkCode;
                    var keyEventArgs = new KeyEventArgs(vkCode);
                    KeyUp(this, keyEventArgs);
                    flag = flag || keyEventArgs.Handled;
                }
            }

            int result;
            if (flag)
                result = 1;
            else
                result = CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            return result;
        }


        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x;


            public int y;
        }


        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            public int dwExtraInfo;

            public int hwnd;

            public POINT pt;

            public int wHitTestCode;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            public int dwExtraInfo;

            public int flags;

            public int mouseData;

            public POINT pt;

            public int time;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            public int dwExtraInfo;

            public int flags;

            public int scanCode;

            public int time;

            public int vkCode;
        }

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
    }
}