using sas_avr_dnet.InnerLogic_Instruction;
using System.Diagnostics;

namespace sas_avr_dnet.AsmCode
{
    internal class AsmCodeLoader
    {
        static internal void MakePrimitiveAsmCodeFile(string srcPath_AsmCodeFile, string dstPath_PrimitiveAsmCodeFile){
				string code;
				using(StreamReader sr = new StreamReader(srcPath_AsmCodeFile)){
					code = sr.ReadToEnd();
				}
				if(code == null)
					throw new ArgumentException("ソースファイルの読み込みに失敗しました。");
				using(StreamWriter sw = new StreamWriter(dstPath_PrimitiveAsmCodeFile)){
					sw.Write(code);
				}
        }
     
        static internal InstructionList FromPrimitiveAsmCode(string srcPath, InstructionList prototypeInstructions)
        {
            var instList = new List<Instruction>();
            using (StreamReader sr = new StreamReader(srcPath))
            {
                int i = 1;
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (line == null)
                        continue;
                    string[] lineList = line.Split(" ");
                    try
                    {
                        instList.Add(makeInstruction(lineList, prototypeInstructions));
                    }
                    catch (Exception ex)
                    {
                        string msg = string.Format("{0}行目のアセンブルに失敗しました。", i);
                        throw new Exception(msg, ex);
                    }
                    i++;
                }
            }
            return new InstructionList(instList);
        }
        static private Instruction makeInstruction(string[] lineList, InstructionList prototypeInstructions)
        {
            Instruction inst = prototypeInstructions.GetCopy(lineList[0]);
            for (int i = 1; i < lineList.Length; i++)
            {
                OpPartType argType = AsmArgFromat.MatchType(lineList[i]);
                int argValue = AsmArgFromat.GetValue(lineList[i]);
                inst.SetOppartBinary(argValue, argType);
            }
            return inst;
        }
    }
}
