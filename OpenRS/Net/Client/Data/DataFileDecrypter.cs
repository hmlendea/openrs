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
            sbyte currentRunByte = blockEntry.lastOutputByte;
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

            int outputDelta = blockEntry.bytesWrittenLow;
            blockEntry.bytesWrittenLow += initialOutput - remainingOutput;

            if (blockEntry.bytesWrittenLow < outputDelta)
            {
                blockEntry.bytesWrittenHigh += 1;
            }

            blockEntry.lastOutputByte = currentRunByte;
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
            int currentGroupMinLength = 0;
            int[] currentGroupLimits = null;
            int[] currentGroupBaseValues = null;
            int[] currentGroupPermutations = null;
            blockEntry.blockSize100k = 1;

            if (BZip2BlockEntry.transformVector is null)
            {
                BZip2BlockEntry.transformVector = new int[blockEntry.blockSize100k * 0x186a0];
            }

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
                blockEntry.blocksRead += 1;
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                GetUByte(blockEntry);
                sbyte randomisedBit = GetBit(blockEntry);
                blockEntry.isRandomised = randomisedBit != 0;

                if (blockEntry.isRandomised)
                {
                    Console.WriteLine("PANIC! RANDOMISED BLOCK!");
                }

                blockEntry.origPtr = 0;
                sbyte origPtrByte0 = GetUByte(blockEntry);
                blockEntry.origPtr = blockEntry.origPtr << 8 | origPtrByte0 & 0xff;
                sbyte origPtrByte1 = GetUByte(blockEntry);
                blockEntry.origPtr = blockEntry.origPtr << 8 | origPtrByte1 & 0xff;
                sbyte origPtrByte2 = GetUByte(blockEntry);
                blockEntry.origPtr = blockEntry.origPtr << 8 | origPtrByte2 & 0xff;

                for (int groupIndex = 0; groupIndex < 16; groupIndex += 1)
                {
                    sbyte groupActiveBit = GetBit(blockEntry);

                    if (groupActiveBit == 1)
                    {
                        blockEntry.symbolGroupFlags[groupIndex] = true;
                    }
                    else
                    {
                        blockEntry.symbolGroupFlags[groupIndex] = false;
                    }
                }

                for (int byteIndex = 0; byteIndex < 256; byteIndex += 1)
                {
                    blockEntry.inUse[byteIndex] = false;
                }

                for (int symbolGroupIndex = 0; symbolGroupIndex < 16; symbolGroupIndex += 1)
                {
                    if (blockEntry.symbolGroupFlags[symbolGroupIndex])
                    {
                        for (int symbolOffset = 0; symbolOffset < 16; symbolOffset += 1)
                        {
                            sbyte symbolActiveBit = GetBit(blockEntry);

                            if (symbolActiveBit == 1)
                            {
                                blockEntry.inUse[symbolGroupIndex * 16 + symbolOffset] = true;
                            }
                        }
                    }
                }

                CreateMaps(blockEntry);
                int alphaSize = blockEntry.inUseOffset + 2;
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

                    blockEntry.selectorMtf[selectorMtfIndex] = (sbyte)mtfPosition;
                }

                sbyte[] selectorPositions = new sbyte[6];

                for (sbyte positionIndex = 0; positionIndex < groupCount; positionIndex += 1)
                {
                    selectorPositions[positionIndex] = positionIndex;
                }

                for (int selectorDecodeIndex = 0; selectorDecodeIndex < selectorCount; selectorDecodeIndex += 1)
                {
                    sbyte mtfIndex = blockEntry.selectorMtf[selectorDecodeIndex];
                    sbyte selectedGroupId = selectorPositions[mtfIndex];

                    for (; mtfIndex > 0; mtfIndex -= 1)
                    {
                        selectorPositions[mtfIndex] = selectorPositions[mtfIndex - 1];
                    }

                    selectorPositions[0] = selectedGroupId;
                    blockEntry.selector[selectorDecodeIndex] = selectedGroupId;
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

                        blockEntry.huffmanCodeLengths[tableIndex][symbolPosition] = (sbyte)currentCodeLength;
                    }
                }

                for (int huffmanGroupIndex = 0; huffmanGroupIndex < groupCount; huffmanGroupIndex += 1)
                {
                    sbyte minimumCodeLength = 32;
                    int maximumCodeLength = 0;

                    for (int lengthScanIndex = 0; lengthScanIndex < alphaSize; lengthScanIndex += 1)
                    {
                        if (blockEntry.huffmanCodeLengths[huffmanGroupIndex][lengthScanIndex] > maximumCodeLength)
                        {
                            maximumCodeLength = blockEntry.huffmanCodeLengths[huffmanGroupIndex][lengthScanIndex];
                        }

                        if (blockEntry.huffmanCodeLengths[huffmanGroupIndex][lengthScanIndex] < minimumCodeLength)
                        {
                            minimumCodeLength = blockEntry.huffmanCodeLengths[huffmanGroupIndex][lengthScanIndex];
                        }
                    }

                    CreateDecodeTables(blockEntry.limit[huffmanGroupIndex], blockEntry.huffmanBaseValues[huffmanGroupIndex], blockEntry.huffmanPermutations[huffmanGroupIndex], blockEntry.huffmanCodeLengths[huffmanGroupIndex], minimumCodeLength, maximumCodeLength, alphaSize);
                    blockEntry.minLengths[huffmanGroupIndex] = minimumCodeLength;
                }

                int endOfStreamSymbol = blockEntry.inUseOffset + 1;
                int currentSelectorIndex = -1;
                int groupSymbolsRemaining = 0;

                for (int tabResetIndex = 0; tabResetIndex <= 255; tabResetIndex += 1)
                {
                    blockEntry.unzftab[tabResetIndex] = 0;
                }

                int mtfBufferIndex = 4095;

                for (int groupRowIndex = 15; groupRowIndex >= 0; groupRowIndex -= 1)
                {
                    for (int groupColIndex = 15; groupColIndex >= 0; groupColIndex -= 1)
                    {
                        blockEntry.moveToFrontBuffer[mtfBufferIndex] = (byte)(groupRowIndex * 16 + groupColIndex);
                        mtfBufferIndex -= 1;
                    }

                    blockEntry.groupPositions[groupRowIndex] = mtfBufferIndex + 1;
                }

                int decodedSymbolCount = 0;

                if (groupSymbolsRemaining == 0)
                {
                    currentSelectorIndex += 1;
                    groupSymbolsRemaining = 50;
                    sbyte selectorId = blockEntry.selector[currentSelectorIndex];
                    currentGroupMinLength = blockEntry.minLengths[selectorId];
                    currentGroupLimits = blockEntry.limit[selectorId];
                    currentGroupPermutations = blockEntry.huffmanPermutations[selectorId];
                    currentGroupBaseValues = blockEntry.huffmanBaseValues[selectorId];
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
                                sbyte nextSelectorId = blockEntry.selector[currentSelectorIndex];
                                currentGroupMinLength = blockEntry.minLengths[nextSelectorId];
                                currentGroupLimits = blockEntry.limit[nextSelectorId];
                                currentGroupPermutations = blockEntry.huffmanPermutations[nextSelectorId];
                                currentGroupBaseValues = blockEntry.huffmanBaseValues[nextSelectorId];
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
                        sbyte decodedOutputByte = (sbyte)blockEntry.seqToUnseq[blockEntry.moveToFrontBuffer[blockEntry.groupPositions[0]] & 0xff];
                        blockEntry.unzftab[decodedOutputByte & 0xff] += runRepeatCount;

                        for (; runRepeatCount > 0; runRepeatCount -= 1)
                        {
                            BZip2BlockEntry.transformVector[decodedSymbolCount] = decodedOutputByte & 0xff;
                            decodedSymbolCount += 1;
                        }
                    }
                    else
                    {
                        int symbolTableOffset = nextSym - 1;
                        sbyte moveToFrontValue;

                        if (symbolTableOffset < 16)
                        {
                            int groupBaseIndex = blockEntry.groupPositions[0];
                            moveToFrontValue = (sbyte)blockEntry.moveToFrontBuffer[groupBaseIndex + symbolTableOffset];

                            for (; symbolTableOffset > 3; symbolTableOffset -= 4)
                            {
                                int shiftIndex = groupBaseIndex + symbolTableOffset;
                                blockEntry.moveToFrontBuffer[shiftIndex] = blockEntry.moveToFrontBuffer[shiftIndex - 1];
                                blockEntry.moveToFrontBuffer[shiftIndex - 1] = blockEntry.moveToFrontBuffer[shiftIndex - 2];
                                blockEntry.moveToFrontBuffer[shiftIndex - 2] = blockEntry.moveToFrontBuffer[shiftIndex - 3];
                                blockEntry.moveToFrontBuffer[shiftIndex - 3] = blockEntry.moveToFrontBuffer[shiftIndex - 4];
                            }

                            for (; symbolTableOffset > 0; symbolTableOffset -= 1)
                            {
                                blockEntry.moveToFrontBuffer[groupBaseIndex + symbolTableOffset] = blockEntry.moveToFrontBuffer[groupBaseIndex + symbolTableOffset - 1];
                            }

                            blockEntry.moveToFrontBuffer[groupBaseIndex] = moveToFrontValue;
                        }
                        else
                        {
                            int blockGroupRow = symbolTableOffset / 16;
                            int blockGroupCol = symbolTableOffset % 16;
                            int blockGroupPos = blockEntry.groupPositions[blockGroupRow] + blockGroupCol;
                            moveToFrontValue = (sbyte)blockEntry.moveToFrontBuffer[blockGroupPos];

                            for (; blockGroupPos > blockEntry.groupPositions[blockGroupRow]; blockGroupPos -= 1)
                            {
                                blockEntry.moveToFrontBuffer[blockGroupPos] = blockEntry.moveToFrontBuffer[blockGroupPos - 1];
                            }

                            blockEntry.groupPositions[blockGroupRow] += 1;

                            for (; blockGroupRow > 0; blockGroupRow -= 1)
                            {
                                blockEntry.groupPositions[blockGroupRow] -= 1;
                                blockEntry.moveToFrontBuffer[blockEntry.groupPositions[blockGroupRow]] = blockEntry.moveToFrontBuffer[blockEntry.groupPositions[blockGroupRow - 1] + 16 - 1];
                            }

                            blockEntry.groupPositions[0] -= 1;
                            blockEntry.moveToFrontBuffer[blockEntry.groupPositions[0]] = moveToFrontValue;

                            if (blockEntry.groupPositions[0] == 0)
                            {
                                int rebuildBufferIndex = 4095;

                                for (int rebuildGroupRow = 15; rebuildGroupRow >= 0; rebuildGroupRow -= 1)
                                {
                                    for (int rebuildGroupCol = 15; rebuildGroupCol >= 0; rebuildGroupCol -= 1)
                                    {
                                        blockEntry.moveToFrontBuffer[rebuildBufferIndex] = blockEntry.moveToFrontBuffer[blockEntry.groupPositions[rebuildGroupRow] + rebuildGroupCol];
                                        rebuildBufferIndex -= 1;
                                    }

                                    blockEntry.groupPositions[rebuildGroupRow] = rebuildBufferIndex + 1;
                                }
                            }
                        }

                        blockEntry.unzftab[blockEntry.seqToUnseq[moveToFrontValue & 0xff] & 0xff] += 1;
                        BZip2BlockEntry.transformVector[decodedSymbolCount] = blockEntry.seqToUnseq[moveToFrontValue & 0xff] & 0xff;
                        decodedSymbolCount += 1;

                        if (groupSymbolsRemaining == 0)
                        {
                            currentSelectorIndex += 1;
                            groupSymbolsRemaining = 50;
                            sbyte updateSelectorId = blockEntry.selector[currentSelectorIndex];
                            currentGroupMinLength = blockEntry.minLengths[updateSelectorId];
                            currentGroupLimits = blockEntry.limit[updateSelectorId];
                            currentGroupPermutations = blockEntry.huffmanPermutations[updateSelectorId];
                            currentGroupBaseValues = blockEntry.huffmanBaseValues[updateSelectorId];
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

                blockEntry.runLength = 0;
                blockEntry.lastOutputByte = 0;
                blockEntry.cumulativeCounts[0] = 0;

                for (int tabFillIndex = 1; tabFillIndex <= 256; tabFillIndex += 1)
                {
                    blockEntry.cumulativeCounts[tabFillIndex] = blockEntry.unzftab[tabFillIndex - 1];
                }

                for (int cumulativeIndex = 1; cumulativeIndex <= 256; cumulativeIndex += 1)
                {
                    blockEntry.cumulativeCounts[cumulativeIndex] += blockEntry.cumulativeCounts[cumulativeIndex - 1];
                }

                for (int transformIndex = 0; transformIndex < decodedSymbolCount; transformIndex += 1)
                {
                    int transformByte = BZip2BlockEntry.transformVector[transformIndex] & 0xff;
                    BZip2BlockEntry.transformVector[blockEntry.cumulativeCounts[transformByte & 0xff]] |= transformIndex << 8;
                    blockEntry.cumulativeCounts[transformByte & 0xff] += 1;
                }

                blockEntry.linkedListNode = BZip2BlockEntry.transformVector[blockEntry.origPtr] >> 8;
                blockEntry.symbolIndex = 0;
                blockEntry.linkedListNode = BZip2BlockEntry.transformVector[blockEntry.linkedListNode];
                blockEntry.currentByteValue = blockEntry.linkedListNode & 0xff;
                blockEntry.linkedListNode >>= 8;
                blockEntry.symbolIndex += 1;
                blockEntry.lastSymbolIndex = decodedSymbolCount;
                DecodeRunLength(blockEntry);

                hasMoreBlocks = false;

                if (blockEntry.symbolIndex == blockEntry.lastSymbolIndex + 1 && blockEntry.runLength == 0)
                {
                    hasMoreBlocks = true;
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

            for (int symbolIndex = 0; symbolIndex < 256; symbolIndex += 1)
            {
                if (blockEntry.inUse[symbolIndex])
                {
                    blockEntry.seqToUnseq[blockEntry.inUseOffset] = (byte)symbolIndex;
                    blockEntry.inUseOffset += 1;
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