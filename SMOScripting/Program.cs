/*
 * Created by: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 04/24/2020
 * Time: 10:27 PM
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SMOScripting
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScriptForm());
        }
    }
}
