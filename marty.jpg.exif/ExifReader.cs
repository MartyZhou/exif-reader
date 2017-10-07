using System;
using System.Globalization;
using System.IO;
using System.Text;
using Marty.JPG.EXIF.Common;

namespace Marty.JPG.EXIF
{
    public class ExifReader
    {
        const short APP1_HEADER_LENGTH = 8;
        readonly Stream stream;
        short APP1_START_POSITION = 12;
        Endian endian = Endian.Little;

        public ExifReader(Stream stream)
        {
            this.stream = stream;
        }

        public ExifMetadata Read()
        {
            var format = ParseFormat();

            if (format == MetadataFormat.EXIF)
            {
                APP1_START_POSITION = 12;
            }
            else if (format == MetadataFormat.JFIF)
            {
                APP1_START_POSITION = 30;
            }

            ParseEndian();

            var meta = new ExifMetadata();

			stream.Position = APP1_START_POSITION + APP1_HEADER_LENGTH;

			var count = Convert2BytesToShort(ReadBytes(2));
			for (ushort i = 0; i < count; i++)
			{
				var tagId = Convert2BytesToTag(ReadBytes(2));
				var tagType = ParseTagType(ReadBytes(2));
				var length = Convert4BytesToInt(ReadBytes(4));
				var valueBuffer = ReadBytes(4);

				if (tagId == TagId.Make)
				{
					meta.Make = GetStringRefValue(valueBuffer, length);
				}
				else if (tagId == TagId.Model)
				{
					meta.Model = GetStringRefValue(valueBuffer, length);
				}
				else if (tagId == TagId.Date)
				{
					meta.TakenDate = GetDateRefValue(valueBuffer, length);
				}
				else if (tagId == TagId.GPSOffset)
				{
					var offset = Convert4BytesToInt(valueBuffer);
					var position = stream.Position;
                    stream.Position = offset + APP1_START_POSITION;
                    var gpsCount = Convert2BytesToUShort(ReadBytes(2));

					for (ushort j = 0; j < gpsCount; j++)
					{
						var gpsTagId = Convert2BytesToTag(ReadBytes(2));
						var gpsTagType = ParseTagType(ReadBytes(2));
						var gpsLength = Convert4BytesToInt(ReadBytes(4));
						var gpsValueBuffer = ReadBytes(4);

                        switch (gpsTagId)
                        {
                            case TagId.GPSLatRef:
                                meta.LatRef = Convert.ToChar(gpsValueBuffer[0]);
                                break;
                            case TagId.GPSLonRef:
                                meta.LonRef = Convert.ToChar(gpsValueBuffer[0]);
                                break;
                            case TagId.GPSLat:
                                meta.Lat = ReadGPSValue(Convert4BytesToInt(gpsValueBuffer));
                                break;
                            case TagId.GPSLon:
                                meta.Lon = ReadGPSValue(Convert4BytesToInt(gpsValueBuffer));
                                break;
                        }
                    }

                    stream.Position = position;
				}
			}

            return meta;
        }

        void ParseEndian()
        {
            stream.Position = APP1_START_POSITION;
            var endianValue = BitConverter.ToUInt16(ReadBytes(2), 0);
            if (endianValue == 0x4D4D)
            {
                endian = Endian.Big;
            }
        }

        MetadataFormat ParseFormat()
        {
            var format = MetadataFormat.EXIF;
            stream.Position = 2;

            var app1Maker = BitConverter.ToUInt16(ReadBytes(2), 0);
            if (app1Maker == 0xe0ff)
            {
                format = MetadataFormat.JFIF;
            }

            return format;
        }

        byte[] ReadBytes(int length)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        string ReadString(long offset, int length)
        {
            var position = stream.Position;
            stream.Position = offset;
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            stream.Position = position;
            return ConvertToString(buffer);
        }

        DateTime ReadDate(long offset, int length)
        {
            var position = stream.Position;
            stream.Position = offset;
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            stream.Position = position;
            return ConvertToDate(buffer);
        }

        string GetStringRefValue(byte[] raw, int length)
        {
            var offset = Convert4BytesToInt(raw) + APP1_START_POSITION;
            return ReadString(offset, length);
        }

        DateTime GetDateRefValue(byte[] raw, int length)
        {
            var offset = Convert4BytesToInt(raw) + APP1_START_POSITION;
            return ReadDate(offset, length);
        }

        int Convert4BytesToInt(byte[] raw)
        {
            if (endian == Endian.Big)
            {
                Array.Reverse(raw);
            }
            return BitConverter.ToInt32(raw, 0);
        }

        short Convert2BytesToShort(byte[] raw)
        {
            if (endian == Endian.Big)
            {
                Array.Reverse(raw);
            }
            return BitConverter.ToInt16(raw, 0);
        }

        ushort Convert2BytesToUShort(byte[] raw)
        {
            if (endian == Endian.Big)
            {
                Array.Reverse(raw);
            }
            return BitConverter.ToUInt16(raw, 0);
        }

        TagId Convert2BytesToTag(byte[] raw)
        {
            return (TagId)Convert2BytesToUShort(raw);
        }

        string ConvertToString(byte[] raw)
        {
            var copy = new byte[raw.Length - 1];
            Array.Copy(raw, copy, raw.Length - 1);
            return Encoding.UTF8.GetString(copy);
        }

        DateTime ConvertToDate(byte[] raw)
        {
            var dateString = ConvertToString(raw);

            if (!DateTime.TryParseExact(dateString, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
            {
                result = DateTime.MinValue;
            }

            return result;
        }

        TagType ParseTagType(byte[] raw)
        {
            return (TagType)Convert.ToInt16(raw[1]);
        }

        float ReadGPSValue(long offset)
        {
            var position = stream.Position;
            stream.Position = offset + APP1_START_POSITION;
            var buffer = ReadBytes(24);
            stream.Position = position;
            return ParseGPSValue(buffer);
        }

        float ParseGPSValue(byte[] raw)
        {
            byte[] degreeNumeratorBuffer = { raw[0], raw[1], raw[2], raw[3] };
            var degreeNumerator = Convert4BytesToInt(degreeNumeratorBuffer);
            byte[] degreeDenumeratorBuffer = { raw[4], raw[5], raw[6], raw[7] };
            var degreeDenominator = Convert4BytesToInt(degreeDenumeratorBuffer);
            var degree = degreeNumerator / degreeDenominator;

            byte[] minuteNumeratorBuffer = { raw[8], raw[9], raw[10], raw[11] };
            var minuteNumerator = Convert4BytesToInt(minuteNumeratorBuffer);
            byte[] minuteDenumeratorBuffer = { raw[12], raw[13], raw[14], raw[15] };
            var minuteDenominator = Convert4BytesToInt(minuteDenumeratorBuffer);
            var minute = minuteNumerator / minuteDenominator;

            byte[] secondNumeratorBuffer = { raw[16], raw[17], raw[18], raw[19] };
            var secondNumerator = Convert4BytesToInt(secondNumeratorBuffer);
            byte[] secondDenumeratorBuffer = { raw[20], raw[21], raw[22], raw[23] };
            var secondDenominator = Convert4BytesToInt(secondDenumeratorBuffer);
            var second = secondNumerator / secondDenominator;

            return degree + (float)minute / 60 + (float)second / 3600;
        }
    }
}
