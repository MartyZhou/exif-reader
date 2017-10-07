using System;
namespace Marty.JPG.EXIF.Common
{
    public class ExifMetadata
    {
		public Endian Endian { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public DateTime TakenDate { get; set; }
		public char LatRef { get; set; }
		public double Lat { get; set; }
		public char LonRef { get; set; }
		public double Lon { get; set; }
    }
}
