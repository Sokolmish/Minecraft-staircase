using System;
using System.Text;
using System.IO;

namespace Minecraft_staircase
{
    class Schematic
    {
        public short XDim { get; }
        public short ZDim { get; }
        public short YDim { get; }

        byte[] _blocksId;
        byte[] _blocksData;

        Stream _stream;
        Encoding _encoding;

        /// <summary>
        /// Initialize new schematic
        /// </summary>
        /// <param name="xDim">First horizontal length</param>
        /// <param name="zDim">Second horizontal length</param>
        /// <param name="yDim">Vertical length</param>
        public Schematic(int xDim, int yDim, int zDim)
        {
            if (xDim <= 0 || yDim <= 0 || zDim <= 0)
                throw new ArgumentException("Length in every dimensions must be greater than zero");
            try
            { uint overflow = checked((uint)(xDim * yDim * zDim)); }
            catch (OverflowException)
            { throw new OverflowException($"Too large scheme: {((ulong)(xDim * yDim * zDim))} blocks (maximum - {uint.MaxValue})"); }

            XDim = (short)xDim;
            YDim = (short)yDim;
            ZDim = (short)zDim;
            _blocksId = new byte[XDim * YDim * ZDim];
            _blocksData = new byte[XDim * YDim * ZDim];
            _encoding = Encoding.UTF8;
        }

        public void SetBlock(short X, short Y, short Z, byte ID, byte data)
        {
            _blocksId[GetIndex(X, Y, Z)] = ID;
            _blocksData[GetIndex(X, Y, Z)] = data;
        }

        int GetIndex(short X, short Y, short Z) => (Y * ZDim + Z) * XDim + X;


        public void WriteToStream(Stream stream)
        {
            _stream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress);
            WriteCompoundHead("Schematic");
            WriteShort("Width", XDim);
            WriteShort("Height", YDim);
            WriteShort("Length", ZDim);
            WriteString("Materials", "Alpha");
            WriteLishHead("Entities");
            WriteLishHead("TileEntities");
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

        void WriteLishHead(string name)
        {
            WriteTag(TagTypes.LIST);
            WriteStringValue(name);
            WriteTag(TagTypes.COMPOUND);
            WriteIntValue(0);
        }


        void WriteShortValue(short value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buff);
            _stream.Write(buff, 0, 2);
        }

        void WriteIntValue(int value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buff);
            _stream.Write(buff, 0, 4);
        }

        void WriteStringValue(string value)
        {
            byte[] buff = _encoding.GetBytes(value);
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
        { END = 0, SHORT = 2, BYTE_ARRAY = 7, STRING = 8, LIST = 9, COMPOUND = 10 }
    }
}
