using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

///FULVIO
namespace Rsx.Dumb
{

    public interface ICalculableRow
    {
        bool IsBusy { set; get; }
        bool ToDo { set; get; }

    }

    public static partial class RegEx
    {
        public static void DecomposeFormula(string formula, ref List<string> elements, ref List<string> moles)
        {
            Regex re = new Regex("[0-9]");
            string[] result = re.Split(formula);
            foreach (string s in result) if (!s.Equals(string.Empty)) elements.Add(s); // gives elements

            //NUMBERS
            Regex re2 = new Regex("[a-z]", RegexOptions.IgnoreCase);
            result = re2.Split(formula);
            foreach (string s in result) if (!s.Equals(string.Empty)) moles.Add(s); // gives moles
        }

        public static IList<string[]> StripComposition(string composition)
        {
            IList<string[]> ls = null;
            // if (Rsx.EC.IsNuDelDetch(this)) return ls;
            if (string.IsNullOrEmpty(composition)) return ls;

            string matCompo = composition.Trim();

            if (matCompo.Contains(';')) matCompo = matCompo.Replace(';', ')');///

            string[] strArray = null;
            if (matCompo.Contains(')')) strArray = matCompo.Split(')');
            else strArray = new string[] { matCompo };
            strArray = strArray.Where(o => !string.IsNullOrEmpty(o.Trim())).ToArray();

            ls = new List<string[]>();

            for (int index = 0; index < strArray.Length; index++)
            {
                string[] strArray2 = strArray[index].Trim().Split('(');
                string formula = strArray2[0].Trim().Replace("#", null);
                string quantity = strArray2[1].Trim();

                string[] formCompo = new string[] { formula, quantity };
                ls.Add(formCompo);
            }

            return ls;
        }

        /// <summary>
        /// Strips the formula into elements and moles
        /// </summary>
        /// <param name="ls"></param>
        public static string StripMoreComposition(ref IList<string[]> ls)
        {
            string buffer = string.Empty;
            //matSSF buffer will cointain the snippet for the Matrix Content in MatSSF

            foreach (string[] formulaQuantity in ls)
            {
                //to auxiliary store elements and moles
                List<string> elements = new List<string>();
                List<string> moles = new List<string>();

                //decomposes Al2O3 into Al 2 O 3  (element and mole)
                DecomposeFormula(formulaQuantity[0], ref elements, ref moles);

                //modified formula
                string modified_formula = string.Empty;
                for (int z = 0; z < elements.Count; z++)
                {
                    modified_formula += elements[z] + " ";
                    if (moles.Count != 0) modified_formula += moles[z] + " ";
                }
                //Decomposed into Al 2 O 3  100

                //full MATSSF Input Data for the provided Matrix Information
                buffer += modified_formula + "\n";
                buffer += formulaQuantity[1] + "\n";
            }

            return buffer;
        }
    }

    public class CalculateBase
    {
        public bool IsCalculating
        {
            get;
            set;
        }
        public  void Set(string path, EventHandler callBackMethod = null, Action<int> resetProg = null, EventHandler showProg = null)
        {
         
            _startupPath = path;

            _showProgress = showProg;

            _resetProgress = resetProg;

            _callBack = callBackMethod;

            if (_processTable == null) _processTable = new System.Collections.Hashtable();
        }



        protected internal static System.Collections.Hashtable _processTable;
        protected internal bool _bkgCalculation = false;
        protected internal EventHandler _callBack = null;
        protected internal Action<int> _resetProgress;
        protected internal EventHandler _showProgress;
        protected internal string _startupPath = string.Empty;
        protected static string CANCELLED = "Self-shielding calculations were cancelled!";
        protected static string CANCELLED_TITLE = "Cancelled";
        protected static string CLONING_ERROR_TITLE = "Code Cloning ERROR...";
        protected static string CLONING_OK = "Code cloning OK";
        protected static string CLONING_OK_TITLE = "Code cloned...";
        protected static string CMD = "cmd";
        protected static string DATA_NOT_OK = "Input data is NOT OK for ";
        protected static string DATA_OK = "Input data is OK for Sample ";
        protected static string DATA_OK_TITLE = "Checking data...";
        protected static string DONE = "DONE!";
        protected static string ERROR_SPEAK = "Sample calculations were cancelled because the proper input data is missing.\n"
            + "Please verify the sample data provided";

        protected static string ERROR_TITLE = "ERROR";
        protected static string ERRORS = "Some samples were not calculated because the proper input data is missing.\n"
            + "Please verify the sample data provided, such as: composition, dimensions and neutron source parameters.";

        protected static string EXE_EXT = ".exe";
        protected static string EXEFILE = "matssf2.exe";

        protected static string FINISHED_ALL = "Self-shielding calculations finished!";

        protected static string FINISHED_SAMPLE = "Finished calculations for sample ";

        protected static string FINISHED_TITLE = "Finished";

        /// <summary>
        /// The input MatSSF file extension
        /// </summary>
        protected static string INPUT_EXT = ".in";

        protected static string NOTHING_SELECTED = "Oops, nothing was selected!";

        /// <summary>
        /// The output MatSSF file extension
        /// </summary>
        protected static string OUTPUT_EXT = ".out";
        protected static string PROBLEMS_CLONING = "Problems when cloning code for ";
        protected static string RUNNING = "Process started...";
        protected static string RUNNING_TITLE = "Running...";
        public string StartupPath
        {
            get { return _startupPath; }
            set { _startupPath = value; }
        }
        public CalculateBase()
        {
        }
    }
}