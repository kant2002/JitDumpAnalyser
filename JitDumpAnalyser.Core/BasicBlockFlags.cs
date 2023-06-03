namespace JitDumpAnalyser.Core;

public enum BasicBlockFlags : ulong
{
    BBF_EMPTY = 0,

    BBF_IS_LIR = 1ul << 0, // Set if the basic block contains LIR (as opposed to HIR)
    BBF_MARKED = 1ul << 1, // BB marked  during optimizations
    BBF_REMOVED = 1ul << 2, // BB has been removed from bb-list
    BBF_DONT_REMOVE = 1ul << 3, // BB should not be removed during flow graph optimizations
    BBF_IMPORTED = 1ul << 4, // BB byte-code has been imported
    BBF_INTERNAL = 1ul << 5, // BB has been added by the compiler
    BBF_FAILED_VERIFICATION = 1ul << 6, // BB has verification exception
    BBF_TRY_BEG = 1ul << 7, // BB starts a 'try' block
    BBF_FUNCLET_BEG = 1ul << 8, // BB is the beginning of a funclet
    BBF_CLONED_FINALLY_BEGIN = 1ul << 9, // First block of a cloned finally region
    BBF_CLONED_FINALLY_END = 1ul << 10, // Last block of a cloned finally region
    BBF_HAS_NULLCHECK = 1ul << 11, // BB contains a null check
    BBF_HAS_SUPPRESSGC_CALL = 1ul << 12, // BB contains a call to a method with SuppressGCTransitionAttribute
    BBF_RUN_RARELY = 1ul << 13, // BB is rarely run (catch clauses, blocks with throws etc)
    BBF_LOOP_HEAD = 1ul << 14, // BB is the head of a loop
    BBF_LOOP_CALL0 = 1ul << 15, // BB starts a loop that sometimes won't call
    BBF_LOOP_CALL1 = 1ul << 16, // BB starts a loop that will always     call
    BBF_HAS_LABEL = 1ul << 17, // BB needs a label
    BBF_LOOP_ALIGN = 1ul << 18, // Block is lexically the first block in a loop we intend to align.
    BBF_HAS_ALIGN = 1ul << 19, // BB ends with 'align' instruction
    BBF_HAS_JMP = 1ul << 20, // BB executes a JMP instruction (instead of return)
    BBF_GC_SAFE_POINT = 1ul << 21, // BB has a GC safe point (a call).  More abstractly, BB does not require a
                                         // (further) poll -- this may be because this BB has a call, or, in some
                                         // cases, because the BB occurs in a loop, and we've determined that all
                                         // paths in the loop body leading to BB include a call.
    BBF_HAS_IDX_LEN = 1ul << 22, // BB contains simple index or length expressions on an SD array local var.
    BBF_HAS_MD_IDX_LEN = 1ul << 23, // BB contains simple index, length, or lower bound expressions on an MD array local var.
    BBF_HAS_MDARRAYREF = 1ul << 24, // Block has a multi-dimensional array reference
    BBF_HAS_NEWOBJ = 1ul << 25, // BB contains 'new' of an object type.

//#if defined(FEATURE_EH_FUNCLETS) && defined(TARGET_ARM)

    BBF_FINALLY_TARGET       = 1ul << 26, // BB is the target of a finally return: where a finally will return during
                                                // non-exceptional flow. Because the ARM calling sequence for calling a
                                                // finally explicitly sets the return address to the finally target and jumps
                                                // to the finally, instead of using a call instruction, ARM needs this to
                                                // generate correct code at the finally target, to allow for proper stack
                                                // unwind from within a non-exceptional call to a finally.

//#endif // defined(FEATURE_EH_FUNCLETS) && defined(TARGET_ARM)

    BBF_RETLESS_CALL = 1ul << 27, // BBJ_CALLFINALLY that will never return (and therefore, won't need a paired
                                        // BBJ_ALWAYS); see isBBCallAlwaysPair().
    BBF_LOOP_PREHEADER = 1ul << 28, // BB is a loop preheader block
    BBF_COLD = 1ul << 29, // BB is cold
    BBF_PROF_WEIGHT = 1ul << 30, // BB weight is computed from profile data
    BBF_KEEP_BBJ_ALWAYS = 1ul << 31, // A special BBJ_ALWAYS block, used by EH code generation. Keep the jump kind
                                           // as BBJ_ALWAYS. Used for the paired BBJ_ALWAYS block following the
                                           // BBJ_CALLFINALLY block, as well as, on x86, the final step block out of a
                                           // finally.
    BBF_HAS_CALL = 1ul << 32, // BB contains a call
    BBF_DOMINATED_BY_EXCEPTIONAL_ENTRY = 1ul << 33, // Block is dominated by exceptional entry.
    BBF_BACKWARD_JUMP = 1ul << 34, // BB is surrounded by a backward jump/switch arc
    BBF_BACKWARD_JUMP_SOURCE = 1ul << 35, // Block is a source of a backward jump
    BBF_BACKWARD_JUMP_TARGET = 1ul << 36, // Block is a target of a backward jump
    BBF_PATCHPOINT = 1ul << 37, // Block is a patchpoint
    BBF_PARTIAL_COMPILATION_PATCHPOINT = 1ul << 38, // Block is a partial compilation patchpoint
    BBF_HAS_HISTOGRAM_PROFILE = 1ul << 39, // BB contains a call needing a histogram profile
    BBF_TAILCALL_SUCCESSOR = 1ul << 40, // BB has pred that has potential tail call
    BBF_RECURSIVE_TAILCALL = 1ul << 41, // Block has recursive tailcall that may turn into a loop

    // The following are sets of flags.

    // Flags that relate blocks to loop structure.

    BBF_LOOP_FLAGS = BBF_LOOP_PREHEADER | BBF_LOOP_HEAD | BBF_LOOP_CALL0 | BBF_LOOP_CALL1 | BBF_LOOP_ALIGN,

    // Flags to update when two blocks are compacted

    BBF_COMPACT_UPD = BBF_GC_SAFE_POINT | BBF_HAS_JMP | BBF_HAS_IDX_LEN | BBF_HAS_MD_IDX_LEN | BBF_BACKWARD_JUMP | 
                      BBF_HAS_NEWOBJ | BBF_HAS_NULLCHECK | BBF_HAS_MDARRAYREF | BBF_LOOP_PREHEADER,

    // Flags a block should not have had before it is split.

    BBF_SPLIT_NONEXIST = BBF_LOOP_HEAD | BBF_LOOP_CALL0 | BBF_LOOP_CALL1 | BBF_RETLESS_CALL | BBF_LOOP_PREHEADER | BBF_COLD,

    // Flags lost by the top block when a block is split.
    // Note, this is a conservative guess.
    // For example, the top block might or might not have BBF_GC_SAFE_POINT,
    // but we assume it does not have BBF_GC_SAFE_POINT any more.

    BBF_SPLIT_LOST = BBF_GC_SAFE_POINT | BBF_HAS_JMP | BBF_KEEP_BBJ_ALWAYS | BBF_CLONED_FINALLY_END | BBF_RECURSIVE_TAILCALL,

    // Flags gained by the bottom block when a block is split.
    // Note, this is a conservative guess.
    // For example, the bottom block might or might not have BBF_HAS_NULLCHECK, but we assume it has BBF_HAS_NULLCHECK.
    // TODO: Should BBF_RUN_RARELY be added to BBF_SPLIT_GAINED ?

    BBF_SPLIT_GAINED = BBF_DONT_REMOVE | BBF_HAS_JMP | BBF_BACKWARD_JUMP | BBF_HAS_IDX_LEN | BBF_HAS_MD_IDX_LEN | BBF_PROF_WEIGHT | 
                       BBF_HAS_NEWOBJ | BBF_KEEP_BBJ_ALWAYS | BBF_CLONED_FINALLY_END | BBF_HAS_NULLCHECK | BBF_HAS_HISTOGRAM_PROFILE | BBF_HAS_MDARRAYREF,

    // Flags that must be propagated to a new block if code is copied from a block to a new block. These are flags that
    // limit processing of a block if the code in question doesn't exist. This is conservative; we might not
    // have actually copied one of these type of tree nodes, but if we only copy a portion of the block's statements,
    // we don't know (unless we actually pay close attention during the copy).

    BBF_COPY_PROPAGATE = BBF_HAS_NEWOBJ | BBF_HAS_NULLCHECK | BBF_HAS_IDX_LEN | BBF_HAS_MD_IDX_LEN | BBF_HAS_MDARRAYREF,
}