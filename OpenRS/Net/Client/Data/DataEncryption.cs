namespace OpenRS.Net.Client.Data
{

//    public class DataEncryption
//    {

//        public DataEncryption(sbyte[] arg0)
//        {
//            data = arg0;
//            offset = 0;
//        }

//        public virtual void AddByte(int arg0)
//        {
//            data[offset++] = (sbyte) arg0;
//        }

//        public virtual void AddInt(int arg0)
//        {
//            data[offset++] = (sbyte)(arg0 >> 24);
//            data[offset++] = (sbyte)(arg0 >> 16);
//            data[offset++] = (sbyte)(arg0 >> 8);
//            data[offset++] = (sbyte) arg0;
//        }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("deprecation") public void AddString(String arg0)
//        //public virtual void AddString(String arg0)
//        //{
//        //    arg0.GetBytes(0, arg0.Length, data, offset);
//        //    offset += arg0.Length;
//        //    data[offset++] = 10;
//        //}

//        public void AddString(String arg0)
//        {

//            //arg0.GetBytes(0, arg0.Length, data, offset);

//            var bytes = Encoding.UTF8.GetBytes(arg0);

//            Array.Copy(bytes, 0, data, offset, bytes.Length);

//            offset += bytes.Length;

//            data[offset++] = 10;
//        }

//        public virtual int GetByte()
//        {
//        //	get
//            {
//                return data[offset++] & 0xff;
//            }
//        }

//        public virtual int GetShort()
//        {
//            //get
//            {
//                offset += 2;
//                return ((data[offset - 2] & 0xff) << 8) + (data[offset - 1] & 0xff);
//            }
//        }

//        public virtual int GetInt()
//        {
//            //get
//            {
//                offset += 4;
//                return ((data[offset - 4] & 0xff) << 24) + ((data[offset - 3] & 0xff) << 16) + ((data[offset - 2] & 0xff) << 8) + (data[offset - 1] & 0xff);
//            }
//        }

//        public virtual void GetBytes(sbyte[] arg0, int arg1, int arg2)
//        {
//            for (int i = arg1; i < arg1 + arg2; i += 1)
//            {
//                arg0[i] = data[offset++];
//            }

//        }

//        public sbyte[] data;
//        public int offset;
//    }
}