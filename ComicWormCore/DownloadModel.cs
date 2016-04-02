using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ComicWormCore
{
    public class DownloadModel
    {
        private static object LockLog = new object();
        private static object LockLogFile = new object();
        private static readonly string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2691.0 Safari/537.36";
        private static readonly int DownloadTimeOut = 30;
        private static readonly int RetryWaitTime = 30;
        private static readonly int WaitTime = 1;
        private static readonly int RetryTimes = 5;

        public ComicModel Comic { get; set; }
        private Database database { get; }

        public DownloadModel(string _name, string _url)
        {
            this.Comic = new ComicModel();
            this.Comic.Name = _name;
            this.Comic.Url = _url;
            this.database = new Database();
        }

        public async Task UpdateComicAsync()
        {
            await GetChapters();
            await GetPages();
        }

        private async Task GetChapters()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(DownloadTimeOut);
                httpClient.DefaultRequestHeaders.Add("UserAgent", UserAgent);
                int retryCounter = 0;
                while (true)
                {
                    retryCounter++;
                    Stream stream = null;
                    try
                    {
                        Log("开始获取[" + this.Comic.Name + "]章节信息，URL地址[" + this.Comic.Url + "]");
                        stream = await httpClient.GetStreamAsync(this.Comic.Url).ConfigureAwait(false);
                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.Load(stream, true);

                        AnalysisChapter(htmlDoc, this.Comic);

                        Log("获取[" + this.Comic.Name + "]章节信息成功");

                        await Task.Delay(TimeSpan.FromSeconds(WaitTime));

                        break;
                    }
                    catch (Exception e)
                    {
                        Log(e);
                        DumpToFile(stream, this.Comic.Url);
                    }
                    finally
                    {
                        stream.Dispose();
                    }

                    if (retryCounter > RetryTimes)
                    {
                        Log("超过最大重试次数，URL地址[" + this.Comic.Url + "]");
                        break;
                    }
                }

                Log("发生错误，等待" + RetryWaitTime + "s后重试");
                await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
            }
        }

        public delegate void AnalysisChapterHandler(HtmlDocument htmlDoc, ComicModel comic);
        public event AnalysisChapterHandler AnalysisChapter;

        private async void DumpToFile(Stream s, string url)
        {
            StreamWriter swDump = null, swLog = null;
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, "dump");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = DateTime.Now.ToString("G") + " " + url;
                swDump = new StreamWriter(Path.Combine(path, Utls.GetMD5(fileName) + ".dump"));
                s.CopyTo(swDump.BaseStream);
                await swDump.FlushAsync();

                swLog = new StreamWriter(Path.Combine(path, Utls.GetMD5(fileName) + ".log"));
                await swLog.WriteLineAsync(url);
                await swLog.FlushAsync();

                Log("已保存[" + Path.Combine(path, Utls.GetMD5(fileName) + ".dump") + "]");
            }
            catch (Exception e)
            {
                Log(e);
                Log("流保存失败");
            }
            finally
            {
                swDump.Dispose();
                swLog.Dispose();
            }
        }

        private async Task GetPages()
        {
            foreach (ChapterModel chapter in this.Comic.Chapters)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(DownloadTimeOut);
                    httpClient.DefaultRequestHeaders.Add("UserAgent", UserAgent);
                    int retryCounter = 0;
                    while (true)
                    {
                        retryCounter++;
                        Stream stream = null;
                        try
                        {
                            Log("开始获取[" + chapter.Name + "]页面信息，URL地址[" + chapter.Url + "]");
                            stream = await httpClient.GetStreamAsync(chapter.Url).ConfigureAwait(false);

                            HtmlDocument htmlDoc = new HtmlDocument();
                            htmlDoc.Load(stream, true);

                            AnalysisPage(htmlDoc, chapter);

                            Log("获取[" + chapter.Name + "]页面信息成功");

                            await Task.Delay(TimeSpan.FromSeconds(WaitTime));

                            break;

                        }
                        catch (Exception e)
                        {
                            Log(e);
                            DumpToFile(stream, this.Comic.Url);
                        }

                        if (retryCounter > RetryTimes)
                        {
                            Log("超过最大重试次数，URL地址[" + chapter.Url + "]");
                            break;
                        }

                        Log("发生错误，等待" + RetryWaitTime + "s后重试");
                        await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
                    }
                }
            }
        }

        public delegate void AnalysisPageHandler(HtmlDocument htmlDoc, ChapterModel chapter);
        public event AnalysisPageHandler AnalysisPage;

        public async Task DownloadComicAsync()
        {
            foreach (ChapterModel chapter in this.Comic.Chapters)
            {
                if (chapter.Downloaded)
                {
                    Log("已获取[" + chapter.Name + "]，跳过获取");
                    continue;
                }

                bool flag = true;
                foreach (PageModel page in chapter.Pages)
                {
                    if (page.Downloaded)
                    {
                        Log("已获取[" + chapter.Name + "]，第[" + page.Number + "]页，跳过获取");
                        continue;
                    }

                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(DownloadTimeOut);
                        httpClient.DefaultRequestHeaders.Add("UserAgent", UserAgent);
                        int counter = 0;
                        while (true)
                        {
                            counter++;
                            try
                            {
                                Log("开始获取[" + chapter.Name + "]，第[" + page.Number + "]页，URL地址[" + page.Url + "]");

                                byte[] picData = await httpClient.GetByteArrayAsync(page.Url).ConfigureAwait(false);
                                string comicName = this.Comic.Name + "_" + this.Comic.MD5;
                                string chapterName = chapter.Number.ToString("D3") + "_" + chapter.Name;
                                string path = Path.Combine(Environment.CurrentDirectory, "comic", comicName, chapterName);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                using (FileStream fileStream = new FileStream(Path.Combine(path, page.Number + ".jpg"), FileMode.Create))
                                {
                                    await fileStream.WriteAsync(picData, 0, picData.Length);
                                    await fileStream.FlushAsync();
                                }

                                this.database.AddPage(page, this.Comic.MD5, chapter.MD5);

                                Log("获取[" + chapter.Name + "]，第[" + page.Number + "]页成功");

                                await Task.Delay(TimeSpan.FromSeconds(WaitTime));

                                break;
                            }
                            catch (Exception e)
                            {
                                Log(e);
                                Log("获取[" + chapter.Name + "]，第[" + page.Number + "]页成功，URL地址[" + page.Url + "]");
                            }

                            if (counter > RetryTimes)
                            {
                                flag = false;
                                Log("超过最大重试次数，URL地址[" + page.Url + "]");
                                break;
                            }

                            Log("发生错误，等待" + RetryWaitTime + "s后重试");
                            await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
                        }
                    }
                    if (flag)
                    {
                        database.AddChapter(chapter, this.Comic.Url);
                    }
                }
            }
        }

        private void Log(Exception e)
        {
            lock (LockLog)
            {
                string name = (new StackTrace()).GetFrame(1).GetMethod().ReflectedType.Name;
                string log = DateTime.Now.ToString("G") + " [" + name + "]发生错误";
                Console.WriteLine(log);
                LogToFile(log);
                Console.WriteLine(e.ToString());
            }
        }

        private void Log(string str)
        {
            lock (LockLog)
            {
                string name = (new StackTrace()).GetFrame(1).GetMethod().ReflectedType.Name;
                Console.WriteLine(DateTime.Now.ToString("G") + " [" + name + "]" + str);
            }
        }

        private void LogToFile(string str)
        {
            StreamWriter sw = null;
            try
            {
                string logPath = Path.Combine(Environment.CurrentDirectory, "log");
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                sw = new StreamWriter(Path.Combine(logPath, DateTime.Now.ToLongDateString() + ".log"), true);
                sw.WriteLine();
                sw.Flush();
            }
            catch
            {
                string name = (new StackTrace()).GetFrame(0).GetMethod().ReflectedType.Name;
                Console.WriteLine(DateTime.Now.ToString("G") + " [" + name + "]log文件保存失败");
            }
            finally
            {
                sw.Dispose();
            }
        }
    }
}
