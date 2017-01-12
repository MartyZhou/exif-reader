using System;

namespace Cluj.Exif
{
    public class PhotoMetadata
    {
        public Endian Endian { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public DateTime TakenDate { get; set; }
        public bool HasLocation { get; set; }
        public GPSInfo GPS { get; set; }
        public string FilePath { get; set; }
    }
}