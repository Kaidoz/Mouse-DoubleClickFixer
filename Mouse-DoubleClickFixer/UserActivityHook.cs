using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Mouse_DoubleClickFixer
{
    // Token: 0x02000003 RID: 3
    public class UserActivityHook
    {
        // Token: 0x0600000E RID: 14
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, UserActivityHook.HookProc lpfn, IntPtr hMod, int dwThreadId);

        // Token: 0x0600000F RID: 15
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        // Token: 0x06000010 RID: 16
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        // Token: 0x06000011 RID: 17
        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        // Token: 0x06000012 RID: 18
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        // Token: 0x06000013 RID: 19
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern short GetKeyState(int vKey);

        // Token: 0x06000014 RID: 20 RVA: 0x0000211F File Offset: 0x0000031F
        public UserActivityHook()
        {
            this.Start();
        }

        // Token: 0x06000015 RID: 21 RVA: 0x0000213F File Offset: 0x0000033F
        public UserActivityHook(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            this.Start(InstallMouseHook, InstallKeyboardHook);
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002508 File Offset: 0x00000708
        ~UserActivityHook()
        {
            this.Stop(true, true, false);
        }

        // Token: 0x14000001 RID: 1
        // (add) Token: 0x06000017 RID: 23 RVA: 0x0000253C File Offset: 0x0000073C
        // (remove) Token: 0x06000018 RID: 24 RVA: 0x00002578 File Offset: 0x00000778
        public event MouseEventHandler OnMouseActivity
        {
            add
            {
                MouseEventHandler mouseEventHandler = this._OnMouseActivity;
                MouseEventHandler mouseEventHandler2;
                do
                {
                    mouseEventHandler2 = mouseEventHandler;
                    MouseEventHandler value2 = (MouseEventHandler)Delegate.Combine(mouseEventHandler2, value);
                    mouseEventHandler = Interlocked.CompareExchange<MouseEventHandler>(ref this._OnMouseActivity, value2, mouseEventHandler2);
                }
                while (mouseEventHandler != mouseEventHandler2);
            }
            remove
            {
                MouseEventHandler mouseEventHandler = this._OnMouseActivity;
                MouseEventHandler mouseEventHandler2;
                do
                {
                    mouseEventHandler2 = mouseEventHandler;
                    MouseEventHandler value2 = (MouseEventHandler)Delegate.Remove(mouseEventHandler2, value);
                    mouseEventHandler = Interlocked.CompareExchange<MouseEventHandler>(ref this._OnMouseActivity, value2, mouseEventHandler2);
                }
                while (mouseEventHandler != mouseEventHandler2);
            }
        }

        // Token: 0x14000002 RID: 2
        // (add) Token: 0x06000019 RID: 25 RVA: 0x000025B4 File Offset: 0x000007B4
        // (remove) Token: 0x0600001A RID: 26 RVA: 0x000025F0 File Offset: 0x000007F0
        public event KeyEventHandler KeyDown
        {
            add
            {
                KeyEventHandler keyEventHandler = this._KeyDown;
                KeyEventHandler keyEventHandler2;
                do
                {
                    keyEventHandler2 = keyEventHandler;
                    KeyEventHandler value2 = (KeyEventHandler)Delegate.Combine(keyEventHandler2, value);
                    keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this._KeyDown, value2, keyEventHandler2);
                }
                while (keyEventHandler != keyEventHandler2);
            }
            remove
            {
                KeyEventHandler keyEventHandler = this._KeyDown;
                KeyEventHandler keyEventHandler2;
                do
                {
                    keyEventHandler2 = keyEventHandler;
                    KeyEventHandler value2 = (KeyEventHandler)Delegate.Remove(keyEventHandler2, value);
                    keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this._KeyDown, value2, keyEventHandler2);
                }
                while (keyEventHandler != keyEventHandler2);
            }
        }

        // Token: 0x14000003 RID: 3
        // (add) Token: 0x0600001B RID: 27 RVA: 0x0000262C File Offset: 0x0000082C
        // (remove) Token: 0x0600001C RID: 28 RVA: 0x00002668 File Offset: 0x00000868
        public event KeyPressEventHandler KeyPress
        {
            add
            {
                KeyPressEventHandler keyPressEventHandler = this._KeyPress;
                KeyPressEventHandler keyPressEventHandler2;
                do
                {
                    keyPressEventHandler2 = keyPressEventHandler;
                    KeyPressEventHandler value2 = (KeyPressEventHandler)Delegate.Combine(keyPressEventHandler2, value);
                    keyPressEventHandler = Interlocked.CompareExchange<KeyPressEventHandler>(ref this._KeyPress, value2, keyPressEventHandler2);
                }
                while (keyPressEventHandler != keyPressEventHandler2);
            }
            remove
            {
                KeyPressEventHandler keyPressEventHandler = this._KeyPress;
                KeyPressEventHandler keyPressEventHandler2;
                do
                {
                    keyPressEventHandler2 = keyPressEventHandler;
                    KeyPressEventHandler value2 = (KeyPressEventHandler)Delegate.Remove(keyPressEventHandler2, value);
                    keyPressEventHandler = Interlocked.CompareExchange<KeyPressEventHandler>(ref this._KeyPress, value2, keyPressEventHandler2);
                }
                while (keyPressEventHandler != keyPressEventHandler2);
            }
        }

        // Token: 0x14000004 RID: 4
        // (add) Token: 0x0600001D RID: 29 RVA: 0x000026A4 File Offset: 0x000008A4
        // (remove) Token: 0x0600001E RID: 30 RVA: 0x000026E0 File Offset: 0x000008E0
        public event KeyEventHandler KeyUp
        {
            add
            {
                KeyEventHandler keyEventHandler = this._KeyUp;
                KeyEventHandler keyEventHandler2;
                do
                {
                    keyEventHandler2 = keyEventHandler;
                    KeyEventHandler value2 = (KeyEventHandler)Delegate.Combine(keyEventHandler2, value);
                    keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this._KeyUp, value2, keyEventHandler2);
                }
                while (keyEventHandler != keyEventHandler2);
            }
            remove
            {
                KeyEventHandler keyEventHandler = this._KeyUp;
                KeyEventHandler keyEventHandler2;
                do
                {
                    keyEventHandler2 = keyEventHandler;
                    KeyEventHandler value2 = (KeyEventHandler)Delegate.Remove(keyEventHandler2, value);
                    keyEventHandler = Interlocked.CompareExchange<KeyEventHandler>(ref this._KeyUp, value2, keyEventHandler2);
                }
                while (keyEventHandler != keyEventHandler2);
            }
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00002161 File Offset: 0x00000361
        public void Start()
        {
            this.Start(true, true);
        }

        // Token: 0x06000020 RID: 32 RVA: 0x0000271C File Offset: 0x0000091C
        public void Start(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            if (this.hMouseHook == 0 && InstallMouseHook)
            {
                UserActivityHook.MouseHookProcedure = new UserActivityHook.HookProc(this.MouseHookProc);
                this.hMouseHook = UserActivityHook.SetWindowsHookEx(14, UserActivityHook.MouseHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (this.hMouseHook == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    this.Stop(true, false, false);
                    throw new Win32Exception(lastWin32Error);
                }
            }
            if (this.hKeyboardHook == 0 && InstallKeyboardHook)
            {
                UserActivityHook.KeyboardHookProcedure = new UserActivityHook.HookProc(this.KeyboardHookProc);
                this.hKeyboardHook = UserActivityHook.SetWindowsHookEx(13, UserActivityHook.KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (this.hKeyboardHook == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    this.Stop(false, true, false);
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0000216D File Offset: 0x0000036D
        public void Stop()
        {
            this.Stop(true, true, true);
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002810 File Offset: 0x00000A10
        public void Stop(bool UninstallMouseHook, bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            if (this.hMouseHook != 0 && UninstallMouseHook)
            {
                int num = UserActivityHook.UnhookWindowsHookEx(this.hMouseHook);
                this.hMouseHook = 0;
                if (num == 0 && ThrowExceptions)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
            if (this.hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                int num2 = UserActivityHook.UnhookWindowsHookEx(this.hKeyboardHook);
                this.hKeyboardHook = 0;
                if (num2 == 0 && ThrowExceptions)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        // Token: 0x06000023 RID: 35 RVA: 0x000028A8 File Offset: 0x00000AA8
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            bool flag = false;
            if (nCode >= 0 && this._OnMouseActivity != null)
            {
                UserActivityHook.MouseLLHookStruct mouseLLHookStruct = (UserActivityHook.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(UserActivityHook.MouseLLHookStruct));
                MouseButtons mouseButtons = MouseButtons.None;
                short delta = 0;
                if (wParam != 513)
                {
                    if (wParam != 516)
                    {
                        if (wParam == 522)
                        {
                            delta = checked((short)(mouseLLHookStruct.mouseData >> 16 & 65535));
                        }
                    }
                    else
                    {
                        this.tnow = DateTime.UtcNow;
                        if ((this.tnow - this.tlast).TotalMilliseconds < 100.0)
                        {
                            flag = true;
                        }
                        this.tlast = this.tnow;
                        mouseButtons = MouseButtons.Right;
                    }
                }
                else
                {
                    this.tnow = DateTime.UtcNow;
                    if ((this.tnow - this.tlast).TotalMilliseconds < 100.0)
                    {
                        flag = true;
                    }
                    this.tlast = this.tnow;
                    mouseButtons = MouseButtons.Left;
                }
                int clicks = 0;
                if (mouseButtons > MouseButtons.None)
                {
                    if (wParam == 515 || wParam == 518)
                    {
                        clicks = 2;
                    }
                    else
                    {
                        clicks = 1;
                    }
                }
                if (flag)
                {
                    clicks = 5;
                }
                MouseEventArgs e = new MouseEventArgs(mouseButtons, clicks, mouseLLHookStruct.pt.x, mouseLLHookStruct.pt.y, (int)delta);
                this._OnMouseActivity(this, e);
            }
            int result;
            if (!flag)
            {
                result = UserActivityHook.CallNextHookEx(this.hMouseHook, nCode, wParam, lParam);
            }
            else
            {
                result = 1;
            }
            return result;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002A2C File Offset: 0x00000C2C
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            bool flag = false;
            if (nCode >= 0 && (this._KeyDown != null || this._KeyUp != null || this._KeyPress != null))
            {
                UserActivityHook.KeyboardHookStruct keyboardHookStruct = (UserActivityHook.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(UserActivityHook.KeyboardHookStruct));
                if (this._KeyDown != null && (wParam == 256 || wParam == 260))
                {
                    Keys vkCode = (Keys)keyboardHookStruct.vkCode;
                    KeyEventArgs keyEventArgs = new KeyEventArgs(vkCode);
                    this._KeyDown(this, keyEventArgs);
                    flag = (flag || keyEventArgs.Handled);
                }
                if (this._KeyPress != null && wParam == 256)
                {
                    bool flag2 = (UserActivityHook.GetKeyState(16) & 128) == 128;
                    bool keyState = UserActivityHook.GetKeyState(20) != 0;
                    byte[] array = new byte[256];
                    UserActivityHook.GetKeyboardState(array);
                    byte[] array2 = new byte[2];
                    if (UserActivityHook.ToAscii(keyboardHookStruct.vkCode, keyboardHookStruct.scanCode, array, array2, keyboardHookStruct.flags) == 1)
                    {
                        char c = (char)array2[0];
                        if ((keyState ^ flag2) && char.IsLetter(c))
                        {
                            c = char.ToUpper(c);
                        }
                        KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs(c);
                        this._KeyPress(this, keyPressEventArgs);
                        flag = (flag || keyPressEventArgs.Handled);
                    }
                }
                if (this._KeyUp != null && (wParam == 257 || wParam == 261))
                {
                    Keys vkCode = (Keys)keyboardHookStruct.vkCode;
                    KeyEventArgs keyEventArgs = new KeyEventArgs(vkCode);
                    this._KeyUp(this, keyEventArgs);
                    flag = (flag || keyEventArgs.Handled);
                }
            }
            int result;
            if (flag)
            {
                result = 1;
            }
            else
            {
                result = UserActivityHook.CallNextHookEx(this.hKeyboardHook, nCode, wParam, lParam);
            }
            return result;
        }

        // Token: 0x04000008 RID: 8
        private const int WH_MOUSE_LL = 14;

        // Token: 0x04000009 RID: 9
        private const int WH_KEYBOARD_LL = 13;

        // Token: 0x0400000A RID: 10
        private const int WH_MOUSE = 7;

        // Token: 0x0400000B RID: 11
        private const int WH_KEYBOARD = 2;

        // Token: 0x0400000C RID: 12
        private const int WM_MOUSEMOVE = 512;

        // Token: 0x0400000D RID: 13
        private const int WM_LBUTTONDOWN = 513;

        // Token: 0x0400000E RID: 14
        private const int WM_RBUTTONDOWN = 516;

        // Token: 0x0400000F RID: 15
        private const int WM_MBUTTONDOWN = 519;

        // Token: 0x04000010 RID: 16
        private const int WM_LBUTTONUP = 514;

        // Token: 0x04000011 RID: 17
        private const int WM_RBUTTONUP = 517;

        // Token: 0x04000012 RID: 18
        private const int WM_MBUTTONUP = 520;

        // Token: 0x04000013 RID: 19
        private const int WM_LBUTTONDBLCLK = 515;

        // Token: 0x04000014 RID: 20
        private const int WM_RBUTTONDBLCLK = 518;

        // Token: 0x04000015 RID: 21
        private const int WM_MBUTTONDBLCLK = 521;

        // Token: 0x04000016 RID: 22
        private const int WM_MOUSEWHEEL = 522;

        // Token: 0x04000017 RID: 23
        private const int WM_KEYDOWN = 256;

        // Token: 0x04000018 RID: 24
        private const int WM_KEYUP = 257;

        // Token: 0x04000019 RID: 25
        private const int WM_SYSKEYDOWN = 260;

        // Token: 0x0400001A RID: 26
        private const int WM_SYSKEYUP = 261;

        // Token: 0x0400001B RID: 27
        private const byte VK_SHIFT = 16;

        // Token: 0x0400001C RID: 28
        private const byte VK_CAPITAL = 20;

        // Token: 0x0400001D RID: 29
        private const byte VK_NUMLOCK = 144;

        // Token: 0x0400001E RID: 30
        private DateTime tnow;

        // Token: 0x0400001F RID: 31
        private DateTime tlast;

        // Token: 0x04000020 RID: 32
        private MouseEventHandler _OnMouseActivity;

        // Token: 0x04000021 RID: 33
        private KeyEventHandler _KeyDown;

        // Token: 0x04000022 RID: 34
        private KeyPressEventHandler _KeyPress;

        // Token: 0x04000023 RID: 35
        private KeyEventHandler _KeyUp;

        // Token: 0x04000024 RID: 36
        private int hMouseHook = 0;

        // Token: 0x04000025 RID: 37
        private int hKeyboardHook = 0;

        // Token: 0x04000026 RID: 38
        private static UserActivityHook.HookProc MouseHookProcedure;

        // Token: 0x04000027 RID: 39
        private static UserActivityHook.HookProc KeyboardHookProcedure;

        // Token: 0x02000004 RID: 4
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            // Token: 0x06000025 RID: 37 RVA: 0x0000217A File Offset: 0x0000037A
            public POINT()
            {
            }

            // Token: 0x04000028 RID: 40
            public int x;

            // Token: 0x04000029 RID: 41
            public int y;
        }

        // Token: 0x02000005 RID: 5
        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            // Token: 0x06000026 RID: 38 RVA: 0x0000217A File Offset: 0x0000037A
            public MouseHookStruct()
            {
            }

            // Token: 0x0400002A RID: 42
            public UserActivityHook.POINT pt;

            // Token: 0x0400002B RID: 43
            public int hwnd;

            // Token: 0x0400002C RID: 44
            public int wHitTestCode;

            // Token: 0x0400002D RID: 45
            public int dwExtraInfo;
        }

        // Token: 0x02000006 RID: 6
        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            // Token: 0x06000027 RID: 39 RVA: 0x0000217A File Offset: 0x0000037A
            public MouseLLHookStruct()
            {
            }

            // Token: 0x0400002E RID: 46
            public UserActivityHook.POINT pt;

            // Token: 0x0400002F RID: 47
            public int mouseData;

            // Token: 0x04000030 RID: 48
            public int flags;

            // Token: 0x04000031 RID: 49
            public int time;

            // Token: 0x04000032 RID: 50
            public int dwExtraInfo;
        }

        // Token: 0x02000007 RID: 7
        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            // Token: 0x06000028 RID: 40 RVA: 0x0000217A File Offset: 0x0000037A
            public KeyboardHookStruct()
            {
            }

            // Token: 0x04000033 RID: 51
            public int vkCode;

            // Token: 0x04000034 RID: 52
            public int scanCode;

            // Token: 0x04000035 RID: 53
            public int flags;

            // Token: 0x04000036 RID: 54
            public int time;

            // Token: 0x04000037 RID: 55
            public int dwExtraInfo;
        }

        // Token: 0x02000008 RID: 8
        // (Invoke) Token: 0x0600002A RID: 42
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
    }
}
