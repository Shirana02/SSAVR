using System.Text.RegularExpressions;

namespace sas_avr_dnet
{
    internal class AsmArgFromat{
      static internal OpPartType MatchType(string argStr){
         var ret = matchType(argStr);
			if(ret == OpPartType.UnKnown)
            throw new ArgumentException("引数プレフィックスが不正です。");
         return ret;
      }

      static private OpPartType matchType(string argStr){
         if(Regex.IsMatch(argStr, @"^sr")){
            return OpPartType.SrcReg;
         }else if(Regex.IsMatch(argStr, @"^dr")){
            return OpPartType.DstReg;
         }else if(Regex.IsMatch(argStr, @"^ma")){
            return OpPartType.MemAddress;
         }else if(Regex.IsMatch(argStr, @"^ia")){
            return OpPartType.IoAddress;
         }else if(Regex.IsMatch(argStr, @"^im")){
            return OpPartType.ImmValue;
         }else if(Regex.IsMatch(argStr, @"^ro")){
            return OpPartType.RegBitOffset;
         }else if(Regex.IsMatch(argStr, @"^so")){
            return OpPartType.StatusBitOffset;
         }else if(Regex.IsMatch(argStr, @"^ao")){
            return OpPartType.AddressOffset;
         }else{
            return OpPartType.UnKnown;
         }
      }

      static internal int GetValue(string argStr){
         if(matchType(argStr) == OpPartType.UnKnown)
            throw new ArgumentException("引数プレフィックスが不正です");
         try {
            string valueStr = argStr.Remove(0,2);
            if(isBin(valueStr)) {
               valueStr = valueStr.Remove(valueStr.Length - 1,1);
               return Convert.ToInt32(valueStr,2);
            }
            else if(isHex(valueStr)) {
               valueStr = valueStr.Remove(valueStr.Length - 1,1);
               return Convert.ToInt32(valueStr,16);
            }
            else if(isDec(valueStr)) {
               return Convert.ToInt32(valueStr,10);
            }
         }
         catch(Exception ex) {
            throw new ArgumentException("引数の変換に失敗しました。", ex);
         }
			throw new ArgumentException("引数の変換に失敗しました。");
      }

      static private bool isBin(string valueStr){
         if(Regex.IsMatch(valueStr,@"b$")) {
            if(Regex.IsMatch(valueStr,@"^[01]+b$")){
					return true;
            }
            throw new ArgumentException("サフィックスと値が一致しません。：2進数に[01]以外の文字が含まれています。");
         }
         return false;
      }
      static private bool isDec(string valueStr){
			if(Regex.IsMatch(valueStr,@"^[0-9]+$")){
				return true;
			}
         return false;
      }
      static private bool isHex(string valueStr){
         if(Regex.IsMatch(valueStr,@"h$")) {
            if(Regex.IsMatch(valueStr,@"^[0-9a-eA-E]+h$")){
					return true;
            }
            throw new ArgumentException("サフィックスと値が一致しません。：16進数に[0~Eまたはe]以外の文字が含まれています。");
         }
         return false;
      }
    }
}
