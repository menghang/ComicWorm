using System;
using System.Collections.Generic;
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
        public static object LockDatabase = new object();
        public static object LockLog = new object();

        private static readonly string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2691.0 Safari/537.36";
        private static readonly int DownloadTimeOut = 30;
        private static readonly int RetryWaitTime = 30;
        private static readonly int WaitTime = 1;
        private static readonly int RetryTimes = 5;

        private ComicModel comic;

        public DownloadModel(string _name, string _url)
        {
            this.comic = new ComicModel();
            this.comic.Name = _name;
            this.comic.Url = _url;
        }

        public async Task DownloadAsync()
        {
            await GetChapters();
            await GetPages();
            await DownloadComic();
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
                        stream = await httpClient.GetStreamAsync(this.comic.Url).ConfigureAwait(false);
                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.Load(stream, true);
                        
                        AnalysisChapter(htmlDoc,this.comic);

                        break;
                    }
                    catch (Exception e)
                    {
                        Log(e);
                    }
                    finally
                    {
                        stream.Dispose();
                    }

                    if (retryCounter > RetryTimes)
                    {
                        break;
                    }
                }

                Log("等待" + RetryWaitTime + "s");
                await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
            }
        }

        public delegate void AnalysisChapterHandler(HtmlDocument htmlDoc, ComicModel comic);
        public event AnalysisChapterHandler AnalysisChapter;

        private async Task GetPages()
        {
            foreach (ChapterModel chapter in this.comic.Chapters)
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
                            stream = await httpClient.GetStreamAsync(chapter.Url).ConfigureAwait(false);

                            HtmlDocument htmlDoc = new HtmlDocument();
                            htmlDoc.Load(stream, true);

                            AnalysisPage(htmlDoc, chapter);

                            break;

                        }
                        catch (Exception e)
                        {
                            Log(e);
                        }

                        if (retryCounter > RetryTimes)
                        {
                            break;
                        }

                        Log("等待" + RetryWaitTime + "s");
                        await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
                    }
                }
            }
        }

        public delegate void AnalysisPageHandler(HtmlDocument htmlDoc, ChapterModel chapter);
        public event AnalysisPageHandler AnalysisPage;

        private async Task DownloadComic()
        {
            foreach (ChapterModel chapter in this.comic.Chapters)
            {
                foreach (PageModel page in chapter.Pages)
                {
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
                                byte[] picData = await httpClient.GetByteArrayAsync(page.Url).ConfigureAwait(false);
                                string comicName = this.comic.Name + "_" + this.comic.MD5;
                                string chapterName = chapter.Number.ToString("D3") + "_" + chapter.Name;
                                string path = Path.Combine(Environment.CurrentDirectory, comicName, chapterName);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                using (FileStream fileStream = new FileStream(Path.Combine(path, page.Number + ".jpg"), FileMode.Create))
                                {
                                    await fileStream.WriteAsync(picData, 0, picData.Length);
                                    await fileStream.FlushAsync();
                                }
                                break;
                            }
                            catch (Exception e)
                            {
                                Log(e);
                            }

                            if (counter > RetryTimes)
                            {
                                break;
                            }

                            Log("等待" + RetryWaitTime + "s");
                            await Task.Delay(TimeSpan.FromSeconds(RetryWaitTime));
                        }
                    }
                }
            }
        }

        private void Log(Exception e)
        {

        }

        private void Log(string str)
        {

        }
    }
}
