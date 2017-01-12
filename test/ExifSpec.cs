using System.IO;
using Xunit;
using Cluj.Exif;

namespace Clug.Exif.UnitTest
{
    public class ExifSpec
    {
        [Fact]
        public void UseExifMetadataReader4()
        {
            using (FileStream stream = new FileStream("./test/p4_exif_header.jpg", FileMode.Open))
            {
                var reader = new PhotoMetadataReader(stream);
                var meta = reader.ParseMetadata();

                Assert.Equal<string>("NIKON CORPORATION", meta.Make);
                Assert.Equal<string>("NIKON D3300", meta.Model);
            }
        }

        [Fact]
        public void UseExifMetadataReader3()
        {
            using (FileStream stream = new FileStream("./test/p3_exif_header.jpg", FileMode.Open))
            {
                var reader = new PhotoMetadataReader(stream);
                var meta = reader.ParseMetadata();

                Assert.Equal<string>("GoPro", meta.Make);
                Assert.Equal<string>("HERO5 Black", meta.Model);
                Assert.Equal<char>('N', meta.GPS.LatRef);
                Assert.Equal<char>('E', meta.GPS.LonRef);
            }
        }

        [Fact]
        public void UseExifMetadataReader2()
        {
            using (FileStream stream = new FileStream("./test/p2_exif_header.jpg", FileMode.Open))
            {
                var reader = new PhotoMetadataReader(stream);
                var meta = reader.ParseMetadata();

                Assert.Equal<string>("Apple", meta.Make);
                Assert.Equal<string>("iPhone SE", meta.Model);
                Assert.Equal<char>('N', meta.GPS.LatRef);
                Assert.Equal<char>('E', meta.GPS.LonRef);
            }
        }

        [Fact]
        public void UseExifMetadataReader1()
        {
            using (FileStream stream = new FileStream("./test/p1_exif_header.jpg", FileMode.Open))
            {
                var reader = new PhotoMetadataReader(stream);
                var meta = reader.ParseMetadata();

                Assert.Equal<string>("NIKON CORPORATION", meta.Make);
                Assert.Equal<string>("NIKON D40", meta.Model);
                Assert.Equal<char>('N', meta.GPS.LatRef);
                Assert.Equal<char>('E', meta.GPS.LonRef);
            }
        }
    }
}