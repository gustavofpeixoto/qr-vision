using FFMpegCore;

namespace QrVision.Domain
{
    public static class FfmpegSettings
    {
        public static void AddFfmpegGlobalSettings()
        {
            GlobalFFOptions.Configure(options => options.BinaryFolder = "./FFmpeg");
        }
    }
}
