using System;
using System.IO;
using System.Net;
using System.Text;

namespace BT.Manage.Tools.Helper
{
    public class FtpUpDown
    {
        private string ftpPassword;
        private string ftpServerIP;

        private string ftpUserID;
        private FtpWebRequest reqFTP;

        public FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;

            this.ftpUserID = ftpUserID;

            this.ftpPassword = ftpPassword;
        }

        /// <summary>
        /// 请求指定url，并将内容写入D:/File文件夹中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HttpFeil(string path, string name)
        {
            string paths = "D:/File";
            bool flag = false;
            try
            {
                if (!Directory.Exists(paths))
                {
                    Directory.CreateDirectory(paths);
                }
                using (FileStream fs = new FileStream(paths + "/" + name.Replace("|", ""), FileMode.Create, FileAccess.Write))
                {
                    //创建请求
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
                    //接收响应
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //输出流
                    Stream responseStream = response.GetResponseStream();
                    byte[] bufferBytes = new byte[10000];//缓冲字节数组
                    int bytesRead = -1;
                    while ((bytesRead = responseStream.Read(bufferBytes, 0, bufferBytes.Length)) > 0)
                    {
                        fs.Write(bufferBytes, 0, bytesRead);
                    }
                    if (fs.Length > 0)
                    {
                        flag = true;
                    }
                    //关闭写入
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception exp)
            {
                //返回错误消息
            }
            return flag;
            //return responseStr;
        }

        /// <summary>
        /// http请求指定url，返回内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string HttpVideo(string path)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(path);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 100000;
            request.AllowAutoRedirect = false;
            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = "";
            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write("");
                requestStream.Close();
                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }
            return responseStr;
        }

        /// <summary>
        /// 移除ftp指定目录
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public bool delDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;

                Connect(uri);//连接

                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除ftp上指定文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteFileName(string fileName)
        {
            try
            {
                FileInfo fileInf = new FileInfo(fileName);

                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;

                Connect(uri);//连接

                // 默认为true，连接不会被关闭

                // 在一个命令之后被执行

                reqFTP.KeepAlive = false;

                // 指定执行什么命令

                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从FTP服务器下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="errorinfo"></param>
        /// <returns></returns>
        public bool Download(string url, string filePath, string fileName, out string errorinfo)////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                String onlyFileName = Path.GetFileName(fileName);

                string newFileName = filePath + "\\" + onlyFileName;

                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                    //errorinfo = string.Format("本地文件{0}已存在,无法下载", newFileName);
                    //return false;
                }
                // string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//连接
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;

                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                long totalDownloadedByte = 0;

                while (readCount > 0)
                {
                    totalDownloadedByte += readCount;
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                errorinfo = "";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},无法下载", ex.Message);

                return false;
            }
        }

        /// <summary>
        /// 根据地址获取ftp服务器上对应文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFileList(string path)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }

        public string[] GetFileList()//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory);
        }

        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        /// <summary>
        /// 根据地质获取ftp服务器上对应详细文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFilesDetailList(string path)
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        /// <summary>
        /// 获取ftp文件大小
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public long GetFileSize(string filename)
        {
            long fileSize = 0;

            try
            {
                FileInfo fileInf = new FileInfo(filename);

                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;

                Connect(uri);//连接

                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                fileSize = response.ContentLength;

                response.Close();
            }
            catch
            {
            }

            return fileSize;
        }

        /// <summary>
        /// 从ftp上获取文件流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ftpOutStream"></param>
        /// <returns></returns>
        public bool GetImageStream(string url, out Stream ftpOutStream)////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                Connect(url);//连接
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                ftpOutStream = new MemoryStream();
                while (readCount > 0)
                {
                    ftpOutStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                // ftpOutStream.Close();
                response.Close();

                return true;
            }
            catch (Exception ex)
            {
                ftpOutStream = null;
                return false;
            }
        }

        /// <summary>
        /// FTP服务器创建目录
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public bool MakeDir(string dirName)
        {
            try
            {
                Connect(dirName);//连接

                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改ftp文件名称
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        /// <returns></returns>
        public bool Rename(string currentFilename, string newFilename)
        {
            try
            {
                FileInfo fileInf = new FileInfo(currentFilename);

                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;

                Connect(uri);//连接

                reqFTP.Method = WebRequestMethods.Ftp.Rename;

                reqFTP.RenameTo = newFilename;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                //Stream ftpStream = response.GetResponseStream();

                //ftpStream.Close();

                response.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从ftp服务器上载文件的功能
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Upload(string uri, string filename) //上面的代码实现了从ftp服务器上载文件的功能
        {
            FileInfo fileInf = new FileInfo(filename);
            Connect(uri);//连接

            // 默认为true，连接不会被关闭

            // 在一个命令之后被执行

            reqFTP.KeepAlive = false;

            // 指定执行什么命令

            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // 上传文件时通知服务器文件的大小

            reqFTP.ContentLength = fileInf.Length;
            // 缓冲大小设置为kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流(System.IO.FileStream) 去读上传的文件
            FileStream fs = fileInf.OpenRead();
            // long totalDownloadedByte = 0;

            try
            {
                // 把上传的文件写入流

                Stream strm = reqFTP.GetRequestStream();

                // 每次读文件流的kb

                contentLen = fs.Read(buff, 0, buffLength);

                // 流内容没有结束

                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();

                fs.Close();

                //文件转换

                return true;
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal("上传文件错误" + ex.Message);
                return false;
                throw ex;
            }
        }

        private void Connect(String path)//连接ftp
        {
            // 根据uri创建FtpWebRequest对象

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));

            // 指定数据传输类型

            reqFTP.UseBinary = true;

            // ftp用户名和密码

            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        //都调用这个

        private string[] GetFileList(string path, string WRMethods)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            try
            {
                Connect(path);

                reqFTP.Method = WRMethods;

                WebResponse response = reqFTP.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名

                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }

                // to remove the trailing '\n'

                result.Remove(result.ToString().LastIndexOf('\n'), 1);

                reader.Close();

                response.Close();

                return result.ToString().Split('\n');
            }
            catch
            {
                downloadFiles = null;

                return downloadFiles;
            }
        }

        //删除文件
        //创建目录
        //删除目录
        //获得文件大小
        //文件改名
        //获得文件明晰
        //获得文件明晰
    }
}