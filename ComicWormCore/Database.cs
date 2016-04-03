using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using ComicModels;

namespace ComicWormCore
{
    public class Database
    {
        private const string DatabaseFile = "data.db";
        private static object LockDatabase = new object();

        public ObservableCollection<ComicModel> DatabaseInitialize()
        {
            lock (LockDatabase)
            {
                if (DatabaseBroken())
                {
                    CreateNewDataBase();
                    return new ObservableCollection<ComicModel>();
                }
                else
                {
                    return GetComicsFromDatabase();
                }
            }
        }

        private bool DatabaseBroken()
        {
            if (!File.Exists(DatabaseFile))
            {
                return true;
            }

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + DatabaseFile))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    DataTable dt = sh.GetTableStatus();
                    bool[] flag = { false, false, false };
                    foreach (DataRow dr in dt.Rows)
                    {
                        if ((dr["type"] as string).Equals("table") && (dr["name"] as string).Equals("Comic"))
                        {
                            flag[0] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("table") && (dr["name"] as string).Equals("Chapter"))
                        {
                            flag[1] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("table") && (dr["name"] as string).Equals("Page"))
                        {
                            flag[2] = true;
                            continue;
                        }
                    }
                    if (!flag[0] || !flag[1] || !flag[2])
                    {
                        conn.Close();
                        return true;
                    }

                    DataTable dt1 = sh.GetColumnStatus("Comic");
                    bool[] flag1 = { false, false, false };
                    foreach (DataRow dr in dt1.Rows)
                    {
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Name")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag1[0] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("URL")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag1[1] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Hash")
                            && Convert.ToInt32(dr["pk"]) == 1)
                        {
                            flag1[2] = true;
                            continue;
                        }
                    }
                    if (!flag1[0] || !flag1[1] || !flag1[2])
                    {
                        conn.Close();
                        return true;
                    }

                    DataTable dt2 = sh.GetColumnStatus("Chapter");
                    bool[] flag2 = { false, false, false, false, false };
                    foreach (DataRow dr in dt2.Rows)
                    {
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Comic")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag2[0] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Name")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag2[1] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("integer")
                            && (dr["name"] as string).Equals("Number")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag2[2] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("URL")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag2[3] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Hash")
                            && Convert.ToInt32(dr["pk"]) == 1)
                        {
                            flag2[4] = true;
                            continue;
                        }
                    }
                    if (!flag2[0] || !flag2[1] || !flag2[2] || !flag2[3] || !flag2[4])
                    {
                        conn.Close();
                        return true;
                    }

                    DataTable dt3 = sh.GetColumnStatus("Page");
                    bool[] flag3 = { false, false, false, false, false };
                    foreach (DataRow dr in dt3.Rows)
                    {
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Comic")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag3[0] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Chapter")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag3[1] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("integer")
                            && (dr["name"] as string).Equals("Number")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag3[2] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("URL")
                            && Convert.ToInt32(dr["pk"]) == 0)
                        {
                            flag3[3] = true;
                            continue;
                        }
                        if ((dr["type"] as string).Equals("text")
                            && (dr["name"] as string).Equals("Hash")
                            && Convert.ToInt32(dr["pk"]) == 1)
                        {
                            flag3[4] = true;
                            continue;
                        }
                    }
                    if (!flag3[0] || !flag3[1] || !flag3[2] || !flag3[3] || !flag3[4])
                    {
                        conn.Close();
                        return true;
                    }

                    conn.Close();
                    return false;
                }
            }
        }

        private void CreateNewDataBase()
        {
            if (File.Exists(DatabaseFile))
            {
                File.Delete(DatabaseFile);
            }
            SQLiteConnection.CreateFile(DatabaseFile);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    SQLiteHelper helper = new SQLiteHelper(command);

                    SQLiteTable tb = new SQLiteTable("Comic");
                    tb.Columns.Add(new SQLiteColumn("Name", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("URL", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("Hash", ColType.Text, true, false, true, null));
                    helper.CreateTable(tb);

                    SQLiteTable tb2 = new SQLiteTable("Chapter");
                    tb2.Columns.Add(new SQLiteColumn("Comic", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Name", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Number", ColType.Integer));
                    tb2.Columns.Add(new SQLiteColumn("URL", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Hash", ColType.Text, true, false, true, null));
                    helper.CreateTable(tb2);

                    SQLiteTable tb3 = new SQLiteTable("Page");
                    tb3.Columns.Add(new SQLiteColumn("Comic", ColType.Text));
                    tb3.Columns.Add(new SQLiteColumn("Chapter", ColType.Text));
                    tb3.Columns.Add(new SQLiteColumn("Number", ColType.Integer));
                    tb3.Columns.Add(new SQLiteColumn("URL", ColType.Text));
                    tb3.Columns.Add(new SQLiteColumn("Hash", ColType.Text, true, false, true, null));
                    helper.CreateTable(tb3);

                    connection.Close();
                }
            }
        }

        private ObservableCollection<ComicModel> GetComicsFromDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    SQLiteHelper helper = new SQLiteHelper(command);

                    string cmd = "select * from Comic;";
                    DataTable dt = helper.Select(cmd);
                    ObservableCollection<ComicModel> comics = new ObservableCollection<ComicModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ComicModel comic = new ComicModel();
                        comic.Name = dr["Name"] as string;
                        comic.Url = dr["Url"] as string;
                        comic.Selected = false;
                        comics.Add(comic);
                    }
                    connection.Close();
                    return comics;
                }
            }
        }

        public void AddComic(ComicModel comic)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic["Name"] = comic.Name;
                        dic["URL"] = comic.Url;
                        dic["Hash"] = comic.Hash;

                        helper.Insert("Comic", dic);

                        connection.Close();
                    }
                }
            }
        }

        public void RemoveComic(ComicModel comic)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "delete from Comic where Hash='" + comic.Hash + "';";
                        helper.Execute(cmd);

                        connection.Close();
                    }
                }
            }
        }

        public void RemoveComicRelated(ComicModel comic)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "delete from Chapter where Comic='" + comic.Hash + "';";
                        helper.Execute(cmd);

                        string cmd2 = "delete from Page where Comic='" + comic.Hash + "';";
                        helper.Execute(cmd2);

                        connection.Close();
                    }
                }
            }
        }

        public void AddChapter(ChapterModel chapter, string comic)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic["Comic"] = comic;
                        dic["Name"] = chapter.Name;
                        dic["Number"] = chapter.Number;
                        dic["URL"] = chapter.Url;
                        dic["Hash"] = chapter.Hash;

                        helper.Insert("Chapter", dic);

                        connection.Close();
                    }
                }
            }
        }

        public void RemoveChapter(ChapterModel chapter)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "delete from Chapter where Hash='" + chapter.Hash + "';";
                        helper.Execute(cmd);

                        connection.Close();
                    }
                }
            }
        }

        public void RemoveChapterRelated(ChapterModel chapter)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "delete from Page where Chapter='" + chapter.Hash + "';";
                        helper.Execute(cmd);

                        connection.Close();
                    }
                }
            }
        }

        public void AddPage(PageModel page, string comic, string chapter)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic["Comic"] = comic;
                        dic["Chapter"] = chapter;
                        dic["Number"] = page.Number;
                        dic["URL"] = page.Url;
                        dic["Hash"] = page.Hash;

                        helper.Insert("Page", dic);

                        connection.Close();
                    }
                }
            }
        }

        public bool IsDownloaded(ChapterModel chapter)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "select * from Chapter where Hash='" + chapter.Hash + "';";
                        DataTable dt = helper.Select(cmd);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public bool IsDownloaded(PageModel page)
        {
            lock (LockDatabase)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "select * from Page where Hash='" + page.Hash + "';";
                        DataTable dt = helper.Select(cmd);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
    }
}
