using System.Diagnostics;

namespace PngReader.lib
{
    public static class PNGParser
    {
        public static byte[] GetBytes(byte[] bytes, uint offset, uint size)
        {
            byte[] result = new byte[size];
            Array.Copy(bytes, offset, result, 0, size);
            return result;
        }

        public static uint GetBytesToUint(byte[] bytes, uint offset, uint size)
        {
            uint result = 0;
            for(uint i = offset + size - 1; i > offset; i--)
            {
                if (size + offset - i - 1 > 0)
                    result += (uint)bytes[i] << 8 * (int)(size + offset - i - 1);
                else
                    result += bytes[i];

            }
            return result;
        }

        /// <summary>
        /// Checks if the first 8 bytes are as every PNG should have
        /// </summary>
        /// <param name="bytes">The PNG parsed in bytes</param>
        /// <returns>If it's valid or not</returns>
        public static bool IsValidPNG(byte[] bytes)
        {
            //First bytes that every valid PNG has.
            byte[] firstBytes =
            {
                137,
                80,
                78,
                71,
                13,
                10,
                26,
                10
            };

            for (int i = 0; i < firstBytes.Length; i++)
            {
                if (firstBytes[i] != bytes[i])
                {
                    return false;
                }
            }

            return true;
        }
        public static Chunk GetChunk(byte[] bytes, uint offset)
        {
            UInt32 length = GetBytesToUint(bytes, offset, 4);
            //I have no idea what chunk type is rn so for now it stores only the chunk type bytes.
            byte[] type = GetBytes(bytes, offset + 4, 4);
            byte[] data = GetBytes(bytes, offset + 8, length);
            byte[] crc = GetBytes(bytes, offset + 8 + length, 4);
            return new(offset, length, type, data, crc);
        }

        public static PNGData ReadPNGData(byte[] bytes)
        {
            PNGData pngData = new PNGData();
            List<Chunk> chunks = new List<Chunk>();
            pngData.valid = IsValidPNG(bytes);
            int index = 8;
            //Read chunks until the whole PNG was read
            while(index < bytes.Length)
            {
                var chunk = GetChunk(bytes, (uint)index);
                chunks.Add(chunk);
                index += (int)chunk.length + 12;
            }
            pngData.chunks = chunks.ToArray();
            return pngData;
        }

        public static PNG ReadPNG(byte[] bytes)
        {
            PNG png = new PNG();
            PNGData pngData = ReadPNGData(bytes);
            //IHDR chunk - Image header chunk
            Chunk ihdr = pngData.chunks[0];
            png.width = (int)GetBytesToUint(ihdr.data, 0, 4);
            png.height = (int)GetBytesToUint(ihdr.data, 4, 4);
            png.depth = ihdr.data[8];
            //If color type is 3, next chunk is a PLTE chunk, otherwise it's an IDAT chunk and PLTE is not in the file
            //Currently no support for pallete files, and i don't think i saw pallete pngs.
            png.colType = ihdr.data[9];
            png.comprMethod = ihdr.data[10];
            png.fMethod = ihdr.data[11];
            png.intMethod = ihdr.data[12];
            if (png.colType == 3)
                throw new NotImplementedException("Pallete png files are not supported.");

            throw new NotImplementedException();
        }

        public static PNG ReadPNG(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return ReadPNG(bytes);
        }

        public struct PNG
        {
            //Size
            public int width;
            public int height;
            /// <summary>
            /// Bit depth
            /// </summary>
            public int depth;
            /// <summary>
            /// Color type
            /// </summary>
            public int colType;
            /// <summary>
            /// Compression method
            /// </summary>
            public int comprMethod;
            /// <summary>
            /// Filter method
            /// </summary>
            public int fMethod;
            /// <summary>
            /// Interlace method
            /// </summary>
            public int intMethod;

        }

        public struct PNGData
        {
            public bool valid;
            public Chunk[] chunks;

            public PNGData()
            {
                valid = false;
                chunks = Array.Empty<Chunk>();
            }
        }

        public struct Chunk
        {
            public uint offset;
            public UInt32 length;
            public byte[] type = new byte[4];
            public byte[] data;
            public byte[] crc = new byte[4];

            public Chunk(uint offset, uint length, byte[] type, byte[] data, byte[] crc)
            {
                this.offset = offset;
                this.length = length;
                this.type = type;
                this.data = data;
                this.crc = crc;
            }
        }
    }
}
