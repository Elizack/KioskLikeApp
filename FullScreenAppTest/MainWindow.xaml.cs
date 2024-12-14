using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FullScreenAppTest
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // true�̂Ƃ��t���X�N���[�����[�h�ɂ���B
        bool fs_flg = false;

        // �t���X�N���[���؂�ւ����O�̃E�C���h�E�ʒu�A�T�C�Y���o���Ă����p�̕ϐ�
        int prev_x = 0, prev_y = 0;
        int prev_width = 0, prev_height = 0;

        public MainWindow()
        {
            this.InitializeComponent();

            _presenter = this.AppWindow.Presenter as OverlappedPresenter;
        }
        private OverlappedPresenter? _presenter;

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            // �{�^�������ĉ�ʃ��[�h��؂�ւ�
            fs_flg = !fs_flg;
            changeScreenMode();
        }

        private void changeScreenMode()
        {
            if (_presenter is not null)
            {
                _presenter.IsAlwaysOnTop = fs_flg & false; // true�ɂ���Ə�ɑO�ʂɂȂ�
                _presenter.SetBorderAndTitleBar(hasBorder: !fs_flg, hasTitleBar: !fs_flg);
                _presenter.IsResizable = !fs_flg; // false�ɂ���Ɖ�ʃT�C�Y�ύX�s�ɂȂ�
                _presenter.IsMinimizable = fs_flg; // true�ɂ���ƃf�B�X�v���C�̒[�܂ŃE�C���h�E���ł����ł���
            }

            if (fs_flg)
            {
                myButton.Content = "�t���X�N���[�����[�h";

                // �t���X�N���[���ɂ��钼�O�̃E�C���h�E�ʒu�A�T�C�Y���o���Ă���(�ēx�{�^���������Ƃ��ɖ߂�����)
                prev_x = this.AppWindow.Position.X;
                prev_y = this.AppWindow.Position.Y;
                prev_width = this.AppWindow.ClientSize.Width;
                prev_height = this.AppWindow.ClientSize.Height;

                 // WebView2�pGrid�̃T�C�Y���v���C�}�����j�^�[�Ɏ��߂邽�߂̃p�����[�^
                double grid_width = 0, grid_height = 0;

                // �f�B�X�v���C�̍���̍��W�ƁA�T�C�Y���擾
                int topLeft_x = 0, topLeft_y = 0;
                int width = 0, height = 0;
                foreach(WindowsDisplayAPI.Display disp in WindowsDisplayAPI.Display.GetDisplays())
                {
                    // ����擾(�v���C�}�����j�^�[������ɖ����ꍇ�Ax��y�ɂ̓}�C�i�X���͂�����ۂ�)
                    topLeft_x = topLeft_x < disp.CurrentSetting.Position.X ? topLeft_x : disp.CurrentSetting.Position.X;
                    topLeft_y = topLeft_y < disp.CurrentSetting.Position.Y ? topLeft_y : disp.CurrentSetting.Position.Y;

                    // �c���Ɖ������v����
                    width += disp.CurrentSetting.Resolution.Width;
                    height += disp.CurrentSetting.Resolution.Height;

                    // �v���C�}�����j�^�[�ł���΁AWebView2�p�̐ݒ�l�ɒl�����
                    if(disp.CurrentSetting.Position.X == 0 && disp.CurrentSetting.Position.Y == 0)
                    {
                        grid_width = disp.CurrentSetting.Resolution.Width;
                        grid_height = disp.CurrentSetting.Resolution.Height;
                    }
                }

                // TextBlock�Ƀf�B�X�v���C�̍���ƉE���̍��W���o��(�m�F�p)
                textBlock1.Text = "((" + topLeft_x + "," + topLeft_y + "),(" + width + "," + height + "))";

                // �E�C���h�E�̍����(min_x,min_y)�ɁA�T�C�Y��(width,height)�ɂ���B
                this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(topLeft_x, topLeft_y, width, height));

                // WebView�̕\���͈͂��v���C�}�����j�^�[�Ɏ��߂�
                grid1.Translation = new Vector3(Math.Abs(topLeft_x), Math.Abs(topLeft_y), 0); // �E�C���h�E�̍��オ���_�B����ɑ΂��鑊�Έʒu���w�肷��B
                grid1.Width = grid_width;
                grid1.Height = grid_height;
            }
            else
            {
                myButton.Content = "�E�C���h�E���[�h";
                this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(prev_x, prev_y, prev_width, prev_height));
                grid1.Translation = new Vector3(0, 0, 0);
                grid1.Width = prev_width;
                grid1.Height = prev_height;
            }
        }
    }
}
