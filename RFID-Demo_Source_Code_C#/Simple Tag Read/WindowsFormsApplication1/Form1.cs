using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket MainSocket;
        List<byte> msg;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8900);
                IPAddress IP = IPAddress.Parse(textBoxReaderIP.Text);
                int Port = Convert.ToInt32(textBoxPort.Text);
                MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                MainSocket.Connect(IP, Port);
                toolStripStatusLabel1.Text = "Connected";
                StateObject state = new StateObject();
                MainSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnDataReceived), state);
            }

            catch (Exception ConnectE)
            {
                MessageBox.Show(ConnectE.Message);
            }
        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            StateObject state = (StateObject)asyn.AsyncState;
            int bytesRead = MainSocket.EndReceive(asyn);
            if (bytesRead > 0)
            {
                byte[] recvBytes = state.buffer;
                msg = new List<byte>();
                for (int i = 0; i < bytesRead; i++)
                {
                    msg.Add(recvBytes[i]);
                }

                int recordIndex;
                string record,tagID,status;
                do
                {
                    try
                    {
                        recordIndex = msg.IndexOf(13);
                        if ((recordIndex >= 0) && (msg[recordIndex + 1] == 13) && (msg[recordIndex + 2] < msg.Count - recordIndex))
                        {
                            record = "";
                            tagID = "";
                            status = "";
                            for (int i = recordIndex; i < msg[recordIndex + 2]; i++)
                            {
                                record = record + msg[i].ToString("X2");
                            }

                            status = record.Substring(record.Length - 4, 2);
                            tagID = record.Substring(12, record.Length - 16);

                            //print record
                            if (record.Length == msg[recordIndex + 2] * 2)
                            {
                                GUIControl.IListBoxAddItem(this, listBox1, "Record:" + record);
                                GUIControl.IListBoxAddItem(this, listBox1, "TagID:" + tagID + " " + "Status:" + status);
                                byte checksum = 0;
                                for (int i = 0; i < record.Length-1; i++)
                                {
                                    checksum = (byte)(checksum ^ Convert.ToByte(record[i]));
                                }
                                GUIControl.IListBoxAddItem(this, listBox1, "CheckSum:" + checksum);
                            }

                            //remove the printed record
                            msg.RemoveRange(recordIndex, msg[recordIndex + 2]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
                while (true);
            }
            MainSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnDataReceived), state);
        }

        private void buttonSetMode_Click(object sender, EventArgs e)
        {
            string Address = textBoxAddress.Text;
            byte Mode;
            if (radioButtonDirectMode.Checked == true)
            {
                Mode = 255;
            }
            else
            {
                Mode = 0;
            }
            byte[] Result = new byte[8];
            
            //Header
            Result[0] = 13;
            Result[1] = 13;

            //Length
            Result[2] = 8;

            //Command
            Result[3] = 160;

            //Address
            Result[4] = Convert.ToByte(Address.Substring(0,2),16);
            Result[5] = Convert.ToByte(Address.Substring(2,2),16);

            //Mode
            Result[6] = Mode;

            //XOR
            Result[7] = (byte)(Result[0] ^ Result[1] ^ Result[2] ^ Result[3] ^ Result[4] ^ Result[5] ^ Result[6]);
            MainSocket.Send(Result);
        }

        private class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 2048;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();

        }

        public class GUIControl
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

            //add an item to a ListBox
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

            //Keyboard Wedge
            public delegate void KeyboardWedge(string msg);
            public static void KeyboardWedge1(string msg)
            {
                SendKeys.SendWait(msg);
            }
            public static void IKeyboardWedge(Control form, string msg)
            {
                if (form.InvokeRequired)
                {
                    KeyboardWedge KW = new KeyboardWedge(KeyboardWedge1);
                    form.Invoke(KW, new Object[] { msg });
                }
                else
                {
                    SendKeys.SendWait(msg);
                }
            }
        }

    }  
}
