using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Org.Mentalis.Network.ProxySocket;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace Multi_TCP
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
        }
        public class Socks_Member
        {
            public int stt { set; get; }
            public string socks_value { set; get; }
            public int use_time { set; get; }
            public string date_sock { set; get; }
            public int black_list { set; get; }
            public string use_sock { set; get; }
            public string pass_sock { set; get; }
            public int type_sock { set; get; }
            public int live { set; get; }
        }
        class Product
        {
            public string account { set; get; }        // tên
            public int runned { set; get; }       // giá
        }

        public class Registered
        {
            public string account { set; get; }        // tên
            public bool reg { set; get; }       // Check Đăng Ký
        }
        public class Gen_Thread
        {
            public Thread Is_Thread { set; get; }
            public int Num_Thread { set; get; }
        }
        public class prxy
        {
            public bool prxreturn { set; get; }        // proxy return true or false
            public ProxySocket prx_var { set; get; }       // giá
            
        }
        List<string> myListOfStrings = new List<string>();
        List<string> reg_list = new List<string>();
        List<Product> full_account = new List<Product>();
        List<Socks_Member> List_Socks = new List<Socks_Member>();
        //List<string> list_sock_live = new List<string>();
        //public string victim_ip = "222.252.26.68";
        //public int victim_port = 7666;
        public string victim_ip = "3.1.168.177";
        public int victim_port = 7103;
        public Random rnd = new Random();
        public string path_file_account = @"C:\Users\Administrator\Google Drive\AutoNox\GomTien\Account_Csharp.txt";
        public string path_sqlite_socks = "socksdb.sqlite";
        public string Url_socks = "http://14.177.239.126:1981/scan/scan_open_all.txt";
        SQLiteConnection _con = new SQLiteConnection();
        SQLiteDataReader db_socks;
        private void button1_Click(object sender, EventArgs e)
        {
            // create a new ProxySocket
            List<string> lines = new List<string>();
            List<Thread> threads = new List<Thread>();
            List<Gen_Thread> t_resign = new List<Gen_Thread>();
            string get_sock_cmd = "Select * From Socks5";
            string get_account_cmd = "Select * From Account";
            createConection();
            using (SQLiteCommand cmd = new SQLiteCommand(get_account_cmd, _con))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        // Set inv properties
                        Product item = new Product { account = rdr.GetString(1), runned = 0 };
                        full_account.Add(item);
                    }
                }
            }
            SQLiteCommand cmd_exc = new SQLiteCommand(get_sock_cmd, _con);
            SQLiteDataReader getall_sock = cmd_exc.ExecuteReader();
            int type_sock = 5;
            //string ip_include = "";
            int count_temp = 0;
            int i = 0;
            while (getall_sock.Read())
            {

                Socks_Member entity = new Socks_Member();
                entity.stt = getall_sock.GetInt32(0);
                entity.socks_value = getall_sock.GetString(1);
                if (getall_sock.IsDBNull(2))
                {
                    entity.use_time = 0;
                }
                else
                {
                    entity.use_time = getall_sock.GetInt32(2);
                }
                if (getall_sock.IsDBNull(3))
                {
                    entity.date_sock = "";
                }
                else
                {
                    entity.date_sock = getall_sock.GetString(3);
                }
                if (getall_sock.IsDBNull(4))
                {
                    entity.black_list = 0;
                }
                else
                {
                    entity.black_list = getall_sock.GetInt32(4);
                }
                if (getall_sock.IsDBNull(5))
                {
                    entity.use_sock = "";
                }
                else
                {
                    entity.use_sock = getall_sock.GetString(5);
                }
                if (getall_sock.IsDBNull(6))
                {
                    entity.pass_sock = "";
                }
                else
                {
                    entity.pass_sock = getall_sock.GetString(6);
                }
                entity.type_sock = getall_sock.GetInt32(7);
                entity.live = 0;
                // Set inv properties
                List_Socks.Add(entity);
            }
            if (Check_live_proxy.Checked)
            {
                for (i = 0; i < List_Socks.Count; i++)
                {
                    count_temp++;
                    Console.WriteLine("Check-Socks-Thread- " + count_temp);
                    type_sock = List_Socks[i].type_sock;
                    if (List_Socks[i].black_list >= 1)
                    {
                        continue;
                    }
                    if (List_Socks[i].use_time >= 4)
                    {
                        continue;
                    }
                    Thread th = new Thread(() =>
                    {
                        CheckLiveSock(List_Socks[i].socks_value, List_Socks[i].type_sock , List_Socks[i].use_sock, List_Socks[i].pass_sock);
                        //calling callback function
                    });
                    th.Start();
                    threads.Add(th);
                }

                for (i = 1; i < threads.Count - 1; i++)
                {
                    Thread.Sleep(100);
                    lb_proxy.Text = i.ToString();
                    Console.WriteLine("T-" + i + " is " + threads[i].IsAlive);
                    if (threads[i].IsAlive == true)
                    {
                        // MessageBox.Show("abc");
                        i = i - 1;
                        continue;
                    }
                }

            }
            int ccount_socks = 0;
            //int retry = 1;
            //bool re_round = false;
            //bool break_loop = false;
            //int cc_temp = 0;
            List<String> hoanthanh = new List<string>();
            List<int> dangchay = new List<int>();
            List<int> thatbai = new List<int>();
            List<int> chuachay = new List<int>();
            List<int> ngoailuong = new List<int>();
            i = 0;
            List<Registered> reg_acc = new List<Registered>();
            int total_reg = 100;
            while (hoanthanh.Count != total_reg)
            {
                Console.WriteLine("Reg_Step: " + i + "/" + total_reg);
                //bool passconnect = false;
                //bool dbpass = true;
                if (ccount_socks == List_Socks.Count -1)
                {
                    MessageBox.Show("Da Su Dung Het Sock Dang Ky");
                    break;
                }
                while (true)
                {
                    if (List_Socks[ccount_socks].use_time >= 4 || List_Socks[ccount_socks].black_list == 1)
                    {
                        ccount_socks++;
                    }
                    else
                    {
                        break;
                    }

                }
                Thread t_reg = new Thread(() =>
                {
                    Resign(List_Socks[ccount_socks].socks_value, List_Socks[ccount_socks].use_sock, List_Socks[ccount_socks].pass_sock, List_Socks[ccount_socks].type_sock);
                    //calling callback function
                });

                t_reg.Name = "Reg" + i;
                t_reg.Start();
                //t_resign.Add(t_reg);
                i++;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // create a new ProxySocket
            List<string> lines = new List<string>();
            List<Thread> threads = new List<Thread>();
            List<Gen_Thread> t_resign = new List<Gen_Thread>();
            string get_sock_cmd = "Select * From Socks5";
            string get_account_cmd = "Select * From Account";
            createConection();
            using (SQLiteCommand cmd = new SQLiteCommand(get_account_cmd, _con))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        // Set inv properties
                        Product item = new Product { account = rdr.GetString(1), runned = 0 };
                        full_account.Add(item);
                    }
                }
            }
            SQLiteCommand cmd_exc = new SQLiteCommand(get_sock_cmd, _con);
            SQLiteDataReader getall_sock = cmd_exc.ExecuteReader();
            int type_sock = 5;
            //string ip_include = "";
            int count_temp = 0;
            int i = 0;
            while (getall_sock.Read())
            {

                Socks_Member entity = new Socks_Member();
                entity.stt = getall_sock.GetInt32(0);
                entity.socks_value = getall_sock.GetString(1);
                if (getall_sock.IsDBNull(2))
                {
                    entity.use_time = 0;
                }
                else
                {
                    entity.use_time = getall_sock.GetInt32(2);
                }
                if (getall_sock.IsDBNull(3))
                {
                    entity.date_sock = "";
                }
                else
                {
                    entity.date_sock = getall_sock.GetString(3);
                }
                if (getall_sock.IsDBNull(4))
                {
                    entity.black_list = 0;
                }
                else
                {
                    entity.black_list = getall_sock.GetInt32(4);
                }
                if (getall_sock.IsDBNull(5))
                {
                    entity.use_sock = "";
                }
                else
                {
                    entity.use_sock = getall_sock.GetString(5);
                }
                if (getall_sock.IsDBNull(6))
                {
                    entity.pass_sock = "";
                }
                else
                {
                    entity.pass_sock = getall_sock.GetString(6);
                }
                entity.type_sock = getall_sock.GetInt32(7);
                // Set inv properties
                List_Socks.Add(entity);
            }
            if (Check_live_proxy.Checked)
            {
                for (i = 0; i < List_Socks.Count; i++)
                {
                    count_temp++;
                    Console.WriteLine("Check-Socks-Thread- " + count_temp);
                    type_sock = List_Socks[i].type_sock;
                    if (List_Socks[i].black_list >= 1)
                    {
                        continue;
                    }
                    if (List_Socks[i].use_time >= 4)
                    {
                        continue;
                    }
                    Thread th = new Thread(() =>
                    {
                        CheckLiveSock(List_Socks[i].socks_value, List_Socks[i].type_sock, List_Socks[i].use_sock, List_Socks[i].pass_sock);
                        //calling callback function
                    });
                    th.Start();
                    threads.Add(th);


                }

                for (i = 1; i < threads.Count - 1; i++)
                {
                    Thread.Sleep(100);
                    lb_proxy.Text = i.ToString();
                    Console.WriteLine("T-" + i + " is " + threads[i].IsAlive);
                    if (threads[i].IsAlive == true)
                    {
                        // MessageBox.Show("abc");
                        i = i - 1;
                        continue;
                    }
                }

            }
            //MessageBox.Show(List_Socks.Count.ToString());
            //int total_sock_live = 0;
            //for (int s =0;s < List_Socks.Count; s++)
            //{
            //    if (List_Socks[s].live == 1)
            //    {
            //        total_sock_live++;
            //    }
            //}    
            //MessageBox.Show(total_sock_live.ToString());
            //Console.WriteLine(myListOfStrings[0]);
            //foreach (string l in myListOfStrings)
            //{
            //    Console.WriteLine(l);
            //}
            //myListOfStrings Thread sau khi check live của socks sẽ thêm vào array này
            //full_account tổng số account
            //MessageBox.Show(full_account.Count.ToString());
            int ccount_socks = 0;
            int retry = 1;
            bool re_round = false;
            bool break_loop = false;
            int cc_temp = 0;
            List<String> hoanthanh = new List<string>();
            List<int> dangchay = new List<int>();
            List<int> thatbai = new List<int>();
            List<int> chuachay = new List<int>();
            List<int> ngoailuong = new List<int>();
            i = 0;
            while (hoanthanh.Count != full_account.Count)
            {
                //Console.WriteLine("Account - " + i + "/" + (full_account.Count -1));
                //if (retry > 0 && thatbai.Count > 0 && chuachay.Count == 0)
                //{
                //    MessageBox.Show("RS");
                //    if (cc_temp == thatbai.Count)
                //    {
                //        cc_temp = 0;
                //    }
                //    if (thatbai[cc_temp] != i)
                //    {
                //        continue;
                //    }
                //    cc_temp++;
                //}
                //if (i == 0)
                //{
                //    MessageBox.Show("1");
                //}
                if (i == (full_account.Count - 1) && break_loop == false)
                {
                    hoanthanh = new List<string>();
                    dangchay = new List<int>();
                    thatbai = new List<int>();
                    chuachay = new List<int>();
                    ngoailuong = new List<int>();
                    cc_temp = 0;
                    // int hoanthanh = 0, dangchay = 0, thatbai = 0,chuachay = 0;
                    for (int j = 0; j < full_account.Count; j++)
                    {
                        switch (full_account[j].runned)
                        {
                            case 1:
                                hoanthanh.Add(full_account[j].account);
                                break;
                            case 2:
                                dangchay.Add(j);
                                break;
                            case 3:
                                thatbai.Add(j);
                                break;
                            case 4:
                                ngoailuong.Add(j);
                                break;
                            case 0:
                                chuachay.Add(j);
                                break;
                        }
                    }
                    Thread.Sleep(45000);
                    Console.WriteLine("Chua Chay: ");
                    for (int m = 0; m < chuachay.Count; m++)
                    {
                        Console.Write(chuachay[m] + ", ");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Dang Chay: ");
                    //for (int m = 0; m < dangchay.Count; m++)
                    //{
                    //    //Console.Write(dangchay[m] + " - Thread : " + find_thread.Num_Thread + " IsLive=" + find_thread.Is_Thread.IsAlive + ", ");
                    //    Gen_Thread find_thread = t_resign.Find(c => c.Num_Thread == m);
                    //    if (find_thread.Is_Thread.IsAlive == false)
                    //    {
                    //        dangchay.Remove(m);
                    //        thatbai.Add(m);
                    //    }
                        
                        
                    //}
                    Console.WriteLine();
                    Console.WriteLine("BaoCao:retry = "+ retry + " TC=" + hoanthanh.Count + " DC=" + dangchay.Count + " CC=" + chuachay.Count + " TB= " + thatbai.Count + " NL= "+ ngoailuong.Count);
                    Console.WriteLine("Sock_Use: " + ccount_socks);
                    if ((retry%4) == 0)
                    {
                        Console.WriteLine("Retry :" + dangchay.Count);
                        Console.WriteLine("TB :");
                        for (int k = 0; k < dangchay.Count; k++)
                        {
                            Console.Write(", " + dangchay[k]);
                            thatbai.Add(dangchay[k]);
                            full_account[dangchay[k]].runned = 3;
                        }
                        dangchay = new List<int>();
                        Console.WriteLine(dangchay.Count);
                        dangchay.Clear();
                        Console.WriteLine(dangchay.Count);
                        Console.WriteLine("Add TB: " + thatbai.Count);
                    }
                    if ( dangchay.Count <= 2 && chuachay.Count <= 2 && thatbai.Count <= 2)
                    {
                        break;
                    }
                    i = 0;
                    re_round = true;
                    retry++;
                }
                if (retry > 0 && chuachay.Count > 0)
                {
                    if (cc_temp == chuachay.Count)
                    {
                        cc_temp = 0;
                    }
                    if (chuachay[cc_temp] != i)
                    {
                        //continue;
                    }
                    cc_temp++;
                }
                if (full_account[i].runned == 2 || full_account[i].runned == 1)
                {
                    i++;
                    continue;
                }
                if (full_account[i].runned != 1 && full_account[i].runned != 2)
                {
                    break_loop = false;
                }
                if (ccount_socks >= (List_Socks.Count - 2))
                {

                    if (break_loop == true)
                    {
                        MessageBox.Show("Tat Ca Account Da Duoc Chay");
                        break;
                    }
                    Thread.Sleep(20000);
                    ccount_socks = 0;
                }
                //bool passconnect = false;
                //int usetime = 0;
                //bool full_user = false;
                ////string stm = "Select * From Socks5 Where Socks = '" + List_Socks[ccount_socks].socks_value + "'";                
                ////SQLiteCommand cmd = new SQLiteCommand(stm, _con);
                ////SQLiteDataReader getall = cmd.ExecuteReader();
                //string username_tb = "";
                //string password_tb = "";
                while (true)
                {
                    if (List_Socks[ccount_socks].use_time >= 4 || List_Socks[ccount_socks].black_list == 1)
                    {
                        ccount_socks++;
                    }
                    else
                    {
                        break;
                    }

                }

                Thread t_reg = new Thread(() =>
                {

                    Reward_gift(i, List_Socks[ccount_socks].socks_value, List_Socks[ccount_socks].use_sock, List_Socks[ccount_socks].pass_sock, List_Socks[ccount_socks].type_sock);
                    //calling callback function
                });
                t_reg.Name = ccount_socks.ToString();
                t_reg.Start();
                Gen_Thread input = new Gen_Thread();
                input.Is_Thread = t_reg;
                input.Num_Thread = i;
                t_resign.Add(input);
                Console.WriteLine("Account - " + (i + 1) + "/" + full_account.Count);
                Console.WriteLine("Tong So Thread Da Chay: " + t_resign.Count.ToString());
                //if ((i + 1) != t_resign.Count)
                //{
                //    MessageBox.Show("9999 doa hong");
                //}    
                //thread_run.Text = t_resign.Count.ToString();
                ccount_socks++;
                if (re_round == true)
                {
                    re_round = false;
                    i = -1;
                }
                i++;
            }
            MessageBox.Show("Kiem Tra Tien Do Thread" + chuachay.Count.ToString());
            MessageBox.Show("Kiem Tra Tien Do Thread" + t_resign.Count.ToString());
            //Console.WriteLine("Running.");
            //for (int i = 0;i < t_resign.Count;i++)
            //{
            //    Thread.Sleep(100);
            //    Console.Write(".");
            //    if (t_resign[i].IsAlive) {
            //        i = 0;
            //    }
            //}    
            //MessageBox.Show(ds.Tables[0].ToString());
            //Thread t_reg = new Thread(() =>
            //{

            //    Reward();
            //    //calling callback function
            //});
            //t_reg.Start();

        }
        private void button3_Click(object sender, EventArgs e)
        {

            // create a new ProxySocket
            List<string> Boss_Gom = new List<string>();
            List<string> Children_account = new List<string>();
            List<Thread> threads = new List<Thread>();
            List<Gen_Thread> t_resign = new List<Gen_Thread>();
            string get_sock_cmd = "Select * From Socks5";
            string get_account_cmd = "Select * From Account Where Type <> 1";
            string get_account_boss_cmd = @"Select * From Account WHERE Type = 1;";
            createConection();
            using (SQLiteCommand cmd = new SQLiteCommand(get_account_boss_cmd, _con))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Boss_Gom.Add(rdr.GetString(1));
                    }
                }
            }
            using (SQLiteCommand cmd = new SQLiteCommand(get_account_cmd, _con))
            {
                using (SQLiteDataReader rdr1 = cmd.ExecuteReader())
                {
                    while (rdr1.Read())
                    {
                        // Set inv properties
                        Product item = new Product { account = rdr1.GetString(1), runned = 0 };
                        Children_account.Add(rdr1.GetString(1));
                    }
                }
            }
            SQLiteCommand cmd_exc = new SQLiteCommand(get_sock_cmd, _con);
            SQLiteDataReader getall_sock = cmd_exc.ExecuteReader();
            int type_sock = 5;
            //string ip_include = "";
            int count_temp = 0;
            while (getall_sock.Read())
            {

                Socks_Member entity = new Socks_Member();
                entity.stt = getall_sock.GetInt32(0);
                entity.socks_value = getall_sock.GetString(1);
                if (getall_sock.IsDBNull(2))
                {
                    entity.use_time = 0;
                }
                else
                {
                    entity.use_time = getall_sock.GetInt32(2);
                }
                if (getall_sock.IsDBNull(3))
                {
                    entity.date_sock = "";
                }
                else
                {
                    entity.date_sock = getall_sock.GetString(3);
                }
                if (getall_sock.IsDBNull(4))
                {
                    entity.black_list = 0;
                }
                else
                {
                    entity.black_list = getall_sock.GetInt32(4);
                }
                if (getall_sock.IsDBNull(5))
                {
                    entity.use_sock = "";
                }
                else
                {
                    entity.use_sock = getall_sock.GetString(5);
                }
                if (getall_sock.IsDBNull(6))
                {
                    entity.pass_sock = "";
                }
                else
                {
                    entity.pass_sock = getall_sock.GetString(6);
                }
                entity.type_sock = getall_sock.GetInt32(7);
                entity.live = 0;
                // Set inv properties
                List_Socks.Add(entity);
            }
            if (Check_live_proxy.Checked)
            {
                for (int i = 0; i < List_Socks.Count -1; i++)
                {
                    count_temp++;
                    Console.WriteLine("Check-Socks-Thread- " + count_temp);
                    type_sock = List_Socks[i].type_sock;
                    if (List_Socks[i].black_list >= 1)
                    {
                        continue;
                    }
                    if (List_Socks[i].use_time >= 4)
                    {
                        continue;
                    }
                    Thread th = new Thread(() =>
                    {
                        bool check_sock = CheckLiveSock(List_Socks[i].socks_value, List_Socks[i].type_sock, List_Socks[i].use_sock, List_Socks[i].pass_sock);
                        if (check_sock)
                        {
                            List_Socks[i].live = 1;
                        }
                        //calling callback function
                    });
                    th.Start();
                    threads.Add(th);


                }

                for (int i = 1; i < threads.Count - 1; i++)
                {
                    Thread.Sleep(100);
                    lb_proxy.Text = i.ToString();
                    Console.WriteLine("T-" + i + " is " + threads[i].IsAlive);
                    if (threads[i].IsAlive == true)
                    {
                        // MessageBox.Show("abc");
                        i = i - 1;
                        continue;
                    }
                }

            }
            int ccount_socks = 0;
            int retry = 1;
            bool re_round = false;
            bool break_loop = false;
            int cc_temp = 0;
            List<String> hoanthanh = new List<string>();
            List<int> dangchay = new List<int>();
            List<int> thatbai = new List<int>();
            List<int> chuachay = new List<int>();
            List<int> ngoailuong = new List<int>();
            for (int i = 0; i < Boss_Gom.Count; i+=10)
            {
                List<string> children_input = new List<string>();
                for (int j = i; j < i+10;j++)
                {
                    if ((j + 10) < Children_account.Count)
                    {
                        children_input.Add(Children_account[j]);
                    }
                    
                }
                while (true)
                {
                    
                    if (List_Socks[ccount_socks].use_time >= 4 || List_Socks[ccount_socks].black_list == 1)
                    {
                        ccount_socks++;
                    }
                    else
                    {                        
                        if (CheckLiveSock(List_Socks[ccount_socks].socks_value, List_Socks[ccount_socks].type_sock, List_Socks[ccount_socks].use_sock, List_Socks[ccount_socks].pass_sock))
                        {
                            List_Socks[ccount_socks].live = 1;                     

                            //1 Lần call hàm tương ứng với 1 lần tạo bàn
                            GomTien_Cate(Boss_Gom[i], children_input, List_Socks[ccount_socks].socks_value, List_Socks[ccount_socks].use_sock, List_Socks[ccount_socks].pass_sock, List_Socks[ccount_socks].type_sock);  
                            break;
                        }
                    }

                }                
                 //calling callback function
                ccount_socks++;
                if (re_round == true)
                {
                    re_round = false;
                    i = -1;
                }
                i++;
            }


        }
        public bool GomTien_Cate(string account_boss, List<string> chilren_account,string sock_value ,string username = "", string password = "", int type = 5)
        {
            string phrase = sock_value;
            string complate = "C4-83-6E-67-20-6E-68-E1-BA-AD-70-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67";
            string ban_byte = "4B-68-C3-B4-6E-67-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67-2E-20-4C-69-C3-AA-6E-20-68-E1-BB-87-20-61-64-6D-69-6E-20-C4-91-E1-BB-83-20-62-69-E1-BA-BF-74-20-74-68-C3-AA-6D-20-63-68-69-20-74-69-E1-BA-BF-74";
            string login_true = "C4-83-6E-67-20-6E-68-E1-BA-AD-70-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67";
            string tb_join_check1 = "07-0A-05-08-EE-0F-10-01-0A-05-08-F0-0F-10-00-0A-05-08-EA-0F-10-00-0A-05";
            byte[] tb_num_start = { 0x33, 0x08, 0x99, 0xcb, 0x01, 0x1a, 0x2d, 0x52, 0x05, 0x08, 0xe1, 0x0f, 0x10, 0x1a, 0x52, 0x05, 0x08, 0xf9, 0x0f, 0x10, 0x00, 0x52, 0x06, 0x08, 0xd1, 0x0f, 0x10 };
            byte[] tb_num_end = { 0x52, 0x0e, 0x08, 0xda, 0x0f, 0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01, 0x5a, 0x05, 0x08, 0xc9, 0x10, 0x10, 0x00 };
            byte[] Catte_lobby1 = { 0x02, 0x08, 0x0a };
            byte[] Catte_lobby2 = { 0x2b, 0x08, 0x93, 0xcb, 0x01, 0x1a, 0x25, 0x0a, 0x05, 0x08, 0xfa, 0x0f, 0x10, 0x00, 0x52, 0x05, 0x08, 0xf9, 0x0f, 0x10, 0x01, 0x52, 0x0e, 0x08, 0xda, 0x0f, 0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01, 0x52, 0x05, 0x08, 0xd6, 0x0f, 0x10, 0x00 };
            byte[] pk_create_table = { 0x44, 0x08, 0x92, 0xcb, 0x01, 0x1a, 0x3e, 0x52, 0x05, 0x08, 0xd8, 0x0f, 0x10, 0x06, 0x52, 0x0e, 0x08, 0xda, 0x0f, 0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01, 0x52, 0x0e, 0x08, 0xd4, 0x0f, 0x10, 0xfc, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01, 0x52, 0x05, 0x08, 0xfc, 0x0f, 0x10, 0x3d, 0x5a, 0x05, 0x08, 0xc9, 0x10, 0x10, 0x00, 0x5a, 0x07, 0x08, 0xd6, 0x0f, 0x10, 0xd0, 0x86, 0x03 };
            //try
            //{

            string[] words = phrase.Split(':');
            string ipres = words[0];
            byte[] conn = packetohex("0x04011BBF0301A8B100");
            //Console.WriteLine(ipres);
            int portres = Int32.Parse(words[1]);
            byte[] pk_login_boss = packetohex(account_boss);
            ProxySocket s = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            List<prxy> connect = new List<prxy>();
            List<Thread> t_sock = new List<Thread>();
            List<ProxySocket> l_socket = new List<ProxySocket>();
            byte[] buffer = new byte[4 * 1024];
            int recv = 0;
            ProxySocket bc = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bc.Connect(IPAddress.Parse(ipres), portres);
            bc.Send(conn);
            for (int i = 0; i < 10; i++)
            {
                ProxySocket sc = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sc.Connect(IPAddress.Parse(ipres), portres);
                sc.Send(conn);
                l_socket.Add(sc);
            }

            //for (int i=0; i < 10;i++)
            //    {
            //        bool sock_temp_ceck = false;
            //        ProxySocket s1 = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    prxy next_g = new prxy();
            //    next_g.prxreturn = false;
            //    next_g.prx_var = s1;
            //    connect.Add(next_g);
            //    Thread t_reg = new Thread(() =>
            //        {

            //            switch (type)
            //            {
            //                case 5:
            //                    next_g = Socks5(sock_value, username, password);
            //                    break;
            //                case 4:
            //                    next_g = Socks4a(sock_value);
            //                    break;
            //                default:
            //                    next_g = Socks5(sock_value, username, password);
            //                    break;
            //            }
            //            if (next_g.prxreturn)
            //            {
            //                sock_temp_ceck = true;
            //                connect.Add(next_g);
            //            }                       
            //        });
            //        t_reg.Start();
            //        t_sock.Add(t_reg);
            //        Thread.Sleep(500);
            //        if  (i == 9)
            //        {
            //        bool check_t_sock = true;

            //        while (check_t_sock)
            //        {
            //            check_t_sock = false;
            //            for (int j = 0;j < t_sock.Count;j++)
            //            {     
            //                if (t_sock[j].IsAlive) // kết thúc thread t_sock[j] nếu isAlive  tiếp tục vòng lặp
            //                {                                
            //                    check_t_sock = true;
            //                } else
            //                {
            //                    if (connect[j].prxreturn == false) // connect[j].prxreturn  false tức socket có vấn đề. Đăng ký lại socket
            //                    {
            //                        Thread t_reg_re = new Thread(() =>
            //                        {
            //                            next_g = new prxy();
            //                            next_g.prxreturn = false;
            //                            connect[j] = next_g;
            //                            switch (type)
            //                            {
            //                                case 5:
            //                                    next_g = Socks5(sock_value, username, password);
            //                                    break;
            //                                case 4:
            //                                    next_g = Socks4a(sock_value);
            //                                    break;
            //                                default:
            //                                    next_g = Socks5(sock_value, username, password);
            //                                    break;
            //                            }
            //                            if (next_g.prxreturn)
            //                            {
            //                                connect[i] = next_g;
            //                            }
            //                        });
            //                        t_reg_re.Start();
            //                        t_sock[j] = t_reg_re;
            //                    }
            //                }

            //            }
            //        }
            //    }
            //    }

            //int recv = 0;
            //byte[] buffer = new byte[4 * 1024];
            for (int i = 0; i < l_socket.Count; i++)
            {
                l_socket[i].Send(packetohex(chilren_account[i]));
            }
            bc.Send(packetohex(account_boss));
                recv = bc.Receive(buffer);
                
                if (recv > 0)
                {

                    string bitString = BitConverter.ToString(buffer);
                    bool reg_check = bitString.Contains(complate);
                    bool reg_check_ban = bitString.Contains(ban_byte);
                    bool login_check_ok = bitString.Contains(login_true);
                    byte[] filewrite_1 = new byte[recv];
                    if (reg_check_ban)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText =
                                "update Account set Banned = :black where Socks=:socks";
                            //getall.GetInt32(0).ToString()
                            command.Parameters.Add("black", DbType.Int32).Value = 1;
                           // command.Parameters.Add("socks", DbType.String).Value = full_account[account_in].account;
                            command.ExecuteNonQuery();
                        }
                    }
                    if (reg_check || login_check_ok)
                    {
                        bc.Send(Catte_lobby1);
                        Thread.Sleep(250);
                        bc.Send(Catte_lobby2);
                        Thread.Sleep(250);
                        bc.Send(pk_create_table);
                    byte[] buffer1 = new byte[10 * 1024]; 
                    recv = bc.Receive(buffer1);
                    string table_str = BitConverter.ToString(buffer1);
                    bool table_check = bitString.Contains(table_str);
                    if(table_check)
                    {
                        int found = 0;
                        found = table_str.IndexOf("52-07-08-d1-0f-10");
                        string table_num = table_str.Substring(found + 19,8);
                        String[] arr = table_str.Split('-');
                        byte[] array_table = new byte[arr.Length];
                        byte[] join_array = new byte[tb_num_start.Length + tb_num_end.Length + array_table.Length];
                        tb_num_start.CopyTo(join_array,0);
                        array_table.CopyTo(join_array, tb_num_start.Length);
                        tb_num_end.CopyTo(join_array, tb_num_start.Length + array_table.Length);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            array_table[i] = Convert.ToByte(arr[i], 16);
                        }

                        Thread.Sleep(250);
                        for (int i = 0; i < l_socket.Count; i++)
                        {
                            l_socket[i].Send(packetohex(chilren_account[i]));
                        }
                    }

                    
                        //createConection();
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText =
                                "update Account set Reward = Reward + 1,'Time Reward'=:timereward where HexReg=:socks";
                            //getall.GetInt32(0).ToString()
                            //command.Parameters.Add("reward", DbType.Int32).Value = 1;
                            DateTime time = DateTime.Now;
                            command.Parameters.Add("timereward", DbType.String).Value = time;
                            //command.Parameters.Add("socks", DbType.String).Value = full_account[account_in].account;
                            command.ExecuteNonQuery();
                        }
                        //full_account[account_in].runned = 1;
                        return true;
                        //closeConnection();
                    }


                }
                //full_account[account_in].runned = 4;
                return false;
            //}
            //catch
            //{
            //    full_account[account_in].runned = 3;
            //    return false;
            //    //}
            //    //s.Send(append_conn);
            //}
            //return false;
            return false;
        }
        public void Reward ()
        {
            // create a new ProxySocket
            List<string> lines = new List<string>();
            List<Thread> threads = new List<Thread>();
            List<Thread> t_resign = new List<Thread>();
            string get_sock_cmd = "Select * From Socks5";
            string get_account_cmd = "Select * From Account";
            createConection();
            using (SQLiteCommand cmd = new SQLiteCommand(get_account_cmd, _con))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        // Set inv properties
                        Product item = new Product { account = rdr.GetString(1), runned = 0 };
                        full_account.Add(item);
                    }
                }
            }
            SQLiteCommand cmd_exc = new SQLiteCommand(get_sock_cmd, _con);
            SQLiteDataReader getall_sock = cmd_exc.ExecuteReader();
            int type_sock = 5;
            //string ip_include = "";
            int count_temp = 0;
            while (getall_sock.Read())
            {

                Socks_Member entity = new Socks_Member();
                entity.stt = getall_sock.GetInt32(0);
                entity.socks_value = getall_sock.GetString(1);
                if (getall_sock.IsDBNull(2))
                {
                    entity.use_time = 0;
                }
                else
                {
                    entity.use_time = getall_sock.GetInt32(2);
                }
                if (getall_sock.IsDBNull(3))
                {
                    entity.date_sock = "";
                }
                else
                {
                    entity.date_sock = getall_sock.GetString(3);
                }
                if (getall_sock.IsDBNull(4))
                {
                    entity.black_list = 0;
                }
                else
                {
                    entity.black_list = getall_sock.GetInt32(4);
                }
                if (getall_sock.IsDBNull(5))
                {
                    entity.use_sock = "";
                }
                else
                {
                    entity.use_sock = getall_sock.GetString(5);
                }
                if (getall_sock.IsDBNull(6))
                {
                    entity.pass_sock = "";
                }
                else
                {
                    entity.pass_sock = getall_sock.GetString(6);
                }
                entity.type_sock = getall_sock.GetInt32(7);
                // Set inv properties
                List_Socks.Add(entity);
            }
            if (Check_live_proxy.Checked)
            {
                for (int i = 0; i < List_Socks.Count; i++)
                {
                    count_temp++;
                    Console.WriteLine("Check-Socks-Thread- " + count_temp);
                    type_sock = List_Socks[i].type_sock;
                    if (List_Socks[i].black_list >= 1)
                    {
                        continue;
                    }
                    if (List_Socks[i].use_time >= 4)
                    {
                        continue;
                    }
                    Thread th = new Thread(() =>
                    {
                        CheckLiveSock(List_Socks[i].socks_value, List_Socks[i].type_sock, List_Socks[i].use_sock, List_Socks[i].pass_sock);
                        //calling callback function
                    });
                    th.Start();
                    threads.Add(th);


                }

                for (int i = 1; i < threads.Count - 1; i++)
                {
                    Thread.Sleep(100);
                    lb_proxy.Text = i.ToString();
                    Console.WriteLine("T-" + i + " is " + threads[i].IsAlive);
                    if (threads[i].IsAlive == true)
                    {
                        // MessageBox.Show("abc");
                        i = i - 1;
                        continue;
                    }
                }

            }
            //MessageBox.Show(myListOfStrings.Count.ToString());
            //Console.WriteLine(myListOfStrings[0]);
            //foreach (string l in myListOfStrings)
            //{
            //    Console.WriteLine(l);
            //}
            //myListOfStrings Thread sau khi check live của socks sẽ thêm vào array này
            //full_account tổng số account
            //MessageBox.Show(full_account.Count.ToString());
            int ccount_socks = 0;
            int retry = 0;
            bool re_round = false;
            bool break_loop = false;
            int cc_temp = 0;
            List<String> hoanthanh = new List<string>();
            List<int> dangchay = new List<int>();
            List<int> thatbai = new List<int>();
            List<int> chuachay = new List<int>();
            for (int i = 0; i < full_account.Count; i++)
            {

                //if (retry > 0 && thatbai.Count > 0 && chuachay.Count == 0)
                //{
                //    MessageBox.Show("RS");
                //    if (cc_temp == thatbai.Count)
                //    {
                //        cc_temp = 0;
                //    }
                //    if (thatbai[cc_temp] != i)
                //    {
                //        continue;
                //    }
                //    cc_temp++;
                //}
                if (i == 0)
                {
                    MessageBox.Show("1");
                }
                if (i == (full_account.Count - 1) && break_loop == false)
                {
                    hoanthanh = new List<string>();
                    dangchay = new List<int>();
                    thatbai = new List<int>();
                    chuachay = new List<int>();
                    cc_temp = 0;
                    // int hoanthanh = 0, dangchay = 0, thatbai = 0,chuachay = 0;
                    for (int j = 0; j < full_account.Count; j++)
                    {
                        switch (full_account[j].runned)
                        {
                            case 1:
                                hoanthanh.Add(full_account[j].account);
                                break;
                            case 2:
                                dangchay.Add(j);
                                break;
                            case 3:
                                thatbai.Add(j);
                                break;
                            case 0:
                                chuachay.Add(j);
                                break;
                        }

                    }
                    Console.WriteLine("Chua Chay: ");
                    for (int m = 0; m < chuachay.Count;m++)
                    {
                        Console.Write(chuachay[m] + ", ");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Dang Chay: ");
                    for (int m = 0; m < dangchay.Count; m++)
                    {
                        Console.Write( dangchay[m] + ", ");
                    }
                    Console.WriteLine();
                    Console.WriteLine("BaoCao: TC=" + hoanthanh.Count + " DC=" + dangchay.Count + " CC=" + chuachay.Count + " TB= " + thatbai.Count);
                    Thread.Sleep(10000);
                    i = -1;
                    re_round = true;
                    retry++;
                }
                if (retry > 0 && chuachay.Count > 0)
                {
                    if (cc_temp == chuachay.Count)
                    {
                        cc_temp = 0;
                    }
                    if (chuachay[cc_temp] != i)
                    {
                        //continue;
                    }
                    cc_temp++;
                }
                if (full_account[i].runned == 2 || full_account[i].runned == 1)
                {
                   // continue;
                }
                if (full_account[i].runned == 1)
                {
                    //break;
                }
                if (full_account[i].runned != 1 && full_account[i].runned != 2)
                {
                    break_loop = false;
                }
                if (ccount_socks >= (List_Socks.Count - 2))
                {

                    if (break_loop == true)
                    {
                        MessageBox.Show("Tat Ca Account Da Duoc Chay");
                        break;
                    }
                    break_loop = true;
                    Thread.Sleep(20000);
                    if (retry >= 4)
                    {
                        MessageBox.Show("Da Su Dung Het Socks");
                    }
                    ccount_socks = 0;
                    retry++;
                }
                //bool passconnect = false;
                //int usetime = 0;
                //bool full_user = false;
                ////string stm = "Select * From Socks5 Where Socks = '" + List_Socks[ccount_socks].socks_value + "'";                
                ////SQLiteCommand cmd = new SQLiteCommand(stm, _con);
                ////SQLiteDataReader getall = cmd.ExecuteReader();
                //string username_tb = "";
                //string password_tb = "";
                while (true)
                {
                    if (List_Socks[ccount_socks].use_time >= 4 || List_Socks[ccount_socks].black_list == 1)
                    {
                        ccount_socks++;
                    }
                    else
                    {
                        break;
                    }

                }

                Thread t_reg = new Thread(() =>
                {

                    Reward_gift(i, List_Socks[ccount_socks].socks_value, List_Socks[ccount_socks].use_sock, List_Socks[ccount_socks].pass_sock, List_Socks[ccount_socks].type_sock);
                    //calling callback function
                });
                t_reg.Name = ccount_socks.ToString() ;
                t_reg.Start();
                t_resign.Add(t_reg);
                
                Console.WriteLine("Tong So Thread Da Chay: " + t_resign.Count.ToString());
                //if ((i + 1) != t_resign.Count)
                //{
                //    MessageBox.Show("9999 doa hong");
                //}    
                //thread_run.Text = t_resign.Count.ToString();
                ccount_socks++;
                //if (re_round == true)
                //{
                //    re_round = false;
                //    i = 0;
                //}
            }
            MessageBox.Show("Kiem Tra Tien Do Thread" + chuachay.Count.ToString());
            MessageBox.Show("Kiem Tra Tien Do Thread" + t_resign.Count.ToString());
            //Console.WriteLine("Running.");
            //for (int i = 0;i < t_resign.Count;i++)
            //{
            //    Thread.Sleep(100);
            //    Console.Write(".");
            //    if (t_resign[i].IsAlive) {
            //        i = 0;
            //    }
            //}    
            //MessageBox.Show(ds.Tables[0].ToString());
        }
        public bool Reward_gift(int account_in ,string socket_in,string username = "",string password = "",int type = 5)
        {
            full_account[account_in].runned = 2;
            string phrase = socket_in;
            string complate = "C4-83-6E-67-20-6E-68-E1-BA-AD-70-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67";
            string ban_byte = "4B-68-C3-B4-6E-67-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67-2E-20-4C-69-C3-AA-6E-20-68-E1-BB-87-20-61-64-6D-69-6E-20-C4-91-E1-BB-83-20-62-69-E1-BA-BF-74-20-74-68-C3-AA-6D-20-63-68-69-20-74-69-E1-BA-BF-74";
            string login_true = "C4-83-6E-67-20-6E-68-E1-BA-AD-70-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67";

            byte[] byte_send0 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send1 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send2 = { 0x0b, 0x08, 0x9b, 0x08, 0x1a, 0x06, 0x52, 0x04, 0x08, 0x6f, 0x10, 0x00 };
            byte[] byte_send3 = { 0x1e, 0x08, 0x80, 0xfa, 0x01, 0x1a, 0x18, 0x52, 0x05, 0x08, 0xd3, 0x17, 0x10, 0x00, 0x52, 0x05, 0x08, 0xd6, 0x17, 0x10, 0x00, 0x72, 0x08, 0x08, 0xc4, 0x17, 0x12, 0x03, 0x4a, 0x50, 0x59 };
            byte[] byte_send4 = { 0x0b, 0x08, 0x9b, 0x08, 0x1a, 0x06, 0x52, 0x04, 0x08, 0x6f, 0x10, 0x00 };
            byte[] byte_send5 = { 0x04, 0x08, 0x84, 0xfa, 0x01 };
            byte[] byte_send6 = { 0x0b, 0x08, 0x9b, 0x08, 0x1a, 0x06, 0x52, 0x04, 0x08, 0x6f, 0x10, 0x04 };
            byte[] byte_send7 = { 0x04, 0x08, 0x9b, 0xf2, 0x01 };
            byte[] byte_send8 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send9 = { 0x04, 0x08, 0xa3, 0xf2, 0x01 };
            byte[] byte_send10 = { 0x15, 0x08, 0x9f, 0xf2, 0x01, 0x1a, 0x0f, 0x52, 0x05, 0x08, 0xd8, 0x17, 0x10, 0x00, 0x52, 0x06, 0x08, 0xd4, 0x17, 0x10, 0xe8, 0x07 };
            byte[] byte_send11 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send12 = { 0x02, 0x08, 0x0a };
            byte[] byte_send13 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send14 = { 0x0d, 0x08, 0xa4, 0xf2, 0x01, 0x1a, 0x07, 0x52, 0x05, 0x08, 0xf9, 0x17, 0x10, 0x01 };
            byte[] byte_send15 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send16 = { 0x0d, 0x08, 0xa4, 0xf2, 0x01, 0x1a, 0x07, 0x52, 0x05, 0x08, 0xf9, 0x17, 0x10, 0x01 };
            byte[] byte_send17 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send18 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send19 = { 0x22, 0x08, 0x9e, 0xf2, 0x01, 0x1a, 0x1c, 0x0a, 0x05, 0x08, 0xf0, 0x17, 0x10, 0x01, 0x0a, 0x05, 0x08, 0xf1, 0x17, 0x10, 0x00, 0x52, 0x05, 0x08, 0xe9, 0x17, 0x10, 0x00, 0x52, 0x05, 0x08, 0xd4, 0x17, 0x10, 0x04 };
            byte[] byte_send20 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send21 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send22 = { 0x02, 0x08, 0x0a };
            byte[] byte_send23 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            //---------------------------------------------------//
            try
            {

                string[] words = phrase.Split(':');
                string ipres = words[0];
                //Console.WriteLine(ipres);
                int portres = Int32.Parse(words[1]);
                byte[] packet_reg_byte = packetohex(full_account[account_in].account);
                ProxySocket s = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                prxy next_g = new prxy();
                switch (type)
                {
                    case 5:
                        next_g = Socks5(socket_in, username, password);
                        break;
                    case 4:
                        next_g = Socks4a(socket_in);
                        break;
                    default:
                        next_g = Socks5(socket_in, username, password);
                        break;
                }
                if (next_g.prxreturn == false)
                {
                    full_account[account_in].runned = 3;
                    return false;
                }
                s = next_g.prx_var;
                int recv = 0;
                byte[] buffer = new byte[4 * 1024];
                s.Send(packet_reg_byte);
                recv = s.Receive(buffer);

                if (recv > 0)
                {
                    string bitString = BitConverter.ToString(buffer);
                    bool reg_check = bitString.Contains(complate);
                    bool reg_check_ban = bitString.Contains(ban_byte);
                    bool login_check_ok = bitString.Contains(login_true);
                    byte[] filewrite_1 = new byte[recv];
                    if (reg_check_ban)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText =
                                "update Account set Banned = :black where Socks=:socks";
                            //getall.GetInt32(0).ToString()
                            command.Parameters.Add("black", DbType.Int32).Value = 1;
                            command.Parameters.Add("socks", DbType.String).Value = full_account[account_in].account;
                            command.ExecuteNonQuery();
                        }
                    }
                    if (reg_check || login_check_ok)
                    {
                        
                        //for (int i = 0; i < recv; i++)
                        //{
                        //    filewrite_1[i] = buffer[i];
                        //}
                        s.Send(byte_send0);
                        Thread.Sleep(250);
                        s.Send(byte_send1);
                        Thread.Sleep(250);
                        s.Send(byte_send2);
                        Thread.Sleep(250);
                        s.Send(byte_send3);
                        Thread.Sleep(250);
                        s.Send(byte_send4);
                        Thread.Sleep(250);
                        s.Send(byte_send5);
                        Thread.Sleep(250);
                        s.Send(byte_send6);
                        Thread.Sleep(250);
                        s.Send(byte_send7);
                        Thread.Sleep(250);
                        s.Send(byte_send8);
                        Thread.Sleep(250);
                        s.Send(byte_send9);
                        Thread.Sleep(250);
                        s.Send(byte_send10);
                        Thread.Sleep(250);
                        s.Send(byte_send11);
                        Thread.Sleep(250);
                        s.Send(byte_send12);
                        Thread.Sleep(250);
                        s.Send(byte_send13);
                        Thread.Sleep(250);
                        s.Send(byte_send14);
                        Thread.Sleep(250);
                        s.Send(byte_send15);
                        Thread.Sleep(250);
                        s.Send(byte_send16);
                        Thread.Sleep(250);
                        s.Send(byte_send17);
                        Thread.Sleep(250);
                        s.Send(byte_send18);
                        Thread.Sleep(250);
                        s.Send(byte_send19);
                        Thread.Sleep(250);
                        s.Send(byte_send20);
                        Thread.Sleep(250);
                        s.Send(byte_send21);
                        Thread.Sleep(250);
                        s.Send(byte_send22);
                        Thread.Sleep(250);
                        s.Send(byte_send23);
                        Thread.Sleep(250);
                        //createConection();
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText =
                                "update Account set Reward = Reward + 1,'Time Reward'=:timereward where HexReg=:socks";
                            //getall.GetInt32(0).ToString()
                            //command.Parameters.Add("reward", DbType.Int32).Value = 1;
                            DateTime time = DateTime.Now;
                            command.Parameters.Add("timereward", DbType.String).Value = time;
                            command.Parameters.Add("socks", DbType.String).Value = full_account[account_in].account;
                            command.ExecuteNonQuery();
                        }
                        full_account[account_in].runned = 1;
                        return true;
                        //closeConnection();
                    }
                        
                    
                }
                full_account[account_in].runned = 4;
                return false;
            }
            catch
            {
                full_account[account_in].runned = 3;
                return false;
                //}
                //s.Send(append_conn);
            }
            //return false;
        }
        public bool Resign(string socket_in,string username = "" ,string password = "",int type = 5)
        {
            string phrase = socket_in;
            string complate = "C4-83-6E-67-20-6E-68-E1-BA-AD-70-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67";
            string ban_byte = "4B-68-C3-B4-6E-67-20-74-68-C3-A0-6E-68-20-63-C3-B4-6E-67-2E-20-4C-69-C3-AA-6E-20-68-E1-BB-87-20-61-64-6D-69-6E-20-C4-91-E1-BB-83-20-62-69-E1-BA-BF-74-20-74-68-C3-AA-6D-20-63-68-69-20-74-69-E1-BA-BF-74";

            byte[] byte_send0 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send1 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send2 = { 0x0b, 0x08, 0x9b, 0x08, 0x1a, 0x06, 0x52, 0x04, 0x08, 0x6f, 0x10, 0x00 };
            byte[] byte_send3 = { 0x1e, 0x08, 0x80, 0xfa, 0x01, 0x1a, 0x18, 0x52, 0x05, 0x08, 0xd3, 0x17, 0x10, 0x00, 0x52, 0x05, 0x08, 0xd6, 0x17, 0x10, 0x00, 0x72, 0x08, 0x08, 0xc4, 0x17, 0x12, 0x03, 0x4a, 0x50, 0x59 };
            byte[] byte_send4 = { 0x0b, 0x08, 0x9b, 0x08, 0x1a, 0x06, 0x52, 0x04, 0x08, 0x6f, 0x10, 0x00 };
            byte[] byte_send5 = { 0x04, 0x08, 0x84, 0xfa, 0x01 };
            byte[] byte_send6 = { 0x0b, 0x08, 0x9b, 0x08, 0x1a, 0x06, 0x52, 0x04, 0x08, 0x6f, 0x10, 0x04 };
            byte[] byte_send7 = { 0x04, 0x08, 0x9b, 0xf2, 0x01 };
            byte[] byte_send8 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send9 = { 0x04, 0x08, 0xa3, 0xf2, 0x01 };
            byte[] byte_send10 = { 0x15, 0x08, 0x9f, 0xf2, 0x01, 0x1a, 0x0f, 0x52, 0x05, 0x08, 0xd8, 0x17, 0x10, 0x00, 0x52, 0x06, 0x08, 0xd4, 0x17, 0x10, 0xe8, 0x07 };
            byte[] byte_send11 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send12 = { 0x02, 0x08, 0x0a };
            byte[] byte_send13 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send14 = { 0x0d, 0x08, 0xa4, 0xf2, 0x01, 0x1a, 0x07, 0x52, 0x05, 0x08, 0xf9, 0x17, 0x10, 0x01 };
            byte[] byte_send15 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send16 = { 0x0d, 0x08, 0xa4, 0xf2, 0x01, 0x1a, 0x07, 0x52, 0x05, 0x08, 0xf9, 0x17, 0x10, 0x01 };
            byte[] byte_send17 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send18 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send19 = { 0x22, 0x08, 0x9e, 0xf2, 0x01, 0x1a, 0x1c, 0x0a, 0x05, 0x08, 0xf0, 0x17, 0x10, 0x01, 0x0a, 0x05, 0x08, 0xf1, 0x17, 0x10, 0x00, 0x52, 0x05, 0x08, 0xe9, 0x17, 0x10, 0x00, 0x52, 0x05, 0x08, 0xd4, 0x17, 0x10, 0x04 };
            byte[] byte_send20 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            byte[] byte_send21 = { 0x03, 0x08, 0xf1, 0x07 };
            byte[] byte_send22 = { 0x02, 0x08, 0x0a };
            byte[] byte_send23 = { 0x04, 0x08, 0x8e, 0xa1, 0x02 };
            //---------------------------------------------------//
            try
            {

                string[] words = phrase.Split(':');
                string ipres = words[0];
                int portres = Int32.Parse(words[1]);
                ProxySocket s = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) ;
                string mac = "00:" + _Randomstring(2, 3).ToUpper() + ":" + _Randomstring(2, 3).ToUpper() + ":" + _Randomstring(2, 3).ToUpper() + ":" + _Randomstring(2, 3).ToUpper() + ":" + _Randomstring(2, 3).ToUpper();
                string imei_device = _Randomstring(15, 0);
                string imei_device_md5;
                using (MD5 md5Hash = MD5.Create())
                {
                    imei_device_md5 = GetMd5(md5Hash, imei_device);
                }
                string id_device = _Randomstring(16, 2);                
                string r_id1 = _Randomstring(8, 2);
                string r_id2 = _Randomstring(4, 2);
                string r_id3 = _Randomstring(4, 2);
                string r_id4 = _Randomstring(4, 2);
                string r_id5 = _Randomstring(12, 2);
                string r_id6 = _Randomstring(22, 2);
                string id_adv = r_id1 + "-" + r_id2 + "-" + r_id3 + "-" + r_id4 + "-" + r_id5;
                string packet_reg = "0xe30108e9071add010a0408131001520408031000520408051000720908021205332e392e36724908141245" + _StringToHex(imei_device_md5) + "23" + _StringToHex(id_device) + "232323" + _StringToHex(mac) + "722908a5011224" + _StringToHex(id_adv) + "72120815120e636f6d2e686474742e6e706c6179720408161200721a08171216" + _StringToHex(r_id6) + "7204081812007205088d011200720508df011200";
                byte[] packet_reg_byte = packetohex(packet_reg);
                prxy next_g = new prxy();
                switch (type)
                {
                    case 5:
                        next_g =  Socks5(socket_in, username, password);
                        break;
                    case 4:
                        next_g = Socks4a(socket_in);
                        break;
                    default:
                        next_g = Socks5(socket_in, username, password);
                        break;
                }
                if (next_g.prxreturn == false)
                {
                    return false;
                }
                s = next_g.prx_var;
                int recv = 0;
                byte[] buffer = new byte[4 * 1024];
                s.Send(packet_reg_byte);
                recv = s.Receive(buffer);
                if (recv > 0)
                {
                    string bitString = BitConverter.ToString(buffer);
                    bool reg_check = bitString.Contains(complate);
                    bool reg_check_ban = bitString.Contains(ban_byte);
                    byte[] filewrite_1 = new byte[recv];
                    if (reg_check_ban)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText =
                                "update Socks5 set Blacklist = :black where Socks=:socks";
                            //getall.GetInt32(0).ToString()
                            command.Parameters.Add("black", DbType.Int32).Value = 1 ;
                            command.Parameters.Add("socks", DbType.String).Value = socket_in;
                            command.ExecuteNonQuery();
                        }
                    }
                    if (reg_check)
                    {
                        //for (int i = 0; i < recv; i++)
                        //{
                        //    filewrite_1[i] = buffer[i];
                        //}
                        s.Send(byte_send0);
                        Thread.Sleep(250);
                        s.Send(byte_send1);
                        Thread.Sleep(250);
                        s.Send(byte_send2);
                        Thread.Sleep(250);
                        s.Send(byte_send3);
                        Thread.Sleep(250);
                        s.Send(byte_send4);
                        Thread.Sleep(250);
                        s.Send(byte_send5);
                        Thread.Sleep(250);
                        s.Send(byte_send6);
                        Thread.Sleep(250);
                        s.Send(byte_send7);
                        Thread.Sleep(250);
                        s.Send(byte_send8);
                        Thread.Sleep(250);
                        s.Send(byte_send9);
                        Thread.Sleep(250);
                        s.Send(byte_send10);
                        Thread.Sleep(250);
                        s.Send(byte_send11);
                        Thread.Sleep(250);
                        s.Send(byte_send12);
                        Thread.Sleep(250);
                        s.Send(byte_send13);
                        Thread.Sleep(250);
                        s.Send(byte_send14);
                        Thread.Sleep(250);
                        s.Send(byte_send15);
                        Thread.Sleep(250);
                        s.Send(byte_send16);
                        Thread.Sleep(250);
                        s.Send(byte_send17);
                        Thread.Sleep(250);
                        s.Send(byte_send18);
                        Thread.Sleep(250);
                        s.Send(byte_send19);
                        Thread.Sleep(250);
                        s.Send(byte_send20);
                        Thread.Sleep(250);
                        s.Send(byte_send21);
                        Thread.Sleep(250);
                        s.Send(byte_send22);
                        Thread.Sleep(250);
                        s.Send(byte_send23);
                        Thread.Sleep(250);
                        //createConection();
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = "INSERT INTO Account (HexReg, Imei, Device_ID,Mac,ID_Adv) VALUES (@fullpack, @imei, @device_id, @mac, @id_adv)";
                            command.Parameters.Add("fullpack", DbType.String).Value = packet_reg;
                            command.Parameters.Add("imei", DbType.String).Value = imei_device;
                            command.Parameters.Add("device_id", DbType.String).Value = id_device;
                            command.Parameters.Add("mac", DbType.String).Value = mac;
                            command.Parameters.Add("id_adv", DbType.String).Value = id_adv;
                            command.ExecuteNonQuery();
                        }
                        //closeConnection();
                    }
                }
            }
            catch
            {

                //return false;
            }
            
        
            return false;
        }
        public byte[] TransformBytes(int num, int byteLength)
        {
            byte[] res = new byte[byteLength];

            byte[] temp = BitConverter.GetBytes(num);

            Array.Copy(temp, res, byteLength);

            return res;
        }

        public prxy Socks5 (string socket_in,string username ,string password)
        {
            prxy returned = new prxy();
            try
            {
                
                string phrase = socket_in;
                string[] words = phrase.Split(':');
                string ipres = words[0];
                //Console.WriteLine(ipres);
                int portres = Int32.Parse(words[1]);
                ProxySocket s = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //s.SendTimeout = 2000;
                s.ProxyEndPoint = new IPEndPoint(IPAddress.Parse("116.202.103.223"), 35562);
                s.ProxyType = ProxyTypes.Socks5;    // if you set this to ProxyTypes.None, 
                                                    // the ProxySocket will act as a normal Socket
                                                    // connect to the remote server
                                                    // (note that the proxy server will resolve the domain name for us)
                s.ProxyUser = username;
                s.ProxyPass = password;
                s.Connect(IPAddress.Parse(ipres), portres);
                byte[] conn = { 0x05, 0x01, 0x00 };
                if (username != "")
                {
                    conn[2] = 0x02;
                }
                byte[] conn_victim = { 0x05, 0x01, 0x00, 0x01 };
                IPAddress address = IPAddress.Parse(victim_ip);
                byte[] bytes = address.GetAddressBytes();
                byte[] conn_port = BitConverter.GetBytes(victim_port);
                Array.Reverse(conn_port);
                byte[] temp_port = { conn_port[2], conn_port[3] };
                byte[] append_conn = new byte[conn_victim.Length + bytes.Length + temp_port.Length];
                //b[0]=55 b[1]=50 b[2]=56
                //Console.WriteLine("Press enter to co2222ntinue..."+ b);
                conn_victim.CopyTo(append_conn, 0);
                bytes.CopyTo(append_conn, conn_victim.Length);
                temp_port.CopyTo(append_conn, conn_victim.Length + bytes.Length);
                s.Send(conn); // Gives you hexadecimal
                int recv = 0;
                byte[] buffer = new byte[4 * 1024];
                recv = s.Receive(buffer);
                if ((buffer[0] == 0x05 && buffer[1] == 0x00) || (buffer[0] == 0x05 && buffer[1] == 0x02))
                {
                    if (username != "")
                    {
                        byte[] nexte = packetohex("0x01" + username.Length.ToString("X2") + _StringToHex(username) + password.Length.ToString("X2") + _StringToHex(password));
                        s.Send(nexte);
                        recv = s.Receive(buffer);
                    }
                    s.Send(append_conn);
                    recv = s.Receive(buffer);
                    if (recv > 0)
                    {
                        if (buffer[0] == 0x05 && buffer[1] == 0x00)
                        {
                            returned.prx_var = s;
                            returned.prxreturn = true;
                        }
                    }
                    else
                    {
                        returned.prxreturn = false;
                    }
                }
                else
                {
                    returned.prxreturn = false;
                }
                return returned;
            }
            catch
            {
                returned.prxreturn = false;
                return returned;
            }
        }
        public prxy Socks4a (string socket_in)
        {
            prxy returned = new prxy();
            string phrase = socket_in;
            string[] words = phrase.Split(':');
            string ipres = words[0];
            int portres = Int32.Parse(words[1]);
            ProxySocket s = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            s.Connect(IPAddress.Parse(ipres), portres);
            byte[] conn = { 0x04, 0x01};
            IPAddress address = IPAddress.Parse(victim_ip);
            byte[] bytes = address.GetAddressBytes();
            byte[] conn_port = BitConverter.GetBytes(victim_port);
            Array.Reverse(conn_port);
            byte[] temp_port = { conn_port[2], conn_port[3] };
            byte[] append_conn = new byte[conn.Length + bytes.Length + temp_port.Length + 1];
            conn.CopyTo(append_conn, 0);
            temp_port.CopyTo(append_conn, conn.Length);
            bytes.CopyTo(append_conn, conn.Length + temp_port.Length);
            append_conn[conn.Length + bytes.Length + temp_port.Length] = 0x00;
            s.Send(append_conn);
            int recv = 0;
            byte[] buffer = new byte[4 * 1024];
            recv = s.Receive(buffer);
            if ((buffer[0] == 0x00 && buffer[1] == 0x5A) || (buffer[0] == 0x05 && buffer[1] == 0x02))
            {
                returned.prx_var = s;
                returned.prxreturn = true; 
            }
            else
            {
                returned.prxreturn = false;
            }
            return returned;        
        }
        public bool CheckLiveSock(string sock_value, int type = 5, string username = "", string password = "")
        {
            string phrase = sock_value;
            try
            {
                string[] words = phrase.Split(':');
                string ipres = words[0];
                //Console.WriteLine(ipres);
                int portres = Int32.Parse(words[1]);
                ProxySocket s = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.ReceiveTimeout = 1000;
                s.SendTimeout = 200;
                s.ProxyUser = username;
                s.ProxyPass = password;
                s.ProxyEndPoint = new IPEndPoint(IPAddress.Parse("116.202.103.223"), 35562);
                s.ProxyType = ProxyTypes.Socks5;    // if you set this to ProxyTypes.None, 
                                                    // the ProxySocket will act as a normal Socket
                                                    // connect to the remote server
                                                    // (note that the proxy server will resolve the domain name for us)

                s.Connect(IPAddress.Parse(ipres), portres);
                switch (type) 
                {
                    case 5:
                        byte[] conn = { 0x05, 0x01, 0x00 };
                        if (username != "")
                        {
                            conn[2] = 0x02;
                        }
                        s.Send(conn); // Gives you hexadecimal
                        int recv = 0;
                        byte[] buffer = new byte[1024];
                        recv = s.Receive(buffer);
                        if ((buffer[0] == 0x05 && buffer[1] == 0x00) || (buffer[0] == 0x05 && buffer[1] == 0x02))
                        {
                            return true;
                        }
                        break;
                    case 4:
                        //0x040100508EFA422E00 google.com (142.250.66.46)
                        byte[] conn1 = packetohex("0x040100508EFA422E00");
                        s.Send(conn1);
                        int recv1 = 0;
                        byte[] buffer1 = new byte[1024];
                        recv1 = s.Receive(buffer1);
                        if ((buffer1[0] == 0x00 && buffer1[1] == 0x5A))
                        {
                            return true;
                        }
                        //0x0401005000000001007777772E676F6F676C652E636F6D00
                        break;
                } 

            }
            catch {
                return false;
            }
            //s.Send(append_conn);

            return false;
        }

        // 0 - Number
        // 1 - Alphabet
        // 2 - Number + aphabet
        // 3 - 16 hex 
        // Default = Number + aphabet + spec
        string _Randomstring(int length, int select)
        {
            string chars = "";
            switch (select)
            {
                case 0:
                    chars = "1234567890";
                    break;
                case 1:
                    chars = "abcdefghijklmnopqrstuvwxyz";
                    break;
                case 2:
                    chars = "abcdefghijklmnopqrstuvwxyz1234567890";
                    break;
                case 3:
                    chars = "abcdef1234567890";
                    break;
                default:
                    chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
                    break;
            }



            string rt = "";

            for (int i = 0; i < length; i++)
            {


                int flip = rnd.Next(chars.Length);
                rt = rt + chars[flip];
            }
            return rt;
        }

        public string _StringToHex(string in_s)
        {
            string decString = in_s;
            byte[] bytes = Encoding.Default.GetBytes(decString);
            string hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", "");
            //Console.WriteLine(hexString);
            return hexString;
        }

        /// <summary>
        /// Chuyển string hex sang byte[]
        /// example : "0x1122312333" => {0x11,0x22,0x31,0x23,0x33}
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public byte[] packetohex(string packet)
        {
            //Input
            string packet_reg = packet;
            List<string> trunggian = new List<string>();
            for (int i = 2; i < packet_reg.Length; i += 2)
            {
                string one = packet_reg[i].ToString() + packet_reg[i + 1].ToString();
                trunggian.Add(one);
            }
            byte[] result = trunggian
              .Select(value => Convert.ToByte(value, 16))
              .ToArray();
            return result;
        }

        public void createConection()
        {
            string _strConnect = "Data Source="+ path_sqlite_socks + ";Version=3;";
            _con.ConnectionString = _strConnect;
            _con.Open();
        }

        public void closeConnection()
        {
            _con.Close();
        }

        public SQLiteDataReader sl_all()
        {
            createConection();
            string stm = "SELECT * FROM Socks5";

            SQLiteCommand cmd = new SQLiteCommand(stm, _con);
            SQLiteDataReader getall = cmd.ExecuteReader();            
            return getall;
        }

        public List<string> Get_Socks_Online(string linked)
        {
            string html = string.Empty;
            string url = linked;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            html = html.Replace("<html><pre>", "");
            html = html.Replace("</pre></html>", "");

            //Console.WriteLine(html);
            
            string[] retu = html.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );
            List<string> sock_return = new List<string>(retu);
            sock_return.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            return sock_return;
        }
        public DataSet loadData()
        {
            DataSet ds = new DataSet();
            createConection();
            SQLiteDataAdapter da = new SQLiteDataAdapter("select id, fullname as [Full Name], email as [Email], address as [Address], phone as [Phone], birthday as [Birthday] from tbl_students", _con);

            da.Fill(ds);
            closeConnection();
            return ds;
        }
        static string GetMd5(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.  
            return sBuilder.ToString();
        }


    }
}
