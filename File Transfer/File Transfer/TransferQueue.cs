using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace File_Transfer
{
    public enum QueueType : byte
    {
        Download,
        upload
    }

    public class TransferQueue
    {
        public static TransferQueue createUploadQueue(TransferClient client, string fileName)
        {
            try
            {
                var queue = new TransferQueue();
                queue.FileName = Path.GetFileName(fileName);
                queue.Client = client;
                queue.type = QueueType.upload;
                queue.Fs = new FileStream(fileName,FileMode.Open);
                queue.Thread = new Thread(new ParameterizedThreadStart(transferProc));
                queue.Thread.IsBackground = true;
                queue.ID = Program.Rand.Next();
                queue.Length = queue.Fs.Length;
                return queue;
            }
            catch
            {
                return null;   
            }
        }

        public static TransferQueue CreateDownloadQueue(TransferClient client,int id,string saveName,long length)
        {
            try
            {
                var queue = new TransferQueue();
                queue.FileName = Path.GetFileName(saveName);
                queue.Client = client;
                queue.type = QueueType.Download;
                queue.Fs = new FileStream(saveName,FileMode.Create);
                queue.Fs.SetLength(length);
                queue.Length = length;
                queue.ID = id;
                return queue;

            }
            catch 
            {
                return null;
                
            }
        }

         
        private const int FILE_BUFFER_SIZE = 8175;
        private static byte[] file_buffer = new byte[FILE_BUFFER_SIZE];

        private ManualResetEvent pauseEvent;

        public int ID;
        public int Progress, LastProgress;

        public long Transferred;
        public long Index;
        public long Length;

        public bool Running;
        public bool Paused;

        public string FileName;

        public QueueType type;

        public TransferClient Client;
        public Thread Thread;
        public FileStream Fs;

        private TransferQueue()
        {
            pauseEvent = new ManualResetEvent(true);
            Running = true;
        }

        public void Start()
        {
            Running = true;
            Thread.Start(this);
        }

        public void Stop()
        {
            Running = false;
        }

        public void Pause()
        {
            if (!Paused)
            {
                pauseEvent.Reset();
            }
            else
            {
                pauseEvent.Set();
            }
            Paused = !Paused;
        }

        public void Close()
        {
            try
            {
                Client.Transfers.Remove(ID);
            }

            catch
            { }
            Running = false;
            Fs.Close();
            pauseEvent.Dispose();
            Client = null;
        }

        public void Write(byte[] bytes, long index)
        {
            lock(this)
            {
                Fs.Position = index;
                Fs.Write(bytes, 0, bytes.Length);
                Transferred += bytes.Length;
            }
        }

        public static void transferProc(Object o)
        {
            TransferQueue queue=(TransferQueue)o;

            while (queue.Running && queue.Index < queue.Length)
            {
                queue.pauseEvent.WaitOne();

                if (!queue.Running)
                {
                    break;
                }

                lock (file_buffer)
                {
                    queue.Fs.Position = queue.Index;
                    int read = queue.Fs.Read(file_buffer, 0, file_buffer.Length);
                    PacketWriter pw = new PacketWriter();
                    pw.Write((byte)Headers.Chunk);
                    pw.Write(queue.ID);
                    pw.Write(queue.Index);
                    pw.Write(read);
                    pw.Write(file_buffer, 0, read);

                    queue.Transferred += read;
                    queue.Index += read;

                    queue.Client.Send(pw.GetBytes());

                    queue.Progress = (int)((queue.Transferred * 100) / queue.Length);
                    if (queue.LastProgress < queue.Progress)
                    {
                        queue.LastProgress = queue.Progress;
                        queue.Client.callProgressChanged(queue);

                    }
                    Thread.Sleep(1); 
                }
            }
            queue.Close();
        }
    }
}
