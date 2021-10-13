using System;

namespace wvw_autodemo
{
    public class Utils
    {
        public static MainForm Form;

        public static void Init(MainForm newForm)
        {
            Form = newForm;
        }

        public static void Log(string text)
        {
            Form.LogOutput.AppendText(Environment.NewLine + text);
        }

        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }
}