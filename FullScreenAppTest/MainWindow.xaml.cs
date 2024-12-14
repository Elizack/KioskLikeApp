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
        // trueのときフルスクリーンモードにする。
        bool fs_flg = false;

        // フルスクリーン切り替え直前のウインドウ位置、サイズを覚えておく用の変数
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
            // ボタン押して画面モードを切り替え
            fs_flg = !fs_flg;
            changeScreenMode();
        }

        private void changeScreenMode()
        {
            if (_presenter is not null)
            {
                _presenter.IsAlwaysOnTop = fs_flg & false; // trueにすると常に前面になる
                _presenter.SetBorderAndTitleBar(hasBorder: !fs_flg, hasTitleBar: !fs_flg);
                _presenter.IsResizable = !fs_flg; // falseにすると画面サイズ変更不可になる
                _presenter.IsMinimizable = fs_flg; // trueにするとディスプレイの端までウインドウをでかくできる
            }

            if (fs_flg)
            {
                myButton.Content = "フルスクリーンモード";

                // フルスクリーンにする直前のウインドウ位置、サイズを覚えておく(再度ボタン押したときに戻すため)
                prev_x = this.AppWindow.Position.X;
                prev_y = this.AppWindow.Position.Y;
                prev_width = this.AppWindow.ClientSize.Width;
                prev_height = this.AppWindow.ClientSize.Height;

                 // WebView2用Gridのサイズをプライマリモニターに収めるためのパラメータ
                double grid_width = 0, grid_height = 0;

                // ディスプレイの左上の座標と、サイズを取得
                int topLeft_x = 0, topLeft_y = 0;
                int width = 0, height = 0;
                foreach(WindowsDisplayAPI.Display disp in WindowsDisplayAPI.Display.GetDisplays())
                {
                    // 左上取得(プライマリモニターが左上に無い場合、xやyにはマイナスがはいるっぽい)
                    topLeft_x = topLeft_x < disp.CurrentSetting.Position.X ? topLeft_x : disp.CurrentSetting.Position.X;
                    topLeft_y = topLeft_y < disp.CurrentSetting.Position.Y ? topLeft_y : disp.CurrentSetting.Position.Y;

                    // 縦幅と横幅合計する
                    width += disp.CurrentSetting.Resolution.Width;
                    height += disp.CurrentSetting.Resolution.Height;

                    // プライマリモニターであれば、WebView2用の設定値に値入れる
                    if(disp.CurrentSetting.Position.X == 0 && disp.CurrentSetting.Position.Y == 0)
                    {
                        grid_width = disp.CurrentSetting.Resolution.Width;
                        grid_height = disp.CurrentSetting.Resolution.Height;
                    }
                }

                // TextBlockにディスプレイの左上と右下の座標を出力(確認用)
                textBlock1.Text = "((" + topLeft_x + "," + topLeft_y + "),(" + width + "," + height + "))";

                // ウインドウの左上を(min_x,min_y)に、サイズを(width,height)にする。
                this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(topLeft_x, topLeft_y, width, height));

                // WebViewの表示範囲をプライマリモニターに収める
                grid1.Translation = new Vector3(Math.Abs(topLeft_x), Math.Abs(topLeft_y), 0); // ウインドウの左上が原点。それに対する相対位置を指定する。
                grid1.Width = grid_width;
                grid1.Height = grid_height;
            }
            else
            {
                myButton.Content = "ウインドウモード";
                this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(prev_x, prev_y, prev_width, prev_height));
                grid1.Translation = new Vector3(0, 0, 0);
                grid1.Width = prev_width;
                grid1.Height = prev_height;
            }
        }
    }
}
