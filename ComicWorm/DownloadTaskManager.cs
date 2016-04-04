using System.Collections.Generic;
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
                        this.downloadTasks.Remove(dm);
                        this.downloadingTasks.Add(dm);
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
                        this.downloadTasks.Remove(dm);
                        this.downloadingTasks.Add(dm);
                        tasks.Add(dm.GetPagesAsync());
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
                        this.downloadTasks.Remove(dm);
                        this.downloadingTasks.Add(dm);
                        tasks.Add(dm.DownloadComicAsync());
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
            foreach (DownloadModel dm in this.downloadTasks)
            {
                bool isDiffWebset = true;
                foreach (DownloadModel dm2 in this.downloadingTasks)
                {
                    if (dm2.IsSameWebset(dm))
                    {
                        isDiffWebset = false;
                    }
                }
                if (isDiffWebset)
                {
                    return dm;
                }
            }
            return null;
        }

        private void ResetDownloadTasks()
        {
            foreach (DownloadModel dm in this.downloadedTasks)
            {
                this.downloadTasks.Add(dm);
            }
            this.downloadedTasks.Clear();
        }
    }
}
