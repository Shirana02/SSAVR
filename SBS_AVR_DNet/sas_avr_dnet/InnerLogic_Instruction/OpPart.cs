using System.Text;
using System.Text.RegularExpressions;

namespace sas_avr_dnet.InnerLogic_Instruction
{
    internal class OpPart
    {
        internal List<uint> StartDigit;
        internal List<uint> DigitNum;
        internal List<string> BinaryText;
        internal OpPartType Type;

        internal OpPart(OpPart copySrc)
        {
            StartDigit = new List<uint>(copySrc.StartDigit);
            DigitNum = new List<uint>(copySrc.DigitNum);
            BinaryText = new List<string>(copySrc.BinaryText);
            Type = copySrc.Type;
        }

        internal OpPart(OpPartType type)
        {
            StartDigit = new List<uint>();
            DigitNum = new List<uint>();
            BinaryText = new List<string>();
            Type = type;
        }

        internal void AddPart(uint startDigit, uint digitNum, OpPartType type)
        {
            if (type != Type)
                throw new ArgumentException("OpPartTypeが一致しません");
            StartDigit.Add(startDigit);
            DigitNum.Add(digitNum);
        }

        internal int GetBinary_AsInt()
        {
            StringBuilder ret = new StringBuilder(32);
            foreach (string binaryText in BinaryText)
            {
                ret.Append(binaryText);
            }
            return Convert.ToInt32(ret.ToString(), 2);
        }

        internal void SetBinary(string binaryStr)
        {
            if (Regex.IsMatch(binaryStr, "[^0,1]+"))
                throw new ArgumentException("binaryStrにゼロイチ以外の文字列が含まれています。");
            int totalDigitNum = getTotalDigitNum();
            int zeroPaddingDigitNum = totalDigitNum - binaryStr.Length;
            if (zeroPaddingDigitNum < 0)
                throw new ArgumentException("代入値が桁数オーバーです");

            //命令内での２進値は上位桁ゼロ埋め
            StringBuilder binStrBld = new StringBuilder();
            for (; 0 < zeroPaddingDigitNum; zeroPaddingDigitNum--)
            {
                binStrBld.Append('0');
            }
            binStrBld.Append(binaryStr);
            binaryStr = binStrBld.ToString();

            BinaryText.Clear();
            int writenOffset = 0;
            foreach (uint digitNum in DigitNum)
            {
                BinaryText.Add(binaryStr.Substring(writenOffset, (int)digitNum));
                writenOffset += (int)digitNum;
            }
        }

        internal void SetBinary(int value)
        {
            string orgBinStr = Convert.ToString(value, 2);
            SetBinary(orgBinStr);
        }

        internal int getTotalDigitNum()
        {
            int ret = 0;
            foreach (int partDigitNum in DigitNum)
            {
                ret += partDigitNum;
            }
            return ret;
        }
    }
}
