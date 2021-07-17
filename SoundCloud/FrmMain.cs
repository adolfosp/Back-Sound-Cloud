using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using NAudio.Wave;
using NAudio.WaveFormRenderer;

namespace SoundCloud
{
    public partial class FrmMain : Form
    {
        private IWavePlayer wavePlayer = new WaveOutEvent();
        private AudioFileReader audioFileReader;
        private FileInfo file;

        public FrmMain()
        {
            InitializeComponent();
            grid.Columns[0].DefaultCellStyle.NullValue = null;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {

            bunifuFormDock1.WindowState = BunifuFormDock.FormWindowStates.Minimized;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string item in files)
            {
                FileInfo fi = new FileInfo(item);
                TagLib.File f = TagLib.File.Create(fi.FullName);
                var r = grid.Rows.Add(new object[]
                {
                    null,
                    fi.Name,
                    f.Tag.JoinedGenres,
                    f.Tag.JoinedAlbumArtists,
                    Math.Round(f.Properties.Duration.TotalMinutes,2)+" Mins"

                });

                grid.Rows[r].Tag = fi;
            }



        }

        private void grid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;
            file = ((FileInfo)grid.Rows[e.RowIndex].Tag);

            Play();
        }

        private void Play()
        {
            Application.UseWaitCursor = true;
            Application.DoEvents();
            if (file == null) return;
            audioFileReader = new AudioFileReader(file.FullName);
            if (wavePlayer.PlaybackState != PlaybackState.Stopped)
            {
                wavePlayer.Stop();
            }
            wavePlayer.Init(audioFileReader);
            GenerateWV();
            wavePlayer.Play();
            picPlay.Enabled = true;
            Application.UseWaitCursor = false;
        }

        private void GenerateWV()
        {
            var myRendererSettings = new StandardWaveFormRendererSettings();
            myRendererSettings.Width = pnWaveForm.Width;
            myRendererSettings.TopHeight = pnWaveForm.Height / 2;
            myRendererSettings.BottomHeight = pnWaveForm.Height / 2;
            myRendererSettings.TopPeakPen = new Pen(Color.FromArgb(255, 255, 0));
            myRendererSettings.BottomPeakPen = new Pen(Color.FromArgb(255, 221, 186));
            myRendererSettings.BackgroundColor = this.BackColor;

            var myRendererSettings2 = new StandardWaveFormRendererSettings();
            myRendererSettings2.Width = pnWaveForm.Width;
            myRendererSettings2.TopHeight = pnWaveForm.Height / 2;
            myRendererSettings2.BottomHeight = pnWaveForm.Height / 2;
            myRendererSettings2.TopPeakPen = new Pen(Color.FromArgb(255, 109, 0));
            myRendererSettings2.BottomPeakPen = new Pen(Color.FromArgb(255, 221, 186));
            myRendererSettings2.BackgroundColor = this.BackColor;

            var renderer = new WaveFormRenderer();
            var audioFilePath = file.FullName;
            pnWaveForm.BackgroundImage = renderer.Render(audioFilePath, new AveragePeakProvider(3), myRendererSettings);
            picWv.BackgroundImage = renderer.Render(audioFilePath, new AveragePeakProvider(3), myRendererSettings2);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

            foreach (var item in openFileDialog1.FileNames)
            {
                FileInfo fi = new FileInfo(item);
                TagLib.File f = TagLib.File.Create(fi.FullName);
                var r = grid.Rows.Add(new object[]
                {
                    null,
                    fi.Name,
                    f.Tag.JoinedGenres,
                    f.Tag.JoinedAlbumArtists,
                    Math.Round(f.Properties.Duration.TotalMinutes,2)+" Mins"
                });
                grid.Rows[r].Tag = fi;
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    wavePlayer.Pause();
                    btnPlayPause.Image = iconPlay.Image;
                }
                else
                {
                    wavePlayer.Play();
                    btnPlayPause.Image = iconPause.Image;
                }

            }
            catch (Exception)
            {
                goto a;

            a:
                if (grid.RowCount == 0)
                {
                    openFileDialog1.ShowDialog();
                    if (grid.RowCount > 0) goto a;
                }
                else
                {
                    file = ((FileInfo)grid.CurrentRow.Tag);
                    Play();
                }

            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if(wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                lblMusica.Text = file.Name;
                picPlay.Enabled = true;
                SetSlider();
                if (!lblArtista.Text.Contains("🤘"))
                {
                    lblArtista.Text = "Tocando agora 🤘🤘🤘";
                }
                else if (lblArtista.Text.Contains("🤘🤘🤘"))
                {
                    lblArtista.Text = "Tocando agora 🤘🤘";
                }else if (lblArtista.Text.Contains("🤘🤘"))
                {
                    lblArtista.Text = "Tocando agora 🤘";
                }
                else
                {
                    lblArtista.Text = "Tocando agora";
                }

            }
            else
            {
                if(wavePlayer.PlaybackState == PlaybackState.Stopped)
                {
                    picWv.Width = 0;
                }
                lblMusica.Text = "Player inspirado no SoundCloud";
                picPlay.Enabled = false;
                lblArtista.Text = wavePlayer.PlaybackState.ToString();
            }
        }

        private void SetSlider()
        {
            double max = audioFileReader.Length;
            double cur = audioFileReader.Position;

            var val = (cur * pnWaveForm.Width) / max;
            picWv.Width = int.Parse(Math.Truncate(val).ToString());

        }

        private void txtBusca_KeyUp(object sender, KeyEventArgs e)
        {
            if(txtBusca.Text.Trim().Length != 0 || e.KeyCode == Keys.Enter)
            {
                var indice = 0;
                foreach (DataGridViewRow item in grid.Rows)
                {
                    if (indice < grid.RowCount-1)
                    {
                        item.Visible = grid.Rows[indice].Cells[1].Value.ToString().ToLower().Contains(txtBusca.Text.Trim().ToLower());

                        indice++;
                    }
                }

               
            }
        }

        private void btnPlaylist_Click(object sender, EventArgs e)
        {
            grid.Visible = !grid.Visible;
        }

        private void grid_VisibleChanged(object sender, EventArgs e)
        {
            this.Height = grid.Visible ? 695 : 340;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                if (grid.CurrentRow.Index == 0)
                {
                    grid.CurrentCell = grid.Rows[grid.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[1];
                }
                else
                {
                    grid.CurrentCell = grid.Rows[grid.CurrentRow.Index - 1].Cells[1];
                }
                file = ((FileInfo)grid.CurrentRow.Tag);
                Play();
            }
            catch (Exception)
            {

            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (grid.CurrentRow.Index == grid.Rows.GetLastRow(DataGridViewElementStates.None))
                {
                    grid.CurrentCell = grid.Rows[0].Cells[1];
                }
                else
                {
                    grid.CurrentCell = grid.Rows[grid.CurrentRow.Index + 1].Cells[1];
                }
                file = ((FileInfo)grid.CurrentRow.Tag);
                Play();
            }
            catch (Exception)
            {

            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                wavePlayer.Stop();
            }
            catch (Exception)
            {

            }
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            try
            {
                audioFileReader.Position = 0;
            }
            catch (Exception)
            {

            }
        }

        private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            audioFileReader.Volume = wavePlayer.Volume = (vol.Value - 1) / 100f;
        }


        private void pnWaveForm_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                picWv.Width = e.X;
               
                var max = audioFileReader.Length;
                var val = (e.X * max) / pnWaveForm.Width;
                audioFileReader.Position = val;
             

            }
            catch (Exception)
            {

            }
        }

        private void picWv_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {

                //picWv.Width = e.X;
                //var max = audioFileReader.Length;
                //var val = (e.X * max) / picWv.Width;

                //audioFileReader.Position = picWv.Width;

                picWv.Width = 0;
                
            }
            catch (Exception)
            {

            }
        }
    }
}
