using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicWormCore;

namespace ComicWorm
{
    public class DownloadTaskManager
    {
        private List<DownloadModel> downloadTasks;
        private List<DownloadModel> downloadingTasks;
        private List<DownloadModel> downloadedTasks;
        private static readonly int MaxThreadNumber = 5;

        public DownloadTaskManager()
        {
            this.downloadTasks = new List<DownloadModel>();
            this.downloadingTasks = new List<DownloadModel>();
            this.downloadedTasks = new List<DownloadModel>();
        }

        public void AddDownloadTask(DownloadModel dm)
        {
            this.downloadTasks.Add(dm);
        }

        public async Task StartGetChaptersTasks()
        {
            List<Task<DownloadModel>> tasks = new List<Task<DownloadModel>>();
            while (true)
            {
                if (tasks.Count < MaxThreadNumber)
                {
                    DownloadModel dm = GetDifferentDownloadModel();
                    if (dm != null)
                    {
                        tasks.Add(dm.GetChaptersAsync());
                        continue;
                    }
                }

                if (tasks.Count == 0)
                {
                    break;
                }

                Task<DownloadModel> t = await Task.WhenAny(tasks);
                tasks.Remove(t);
                DownloadModel dm2 = await t;
                this.downloadingTasks.Remove(dm2);
                this.downloadedTasks.Add(dm2);
            }

            ResetDownloadTasks();
        }

        private DownloadModel GetDifferentDownloadModel()
        {
            DownloadModel ddm = null;
            foreach (DownloadModel dm in this.downloadTasks)
            {
                if (this.downloadingTasks.Count == 0)
                {
                    ddm = dm;
                    break;
                }
                foreach (DownloadModel dingm in this.downloadingTasks)
                {
                    if (!dingm.IsSameWebset(dm))
                    {
                        ddm = dm;
                        break;
                    }
                }
                if (ddm != null)
                {
                    break;
                }
            }
            if (ddm != null)
            {
                this.downloadTasks.Remove(ddm);
                return ddm;
            }
            else
            {
                return null;
            }
        }

        private void ResetDownloadTasks()
        {
            foreach (DownloadModel dm in this.downloadedTasks)
            {
                this.downloadTasks.Add(dm);
            }
            this.downloadedTasks.Clear();
        }

        public async Task StartGetPagesTasks()
        {
            List<Task<DownloadModel>> tasks = new List<Task<DownloadModel>>();
            while (true)
            {
                if (tasks.Count < MaxThreadNumber)
                {
                    DownloadModel dm = GetDifferentDownloadModel();
                    if (dm != null)
                    {
                        tasks.Add(dm.GetPagesAsync());
                        continue;
                    }
                }
                Task<DownloadModel> t = await Task.WhenAny(tasks);
                tasks.Remove(t);
                DownloadModel dm2 = await t;
                this.downloadingTasks.Remove(dm2);
                this.downloadedTasks.Add(dm2);

                if (tasks.Count == 0 && this.downloadTasks.Count == 0)
                {
                    break;
                }
            }

            ResetDownloadTasks();
        }

        public async Task StartDownloadComicsTasks()
        {

            List<Task<DownloadModel>> tasks = new List<Task<DownloadModel>>();
            while (true)
            {
                if (tasks.Count < MaxThreadNumber)
                {
                    DownloadModel dm = GetDifferentDownloadModel();
                    if (dm != null)
                    {
                        tasks.Add(dm.DownloadComicAsync());
                        continue;
                    }
                }
                Task<DownloadModel> t = await Task.WhenAny(tasks);
                tasks.Remove(t);
                DownloadModel dm2 = await t;
                this.downloadingTasks.Remove(dm2);
                this.downloadedTasks.Add(dm2);

                if (tasks.Count == 0 && this.downloadTasks.Count == 0)
                {
                    break;
                }
            }

            ResetDownloadTasks();
        }
    }
}
