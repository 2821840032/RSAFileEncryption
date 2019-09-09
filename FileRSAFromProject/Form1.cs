using RSACryption;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRSAFromProject
{
    public partial class Form1 : Form
    {
        string prKey, PuKey;
        public Form1()
        {
            InitializeComponent();
            RSAChange();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var path = DiaShowFilePath("All files (*.*)|*.*");
            if (path is null)
            {
                return;
            }
            Task.Run(async () =>
            {
                var topath = path + ".RSAEncrypted";
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    file.Seek(0, SeekOrigin.Begin);
                    await RSACryptionTool.RsaEncrypt(PuKey, file, ChangeProgressBar);
                }
                FileHelpTool.CopyToFile(FileHelpTool.ProgramPath, topath);
            });
        }
        /// <summary>
        /// 打开文件选择框
        /// </summary>
        private string DiaShowFilePath(string filter) {
            openFileDialog1.Filter = filter;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
               return openFileDialog1.FileName;          //显示文件路径
            }
            return null;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var path = DiaShowFilePath("Rsa File (*.RSAEncrypted)|*.RSAEncrypted");
            var topath = path.Replace(".RSAEncrypted", "");
            if (path is null)
            {
                return;
            }
            Task.Run(async () =>
            {
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    file.Seek(0, SeekOrigin.Begin);
                    await RSACryptionTool.RsaDecrypt(prKey, file, ChangeProgressBar);
                }
                FileHelpTool.CopyToFile(FileHelpTool.ProgramPath, topath);
            });
        }

        delegate void SetTextCallback(Action SetUIFunc);

        private void SetUI(Action SetUIFunc)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.Disposing || this.IsDisposed)
                        return;
                }
                SetTextCallback d = new SetTextCallback(SetUI);
                this.Invoke(d, new object[] { SetUIFunc });
            }
            else
            {
                SetUIFunc.Invoke();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            prKey = PuKeyText.Text;
            prKey = PrKeyText.Text;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            RSAChange();
        }
        public async void RSAChange() {
            await Task.Yield();
            RSACryptionTool.BuildRsaKey(out prKey, out PuKey);
            SetUI(() =>
            {
                PuKeyText.Text = prKey;
                PrKeyText.Text = prKey;
            });
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="remainingProgress">剩余值</param>
        /// <param name="maxProgress">最大值</param>
        private void ChangeProgressBar(long remainingProgress,long maxProgress) {
            SetUI(() =>
            {
                this.SpeedProgress.Text = (maxProgress - remainingProgress).ToString();
                this.MaxProgress.Text = maxProgress.ToString();
                this.progressBar.Value =(int)(((double)(maxProgress - remainingProgress) / (double)maxProgress)*100);
            });
           
        }
    }
}
