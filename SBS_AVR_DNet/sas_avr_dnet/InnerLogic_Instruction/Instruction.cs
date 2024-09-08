
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace sas_avr_dnet.InnerLogic_Instruction
{
    internal class Instruction
    {
        internal string Mnimonic;
        internal List<OpPart> OpParts = new List<OpPart>();

        internal Instruction(string mnimonic)
        {
            Mnimonic = mnimonic;
        }

        internal Instruction(Instruction copySrc)
        {
            Mnimonic = new string(copySrc.Mnimonic);
            foreach (OpPart copySrcPart in copySrc.OpParts)
            {
                OpParts.Add(new OpPart(copySrcPart));
            }
        }

        internal void AddOpPart(uint startDigit, uint digitNum, string typeStr)
        {
            var type = Enum.Parse(typeof(OpPartType), typeStr);

            int idx;
            idx = serchOppartIdx((OpPartType)type);
            if (idx == -1)
            {
                var newOpPart = new OpPart((OpPartType)type);
                newOpPart.AddPart(startDigit, digitNum, (OpPartType)type);
                OpParts.Add(newOpPart);
            }
            else
            {
                OpParts[idx].AddPart(startDigit, digitNum, (OpPartType)type);
            }
        }

        internal void SetOppartBinary(int value, OpPartType type)
        {
            foreach (OpPart opPart in OpParts)
            {
                if (opPart.Type == type)
                {
                    opPart.SetBinary(value);
                    return;
                }
            }
            throw new ArgumentException("typeに対応する命令引数はありません。");
        }
        internal void SetOppartBinary(string binaryStr, OpPartType type)
        {
            foreach (OpPart opPart in OpParts)
            {
                if (opPart.Type == type)
                {
                    opPart.SetBinary(binaryStr);
                    return;
                }
            }
            throw new ArgumentException("typeに対応する命令引数はありません。");
        }

        private int serchOppartIdx(OpPartType target_type)
        {
            for (int i = 0; i < OpParts.Count; i++)
            {
                if (target_type == OpParts[i].Type)
                    return i;
            }
            return -1;
        }

        internal string ToHexText()
        {
            uint value = Convert.ToUInt32(ToBinText(), 2);
            string hexStr = Convert.ToString(value, 16);
            if (hexStr.Length == 4 || hexStr.Length == 8)
            {
                return hexStr;
            }
            else if (hexStr.Length < 4)
            {
                return hexStr.PadLeft(4, '0');
            }
            else if (hexStr.Length < 8)
            {
                return hexStr.PadLeft(8, '0');
            }
            throw new ArgumentException("１６進変換後の桁数が大きすぎます。");
        }

        internal string ToBinText()
        {
            StringBuilder hexTextBld = new StringBuilder(32);
            while (true)
            {
                int[] opPartInnerIdx;
                opPartInnerIdx = serachOpPartInnerIdxSet(hexTextBld.Length);
                if (opPartInnerIdx[0] == -1)
                    break;
                hexTextBld.Append(OpParts[opPartInnerIdx[0]].BinaryText[opPartInnerIdx[1]]);
            }
            if (hexTextBld.Length == 16 || hexTextBld.Length == 32)
            {
                return hexTextBld.ToString();
            }
            else
            {
                throw new ArgumentException("命令バイナリの桁数が、AVR仕様の桁数と異なっています");
            }
        }
        private int[] serachOpPartInnerIdxSet(int target_opPartInnerIdx)
        {
            int[] ret = new int[2];
            for (int i = 0; i < OpParts.Count; i++)
            {
                for (int j = 0; j < OpParts[i].StartDigit.Count; j++)
                {
                    if (OpParts[i].StartDigit[j] == target_opPartInnerIdx)
                    {
                        ret[0] = i;
                        ret[1] = j;
                        return ret;
                    }
                }
            }
            ret[0] = -1;
            ret[1] = -1;
            return ret;
        }

        private int calcTotalBinaryDigitNum()
        {
            int total = 0;
            foreach (OpPart oppart in OpParts)
            {
                total += oppart.getTotalDigitNum();
            }
            return total;
        }
    }
}
