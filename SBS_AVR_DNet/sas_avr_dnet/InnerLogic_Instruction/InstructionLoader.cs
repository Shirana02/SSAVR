
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace sas_avr_dnet.InnerLogic_Instruction
{
    internal class InstructionLoader
    {
        const int START_DIGIT_IDX = 0;
        const int DIGIT_NUM_IDX = 1;
        const int OPPART_TYPE_IDX = 2;

        static internal List<Instruction> FromCsv_All(string path)
        {
            var ret = new List<Instruction>();
            var lines = new List<List<List<string>>>();

            using (StreamReader sr = new StreamReader(path))
            {
                try
                {
                    lines = getInstrunctionLine_All(sr);
                }
                catch (ArgumentException argExc)
                {
                    throw new ArgumentException("ファイルの読み取りに失敗しました。アクセスエラーまたは構文エラーです。" + Environment.NewLine + argExc.Message);
                }
            }

            foreach (List<List<string>> instructionLines in lines)
            {
                Instruction instruction;
                try
                {
                    instruction = createInstruction(instructionLines);
                }
                catch (ArgumentException argExc)
                {
                    throw new ArgumentException("instructionオブジェクトの作成に失敗しました。\t" + "mnimonic:" + instructionLines[0][1] + Environment.NewLine + argExc.Message);
                }
                ret.Add(instruction);
            }
            return ret;
        }

        static private List<List<List<string>>> getInstrunctionLine_All(StreamReader sr)
        {
            var instructionLinesList = new List<List<List<string>>>();
            var instruction = new List<List<string>>();
            bool firstMnimonicLine = true;

            for (int i = 0; !sr.EndOfStream; i++)
            {
                var lineStr = sr.ReadLine();
                var lineList = new List<string>();

                if (lineStr == null)
                    continue;
                if (Regex.IsMatch(lineStr, @"^[,#]"))
                    continue;

                lineList = lineStr.Split(',').ToList();
                if (lineList[0] == "mnimonic")
                {
                    if (!firstMnimonicLine)
                    {
                        instructionLinesList.Add(instruction);
                        instruction = new List<List<string>>(); //参照だけ入れ替えて器は使いまわし
                    }
                    firstMnimonicLine = false;
                }
                instruction.Add(lineList);
            }

            instructionLinesList.Add(instruction);
            return instructionLinesList;
        }

        static private Instruction createInstruction(List<List<string>> instructionLines)
        {
            if (!verifyLine_DigitConsistency(instructionLines))
                throw new ArgumentException("命令を構成するOpPartの桁数に不整合があります。");
            StringBuilder opcodeStrBld = new StringBuilder(32);

            //0行目(i=0)がMnimonic行 [mnimonic,<value>]
            Instruction ret = new Instruction(instructionLines[0][1]);

            //1行目以降のOpPart行　[<startDigitValue>, <digitNumValue>, <opPartTypeValue>, ?<binaryString>]
            for (int i = 1; i < instructionLines.Count; i++)
            {
                ret.AddOpPart(uint.Parse(instructionLines[i][0]), uint.Parse(instructionLines[i][1]), instructionLines[i][2]);

                if (instructionLines[i][2] == OpPartType.Opcode.ToString())
                {
                    if (instructionLines[i].Count != 4)
                        throw new ArgumentException("Opcodeにバイナリ文字列が指定されていません。");
                    opcodeStrBld.Append(instructionLines[i][3]);
                }
            }
            ret.SetOppartBinary(opcodeStrBld.ToString(), OpPartType.Opcode);
            return ret;
        }

        static private bool verifyLine_DigitConsistency(List<List<string>> instructionLines)
        {
            uint currentDigit = 0;
            int digitCount = 0;
            List<List<uint>> uintLines = new List<List<uint>>();

            for (int i = 1; i < instructionLines.Count; i++)
            {
                digitCount += Convert.ToInt32(instructionLines[i][DIGIT_NUM_IDX]);
            }
            if (digitCount != 16 && digitCount != 32)
            {
                return false; //AVRの命令は16or32bitしかない。決め打ち。
            }

            for (int i = 1; i < instructionLines.Count; i++)
            {
                List<uint> uintLine = new List<uint>();
                foreach (string num in instructionLines[i])
                {
                    uint n;
                    if (uint.TryParse(num, out n))
                    {
                        uintLine.Add(n);
                    }
                }
                uintLines.Add(uintLine);
            }

            while (currentDigit < digitCount)
            {
                int idx = searchLine_AtStartDigit(currentDigit, uintLines);
                if (idx == -1)
                    return false;
                currentDigit += uintLines[idx][DIGIT_NUM_IDX];
            }
            if (currentDigit != digitCount)
                return false;
            return true;
        }

        static private int searchLine_AtStartDigit(uint target, List<List<uint>> src)
        {
            for (int i = 0; i < src.Count; i++)
            {
                if (target == src[i][START_DIGIT_IDX])
                    return i;
            }
            return -1;
        }

    }
}
