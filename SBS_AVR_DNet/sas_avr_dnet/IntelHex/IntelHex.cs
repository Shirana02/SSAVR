
using System.Text;

namespace sas_avr_dnet.IntelHex
{
    internal class IntelHex
    {
        const int RECORD_STARTCODE_CHAR = 1;
        const int RECORD_BYTECOUNT_CHAR = 2;
        const int RECORD_ADDRESS_CHAR = 4;
        const int RECORD_RECORDTYPE_CHAR = 2;
        const int RECORD_DATA_MAX_CHAR = 32;
        const int RECORD_CHECKSUM_CHAR = 2;

        static internal string ToIntelHex(string hexText)
        {
            List<string> records = ToIntelHexRecords(hexText);
            StringBuilder intelHexBld = new StringBuilder(1024);
            foreach (string record in records)
            {
                intelHexBld.AppendLine(record);
            }
            return intelHexBld.ToString();
        }

        static internal List<string> ToIntelHexRecords(string hexText)
        {
            int address = 0;
            string srcHexText = hexText;
            List<string> records = new List<string>();

            while (true)
            {
                if (srcHexText.Length == 0)
                    break;

                StringBuilder record = new StringBuilder(
                                                        RECORD_STARTCODE_CHAR
                                                        + RECORD_BYTECOUNT_CHAR
                                                        + RECORD_ADDRESS_CHAR
                                                        + RECORD_RECORDTYPE_CHAR
                                                        + RECORD_DATA_MAX_CHAR
                                                        + RECORD_CHECKSUM_CHAR
                                                    );
                string rawDataHex = getRawData(srcHexText);
                int byteCount = countByteSize(rawDataHex);
                string byteCountStr = Convert.ToString(byteCount, 16).PadLeft(2, '0');
                string addressStr = Convert.ToString(address, 16).PadLeft(4, '0');
                string recordTypeStr = "00"; //AVRではデータレコード00と、EndOfFileレコード01しかない。決め打ち。
                string dataStr = convertToLitleEndian(rawDataHex);

                record.Append(":");
                record.Append(byteCountStr);
                record.Append(addressStr);
                record.Append(recordTypeStr);
                record.Append(dataStr);

                string checkSumStr = calcCheckSum(record.ToString().Substring(1));
                record.Append(checkSumStr);

                records.Add(record.ToString());
                srcHexText = srcHexText.Remove(0, rawDataHex.Length);
                address += byteCount;
            }
            records.Add(":00000001FF"); //AVRでは、最後は必ずこの行で〆る。決め打ち。
            return records;
        }

        /// <summary>
        /// 最大RECORD_DATA_MAX_CHAR文字まで取得する。
        /// </summary>
        /// <param name="srcHexText"></param>
        /// <returns></returns>
        static private string getRawData(string srcHexText)
        {
            if (RECORD_DATA_MAX_CHAR <= srcHexText.Length)
            {
                return srcHexText.Substring(0, RECORD_DATA_MAX_CHAR);
            }
            else
            {
                return new string(srcHexText);
            }
        }
        static private string convertToLitleEndian(string srcHexText)
        {
            char[] copyBuffer = new char[2];
            StringBuilder ret = new StringBuilder(32);
            for (int i = 0; i < srcHexText.Length; i += 4)
            {
                ret.Append(srcHexText[i + 2]);
                ret.Append(srcHexText[i + 3]);
                ret.Append(srcHexText[i + 0]);
                ret.Append(srcHexText[i + 1]);
            }
            return ret.ToString();
        }
        static private string calcCheckSum(string srcHexText)
        {
            int dataSum = 0;
            for (int i = 0; i < srcHexText.Length; i += 2)
            {
                int byteValue = Convert.ToInt32(srcHexText.Substring(i, 2), 16);
                dataSum += byteValue;
            }
            int retInt = ~dataSum + 1;
            string ret = Convert.ToString(retInt, 16);
            if (2 < ret.Length)
            {
                ret = ret.Remove(0, ret.Length - 2);
            }
            return ret.PadLeft(2, '0');
        }
        static private int countByteSize(string srcHexText)
        {
            return srcHexText.Length / 2;
        }
    }
}
