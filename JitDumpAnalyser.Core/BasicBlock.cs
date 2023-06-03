namespace JitDumpAnalyser.Core;

public class BasicBlock
{
    public BasicBlock? bbNext; // next BB in ascending PC offset order
    public BasicBlock? bbPrev;
    public BasicBlockFlags flags;
    public uint BBnum;
    public uint bbRefs;
    public int weight;
    public BBjumpKinds bbJumpKind;
    //union {
    //    unsigned    bbJumpOffs; // PC offset (temporary only)
    //    BasicBlock* bbJumpDest; // basic block
    //    BBswtDesc*  bbJumpSwt;  // switch descriptor
    //};

    // Statement* bbStmtList;
    // EntryState* bbEntryState; // verifier tracked state of all entries in stack.

    //union {
    //    unsigned bbStkTempsIn;       // base# for input stack temps
    //    int      bbCountSchemaIndex; // schema index for count instrumentation
    //};

    //union {
    //    unsigned bbStkTempsOut;          // base# for output stack temps
    //    int      bbHistogramSchemaIndex; // schema index for histogram instrumentation
    //};
    public ushort bbTryIndex;
    public ushort bbHndIndex;
    public uint bbCatchTyp;
    //public int @try;
    //public int EHRegion;

    //union {
    //    unsigned short bbStkDepth; // stack depth on entry
    //    unsigned short bbFPinVars; // number of inner enregistered FP vars
    //};

    // FlowEdge* bbPreds;
    //public int preds;

    // BlockSet bbReach; // Set of all blocks that can reach this one
    // void* bbSparseCountInfo; // Used early on by fgIncorporateEdgeCounts

    public uint bbPreorderNum;  // the block's  preorder number in the graph (1...fgMaxBBNum]
    public uint bbPostorderNum; // the block's postorder number in the graph (1...fgMaxBBNum]

    public uint bbCodeOffs;    // IL offset of the beginning of the block
    public uint bbCodeOffsEnd; // IL offset past the end of the block. Thus, the [bbCodeOffs..bbCodeOffsEnd)
                               // range is not inclusive of the end offset. The count of IL bytes in the block
                               // is bbCodeOffsEnd - bbCodeOffs, assuming neither are BAD_IL_OFFSET.
    //public int lp_IL_range;

    // From DUMP file
    public int BBid;
    public int @ref;
    public int hnd;
    public int @jump;
    public string SourceString { get; set; } = null!;

    public override string ToString()
    {
        return SourceString;
    }
}
