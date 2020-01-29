using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI_Program.Code
{
    class Menu
    {
        GetHash getHash;
        SQLCommand sqlcom;

        public Menu()
        {
            getHash = new GetHash();
            sqlcom = new SQLCommand();
        }

        //展示菜单
        public void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("\n  FileHashConfirm-CLI Program\n");
            Console.WriteLine("  1. 创建数据库\n");
            Console.WriteLine("  2. 加入数据\n");
            Console.WriteLine("  3. 校验数据\n");
            Console.WriteLine("  4. 退出程序\n");
            ExcuteOrder();
        }

        public void ExcuteOrder()
        {
            int command = 0;
            Console.Write("请输入要执行的操作：  ");
            string input = Console.ReadLine();
            try
            {
                command = int.Parse(input);
            }catch(Exception e)
            {
                Console.WriteLine("输入格式错误！点击回车继续！");
                Console.ReadLine();
                ShowMenu();
            }
            
            switch (command)
            {
                case 1:
                    sqlcom.CreateTable();
                    Console.WriteLine("创建成功！点击回车继续！");
                    Console.ReadLine();
                    break;
                case 2:
                    AddFolderData();
                    Console.ReadLine();
                    break;
                case 3:
                    CheckFileHashValue();
                    Console.ReadLine();
                    break;
                case 4:
                    ExitProgram();
                    break;
            }
            ShowMenu();
        }

        public void AddFolderData()
        {
            Console.WriteLine("请输入要读入的目录：");
            string input = Console.ReadLine();
            if (!Directory.Exists(input))
            {
                Console.WriteLine("目录不存在！点击回车继续！");
                return;
            }
            DirectoryInfo di = new DirectoryInfo(input);
            int num = 0;
            foreach(FileInfo fi in di.GetFiles())
            {
                Console.WriteLine("写入数据：" + fi.Name);
                sqlcom.InsertData(fi.Name, getHash.GetSHA256Hash(fi));
                num++;
            }
            Console.WriteLine($"已写入{num}条数据！点击回车继续！");
        }

        //校验数据
        public void CheckFileHashValue()
        {
            Console.WriteLine("请输入要校验的文件所在的目录：");
            string input = Console.ReadLine();
            if (!Directory.Exists(input))
            {
                Console.WriteLine("目录不存在！点击回车继续！");
                return;
            }
            DirectoryInfo di = new DirectoryInfo(input);
            List<string> diff_file = new List<string>();
            foreach(FileInfo fi in di.GetFiles())
            {
                string cal_hash;
                string ori_hash;
                Console.WriteLine($"正在校验{fi.Name}的Hash值");
                Console.WriteLine($"{fi.Name}的计算Hash值为：{cal_hash=getHash.GetSHA256Hash(fi)}");
                ori_hash = sqlcom.GetHashValue(fi.Name);
                if (string.IsNullOrEmpty(ori_hash))
                    Console.WriteLine($"{fi.Name}在数据库中不存在！");
                else
                    Console.WriteLine($"{fi.Name}的原有Hash值为：{ori_hash=sqlcom.GetHashValue(fi.Name)}");
                if (string.Compare(cal_hash, ori_hash) == 0)
                    Console.WriteLine("两文件的Hash比较结果相同！\n");
                else
                {
                    Console.WriteLine("两文件的比较结果不同！\n");
                    diff_file.Add(fi.FullName);
                }
            }
            Console.WriteLine("已完成文件校验，Hash不同的文件数：" + diff_file.Count);
            foreach (string str in diff_file)
                Console.WriteLine(str);
            Console.WriteLine("\n校验完成！点击回车继续！");
        }

        //退出程序
        public void ExitProgram()
        {
            sqlcom.CloseDataBase();
            Environment.Exit(0);
        }
    }
}
