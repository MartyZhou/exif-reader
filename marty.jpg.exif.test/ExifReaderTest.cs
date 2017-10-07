using System.IO;
using Xunit;

namespace Marty.JPG.EXIF.Test
{
    public class ExifReaderTest
    {
		[Fact]
		public void UseExifMetadataReader4()
		{
			using (var stream = new FileStream("./Images/p4_exif_header.jpg", FileMode.Open))
			{
				var reader = new ExifReader(stream);
                var meta = reader.Read();

				Assert.Equal<string>("NIKON CORPORATION", meta.Make);
				Assert.Equal<string>("NIKON D3300", meta.Model);
			}
		}

		[Fact]
		public void UseExifMetadataReader3()
		{
			using (FileStream stream = new FileStream("./Images/p3_exif_header.jpg", FileMode.Open))
			{
				var reader = new ExifReader(stream);
				var meta = reader.Read();

				Assert.Equal<string>("GoPro", meta.Make);
				Assert.Equal<string>("HERO5 Black", meta.Model);
				Assert.Equal<char>('N', meta.LatRef);
				Assert.Equal<char>('E', meta.LonRef);
			}
		}

		[Fact]
		public void UseExifMetadataReader2()
		{
			using (FileStream stream = new FileStream("./Images/p2_exif_header.jpg", FileMode.Open))
			{
				var reader = new ExifReader(stream);
				var meta = reader.Read();

				Assert.Equal<string>("Apple", meta.Make);
				Assert.Equal<string>("iPhone SE", meta.Model);
				Assert.Equal<char>('N', meta.LatRef);
				Assert.Equal<char>('E', meta.LonRef);
			}
		}

		[Fact]
		public void UseExifMetadataReader1()
		{
			using (FileStream stream = new FileStream("./Images/p1_exif_header.jpg", FileMode.Open))
			{
				var reader = new ExifReader(stream);
				var meta = reader.Read();

				Assert.Equal<string>("NIKON CORPORATION", meta.Make);
				Assert.Equal<string>("NIKON D40", meta.Model);
				Assert.Equal<char>('N', meta.LatRef);
				Assert.Equal<char>('E', meta.LonRef);
			}
		}
    }
}
