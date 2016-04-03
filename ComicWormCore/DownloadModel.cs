using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AnalysisModels;
using ComicModels;
using HtmlAgilityPack;

namespace ComicWormCore
{
    public class DownloadModel: IEquatable<DownloadModel>
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
        public IAnalysisModel AnalysisModel { get; private set; }
        private Database database { get; }

        public DownloadModel(ComicModel _comic)
        {
            this.Comic = _comic;
            this.database = new Database();
            if (!SetAnalysisModel(GetWebset(this.Comic.Url)))
            {
                Log("找不到对应的解析模型，名称["+this.Comic.Name+"]，URL地址[" + this.Comic.Url + "]");
                throw new NoAnalysisModelException();
            }
        }

        public class NoAnalysisModelException : ApplicationException { }

        private string GetWebset(string _url)
        {
            string str;
            if (_url.StartsWith("http://"))
            {
                str = _url.Remove(0,"http://".Length);
            }
            else if (_url.StartsWith("https://"))
            {
                str = _url.Remove(0,"https://".Length);
            }
            else
            {
                str = _url;
            }

            return (str.Split('/'))[0];
        }

        private bool SetAnalysisModel(string _webset)
        {
            foreach( Type t in Assembly.Load(nameof(AnalysisModels)).GetTypes())
            {
                if (t.GetInterface(nameof(IAnalysisModel)) != null)
                {
                    IAnalysisModel iam = Activator.CreateInstance(t) as IAnalysisModel;
                    if (iam.GetWebset().Equals(_webset))
                    {
                        this.AnalysisModel = iam;
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<DownloadModel> GetChaptersAsync()
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

                        AnalysisModel.GetAnalysisModel().Item2(htmlDoc, this.Comic);

                        foreach(ChapterModel chapter in this.Comic.Chapters)
                        {
                            chapter.Downloaded = this.database.IsDownloaded(chapter);
                            chapter.Selected = !chapter.Downloaded;
                        }

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

                    Log("发生错误，等待" + RetryWaitTime + "s后重试");
                    await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
                }
            }

            return this;
        }

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
                swDump.Close();

                swLog = new StreamWriter(Path.Combine(path, Utls.GetMD5(fileName) + ".log"));
                await swLog.WriteLineAsync(url);
                await swLog.FlushAsync();
                swLog.Close();

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

        public async Task<DownloadModel> GetPagesAsync()
        {
            foreach (ChapterModel chapter in this.Comic.Chapters)
            {
                if (!chapter.Selected)
                {
                    Log("跳过获取[" + chapter.Name + "]页面信息，URL地址[" + chapter.Url + "]");
                    continue;
                }
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

                            AnalysisModel.GetAnalysisModel().Item3(htmlDoc, chapter);

                            foreach(PageModel page in chapter.Pages)
                            {
                                page.Downloaded = this.database.IsDownloaded(page);
                                page.Selected = true;
                            }

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

            return this;
        }

        public async Task<DownloadModel> DownloadComicAsync()
        {
            foreach (ChapterModel chapter in this.Comic.Chapters)
            {
                if (!chapter.Selected)
                {
                    Log("跳过获取[" + chapter.Name + "]，URL地址[" + chapter.Url + "]");
                    continue;
                }

                bool successfulDownloaded = true;
                foreach (PageModel page in chapter.Pages)
                {
                    if (!page.Selected)
                    {
                        Log("跳过获取[" + chapter.Name + "]，第[" + page.Number + "]页，URL地址[" + page.Url + "]");
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
                                string comicName = this.Comic.Name + "_" + this.Comic.Hash;
                                string chapterName = chapter.Number.ToString("D3") + "_" + chapter.Name;
                                string path = Path.Combine(Environment.CurrentDirectory, "comic", comicName, chapterName);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                using (FileStream fileStream = new FileStream(Path.Combine(path, page.Number.ToString("D3") + ".jpg"), FileMode.Create))
                                {
                                    await fileStream.WriteAsync(picData, 0, picData.Length);
                                    await fileStream.FlushAsync();
                                }

                                if (!this.database.IsDownloaded(page))
                                {
                                    this.database.AddPage(page, this.Comic.Hash, chapter.Hash);
                                    page.Downloaded = true;
                                }

                                Log("获取[" + chapter.Name + "]，第[" + page.Number + "]页成功");

                                await Task.Delay(TimeSpan.FromSeconds(WaitTime));

                                break;
                            }
                            catch (Exception e)
                            {
                                Log(e);
                                Log("获取[" + chapter.Name + "]，第[" + page.Number + "]页失败，URL地址[" + page.Url + "]");
                            }

                            if (counter > RetryTimes)
                            {
                                successfulDownloaded = false;
                                Log("超过最大重试次数，URL地址[" + page.Url + "]");
                                break;
                            }

                            Log("发生错误，等待" + RetryWaitTime + "s后重试");
                            await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
                        }
                    }
                }
                if (successfulDownloaded&&!chapter.Downloaded)
                {
                    database.AddChapter(chapter, this.Comic.Url);
                    chapter.Downloaded = true;
                }
            }

            return this;
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
                LogToFile(e.ToString());
            }
        }

        private void Log(string str)
        {
            lock (LockLog)
            {
                string name = (new StackTrace()).GetFrame(1).GetMethod().ReflectedType.Name;
                string str2 = DateTime.Now.ToString("G") + " [" + name + "]" + str;
                Console.WriteLine(str2);
                LogToFile(str2);
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

                sw = new StreamWriter(Path.Combine(logPath, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), true);
                sw.WriteLine(str);
                sw.Flush();
                sw.Close();
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

        public bool Equals(DownloadModel other)
        {
            return this.Comic.Equals(other.Comic);
        }

        public bool IsSameWebset(DownloadModel other)
        {
            return this.AnalysisModel.GetWebset().Equals(other.AnalysisModel.GetWebset());
        }
    }
}
