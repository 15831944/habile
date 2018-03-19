using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logic_Reinf
{
    public partial class DebugerWindow : Form
    {
        public static DebugerWindow _form;

        public DebugerWindow()
        {
            InitializeComponent();

            _form = this;
        }

        public void print(string message)
        {
            txt_messages.AppendText(message + "\n");
        }

        public void clear()
        {
            txt_messages.Clear();
        }
    }

    public static class Debuger
    {
        public static void Print(string s)
        {
            DebugerWindow._form.print(s);
        }
    }
}