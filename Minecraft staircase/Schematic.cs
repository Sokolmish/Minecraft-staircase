using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Minecraft_staircase
{
    class Schematic
    {
        short XDim { get; }
        short ZDim { get; }
        short YDim { get; }

        byte[] _blocksId;
        byte[] _blocksData;

        Stream _stream;
        Encoding encoding;

        /// <summary>
        /// Initialize new schematic
        /// </summary>
        /// <param name="xDim">First horizontal coordinate</param>
        /// <param name="zDim">Second horizontal coordinate</param>
        /// <param name="yDim">Vertical coordinate</param>
        public Schematic(int xDim, int yDim, int zDim)
        {
            if (xDim * yDim * zDim > int.MaxValue)
                throw new Exception($"Too large scheme");
            XDim = (short)xDim;
            YDim = (short)yDim;
            ZDim = (short)zDim;
            _blocksId = new byte[XDim * YDim * ZDim];
            _blocksData = new byte[XDim * YDim * ZDim];
            encoding = Encoding.UTF8;
        }

        public void SetBlock(short X, short Y, short Z, byte ID, byte data)
        {
            _blocksId[GetIndex(X, Y, Z)] = ID;
            _blocksData[GetIndex(X, Y, Z)] = data;
        }

        public void SetBlock(int X, int Y, int Z, byte ID, byte data) => SetBlock((short)X, (short)Y, (short)Z, ID, data);

        int GetIndex(short X, short Y, short Z) => (Y * ZDim + Z) * XDim + X;


        public void WriteToStream(Stream stream)
        {
            _stream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress);
            WriteCompoundHead("Schematic");
            WriteShort("Width", XDim);
            WriteShort("Height", YDim);
            WriteShort("Length", ZDim);
            WriteString("Materials", "Alpha");
            WriteByteArray("Blocks", _blocksId);
            WriteByteArray("Data", _blocksData);
            WriteTag(TagTypes.END);
            _stream.Dispose();
        }


        void WriteCompoundHead(string name)
        {
            WriteTag(TagTypes.COMPOUND);
            WriteStringValue(name);
        }

        void WriteShort(string name, short value)
        {
            WriteTag(TagTypes.SHORT);
            WriteStringValue(name);
            WriteShortValue(value);
        }

        void WriteString(string name, string value)
        {
            WriteTag(TagTypes.STRING);
            WriteStringValue(name);
            WriteStringValue(value);
        }

        void WriteByteArray(string name, byte[] value)
        {
            WriteTag(TagTypes.BYTE_ARRAY);
            WriteStringValue(name);
            WriteByteArrayValue(value);
        }


        void WriteShortValue(short value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buff);
            _stream.Write(buff, 0, 2);
        }

        void WriteStringValue(string value)
        {
            byte[] buff = encoding.GetBytes(value);
            WriteShortValue((short)buff.Length);
            _stream.Write(buff, 0, buff.Length);
        }

        void WriteByteArrayValue(byte[] value)
        {
            byte[] buff = BitConverter.GetBytes(value.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buff);
            _stream.Write(buff, 0, 4);
            _stream.Write(value, 0, value.Length);
        }

        void WriteTag(TagTypes type)
        {
            _stream.WriteByte((byte)type);
        }


        enum TagTypes
        { END = 0, SHORT = 2, BYTE_ARRAY = 7, STRING = 8, COMPOUND = 10 }
    }
}
