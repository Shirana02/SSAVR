namespace sas_avr_dnet.InnerLogic_Instruction
{
    internal enum OpPartType
    {
        Opcode,
        SrcReg, //Rr
        DstReg, //Rd
        MemAddress, //k
        IoAddress, //A
        ImmValue, //K
        RegBitOffset, //b
        StatusBitOffset, //s
        AddressOffset, //q
        UnKnown,
    }
}
