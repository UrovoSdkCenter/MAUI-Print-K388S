# UROVO K388S Printer SDK for .NET MAUI

UROVO K388S打印机SDK的.NET MAUI/Xamarin封装，提供扫描和打印功能支持。

## 平台支持

- Android

## 功能特性

### 扫描功能
- **多种触发方式**: 代码触发、硬件按键触发
- **输出模式**: 广播模式、文本框输出模式
- **条码类型**: 支持多种一维/二维条码
- **符号配置**: 启用/禁用特定条码类型
- **触发锁定**: 锁定/解锁扫描触发键
- **参数配置**: 整数和字符串参数配置

### 打印功能
- **基础打印**: PrinterSDK API（文本、条码、二维码、图片）
- **高级打印**: PrinterConnector API（CPCL、ESC/POS等原始指令）
- **页面设置**: 宽度、高度、旋转
- **打印控制**: 灰度、速度、对比度调节
- **纸张检测**: 标签检测、黑标检测
- **状态查询**: 实时查询打印机状态

## 环境要求

- **.NET**: 8.0+
- **MAUI/Xamarin**: 支持.NET MAUI和Xamarin.Android
- **Android SDK**: API 19+
- **依赖**: 
  - android.device.ScanManager
  - Com.Urovo.K388.PrinterSDK
  - Com.Urovo.K388.PrinterConnector

## 安装

### 1. 添加DLL引用

将`K388_PrintSDK_Lib.dll`添加到项目引用中。

### 2. ProGuard配置

如果启用ProGuard，添加以下规则：

```
-keep class android.content.** { *; }
-keep class android.os.* { *; }
-keep class android.device.* { *; }
-keep class com.urovo.* { *; }
```

## 扫描功能

### 快速开始

#### 1. 注册广播接收器

```csharp
using Android.Content;
using Android.Device;

// 定义广播接收器
private ScanBroadcastReceiver scanReceiver;

protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    
    // 注册广播
    scanReceiver = new ScanBroadcastReceiver(this);
    IntentFilter filter = new IntentFilter();
    filter.AddAction(ScanManager.ActionDecode);
    RegisterReceiver(scanReceiver, filter);
}

// 广播接收器类
class ScanBroadcastReceiver : BroadcastReceiver
{
    private MainActivity activity;
    
    public ScanBroadcastReceiver(MainActivity activity)
    {
        this.activity = activity;
    }
    
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action == ScanManager.ActionDecode)
        {
            string barcode = intent.GetStringExtra(ScanManager.BarcodeStringTag);
            int barcodeType = intent.GetIntExtra(ScanManager.BarcodeTypeTag, 0);
            
            activity.OnScanResult(barcode, barcodeType);
        }
    }
}
```

#### 2. 初始化扫描器

```csharp
using Android.Device;

private ScanManager scanManager;

private void InitScanner()
{
    scanManager = new ScanManager();
    
    // 打开扫描器
    bool isOpen = scanManager.OpenScanner();
    
    // 设置输出模式为广播模式
    // 0 - 广播模式, 1 - 文本框输出
    scanManager.SwitchOutputMode(0);
}
```

#### 3. 启动/停止扫描

```csharp
// 启动扫描
private void StartScan()
{
    if (scanManager != null)
    {
        bool started = scanManager.StartDecode();
        if (started)
        {
            Console.WriteLine("扫描启动成功");
        }
    }
}

// 停止扫描
private void StopScan()
{
    if (scanManager != null)
    {
        bool stopped = scanManager.StopDecode();
        if (stopped)
        {
            Console.WriteLine("扫描已停止");
        }
    }
}
```

#### 4. 清理资源

```csharp
protected override void OnDestroy()
{
    base.OnDestroy();
    
    if (scanManager != null)
    {
        scanManager.StopDecode();
        scanManager.CloseScanner();
    }
    
    if (scanReceiver != null)
    {
        UnregisterReceiver(scanReceiver);
    }
}
```

### ScanManager API

#### 基础控制

##### OpenScanner()
打开扫描器电源

```csharp
bool isOpen = scanManager.OpenScanner();
```

**返回值:** `true`=成功, `false`=失败

##### CloseScanner()
关闭扫描器电源

```csharp
bool isClosed = scanManager.CloseScanner();
```

**返回值:** `true`=成功, `false`=失败

##### GetScannerState()
获取扫描器电源状态

```csharp
bool isOn = scanManager.GetScannerState();
```

**返回值:** `true`=已开启, `false`=已关闭

#### 扫描控制

##### StartDecode()
启动条码解码

```csharp
bool started = scanManager.StartDecode();
```

**返回值:** `true`=成功, `false`=失败

**说明:** 扫描器开始扫描，扫描结果通过广播发送

##### StopDecode()
停止条码解码

```csharp
bool stopped = scanManager.StopDecode();
```

**返回值:** `true`=成功, `false`=失败

#### 输出模式

##### SwitchOutputMode(int mode)
设置扫描结果输出模式

```csharp
bool switched = scanManager.SwitchOutputMode(0);
```

**参数:**
- `mode`: 输出模式
  - `0`: 广播模式（推荐）
  - `1`: 文本框输出模式

**返回值:** `true`=成功, `false`=失败

##### GetOutputMode()
获取当前输出模式

```csharp
int mode = scanManager.GetOutputMode();
```

**返回值:** `0`=广播模式, `1`=文本框模式

#### 触发控制

##### SetTriggerMode(Triggering mode)
设置触发模式

```csharp
scanManager.SetTriggerMode(Triggering.Host);
```

**参数:**
- `Triggering.Host`: 主机触发（通过代码）
- `Triggering.Level`: 电平触发
- `Triggering.Pulse`: 脉冲触发

##### GetTriggerMode()
获取当前触发模式

```csharp
Triggering mode = scanManager.GetTriggerMode();
```

##### LockTrigger()
锁定扫描触发键（禁用扫描按钮）

```csharp
bool locked = scanManager.LockTrigger();
```

**返回值:** `true`=成功, `false`=失败

##### UnlockTrigger()
解锁扫描触发键（启用扫描按钮）

```csharp
bool unlocked = scanManager.UnlockTrigger();
```

**返回值:** `true`=成功, `false`=失败

##### GetTriggerLockState()
获取触发键锁定状态

```csharp
bool isLocked = scanManager.GetTriggerLockState();
```

**返回值:** `true`=已锁定, `false`=未锁定

#### 符号类型管理

##### EnableAllSymbologies(bool enable)
启用/禁用所有支持的条码类型

```csharp
scanManager.EnableAllSymbologies(true);  // 启用所有
scanManager.EnableAllSymbologies(false); // 禁用所有
```

**参数:**
- `enable`: `true`=启用, `false`=禁用

##### EnableSymbology(Symbology barcodeType, bool enable)
启用/禁用指定条码类型

```csharp
scanManager.EnableSymbology(Symbology.Code128, true);
scanManager.EnableSymbology(Symbology.QrCode, true);
```

**参数:**
- `barcodeType`: 条码类型（如Code128, QrCode等）
- `enable`: `true`=启用, `false`=禁用

##### IsSymbologyEnabled(Symbology barcodeType)
检查指定条码类型是否已启用

```csharp
bool isEnabled = scanManager.IsSymbologyEnabled(Symbology.Code128);
```

**返回值:** `true`=已启用, `false`=未启用

##### IsSymbologySupported(Symbology barcodeType)
检查设备是否支持指定条码类型

```csharp
bool isSupported = scanManager.IsSymbologySupported(Symbology.DataMatrix);
```

**返回值:** `true`=支持, `false`=不支持

#### 参数配置

##### SetParameterInts(int[] idBuffer, int[] valueBuffer)
设置整数类型参数

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = { value1, value2 };
int result = scanManager.SetParameterInts(ids, values);
```

**返回值:** `0`=成功, 其他值=失败

##### GetParameterInts(int[] idBuffer)
获取整数类型参数

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = scanManager.GetParameterInts(ids);
```

**返回值:** 参数值数组

##### SetParameterString(int[] idBuffer, string[] valueBuffer)
设置字符串类型参数

```csharp
int[] ids = { paramId1 };
string[] values = { "value1" };
bool result = scanManager.SetParameterString(ids, values);
```

**返回值:** `true`=成功, `false`=失败

##### GetParameterString(int[] idBuffer)
获取字符串类型参数

```csharp
int[] ids = { paramId1 };
string[] values = scanManager.GetParameterString(ids);
```

**返回值:** 参数值数组

##### ResetScannerParameters()
重置所有参数为出厂默认值

```csharp
bool reset = scanManager.ResetScannerParameters();
```

**返回值:** `true`=成功, `false`=失败

## 打印功能

### 方式一：PrinterSDK（推荐用于基础打印）

#### 快速开始

```csharp
using Com.Urovo.K388;

// 1. 获取PrinterSDK实例
PrinterSDK printer = PrinterSDK.GetInstance();

// 2. 设置页面尺寸 (8像素 = 1mm)
int maxWidth = 50 * 8;   // 50mm宽
int maxHeight = 30 * 8;  // 30mm高
printer.PageSetup(maxWidth, maxHeight);

// 3. 添加打印内容
int currentY = 10;

// 打印文本
printer.DrawText(0, currentY, "Label Print Test", 24, 0, 0, false, false);
currentY += 40;

// 打印条码
printer.DrawBarCode(0, currentY, "20230815143001", 1, 0, 3, 100);
currentY += 120;

// 打印二维码
printer.DrawQrCode(100, currentY, "https://www.urovo.com", 0, 4, 2);

// 4. 执行打印
printer.Print(0, new PrintListenerImpl());

// 打印监听器
class PrintListenerImpl : Java.Lang.Object, IPrintListener
{
    public void OnPrintResult(int result)
    {
        if (result == 0)
        {
            Console.WriteLine("打印成功");
        }
        else
        {
            Console.WriteLine($"打印失败: {result}");
        }
    }
}
```

#### PrinterSDK API

##### GetInstance()
获取PrinterSDK单例实例

```csharp
PrinterSDK printer = PrinterSDK.GetInstance();
```

##### PageSetup(int pageWidth, int pageHeight)
设置页面尺寸

```csharp
printer.PageSetup(400, 240);  // 宽400像素, 高240像素
```

**参数:**
- `pageWidth`: 页面宽度（像素），8像素 = 1mm
- `pageHeight`: 页面高度（像素）

##### PageSetup(int pageWidth, int pageHeight, int rotation)
设置页面尺寸（带旋转）

```csharp
printer.PageSetup(400, 240, 90);  // 旋转90度
```

**参数:**
- `rotation`: 旋转角度（0, 90, 180, 270）

#### 文本打印

##### DrawText(int x, int y, string text, int fontSize, int rotate, int bold, bool reverse, bool underline)
打印文本（简化版）

```csharp
printer.DrawText(0, 10, "Hello World", 24, 0, 0, false, false);
```

**参数:**
- `x, y`: 起始坐标（像素）
- `text`: 文本内容
- `fontSize`: 字体大小
- `rotate`: 旋转角度（0/90/180/270）
- `bold`: 加粗级别（0=不加粗, >1=加粗）
- `reverse`: 反色打印
- `underline`: 下划线

##### DrawText(int x, int y, string text, int fontType, int fontSize, int rotate, int bold, bool reverse, bool underline)
打印文本（完整版）

```csharp
printer.DrawText(0, 10, "Hello World", 24, 32, 0, 1, false, false);
```

**参数:**
- `fontType`: 字体类型
- 其他参数同简化版

##### DrawText(int x, int y, string text, string fontType, int fontSize, int rotate, int bold, bool reverse, bool underline)
打印文本（字体名称）

```csharp
printer.DrawText(0, 10, "Hello World", "Arial", 24, 0, 0, false, false);
```

**参数:**
- `fontType`: 字体名称（如"Arial", "Times"等）

##### MultLine(int height, int fontType, int fontSize, int x, int y, int rotate, params string[] strs)
多行打印

```csharp
printer.MultLine(200, 24, 24, 0, 10, 0, "Line 1", "Line 2", "Line 3");
```

**参数:**
- `height`: 多行区域总高度
- `strs`: 多行文本数组

#### 条码打印

##### DrawBarCode(int x, int y, string text, int type, int rotate, int linewidth, int height)
打印条码

```csharp
printer.DrawBarCode(0, 50, "1234567890", 1, 0, 3, 100);
```

**参数:**
- `x, y`: 起始坐标
- `text`: 条码内容
- `type`: 条码类型
  - `1`: Code128
  - `2`: Code39
  - `3`: EAN13
  - `4`: EAN8
  - 其他值参考SDK文档
- `rotate`: 旋转角度
- `linewidth`: 线宽（1-4）
- `height`: 条码高度（像素）

##### BarcodeText(int font, int size, int offset, int rotate, int width, int ratio, int height, int x, int y, string data)
条码标识符（带文本）

```csharp
printer.BarcodeText(24, 16, 0, 0, 3, 2, 100, 0, 50, "1234567890");
```

#### 二维码打印

##### DrawQrCode(int x, int y, string text, int rotate, int ver, int lel)
打印二维码

```csharp
printer.DrawQrCode(100, 100, "https://www.urovo.com", 0, 4, 2);
```

**参数:**
- `x, y`: 起始坐标
- `text`: 二维码内容
- `rotate`: 旋转角度
- `ver`: 版本（1-10，数字越大二维码越大）
- `lel`: 纠错级别
  - `1`: L级（7%纠错）
  - `2`: M级（15%纠错）
  - `3`: Q级（25%纠错）
  - `4`: H级（30%纠错）

#### 图片打印

##### DrawGraphic(int x, int y, Bitmap bmp)
打印图片

```csharp
Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.logo);
printer.DrawGraphic(0, 100, bitmap);
```

**参数:**
- `x, y`: 起始坐标
- `bmp`: Bitmap对象

#### 图形绘制

##### DrawLine(int lineWidth, int x0, int y0, int x1, int y1)
打印直线

```csharp
printer.DrawLine(2, 0, 100, 400, 100);  // 水平线
```

**参数:**
- `lineWidth`: 线宽（像素）
- `x0, y0`: 起始坐标
- `x1, y1`: 结束坐标

##### DrawBox(int lineWidth, int x0, int y0, int x1, int y1)
打印矩形框

```csharp
printer.DrawBox(2, 10, 10, 390, 230);
```

**参数:**
- `lineWidth`: 边框线宽
- `x0, y0`: 左上角坐标
- `x1, y1`: 右下角坐标

##### DrawINVERSE(int x0, int y0, int x1, int y1, int width)
打印反色线段

```csharp
printer.DrawINVERSE(0, 50, 400, 70, 20);
```

**参数:**
- `width`: 线段宽度

#### 打印控制

##### Print(int skip, IPrintListener listener)
执行打印

```csharp
printer.Print(0, new PrintListenerImpl());
```

**参数:**
- `skip`: 纸张检测方式
  - `0`: 不进纸（直接打印）
  - `1`: 标签检测
  - `2`: 左侧黑标检测
  - `3`: 右侧黑标检测
- `listener`: 打印结果监听器

##### Print(IPrintListener listener)
执行打印（默认不进纸）

```csharp
printer.Print(new PrintListenerImpl());
```

##### Speed(int level)
设置打印速度

```csharp
printer.Speed(3);
```

**参数:**
- `level`: 速度级别（0-5）
  - `0`: 最慢
  - `5`: 最快

##### ContRast(int level)
设置对比度

```csharp
printer.ContRast(2);
```

**参数:**
- `level`: 对比度级别
  - `0`: 默认
  - `1`: 中等
  - `2`: 深色
  - `3`: 非常深
  
**说明:** 对比度越高打印越深，但速度越慢

##### BackGround(int level)
设置水印灰度级别

```csharp
printer.BackGround(1);
```

**参数:**
- `level`: 灰度级别

##### SetBold(int level)
设置加粗字体

```csharp
printer.SetBold(1);  // 启用加粗
printer.SetBold(0);  // 取消加粗
```

**参数:**
- `level`: `0`=取消加粗, `>1`=启用加粗

#### 高级功能

##### Prefeed(int len)
打印前进纸

```csharp
printer.Prefeed(20);  // 打印前进纸20像素
```

##### Postfeed(int len)
打印后进纸

```csharp
printer.Postfeed(30);  // 打印后进纸30像素
```

##### SetPrintTime(int time)
设置打印份数

```csharp
printer.SetPrintTime(3);  // 打印3份
```

##### Count(int num)
自动递增/递减数字

```csharp
printer.Count(1);   // 递增
printer.Count(-1);  // 递减
```

**参数:**
- `num`: 递增/递减值（±65535）

##### PrintWait(int time)
延迟打印

```csharp
printer.PrintWait(8);  // 延迟1秒（8 * 1/8秒）
```

**参数:**
- `time`: 延迟时间（1/8秒为单位）

##### SetPace()
批量打印（按进纸按钮打印下一张）

```csharp
printer.SetPace();
```

##### AlignLeft()
左对齐

```csharp
printer.AlignLeft();
```

##### AlignCenter()
居中对齐

```csharp
printer.AlignCenter();
```

##### AlignRight()
右对齐

```csharp
printer.AlignRight();
```

##### UnderLine(bool mode)
下划线

```csharp
printer.UnderLine(true);   // 启用
printer.UnderLine(false);  // 禁用
```

##### SetMag(int w, int h)
字符放大

```csharp
printer.SetMag(2, 2);  // 宽度和高度都放大2倍
```

##### SetSP(int spacing)
字符间距

```csharp
printer.SetSP(4);  // 间距 4 * 0.125mm
```

##### GetPrintBytes()
获取当前打印数据

```csharp
byte[] printData = printer.GetPrintBytes();
```

**返回值:** 打印数据字节数组

##### Version()
获取SDK版本信息

```csharp
string version = printer.Version();
```

### 方式二：PrinterConnector（高级打印）

用于发送CPCL、ESC/POS等原始打印指令。

#### 快速开始

```csharp
using Com.Urovo.K388;
using System.Text;

// 必须在子线程执行
await Task.Run(() =>
{
    PrinterConnector connector = new PrinterConnector();
    
    try
    {
        // 1. 连接打印机
        bool isOpen = connector.Open();
        if (!isOpen)
        {
            Console.WriteLine("连接打印机失败");
            return;
        }
        
        // 2. 清空缓冲区
        connector.Flush();
        
        // 3. 检查打印机状态
        PrinterConnector.PrinterResult status = connector.Status();
        if (status != PrinterConnector.PrinterResult.Ok)
        {
            Console.WriteLine($"打印机状态异常: {status}");
            connector.Close();
            return;
        }
        
        // 4. 构建CPCL指令
        StringBuilder cpcl = new StringBuilder();
        cpcl.Append("! 0 200 200 240 1\r\n");
        cpcl.Append("CENTER\r\n");
        cpcl.Append("PAGE-WIDTH 400\r\n");
        cpcl.Append("T 24 1 0 0 Label CPCL Print Test\r\n");
        cpcl.Append("T 24 1 0 35 Barcode: 20230815143001\r\n");
        cpcl.Append("BARCODE 128 2 1 100 0 70 20230815143001\r\n");
        cpcl.Append("GAP-SENSE\r\n");
        cpcl.Append("FORM\r\n");
        cpcl.Append("PRINT\r\n");
        
        // 5. 转换为GBK编码（支持中文）
        byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(cpcl.ToString());
        
        // 6. GZIP压缩（可选）
        byte[] gzipBytes = GZIPFrame.Codec(bytes);
        
        // 7. 发送数据
        bool written = connector.Write(gzipBytes, gzipBytes.Length);
        if (written)
        {
            Console.WriteLine("CPCL指令发送成功");
        }
        
        // 8. 关闭连接
        connector.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"打印失败: {ex.Message}");
        connector.Close();
    }
});
```

#### PrinterConnector API

##### Open()
连接打印机

```csharp
bool isOpen = connector.Open();
```

**返回值:** `true`=连接成功, `false`=连接失败

**说明:** 必须在子线程中调用

##### Close()
断开打印机连接

```csharp
connector.Close();
```

##### IsConnected()
获取打印机连接状态

```csharp
bool isConnected = connector.IsConnected();
```

**返回值:** `true`=已连接, `false`=未连接

##### Flush()
清空打印响应缓冲区

```csharp
connector.Flush();
```

**说明:** 连接成功后建议先调用此方法

##### Status()
获取打印机状态

```csharp
PrinterConnector.PrinterResult status = connector.Status();
```

**返回值:**
- `PrinterResult.Ok`: 正常
- `PrinterResult.OutOfPaper`: 缺纸
- `PrinterResult.OverHeat`: 过热
- 其他状态参考SDK

##### Write(byte[] data, int len)
发送打印数据

```csharp
bool written = connector.Write(data, data.Length);
```

**参数:**
- `data`: 打印数据字节数组
- `len`: 数据长度

**返回值:** `true`=发送成功, `false`=发送失败

##### Write(string str)
发送打印数据（字符串）

```csharp
bool written = connector.Write(cpclString);
```

**参数:**
- `str`: 打印指令字符串

**返回值:** `true`=发送成功, `false`=发送失败

##### Read(byte[] data, int readLen, int timeout_ms)
读取打印机响应

```csharp
byte[] buffer = new byte[1024];
int bytesRead = connector.Read(buffer, 1024, 5000);
```

**参数:**
- `data`: 接收缓冲区
- `readLen`: 读取长度
- `timeout_ms`: 超时时间（毫秒）

**返回值:** 实际读取的字节数

## CPCL打印示例

### 基本CPCL模板

```csharp
// CPCL标签打印模板
string cpcl = "! 0 200 200 240 1\r\n"           // 初始化: 偏移0, 高度200, 数量1
            + "CENTER\r\n"                       // 居中对齐
            + "PAGE-WIDTH 400\r\n"               // 页面宽度400点
            + "T 24 1 0 0 Title Text\r\n"        // 文本: 字体24, 旋转1, 坐标(0,0)
            + "T 16 1 0 35 Content Line 1\r\n"   // 文本: 字体16
            + "BARCODE 128 2 1 100 0 70 1234567890\r\n"  // 条码: Code128
            + "FORM\r\n"                         // 结束表单
            + "PRINT\r\n";                       // 打印
```

### 完整打印流程

```csharp
private async Task PrintCPCL(string cpclCommand)
{
    await Task.Run(() =>
    {
        PrinterConnector connector = new PrinterConnector();
        
        try
        {
            // 连接
            if (!connector.Open())
            {
                ShowMessage("连接失败");
                return;
            }
            
            // 清空缓冲
            connector.Flush();
            
            // 检查状态
            var status = connector.Status();
            if (status != PrinterConnector.PrinterResult.Ok)
            {
                ShowMessage($"打印机状态: {status}");
                connector.Close();
                return;
            }
            
            // 编码转换（支持中文）
            byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(cpclCommand);
            
            // 可选：GZIP压缩
            byte[] compressedBytes = GZIPFrame.Codec(bytes);
            
            // 发送
            bool written = connector.Write(compressedBytes, compressedBytes.Length);
            
            if (written)
            {
                ShowMessage("打印成功");
            }
            else
            {
                ShowMessage("发送失败");
            }
            
            // 关闭
            connector.Close();
        }
        catch (Exception ex)
        {
            ShowMessage($"错误: {ex.Message}");
            connector.Close();
        }
    });
}
```

## 完整打印示例

```csharp
using Com.Urovo.K388;
using Android.Graphics;

public class PrintManager
{
    private PrinterSDK printer;
    
    public PrintManager()
    {
        printer = PrinterSDK.GetInstance();
    }
    
    // 打印标签示例
    public void PrintLabel(string title, string barcode, Bitmap logo)
    {
        // 1. 设置页面 (50mm x 30mm)
        int width = 50 * 8;
        int height = 30 * 8;
        printer.PageSetup(width, height);
        
        int currentY = 10;
        
        // 2. 打印标题（居中）
        printer.AlignCenter();
        printer.SetBold(1);
        printer.DrawText(0, currentY, title, 32, 0, 0, false, false);
        printer.SetBold(0);
        printer.AlignLeft();
        currentY += 50;
        
        // 3. 打印分割线
        printer.DrawLine(2, 0, currentY, width, currentY);
        currentY += 10;
        
        // 4. 打印Logo（如果有）
        if (logo != null)
        {
            printer.DrawGraphic(10, currentY, logo);
            currentY += logo.Height + 10;
        }
        
        // 5. 打印条码
        printer.DrawBarCode(50, currentY, barcode, 1, 0, 3, 80);
        currentY += 100;
        
        // 6. 打印条码文本
        printer.DrawText(70, currentY, barcode, 20, 0, 0, false, false);
        currentY += 30;
        
        // 7. 打印二维码
        printer.DrawQrCode(width - 120, currentY - 120, 
            $"https://example.com?code={barcode}", 0, 4, 2);
        
        // 8. 设置打印参数
        printer.Speed(3);        // 中等速度
        printer.ContRast(1);     // 中等对比度
        
        // 9. 执行打印
        printer.Print(1, new PrintListener());  // 1=标签检测
    }
    
    // 打印监听器
    class PrintListener : Java.Lang.Object, IPrintListener
    {
        public void OnPrintResult(int result)
        {
            switch (result)
            {
                case 0:
                    Console.WriteLine("打印成功");
                    break;
                case -1:
                    Console.WriteLine("打印失败: 缺纸");
                    break;
                case -2:
                    Console.WriteLine("打印失败: 过热");
                    break;
                case -3:
                    Console.WriteLine("打印失败: 电压不足");
                    break;
                default:
                    Console.WriteLine($"打印失败: 错误码{result}");
                    break;
            }
        }
    }
}
```

## 注意事项

### 扫描功能

1. **广播模式推荐**: 输出模式设置为0（广播模式）更稳定
2. **资源释放**: 必须在Activity销毁时注销广播接收器
3. **硬件按键**: 设备物理扫描键会自动触发扫描
4. **触发锁定**: 使用`LockTrigger()`可禁用硬件扫描键

### 打印功能

1. **坐标系统**: 原点(0,0)在左上角，8像素 = 1mm
2. **页面设置**: 必须在添加打印内容前调用`PageSetup()`
3. **Y坐标累加**: 手动维护`currentY`，避免内容重叠
4. **打印顺序**: 
   ```
   PageSetup() → Draw...() → 设置参数 → Print()
   ```

5. **PrinterConnector注意**:
   - 必须在子线程中使用
   - 连接后先调用`Flush()`清空缓冲
   - 检查`Status()`确保打印机正常
   - 使用完毕必须调用`Close()`

6. **中文支持**: 
   - PrinterSDK自动支持中文
   - PrinterConnector需要使用GBK编码

7. **CPCL指令**: 
   - 使用`\r\n`作为换行符
   - 坐标单位是点（dot）
   - 可选GZIP压缩提高传输效率

## 故障排查

### 扫描问题

**扫描器无响应**
- 检查`OpenScanner()`是否返回true
- 确认输出模式设置正确
- 验证广播接收器是否注册

**收不到扫描结果**
- 检查广播Action是否为`ScanManager.ActionDecode`
- 确认输出模式为0（广播模式）
- 查看logcat日志

### 打印问题

**打印无内容**
- 确认调用了`PageSetup()`
- 检查Y坐标是否在页面范围内
- 验证是否调用了`Print()`

**PrinterConnector连接失败**
- 确认在子线程中调用
- 检查打印机是否被其他应用占用
- 查看打印机状态

**中文乱码**
- PrinterConnector使用GBK编码
- 检查字符串编码转换

**打印内容重叠**
- 正确累加Y坐标
- 使用返回的实际高度更新currentY

## 版本要求

- **.NET**: 8.0+ 或 Xamarin
- **Android SDK**: API 19+
- **依赖库**: 
  - K388_PrintSDK_Lib.dll
  - android.device.ScanManager
  - Com.Urovo.K388.*

## 技术支持

如需技术支持,请联系UROVO技术支持团队。

## License

Copyright © UROVO Technology Co., Ltd.
