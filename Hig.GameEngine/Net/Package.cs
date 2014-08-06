namespace Hig.GameEngine.Net
{
    using System.IO;

    public class Package
    {
        public PackageType Type { get; protected set; }
        public ulong TotalMSec { get; protected set; }
        public byte[] Content { get; protected set; }

        public Package(PackageType packageType, ulong totoalMsec, byte[] content = null)
        {
            Content = (content == null) ? new byte[0] : content;
            TotalMSec = totoalMsec;
            Type = packageType;
        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)Type);
                    bw.Write(TotalMSec);
                    bw.Write((ushort)Content.Length);
                    bw.Write(Content);
                }

                return ms.ToArray();
            }
        }

        public static Package FromBytes(byte[] buffer)
        {
            if (buffer.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        PackageType type = (PackageType)br.ReadByte();
                        ulong totalMsec = br.ReadUInt64();
                        ushort count = br.ReadUInt16();

                        return new Package(type, totalMsec, br.ReadBytes(count));
                    }
                }
            }

            return null;
        }
    }
}
