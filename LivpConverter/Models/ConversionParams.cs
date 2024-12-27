using ImageMagick;

namespace LivpConverter.Models
{
    /// <summary>
    /// 转码参数
    /// </summary>
    public struct ConversionParams
    {
        public string InputFolderPath;

        public string OutputFolderPath;

        public MagickFormat Format;

        public uint Quality;
    }
}
