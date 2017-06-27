using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

///FULVIO
namespace Rsx.Dumb
{
   
        public static  class UIControl
        {
            /// <summary>
            /// Gets ToolStripTextBox control content as T-type
            /// </summary>
            /// <typeparam name="T">
            /// the type that you want to obtain from the control box Text Property
            /// </typeparam>
            /// <param name="mbox">the box = textbox, combobox / tooltipbox, tooltipcombobox</param>
            /// <returns></returns>
            public static T GetControlAs<T>(dynamic mbox)
            {
                // ToolStripTextBox mbox = (ToolStripTextBox)control;

                T mass = default(T);
                Type tipo = typeof(T);

                bool m = string.IsNullOrWhiteSpace(mbox.Text);

                if (tipo.Equals(typeof(double)))
                {
                    double massAux = Convert.ToDouble(mbox.Text);

                    mass = (T)Convert.ChangeType(massAux, typeof(T));
                }
                else if (tipo.Equals(typeof(string)))
                {
                    string massAux = mbox.Text.ToString();

                    mass = (T)Convert.ChangeType(massAux, typeof(T));
                }

                return mass;
            }

            public static void FillABox(ComboBox combo, ICollection<string> hs, bool clear, bool AddAsterisk)
            {
                combo.AutoCompleteMode = AutoCompleteMode.Suggest;
                combo.AutoCompleteSource = AutoCompleteSource.CustomSource;
                if (clear)
                {
                    combo.AutoCompleteCustomSource.Clear();
                    combo.Items.Clear();
                }
                if (hs != null && hs.Count() != 0)
                {
                    combo.Items.AddRange(hs.ToArray<string>());
                    combo.AutoCompleteCustomSource.AddRange(hs.ToArray<string>());
                    if (AddAsterisk)
                    {
                        combo.Items.Add("*");
                        combo.AutoCompleteCustomSource.Add("*");
                    }
                }
            }

            public static void FillABox(ToolStripComboBox combo, ICollection<string> hs, bool clear, bool AddAsterisk)
            {
                FillABox(combo.ComboBox, hs, clear, AddAsterisk);
            }

            public static WebBrowser NavigateTo(string text, string uri)
            {
                WebBrowser browser = new WebBrowser();
                Form any = new Form();
                any.Text = text;
                browser.Dock = DockStyle.Fill;
                any.Show();
                any.Controls.Add(browser);
                any.WindowState = FormWindowState.Maximized;

                browser.Navigate(uri);

                return browser;
            }

            public static IEnumerable<T> GetChildControls<T>(UserControl control)
            {
                IEnumerable<Control> ctrols = control.Controls.OfType<Control>();
                IEnumerable<Control> exts = null;
                while (ctrols.Count() != 0)
                {
                    if (exts == null) exts = ctrols.SelectMany(o => o.Controls.OfType<Control>()).ToList();
                    exts = exts.Union(ctrols.SelectMany(o => o.Controls.OfType<Control>())).ToList();
                    ctrols = ctrols.SelectMany(o => o.Controls.OfType<Control>());
                }
                return exts.OfType<T>();
            }
        }
    
}