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

        [DllImport("winmm.dll", EntryPoint = "PlaySound", CharSet = CharSet.Auto)]
        private static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);

        // PlaySound flags
        private const uint SND_SYNC = 0x0000;      // 소리가 끝날 때까지 리턴하지 않음 (블로킹)
        private const uint SND_ASYNC = 0x0001;     // 소리 재생 시작 후 즉시 리턴
        private const uint SND_ALIAS = 0x10000;    // 시스템 이벤트 이름 사용

        // 연속된 음표 사이의 간격 (30ms 정도로 아주 짧게 주어 자연스럽게 연결)
        private const int NOTE_SEPARATOR_MS = 30;

        private static readonly System.Collections.Concurrent.BlockingCollection<BeepType> _beepQueue = new System.Collections.Concurrent.BlockingCollection<BeepType>();

        static BeepService()
        {
            // Start background consumer thread
            System.Threading.Tasks.Task.Factory.StartNew(ProcessBeepQueue, System.Threading.Tasks.TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// [외부 API] 비프음을 큐에 추가하고 즉시 리턴 (UI Non-blocking)
        /// 사용법: BeepService.PlayBeep(BeepType.Warning);
        /// </summary>
        public static void PlayBeep(BeepType beepType = BeepType.Alert)
        {
            _beepQueue.Add(beepType);
        }

        private static void ProcessBeepQueue()
        {
            foreach (var beepType in _beepQueue.GetConsumingEnumerable())
            {
                PlayBeepInternal(beepType);
                // 소리 간 아주 미세한 간격 (50ms)
                System.Threading.Thread.Sleep(50);
            }
        }

        private static void PlayBeepInternal(BeepType beepType)
        {
            try
            {
                // PlaySound with SND_SYNC will wait until the sound finishes
                const uint flags = SND_SYNC | SND_ALIAS;

                switch (beepType)
                {
                    case BeepType.LowTone:
                        Beep(300, 200); // Beep is already synchronous
                        break;
                    case BeepType.MediumTone:
                        Beep(800, 200);
                        break;
                    case BeepType.HighTone:
                        Beep(1500, 1000);
                        break;
                    case BeepType.Alert:
                        Beep(1000, 150);
                        System.Threading.Thread.Sleep(NOTE_SEPARATOR_MS);
                        Beep(1000, 150);
                        break;
                    case BeepType.Alarm:
                        Beep(1200, 100);
                        System.Threading.Thread.Sleep(NOTE_SEPARATOR_MS);
                        Beep(800, 100);
                        System.Threading.Thread.Sleep(NOTE_SEPARATOR_MS);
                        Beep(1200, 100);
                        break;
                    
                    // Synchronous System Sounds via PlaySound
                    case BeepType.Error:
                        PlaySound("SystemHand", IntPtr.Zero, flags);
                        break;
                    case BeepType.Warning:
                        PlaySound("SystemExclamation", IntPtr.Zero, flags);
                        break;
                    case BeepType.Information:
                        PlaySound("SystemAsterisk", IntPtr.Zero, flags);
                        break;
                        
                    case BeepType.Question:
                        Beep(600, 100);
                        System.Threading.Thread.Sleep(NOTE_SEPARATOR_MS);
                        Beep(800, 100);
                        System.Threading.Thread.Sleep(NOTE_SEPARATOR_MS);
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
