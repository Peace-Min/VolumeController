using System;
using System.Runtime.InteropServices;

namespace VolumeController
{
    public enum BeepType
    {
        LowTone,
        MediumTone,
        HighTone,
        Alert,
        Alarm,
        Error,
        Warning,
        Information,
        Question
    }

    public static class BeepService
    {
        [DllImport("kernel32.dll")]
        private static extern bool Beep(uint dwFreq, uint dwDuration);

        [DllImport("user32.dll")]
        private static extern bool MessageBeep(uint uType);

        // MessageBeep types
        private const uint MB_ICONHAND = 0x00000010;
        private const uint MB_ICONQUESTION = 0x00000020;
        private const uint MB_ICONEXCLAMATION = 0x00000030;
        private const uint MB_ICONASTERISK = 0x00000040;

        private static readonly System.Collections.Concurrent.BlockingCollection<BeepType> _beepQueue = new System.Collections.Concurrent.BlockingCollection<BeepType>();

        static BeepService()
        {
            // Start background consumer thread
            System.Threading.Tasks.Task.Factory.StartNew(ProcessBeepQueue, System.Threading.Tasks.TaskCreationOptions.LongRunning);
        }

        public static void PlayBeep(BeepType beepType = BeepType.Alert)
        {
            _beepQueue.Add(beepType);
        }

        private static void ProcessBeepQueue()
        {
            foreach (var beepType in _beepQueue.GetConsumingEnumerable())
            {
                PlayBeepInternal(beepType);
            }
        }

        private static void PlayBeepInternal(BeepType beepType)
        {
            try
            {
                switch (beepType)
                {
                    case BeepType.LowTone:
                        // Low frequency beep (300 Hz, 200ms)
                        Beep(300, 200);
                        break;
                    case BeepType.MediumTone:
                        // Medium frequency beep (800 Hz, 200ms)
                        Beep(800, 200);
                        break;
                    case BeepType.HighTone:
                        // High frequency beep (1500 Hz, 1000ms)
                        Beep(1500, 1000);
                        break;
                    case BeepType.Alert:
                        // Alert sound: two quick beeps (1000 Hz)
                        Beep(1000, 150);
                        System.Threading.Thread.Sleep(50);
                        Beep(1000, 150);
                        break;
                    case BeepType.Alarm:
                        // Alarm sound: alternating high-low beeps
                        Beep(1200, 100);
                        System.Threading.Thread.Sleep(50);
                        Beep(800, 100);
                        System.Threading.Thread.Sleep(50);
                        Beep(1200, 100);
                        break;
                    case BeepType.Error:
                        // System error beep
                        MessageBeep(MB_ICONHAND);
                        break;
                    case BeepType.Warning:
                        // System warning beep
                        MessageBeep(MB_ICONEXCLAMATION);
                        break;
                    case BeepType.Information:
                        // System information beep
                        MessageBeep(MB_ICONASTERISK);
                        break;
                    case BeepType.Question:
                        // Question sound: rising tone beeps
                        Beep(600, 100);
                        System.Threading.Thread.Sleep(30);
                        Beep(800, 100);
                        System.Threading.Thread.Sleep(30);
                        Beep(1000, 100);
                        break;
                }
            }
            catch (DllNotFoundException ex)
            {
                // DLL을 찾을 수 없는 경우
                System.Windows.MessageBox.Show($"BeepService DLL 오류: {ex.Message}");
            }
            catch (EntryPointNotFoundException ex)
            {
                // API 함수를 찾을 수 없는 경우
                System.Windows.MessageBox.Show($"BeepService API 오류: {ex.Message}");
            }
        }
    }
}
