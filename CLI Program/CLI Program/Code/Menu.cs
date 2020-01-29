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
            if (OverAllData.if_traversal)
                Console.WriteLine("  0. 更改是否遍历（当前：遍历）\n");
            else
                Console.WriteLine("  0. 更改是否遍历（当前：不遍历）\n");
            Console.WriteLine("  1. 创建数据库\n");
            Console.WriteLine("  2. 加入数据\n");
            Console.WriteLine("  3. 校验数据\n");
            Console.WriteLine("  4. 清空数据库\n");
            Console.WriteLine("  5. 列出数据库\n");
            Console.WriteLine("  6. 退出程序\n");
            ExcuteOrder();
        }

        public void ExcuteOrder()
        {
            int command = -1;
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
                case 0:
                    ChangeIfTraversal();
                    break;
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
                    ClearDataBase();
                    Console.WriteLine("已清空数据库！点击回车继续！");
                    Console.ReadLine();
                    break;
                case 5:
                    ShowDataBase();
                    Console.WriteLine("数据库列出成功！点击回车继续！");
                    Console.ReadLine();
                    break;
                case 6:
                    ExitProgram();
                    break;
            }
            ShowMenu();
        }

        //更改是否遍历
        public void ChangeIfTraversal()
        {
            if (OverAllData.if_traversal == false)
            {
                OverAllData.if_traversal = true;
                Console.WriteLine("已更改是否遍历为：true\n点击回车继续！");
                Console.ReadLine();
            }
            else
            {
                OverAllData.if_traversal = false;
                Console.WriteLine("已更改是否遍历为：false\n点击回车继续！");
                Console.ReadLine();
            }
        }

        //加入数据
        public void AddFolderData()
        {
            Console.WriteLine("请输入要读入的目录：");
            string input = Console.ReadLine();
            if (!Directory.Exists(input))
            {
                Console.WriteLine("目录不存在！点击回车继续！");
                return;
            }
            int num = 0;
            //如果设定遍历目录
            if (OverAllData.if_traversal)
            {
                List<FileInfo> all_file = new List<FileInfo>();
                GetAllFileListFromFolder(input, all_file);
                foreach(FileInfo fi in all_file)
                {
                    Console.WriteLine("写入数据：" + fi.Name);
                    sqlcom.InsertData(fi.Name, getHash.GetSHA256Hash(fi));
                    num++;
                }
            }
            //不遍历目录
            else
            {
                DirectoryInfo di = new DirectoryInfo(input);
                foreach (FileInfo fi in di.GetFiles())
                {
                    Console.WriteLine("写入数据：" + fi.Name);
                    sqlcom.InsertData(fi.Name, getHash.GetSHA256Hash(fi));
                    num++;
                }
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
            List<string> diff_file = new List<string>();
            //如果设定遍历目录
            if (OverAllData.if_traversal)
            {
                List<FileInfo> all_file = new List<FileInfo>();
                GetAllFileListFromFolder(input, all_file);
                foreach (FileInfo fi in all_file)
                {
                    string cal_hash;
                    string ori_hash;
                    Console.WriteLine($"正在校验{fi.Name}的Hash值");
                    Console.WriteLine($"{fi.Name}的计算Hash值为：{cal_hash = getHash.GetSHA256Hash(fi)}");
                    ori_hash = sqlcom.GetHashValue(fi.Name);
                    if (string.IsNullOrEmpty(ori_hash))
                        Console.WriteLine($"{fi.Name}在数据库中不存在！");
                    else
                        Console.WriteLine($"{fi.Name}的原有Hash值为：{ori_hash = sqlcom.GetHashValue(fi.Name)}");
                    if (string.Compare(cal_hash, ori_hash) == 0)
                        Console.WriteLine("两文件的Hash比较结果相同！\n");
                    else
                    {
                        Console.WriteLine("两文件的比较结果不同！\n");
                        diff_file.Add(fi.FullName);
                    }
                }
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(input);
                foreach (FileInfo fi in di.GetFiles())
                {
                    string cal_hash;
                    string ori_hash;
                    Console.WriteLine($"正在校验{fi.Name}的Hash值");
                    Console.WriteLine($"{fi.Name}的计算Hash值为：{cal_hash = getHash.GetSHA256Hash(fi)}");
                    ori_hash = sqlcom.GetHashValue(fi.Name);
                    if (string.IsNullOrEmpty(ori_hash))
                        Console.WriteLine($"{fi.Name}在数据库中不存在！");
                    else
                        Console.WriteLine($"{fi.Name}的原有Hash值为：{ori_hash = sqlcom.GetHashValue(fi.Name)}");
                    if (string.Compare(cal_hash, ori_hash) == 0)
                        Console.WriteLine("两文件的Hash比较结果相同！\n");
                    else
                    {
                        Console.WriteLine("两文件的比较结果不同！\n");
                        diff_file.Add(fi.FullName);
                    }
                }
            }
            //不遍历目录
            Console.WriteLine("已完成文件校验，Hash不同的文件数：" + diff_file.Count);
            foreach (string str in diff_file)
                Console.WriteLine(str);
            Console.WriteLine("\n校验完成！点击回车继续！");
        }

        //清空数据库
        public void ClearDataBase()
        {
            sqlcom.CloseDataBase();
            File.Delete(OverAllData.database_path);
            sqlcom.OpenDataBase();
        }

        //浏览数据库
        public void ShowDataBase()
        {
            Console.Clear();
            sqlcom.GetAllDataFromDB();
        }

        //退出程序
        public void ExitProgram()
        {
            sqlcom.CloseDataBase();
            Environment.Exit(0);
        }

        //获取文件夹中的所有数据
        public void GetAllFileListFromFolder(string path, List<FileInfo> all_file)
        {
            Queue<DirectoryInfo> temp_folder = new Queue<DirectoryInfo>();
            temp_folder.Enqueue(new DirectoryInfo(path));
            while (temp_folder.Count != 0)
            {
                DirectoryInfo fn = temp_folder.Dequeue();
                foreach (DirectoryInfo di in fn.GetDirectories())
                    temp_folder.Enqueue(di);
                foreach (FileInfo fi in fn.GetFiles())
                    all_file.Add(fi);
            }
        }
    }
}
