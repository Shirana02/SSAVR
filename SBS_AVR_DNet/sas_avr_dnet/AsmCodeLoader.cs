
namespace sas_avr_dnet {
	internal class AsmCodeLoader {
		static internal InstructionList FromPrimitiveAsmCode_All(string path, InstructionList prototypeInstructions){
			var instList = new List<Instruction>();
			using(StreamReader sr = new StreamReader(path)){
				int i = 1;
				while(!sr.EndOfStream){
					string? line = sr.ReadLine();
					if(line == null)
						continue;
					string[] lineList = line.Split(" ");
					try
					{
						instList.Add(makeInstruction(lineList,prototypeInstructions));
					}catch(Exception ex){
						string msg = string.Format("{0}行目のアセンブルに失敗しました。",i);
						throw new Exception(msg, ex);
					}
					i++;
				}
			}
			return new InstructionList(instList);
		}
		static private Instruction makeInstruction(string[] lineList, InstructionList prototypeInstructions){
			Instruction inst = prototypeInstructions.GetCopy(lineList[0]);
			for(int i = 1; i < lineList.Length; i++){
				OpPartType argType = AsmArgFromat.MatchType(lineList[i]);
				int argValue = AsmArgFromat.GetValue(lineList[i]);
				inst.SetOppartBinary(argValue,argType);
			}
			return inst;
		}
	}
}
