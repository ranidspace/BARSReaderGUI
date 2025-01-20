﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BARSReaderGUI
{
    public class AudioAsset
    {
        public uint crcHash;
        public uint amtaOffset;
        public int assetOffset;
        public string assetName;
        public string assetType;
        public bool isPrefetch = false;
        public byte[] assetData;
        public byte[] amtaAssetData;
        public AMTA amtaData = new AMTA();
    }
}
