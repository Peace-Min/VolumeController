using System;
using System.Runtime.InteropServices;

namespace VolumeController
{
    public static class VolumeService
    {
        private static IAudioEndpointVolume _audioEndpointVolume;
        private static IMMDeviceEnumerator _deviceEnumerator;
        private static IMMDevice _device;

        static VolumeService()
        {
            InitializeAudioDevice();
        }



        private static void InitializeAudioDevice()
        {
            try
            {
                _deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                int hr = _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out _device);
                if (hr != 0) throw new MarshalDirectiveException($"GetDefaultAudioEndpoint failed with HRESULT: {hr}");

                Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;
                object o;
                // CLSCTX_ALL = 23 (0x17)
                hr = _device.Activate(ref IID_IAudioEndpointVolume, 23, IntPtr.Zero, out o);
                if (hr != 0) throw new MarshalDirectiveException($"Activate failed with HRESULT: {hr}");
                
                _audioEndpointVolume = (IAudioEndpointVolume)o;
            }
            catch (MarshalDirectiveException ex)
            {
                System.Windows.MessageBox.Show($"VolumeService COM 오류: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
            catch (InvalidCastException ex)
            {
                System.Windows.MessageBox.Show($"VolumeService 형변환 오류: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
            catch (COMException ex)
            {
                System.Windows.MessageBox.Show($"VolumeService COM 예외: {ex.Message}\n\nHRESULT: 0x{ex.ErrorCode:X}\n\nStack Trace:\n{ex.StackTrace}");
            }
        }

        public static float GetVolume()
        {
            if (_audioEndpointVolume == null) return 0;
            float currentVolume = 0;
            _audioEndpointVolume.GetMasterVolumeLevelScalar(out currentVolume);
            return currentVolume * 100;
        }

        public static void SetVolume(float percentage)
        {
            if (_audioEndpointVolume == null) return;
            Guid guid = Guid.Empty;
            // Ensure percentage is between 0 and 100
            float scalar = Math.Max(0, Math.Min(percentage, 100)) / 100.0f;
            _audioEndpointVolume.SetMasterVolumeLevelScalar(scalar, ref guid);
        }

        public static bool GetMute()
        {
            if (_audioEndpointVolume == null) return false;
            bool isMuted = false;
            _audioEndpointVolume.GetMute(out isMuted);
            return isMuted;
        }

        public static void SetMute(bool isMuted)
        {
            if (_audioEndpointVolume == null) return;
            Guid guid = Guid.Empty;
            _audioEndpointVolume.SetMute(isMuted, ref guid);
        }
    }
}
