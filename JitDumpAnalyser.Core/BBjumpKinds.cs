namespace JitDumpAnalyser.Core;

public enum BBjumpKinds : byte
{
    BBJ_EHFINALLYRET,// block ends with 'endfinally' (for finally)
    BBJ_EHFAULTRET,  // block ends with 'endfinally' (IL alias for 'endfault') (for fault)
    BBJ_EHFILTERRET, // block ends with 'endfilter'
    BBJ_EHCATCHRET,  // block ends with a leave out of a catch (only #if defined(FEATURE_EH_FUNCLETS))
    BBJ_THROW,       // block ends with 'throw'
    BBJ_RETURN,      // block ends with 'ret'
    BBJ_NONE,        // block flows into the next one (no jump)
    BBJ_ALWAYS,      // block always jumps to the target
    BBJ_LEAVE,       // block always jumps to the target, maybe out of guarded region. Only used until importing.
    BBJ_CALLFINALLY, // block always calls the target finally
    BBJ_COND,        // block conditionally jumps to the target
    BBJ_SWITCH,      // block ends with a switch statement

    BBJ_COUNT
};