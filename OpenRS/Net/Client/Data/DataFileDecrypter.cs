using System;

namespace OpenRS.Net.Client.Data
{

    public sealed class DataFileDecrypter
    {

        public static long UnpackData(sbyte[] outputBuffer, int decompressedSize, sbyte[] inputBuffer, int compressedSize, int startOffset)
        {
            BZip2BlockEntry blockEntry = new()
            {
                inputBuffer = inputBuffer,
                offset = startOffset,
                outputBuffer = outputBuffer,
                outputIndex = 0,
                compressedSize = compressedSize,
                decompressedSize = decompressedSize,
                bitsAvailable = 0,
                currentBitWord = 0,
                bytesReadLow = 0,
                bytesReadHigh = 0,
                bytesWrittenLow = 0,
                bytesWrittenHigh = 0,
                blocksRead = 0
            };
            ReadBlock(blockEntry);
            decompressedSize -= blockEntry.decompressedSize;

            return decompressedSize;
        }

        public static long UnpackData(byte[] outputBuffer, int decompressedSize, byte[] inputBuffer, int compressedSize, int startOffset)
        {
            BZip2BlockEntry blockEntry = new()
            {
                inputBuffer = (sbyte[])(Array)inputBuffer,
                offset = startOffset,
                outputBuffer = (sbyte[])(Array)outputBuffer,
                outputIndex = 0,
                compressedSize = compressedSize,
                decompressedSize = decompressedSize,
                bitsAvailable = 0,
                currentBitWord = 0,
                bytesReadLow = 0,
                bytesReadHigh = 0,
                bytesWrittenLow = 0,
                bytesWrittenHigh = 0,
                blocksRead = 0
            };
            ReadBlock(blockEntry);
            decompressedSize -= blockEntry.decompressedSize;

            return decompressedSize;
        }


        private static void DecodeRunLength(BZip2BlockEntry blockEntry)
        {
            sbyte sbyte4 = blockEntry.lastOutputByte;
            int runCount = blockEntry.runLength;
            int symbolIndex = blockEntry.symbolIndex;
            int currentByte = blockEntry.currentByteValue;
            int[] transformVector = BZip2BlockEntry.transformVector;
            int linkedListNode = blockEntry.linkedListNode;
            sbyte[] outputData = blockEntry.outputBuffer;
            int outputIndex = blockEntry.outputIndex;
            int remainingOutput = blockEntry.decompressedSize;
            int initialOutput = remainingOutput;
            int symbolCount = blockEntry.lastSymbolIndex + 1;
            // label0:
            do
            {
                if (runCount > 0)
                {
                    do
                    {
                        if (remainingOutput == 0)
                        {
                            goto done;
                        }

                        if (runCount == 1)
                        {
                            break;
                        }

                        outputData[outputIndex] = sbyte4;
                        runCount -= 1;
                        outputIndex += 1;
                        remainingOutput -= 1;
                    } while (true);
                    if (remainingOutput == 0)
                    {
                        runCount = 1;
                        break;
                    }
                    outputData[outputIndex] = sbyte4;
                    outputIndex += 1;
                    remainingOutput -= 1;
                }

                bool processSymbol = true;

                while (processSymbol)
                {
                    processSymbol = false;

                    if (symbolIndex == symbolCount)
                    {
                        runCount = 0;
                        goto done;
                    }

                    sbyte4 = (sbyte)currentByte;
                    linkedListNode = transformVector[linkedListNode];
                    byte nextByte0 = (byte)(linkedListNode & 0xff);
                    linkedListNode >>= 8;
                    symbolIndex += 1;

                    if (nextByte0 != currentByte)
                    {
                        currentByte = nextByte0;

                        if (remainingOutput == 0)
                        {
                            runCount = 1;
                        }
                        else
                        {
                            outputData[outputIndex] = sbyte4;
                            outputIndex += 1;
                            remainingOutput -= 1;
                            processSymbol = true;
                            continue;
                        }

                        goto done;
                    }

                    if (symbolIndex != symbolCount)
                    {
                        continue;
                    }

                    if (remainingOutput == 0)
                    {
                        runCount = 1;
                        goto done;
                    }

                    outputData[outputIndex] = sbyte4;
                    outputIndex += 1;
                    remainingOutput -= 1;
                    processSymbol = true;
                }

                runCount = 2;
                linkedListNode = transformVector[linkedListNode];
                byte nextByte1 = (byte)(linkedListNode & 0xff);
                linkedListNode >>= 8;
                symbolIndex += 1;

                if (symbolIndex != symbolCount)
                {
                    if (nextByte1 != currentByte)
                    {
                        currentByte = nextByte1;
                    }
                    else
                    {
                        runCount = 3;
                        linkedListNode = transformVector[linkedListNode];
                        byte nextByte2 = (byte)(linkedListNode & 0xff);
                        linkedListNode >>= 8;
                        symbolIndex += 1;

                        if (symbolIndex != symbolCount)
                        {
                            if (nextByte2 != currentByte)
                            {
                                currentByte = nextByte2;
                            }
                            else
                            {
                                linkedListNode = transformVector[linkedListNode];
                                byte nextByte3 = (byte)(linkedListNode & 0xff);
                                linkedListNode >>= 8;
                                symbolIndex += 1;
                                runCount = (nextByte3 & 0xff) + 4;
                                linkedListNode = transformVector[linkedListNode];
                                currentByte = (linkedListNode & 0xff);
                                linkedListNode >>= 8;
                                symbolIndex += 1;
                            }
                        }
                    }
                }
            } while (true);

        done:

            int outputDelta = blockEntry.bytesWrittenLow;
            blockEntry.bytesWrittenLow += initialOutput - remainingOutput;

            if (blockEntry.bytesWrittenLow < outputDelta)
            {
                blockEntry.bytesWrittenHigh += 1;
            }

            blockEntry.lastOutputByte = sbyte4;
            blockEntry.runLength = runCount;
            blockEntry.symbolIndex = symbolIndex;
            blockEntry.currentByteValue = currentByte;
            BZip2BlockEntry.transformVector = transformVector;
            blockEntry.linkedListNode = linkedListNode;
            blockEntry.outputBuffer = outputData;
            blockEntry.outputIndex = outputIndex;
            blockEntry.decompressedSize = remainingOutput;
        }


        private static void ReadBlock(BZip2BlockEntry blockEntry)
        {
            int minLens_zt = 0;
            int[] limit_zt = null;
            int[] base_zt = null;
            int[] perm_zt = null;
            blockEntry.blockSize100k = 1;
            if (BZip2BlockEntry.transformVector is null)
            {
                BZip2BlockEntry.transformVector = new int[blockEntry.blockSize100k * 0x186a0];
            }
            bool flag19 = true;
            while (flag19)
            {
                sbyte tmpRegister = GetUByte(blockEntry);
                if (tmpRegister == 23)
                {
                    return;
                }
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                blockEntry.blocksRead += 1;
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetUByte(blockEntry);
                tmpRegister = GetBit(blockEntry);
                blockEntry.isRandomised = tmpRegister != 0;
                if (blockEntry.isRandomised)
                {
                    Console.WriteLine("PANIC! RANDOMISED BLOCK!");
                }
                blockEntry.origPtr = 0;
                tmpRegister = GetUByte(blockEntry);
                blockEntry.origPtr = blockEntry.origPtr << 8 | tmpRegister & 0xff;
                tmpRegister = GetUByte(blockEntry);
                blockEntry.origPtr = blockEntry.origPtr << 8 | tmpRegister & 0xff;
                tmpRegister = GetUByte(blockEntry);
                blockEntry.origPtr = blockEntry.origPtr << 8 | tmpRegister & 0xff;
                for (int j = 0; j < 16; j++)
                {
                    sbyte sbyte1 = GetBit(blockEntry);
                    if (sbyte1 == 1)
                    {
                        blockEntry.inUse16[j] = true;
                    }
                    else
                    {
                        blockEntry.inUse16[j] = false;
                    }
                }

                for (int k = 0; k < 256; k++)
                {
                    blockEntry.inUse[k] = false;
                }

                for (int l = 0; l < 16; l++)
                {
                    if (blockEntry.inUse16[l])
                    {
                        for (int i3 = 0; i3 < 16; i3++)
                        {
                            sbyte sbyte2 = GetBit(blockEntry);
                            if (sbyte2 == 1)
                            {
                                blockEntry.inUse[l * 16 + i3] = true;
                            }
                        }

                    }
                }

                CreateMaps(blockEntry);
                int alphaSize = blockEntry.inUseOffset + 2;
                int groupCount = GetBits(3, blockEntry);
                int selectorCount = GetBits(15, blockEntry);
                for (int i1 = 0; i1 < selectorCount; i1++)
                {
                    int j3 = 0;
                    do
                    {
                        sbyte sbyte3 = GetBit(blockEntry);
                        if (sbyte3 == 0)
                        {
                            break;
                        }
                        j3 += 1;
                    } while (true);
                     blockEntry.selectorMtf[i1] = (sbyte)j3;
                    //blockEntry.selectorMtf[i1] = (byte)j3;
                }

                sbyte[] pos = new sbyte[6];
                for (sbyte sbyte16 = 0; sbyte16 < groupCount; sbyte16++)
                {
                    pos[sbyte16] = sbyte16;
                }

                for (int j1 = 0; j1 < selectorCount; j1++)
                {
                    sbyte sbyte17 = blockEntry.selectorMtf[j1];
                    sbyte sbyte15 = pos[sbyte17];
                    for (; sbyte17 > 0; sbyte17--)
                    {
                        pos[sbyte17] = pos[sbyte17 - 1];
                    }

                    pos[0] = sbyte15;
                    blockEntry.selector[j1] = sbyte15;
                }

                for (int k3 = 0; k3 < groupCount; k3++)
                {
                    int l6 = GetBits(5, blockEntry);
                    for (int k1 = 0; k1 < alphaSize; k1++)
                    {
                        do
                        {
                            sbyte sbyte4 = GetBit(blockEntry);
                            if (sbyte4 == 0)
                            {
                                break;
                            }
                            sbyte4 = GetBit(blockEntry);
                            if (sbyte4 == 0)
                            {
                                l6 += 1;
                            }
                            else
                            {
                                l6 -= 1;
                            }
                        } while (true);
                        blockEntry.len[k3][k1] = (sbyte)l6;
                    }

                }

                for (int l3 = 0; l3 < groupCount; l3++)
                {
                    sbyte minlen = 32;
                    int maxlen = 0;
                    for (int l1 = 0; l1 < alphaSize; l1++)
                    {
                        if (blockEntry.len[l3][l1] > maxlen)
                        {
                            maxlen = blockEntry.len[l3][l1];
                        }
                        if (blockEntry.len[l3][l1] < minlen)
                        {
                            minlen = (sbyte)blockEntry.len[l3][l1];
                        }
                    }

                CreateDecodeTables(blockEntry.limit[l3], blockEntry._base[l3], blockEntry.perm[l3], blockEntry.len[l3], minlen, maxlen, alphaSize);
                    blockEntry.minLengths[l3] = minlen;
                }

                int l4 = blockEntry.inUseOffset + 1;
                int lastShadow = -1;
                int groupPos = 0;
                for (int i2 = 0; i2 <= 255; i2++)
                {
                    blockEntry.unzftab[i2] = 0;
                }

                int j9 = 4095;
                for (int l8 = 15; l8 >= 0; l8--)
                {
                    for (int i9 = 15; i9 >= 0; i9--)
                    {
                        blockEntry.yy[j9] = (byte)(l8 * 16 + i9);
                        j9 -= 1;
                    }

                    blockEntry.groupPositions[l8] = j9 + 1;
                }

                int i6 = 0;
                if (groupPos == 0)
                {
                    lastShadow += 1;
                    groupPos = 50;
                    sbyte sbyte12 = blockEntry.selector[lastShadow];
                    minLens_zt = blockEntry.minLengths[sbyte12];
                    limit_zt = blockEntry.limit[sbyte12];
                    perm_zt = blockEntry.perm[sbyte12];
                    base_zt = blockEntry._base[sbyte12];
                }
                groupPos -= 1;
                int i7 = minLens_zt;
                int l7;
                sbyte sbyte9;
                for (l7 = GetBits(i7, blockEntry); l7 > limit_zt[i7]; l7 = l7 << 1 | sbyte9)
                {
                    i7 += 1;
                    sbyte9 = GetBit(blockEntry);
                }

                for (int nextSym = perm_zt[l7 - base_zt[i7]]; nextSym != l4; )
                {
                    if (nextSym == 0 || nextSym == 1)
                    {
                        int j6 = -1;
                        int k6 = 1;
                        do
                        {
                            if (nextSym == 0)
                            {
                                j6 += k6;
                            }
                            else
                            {
                                if (nextSym == 1)
                                {
                                    j6 += 2 * k6;
                                }
                            }
                            k6 *= 2;
                            if (groupPos == 0)
                            {
                                lastShadow += 1;
                                groupPos = 50;
                                sbyte sbyte13 = blockEntry.selector[lastShadow];
                                minLens_zt = blockEntry.minLengths[sbyte13];
                                limit_zt = blockEntry.limit[sbyte13];
                                perm_zt = blockEntry.perm[sbyte13];
                                base_zt = blockEntry._base[sbyte13];
                            }
                            groupPos -= 1;
                            int j7 = minLens_zt;
                            int i8;
                            sbyte sbyte10;
                            for (i8 = GetBits(j7, blockEntry); i8 > limit_zt[j7]; i8 = i8 << 1 | (byte)sbyte10)
                            {
                                j7 += 1;
                                sbyte10 = GetBit(blockEntry);
                            }

                            nextSym = perm_zt[i8 - base_zt[j7]];
                        } while (nextSym == 0 || nextSym == 1);
                        j6 += 1;
                        sbyte sbyte5 = (sbyte)blockEntry.seqToUnseq[blockEntry.yy[blockEntry.groupPositions[0]] & 0xff];
                        blockEntry.unzftab[sbyte5 & 0xff] += j6;
                        for (; j6 > 0; j6--)
                        {
                            BZip2BlockEntry.transformVector[i6] = sbyte5 & 0xff;
                            i6 += 1;
                        }

                    }
                    else
                    {
                        int j11 = nextSym - 1;
                        sbyte sbyte6;
                        if (j11 < 16)
                        {
                            int j10 = blockEntry.groupPositions[0];
                            sbyte6 = (sbyte)blockEntry.yy[j10 + j11];
                            for (; j11 > 3; j11 -= 4)
                            {
                                int k11 = j10 + j11;
                                blockEntry.yy[k11] = blockEntry.yy[k11 - 1];
                                blockEntry.yy[k11 - 1] = blockEntry.yy[k11 - 2];
                                blockEntry.yy[k11 - 2] = blockEntry.yy[k11 - 3];
                                blockEntry.yy[k11 - 3] = blockEntry.yy[k11 - 4];
                            }

                            for (; j11 > 0; j11--)
                            {
                                blockEntry.yy[j10 + j11] = blockEntry.yy[(j10 + j11) - 1];
                            }

                            blockEntry.yy[j10] = sbyte6;
                        }
                        else
                        {
                            int l10 = j11 / 16;
                            int i11 = j11 % 16;
                            int k10 = blockEntry.groupPositions[l10] + i11;
                            sbyte6 = (sbyte)blockEntry.yy[k10];
                            for (; k10 > blockEntry.groupPositions[l10]; k10--)
                            {
                                blockEntry.yy[k10] = blockEntry.yy[k10 - 1];
                            }

                            blockEntry.groupPositions[l10] += 1;
                            for (; l10 > 0; l10--)
                            {
                                blockEntry.groupPositions[l10] -= 1;
                                blockEntry.yy[blockEntry.groupPositions[l10]] = blockEntry.yy[(blockEntry.groupPositions[l10 - 1] + 16) - 1];
                            }

                            blockEntry.groupPositions[0] -= 1;
                            blockEntry.yy[blockEntry.groupPositions[0]] = sbyte6;
                            if (blockEntry.groupPositions[0] == 0)
                            {
                                int i10 = 4095;
                                for (int k9 = 15; k9 >= 0; k9--)
                                {
                                    for (int l9 = 15; l9 >= 0; l9--)
                                    {
                                        blockEntry.yy[i10] = blockEntry.yy[blockEntry.groupPositions[k9] + l9];
                                        i10 -= 1;
                                    }

                                    blockEntry.groupPositions[k9] = i10 + 1;
                                }

                            }
                        }
                        blockEntry.unzftab[blockEntry.seqToUnseq[sbyte6 & 0xff] & 0xff] += 1;;
                        BZip2BlockEntry.transformVector[i6] = blockEntry.seqToUnseq[sbyte6 & 0xff] & 0xff;
                        i6 += 1;
                        if (groupPos == 0)
                        {
                            lastShadow += 1;
                            groupPos = 50;
                            sbyte sbyte14 = blockEntry.selector[lastShadow];
                            minLens_zt = blockEntry.minLengths[sbyte14];
                            limit_zt = blockEntry.limit[sbyte14];
                            perm_zt = blockEntry.perm[sbyte14];
                            base_zt = blockEntry._base[sbyte14];
                        }
                        groupPos -= 1;
                        int k7 = minLens_zt;
                        int j8;
                        sbyte sbyte11;
                        for (j8 = GetBits(k7, blockEntry); j8 > limit_zt[k7]; j8 = j8 << 1 | sbyte11)
                        {
                            k7 += 1;
                            sbyte11 = GetBit(blockEntry);
                        }

                        nextSym = perm_zt[j8 - base_zt[k7]];
                    }
                }

                blockEntry.runLength = 0;
                blockEntry.lastOutputByte = 0;
                blockEntry.cumulativeCounts[0] = 0;
                for (int j2 = 1; j2 <= 256; j2++)
                {
                    blockEntry.cumulativeCounts[j2] = blockEntry.unzftab[j2 - 1];
                }

                for (int k2 = 1; k2 <= 256; k2++)
                {
                    blockEntry.cumulativeCounts[k2] += blockEntry.cumulativeCounts[k2 - 1];
                }

                for (int l2 = 0; l2 < i6; l2++)
                {
                    int sbyte7 = ((BZip2BlockEntry.transformVector[l2]) & 0xff);
                    BZip2BlockEntry.transformVector[blockEntry.cumulativeCounts[sbyte7 & 0xff]] |= l2 << 8;
                    blockEntry.cumulativeCounts[sbyte7 & 0xff] += 1;
                }

                blockEntry.linkedListNode = BZip2BlockEntry.transformVector[blockEntry.origPtr] >> 8;
                blockEntry.symbolIndex = 0;
                blockEntry.linkedListNode = BZip2BlockEntry.transformVector[blockEntry.linkedListNode];
                blockEntry.currentByteValue = (blockEntry.linkedListNode & 0xff);
                blockEntry.linkedListNode >>= 8;
                blockEntry.symbolIndex += 1;
                blockEntry.lastSymbolIndex = i6;
                DecodeRunLength(blockEntry);

                if (blockEntry.symbolIndex == blockEntry.lastSymbolIndex + 1 && blockEntry.runLength == 0)
                {
                    flag19 = true;
                }
                else
                {
                    flag19 = false;
                }
            }
        }

        private static sbyte GetUByte(BZip2BlockEntry blockEntry) => (sbyte)GetBits(8, blockEntry);

        private static sbyte GetBit(BZip2BlockEntry blockEntry) => (sbyte)GetBits(1, blockEntry);

        private static int GetBits(int bitCount, BZip2BlockEntry blockEntry)
        {
            int result;

            do
            {
                if (blockEntry.bitsAvailable >= bitCount)
                {
                    int value = blockEntry.currentBitWord >> blockEntry.bitsAvailable - bitCount & (1 << bitCount) - 1;
                    blockEntry.bitsAvailable -= bitCount;
                    result = value;
                    break;
                }

                blockEntry.currentBitWord = blockEntry.currentBitWord << 8 | blockEntry.inputBuffer[blockEntry.offset] & 0xff;
                blockEntry.bitsAvailable += 8;
                blockEntry.offset += 1;
                blockEntry.compressedSize -= 1;
                blockEntry.bytesReadLow += 1;

                if (blockEntry.bytesReadLow == 0)
                {
                    blockEntry.bytesReadHigh += 1;
                }
            } while (true);

            return result;
        }

        private static void CreateMaps(BZip2BlockEntry blockEntry)
        {
            blockEntry.inUseOffset = 0;
            for (int i = 0; i < 256; i++)
            {
                if (blockEntry.inUse[i])
                {
                    blockEntry.seqToUnseq[blockEntry.inUseOffset] = (byte)i;
                    blockEntry.inUseOffset += 1;
                }
            }

        }

        private static void CreateDecodeTables(int[] limit, int[] _base, int[] perm, sbyte[] length, int minlen, int maxlen, int alphasize)
        {
            int i = 0;
            for (int j = minlen; j <= maxlen; j++)
            {
                for (int i2 = 0; i2 < alphasize; i2++)
                {
                    if (length[i2] == j)
                    {
                        perm[i] = i2;
                        i += 1;
                    }
                }

            }

            for (int k = 0; k < 23; k++)
            {
                _base[k] = 0;
            }

            for (int l = 0; l < alphasize; l++)
            {
                _base[length[l] + 1]++;
            }

            for (int i1 = 1; i1 < 23; i1++)
            {
                _base[i1] += _base[i1 - 1];
            }

            for (int j1 = 0; j1 < 23; j1++)
            {
                limit[j1] = 0;
            }

            int j2 = 0;
            for (int k1 = minlen; k1 <= maxlen; k1++)
            {
                j2 += _base[k1 + 1] - _base[k1];
                limit[k1] = j2 - 1;
                j2 <<= 1;
            }

            for (int l1 = minlen + 1; l1 <= maxlen; l1++)
            {
                _base[l1] = (limit[l1 - 1] + 1 << 1) - _base[l1];
            }

        }
    }

}