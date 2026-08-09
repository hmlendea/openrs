using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Data
{
    internal sealed class DataFileDecrypter
    {
        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<DataFileDecrypter>();

        public static long UnpackData(sbyte[] outputBuffer, int decompressedSize, sbyte[] inputBuffer, int compressedSize, int startOffset)
        {
            BZip2BlockEntry blockEntry = new()
            {
                InputBuffer = inputBuffer,
                Offset = startOffset,
                OutputBuffer = outputBuffer,
                OutputIndex = 0,
                CompressedSize = compressedSize,
                DecompressedSize = decompressedSize,
                BitsAvailable = 0,
                CurrentBitWord = 0,
                BytesReadLow = 0,
                BytesReadHigh = 0,
                BytesWrittenLow = 0,
                BytesWrittenHigh = 0,
                BlocksRead = 0
            };
            ReadBlock(blockEntry);

            return decompressedSize - blockEntry.DecompressedSize;
        }

        public static long UnpackData(byte[] outputBuffer, int decompressedSize, byte[] inputBuffer, int compressedSize, int startOffset)
        {
            return UnpackData(
                (sbyte[])(Array)outputBuffer,
                decompressedSize,
                (sbyte[])(Array)inputBuffer,
                compressedSize,
                startOffset);
        }

        private static void DecodeRunLength(BZip2BlockEntry blockEntry)
        {
            sbyte currentRunByte = blockEntry.LastOutputByte;
            int runCount = blockEntry.RunLength;
            int symbolIndex = blockEntry.SymbolIndex;
            int currentByte = blockEntry.CurrentByteValue;
            int[] transformVector = BZip2BlockEntry.TransformVector;
            int linkedListNode = blockEntry.LinkedListNode;
            sbyte[] outputData = blockEntry.OutputBuffer;
            int outputIndex = blockEntry.OutputIndex;
            int remainingOutput = blockEntry.DecompressedSize;
            int initialOutput = remainingOutput;
            int symbolCount = blockEntry.LastSymbolIndex + 1;

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

                        outputData[outputIndex] = currentRunByte;
                        runCount -= 1;
                        outputIndex += 1;
                        remainingOutput -= 1;
                    } while (true);

                    if (remainingOutput == 0)
                    {
                        runCount = 1;
                        break;
                    }

                    outputData[outputIndex] = currentRunByte;
                    outputIndex += 1;
                    remainingOutput -= 1;
                }

                bool shouldProcessSymbol = true;

                while (shouldProcessSymbol)
                {
                    shouldProcessSymbol = false;

                    if (symbolIndex == symbolCount)
                    {
                        runCount = 0;
                        goto done;
                    }

                    currentRunByte = (sbyte)currentByte;
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
                            outputData[outputIndex] = currentRunByte;
                            outputIndex += 1;
                            remainingOutput -= 1;
                            shouldProcessSymbol = true;
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

                    outputData[outputIndex] = currentRunByte;
                    outputIndex += 1;
                    remainingOutput -= 1;
                    shouldProcessSymbol = true;
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
                                currentByte = linkedListNode & 0xff;
                                linkedListNode >>= 8;
                                symbolIndex += 1;
                            }
                        }
                    }
                }
            } while (true);

        done:

            int outputDelta = blockEntry.BytesWrittenLow;
            blockEntry.BytesWrittenLow += initialOutput - remainingOutput;

            if (blockEntry.BytesWrittenLow < outputDelta)
            {
                blockEntry.BytesWrittenHigh += 1;
            }

            blockEntry.LastOutputByte = currentRunByte;
            blockEntry.RunLength = runCount;
            blockEntry.SymbolIndex = symbolIndex;
            blockEntry.CurrentByteValue = currentByte;
            BZip2BlockEntry.TransformVector = transformVector;
            blockEntry.LinkedListNode = linkedListNode;
            blockEntry.OutputBuffer = outputData;
            blockEntry.OutputIndex = outputIndex;
            blockEntry.DecompressedSize = remainingOutput;
        }

        private static void ReadBlock(BZip2BlockEntry blockEntry)
        {
            int currentGroupMinLength = 0;
            int[] currentGroupLimits = null;
            int[] currentGroupBaseValues = null;
            int[] currentGroupPermutations = null;
            blockEntry.BlockSize100k = 1;

            BZip2BlockEntry.TransformVector ??= new int[blockEntry.BlockSize100k * 0x186a0];

            bool hasMoreBlocks = true;

            while (hasMoreBlocks)
            {
                sbyte headerByte = GetUByte(blockEntry);

                if (headerByte == 23)
                {
                    return;
                }

                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                blockEntry.BlocksRead += 1;
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                sbyte randomisedBit = GetBit(blockEntry);
                blockEntry.IsRandomised = randomisedBit != 0;

                if (blockEntry.IsRandomised)
                {
                    logger.Error(GameOperation.ReadDataBlock, "Encountered a randomised block.");
                }

                blockEntry.OriginPointer = 0;
                sbyte origPtrByte0 = GetUByte(blockEntry);
                blockEntry.OriginPointer = blockEntry.OriginPointer << 8 | origPtrByte0 & 0xff;
                sbyte origPtrByte1 = GetUByte(blockEntry);
                blockEntry.OriginPointer = blockEntry.OriginPointer << 8 | origPtrByte1 & 0xff;
                sbyte origPtrByte2 = GetUByte(blockEntry);
                blockEntry.OriginPointer = blockEntry.OriginPointer << 8 | origPtrByte2 & 0xff;

                for (int groupIndex = 0; groupIndex < 16; groupIndex += 1)
                {
                    sbyte groupActiveBit = GetBit(blockEntry);

                    if (groupActiveBit == 1)
                    {
                        blockEntry.SymbolGroupFlags[groupIndex] = true;
                    }
                    else
                    {
                        blockEntry.SymbolGroupFlags[groupIndex] = false;
                    }
                }

                for (int byteIndex = 0; byteIndex < 256; byteIndex += 1)
                {
                    blockEntry.InUse[byteIndex] = false;
                }

                for (int symbolGroupIndex = 0; symbolGroupIndex < 16; symbolGroupIndex += 1)
                {
                    if (blockEntry.SymbolGroupFlags[symbolGroupIndex])
                    {
                        for (int symbolOffset = 0; symbolOffset < 16; symbolOffset += 1)
                        {
                            sbyte symbolActiveBit = GetBit(blockEntry);

                            if (symbolActiveBit == 1)
                            {
                                blockEntry.InUse[symbolGroupIndex * 16 + symbolOffset] = true;
                            }
                        }
                    }
                }

                CreateMaps(blockEntry);
                int alphaSize = blockEntry.ActiveSymbolCount + 2;
                int groupCount = GetBits(3, blockEntry);
                int selectorCount = GetBits(15, blockEntry);

                for (int selectorMtfIndex = 0; selectorMtfIndex < selectorCount; selectorMtfIndex += 1)
                {
                    int mtfPosition = 0;

                    do
                    {
                        sbyte selectorBit = GetBit(blockEntry);

                        if (selectorBit == 0)
                        {
                            break;
                        }

                        mtfPosition += 1;
                    } while (true);

                    blockEntry.SelectorMoveToFront[selectorMtfIndex] = (sbyte)mtfPosition;
                }

                sbyte[] selectorPositions = new sbyte[6];

                for (sbyte positionIndex = 0; positionIndex < groupCount; positionIndex += 1)
                {
                    selectorPositions[positionIndex] = positionIndex;
                }

                for (int selectorDecodeIndex = 0; selectorDecodeIndex < selectorCount; selectorDecodeIndex += 1)
                {
                    sbyte mtfIndex = blockEntry.SelectorMoveToFront[selectorDecodeIndex];
                    sbyte selectedGroupId = selectorPositions[mtfIndex];

                    for (; mtfIndex > 0; mtfIndex -= 1)
                    {
                        selectorPositions[mtfIndex] = selectorPositions[mtfIndex - 1];
                    }

                    selectorPositions[0] = selectedGroupId;
                    blockEntry.Selector[selectorDecodeIndex] = selectedGroupId;
                }

                for (int tableIndex = 0; tableIndex < groupCount; tableIndex += 1)
                {
                    int currentCodeLength = GetBits(5, blockEntry);

                    for (int symbolPosition = 0; symbolPosition < alphaSize; symbolPosition += 1)
                    {
                        do
                        {
                            sbyte bitAdjustDirection = GetBit(blockEntry);

                            if (bitAdjustDirection == 0)
                            {
                                break;
                            }

                            bitAdjustDirection = GetBit(blockEntry);

                            if (bitAdjustDirection == 0)
                            {
                                currentCodeLength += 1;
                            }
                            else
                            {
                                currentCodeLength -= 1;
                            }
                        } while (true);

                        blockEntry.HuffmanCodeLengths[tableIndex][symbolPosition] = (sbyte)currentCodeLength;
                    }
                }

                for (int huffmanGroupIndex = 0; huffmanGroupIndex < groupCount; huffmanGroupIndex += 1)
                {
                    sbyte minimumCodeLength = 32;
                    int maximumCodeLength = 0;

                    for (int lengthScanIndex = 0; lengthScanIndex < alphaSize; lengthScanIndex += 1)
                    {
                        if (blockEntry.HuffmanCodeLengths[huffmanGroupIndex][lengthScanIndex] > maximumCodeLength)
                        {
                            maximumCodeLength = blockEntry.HuffmanCodeLengths[huffmanGroupIndex][lengthScanIndex];
                        }

                        if (blockEntry.HuffmanCodeLengths[huffmanGroupIndex][lengthScanIndex] < minimumCodeLength)
                        {
                            minimumCodeLength = blockEntry.HuffmanCodeLengths[huffmanGroupIndex][lengthScanIndex];
                        }
                    }

                    CreateDecodeTables(blockEntry.HuffmanLimits[huffmanGroupIndex], blockEntry.HuffmanBaseValues[huffmanGroupIndex], blockEntry.HuffmanPermutations[huffmanGroupIndex], blockEntry.HuffmanCodeLengths[huffmanGroupIndex], minimumCodeLength, maximumCodeLength, alphaSize);
                    blockEntry.HuffmanMinLengths[huffmanGroupIndex] = minimumCodeLength;
                }

                int endOfStreamSymbol = blockEntry.ActiveSymbolCount + 1;
                int currentSelectorIndex = -1;
                int groupSymbolsRemaining = 0;

                for (int tabResetIndex = 0; tabResetIndex <= 255; tabResetIndex += 1)
                {
                    blockEntry.SymbolFrequencyTable[tabResetIndex] = 0;
                }

                int mtfBufferIndex = 4095;

                for (int groupRowIndex = 15; groupRowIndex >= 0; groupRowIndex -= 1)
                {
                    for (int groupColIndex = 15; groupColIndex >= 0; groupColIndex -= 1)
                    {
                        blockEntry.MoveToFrontBuffer[mtfBufferIndex] = (byte)(groupRowIndex * 16 + groupColIndex);
                        mtfBufferIndex -= 1;
                    }

                    blockEntry.GroupPositions[groupRowIndex] = mtfBufferIndex + 1;
                }

                int decodedSymbolCount = 0;

                if (groupSymbolsRemaining == 0)
                {
                    currentSelectorIndex += 1;
                    groupSymbolsRemaining = 50;
                    sbyte selectorId = blockEntry.Selector[currentSelectorIndex];
                    currentGroupMinLength = blockEntry.HuffmanMinLengths[selectorId];
                    currentGroupLimits = blockEntry.HuffmanLimits[selectorId];
                    currentGroupPermutations = blockEntry.HuffmanPermutations[selectorId];
                    currentGroupBaseValues = blockEntry.HuffmanBaseValues[selectorId];
                }

                groupSymbolsRemaining -= 1;
                int currentBitLength = currentGroupMinLength;
                int currentCode;
                sbyte nextDecodeBit;

                for (currentCode = GetBits(currentBitLength, blockEntry); currentCode > currentGroupLimits[currentBitLength]; currentCode = currentCode << 1 | (byte)nextDecodeBit)
                {
                    currentBitLength += 1;
                    nextDecodeBit = GetBit(blockEntry);
                }

                for (int nextSym = currentGroupPermutations[currentCode - currentGroupBaseValues[currentBitLength]]; nextSym != endOfStreamSymbol; )
                {
                    if (nextSym == 0 || nextSym == 1)
                    {
                        int runRepeatCount = -1;
                        int runLengthMultiplier = 1;

                        do
                        {
                            if (nextSym == 0)
                            {
                                runRepeatCount += runLengthMultiplier;
                            }
                            else
                            {
                                if (nextSym == 1)
                                {
                                    runRepeatCount += 2 * runLengthMultiplier;
                                }
                            }

                            runLengthMultiplier *= 2;

                            if (groupSymbolsRemaining == 0)
                            {
                                currentSelectorIndex += 1;
                                groupSymbolsRemaining = 50;
                                sbyte nextSelectorId = blockEntry.Selector[currentSelectorIndex];
                                currentGroupMinLength = blockEntry.HuffmanMinLengths[nextSelectorId];
                                currentGroupLimits = blockEntry.HuffmanLimits[nextSelectorId];
                                currentGroupPermutations = blockEntry.HuffmanPermutations[nextSelectorId];
                                currentGroupBaseValues = blockEntry.HuffmanBaseValues[nextSelectorId];
                            }

                            groupSymbolsRemaining -= 1;
                            int innerBitLength = currentGroupMinLength;
                            int innerCode;
                            sbyte innerNextBit;

                            for (innerCode = GetBits(innerBitLength, blockEntry); innerCode > currentGroupLimits[innerBitLength]; innerCode = innerCode << 1 | (byte)innerNextBit)
                            {
                                innerBitLength += 1;
                                innerNextBit = GetBit(blockEntry);
                            }

                            nextSym = currentGroupPermutations[innerCode - currentGroupBaseValues[innerBitLength]];
                        } while (nextSym == 0 || nextSym == 1);

                        runRepeatCount += 1;
                        sbyte decodedOutputByte = (sbyte)blockEntry.SequenceToSymbol[blockEntry.MoveToFrontBuffer[blockEntry.GroupPositions[0]] & 0xff];
                        blockEntry.SymbolFrequencyTable[decodedOutputByte & 0xff] += runRepeatCount;

                        for (; runRepeatCount > 0; runRepeatCount -= 1)
                        {
                            BZip2BlockEntry.TransformVector[decodedSymbolCount] = decodedOutputByte & 0xff;
                            decodedSymbolCount += 1;
                        }
                    }
                    else
                    {
                        int symbolTableOffset = nextSym - 1;
                        sbyte moveToFrontValue;

                        if (symbolTableOffset < 16)
                        {
                            int groupBaseIndex = blockEntry.GroupPositions[0];
                            moveToFrontValue = (sbyte)blockEntry.MoveToFrontBuffer[groupBaseIndex + symbolTableOffset];

                            for (; symbolTableOffset > 3; symbolTableOffset -= 4)
                            {
                                int shiftIndex = groupBaseIndex + symbolTableOffset;
                                blockEntry.MoveToFrontBuffer[shiftIndex] = blockEntry.MoveToFrontBuffer[shiftIndex - 1];
                                blockEntry.MoveToFrontBuffer[shiftIndex - 1] = blockEntry.MoveToFrontBuffer[shiftIndex - 2];
                                blockEntry.MoveToFrontBuffer[shiftIndex - 2] = blockEntry.MoveToFrontBuffer[shiftIndex - 3];
                                blockEntry.MoveToFrontBuffer[shiftIndex - 3] = blockEntry.MoveToFrontBuffer[shiftIndex - 4];
                            }

                            for (; symbolTableOffset > 0; symbolTableOffset -= 1)
                            {
                                blockEntry.MoveToFrontBuffer[groupBaseIndex + symbolTableOffset] = blockEntry.MoveToFrontBuffer[groupBaseIndex + symbolTableOffset - 1];
                            }

                            blockEntry.MoveToFrontBuffer[groupBaseIndex] = moveToFrontValue;
                        }
                        else
                        {
                            int blockGroupRow = symbolTableOffset / 16;
                            int blockGroupCol = symbolTableOffset % 16;
                            int blockGroupPos = blockEntry.GroupPositions[blockGroupRow] + blockGroupCol;
                            moveToFrontValue = (sbyte)blockEntry.MoveToFrontBuffer[blockGroupPos];

                            for (; blockGroupPos > blockEntry.GroupPositions[blockGroupRow]; blockGroupPos -= 1)
                            {
                                blockEntry.MoveToFrontBuffer[blockGroupPos] = blockEntry.MoveToFrontBuffer[blockGroupPos - 1];
                            }

                            blockEntry.GroupPositions[blockGroupRow] += 1;

                            for (; blockGroupRow > 0; blockGroupRow -= 1)
                            {
                                blockEntry.GroupPositions[blockGroupRow] -= 1;
                                blockEntry.MoveToFrontBuffer[blockEntry.GroupPositions[blockGroupRow]] = blockEntry.MoveToFrontBuffer[blockEntry.GroupPositions[blockGroupRow - 1] + 16 - 1];
                            }

                            blockEntry.GroupPositions[0] -= 1;
                            blockEntry.MoveToFrontBuffer[blockEntry.GroupPositions[0]] = moveToFrontValue;

                            if (blockEntry.GroupPositions[0] == 0)
                            {
                                int rebuildBufferIndex = 4095;

                                for (int rebuildGroupRow = 15; rebuildGroupRow >= 0; rebuildGroupRow -= 1)
                                {
                                    for (int rebuildGroupCol = 15; rebuildGroupCol >= 0; rebuildGroupCol -= 1)
                                    {
                                        blockEntry.MoveToFrontBuffer[rebuildBufferIndex] = blockEntry.MoveToFrontBuffer[blockEntry.GroupPositions[rebuildGroupRow] + rebuildGroupCol];
                                        rebuildBufferIndex -= 1;
                                    }

                                    blockEntry.GroupPositions[rebuildGroupRow] = rebuildBufferIndex + 1;
                                }
                            }
                        }

                        blockEntry.SymbolFrequencyTable[blockEntry.SequenceToSymbol[moveToFrontValue & 0xff] & 0xff] += 1;
                        BZip2BlockEntry.TransformVector[decodedSymbolCount] = blockEntry.SequenceToSymbol[moveToFrontValue & 0xff] & 0xff;
                        decodedSymbolCount += 1;

                        if (groupSymbolsRemaining == 0)
                        {
                            currentSelectorIndex += 1;
                            groupSymbolsRemaining = 50;
                            sbyte updateSelectorId = blockEntry.Selector[currentSelectorIndex];
                            currentGroupMinLength = blockEntry.HuffmanMinLengths[updateSelectorId];
                            currentGroupLimits = blockEntry.HuffmanLimits[updateSelectorId];
                            currentGroupPermutations = blockEntry.HuffmanPermutations[updateSelectorId];
                            currentGroupBaseValues = blockEntry.HuffmanBaseValues[updateSelectorId];
                        }

                        groupSymbolsRemaining -= 1;
                        int outerBitLength = currentGroupMinLength;
                        int outerCode;
                        sbyte outerNextBit;

                        for (outerCode = GetBits(outerBitLength, blockEntry); outerCode > currentGroupLimits[outerBitLength]; outerCode = outerCode << 1 | (byte)outerNextBit)
                        {
                            outerBitLength += 1;
                            outerNextBit = GetBit(blockEntry);
                        }

                        nextSym = currentGroupPermutations[outerCode - currentGroupBaseValues[outerBitLength]];
                    }
                }

                blockEntry.RunLength = 0;
                blockEntry.LastOutputByte = 0;
                blockEntry.CumulativeCounts[0] = 0;

                for (int tabFillIndex = 1; tabFillIndex <= 256; tabFillIndex += 1)
                {
                    blockEntry.CumulativeCounts[tabFillIndex] = blockEntry.SymbolFrequencyTable[tabFillIndex - 1];
                }

                for (int cumulativeIndex = 1; cumulativeIndex <= 256; cumulativeIndex += 1)
                {
                    blockEntry.CumulativeCounts[cumulativeIndex] += blockEntry.CumulativeCounts[cumulativeIndex - 1];
                }

                for (int transformIndex = 0; transformIndex < decodedSymbolCount; transformIndex += 1)
                {
                    int transformByte = BZip2BlockEntry.TransformVector[transformIndex] & 0xff;
                    BZip2BlockEntry.TransformVector[blockEntry.CumulativeCounts[transformByte & 0xff]] |= transformIndex << 8;
                    blockEntry.CumulativeCounts[transformByte & 0xff] += 1;
                }

                blockEntry.LinkedListNode = BZip2BlockEntry.TransformVector[blockEntry.OriginPointer] >> 8;
                blockEntry.SymbolIndex = 0;
                blockEntry.LinkedListNode = BZip2BlockEntry.TransformVector[blockEntry.LinkedListNode];
                blockEntry.CurrentByteValue = blockEntry.LinkedListNode & 0xff;
                blockEntry.LinkedListNode >>= 8;
                blockEntry.SymbolIndex += 1;
                blockEntry.LastSymbolIndex = decodedSymbolCount;
                DecodeRunLength(blockEntry);

                hasMoreBlocks = false;

                if (blockEntry.SymbolIndex == blockEntry.LastSymbolIndex + 1 && blockEntry.RunLength == 0)
                {
                    hasMoreBlocks = true;
                }
            }
        }

        private static sbyte GetUByte(BZip2BlockEntry blockEntry) => (sbyte)GetBits(8, blockEntry);

        private static sbyte GetBit(BZip2BlockEntry blockEntry) => (sbyte)GetBits(1, blockEntry);

        private static int GetBits(int bitCount, BZip2BlockEntry blockEntry)
        {
            while (true)
            {
                if (blockEntry.BitsAvailable >= bitCount)
                {
                    int value = blockEntry.CurrentBitWord >> blockEntry.BitsAvailable - bitCount & (1 << bitCount) - 1;
                    blockEntry.BitsAvailable -= bitCount;

                    return value;
                }

                blockEntry.CurrentBitWord = blockEntry.CurrentBitWord << 8 | blockEntry.InputBuffer[blockEntry.Offset] & 0xff;
                blockEntry.BitsAvailable += 8;
                blockEntry.Offset += 1;
                blockEntry.CompressedSize -= 1;
                blockEntry.BytesReadLow += 1;

                if (blockEntry.BytesReadLow == 0)
                {
                    blockEntry.BytesReadHigh += 1;
                }
            }
        }

        private static void CreateMaps(BZip2BlockEntry blockEntry)
        {
            blockEntry.ActiveSymbolCount = 0;

            for (int symbolIndex = 0; symbolIndex < 256; symbolIndex += 1)
            {
                if (blockEntry.InUse[symbolIndex])
                {
                    blockEntry.SequenceToSymbol[blockEntry.ActiveSymbolCount] = (byte)symbolIndex;
                    blockEntry.ActiveSymbolCount += 1;
                }
            }
        }

        private static void CreateDecodeTables(int[] limit, int[] baseValues, int[] permutations, sbyte[] codeLengths, int minimumLength, int maximumLength, int alphabetSize)
        {
            int permIndex = 0;

            for (int lengthLevel = minimumLength; lengthLevel <= maximumLength; lengthLevel += 1)
            {
                for (int symbolScanIndex = 0; symbolScanIndex < alphabetSize; symbolScanIndex += 1)
                {
                    if (codeLengths[symbolScanIndex] == lengthLevel)
                    {
                        permutations[permIndex] = symbolScanIndex;
                        permIndex += 1;
                    }
                }
            }

            for (int baseZeroIndex = 0; baseZeroIndex < 23; baseZeroIndex += 1)
            {
                baseValues[baseZeroIndex] = 0;
            }

            for (int baseSetupIndex = 0; baseSetupIndex < alphabetSize; baseSetupIndex += 1)
            {
                baseValues[codeLengths[baseSetupIndex] + 1] += 1;
            }

            for (int baseCumulIndex = 1; baseCumulIndex < 23; baseCumulIndex += 1)
            {
                baseValues[baseCumulIndex] += baseValues[baseCumulIndex - 1];
            }

            for (int limitZeroIndex = 0; limitZeroIndex < 23; limitZeroIndex += 1)
            {
                limit[limitZeroIndex] = 0;
            }

            int limitAccumulator = 0;

            for (int limitFillIndex = minimumLength; limitFillIndex <= maximumLength; limitFillIndex += 1)
            {
                limitAccumulator += baseValues[limitFillIndex + 1] - baseValues[limitFillIndex];
                limit[limitFillIndex] = limitAccumulator - 1;
                limitAccumulator <<= 1;
            }

            for (int baseAdjustIndex = minimumLength + 1; baseAdjustIndex <= maximumLength; baseAdjustIndex += 1)
            {
                baseValues[baseAdjustIndex] = (limit[baseAdjustIndex - 1] + 1 << 1) - baseValues[baseAdjustIndex];
            }
        }
    }
}