namespace BARSReaderGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFile = fileDialog.FileName;

                using (NativeReader reader = new(new FileStream(inputFile, FileMode.Open)))
                {
                    if (reader.ReadUInt() == 0xFD2FB528) //zstd check
                    {
                        MessageBox.Show("ZSTD Compressed files not supported at this time."); //need to add zstd decompression
                        return;
                    }
                    else
                    {
                        reader.Position = 0; //reset position
                    }

                    string magic;
                    uint size;
                    ushort endian;
                    ushort version;
                    uint assetcount;
                    KeyValuePair<uint, AssetOffsetPair>[] assets;

                    magic = reader.ReadSizedString(4);
                    if (magic != "BARS")
                    {
                        MessageBox.Show("Not a BARS file.");
                        return;
                    }

                    size = reader.ReadUInt();

                    endian = reader.ReadUShort();
                    if (endian != 0xFEFF)
                    {
                        MessageBox.Show("Unsupported endian!");
                        return;
                    }

                    version = reader.ReadUShort();
                    if (version != 0x0102)
                    {
                        MessageBox.Show("BARS V1.1 Is unsupported at this time."); //we don't support anything but v102 atm
                        return;
                    }

                    assetcount = reader.ReadUInt();
                    assets = new KeyValuePair<uint, AssetOffsetPair>[assetcount];

                    // Pair CRC32 hash with asset
                    for (int i = 0; i < assetcount; i++)
                        assets[i] = new KeyValuePair<uint, AssetOffsetPair>(reader.ReadUInt(), new AssetOffsetPair());

                    // Pair ATMA/BWAV offsets with asset
                    for (int i = 0; i < assetcount; i++)
                    {
                        assets[i].Value.amtaoffset = reader.ReadUInt();
                        assets[i].Value.bwavoffset = reader.ReadUInt();
                    }

                    bwavListBox.Items.Clear();
                    for (int i = 0; i < assetcount; i++)
                    {
                        reader.Position = assets[i].Value.amtaoffset + 0x24;
                        uint unkOffset = reader.ReadUInt();
                        reader.Position = assets[i].Value.amtaoffset + unkOffset + 36;

                        bwavListBox.Items.Add(reader.ReadNullTerminatedString());
                    }

                    MessageBox.Show("Successfully read " + assetcount + " assets.");
                }
            }
        }

        public void ReadAMTA(uint startPosition, NativeReader reader)
        {

            //reader.Position = startPosition;
            //string magic = reader.ReadSizedString(4);
            //ushort endian = reader.ReadUShort();
            //ushort version = reader.ReadUShort();
            //uint size = reader.ReadUInt();
            //uint unk1 = reader.ReadUInt();
            //uint unk2 = reader.ReadUInt();
            //uint unk3 = reader.ReadUInt();
            //uint unk4 = reader.ReadUInt();
            //uint unk5 = reader.ReadUInt();
            //uint unk6 = reader.ReadUInt();

            //string fileName;

            //if (magic != "AMTA")
            //    return "";

            //if (endian != 0xFEFF)
            //    return "";

            //reader.Position += 0x1C;

            //uint nameOffset1 = reader.ReadUInt();
            //reader.Position = startPosition + nameOffset1 + 36;
            //fileName = reader.ReadNullTerminatedString();

            //reader.Position = startPosition;

            //return fileName;
        }
    }
    public class AssetOffsetPair
    {
        public uint amtaoffset;
        public uint bwavoffset;
    }
}