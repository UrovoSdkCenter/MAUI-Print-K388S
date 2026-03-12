using System;
using Xamarin.Forms;
using Android.Content;
using System.Text;
using Android.Device;
using Android.Util;
using Android.Graphics;
using Android;
using Com.Urovo.Printer;
using Android.Runtime;
using Android.Widget;
using System.Threading.Tasks;

namespace K388S_SDK
{
    public partial class MainPage : ContentPage
    {
        ScanManager scanManager;
        string _action = ScanManager.ActionDecode;
#if __ANDROID__
        // 广播接收器实例
        ScannerBroadcastReceiver _broadcastReceiver;
#endif
        public MainPage()
        {
            InitializeComponent();
            CheckSymbols();
            InitScanner();
        }
        private void CheckSymbols()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("编译器符号检查:");

#if __ANDROID__
            sb.AppendLine("__ANDROID__ 已定义");
#else
            sb.AppendLine("__ANDROID__ 未定义");
#endif

#if DEBUG
            sb.AppendLine("DEBUG 已定义");
#endif

#if RELEASE
            sb.AppendLine("RELEASE 已定义");
#endif

            InfoTv.Text = sb.ToString();
        }
        private void InitScanner()
        {
            scanManager = new ScanManager();
#if __ANDROID__
            scanManager.OpenScanner(); // 上电
            scanManager.SwitchOutputMode(0); // 0-广播模式,1-文本框输出
            /*int[] index = new int[] { 9 };//GOOD_READ_BEEP_ENABLE
            int[] value = new int[] { 1 };//0 : None,1 : Short, 2 : Sharp
            int ret = scanManager.SetParameterInts(index, value);*/
#endif

        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            InfoTv.Text = "开始扫码";
#if __ANDROID__
            // 软件按钮触发扫码使用StartDecode()，硬件按钮仅需监听广播即可获取扫码结果
            InfoTv.Text = "正在扫码";
            scanManager.StartDecode();
#endif
        }

        public void RegisterBroadcastReceiver(string action)
        {
            _action = action;
#if __ANDROID__
            if (_broadcastReceiver != null)
            {
                UnregisterBroadcastReceiver();
            }

            // 创建广播接收器
            _broadcastReceiver = new ScannerBroadcastReceiver();
            _broadcastReceiver.BroadcastReceived += OnBroadcastReceived;

            // 注册广播接收器
            IntentFilter filter = new IntentFilter(action);
            Android.App.Application.Context.RegisterReceiver(_broadcastReceiver, filter);
            Log.Debug("test", "RegisterBroadcastReceiver========");
#endif
        }

        public void UnregisterBroadcastReceiver()
        {
#if __ANDROID__
            if (_broadcastReceiver != null)
            {
                Log.Debug("test", "UnregisterBroadcastReceiver========");
                Android.App.Application.Context.UnregisterReceiver(_broadcastReceiver);
                _broadcastReceiver.BroadcastReceived -= OnBroadcastReceived;
                _broadcastReceiver = null;
            }
#endif
        }

        private void OnBroadcastReceived(object sender, string result)
        {
            // 更新UI需要在主线程执行
            Device.BeginInvokeOnMainThread(() =>
            {
#if __ANDROID__
                Log.Debug("test", $"Broadcast received: {result}");
                InfoTv.Text = $"扫描结果: {result}";
#endif
            });
        }

#if __ANDROID__
        // 广播接收器类
        private class ScannerBroadcastReceiver : BroadcastReceiver
        {
            public event EventHandler<string> BroadcastReceived;

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == ScanManager.ActionDecode)
                {
                    byte[] bytes = intent.GetByteArrayExtra("barcode");
                    if (bytes != null && bytes.Length > 0)
                    {
                        string str = Encoding.UTF8.GetString(bytes);
                        BroadcastReceived?.Invoke(this, str);
                    }
                }
            }
        }
#endif

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // 
            RegisterBroadcastReceiver(_action);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnregisterBroadcastReceiver();
        }

        private void PrintTicketButton_Clicked(object sender, EventArgs e)
        {
            InfoTv.Text = "Print Ticket...";
#if __ANDROID__
            PrinterSDK printer = PrinterSDK.Instance;
            int maxWidth = 48 * 8;
            printer.PageSetup(maxWidth, 110 * 8);

            int margin = 0;
            int contentWidth = maxWidth - margin * 2;
            int currentY = 20;
            int fontSize = 1;
            int fontType = 24;
            int lineHeight = 10;
            int fontHeight = 25;

            printer.AlignCenter();
            printer.DrawText(0, currentY, "测试超市", fontType, fontSize, 0, 1, false, false);
            currentY += fontHeight + lineHeight;

            Bitmap bitmap = CreateBitmap(GetDrawableId("vip"), 100);
            if (bitmap != null)
            {
                Log.Debug("Test", $"bitmap size:{bitmap.Width} x {bitmap.Height}");
                printer.DrawGraphic(0, currentY, bitmap);
                currentY += bitmap.Height + lineHeight;
            }

            printer.DrawLine(1, 0, currentY, contentWidth, currentY);
            currentY += fontHeight + lineHeight;

            printer.AlignLeft();
            printer.DrawText(margin, currentY, "收银员：张三", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(margin, currentY, "交易号：20250815001", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(margin, currentY, "时间：2025-08-15 14:30", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;

            printer.UnderLine(true);
            printer.DrawText(margin, currentY, "商品  数量  单价  小计", fontType, fontSize, 0, 0, false, false);
            printer.UnderLine(false);
            currentY += fontHeight + lineHeight;

            printer.DrawText(margin, currentY, "矿泉水550ml  2  1.50  3.00", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(margin, currentY, "薯片(大包装)  1  12.80  12.80", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(margin, currentY, "鲜牛奶1L  1  18.50  18.50", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;

            printer.DrawLine(1, 0, currentY, contentWidth, currentY);
            currentY += lineHeight;

            printer.AlignRight();
            printer.DrawText(contentWidth, currentY, "合计：34.30", fontType, fontSize, 0, 1, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(contentWidth, currentY, "优惠：-5.00", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(contentWidth, currentY, "实收：29.30", fontType, fontSize, 0, 1, false, true);
            currentY += fontHeight + lineHeight;

            printer.AlignCenter();
            printer.DrawText(0, currentY, "支付方式：支付宝", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(0, currentY, "交易流水：20230815143001", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;

            printer.DrawBarCode(0, currentY, "20230815143001", 1, 0, 3, 120);
            currentY += 140 + lineHeight;

            printer.DrawText(0, currentY, "谢谢惠顾！欢迎再次光临", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;

            printer.Postfeed(20);
            MPrinterListener listener = new MPrinterListener(InfoTv);
            printer.Print(0, listener);
#else 
            InfoTv.Text = "Not support";
#endif
        }
#if __ANDROID__
        private int GetDrawableId(string resourceName)
        {
            return Android.App.Application.Context.Resources?.GetIdentifier(
                resourceName,
                "drawable",
                Android.App.Application.Context.PackageName) ?? 0;
        }

        private Bitmap CreateBitmap(int resourceId, int targetWidth)
        {
            if (resourceId == 0) { return null; }
            Android.Content.Res.Resources res = Android.App.Application.Context.Resources;
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };
            BitmapFactory.DecodeResource(res, resourceId, options);
            int originalWidth = options.OutWidth;
            int originalHeight = options.OutHeight;

            if (originalWidth <= targetWidth)
            {
                return BitmapFactory.DecodeResource(res, resourceId);
            }

            float scaleRatio = (float)targetWidth / originalWidth;
            int targetHeight = (int)(originalHeight * scaleRatio);
            options.InJustDecodeBounds = false;
            Bitmap originalBitmap = BitmapFactory.DecodeResource(res, resourceId, options);
            return Bitmap.CreateScaledBitmap(originalBitmap, targetWidth, targetHeight, true);
        }
#endif
        public class MPrinterListener : PrintListener
        {
            // 使用弱引用避免内存泄漏
            private readonly WeakReference<Label> _textViewReference;

            public MPrinterListener(Label textView)
            {
                _textViewReference = new WeakReference<Label>(textView);
            }

            public override void PrintError(PrinterConnector.PrinterResult result)
            {
                UpdateTextView("PrintError:" + result);
            }

            public override void PrintSuccess()
            {
                UpdateTextView("PrintSuccess");
            }

            private void UpdateTextView(string text)
            {
                // 获取TextView，如果已经被回收则忽略
                if (!_textViewReference.TryGetTarget(out Label tv))
                    return;

                tv.Text = text;
            }
        }

        private void PrintLabelButton_Clicked(object sender, EventArgs e)
        {
            InfoTv.Text = "Print Label...";
#if __ANDROID__
            PrinterSDK printer = PrinterSDK.Instance;
            int maxWidth = 50 * 8;
            printer.PageSetup(maxWidth, 30 * 8);
            int currentY = 0;
            int fontSize = 1;
            int fontType = 24; // 中文标识
            int lineHeight = 10;
            int fontHeight = 25;

            printer.DrawText(0, currentY, "标签打印测试", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawText(0, currentY, "条码：20230815143001", fontType, fontSize, 0, 0, false, false);
            currentY += fontHeight + lineHeight;
            printer.DrawBarCode(0, currentY, "20230815143001", 1, 0, 3, 100);
            MPrinterListener listener = new MPrinterListener(InfoTv);
            printer.Print(1, listener);

#else 
            InfoTv.Text = "Not support";
#endif
        }
        private void PrintCustomCMD_Clicked(object sender, EventArgs e)
        {
            InfoTv.Text = "PrintCustomCMD...";
#if __ANDROID__
            string cpcl = "! 0 200 200 240 1\r\n" +
              "CENTER\r\n" +
              "PAGE-WIDTH 400\r\n" +
              "T 24 1 0 0 标签CPCL打印测试\r\n" +
              "T 24 1 0 35 条码：20230815143001\r\n" +
              "BARCODE 128 2 1 100 0 70 20230815143001\r\n" +
              "GAP-SENSE\r\n" +
              "FORM\r\n" +
              "PRINT\r\n";

            byte[] bytes  = GetBytesInGBK(cpcl);//中文字符需进行GBK编码

            Task.Run(() =>
            {
                var printerConnector = new PrinterConnector();
                bool open = printerConnector.Open();
                if (open)
                {
                    printerConnector.Flush();
                    PrinterConnector.PrinterResult status = printerConnector.Status();
                    if (status == PrinterConnector.PrinterResult.PrinterOk)
                    {
                        byte[] gzipBytes = GZIPFrame.Codec(bytes);
                        bool write = printerConnector.Write(gzipBytes, gzipBytes.Length);
                        printerConnector.Flush();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            InfoTv.Text = write ? "打印完成" : "打印异常";
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            InfoTv.Text = $"打印机异常: {status}";
                        });
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        InfoTv.Text = $"打印机连接失败";
                    });
                }
                printerConnector.Close();
            });
#else 
            InfoTv.Text = "Not support";
#endif
        }

        public static byte[] GetBytesInGBK(string input)
        {
            var gbkCharset = Java.Nio.Charset.Charset.ForName("GBK");
            var javaString = new Java.Lang.String(input);
            return javaString.GetBytes(gbkCharset);
        }
    }
}