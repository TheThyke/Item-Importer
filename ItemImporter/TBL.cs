namespace ItemImporter
{
    using System;
    using System.IO;
    using System.Text;

    internal class TBL
    {
        private uint[] m_columns;
        private object[,] m_dataObjects;
        private byte m_decode;
        private string m_filename;
        private byte[] m_rawData;
        private BinaryReader m_reader;
        private MemoryStream m_stream;
        private uint m_totalColumns;
        private uint m_totalRows;

        public TBL(string filename, byte decode)
        {
            this.m_filename = filename;
            this.m_decode = decode;
            this.Load();
        }

        private void Decode()
        {
            if (this.m_decode != 0)
            {
                byte[] buffer = new byte[this.m_rawData.Length];
                if (this.m_decode == 1)
                {
                    uint num = 0x816;
                    for (int i = 0; i < this.m_rawData.Length; i++)
                    {
                        byte num3 = this.m_rawData[i];
                        byte num4 = (byte) ((num & 0xff00) >> 8);
                        num = (((((num3 + num) & 0xffff) * 0x6081) & 0xffff) + 0x1608) & 0xffff;
                        buffer[i] = (byte) (num4 ^ num3);
                    }
                }
                else if (this.m_decode == 2)
                {
                    uint num5 = 0x418;
                    for (int j = 0; j < this.m_rawData.Length; j++)
                    {
                        byte num7 = this.m_rawData[j];
                        byte num8 = (byte) (((num5 >> 8) & 0xff) ^ num7);
                        num5 = ((((num7 + num5) & 0xffff) * 0x8041) + 0x1804) & 0xffff;
                        buffer[j] = num8;
                    }
                }
                this.m_rawData = buffer;
            }
        }

        private byte getByte() => 
            this.m_reader.ReadByte();

        public uint getColumn(int i) => 
            this.m_columns[i];

        public uint getColumns() => 
            this.m_totalColumns;

        private float getFloat() => 
            this.m_reader.ReadSingle();

        private short getInt16() => 
            this.m_reader.ReadInt16();

        private int getInt32() => 
            this.m_reader.ReadInt32();

        public uint getRows() => 
            this.m_totalRows;

        private sbyte getSByte() => 
            this.m_reader.ReadSByte();

        private string getString()
        {
            int count = this.getInt32();
            byte[] bytes = this.m_reader.ReadBytes(count);
            return Encoding.ASCII.GetString(bytes);
        }

        private ushort getUInt16() => 
            this.m_reader.ReadUInt16();

        private uint getUInt32() => 
            this.m_reader.ReadUInt32();

        public object getValue(int i, int n) => 
            this.m_dataObjects[i, n];

        private void Load()
        {
            this.m_rawData = File.ReadAllBytes(this.m_filename);
            this.Decode();
            this.m_stream = new MemoryStream(this.m_rawData);
            this.m_reader = new BinaryReader(this.m_stream);
            this.m_totalColumns = this.getUInt32();
            this.m_columns = new uint[this.m_totalColumns];
            for (int i = 0; i < this.m_totalColumns; i++)
            {
                this.m_columns[i] = this.getUInt32();
            }
            this.m_totalRows = this.getUInt32();
            this.m_dataObjects = new object[this.m_totalRows, this.m_totalColumns];
            for (int j = 0; j < this.getRows(); j++)
            {
                for (int k = 0; k < this.m_totalColumns; k++)
                {
                    switch (this.m_columns[k])
                    {
                        case 1:
                            this.m_dataObjects[j, k] = this.getSByte();
                            break;

                        case 2:
                            this.m_dataObjects[j, k] = this.getByte();
                            break;

                        case 3:
                            this.m_dataObjects[j, k] = this.getInt16();
                            break;

                        case 4:
                            this.m_dataObjects[j, k] = this.getUInt16();
                            break;

                        case 5:
                            this.m_dataObjects[j, k] = this.getInt32();
                            break;

                        case 6:
                            this.m_dataObjects[j, k] = this.getUInt32();
                            break;

                        case 7:
                            this.m_dataObjects[j, k] = this.getString();
                            break;

                        case 8:
                            this.m_dataObjects[j, k] = this.getFloat();
                            break;
                    }
                }
            }
        }
    }
}

