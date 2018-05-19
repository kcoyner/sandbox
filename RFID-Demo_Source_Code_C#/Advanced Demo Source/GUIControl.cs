using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    class GUIControl
    {
        //add an item to a ListView
        public delegate void ListViewAddItem(ListView l, string s);
        public static void ListViewAddItem1(ListView l, string s)
        {

            l.Items.Add(new ListViewItem(s));
        }
        public static void IListViewAddItem(Control form, ListView l, string s)
        {

            if (l.InvokeRequired)
            {
                ListViewAddItem LVAI = new ListViewAddItem(GUIControl.ListViewAddItem1);
                form.Invoke(LVAI, new Object[] { l, s });
            }
            else
            {
                l.Items.Add(new ListViewItem(s));
            }
        }

        //remove an item from a ListView
        public delegate void ListViewRemoveItem(ListView l, string s);
        public static void ListViewRemoveItem1(ListView l, string s)
        {

            for (int i = 0; i < l.Items.Count; i++)
            {
                if (l.Items[i].Text.Contains(s))
                {
                    l.Items[i].Remove();
                    break;
                }
            }
        }
        public static void IListViewRemoveItem(Control form, ListView l, string s)
        {

            if (l.InvokeRequired)
            {
                ListViewRemoveItem LVAI = new ListViewRemoveItem(GUIControl.ListViewRemoveItem1);
                form.Invoke(LVAI, new Object[] { l, s });
            }
            else
            {
                for (int i = 0; i < l.Items.Count; i++)
                {
                    if (l.Items[i].Text.Contains(s))
                    {
                        l.Items[i].Remove();
                        break;
                    }
                }
            }
        }


        //Insert an item to a ListView

        public delegate void ListViewInsertItem(ListView l, int i, string s);
        public static void ListViewInsertItem1(ListView l, int i, string s)
        {
            l.Items.Insert(i, new ListViewItem(s));
        }
        public static void IListViewInsertItem(Control form, ListView l, int i, string s)
        {

            if (l.InvokeRequired)
            {
                ListViewInsertItem LVAI = new ListViewInsertItem(GUIControl.ListViewInsertItem1);
                form.Invoke(LVAI, new Object[] { l, i, s });
            }
            else
            {
                l.Items.Insert(i, new ListViewItem(s));
            }
        }

        //add an item to a ListView
        public delegate void ListBoxAddItem(ListBox l, string s);
        public static void ListBoxAddItem1(ListBox l, string s)
        {
            l.Items.Add(s);
        }
        public static void IListBoxAddItem(Control form, ListBox l, string s)
        {

            if (l.InvokeRequired)
            {
                ListBoxAddItem LBAI = new ListBoxAddItem(GUIControl.ListBoxAddItem1);
                form.Invoke(LBAI, new Object[] { l, s });
            }
            else
            {
                l.Items.Add(s);
            }
        }

        //enable or disable a control
        public delegate void ControlEnable(Control c, bool enable);
        public static void ControlEnable1(Control c, bool enable)
        {
            c.Enabled = enable;
        }
        public static void IControlEnable(Control form, Control c, bool enable)
        {

            if (c.InvokeRequired)
            {
                ControlEnable ce = new ControlEnable(GUIControl.ControlEnable1);
                form.Invoke(ce, new Object[] { c, enable });
            }
            else
            {
                c.Enabled = enable;
            }
        }

        //update a control's text
        public delegate void ControlSetText(Control c, string s);
        public static void ControlSetText1(Control c, string s)
        {
            c.Text = s;
        }
        public static void IControlSetText(Control form, Control c, string s)
        {

            if (c.InvokeRequired)
            {
                ControlSetText cst = new ControlSetText(GUIControl.ControlSetText1);
                form.Invoke(cst, new Object[] { c, s });
            }
            else
            {
                c.Text = s;
            }
        }

        //get count
        public delegate int ListViewItemCount(ListView l);
        public static int ListViewItemCount1(ListView l)
        {
            return l.Items.Count;
        }
        public static int IListViewItemCount(Control form, ListView l)
        {

            if (l.InvokeRequired)
            {
                ListViewItemCount LVIC = new ListViewItemCount(GUIControl.ListViewItemCount1);
                return (int)(form.Invoke(LVIC, new Object[] { l }));
            }
            else
            {
                return l.Items.Count;
            }
        }

    }
}
