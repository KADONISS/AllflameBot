using System;
using System.Drawing;
using System.Runtime.InteropServices;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Game;

namespace AllflameBot
{
    internal sealed class UiWindowStamp
    {
        public IntPtr Window;
        public int Left;
        public int Top;
        public int Width;
        public int Height;
        public uint Dpi;

        public bool MatchesCurrent()
        {
            UiWindowStamp current;
            return BackgroundUiInput.TryGetWindowStamp(out current) &&
                   current.Window == Window &&
                   current.Left == Left &&
                   current.Top == Top &&
                   current.Width == Width &&
                   current.Height == Height &&
                   current.Dpi == Dpi;
        }
    }

    internal sealed class UiInputResult
    {
        public bool InputSent;
        public string Mode;
        public IntPtr Window;
        public Vector2i Client;
        public Vector2i Screen;
        public bool Foreground;
        public bool Uncovered;

        public static UiInputResult Failed(
            IntPtr window,
            Vector2i client,
            string mode)
        {
            return new UiInputResult
            {
                InputSent = false,
                Mode = mode ?? "Failed",
                Window = window,
                Client = client,
                Screen = new Vector2i(),
                Foreground = false,
                Uncovered = false
            };
        }
    }

    /// <summary>
    /// Routes UI input to the game window without requiring it to be the
    /// foreground window. Physical mouse input remains a guarded fallback.
    /// </summary>
    internal static class BackgroundUiInput
    {
        private const uint WmMouseMove = 0x0200;
        private const uint WmLButtonDown = 0x0201;
        private const uint WmLButtonUp = 0x0202;
        private const uint WmLButtonDblClk = 0x0203;
        private const uint WmRButtonDown = 0x0204;
        private const uint WmRButtonUp = 0x0205;
        private const uint WmRButtonDblClk = 0x0206;
        private const uint WmKeyDown = 0x0100;
        private const uint WmKeyUp = 0x0101;
        private const uint WmChar = 0x0102;
        private const uint VkControl = 0x0011;
        private const uint VkShift = 0x0010;
        private const uint MkLButton = 0x0001;
        private const uint MkRButton = 0x0002;
        private const uint MkShift = 0x0004;
        private const uint MkControl = 0x0008;
        private const uint GaRoot = 2;
        private const uint PrintWindowClientOnly = 0x00000001;
        private const uint PrintWindowRenderFullContent = 0x00000002;

        public static UiInputResult ClickClient(
            Vector2i client,
            string action)
        {
            return SendClick(
                client,
                action,
                false,
                false,
                false,
                false);
        }

        public static UiInputResult ClickScreen(
            Vector2i screen,
            string action)
        {
            return SendScreenClick(
                screen,
                action,
                false,
                false,
                false,
                false);
        }

        public static UiInputResult CtrlClickClient(
            Vector2i client,
            string action)
        {
            return SendClick(
                client,
                action,
                true,
                false,
                false,
                false);
        }

        public static UiInputResult CtrlClickScreen(
            Vector2i screen,
            string action)
        {
            return SendScreenClick(
                screen,
                action,
                true,
                false,
                false,
                false);
        }

        public static UiInputResult CtrlShiftClickClient(
            Vector2i client,
            string action)
        {
            return SendClick(
                client,
                action,
                true,
                true,
                false,
                false);
        }

        public static UiInputResult CtrlShiftClickScreen(
            Vector2i screen,
            string action)
        {
            return SendScreenClick(
                screen,
                action,
                true,
                true,
                false,
                false);
        }

        public static UiInputResult RightClickClient(
            Vector2i client,
            string action,
            bool control)
        {
            return SendClick(
                client,
                action,
                control,
                false,
                true,
                false);
        }

        public static UiInputResult RightClickScreen(
            Vector2i screen,
            string action,
            bool control)
        {
            return SendScreenClick(
                screen,
                action,
                control,
                false,
                true,
                false);
        }

        public static UiInputResult DoubleClickClient(
            Vector2i client,
            string action)
        {
            return SendClick(
                client,
                action,
                false,
                false,
                false,
                true);
        }

        public static UiInputResult DoubleClickScreen(
            Vector2i screen,
            string action)
        {
            return SendScreenClick(
                screen,
                action,
                false,
                false,
                false,
                true);
        }

        public static bool TryGetClientLocation(
            Vector2i screen,
            out Vector2i client)
        {
            client = new Vector2i();
            UiWindowStamp stamp;
            if (!TryGetWindowStamp(out stamp))
            {
                return false;
            }

            client = new Vector2i(
                screen.X - stamp.Left,
                screen.Y - stamp.Top);
            return IsClientPointValid(stamp, client);
        }

        public static bool PressEnter(string action)
        {
            return PressKey(
                System.Windows.Forms.Keys.Enter,
                action);
        }

        public static bool PressKey(
            System.Windows.Forms.Keys key,
            string action)
        {
            UiWindowStamp stamp;
            if (!TryGetWindowStamp(out stamp))
            {
                LogKey(
                    action,
                    (uint)key,
                    "WindowUnavailable",
                    false,
                    IntPtr.Zero);
                return false;
            }

            if (IsForegroundWindow(stamp.Window))
            {
                try
                {
                    LokiPoe.ProcessHookManager.ClearAllKeyStates();
                    LokiPoe.Input.SimulateKeyEvent(
                        key,
                        true,
                        false,
                        true);
                    LogKey(
                        action,
                        (uint)key,
                        "PhysicalForeground",
                        true,
                        stamp.Window);
                    return true;
                }
                catch
                {
                }
                finally
                {
                    LokiPoe.ProcessHookManager.ClearAllKeyStates();
                }
            }

            return PostKey(
                (uint)key,
                action);
        }

        public static bool TypeCharacter(
            char character,
            string action)
        {
            UiWindowStamp stamp;
            if (!TryGetWindowStamp(out stamp))
            {
                LogKey(
                    action,
                    character,
                    "WindowUnavailable",
                    false,
                    IntPtr.Zero);
                return false;
            }

            if (IsForegroundWindow(stamp.Window))
            {
                try
                {
                    LokiPoe.Input.SimulateKeyEvent(character);
                    LogKey(
                        action,
                        character,
                        "PhysicalForeground",
                        true,
                        stamp.Window);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            bool sent = PostMessage(
                stamp.Window,
                WmChar,
                new IntPtr((long)character),
                IntPtr.Zero);
            LogKey(
                action,
                character,
                "BackgroundWindowMessage",
                sent,
                stamp.Window);
            return sent;
        }

        public static bool TryGetWindowStamp(
            out UiWindowStamp stamp)
        {
            stamp = null;
            try
            {
                var client = LokiPoe.ClientWindowInfo.Client;
                int width = client.Right - client.Left;
                int height = client.Bottom - client.Top;
                if (LokiPoe.ClientWindowHandle == IntPtr.Zero ||
                    width <= 0 ||
                    height <= 0)
                {
                    return false;
                }

                stamp = new UiWindowStamp
                {
                    Window = LokiPoe.ClientWindowHandle,
                    Left = client.Left,
                    Top = client.Top,
                    Width = width,
                    Height = height,
                    Dpi = GetDpiSafe(LokiPoe.ClientWindowHandle)
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryCaptureClient(
            out Bitmap screenshot,
            out string captureMode)
        {
            screenshot = null;
            captureMode = "Unavailable";
            try
            {
                UiWindowStamp stamp;
                if (!TryGetWindowStamp(out stamp))
                {
                    captureMode = "WindowUnavailable";
                    return false;
                }

                if (IsClientRegionUncovered(stamp))
                {
                    Bitmap desktop;
                    if (TryCopyFromScreen(stamp, out desktop))
                    {
                        screenshot = desktop;
                        captureMode = "DesktopVisible";
                        return true;
                    }
                }

                Bitmap offscreen = new Bitmap(
                    stamp.Width,
                    stamp.Height,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                if (TryPrintWindow(
                        stamp.Window,
                        offscreen,
                        PrintWindowClientOnly |
                        PrintWindowRenderFullContent))
                {
                    screenshot = offscreen;
                    captureMode = "PrintWindowClient";
                    return true;
                }
                offscreen.Dispose();

                if (!IsClientRegionUncovered(stamp))
                {
                    captureMode = "OccludedCaptureUnavailable";
                    LogCaptureFailure(captureMode, stamp);
                    return false;
                }

                Bitmap visibleFallback;
                if (TryCopyFromScreen(stamp, out visibleFallback))
                {
                    screenshot = visibleFallback;
                    captureMode = "DesktopVisibleFallback";
                    return true;
                }

                captureMode = "DesktopCaptureFailed";
                LogCaptureFailure(captureMode, stamp);
                return false;
            }
            catch (Exception exception)
            {
                captureMode = "CaptureError:" +
                    exception.GetType().Name;
                LogCaptureFailure(captureMode, null);
                return false;
            }
        }

        private static UiInputResult SendClick(
            Vector2i client,
            string action,
            bool control,
            bool shift,
            bool rightButton,
            bool doubleClick,
            UiWindowStamp knownStamp = null)
        {
            IntPtr window = IntPtr.Zero;
            UiWindowStamp stamp;
            try
            {
                window = LokiPoe.ClientWindowHandle;
                stamp = knownStamp;
                if ((stamp == null && !TryGetWindowStamp(out stamp)) ||
                    !IsClientPointValid(stamp, client))
                {
                    UiInputResult invalid = UiInputResult.Failed(
                        window,
                        client,
                        "InvalidClientPoint");
                    LogInput(action, invalid);
                    return invalid;
                }

                Vector2i screen;
                bool foreground = IsForegroundWindow(stamp.Window);
                bool uncovered = IsClientPointUncovered(stamp, client);
                if (foreground &&
                    uncovered &&
                    TryClientToScreen(stamp, client, out screen))
                {
                    UiInputResult physical = SendPhysicalClick(
                        stamp,
                        screen,
                        client,
                        control,
                        shift,
                        rightButton,
                        doubleClick);
                    LogInput(action, physical);
                    if (physical.InputSent)
                    {
                        return physical;
                    }
                }

                if (PostClientClick(
                        stamp.Window,
                        client,
                        control,
                        shift,
                        rightButton,
                        doubleClick))
                {
                    UiInputResult background = new UiInputResult
                    {
                        InputSent = true,
                        Mode = "BackgroundWindowMessage",
                        Window = stamp.Window,
                        Client = client,
                        Screen = new Vector2i(
                            stamp.Left + client.X,
                            stamp.Top + client.Y),
                        Foreground = foreground,
                        Uncovered = uncovered
                    };
                    LogInput(action, background);
                    return background;
                }

                UiInputResult unavailable = UiInputResult.Failed(
                    stamp.Window,
                    client,
                    foreground
                        ? "InputDispatchFailed"
                        : "BackgroundInputUnavailable");
                unavailable.Screen = new Vector2i(
                    stamp.Left + client.X,
                    stamp.Top + client.Y);
                unavailable.Foreground = foreground;
                unavailable.Uncovered = uncovered;
                LogInput(action, unavailable);
                return unavailable;
            }
            catch (Exception exception)
            {
                UiInputResult failed = UiInputResult.Failed(
                    window,
                    client,
                    "InputError:" + exception.GetType().Name);
                LogInput(action, failed);
                return failed;
            }
        }

        private static UiInputResult SendPhysicalClick(
            UiWindowStamp stamp,
            Vector2i screen,
            Vector2i client,
            bool control,
            bool shift,
            bool rightButton,
            bool doubleClick)
        {
            bool controlDown = false;
            bool shiftDown = false;
            try
            {
                LokiPoe.ProcessHookManager.ClearAllKeyStates();
                if (control)
                {
                    LokiPoe.ProcessHookManager.SetKeyState(
                        System.Windows.Forms.Keys.ControlKey,
                        short.MinValue,
                        System.Windows.Forms.Keys.None);
                    controlDown = true;
                }
                if (shift)
                {
                        LokiPoe.ProcessHookManager.SetKeyState(
                            System.Windows.Forms.Keys.ShiftKey,
                        short.MinValue,
                        System.Windows.Forms.Keys.None);
                    shiftDown = true;
                }
                DreamPoeBot.BotFramework.MouseManager
                    .SetMousePosition(screen, false);
                if (rightButton)
                {
                    if (doubleClick)
                    {
                        DreamPoeBot.BotFramework.MouseManager
                            .DblClickRMB(screen.X, screen.Y);
                    }
                    else
                    {
                        DreamPoeBot.BotFramework.MouseManager
                            .ClickRMB(screen.X, screen.Y);
                    }
                }
                else if (doubleClick)
                {
                    DreamPoeBot.BotFramework.MouseManager
                        .DblClickLMB(screen.X, screen.Y);
                }
                else
                {
                    DreamPoeBot.BotFramework.MouseManager
                        .ClickLMB(screen.X, screen.Y);
                }
                return new UiInputResult
                {
                    InputSent = true,
                    Mode = "PhysicalScreenFallback",
                    Window = stamp.Window,
                    Client = client,
                    Screen = screen,
                    Foreground = true,
                    Uncovered = true
                };
            }
            finally
            {
                if (shiftDown)
                {
                    LokiPoe.ProcessHookManager.SetKeyState(
                        System.Windows.Forms.Keys.ShiftKey,
                        (short)0,
                        System.Windows.Forms.Keys.None);
                }
                if (controlDown)
                {
                    LokiPoe.ProcessHookManager.SetKeyState(
                        System.Windows.Forms.Keys.ControlKey,
                        (short)0,
                        System.Windows.Forms.Keys.None);
                }
                LokiPoe.ProcessHookManager.ClearAllKeyStates();
            }
        }

        private static bool PostClientClick(
            IntPtr window,
            Vector2i client,
            bool control,
            bool shift,
            bool rightButton,
            bool doubleClick)
        {
            IntPtr coordinates = PackClientCoordinates(client);
            uint modifiers = 0;
            if (control)
            {
                modifiers |= MkControl;
            }
            if (shift)
            {
                modifiers |= MkShift;
            }

            bool sent = true;
            if (control)
            {
                sent = PostMessage(
                    window,
                    WmKeyDown,
                    new IntPtr((long)VkControl),
                    new IntPtr(0x001D0001)) && sent;
            }
            if (shift)
            {
                sent = PostMessage(
                    window,
                    WmKeyDown,
                    new IntPtr((long)VkShift),
                    new IntPtr(0x002A0001)) && sent;
            }

            bool moved = PostMessage(
                window,
                WmMouseMove,
                new IntPtr((long)modifiers),
                coordinates) && sent;
            uint down = rightButton
                ? WmRButtonDown
                : WmLButtonDown;
            uint up = rightButton
                ? WmRButtonUp
                : WmLButtonUp;
            uint dbl = rightButton
                ? WmRButtonDblClk
                : WmLButtonDblClk;
            uint buttonFlag = rightButton
                ? MkRButton
                : MkLButton;
            IntPtr downState = new IntPtr(
                (long)(modifiers | buttonFlag));
            IntPtr upState = new IntPtr((long)modifiers);
            bool sentDown = PostMessage(
                window,
                down,
                downState,
                coordinates) && sent;
            bool sentUp = PostMessage(
                window,
                up,
                upState,
                coordinates) && sentDown;
            bool sentDouble = true;
            if (doubleClick)
            {
                sentDouble = PostMessage(
                        window,
                        dbl,
                        downState,
                        coordinates) &&
                    PostMessage(
                        window,
                        up,
                        upState,
                        coordinates) &&
                    sentUp;
            }

            if (shift)
            {
                sent = PostMessage(
                    window,
                    WmKeyUp,
                    new IntPtr((long)VkShift),
                    new IntPtr(unchecked((long)0xC02A0001))) &&
                    sent;
            }
            if (control)
            {
                sent = PostMessage(
                    window,
                    WmKeyUp,
                    new IntPtr((long)VkControl),
                    new IntPtr(unchecked((long)0xC01D0001))) &&
                    sent;
            }
            return moved && sentDown && sentUp && sentDouble && sent;
        }

        private static UiInputResult SendScreenClick(
            Vector2i screen,
            string action,
            bool control,
            bool shift,
            bool rightButton,
            bool doubleClick)
        {
            UiWindowStamp stamp;
            if (!TryGetWindowStamp(out stamp))
            {
                UiInputResult unavailable = UiInputResult.Failed(
                    LokiPoe.ClientWindowHandle,
                    new Vector2i(),
                    "WindowUnavailable");
                unavailable.Screen = screen;
                LogInput(action, unavailable);
                return unavailable;
            }

            Vector2i client = new Vector2i(
                screen.X - stamp.Left,
                screen.Y - stamp.Top);
            if (!IsClientPointValid(stamp, client))
            {
                UiInputResult invalid = UiInputResult.Failed(
                    stamp.Window,
                    client,
                    "InvalidScreenPoint");
                invalid.Screen = screen;
                LogInput(action, invalid);
                return invalid;
            }

            return SendClick(
                client,
                action,
                control,
                shift,
                rightButton,
                doubleClick,
                stamp);
        }

        private static bool PostKey(
            uint virtualKey,
            string action)
        {
            UiWindowStamp stamp;
            bool sent = TryGetWindowStamp(out stamp) &&
                PostMessage(
                    stamp.Window,
                    WmKeyDown,
                    new IntPtr((long)virtualKey),
                    IntPtr.Zero) &&
                PostMessage(
                    stamp.Window,
                    WmKeyUp,
                    new IntPtr((long)virtualKey),
                    IntPtr.Zero);
            LogKey(
                action,
                virtualKey,
                "BackgroundWindowMessage",
                sent,
                stamp == null ? IntPtr.Zero : stamp.Window);
            return sent;
        }

        private static void LogKey(
            string action,
            uint virtualKey,
            string mode,
            bool inputSent,
            IntPtr window)
        {
            AllflameLog.Info(
                "UI_KEY" +
                "|Action=" + (action ?? String.Empty) +
                "|VirtualKey=0x" + virtualKey.ToString("X") +
                "|Mode=" + (mode ?? String.Empty) +
                "|InputSent=" + inputSent +
                "|Postcondition=Pending" +
                "|Hwnd=0x" + window.ToString("X"));
        }

        private static bool TryCopyFromScreen(
            UiWindowStamp stamp,
            out Bitmap screenshot)
        {
            screenshot = null;
            Bitmap desktop = null;
            try
            {
                desktop = new Bitmap(
                    stamp.Width,
                    stamp.Height,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(desktop))
                {
                    graphics.CopyFromScreen(
                        stamp.Left,
                        stamp.Top,
                        0,
                        0,
                        new Size(stamp.Width, stamp.Height),
                        CopyPixelOperation.SourceCopy);
                }
                screenshot = desktop;
                return true;
            }
            catch
            {
                if (desktop != null)
                {
                    desktop.Dispose();
                }
                return false;
            }
        }

        private static bool TryPrintWindow(
            IntPtr window,
            Bitmap bitmap,
            uint flags)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                IntPtr hdc = graphics.GetHdc();
                try
                {
                    return PrintWindow(window, hdc, flags);
                }
                finally
                {
                    graphics.ReleaseHdc(hdc);
                }
            }
        }

        private static bool TryClientToScreen(
            UiWindowStamp stamp,
            Vector2i client,
            out Vector2i screen)
        {
            screen = new Vector2i(
                stamp.Left + client.X,
                stamp.Top + client.Y);
            return IsClientPointValid(stamp, client);
        }

        private static bool IsClientPointValid(
            UiWindowStamp stamp,
            Vector2i client)
        {
            return stamp != null &&
                   client.X >= 0 &&
                   client.Y >= 0 &&
                   client.X < stamp.Width &&
                   client.Y < stamp.Height;
        }

        private static bool IsClientPointUncovered(
            UiWindowStamp stamp,
            Vector2i client)
        {
            if (stamp == null ||
                !IsClientPointValid(stamp, client))
            {
                return false;
            }
            if (!IsForegroundWindow(stamp.Window))
            {
                return false;
            }
            POINT point = new POINT
            {
                X = stamp.Left + client.X,
                Y = stamp.Top + client.Y
            };
            IntPtr pointWindow = WindowFromPoint(point);
            return GetAncestor(pointWindow, GaRoot) ==
                GetAncestor(stamp.Window, GaRoot);
        }

        private static bool IsClientRegionUncovered(
            UiWindowStamp stamp)
        {
            if (stamp == null ||
                !IsForegroundWindow(stamp.Window) ||
                stamp.Width < 3 ||
                stamp.Height < 3)
            {
                return false;
            }

            for (int yIndex = 0; yIndex < 5; yIndex++)
            {
                int y = yIndex == 0
                    ? 1
                    : (yIndex == 4
                        ? stamp.Height - 2
                        : stamp.Height * yIndex / 4);
                for (int xIndex = 0; xIndex < 5; xIndex++)
                {
                    int x = xIndex == 0
                        ? 1
                        : (xIndex == 4
                            ? stamp.Width - 2
                            : stamp.Width * xIndex / 4);
                    if (!IsClientPointUncovered(
                            stamp,
                            new Vector2i(x, y)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsForegroundWindow(IntPtr window)
        {
            if (window == IntPtr.Zero)
            {
                return false;
            }
            return GetAncestor(GetForegroundWindow(), GaRoot) ==
                GetAncestor(window, GaRoot);
        }

        private static IntPtr PackClientCoordinates(Vector2i client)
        {
            int packed = (client.Y << 16) |
                (client.X & 0xFFFF);
            return new IntPtr(packed);
        }

        private static uint GetDpiSafe(IntPtr window)
        {
            try
            {
                uint dpi = GetDpiForWindow(window);
                return dpi == 0 ? 96U : dpi;
            }
            catch
            {
                return 96U;
            }
        }

        private static void LogInput(
            string action,
            UiInputResult result)
        {
            AllflameLog.Info(
                "UI_INPUT" +
                "|Action=" + (action ?? String.Empty) +
                "|Mode=" + (result.Mode ?? String.Empty) +
                "|InputSent=" + result.InputSent +
                "|Postcondition=Pending" +
                "|Hwnd=0x" + result.Window.ToString("X") +
                "|Client=" + result.Client +
                "|Screen=" + result.Screen +
                "|Foreground=" + result.Foreground +
                "|Uncovered=" + result.Uncovered);
        }

        private static void LogCaptureFailure(
            string reason,
            UiWindowStamp stamp)
        {
            AllflameLog.Info(
                "UI_CAPTURE" +
                "|Mode=" + (reason ?? String.Empty) +
                "|Window=" +
                (stamp == null
                    ? "Unavailable"
                    : "0x" + stamp.Window.ToString("X")) +
                "|Client=" +
                (stamp == null
                    ? "Unavailable"
                    : stamp.Width + "x" + stamp.Height) +
                "|Postcondition=Pending");
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(
            IntPtr hWnd,
            uint message,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(
            IntPtr hWnd,
            IntPtr hdcBlt,
            uint nFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetDpiForWindow")]
        private static extern uint GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(
            IntPtr hWnd,
            uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }
    }
}
